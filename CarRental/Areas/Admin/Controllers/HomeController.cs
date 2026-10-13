using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            // Thống kê tổng hợp
            ViewBag.TotalCustomers = _context.Customers.Count();
            ViewBag.TotalCarOwners = _context.Accounts.Count(a => a.RoleId == 3);
            ViewBag.TotalCars = _context.Cars.Count();
            ViewBag.TotalOrders = _context.CarRentalOrders.Count();
            ViewBag.TotalRevenue = _context.CarRentalOrders.Where(o => o.StatusId == 3).Sum(o => o.Payment ?? 0);

            // Danh sách đơn hàng gần đây
            ViewBag.RecentOrders = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .OrderByDescending(o => o.OrderId)
                .Take(5)
                .ToList();

            // Danh sách xe gần đây
            ViewBag.RecentCars = _context.Cars
                .Include(c => c.Type)
                .OrderByDescending(c => c.CarId)
                .Take(5)
                .ToList();

            return View();
        }

        public IActionResult Logout()
        {
            Function.ClearSession(HttpContext.Session);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
