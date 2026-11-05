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
        public async Task<IActionResult> Index(string? Email, string? Password, string? ReturnUrl)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Function.SetMessage(HttpContext.Session, "Vui lòng nhập đầy đủ tài khoản và mật khẩu.");
                return View();
            }

            // Mã hóa mật khẩu trước khi kiểm tra
            string p = Function.MD5Password(Password);

            // Kiểm tra tài khoản có tồn tại trong bảng Customers và là chủ xe (RoleId = 3)
            var check = await _context.Customers
                .Include(a => a.Role)
                .Where(m => (m.Email == Email || m.Username == Email) && 
                            m.Password == p && 
                            m.RoleId == 3) // RoleId 3 = Chủ xe
                .FirstOrDefaultAsync();

            if (check == null)
            {
                Function.SetMessage(HttpContext.Session, "Sai thông tin đăng nhập hoặc tài khoản của bạn chưa được cấp quyền Chủ xe.");
                return View();
            }

            // Kiểm tra tài khoản có bị khóa không
            if (!check.IsActive)
            {
                Function.SetMessage(HttpContext.Session, "Tài khoản đối tác của bạn đang bị tạm khóa hoặc chờ phê duyệt.");
                return View();
            }

            // Đăng nhập thành công - lưu độc lập và đồng bộ vào CarOwner Session với CustomerId
            Function.SetMessage(HttpContext.Session, string.Empty);
            Function.SetCarOwnerSession(HttpContext.Session, check.CustomerId, check.Username ?? check.Name ?? "Chủ xe", check.Email ?? string.Empty, check.FullName ?? check.Name ?? "Chủ xe");

            // Cập nhật thời điểm trực tuyến mới nhất
            check.LastLogin = Function.NowVn.ToString("yyyy-MM-dd HH:mm:ss");
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
