using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.CarOwner.Controllers
{
    [Area("CarOwner")]
    public class AnalyticsController : Controller
    {
        private readonly DbRenalCarContext _context;

        public AnalyticsController(DbRenalCarContext context)
        {
            _context = context;
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

        public IActionResult Index(
            string period = "6m",
            string? start = null,
            string? end = null,
            int? carId = null,
            string brand = "all",
            string segment = "all",
            string sort = "net_desc",
            string occupancySort = "desc",
            string chartMode = "net")
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

            DateTime today = DateTime.Today;
            DateTime startDate;
            DateTime endDate = today;

            switch (period)
            {
                case "7":
                    startDate = today.AddDays(-6);
                    break;
                case "30":
                    startDate = today.AddDays(-29);
                    break;
                case "month":
                    startDate = new DateTime(today.Year, today.Month, 1);
                    endDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                    break;
                case "prev":
                    var prevMonth = today.AddMonths(-1);
                    startDate = new DateTime(prevMonth.Year, prevMonth.Month, 1);
                    endDate = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
                    break;
                case "custom":
                    startDate = DateTime.TryParse(start, out var s) ? s.Date : today.AddMonths(-3);
                    endDate = DateTime.TryParse(end, out var e) ? e.Date : today;
                    break;
                default: // "6m"
                    var sixMonthsAgo = today.AddMonths(-5);
                    startDate = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1);
                    endDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                    break;
            }

            if (endDate < startDate)
            {
                endDate = startDate;
            }

            int totalDaysInPeriod = Math.Max(1, (int)(endDate.Date - startDate.Date).TotalDays + 1);

            var allOwnerCars = _context.Cars
                .Include(c => c.Type)
                .Include(c => c.CarReviews)
                .Where(c => c.OwnerId == owner.CustomerId)
                .OrderBy(c => c.CarName)
                .ToList();

            var brands = allOwnerCars
                .Select(c => c.CarBrand)
                .Where(b => !string.IsNullOrWhiteSpace(b))
                .Distinct()
                .OrderBy(b => b)
                .ToList()!;

            var segments = allOwnerCars
                .Select(c => c.Type?.CarTypeName ?? (c.Seat.HasValue ? $"{c.Seat} chỗ" : "Tiêu chuẩn"))
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            var filteredCars = allOwnerCars
                .Where(c => !carId.HasValue || c.CarId == carId.Value)
                .Where(c => brand == "all" || string.Equals(c.CarBrand, brand, StringComparison.OrdinalIgnoreCase))
                .Where(c =>
                {
                    if (segment == "all") return true;
                    string carSeg = c.Type?.CarTypeName ?? (c.Seat.HasValue ? $"{c.Seat} chỗ" : "Tiêu chuẩn");
                    return string.Equals(carSeg, segment, StringComparison.OrdinalIgnoreCase);
                })
                .ToList();

            var filteredCarIds = filteredCars.Select(c => c.CarId).ToList();

            var ordersInRange = _context.CarRentalOrders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .Where(o => (o.StatusId == 5 || o.StatusId == 4 || o.StatusId == 3) &&
                            o.OrderDetails.Any(od => filteredCarIds.Contains(od.CarId)))
                .ToList();

            List<CarCalendarBlock> blocksInRange = new();
            try
            {
                blocksInRange = _context.CarCalendarBlocks
                    .Where(b => filteredCarIds.Contains(b.CarId) && b.StartTime <= endDate.AddDays(1) && b.EndTime >= startDate)
                    .ToList();
            }
            catch
            {
                // Ignore if table not created yet
            }

            var carRows = new List<CarPerformanceRowVm>();
            foreach (var c in filteredCars)
            {
                string carSeg = c.Type?.CarTypeName ?? (c.Seat.HasValue ? $"{c.Seat} chỗ" : "Tiêu chuẩn");

                // Tính số ngày xe bị chặn trong kỳ
                var carBlocks = blocksInRange.Where(b => b.CarId == c.CarId).ToList();
                int blockedDays = 0;
                foreach (var b in carBlocks)
                {
                    DateTime bStart = b.StartTime.Date < startDate ? startDate : b.StartTime.Date;
                    DateTime bEnd = b.EndTime.Date > endDate ? endDate : b.EndTime.Date;
                    if (bEnd >= bStart)
                    {
                        blockedDays += (int)(bEnd - bStart).TotalDays + 1;
                    }
                }
                blockedDays = Math.Min(totalDaysInPeriod - 1, blockedDays);
                int availableDays = Math.Max(1, totalDaysInPeriod - blockedDays);

                int tripsCount = 0;
                int rentedDays = 0;
                decimal grossRevenue = 0m;

                foreach (var ord in ordersInRange)
                {
                    foreach (var od in ord.OrderDetails.Where(d => d.CarId == c.CarId))
                    {
                        DateTime pDate = (od.PickupDate ?? (ord.OrderDate.HasValue ? ord.OrderDate.Value.ToDateTime(TimeOnly.MinValue) : startDate)).Date;
                        DateTime rDate = (od.ReturnDate ?? (ord.ReturnDate.HasValue ? ord.ReturnDate.Value.ToDateTime(TimeOnly.MinValue) : pDate.AddDays(1))).Date;
                        if (rDate < pDate) rDate = pDate;

                        // Kiểm tra giao nhau với khoảng lọc
                        if (pDate <= endDate && rDate >= startDate)
                        {
                            DateTime overlapStart = pDate < startDate ? startDate : pDate;
                            DateTime overlapEnd = rDate > endDate ? endDate : rDate;
                            int daysInOverlap = Math.Max(1, (int)(overlapEnd - overlapStart).TotalDays + 1);
                            rentedDays += daysInOverlap;

                            if (ord.StatusId == 5)
                            {
                                tripsCount++;
                                decimal orderTotal = WalletController.CalculateOrderTotal(ord);
                                grossRevenue += orderTotal;
                            }
                        }
                    }
                }

                rentedDays = Math.Min(availableDays, rentedDays);
                int occupancyPct = (int)Math.Round((rentedDays * 100.0) / availableDays);
                decimal netRevenue = Math.Round(grossRevenue * 0.90m, 0);

                var activeReviews = c.CarReviews.Where(r => r.IsActive != false && r.Star.HasValue).ToList();
                double ratingAvg = activeReviews.Any()
                    ? Math.Round(activeReviews.Average(r => r.Star!.Value), 1)
                    : (c.Rate ?? 5.0);

                carRows.Add(new CarPerformanceRowVm
                {
                    CarId = c.CarId,
                    CarName = c.CarName ?? "Xe tự lái",
                    LicensePlate = c.LicensePlate ?? "",
                    Brand = c.CarBrand ?? "",
                    Segment = carSeg,
                    BasePrice = c.Price ?? 0m,
                    Trips = tripsCount,
                    RentedDays = rentedDays,
                    BlockedDays = blockedDays,
                    AvailableDays = availableDays,
                    OccupancyPct = occupancyPct,
                    Gross = grossRevenue,
                    Net = netRevenue,
                    Rating = ratingAvg,
                    ReviewCount = activeReviews.Count
                });
            }

            // Sắp xếp bảng hiệu suất từng xe
            carRows = sort switch
            {
                "occupancy_desc" => carRows.OrderByDescending(r => r.OccupancyPct).ToList(),
                "occupancy_asc" => carRows.OrderBy(r => r.OccupancyPct).ToList(),
                "trips_desc" => carRows.OrderByDescending(r => r.Trips).ToList(),
                "rating_desc" => carRows.OrderByDescending(r => r.Rating).ToList(),
                _ => carRows.OrderByDescending(r => r.Net).ToList()
            };

            var occupancyChartRows = occupancySort == "asc"
                ? carRows.OrderBy(r => r.OccupancyPct).ToList()
                : carRows.OrderByDescending(r => r.OccupancyPct).ToList();

            // Dữ liệu biểu đồ doanh thu theo tháng (6 điểm thời gian gần nhất trong kỳ)
            var trendLabels = new List<string>();
            var trendGross = new List<decimal>();
            var trendNet = new List<decimal>();

            DateTime cursorMonth = new DateTime(startDate.Year, startDate.Month, 1);
            DateTime endMonth = new DateTime(endDate.Year, endDate.Month, 1);
            if (cursorMonth == endMonth)
            {
                cursorMonth = endMonth.AddMonths(-5);
            }

            while (cursorMonth <= endMonth)
            {
                DateTime mStart = cursorMonth;
                DateTime mEnd = new DateTime(cursorMonth.Year, cursorMonth.Month, DateTime.DaysInMonth(cursorMonth.Year, cursorMonth.Month));
                decimal mGross = 0m;

                foreach (var ord in ordersInRange.Where(o => o.StatusId == 5))
                {
                    var refDate = ord.ReturnDate?.ToDateTime(TimeOnly.MinValue)
                                  ?? ord.OrderDetails.FirstOrDefault()?.ReturnDate
                                  ?? ord.OrderDate?.ToDateTime(TimeOnly.MinValue)
                                  ?? today;
                    if (refDate >= mStart && refDate <= mEnd)
                    {
                        mGross += WalletController.CalculateOrderTotal(ord);
                    }
                }

                trendLabels.Add($"T{cursorMonth.Month}/{cursorMonth.Year % 100:D2}");
                trendGross.Add(mGross);
                trendNet.Add(Math.Round(mGross * 0.90m, 0));

                cursorMonth = cursorMonth.AddMonths(1);
            }

            // Tổng hợp đánh giá của khách thuê
            var allReviews = _context.CarReviews
                .Include(r => r.Car)
                .Where(r => r.CarId.HasValue && filteredCarIds.Contains(r.CarId.Value) && r.IsActive != false)
                .OrderByDescending(r => r.CreatedDate)
                .ThenByDescending(r => r.CarReviewId)
                .ToList();

            double overallRating = allReviews.Any(r => r.Star.HasValue)
                ? Math.Round(allReviews.Where(r => r.Star.HasValue).Average(r => r.Star!.Value), 1)
                : (carRows.Any() ? Math.Round(carRows.Average(r => r.Rating), 1) : 5.0);

            int totalRented = carRows.Sum(r => r.RentedDays);
            int totalAvail = Math.Max(1, carRows.Sum(r => r.AvailableDays));
            int overallOccupancy = (int)Math.Round((totalRented * 100.0) / totalAvail);
            decimal totalGross = carRows.Sum(r => r.Gross);
            decimal totalNet = carRows.Sum(r => r.Net);
            int totalTrips = carRows.Sum(r => r.Trips);
            string avgDaysPerTrip = totalTrips > 0 ? Math.Round((double)totalRented / totalTrips, 1).ToString("0.0") : "0";

            var vm = new AnalyticsIndexVm
            {
                Period = period,
                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd"),
                SelectedCarId = carId,
                BrandFilter = brand,
                SegmentFilter = segment,
                SortBy = sort,
                OccupancySort = occupancySort,
                ChartMode = chartMode,
                OwnerCars = allOwnerCars,
                Brands = brands,
                Segments = segments,
                OverallOccupancyPct = overallOccupancy,
                TotalRentedDays = totalRented,
                TotalAvailableDays = totalAvail,
                TotalGross = totalGross,
                TotalNet = totalNet,
                TotalTrips = totalTrips,
                AvgDaysPerTrip = avgDaysPerTrip,
                OverallRating = overallRating,
                TotalReviewsCount = allReviews.Count,
                CarRows = carRows,
                OccupancyChartRows = occupancyChartRows,
                TrendLabels = trendLabels,
                TrendGross = trendGross,
                TrendNet = trendNet,
                Reviews = allReviews
            };

            ViewData["Title"] = "Phân tích hiệu suất";
            return View(vm);
        }
    }

    public class AnalyticsIndexVm
    {
        public string Period { get; set; } = "6m";
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public int? SelectedCarId { get; set; }
        public string BrandFilter { get; set; } = "all";
        public string SegmentFilter { get; set; } = "all";
        public string SortBy { get; set; } = "net_desc";
        public string OccupancySort { get; set; } = "desc";
        public string ChartMode { get; set; } = "net";

        public List<Car> OwnerCars { get; set; } = new();
        public List<string> Brands { get; set; } = new();
        public List<string> Segments { get; set; } = new();

        public int OverallOccupancyPct { get; set; }
        public int TotalRentedDays { get; set; }
        public int TotalAvailableDays { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalNet { get; set; }
        public int TotalTrips { get; set; }
        public string AvgDaysPerTrip { get; set; } = "0";
        public double OverallRating { get; set; }
        public int TotalReviewsCount { get; set; }

        public List<CarPerformanceRowVm> CarRows { get; set; } = new();
        public List<CarPerformanceRowVm> OccupancyChartRows { get; set; } = new();

        public List<string> TrendLabels { get; set; } = new();
        public List<decimal> TrendGross { get; set; } = new();
        public List<decimal> TrendNet { get; set; } = new();

        public List<CarReview> Reviews { get; set; } = new();
    }

    public class CarPerformanceRowVm
    {
        public int CarId { get; set; }
        public string CarName { get; set; } = "";
        public string LicensePlate { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Segment { get; set; } = "";
        public decimal BasePrice { get; set; }
        public int Trips { get; set; }
        public int RentedDays { get; set; }
        public int BlockedDays { get; set; }
        public int AvailableDays { get; set; }
        public int OccupancyPct { get; set; }
        public decimal Gross { get; set; }
        public decimal Net { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
    }
}

