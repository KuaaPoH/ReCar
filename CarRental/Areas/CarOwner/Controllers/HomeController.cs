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

            // Kiểm tra role chủ xe linh hoạt
            var accountId = Function.GetAccountId(HttpContext.Session);
            var carOwnerId = Function.GetCarOwnerAccountId(HttpContext.Session);
            var userEmail = Function.GetEmail(HttpContext.Session);

            var account = _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefault(a => (carOwnerId > 0 && a.AccountId == carOwnerId) ||
                                     a.AccountId == accountId ||
                                     (!string.IsNullOrEmpty(userEmail) && a.Email == userEmail && a.RoleId == 3));

            if (account == null || account.RoleId != 3) // RoleId 3 = Chủ xe
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

            // Đảm bảo phiên CarOwner được lưu độc lập
            Function.SetCarOwnerSession(HttpContext.Session, account.AccountId, account.Username ?? string.Empty, account.Email ?? string.Empty, account.FullName ?? string.Empty);

            var ownerId = account.AccountId;
            var totalCars = _context.Cars.Count(c => c.OwnerId == ownerId);
            var activeCars = _context.Cars.Count(c => c.OwnerId == ownerId && c.IsActive);
            var totalOrders = _context.OrderDetails
                .Include(od => od.Order)
                .Count(od => od.Car != null && od.Car.OwnerId == ownerId);
            var totalRevenue = _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.Car != null && od.Car.OwnerId == ownerId && od.Order.StatusId == 3) // Đã thanh toán
                .Sum(od => od.Price * od.Quantity) ?? 0;

            ViewBag.TotalCars = totalCars;
            ViewBag.ActiveCars = activeCars;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;

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
    }
}

