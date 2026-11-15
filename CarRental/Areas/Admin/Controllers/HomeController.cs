using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly DbRenalCarContext _context;

        public HomeController(DbRenalCarContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Kiểm tra trạng thái đăng nhập và quyền Admin
            if (!Function.IsLogin(HttpContext.Session) || !Function.IsAdmin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            // 1. Thống kê KPI cơ bản (100% dữ liệu thật từ CSDL)
            var allCars = _context.Cars.Include(c => c.Type).ToList();
            var allOrders = _context.CarRentalOrders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .ToList();

            var validOrders = allOrders.Where(o => o.StatusId == 3 || o.StatusId == 4 || o.StatusId == 5).ToList();

            decimal CalcOrderTotal(CarRentalOrder o)
            {
                decimal total = (o.Deposit ?? 0m) + (o.Payment ?? 0m);
                if (total <= 0 && o.OrderDetails != null && o.OrderDetails.Any())
                {
                    foreach (var d in o.OrderDetails)
                    {
                        var pDate = d.PickupDate ?? (o.OrderDate.HasValue ? o.OrderDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Today);
                        var rDate = d.ReturnDate ?? (o.ReturnDate.HasValue ? o.ReturnDate.Value.ToDateTime(TimeOnly.MinValue) : pDate.AddDays(1));
                        int days = Math.Max(1, (int)Math.Ceiling((rDate.Date - pDate.Date).TotalDays));
                        total += (d.Price ?? d.Car?.Price ?? 0m) * days;
                    }
                }
                if (total <= 0 && (o.Deposit ?? 0m) > 0)
                {
                    total = Math.Round((o.Deposit ?? 0m) / 0.3m, 0);
                }
                return total;
            }

            decimal gmv = validOrders.Sum(o => CalcOrderTotal(o));
            var sysSetting = _context.SystemSettings.FirstOrDefault();
            decimal commissionPct = sysSetting?.CommissionPct ?? 10m;
            decimal netCommission = validOrders.Sum(o => o.PlatformFee ?? Math.Round(CalcOrderTotal(o) * commissionPct / 100m, 0));

            var nowVn = Function.NowVn;
            var prevMonthDate = nowVn.AddMonths(-1);
            decimal thisMonthGmv = validOrders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == nowVn.Year && o.OrderDate.Value.Month == nowVn.Month)
                .Sum(o => CalcOrderTotal(o));
            decimal prevMonthGmv = validOrders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == prevMonthDate.Year && o.OrderDate.Value.Month == prevMonthDate.Month)
                .Sum(o => CalcOrderTotal(o));
            double gmvDelta = prevMonthGmv > 0
                ? Math.Round((double)((thisMonthGmv - prevMonthGmv) / prevMonthGmv * 100m), 1)
                : (thisMonthGmv > 0 ? 100.0 : 0.0);

            var activeCars = allCars.Where(c => c.CarStatus == "Sẵn sàng" || c.CarStatus == "Đang bận" || (c.IsActive && c.CarStatus != "Chờ duyệt" && c.CarStatus != "Bảo trì")).ToList();
            var rentedCars = allCars.Where(c => c.CarStatus == "Đang bận" || c.CarStatus == "Đang có khách").ToList();
            int occupancyRate = activeCars.Count > 0 ? (int)Math.Round((double)rentedCars.Count * 100 / activeCars.Count) : 0;
            int pendingCount = allCars.Count(c => c.CarStatus == "Chờ duyệt" || c.CarStatus == "Chờ cập nhật");

            ViewBag.GMV = gmv;
            ViewBag.GmvDelta = gmvDelta;
            ViewBag.CommissionPct = commissionPct;
            ViewBag.NetCommission = netCommission;
            ViewBag.ActiveCarsCount = activeCars.Count;
            ViewBag.OccupancyRate = occupancyRate;
            ViewBag.PendingCarsCount = pendingCount;

            // 2. Cơ cấu trạng thái đội xe
            ViewBag.ReadyCarsCount = allCars.Count(c => c.CarStatus == "Sẵn sàng");
            ViewBag.RentedCarsCount = rentedCars.Count;
            ViewBag.MaintenanceCarsCount = allCars.Count(c => c.CarStatus == "Bảo trì");
            ViewBag.PendingAndSuspendedCount = allCars.Count(c => c.CarStatus == "Chờ duyệt" || c.CarStatus == "Chờ cập nhật" || c.CarStatus == "Từ chối" || !c.IsActive);

            // 3. Biểu đồ 6 tháng gần nhất (tính đến tháng hiện tại từ dữ liệu thật CSDL)
            var chartMonths = new List<string>();
            var chartGmv = new List<decimal>();
            var chartCommission = new List<decimal>();
            var chartTrips = new List<int>();
            for (int i = -5; i <= 0; i++)
            {
                var m = nowVn.AddMonths(i);
                chartMonths.Add($"T{m.Month}");
                var mOrders = validOrders
                    .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == m.Year && o.OrderDate.Value.Month == m.Month)
                    .ToList();
                decimal mGmv = mOrders.Sum(o => CalcOrderTotal(o));
                decimal mComm = mOrders.Sum(o => o.PlatformFee ?? Math.Round(CalcOrderTotal(o) * commissionPct / 100m, 0));
                chartGmv.Add(mGmv);
                chartCommission.Add(mComm);
                chartTrips.Add(mOrders.Count);
            }
            ViewBag.ChartMonths = chartMonths.ToArray();
            ViewBag.ChartGMV = chartGmv.ToArray();
            ViewBag.ChartCommission = chartCommission.ToArray();
            ViewBag.ChartTrips = chartTrips.ToArray();

            // 4. Danh sách đơn hàng gần đây (kèm thông tin xe & khách)
            var recentOrders = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .OrderByDescending(o => o.OrderId)
                .Take(6)
                .ToList();
            ViewBag.RecentOrders = recentOrders;

            // 5. Danh sách Top 5 chủ xe có doanh thu cao (tính thật từ đơn thuê & đánh giá CSDL)
            var ownersList = _context.Customers.Where(a => a.RoleId == 3).ToList();
            var allReviews = _context.CarReviews.Where(r => r.IsActive != false && r.Star.HasValue).ToList();

            var topOwners = ownersList.Select(a => {
                var ownerCars = allCars.Where(c => c.OwnerId == a.CustomerId).ToList();
                var ownerCarIds = ownerCars.Select(c => c.CarId).ToHashSet();
                var ownerOrders = validOrders.Where(o => o.OrderDetails.Any(od => ownerCarIds.Contains(od.CarId))).ToList();

                int carCount = ownerCars.Count;
                int trips = ownerOrders.Count;
                decimal revenue = ownerOrders.Sum(o => CalcOrderTotal(o));

                var ownerReviews = allReviews.Where(r => r.CarId.HasValue && ownerCarIds.Contains(r.CarId.Value)).ToList();
                double rating = ownerReviews.Any() ? Math.Round(ownerReviews.Average(r => r.Star!.Value), 1) : 5.0;

                return new CarRental.ViewModels.TopOwnerVM {
                    AccountId = a.CustomerId,
                    FullName = a.FullName ?? a.Name ?? a.Username ?? "Chủ xe đối tác",
                    Phone = a.PhoneNumber ?? "Chưa cập nhật",
                    CarCount = carCount,
                    Trips = trips,
                    Revenue = revenue,
                    Rating = rating
                };
            }).OrderByDescending(x => x.Revenue).ThenByDescending(x => x.Trips).Take(5).ToList();
            ViewBag.TopOwners = topOwners;

            return View();
        }

        public IActionResult Logout()
        {
            Function.ClearAdminSession(HttpContext.Session);
            return RedirectToAction("Index", "Login", new { area = "Admin" });
        }
    }
}
