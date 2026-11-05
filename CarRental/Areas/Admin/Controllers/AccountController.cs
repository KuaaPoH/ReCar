using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly DbRenalCarContext _context;
        public AccountController(DbRenalCarContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // Kiểm tra trạng thái đăng nhập
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login");
            }

            var id = Function.GetAccountId(HttpContext.Session);
            var user = await _context.Customers.FirstOrDefaultAsync(a => a.CustomerId == id);

            if (user == null)
            {
                // Fallback theo email admin
                var adminEmail = Function.GetEmail(HttpContext.Session);
                if (!string.IsNullOrEmpty(adminEmail))
                {
                    user = await _context.Customers.FirstOrDefaultAsync(a => a.Email == adminEmail);
                }
            }

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Hành động để xử lý chỉnh sửa thông tin tài khoản
        [HttpPost]
        public IActionResult Edit(Customer userModel)
        {
            if (ModelState.IsValid)
            {
                var id = Function.GetAccountId(HttpContext.Session);
                var user = _context.Customers.FirstOrDefault(u => u.CustomerId == id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Username = userModel.Username;

                // Kiểm tra xem có tồn tại Email trong DB hay không
                if (userModel.Email != user.Email)
                {
                    var emailExists = _context.Customers.Any(p => p.Email == userModel.Email && p.CustomerId != user.CustomerId);
                    if (emailExists)
                    {
                        Function._MessageEmail = "Email đã tồn tại";
                        return View(user);
                    }
                    else
                    {
                        user.Email = userModel.Email;
                    }
                }

                user.PhoneNumber = userModel.PhoneNumber ?? userModel.Phone;
                user.FullName = userModel.FullName;
                Function._MessageEmail = string.Empty;

                if (!string.IsNullOrEmpty(userModel.Password))
                {
                    user.Password = Function.MD5Password(userModel.Password);
                }

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return RedirectToAction("Edit", "Account");
            }

            return View(userModel);
        }

        // Kiểm tra xem người dùng có tồn tại hay không
        private bool UserExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
