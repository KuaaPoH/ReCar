using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using CarRental.Utilities;
using System.Linq;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RegisterController : Controller
    {
        private readonly DbRenalCarContext _context;
        public RegisterController(DbRenalCarContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Customer acc) 
        {
            if (acc == null) {
                return NotFound();
            }
            // Kiểm tra sự tồn tại của email trong Customers
            var check = _context.Customers.FirstOrDefault(m => m.Email == acc.Email);
            if (check != null) {
                Function._MessageEmail = "Dupplicate Email";
                return RedirectToAction("Index", "Register");
            }
            
            Function._MessageEmail = string.Empty;
            acc.Password = Function.MD5Password(acc.Password);
            acc.RoleId = acc.RoleId > 0 ? acc.RoleId : 1; // Mặc định role Admin nếu đăng ký tại trang Admin
            acc.IsActive = true;
            _context.Customers.Add(acc);
            _context.SaveChanges();
            return RedirectToAction("Index", "Login");
        }
    }
}
