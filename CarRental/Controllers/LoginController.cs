using CarRental.Models;
using CarRental.Services;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CarRental.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly IEmailService _emailService;
        public LoginController(DbRenalCarContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
                return RedirectToAction("Index", "Login", new { ReturnUrl });
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
                    return RedirectToAction("Index", "Login", new { ReturnUrl });
                }

                // Kiểm tra trạng thái khóa tài khoản
                if (!check.IsActive)
                {
                    Function.SetMessage(HttpContext.Session, "LOCKED");
                    return RedirectToAction("Index", "Login", new { ReturnUrl });
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
                Function.SetAvatar(HttpContext.Session, check.Avartar ?? string.Empty);

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

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string? email)
        {
            var cleanEmail = email?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cleanEmail) || !cleanEmail.Contains('@'))
            {
                return Json(new { success = false, message = "Vui lòng nhập địa chỉ Email hợp lệ." });
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email != null && c.Email.ToLower() == cleanEmail.ToLower());

            if (customer == null)
            {
                return Json(new { success = false, message = "Không tìm thấy tài khoản nào ứng với địa chỉ Email này." });
            }

            if (!customer.IsActive)
            {
                return Json(new { success = false, message = "Tài khoản này hiện đang bị khóa. Vui lòng liên hệ quản trị viên." });
            }

            var newPassword = GenerateRandomPassword(8);
            var fullName = !string.IsNullOrWhiteSpace(customer.FullName) ? customer.FullName : (customer.Name ?? "Quý khách");

            var (mailOk, mailMsg) = await _emailService.SendResetPasswordAsync(customer.Email!, fullName, newPassword);
            if (!mailOk)
            {
                return Json(new { success = false, message = mailMsg });
            }

            customer.Password = Function.MD5Password(newPassword);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                email = customer.Email,
                message = $"Mật khẩu mới đã được gửi về email {customer.Email}. Vui lòng kiểm tra hộp thư để đăng nhập!"
            });
        }

        private static string GenerateRandomPassword(int length)
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "23456789";
            const string all = upper + lower + digits;

            var chars = new char[length];
            chars[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
            chars[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
            chars[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];

            for (int i = 3; i < length; i++)
            {
                chars[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
            }

            for (int i = length - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            return new string(chars);
        }
    }
}
