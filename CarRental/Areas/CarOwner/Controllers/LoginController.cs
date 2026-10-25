using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarRental.Areas.CarOwner.Controllers
{
    [Area("CarOwner")]
    public class LoginController : Controller
    {
        private readonly DbRenalCarContext _context;

        public LoginController(DbRenalCarContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string? ReturnUrl = null)
        {
            if (Function.IsCarOwnerLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Index(Account account, string? ReturnUrl)
        {
            if (account == null)
            {
                return NotFound();
            }

            // Mã hóa mật khẩu trước khi kiểm tra
            string p = Function.MD5Password(account.Password);

            // Kiểm tra tài khoản có tồn tại và là chủ xe (RoleId = 3)
            var check = _context.Accounts
                .Include(a => a.Role)
                .Where(m => (m.Email == account.Email || m.Username == account.Email) && 
                            m.Password == p && 
                            m.RoleId == 3) // RoleId 3 = Chủ xe
                .FirstOrDefault();

            if (check == null)
            {
                Function.SetMessage(HttpContext.Session, "Sai thông tin đăng nhập hoặc tài khoản của bạn chưa được cấp quyền Chủ xe.");
                return View(account);
            }

            // Kiểm tra tài khoản có bị khóa không
            if (check.IsActive == false)
            {
                Function.SetMessage(HttpContext.Session, "Tài khoản đối tác của bạn đang bị tạm khóa hoặc chờ phê duyệt.");
                return View(account);
            }

            // Đăng nhập thành công - lưu độc lập vào CarOwner Session
            Function.SetMessage(HttpContext.Session, string.Empty);
            Function.SetCarOwnerSession(HttpContext.Session, check.AccountId, check.Username ?? string.Empty, check.Email ?? string.Empty, check.FullName ?? string.Empty);

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
