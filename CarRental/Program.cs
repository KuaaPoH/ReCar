using CarRental.Models;
using CarRental.Services;
using CarRental.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
// SQL Server (commented for MySQL)
// using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DbRenalCarContext>(optios =>
{
    // SQL Server Configuration (commented for MySQL)
    // optios.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    
    // MySQL Configuration
    optios.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    );
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
    AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        //đăng nhập rồi nhưng chưa có quyền thì sẽ chuyển tới địa chỉ này
        options.AccessDeniedPath = "/AccessDenied";
    });

builder.Services.AddHttpClient<IZaloPayService, ZaloPayService>();
builder.Services.AddScoped<CarRental.Services.INotificationService, CarRental.Services.NotificationService>();
builder.Services.AddHostedService<PendingOrderExpiryService>();
builder.Services.AddSignalR();
var app = builder.Build();

// Tự động tạo database và các bảng nếu chưa tồn tại (chỉ trong Development)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            var context = services.GetRequiredService<DbRenalCarContext>();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogError("Connection string is null or empty");
                return;
            }
            
            // Extract database name từ connection string
            var databaseName = ExtractDatabaseName(connectionString);
            
            if (!string.IsNullOrEmpty(databaseName))
            {
                // Tạo connection string không có database để tạo database
                var serverConnectionString = connectionString
                    .Replace($"Database={databaseName};", "")
                    .Replace($"Database={databaseName}", "");
                
                // Tạo database nếu chưa tồn tại
                using (var tempContext = new DbRenalCarContext(new DbContextOptionsBuilder<DbRenalCarContext>()
                    .UseMySql(serverConnectionString, new MySqlServerVersion(new Version(8, 0, 21)))
                    .Options))
                {
                    try
                    {
                        tempContext.Database.ExecuteSqlRaw($"CREATE DATABASE IF NOT EXISTS `{databaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;");
                    }
                    catch (Exception dbEx)
                    {
                        // Database có thể đã tồn tại hoặc lỗi khác, log và tiếp tục
                        logger.LogWarning(dbEx, "Could not create database, it may already exist");
                    }
                }
            }
            
            // Tạo các bảng trong database
            logger.LogInformation("Checking if database exists and creating tables...");
            
            try
            {
                // Kiểm tra xem database có tồn tại không
                if (!context.Database.CanConnect())
                {
                    logger.LogWarning("Cannot connect to database. Attempting to create database...");
                }
                
                // Tạo các bảng nếu chưa tồn tại
                var created = context.Database.EnsureCreated();
                if (created)
                {
                    logger.LogInformation("Database and tables created successfully.");
                }
                else
                {
                    logger.LogInformation("Database already exists. Checking if all tables exist...");
                }
                
                // Kiểm tra và tạo các bảng còn thiếu
                EnsureAllTablesExist(context, logger);
                
                // Kiểm tra và insert dữ liệu nếu database trống
                try
                {
                    var hasData = context.Customers.Any() || context.Roles.Any() || context.CarTypes.Any();
                    if (!hasData)
                    {
                        logger.LogInformation("Database is empty. Attempting to seed initial data...");
                        SeedDatabase(context, logger);
                        logger.LogInformation("Initial data seeded successfully.");
                    }
                    else
                    {
                        logger.LogInformation("Database already contains data. Skipping seed.");
                    }
                }
                catch (Exception seedEx)
                {
                    logger.LogWarning(seedEx, "Could not seed database: {Message}. You may need to run DbRenalCar_MySQL.sql manually.", seedEx.Message);
                }
            }
            catch (Exception ensureEx)
            {
                logger.LogError(ensureEx, "Error in EnsureCreated: {Message}", ensureEx.Message);
                // Thử lại một lần nữa
                try
                {
                    logger.LogInformation("Retrying table creation...");
                    context.Database.EnsureCreated();
                    logger.LogInformation("Tables created on retry.");
                }
                catch (Exception retryEx)
                {
                    logger.LogError(retryEx, "Failed to create tables on retry: {Message}", retryEx.Message);
                    throw;
                }
            }   
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred creating the DB: {Message}", ex.Message);
            // Không throw để ứng dụng vẫn có thể chạy, nhưng log lỗi
        }
    }
}

// Method để đảm bảo tất cả các bảng đều tồn tại
static void EnsureAllTablesExist(DbRenalCarContext context, ILogger logger)
{
    try
    {
        var tablesToCheck = new Dictionary<string, string>
        {
            { "CarReviews", @"CREATE TABLE IF NOT EXISTS `CarReviews` (
                `CarReviewID` INT NOT NULL AUTO_INCREMENT,
                `Name` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `Phone` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `Email` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `CreatedDate` DATETIME NULL,
                `Detail` VARCHAR(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `CarID` INT NULL,
                `Star` INT NULL,
                `IsActive` TINYINT(1) NULL,
                PRIMARY KEY (`CarReviewID`),
                INDEX `FK_CarReviews_Cars` (`CarID`),
                CONSTRAINT `FK_CarReviews_Cars` FOREIGN KEY (`CarID`) REFERENCES `Cars` (`CarID`) ON DELETE RESTRICT ON UPDATE RESTRICT
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;" },
            
            { "OrderDetails", @"CREATE TABLE IF NOT EXISTS `OrderDetails` (
                `OrderDetailID` INT NOT NULL AUTO_INCREMENT,
                `OrderID` INT NOT NULL,
                `CarID` INT NOT NULL,
                `Description` LONGTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `Price` DECIMAL(18, 2) NULL,
                `Quantity` INT NULL,
                `PickupDate` DATETIME NULL,
                `ReturnDate` DATETIME NULL,
                PRIMARY KEY (`OrderDetailID`),
                INDEX `FK_OrderDetails_Order` (`OrderID`),
                CONSTRAINT `FK_OrderDetails_Order` FOREIGN KEY (`OrderID`) REFERENCES `CarRentalOrders` (`OrderID`) ON DELETE RESTRICT ON UPDATE RESTRICT
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;" },
            
            { "carowner_registrations", @"CREATE TABLE IF NOT EXISTS `carowner_registrations` (
                `Id` INT NOT NULL AUTO_INCREMENT,
                `CustomerId` INT NOT NULL,
                `FullName` VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `Phone` VARCHAR(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `Email` VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `Address` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `IdCard` VARCHAR(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `IdCardIssueDate` DATETIME NULL,
                `IdCardIssuePlace` VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `IdCardFrontImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `IdCardBackImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `LicenseNumber` VARCHAR(25) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `LicenseClass` VARCHAR(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT 'B2',
                `LicenseExpiryDate` DATETIME NULL,
                `LicenseImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `BankName` VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `AccountNumber` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `AccountHolder` VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `Status` VARCHAR(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Pending',
                `RejectReason` LONGTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                `ApprovedAt` DATETIME NULL,
                `ApprovedBy` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                PRIMARY KEY (`Id`),
                INDEX `idx_reg_customer` (`CustomerId`),
                INDEX `idx_reg_status` (`Status`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;" },

            { "banks", @"CREATE TABLE IF NOT EXISTS `banks` (
                `BankId` INT NOT NULL AUTO_INCREMENT,
                `BankCode` VARCHAR(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `BankName` VARCHAR(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `ShortName` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `Bin` VARCHAR(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `Logo` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `DisplayOrder` INT NOT NULL DEFAULT 0,
                `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
                PRIMARY KEY (`BankId`),
                INDEX `idx_bank_code` (`BankCode`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;" },

            { "amenities", @"CREATE TABLE IF NOT EXISTS `amenities` (
                `AmenityId` INT NOT NULL AUTO_INCREMENT,
                `Name` VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `Icon` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `IconColor` VARCHAR(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `DisplayOrder` INT NOT NULL DEFAULT 0,
                `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
                PRIMARY KEY (`AmenityId`),
                INDEX `idx_amenity_order` (`DisplayOrder`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;" },

            { "car_amenities", @"CREATE TABLE IF NOT EXISTS `car_amenities` (
                `CarId` INT NOT NULL,
                `AmenityId` INT NOT NULL,
                PRIMARY KEY (`CarId`, `AmenityId`),
                INDEX `FK_CarAmenities_Amenities` (`AmenityId`),
                CONSTRAINT `FK_CarAmenities_Cars` FOREIGN KEY (`CarId`) REFERENCES `Cars` (`CarID`) ON DELETE CASCADE ON UPDATE CASCADE,
                CONSTRAINT `FK_CarAmenities_Amenities` FOREIGN KEY (`AmenityId`) REFERENCES `amenities` (`AmenityId`) ON DELETE CASCADE ON UPDATE CASCADE
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;" }
        };
        
        // Biên bản bàn giao xe điện tử (Digital Check-in / Check-out)
        tablesToCheck["vehicle_inspections"] = @"CREATE TABLE IF NOT EXISTS `vehicle_inspections` (
                `InspectionId` INT NOT NULL AUTO_INCREMENT,
                `OrderId` INT NOT NULL,
                `CarId` INT NOT NULL,
                `InspectionType` VARCHAR(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                `Odometer` INT NOT NULL DEFAULT 0,
                `FuelLevel` INT NOT NULL DEFAULT 0,
                `FrontImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `RearImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `LeftImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `RightImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `Notes` VARCHAR(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                `CreatedBy` INT NULL,
                PRIMARY KEY (`InspectionId`),
                UNIQUE INDEX `UX_Inspection_Order_Type` (`OrderId`, `InspectionType`),
                INDEX `IX_Inspection_Car` (`CarId`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

        // Lịch chặn xe bận / bảo dưỡng của Chủ xe
        tablesToCheck["car_calendar_blocks"] = @"CREATE TABLE IF NOT EXISTS `car_calendar_blocks` (
                `BlockId` INT NOT NULL AUTO_INCREMENT,
                `CarId` INT NOT NULL,
                `StartTime` DATETIME NOT NULL,
                `EndTime` DATETIME NOT NULL,
                `Kind` VARCHAR(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'personal',
                `Note` VARCHAR(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (`BlockId`),
                INDEX `IX_CalendarBlock_Car` (`CarId`, `StartTime`, `EndTime`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

        // Cấu hình sàn & Thông số tài chính (System Settings)
        tablesToCheck["system_settings"] = @"CREATE TABLE IF NOT EXISTS `system_settings` (
                `SettingId` INT NOT NULL AUTO_INCREMENT,
                `DepositPct` INT NOT NULL DEFAULT 30,
                `CommissionPct` INT NOT NULL DEFAULT 10,
                `DefaultKmPerDay` INT NOT NULL DEFAULT 350,
                `ExtraKmPrice` DECIMAL(18, 2) NOT NULL DEFAULT 3000.00,
                `Hotline` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '1900 8888',
                `SupportEmail` VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'hotro@recar.vn',
                `PlatformName` VARCHAR(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'ReCar — Car-Sharing Platform',
                `Address` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Tòa tháp Keangnam Landmark 72, Mễ Trì, Nam Từ Liêm, Hà Nội',
                `UpdatedAt` DATETIME NULL,
                PRIMARY KEY (`SettingId`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

        foreach (var table in tablesToCheck)
        {
            try
            {
                // Kiểm tra xem bảng có tồn tại không bằng cách thử query
                try
                {
                    // Dùng ExecuteSqlRaw với tên bảng nội bộ để tránh EF Core biến tên bảng thành parameter @p0
                    context.Database.ExecuteSqlRaw(table.Value);
                    // Nếu không có lỗi, bảng đã tồn tại
                }
                catch
                {
                    // Bảng không tồn tại, tạo mới
                    logger.LogInformation($"Creating missing table: {table.Key}");
                    context.Database.ExecuteSqlRaw(table.Value);
                    logger.LogInformation($"Table {table.Key} created successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Error checking/creating table {table.Key}: {ex.Message}");
            }
        }

        // Mở rộng cột Notes của bảng CarRentalOrders lên TEXT và đảm bảo bảng vehicle_inspections tồn tại
        try { context.Database.ExecuteSqlRaw("ALTER TABLE `carrentalorders` MODIFY COLUMN `Notes` TEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;"); } catch { }

        // Snapshot Pattern: Bổ sung an toàn 2 cột tỷ lệ phí sàn và tiền phí sàn cho carrentalorders
        try
        {
            var conn = context.Database.GetDbConnection();
            bool needClose = false;
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
                needClose = true;
            }

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'carrentalorders' AND COLUMN_NAME IN ('PlatformFeeRate', 'PlatformFee');";
                var existingCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingCols.Add(reader.GetString(0));
                    }
                }

                if (!existingCols.Contains("PlatformFeeRate"))
                {
                    using var alterCmd = conn.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE `carrentalorders` ADD COLUMN `PlatformFeeRate` DECIMAL(5, 2) NULL DEFAULT 10.00;";
                    alterCmd.ExecuteNonQuery();
                }

                if (!existingCols.Contains("PlatformFee"))
                {
                    using var alterCmd = conn.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE `carrentalorders` ADD COLUMN `PlatformFee` DECIMAL(18, 2) NULL;";
                    alterCmd.ExecuteNonQuery();
                }

                using var updateCmd = conn.CreateCommand();
                updateCmd.CommandText = "UPDATE `carrentalorders` SET `PlatformFeeRate` = 10.00 WHERE `PlatformFeeRate` IS NULL;";
                updateCmd.ExecuteNonQuery();

                using var updateFeeCmd = conn.CreateCommand();
                updateFeeCmd.CommandText = "UPDATE `carrentalorders` SET `PlatformFee` = ROUND(((COALESCE(`Deposit`, 0) + COALESCE(`Payment`, 0)) * COALESCE(`PlatformFeeRate`, 10.00)) / 100, 0) WHERE `PlatformFee` IS NULL;";
                updateFeeCmd.ExecuteNonQuery();
            }
            finally
            {
                if (needClose && conn.State == System.Data.ConnectionState.Open) conn.Close();
            }
        }
        catch (Exception colEx)
        {
            logger.LogWarning("Kiểm tra/cập nhật schema Snapshot carrentalorders: {msg}", colEx.Message);
        }

        // Bổ sung an toàn các cột Views, IsFeatured, Badge cho bảng news và seed tin tức chuẩn
        try
        {
            var conn = context.Database.GetDbConnection();
            bool needClose = false;
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
                needClose = true;
            }

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'news' AND COLUMN_NAME IN ('Views', 'IsFeatured', 'Badge');";
                var existingCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingCols.Add(reader.GetString(0));
                    }
                }

                if (!existingCols.Contains("Views"))
                {
                    using var alterCmd = conn.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE `news` ADD COLUMN `Views` INT NOT NULL DEFAULT 0;";
                    alterCmd.ExecuteNonQuery();
                }

                if (!existingCols.Contains("IsFeatured"))
                {
                    using var alterCmd = conn.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE `news` ADD COLUMN `IsFeatured` TINYINT(1) NOT NULL DEFAULT 0;";
                    alterCmd.ExecuteNonQuery();
                }

                if (!existingCols.Contains("Badge"))
                {
                    using var alterCmd = conn.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE `news` ADD COLUMN `Badge` VARCHAR(50) NULL;";
                    alterCmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (needClose && conn.State == System.Data.ConnectionState.Open) conn.Close();
            }

            // Seed các bài tin tức mẫu vào database nếu chưa có để đảm bảo cả Admin và Khách hàng đồng bộ 5 bài
            try
            {
                var curatedList = CarRental.Controllers.NewsController.GetCuratedNews();
                foreach (var c in curatedList)
                {
                    if (!context.News.Any(n => n.Alias == c.Alias || n.Title == c.Title))
                    {
                        var newNews = new News
                        {
                            Title = c.Title,
                            Alias = c.Alias,
                            Description = c.Description,
                            Detail = c.Detail,
                            Image = c.Image,
                            CreatedBy = c.CreatedBy,
                            CreatedDate = c.CreatedDate,
                            SeoKeywords = c.SeoKeywords,
                            IsActive = true
                        };

                        if (c.Alias.Contains("da-lat")) { newNews.Views = 1520; newNews.IsFeatured = true; newNews.Badge = "Nổi bật"; }
                        else if (c.Alias.Contains("vinfast")) { newNews.Views = 2340; newNews.IsFeatured = true; newNews.Badge = "Xu hướng"; }
                        else { newNews.Views = 860; newNews.IsFeatured = false; newNews.Badge = "Mới nhất"; }

                        context.News.Add(newNews);
                    }
                }

                // Cập nhật số lượt xem và nhãn mẫu cho các bài có sẵn nếu Views còn bằng 0 hoặc có emoji
                var existingNewsList = context.News.ToList();
                foreach (var item in existingNewsList)
                {
                    if (item.Views == null || item.Views == 0)
                    {
                        item.Views = item.Title.Contains("Porsche") ? 3120 : (item.Title.Contains("Đà Lạt") ? 1520 : 680);
                    }
                    if (!string.IsNullOrEmpty(item.Badge))
                    {
                        item.Badge = item.Badge.Replace("🔥 ", "").Replace("⭐ ", "").Replace("⚡ ", "").Trim();
                    }
                }

                // Cập nhật schema cho bảng Blog: Views, IsFeatured, Badge
                try
                {
                    var blogConn = context.Database.GetDbConnection();
                    bool blogNeedClose = false;
                    if (blogConn.State != System.Data.ConnectionState.Open)
                    {
                        blogConn.Open();
                        blogNeedClose = true;
                    }

                    var blogCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (var cmd = blogConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME IN ('Blog', 'blogs', 'blog');";
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            blogCols.Add(reader.GetString(0));
                        }
                    }

                    if (!blogCols.Contains("Views"))
                    {
                        using var alterCmd = blogConn.CreateCommand();
                        alterCmd.CommandText = "ALTER TABLE `Blog` ADD COLUMN `Views` INT NOT NULL DEFAULT 0;";
                        alterCmd.ExecuteNonQuery();
                    }

                    if (!blogCols.Contains("IsFeatured"))
                    {
                        using var alterCmd = blogConn.CreateCommand();
                        alterCmd.CommandText = "ALTER TABLE `Blog` ADD COLUMN `IsFeatured` TINYINT(1) NOT NULL DEFAULT 0;";
                        alterCmd.ExecuteNonQuery();
                    }

                    if (!blogCols.Contains("Badge"))
                    {
                        using var alterCmd = blogConn.CreateCommand();
                        alterCmd.CommandText = "ALTER TABLE `Blog` ADD COLUMN `Badge` VARCHAR(50) NULL;";
                        alterCmd.ExecuteNonQuery();
                    }

                    if (blogNeedClose && blogConn.State == System.Data.ConnectionState.Open) blogConn.Close();

                    // Seed mẫu lượt xem cho các bài Blog nếu Views còn 0
                    var existingBlogList = context.Blogs.ToList();
                    foreach (var b in existingBlogList)
                    {
                        if (b.Views == null || b.Views == 0)
                        {
                            b.Views = b.Title.Contains("Đà Lạt") ? 2890 : (b.Title.Contains("giấy tờ") ? 1750 : 920);
                        }
                        if (!string.IsNullOrEmpty(b.Badge))
                        {
                            b.Badge = b.Badge.Replace("🔥 ", "").Replace("⭐ ", "").Replace("⚡ ", "").Trim();
                        }
                    }
                }
                catch (Exception blogSchemaEx)
                {
                    logger.LogWarning("Kiểm tra/cập nhật schema Blog: {msg}", blogSchemaEx.Message);
                }

                context.SaveChanges();
            }
            catch (Exception seedNewsEx)
            {
                logger.LogWarning(seedNewsEx, "Lỗi seed bài viết tin tức: {msg}", seedNewsEx.Message);
            }
        }
        catch (Exception newsEx)
        {
            logger.LogWarning("Kiểm tra/cập nhật schema news: {msg}", newsEx.Message);
        }

        // Tự động bổ sung cột Latitude, Longitude cho bảng cars và đồng bộ 34 tỉnh/thành mới của Việt Nam
        try
        {
            var carConn = context.Database.GetDbConnection();
            bool carNeedClose = carConn.State != System.Data.ConnectionState.Open;
            if (carNeedClose) carConn.Open();
            try
            {
                var carCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = carConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'cars' AND COLUMN_NAME IN ('Latitude', 'Longitude');";
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read()) carCols.Add(reader.GetString(0));
                }

                if (!carCols.Contains("Latitude"))
                {
                    using var alterCmd = carConn.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE `cars` ADD COLUMN `Latitude` DOUBLE NULL;";
                    alterCmd.ExecuteNonQuery();
                }

                if (!carCols.Contains("Longitude"))
                {
                    using var alterCmd = carConn.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE `cars` ADD COLUMN `Longitude` DOUBLE NULL;";
                    alterCmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (carNeedClose && carConn.State == System.Data.ConnectionState.Open) carConn.Close();
            }

            // Cập nhật địa điểm cũ sang đơn vị hành chính mới của Việt Nam (34 tỉnh/thành phố từ 07/2025)
            var existingLocs = context.Locations.ToList();
            var renameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Vũng Tàu (Bà Rịa)"] = "Vũng Tàu (TP. Hồ Chí Minh)",
                ["Huế (Thừa Thiên Huế)"] = "Thành phố Huế",
                ["Quy Nhơn (Bình Định)"] = "Quy Nhơn (Gia Lai)",
                ["Phan Thiết (Bình Thuận)"] = "Phan Thiết (Lâm Đồng)",
                ["Phú Quốc (Kiên Giang)"] = "Phú Quốc (An Giang)"
            };
            bool locChanged = false;
            foreach (var loc in existingLocs)
            {
                if (!string.IsNullOrEmpty(loc.CityName) && renameMap.TryGetValue(loc.CityName.Trim(), out var newName))
                {
                    loc.CityName = newName;
                    locChanged = true;
                }
            }

            int nextOrder = existingLocs.Any() ? existingLocs.Max(l => l.DisplayOrder) + 1 : 1;
            foreach (var prov in CarRental.Utilities.VietnamLocations.Provinces)
            {
                bool exists = existingLocs.Any(l =>
                    l.CityName.Equals(prov.Name, StringComparison.OrdinalIgnoreCase) ||
                    l.CityName.Contains(prov.Name, StringComparison.OrdinalIgnoreCase));
                if (!exists)
                {
                    context.Locations.Add(new CarRental.Models.Location
                    {
                        CityName = prov.Name,
                        Region = prov.Region,
                        IsPopular = prov.IsPopular,
                        DisplayOrder = nextOrder++,
                        IsActive = prov.IsPopular
                    });
                    locChanged = true;
                }
            }
            if (locChanged) context.SaveChanges();
        }
        catch (Exception carLocEx)
        {
            logger.LogWarning("Kiểm tra/cập nhật tọa độ xe & địa điểm VN mới: {msg}", carLocEx.Message);
        }

        // Tự động tạo bảng partner_payouts để liên kết đối soát giữa Admin và Ví Chủ xe
        try
        {
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS `partner_payouts` (
                    `PayoutId` INT NOT NULL AUTO_INCREMENT,
                    `SettlementCode` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                    `OwnerId` INT NOT NULL,
                    `CycleKey` VARCHAR(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `Period` VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `Amount` DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
                    `Status` VARCHAR(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'paid',
                    `PaidAt` DATETIME NOT NULL,
                    `ConfirmedBy` VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    PRIMARY KEY (`PayoutId`),
                    UNIQUE INDEX `UX_partner_payouts_code` (`SettlementCode`),
                    INDEX `IX_partner_payouts_owner` (`OwnerId`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;");
        }
        catch (Exception payoutEx)
        {
            logger.LogWarning("Kiểm tra/tạo bảng partner_payouts: {msg}", payoutEx.Message);
        }

        // Tự động bổ sung cột Balance cho customers và tạo bảng wallet_transactions cho Ví người dùng
        try
        {
            var connWallet = context.Database.GetDbConnection();
            if (connWallet.State != System.Data.ConnectionState.Open) connWallet.Open();
            using (var cmd = connWallet.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'customers' AND COLUMN_NAME = 'Balance';";
                var colCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (colCount == 0)
                {
                    context.Database.ExecuteSqlRaw("ALTER TABLE `customers` ADD COLUMN `Balance` DECIMAL(18, 2) NOT NULL DEFAULT 0.00;");
                }

                cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'customers' AND COLUMN_NAME = 'LicenseBackImage';";
                var backColCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (backColCount == 0)
                {
                    context.Database.ExecuteSqlRaw("ALTER TABLE `customers` ADD COLUMN `LicenseBackImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;");
                }

                cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'carowner_registrations' AND COLUMN_NAME = 'LicenseBackImage';";
                var regBackColCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (regBackColCount == 0)
                {
                    context.Database.ExecuteSqlRaw("ALTER TABLE `carowner_registrations` ADD COLUMN `LicenseBackImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;");
                }
            }

            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS `wallet_transactions` (
                    `TransactionId` INT NOT NULL AUTO_INCREMENT,
                    `CustomerId` INT NOT NULL,
                    `OrderId` INT NULL,
                    `Type` VARCHAR(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                    `Amount` DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
                    `BalanceAfter` DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
                    `Description` VARCHAR(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
                    `BankName` VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `BankAccount` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `AccountHolder` VARCHAR(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `Status` VARCHAR(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'completed',
                    `CreatedAt` DATETIME NOT NULL,
                    `ProcessedAt` DATETIME NULL,
                    PRIMARY KEY (`TransactionId`),
                    INDEX `IX_wallet_transactions_customer` (`CustomerId`),
                    INDEX `IX_wallet_transactions_order` (`OrderId`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;");
        }
        catch (Exception walletEx)
        {
            logger.LogWarning("Kiểm tra/tạo bảng wallet_transactions: {msg}", walletEx.Message);
        }

        // Tự động Seed cấu hình hệ thống mặc định nếu bảng system_settings chưa có dữ liệu
        try
        {
            if (!context.SystemSettings.Any())
            {
                logger.LogInformation("Seeding default system settings...");
                context.SystemSettings.Add(new SystemSetting
                {
                    DepositPct = 30,
                    CommissionPct = 10,
                    DefaultKmPerDay = 350,
                    ExtraKmPrice = 3000m,
                    Hotline = "1900 8888",
                    SupportEmail = "hotro@recar.vn",
                    PlatformName = "ReCar — Car-Sharing Platform",
                    Address = "Tòa tháp Keangnam Landmark 72, Mễ Trì, Nam Từ Liêm, Hà Nội",
                    UpdatedAt = null
                });
                context.SaveChanges();
                logger.LogInformation("System settings seeded successfully.");
            }
        }
        catch (Exception stEx)
        {
            logger.LogWarning(stEx, "Could not seed system settings: {Message}", stEx.Message);
        }
        // Tự động Seed danh sách ngân hàng nếu bảng banks chưa có dữ liệu
        try
        {
            if (!context.Banks.Any())
            {
                logger.LogInformation("Seeding default Vietnam recipient banks...");
                context.Banks.AddRange(new[]
                {
                    new Bank { BankCode = "VCB", ShortName = "Vietcombank", BankName = "Ngân hàng TMCP Ngoại thương Việt Nam", Bin = "970436", DisplayOrder = 1, IsActive = true },
                    new Bank { BankCode = "MB", ShortName = "MB Bank", BankName = "Ngân hàng TMCP Quân đội", Bin = "970422", DisplayOrder = 2, IsActive = true },
                    new Bank { BankCode = "TCB", ShortName = "Techcombank", BankName = "Ngân hàng TMCP Kỹ thương Việt Nam", Bin = "970407", DisplayOrder = 3, IsActive = true },
                    new Bank { BankCode = "BIDV", ShortName = "BIDV", BankName = "Ngân hàng TMCP Đầu tư và Phát triển Việt Nam", Bin = "970418", DisplayOrder = 4, IsActive = true },
                    new Bank { BankCode = "CTG", ShortName = "VietinBank", BankName = "Ngân hàng TMCP Công thương Việt Nam", Bin = "970415", DisplayOrder = 5, IsActive = true },
                    new Bank { BankCode = "VPB", ShortName = "VPBank", BankName = "Ngân hàng TMCP Việt Nam Thịnh Vượng", Bin = "970432", DisplayOrder = 6, IsActive = true },
                    new Bank { BankCode = "ACB", ShortName = "ACB", BankName = "Ngân hàng TMCP Á Châu", Bin = "970416", DisplayOrder = 7, IsActive = true },
                    new Bank { BankCode = "TPB", ShortName = "TPBank", BankName = "Ngân hàng TMCP Tiên Phong", Bin = "970423", DisplayOrder = 8, IsActive = true },
                    new Bank { BankCode = "STB", ShortName = "Sacombank", BankName = "Ngân hàng TMCP Sài Gòn Thương Tín", Bin = "970403", DisplayOrder = 9, IsActive = true },
                    new Bank { BankCode = "HDB", ShortName = "HDBank", BankName = "Ngân hàng TMCP Phát triển TP.HCM", Bin = "970437", DisplayOrder = 10, IsActive = true },
                    new Bank { BankCode = "VBA", ShortName = "Agribank", BankName = "Ngân hàng Nông nghiệp & PTNT Việt Nam", Bin = "970405", DisplayOrder = 11, IsActive = true },
                    new Bank { BankCode = "VIB", ShortName = "VIB", BankName = "Ngân hàng TMCP Quốc tế Việt Nam", Bin = "970441", DisplayOrder = 12, IsActive = true },
                    new Bank { BankCode = "SHB", ShortName = "SHB", BankName = "Ngân hàng TMCP Sài Gòn - Hà Nội", Bin = "970443", DisplayOrder = 13, IsActive = true },
                    new Bank { BankCode = "MSB", ShortName = "MSB", BankName = "Ngân hàng TMCP Hàng Hải Việt Nam", Bin = "970426", DisplayOrder = 14, IsActive = true },
                    new Bank { BankCode = "OCB", ShortName = "OCB", BankName = "Ngân hàng TMCP Phương Đông", Bin = "970448", DisplayOrder = 15, IsActive = true },
                    new Bank { BankCode = "SSB", ShortName = "SeABank", BankName = "Ngân hàng TMCP Đông Nam Á", Bin = "970440", DisplayOrder = 16, IsActive = true },
                    new Bank { BankCode = "LPB", ShortName = "LPBank", BankName = "Ngân hàng TMCP Lộc Phát Việt Nam", Bin = "970449", DisplayOrder = 17, IsActive = true },
                    new Bank { BankCode = "NAB", ShortName = "Nam A Bank", BankName = "Ngân hàng TMCP Nam Á", Bin = "970428", DisplayOrder = 18, IsActive = true },
                    new Bank { BankCode = "BVB", ShortName = "BVBank", BankName = "Ngân hàng TMCP Bản Việt", Bin = "970454", DisplayOrder = 19, IsActive = true },
                    new Bank { BankCode = "BAB", ShortName = "Bac A Bank", BankName = "Ngân hàng TMCP Bắc Á", Bin = "970409", DisplayOrder = 20, IsActive = true }
                });
                context.SaveChanges();
                logger.LogInformation("Banks seeded successfully.");
            }
        }
        catch (Exception bankEx)
        {
            logger.LogWarning(bankEx, "Could not seed banks: {Message}", bankEx.Message);
        }

        // Tự động Seed danh sách 14 tiện nghi xe mặc định nếu bảng amenities chưa có dữ liệu
        try
        {
            if (!context.Amenities.Any())
            {
                logger.LogInformation("Seeding default vehicle amenities...");
                context.Amenities.AddRange(new[]
                {
                    new Amenity { Name = "Bản đồ dẫn đường GPS", Icon = "bi-geo-alt-fill", IconColor = "text-danger", DisplayOrder = 1, IsActive = true },
                    new Amenity { Name = "Camera lùi / Camera 360", Icon = "bi-camera-video-fill", IconColor = "text-primary", DisplayOrder = 2, IsActive = true },
                    new Amenity { Name = "Camera hành trình", Icon = "bi-record-circle-fill", IconColor = "text-danger", DisplayOrder = 3, IsActive = true },
                    new Amenity { Name = "Cảm biến áp suất lốp", Icon = "bi-speedometer2", IconColor = "text-warning", DisplayOrder = 4, IsActive = true },
                    new Amenity { Name = "Apple CarPlay & Android Auto", Icon = "bi-phone-fill", IconColor = "text-info", DisplayOrder = 5, IsActive = true },
                    new Amenity { Name = "Cửa sổ trời Panorama", Icon = "bi-brightness-high-fill", IconColor = "text-warning", DisplayOrder = 6, IsActive = true },
                    new Amenity { Name = "Thu phí tự động ETC", Icon = "bi-credit-card-2-front-fill", IconColor = "text-success", DisplayOrder = 7, IsActive = true },
                    new Amenity { Name = "Ghế da cao cấp chỉnh điện", Icon = "bi-award-fill", IconColor = "text-primary", DisplayOrder = 8, IsActive = true },
                    new Amenity { Name = "Ghế trẻ em ISOFIX", Icon = "bi-person-heart", IconColor = "text-danger", DisplayOrder = 9, IsActive = true },
                    new Amenity { Name = "Sạc không dây Qi", Icon = "bi-lightning-charge-fill", IconColor = "text-warning", DisplayOrder = 10, IsActive = true },
                    new Amenity { Name = "Cảm biến va chạm trước/sau", Icon = "bi-shield-check", IconColor = "text-success", DisplayOrder = 11, IsActive = true },
                    new Amenity { Name = "Cảnh báo điểm mù BSM", Icon = "bi-eye-fill", IconColor = "text-secondary", DisplayOrder = 12, IsActive = true },
                    new Amenity { Name = "Khóa thông minh SmartKey", Icon = "bi-key-fill", IconColor = "text-primary", DisplayOrder = 13, IsActive = true },
                    new Amenity { Name = "Cốp điện thông minh", Icon = "bi-box-seam-fill", IconColor = "text-secondary", DisplayOrder = 14, IsActive = true }
                });
                context.SaveChanges();
                logger.LogInformation("Amenities seeded successfully.");
            }

            // Đảm bảo trạng thái OrderStatus = 5 (Hoàn tất) luôn tồn tại trong CSDL
            try
            {
                if (!context.OrderStatuses.Any(s => s.StatusId == 5))
                {
                    context.OrderStatuses.Add(new OrderStatus { StatusId = 5, StatusDescription = "Hoàn tất" });
                    context.SaveChanges();
                    logger.LogInformation("OrderStatus 5 (Hoàn tất) ensured successfully.");
                }
            }
            catch (Exception osEx)
            {
                logger.LogWarning(osEx, "Could not ensure OrderStatus 5: {Message}", osEx.Message);
            }

            // Đảm bảo Menu "Quản lý hãng xe" luôn tồn tại trong phân hệ Admin
            try
            {
                var brandMenuExists = context.Menus.Any(m => m.Subsystem == "Admin" && (m.Url == "/Admin/CarBrands" || m.Alias == "Admin/CarBrands"));
                if (!brandMenuExists)
                {
                    var carGroup = context.Menus.FirstOrDefault(m => m.Subsystem == "Admin" && (m.ParentId == null || m.ParentId == 0) && (m.Title.Contains("XE") || m.Title.Contains("ĐƠN HÀNG")));
                    int? parentId = carGroup?.MenuId;

                    var carTypeMenu = context.Menus.FirstOrDefault(m => m.Subsystem == "Admin" && (m.Url == "/Admin/CarTypes" || m.Alias == "Admin/CarTypes"));
                    int pos = carTypeMenu != null ? (carTypeMenu.Position ?? 4) + 1 : 5;

                    context.Menus.Add(new Menu
                    {
                        Title = "Quản lý hãng xe",
                        Alias = "Admin/CarBrands",
                        Url = "/Admin/CarBrands",
                        Icon = "bi-patch-check",
                        Subsystem = "Admin",
                        ParentId = parentId,
                        Levels = parentId.HasValue ? 2 : 1,
                        Position = pos,
                        IsActive = true
                    });
                    context.SaveChanges();
                    logger.LogInformation("Menu 'Quản lý hãng xe' seeded successfully into Admin subsystem.");
                }
            }
            catch (Exception menuEx)
            {
                logger.LogWarning(menuEx, "Could not seed CarBrands menu: {Message}", menuEx.Message);
            }

            // Đảm bảo Menu "Quản lý tin tức" và chuẩn hóa "Quản lý cẩm nang" luôn tồn tại trong phân hệ Admin
            try
            {
                var blogMenu = context.Menus.FirstOrDefault(m => m.Subsystem == "Admin" && (m.Url == "/Admin/Blogs" || m.Alias == "Admin/Blogs" || m.Title.Contains("Bài viết")));
                if (blogMenu != null)
                {
                    blogMenu.Title = "Quản lý cẩm nang";
                    blogMenu.Icon = "bi-journal-bookmark";
                    blogMenu.Url = "/Admin/Blogs";
                }

                var newsMenuExists = context.Menus.Any(m => m.Subsystem == "Admin" && (m.Url == "/Admin/News" || m.Alias == "Admin/News"));
                if (!newsMenuExists)
                {
                    int? parentId = blogMenu?.ParentId;
                    int pos = blogMenu != null ? (blogMenu.Position ?? 6) + 1 : 7;

                    context.Menus.Add(new Menu
                    {
                        Title = "Quản lý tin tức",
                        Alias = "Admin/News",
                        Url = "/Admin/News",
                        Icon = "bi-newspaper",
                        Subsystem = "Admin",
                        ParentId = parentId,
                        Levels = parentId.HasValue ? 2 : 1,
                        Position = pos,
                        IsActive = true
                    });
                    logger.LogInformation("Menu 'Quản lý tin tức' seeded successfully into Admin subsystem.");
                }
                context.SaveChanges();
            }
            catch (Exception newsMenuEx)
            {
                logger.LogWarning(newsMenuEx, "Could not seed News menu: {Message}", newsMenuEx.Message);
            }
        }
        catch (Exception amenEx)
        {
            logger.LogWarning(amenEx, "Could not seed amenities: {Message}", amenEx.Message);
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Error checking/creating missing tables: {Message}", ex.Message);
    }
}

// Helper method để extract database name từ connection string
static string ExtractDatabaseName(string connectionString)
{
    if (string.IsNullOrEmpty(connectionString)) return string.Empty;
    
    var parts = connectionString.Split(';');
    foreach (var part in parts)
    {
        var trimmed = part.Trim();
        if (trimmed.StartsWith("Database=", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed.Substring(9).Trim();
        }
    }
    return string.Empty;
}

// Method để seed dữ liệu ban đầu vào database
static void SeedDatabase(DbRenalCarContext context, ILogger logger)
{
    try
    {
        // Insert Role data trước (vì Account phụ thuộc vào Role)
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(new[]
            {
                new Role { RoleId = 1, RoleName = "Admin", Description = "quanli" },
                new Role { RoleId = 2, RoleName = "guest", Description = "khách" },
                new Role { RoleId = 3, RoleName = "Chủ xe", Description = "chủ xe cung cấp xe cho hệ thống" }
            });
            context.SaveChanges();
            logger.LogInformation("Roles seeded.");
        }

        // Insert OrderStatus data
        if (!context.OrderStatuses.Any())
        {
            context.OrderStatuses.AddRange(new[]
            {
                new OrderStatus { StatusId = 1, StatusDescription = " Đã đặt" },
                new OrderStatus { StatusId = 2, StatusDescription = "Đã Huỷ" },
                new OrderStatus { StatusId = 3, StatusDescription = "Đã thanh toán" },
                new OrderStatus { StatusId = 4, StatusDescription = "Đang giao" },
                new OrderStatus { StatusId = 5, StatusDescription = "Hoàn tất" }
            });
            context.SaveChanges();
            logger.LogInformation("OrderStatuses seeded.");
        }
        else if (!context.OrderStatuses.Any(s => s.StatusId == 5))
        {
            context.OrderStatuses.Add(new OrderStatus { StatusId = 5, StatusDescription = "Hoàn tất" });
            context.SaveChanges();
            logger.LogInformation("OrderStatus 5 (Hoàn tất) seeded.");
        }

        // Insert CarTypes data
        if (!context.CarTypes.Any())
        {
            context.CarTypes.AddRange(new[]
            {
                new CarType { TypeId = 1, CarTypeName = "Mini Class Vehicles", Seats = 4, Manufacturer = "pinlk", Image = "A.jpg" },
                new CarType { TypeId = 2, CarTypeName = "Small Class Vehicles", Image = "B.jpg" },
                new CarType { TypeId = 3, CarTypeName = "Sub-Medium Class Vehicles", Image = "C.jpg" },
                new CarType { TypeId = 4, CarTypeName = "Top-Middle Class Vehicles", Image = "D.jpg" },
                new CarType { TypeId = 5, CarTypeName = "Upper Class Vehicles", Image = "E.jpg" },
                new CarType { TypeId = 6, CarTypeName = "Luxury Class Vehicles", Image = "F.jpg" },
                new CarType { TypeId = 7, CarTypeName = "Multi Purpose Cars", Image = "M.jpg" },
                new CarType { TypeId = 8, CarTypeName = "Crossover Utility Vehicle ", Image = "J.jpg" },
                new CarType { TypeId = 9, CarTypeName = "Super Luxury Vehicle", Image = "S.jpg" }
            });
            context.SaveChanges();
            logger.LogInformation("CarTypes seeded.");
        }

        // Bảng Account đã được loại bỏ, dữ liệu quản lý duy nhất tại Customers.

        // Insert Menu data
        if (!context.Menus.Any())
        {
            context.Menus.AddRange(new[]
            {
                new Menu { MenuId = 1, Title = "Trang chủ", Alias = "home", Levels = 1, Position = 1, IsActive = true },
                new Menu { MenuId = 2, Title = "Sản phẩm", Alias = "product", Levels = 1, Position = 2, IsActive = true },
                new Menu { MenuId = 3, Title = "Blog", Alias = "blog", Levels = 1, Position = 3, IsActive = true },
                new Menu { MenuId = 4, Title = "Contact", Alias = "contact", Levels = 1, Position = 4, IsActive = true },
                new Menu { MenuId = 6, Title = "Cart", Alias = "cart", Levels = 1, Position = 5, IsActive = true },
                new Menu { MenuId = 14, Title = "New", Alias = "news", Description = "tin tức", Levels = 1, ParentId = 0, Position = 7, IsActive = true }
            });
            context.SaveChanges();
            logger.LogInformation("Menus seeded.");
        }

        logger.LogInformation("Basic seed data inserted successfully. For full data, please run DbRenalCar_MySQL.sql manually.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error seeding database: {Message}", ex.Message);
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Tắt HTTPS redirection để demo với ngrok
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseMiddleware<CarRental.Middleware.SessionSyncMiddleware>();
app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<CarRental.Hubs.ReCarHub>("/recarHub");

app.Run();
