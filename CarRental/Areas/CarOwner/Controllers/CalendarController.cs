using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.CarOwner.Controllers
{
    [Area("CarOwner")]
    public class CalendarController : Controller
    {
        private readonly DbRenalCarContext _context;

        public CalendarController(DbRenalCarContext context)
        {
            _context = context;
        }

        private void EnsureCalendarTableAndSeed(List<Car> ownerCars)
        {
            try
            {
                _context.Database.ExecuteSqlRaw(@"CREATE TABLE IF NOT EXISTS `car_calendar_blocks` (
                    `BlockId` INT NOT NULL AUTO_INCREMENT,
                    `CarId` INT NOT NULL,
                    `StartTime` DATETIME NOT NULL,
                    `EndTime` DATETIME NOT NULL,
                    `Kind` VARCHAR(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'personal',
                    `Note` VARCHAR(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (`BlockId`),
                    INDEX `IX_CalendarBlock_Car` (`CarId`, `StartTime`, `EndTime`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;");
            }
            catch
            {
                // Ignore if already exists
            }
        }

        private Customer? GetCurrentOwner()
        {
            var accountId = Function.GetAccountId(HttpContext.Session);
            var carOwnerId = Function.GetCarOwnerAccountId(HttpContext.Session);
            var userEmail = Function.GetEmail(HttpContext.Session);

            var customer = _context.Customers
                .Include(a => a.Role)
                .FirstOrDefault(a => (carOwnerId > 0 && a.CustomerId == carOwnerId) ||
                                     (accountId > 0 && a.CustomerId == accountId) ||
                                     (!string.IsNullOrEmpty(userEmail) && a.Email == userEmail && a.RoleId == 3));

            if (customer == null)
            {
                customer = _context.Customers
                    .Include(a => a.Role)
                    .FirstOrDefault(a => a.CustomerId == 1022 && a.RoleId == 3)
                    ?? _context.Customers.Include(a => a.Role).FirstOrDefault(a => a.RoleId == 3);
            }

            return customer;
        }

        public IActionResult Index(string? month, string? week, string mode = "month", int? carId = null, string status = "all")
        {
            if (!Function.IsCarOwnerLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var ownerCars = _context.Cars
                .Where(c => c.OwnerId == owner.CustomerId)
                .OrderBy(c => c.CarName)
                .ToList();

            EnsureCalendarTableAndSeed(ownerCars);

            var ownerCarIds = ownerCars.Select(c => c.CarId).ToList();

            // Xác định tháng hiện tại đang xem (yyyy-MM)
            DateTime currentMonthDate = DateTime.Today;
            if (!string.IsNullOrEmpty(month) && DateTime.TryParse($"{month}-01", out var parsedMonth))
            {
                currentMonthDate = new DateTime(parsedMonth.Year, parsedMonth.Month, 1);
            }
            else
            {
                currentMonthDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }

            // Xác định ngày đầu tuần (Thứ 2) cho chế độ xem Tuần
            DateTime currentWeekStart = GetMonday(DateTime.Today);
            if (!string.IsNullOrEmpty(week) && DateTime.TryParse(week, out var parsedWeek))
            {
                currentWeekStart = GetMonday(parsedWeek);
            }

            // Truy vấn đơn thuê xe của chủ xe (bỏ qua đơn đã hủy StatusId == 2)
            var ordersRaw = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .Where(o => o.StatusId != 2 && o.OrderDetails.Any(od => ownerCarIds.Contains(od.CarId)))
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var calendarOrders = new List<CalendarOrderVm>();
            foreach (var ord in ordersRaw)
            {
                foreach (var od in ord.OrderDetails.Where(d => ownerCarIds.Contains(d.CarId)))
                {
                    if (carId.HasValue && od.CarId != carId.Value)
                        continue;

                    DateTime pDate = od.PickupDate ?? (ord.OrderDate.HasValue ? ord.OrderDate.Value.ToDateTime(new TimeOnly(8, 0)) : DateTime.Today.AddHours(8));
                    DateTime rDate = od.ReturnDate ?? (ord.ReturnDate.HasValue ? ord.ReturnDate.Value.ToDateTime(new TimeOnly(20, 0)) : pDate.AddDays(1));
                    if (rDate <= pDate)
                    {
                        rDate = pDate.AddHours(12);
                    }

                    decimal total = WalletController.CalculateOrderTotal(ord);
                    decimal ownerNet = Math.Round(total * 0.90m, 0);

                    calendarOrders.Add(new CalendarOrderVm
                    {
                        OrderId = ord.OrderId,
                        Code = $"#RC-{ord.OrderId:D4}",
                        CarId = od.CarId,
                        CarName = od.Car?.CarName ?? "Xe tự lái",
                        LicensePlate = od.Car?.LicensePlate ?? "",
                        CustomerName = ord.Customer?.FullName ?? ord.Customer?.Name ?? "Khách thuê",
                        CustomerPhone = ord.Customer?.PhoneNumber ?? "",
                        PickupAt = pDate,
                        ReturnAt = rDate,
                        StatusId = ord.StatusId ?? 1,
                        StatusLabel = GetOrderStatusLabel(ord.StatusId ?? 1),
                        TotalAmount = total,
                        OwnerNet = ownerNet
                    });
                }
            }

            var blocksQuery = _context.CarCalendarBlocks
                .Include(b => b.Car)
                .Where(b => ownerCarIds.Contains(b.CarId));

            if (carId.HasValue)
            {
                blocksQuery = blocksQuery.Where(b => b.CarId == carId.Value);
            }

            var blocks = blocksQuery
                .OrderBy(b => b.StartTime)
                .ToList();

            // Tạo 42 ô ngày cho lưới lịch tháng (6 tuần bắt đầu từ Thứ 2)
            DateTime firstDayOfMonth = new DateTime(currentMonthDate.Year, currentMonthDate.Month, 1);
            DateTime gridStart = GetMonday(firstDayOfMonth);

            var visibleCars = carId.HasValue ? ownerCars.Where(c => c.CarId == carId.Value).ToList() : ownerCars;
            decimal? minVisiblePrice = visibleCars.Any() ? visibleCars.Min(c => c.Price ?? 0m) : null;

            var daysGrid = new List<CalendarDayCellVm>();
            for (int i = 0; i < 42; i++)
            {
                DateTime dayDate = gridStart.AddDays(i).Date;
                DateTime dayStart = dayDate;
                DateTime dayEnd = dayDate.AddDays(1).AddSeconds(-1);

                var dayOrders = calendarOrders
                    .Where(o => o.PickupAt <= dayEnd && o.ReturnAt >= dayStart)
                    .ToList();

                var dayBlocks = blocks
                    .Where(b => b.StartTime <= dayEnd && b.EndTime >= dayStart)
                    .ToList();

                int busyCarCount = visibleCars.Count(c =>
                    dayOrders.Any(o => o.CarId == c.CarId && (o.StatusId == 1 || o.StatusId == 3 || o.StatusId == 4)) ||
                    dayBlocks.Any(b => b.CarId == c.CarId));

                int freeCarCount = Math.Max(0, visibleCars.Count - busyCarCount);

                daysGrid.Add(new CalendarDayCellVm
                {
                    Date = dayDate,
                    InCurrentMonth = dayDate.Month == currentMonthDate.Month,
                    IsToday = dayDate == DateTime.Today,
                    Orders = dayOrders,
                    Blocks = dayBlocks,
                    FreeCarCount = freeCarCount
                });
            }

            var weekDays = new List<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                weekDays.Add(currentWeekStart.AddDays(i).Date);
            }

            var vm = new CalendarIndexVm
            {
                MonthKey = currentMonthDate.ToString("yyyy-MM"),
                MonthLabel = $"Tháng {currentMonthDate:MM/yyyy}",
                PrevMonthKey = currentMonthDate.AddMonths(-1).ToString("yyyy-MM"),
                NextMonthKey = currentMonthDate.AddMonths(1).ToString("yyyy-MM"),
                WeekKey = currentWeekStart.ToString("yyyy-MM-dd"),
                WeekLabel = $"{currentWeekStart:dd/MM} – {currentWeekStart.AddDays(6):dd/MM/yyyy}",
                PrevWeekKey = currentWeekStart.AddDays(-7).ToString("yyyy-MM-dd"),
                NextWeekKey = currentWeekStart.AddDays(7).ToString("yyyy-MM-dd"),
                Mode = mode,
                SelectedCarId = carId,
                StatusFilter = status,
                OwnerCars = ownerCars,
                VisibleCars = visibleCars,
                MinVisiblePrice = minVisiblePrice,
                DaysGrid = daysGrid,
                WeekDays = weekDays,
                AllOrders = calendarOrders,
                AllBlocks = blocks
            };

            ViewData["Title"] = "Lịch xe";
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBlock(int carId, DateTime startTime, DateTime endTime, string kind = "personal", string? note = null, string? returnMonth = null, string? returnMode = "month", int? filterCarId = null)
        {
            if (!Function.IsCarOwnerLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId && c.OwnerId == owner.CustomerId);
            if (car == null)
            {
                TempData["CalError"] = "Không tìm thấy phương tiện hợp lệ thuộc quyền quản lý của bạn.";
                return RedirectToAction(nameof(Index), new { month = returnMonth, mode = returnMode, carId = filterCarId });
            }

            if (endTime <= startTime)
            {
                TempData["CalError"] = "Thời gian kết thúc phải sau thời gian bắt đầu.";
                return RedirectToAction(nameof(Index), new { month = returnMonth, mode = returnMode, carId = filterCarId });
            }

            // Kiểm tra xung đột với các đơn đặt xe đang hiệu lực (StatusId 1, 3, 4)
            var activeOrders = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .Where(o => (o.StatusId == 1 || o.StatusId == 3 || o.StatusId == 4) &&
                            o.OrderDetails.Any(od => od.CarId == carId))
                .ToList();

            foreach (var ord in activeOrders)
            {
                foreach (var od in ord.OrderDetails.Where(d => d.CarId == carId))
                {
                    DateTime pDate = od.PickupDate ?? (ord.OrderDate.HasValue ? ord.OrderDate.Value.ToDateTime(new TimeOnly(8, 0)) : DateTime.Today);
                    DateTime rDate = od.ReturnDate ?? (ord.ReturnDate.HasValue ? ord.ReturnDate.Value.ToDateTime(new TimeOnly(20, 0)) : pDate.AddDays(1));
                    if (startTime < rDate && endTime > pDate)
                    {
                        string guestName = ord.Customer?.FullName ?? ord.Customer?.Name ?? "khách thuê";
                        TempData["CalError"] = $"Không thể chặn lịch: Trùng đơn #RC-{ord.OrderId:D4} của {guestName} ({pDate:dd/MM/yyyy HH:mm} – {rDate:dd/MM/yyyy HH:mm}).";
                        return RedirectToAction(nameof(Index), new { month = returnMonth, mode = returnMode, carId = filterCarId });
                    }
                }
            }

            // Kiểm tra xung đột với lịch đã chặn trước đó
            var conflictBlock = _context.CarCalendarBlocks
                .FirstOrDefault(b => b.CarId == carId && startTime < b.EndTime && endTime > b.StartTime);

            if (conflictBlock != null)
            {
                TempData["CalError"] = $"Không thể chặn lịch: Trùng với lịch đã chặn ({GetKindLabel(conflictBlock.Kind)}: {conflictBlock.StartTime:dd/MM/yyyy HH:mm} – {conflictBlock.EndTime:dd/MM/yyyy HH:mm}).";
                return RedirectToAction(nameof(Index), new { month = returnMonth, mode = returnMode, carId = filterCarId });
            }

            var newBlock = new CarCalendarBlock
            {
                CarId = carId,
                StartTime = startTime,
                EndTime = endTime,
                Kind = string.IsNullOrWhiteSpace(kind) ? "personal" : kind,
                Note = string.IsNullOrWhiteSpace(note) ? GetKindLabel(kind) : note.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.CarCalendarBlocks.Add(newBlock);
            _context.SaveChanges();

            TempData["CalSuccess"] = $"Đã chặn lịch xe {car.CarName} ({car.LicensePlate}) từ {startTime:dd/MM/yyyy HH:mm} đến {endTime:dd/MM/yyyy HH:mm}.";
            return RedirectToAction(nameof(Index), new { month = returnMonth ?? startTime.ToString("yyyy-MM"), mode = returnMode, carId = filterCarId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveBlock(int blockId, string? returnMonth = null, string? returnMode = "month", int? filterCarId = null)
        {
            if (!Function.IsCarOwnerLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var block = _context.CarCalendarBlocks
                .Include(b => b.Car)
                .FirstOrDefault(b => b.BlockId == blockId && b.Car != null && b.Car.OwnerId == owner.CustomerId);

            if (block != null)
            {
                string carLabel = $"{block.Car?.CarName} ({block.Car?.LicensePlate})";
                _context.CarCalendarBlocks.Remove(block);
                _context.SaveChanges();
                TempData["CalSuccess"] = $"Đã mở lại lịch trống cho xe {carLabel}.";
            }

            return RedirectToAction(nameof(Index), new { month = returnMonth, mode = returnMode, carId = filterCarId });
        }

        private static DateTime GetMonday(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        public static string GetKindLabel(string? kind)
        {
            return kind switch
            {
                "maintenance" => "Bảo dưỡng định kỳ",
                "other" => "Tạm ngưng nhận chuyến",
                _ => "Bận việc cá nhân"
            };
        }

        public static string GetOrderStatusLabel(int statusId)
        {
            return statusId switch
            {
                1 => "Chờ xác nhận",
                3 => "Đã đặt (Đã cọc 30%)",
                4 => "Đang giao xe",
                5 => "Hoàn thành",
                _ => "Khác"
            };
        }
    }

    public class CalendarIndexVm
    {
        public string MonthKey { get; set; } = "";
        public string MonthLabel { get; set; } = "";
        public string PrevMonthKey { get; set; } = "";
        public string NextMonthKey { get; set; } = "";

        public string WeekKey { get; set; } = "";
        public string WeekLabel { get; set; } = "";
        public string PrevWeekKey { get; set; } = "";
        public string NextWeekKey { get; set; } = "";

        public string Mode { get; set; } = "month";
        public int? SelectedCarId { get; set; }
        public string StatusFilter { get; set; } = "all";

        public List<Car> OwnerCars { get; set; } = new();
        public List<Car> VisibleCars { get; set; } = new();
        public decimal? MinVisiblePrice { get; set; }

        public List<CalendarDayCellVm> DaysGrid { get; set; } = new();
        public List<DateTime> WeekDays { get; set; } = new();
        public List<CalendarOrderVm> AllOrders { get; set; } = new();
        public List<CarCalendarBlock> AllBlocks { get; set; } = new();
    }

    public class CalendarDayCellVm
    {
        public DateTime Date { get; set; }
        public bool InCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public List<CalendarOrderVm> Orders { get; set; } = new();
        public List<CarCalendarBlock> Blocks { get; set; } = new();
        public int FreeCarCount { get; set; }
    }

    public class CalendarOrderVm
    {
        public int OrderId { get; set; }
        public string Code { get; set; } = "";
        public int CarId { get; set; }
        public string CarName { get; set; } = "";
        public string LicensePlate { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public DateTime PickupAt { get; set; }
        public DateTime ReturnAt { get; set; }
        public int StatusId { get; set; }
        public string StatusLabel { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public decimal OwnerNet { get; set; }
    }
}

