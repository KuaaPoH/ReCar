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

builder.Services.AddSingleton<IVnpayServices,VnPayService>();
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
                    var hasData = context.Accounts.Any() || context.Roles.Any() || context.CarTypes.Any();
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
            
            { "ContractDetails", @"CREATE TABLE IF NOT EXISTS `ContractDetails` (
                `ContractDetailID` INT NOT NULL AUTO_INCREMENT,
                `ContractID` INT NOT NULL,
                `CarTypeID` INT NOT NULL,
                `Quantity` INT NULL,
                `Price` DECIMAL(18, 2) NULL,
                `ReceiveDate` DATE NULL,
                `ReturnDate` DATE NULL,
                `Notes` LONGTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                PRIMARY KEY (`ContractDetailID`),
                INDEX `FK_ContractDetails_Contract` (`ContractID`),
                INDEX `FK_ContractDetails_CarType` (`CarTypeID`),
                CONSTRAINT `FK_ContractDetails_Contract` FOREIGN KEY (`ContractID`) REFERENCES `Contract` (`ContractID`) ON DELETE RESTRICT ON UPDATE RESTRICT,
                CONSTRAINT `FK_ContractDetails_CarType` FOREIGN KEY (`CarTypeID`) REFERENCES `CarTypes` (`TypeID`) ON DELETE RESTRICT ON UPDATE RESTRICT
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;" },
            
            { "ContractSettlements", @"CREATE TABLE IF NOT EXISTS `ContractSettlements` (
                `SettlementID` INT NOT NULL AUTO_INCREMENT,
                `ContractID` INT NOT NULL,
                `Date` DATE NULL,
                `PaymentMethod` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `TotalCarsRented` INT NULL,
                `AdvancePayment` DECIMAL(18, 2) NULL,
                `TotalPayment` DECIMAL(18, 2) NULL,
                `PaidAmount` DECIMAL(18, 2) NULL,
                `Notes` LONGTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                PRIMARY KEY (`SettlementID`),
                INDEX `FK_ContractSettlements_Contract` (`ContractID`),
                CONSTRAINT `FK_ContractSettlements_Contract` FOREIGN KEY (`ContractID`) REFERENCES `Contract` (`ContractID`) ON DELETE RESTRICT ON UPDATE RESTRICT
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;" },
            
            { "ContractSettlementDetails", @"CREATE TABLE IF NOT EXISTS `ContractSettlementDetails` (
                `SettlementDetailID` INT NOT NULL AUTO_INCREMENT,
                `SettlementID` INT NOT NULL,
                `CarID` INT NOT NULL,
                `ReceiveDate` DATE NULL,
                `ReturnDate` DATE NULL,
                `Price` DECIMAL(18, 2) NULL,
                `Total` DECIMAL(18, 2) NULL,
                PRIMARY KEY (`SettlementDetailID`),
                INDEX `FK_ContractSettlementDetails_Settlement` (`SettlementID`),
                CONSTRAINT `FK_ContractSettlementDetails_Settlement` FOREIGN KEY (`SettlementID`) REFERENCES `ContractSettlements` (`SettlementID`) ON DELETE RESTRICT ON UPDATE RESTRICT
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
            
            { "Users", @"CREATE TABLE IF NOT EXISTS `Users` (
                `UserID` INT NOT NULL AUTO_INCREMENT,
                `UserName` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `Email` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                `Password` VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                PRIMARY KEY (`UserID`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;" }
        };
        
        foreach (var table in tablesToCheck)
        {
            try
            {
                // Kiểm tra xem bảng có tồn tại không bằng cách thử query
                try
                {
                    // Dùng ExecuteSqlRaw với tên bảng nội bộ để tránh EF Core biến tên bảng thành parameter @p0
                    context.Database.ExecuteSqlRaw($"SELECT 1 FROM `{table.Key}` LIMIT 1;");
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
                new OrderStatus { StatusId = 4, StatusDescription = "Đang giao" }
            });
            context.SaveChanges();
            logger.LogInformation("OrderStatuses seeded.");
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

        // Insert Account data
        if (!context.Accounts.Any())
        {
            context.Accounts.AddRange(new[]
            {
                new Account { AccountId = 2, Username = "quan", Password = "124", FullName = "nguyenhongquan", Phone = "2424242", Email = "sdfdfdfdsfd", RoleId = 1, IsActive = true },
                new Account { AccountId = 3, Username = "thanh thuy", Password = "d0afd43f169f68af6d56cf454d74b167", Phone = "123456789", Email = "thanh@gmail.com" },
                new Account { AccountId = 4, Username = "quan", Password = "d0afd43f169f68af6d56cf454d74b167", Email = "quan@gmail.com" }
            });
            context.SaveChanges();
            logger.LogInformation("Accounts seeded.");
        }

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




app.Run();
