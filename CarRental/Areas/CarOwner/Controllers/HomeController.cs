using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.CarOwner.Controllers
{
    [Area("CarOwner")]
    public class HomeController : Controller
    {
        private readonly DbRenalCarContext _context;

        public HomeController(DbRenalCarContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Kiểm tra đăng nhập và role
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            // Kiểm tra role chủ xe linh hoạt trực tiếp từ bảng Customers
            var accountId = Function.GetAccountId(HttpContext.Session);
            var carOwnerId = Function.GetCarOwnerAccountId(HttpContext.Session);
            var userEmail = Function.GetEmail(HttpContext.Session);

            var customer = _context.Customers
                .Include(a => a.Role)
                .FirstOrDefault(a => (carOwnerId > 0 && a.CustomerId == carOwnerId) ||
                                     a.CustomerId == accountId ||
                                     (!string.IsNullOrEmpty(userEmail) && a.Email == userEmail && a.RoleId == 3));

            if (customer == null || customer.RoleId != 3) // RoleId 3 = Chủ xe
            {
                // Kiểm tra xem đã nộp hồ sơ đăng ký chủ xe chưa
                var reg = _context.CarOwnerRegistrations
                    .OrderByDescending(r => r.CreatedAt)
                    .FirstOrDefault(r => r.CustomerId == accountId || (!string.IsNullOrEmpty(userEmail) && r.Email == userEmail));

                if (reg != null)
                {
                    if (reg.Status == "Pending")
                    {
                        return RedirectToAction("Pending", "Register", new { area = "CarOwner" });
                    }
                    if (reg.Status == "Rejected")
                    {
                        return RedirectToAction("Rejected", "Register", new { area = "CarOwner" });
                    }
                }

                // Nếu chưa nộp hồ sơ đăng ký -> chuyển đến trang đăng ký chủ xe
                return RedirectToAction("Index", "Register", new { area = "CarOwner" });
            }

            // Đảm bảo phiên CarOwner được lưu đồng bộ với CustomerId
            Function.SetCarOwnerSession(HttpContext.Session, customer.CustomerId, customer.Username ?? customer.Name ?? "Chủ xe", customer.Email ?? string.Empty, customer.FullName ?? customer.Name ?? "Chủ xe");
            customer.LastLogin = Function.NowVn.ToString("yyyy-MM-dd HH:mm:ss");
            _context.SaveChanges();

            var ownerId = customer.CustomerId;
            var ownerCarIds = _context.Cars.Where(c => c.OwnerId == ownerId).Select(c => c.CarId).ToList();

            var totalCars = _context.Cars.Count(c => c.OwnerId == ownerId);
            var activeCars = _context.Cars.Count(c => c.OwnerId == ownerId && (c.CarStatus == "Sẵn sàng" || (c.CarStatus == null && c.IsActive)));
            var busyCars = _context.Cars.Count(c => c.OwnerId == ownerId && (c.CarStatus == "Đang bận" || c.CarStatus == "Đang có khách"));
            var otherCars = Math.Max(0, totalCars - activeCars - busyCars);

            // Lấy tất cả đơn thuê liên quan đến xe của chủ xe
            var ownerOrders = _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.Car != null && od.Car.OwnerId == ownerId && od.Order != null)
                .ToList();

            var totalOrders = ownerOrders.Count;
            var completedOrders = ownerOrders.Count(od => od.Order!.StatusId == 5);
            var processingOrders = ownerOrders.Count(od => od.Order!.StatusId == 3 || od.Order!.StatusId == 4);

            decimal CalcOrderDetailAmount(OrderDetail od)
            {
                decimal orderTotal = (od.Order?.Deposit ?? 0m) + (od.Order?.Payment ?? 0m);
                if (orderTotal > 0)
                    return orderTotal;
                int days = od.Quantity ?? 1;
                if (od.PickupDate.HasValue && od.ReturnDate.HasValue)
                    days = Math.Max(1, (int)Math.Ceiling((od.ReturnDate.Value.Date - od.PickupDate.Value.Date).TotalDays));
                return (od.Price ?? 0m) * days;
            }

            var sysSetting = _context.SystemSettings.FirstOrDefault();
            int cfgCommissionPct = sysSetting?.CommissionPct ?? 10;
            int cfgDepositPct = sysSetting?.DepositPct ?? 30;
            decimal defaultCommRate = cfgCommissionPct / 100m;
            decimal defaultDepRate = cfgDepositPct / 100m;

            decimal CalcOrderNetAmount(OrderDetail od)
            {
                decimal gross = CalcOrderDetailAmount(od);
                decimal fee = od.Order?.PlatformFee ?? Math.Round(gross * defaultCommRate, 0);
                return Math.Max(0m, gross - fee);
            }

            var validRevenueOrders = ownerOrders
                .Where(od => od.Order!.StatusId == 3 || od.Order!.StatusId == 4 || od.Order!.StatusId == 5)
                .ToList();

            var totalGross = validRevenueOrders.Sum(od => CalcOrderDetailAmount(od));
            var totalNet = validRevenueOrders.Sum(od => CalcOrderNetAmount(od));
            var heldDeposits = validRevenueOrders
                .Where(od => od.Order!.StatusId == 3 || od.Order!.StatusId == 4)
                .Sum(od => od.Order!.Deposit ?? Math.Round(CalcOrderDetailAmount(od) * defaultDepRate, 0));

            // Số liệu tháng hiện tại & tháng trước
            var nowVn = Function.NowVn;
            var thisMonthStart = new DateOnly(nowVn.Year, nowVn.Month, 1);
            var prevMonthDate = nowVn.AddMonths(-1);
            var prevMonthStart = new DateOnly(prevMonthDate.Year, prevMonthDate.Month, 1);

            var thisMonthOrders = validRevenueOrders.Where(od =>
                od.Order!.OrderDate.HasValue &&
                od.Order.OrderDate.Value.Year == thisMonthStart.Year &&
                od.Order.OrderDate.Value.Month == thisMonthStart.Month).ToList();

            var prevMonthOrders = validRevenueOrders.Where(od =>
                od.Order!.OrderDate.HasValue &&
                od.Order.OrderDate.Value.Year == prevMonthStart.Year &&
                od.Order.OrderDate.Value.Month == prevMonthStart.Month).ToList();

            decimal monthGross = thisMonthOrders.Sum(od => CalcOrderDetailAmount(od));
            decimal monthNet = thisMonthOrders.Sum(od => CalcOrderNetAmount(od));
            decimal monthHeldDeposits = thisMonthOrders
                .Where(od => od.Order!.StatusId == 3 || od.Order!.StatusId == 4)
                .Sum(od => od.Order!.Deposit ?? Math.Round(CalcOrderDetailAmount(od) * defaultDepRate, 0));

            decimal prevMonthNet = prevMonthOrders.Sum(od => CalcOrderNetAmount(od));
            double? growthPercent = prevMonthNet > 0
                ? Math.Round((double)((monthNet - prevMonthNet) / prevMonthNet * 100m), 1)
                : (monthNet > 0 ? 100.0 : (double?)null);

            // Đánh giá trung bình thật từ bảng CarReviews
            var ownerReviews = _context.CarReviews
                .Where(r => r.CarId.HasValue && ownerCarIds.Contains(r.CarId.Value) && r.IsActive != false && r.Star.HasValue)
                .ToList();
            int reviewCount = ownerReviews.Count;
            double avgRating = reviewCount > 0 ? Math.Round(ownerReviews.Average(r => r.Star!.Value), 1) : 0.0;
            int positiveRate = reviewCount > 0 ? (int)Math.Round(ownerReviews.Count(r => r.Star!.Value >= 4) * 100.0 / reviewCount) : 0;

            // Biểu đồ 6 tháng gần nhất thật từ CSDL
            var chartLabels = new List<string>();
            var chartGrossData = new List<decimal>();
            var chartNetData = new List<decimal>();
            for (int i = -5; i <= 0; i++)
            {
                var mDate = nowVn.AddMonths(i);
                chartLabels.Add($"T{mDate.Month}");
                var mDetails = validRevenueOrders
                    .Where(od => od.Order!.OrderDate.HasValue &&
                                 od.Order.OrderDate.Value.Year == mDate.Year &&
                                 od.Order.OrderDate.Value.Month == mDate.Month)
                    .ToList();
                decimal mGross = mDetails.Sum(od => CalcOrderDetailAmount(od));
                decimal mNet = mDetails.Sum(od => CalcOrderNetAmount(od));
                chartGrossData.Add(Math.Round(mGross / 1_000_000m, 2));
                chartNetData.Add(Math.Round(mNet / 1_000_000m, 2));
            }

            ViewBag.TotalCars = totalCars;
            ViewBag.CommissionPct = cfgCommissionPct;
            ViewBag.ActiveCars = activeCars;
            ViewBag.BusyCars = busyCars;
            ViewBag.OtherCars = otherCars;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.CompletedOrders = completedOrders;
            ViewBag.ProcessingOrders = processingOrders;
            ViewBag.TotalRevenue = totalGross;
            ViewBag.NetRevenue = totalNet;
            ViewBag.HeldDeposits = heldDeposits;
            ViewBag.MonthGross = monthGross;
            ViewBag.MonthNet = monthNet;
            ViewBag.MonthHeldDeposits = monthHeldDeposits;
            ViewBag.GrowthPercent = growthPercent;
            ViewBag.ReviewCount = reviewCount;
            ViewBag.AvgRating = avgRating;
            ViewBag.PositiveRate = positiveRate;
            ViewBag.ChartLabelsJson = System.Text.Json.JsonSerializer.Serialize(chartLabels);
            ViewBag.ChartGrossJson = System.Text.Json.JsonSerializer.Serialize(chartGrossData);
            ViewBag.ChartNetJson = System.Text.Json.JsonSerializer.Serialize(chartNetData);

            // Lấy danh sách xe gần đây
            var recentCars = _context.Cars
                .Include(c => c.Type)
                .Where(c => c.OwnerId == ownerId)
                .OrderByDescending(c => c.CarId)
                .Take(5)
                .ToList();

            ViewBag.RecentCars = recentCars;

            // Lấy danh sách đơn thuê gần đây của chủ xe
            var recentOrders = _context.OrderDetails
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Status)
                .Include(od => od.Car)
                .Where(od => od.Car != null && od.Car.OwnerId == ownerId)
                .OrderByDescending(od => od.OrderDetailId)
                .Take(5)
                .ToList();

            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        public IActionResult Logout()
        {
            Function.ClearCarOwnerSession(HttpContext.Session);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // GET: /CarOwner/Home/Roadmap (Lộ trình tính năng tương lai theo template)
        public IActionResult Roadmap()
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            return View();
        }
    }
}
