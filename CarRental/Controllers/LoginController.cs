using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbRenalCarContext _context;
        public LoginController (DbRenalCarContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index(string? ReturnUrl = null)
        {
            // Nếu đã đăng nhập thì không cho vào lại trang Login, chuyển thẳng về Trang chủ
            if (Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Home");
            }

            // Không cho trình duyệt cache trang Login để tránh quay lại bằng nút Back
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(Customer c, string? ReturnUrl = null)
        {
            if (c == null)
            {
                return NotFound();
            }
            
            // Kiểm tra rỗng để tránh lỗi khi chưa nhập gì đã bấm Login
            if (string.IsNullOrWhiteSpace(c.Email) || string.IsNullOrWhiteSpace(c.Password))
            {
                Function.SetMessage(HttpContext.Session, "Vui lòng nhập đầy đủ tài khoản và mật khẩu.");
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                // Mã hoá mật khẩu trước khi kiểm tra
                string p = Function.MD5Password(c.Password);
                
                // Kiểm tra tài khoản có tồn tại trong bảng Customers (bằng Email hoặc Username)
                var check = await _context.Customers
                    .Include(m => m.Role)
                    .FirstOrDefaultAsync(m => (m.Email == c.Email || m.Username == c.Email) && m.Password == p);

                if (check == null)
                {
                    Function.SetMessage(HttpContext.Session, "Sai thông tin đăng nhập");
                    return RedirectToAction("Index", "Login");
                }

                // Kiểm tra trạng thái khóa tài khoản
                if (!check.IsActive)
                {
                    Function.SetMessage(HttpContext.Session, "LOCKED");
                    return RedirectToAction("Index", "Login");
                }

                // Cập nhật thời điểm đăng nhập mới nhất
                check.LastLogin = Function.NowVn.ToString("yyyy-MM-dd HH:mm:ss");
                await _context.SaveChangesAsync();

                // Đăng nhập thành công - lưu thông tin vào Session với CustomerId duy nhất
                Function.SetMessage(HttpContext.Session, string.Empty);
                Function.SetAccountId(HttpContext.Session, check.CustomerId);
                Function.SetUserName(HttpContext.Session, !string.IsNullOrEmpty(check.FullName) ? check.FullName : (check.Name ?? string.Empty));
                Function.SetEmail(HttpContext.Session, check.Email ?? string.Empty);
                Function.SetAddress(HttpContext.Session, check.Address ?? string.Empty);
                Function.SetPhone(HttpContext.Session, check.PhoneNumber ?? string.Empty);

                // Nếu tài khoản có quyền Chủ xe (RoleId = 3), thiết lập luôn CarOwner Session đồng bộ
                if (check.RoleId == 3)
                {
                    Function.SetCarOwnerSession(HttpContext.Session, check.CustomerId, check.Username ?? check.Name ?? "Chủ xe", check.Email ?? string.Empty, check.FullName ?? check.Name ?? "Chủ xe");
                }
                else
                {
                    Function.SetCarOwnerAccountId(HttpContext.Session, 0);
                }

                // Nếu có ReturnUrl hợp lệ thì quay lại đúng trang trước đó, ngược lại về Trang chủ
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && !ReturnUrl.StartsWith("/Login", StringComparison.OrdinalIgnoreCase))
                {
                    return Redirect(ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
