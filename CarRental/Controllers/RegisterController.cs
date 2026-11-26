using CarRental.Models;
using CarRental.Services;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CarRental.Controllers
{
    public class RegisterController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public RegisterController(DbRenalCarContext context, IEmailService emailService, IWebHostEnvironment env)
        {
            _context = context;
            _emailService = emailService;
            _env = env;
        }

        public IActionResult Index(string? ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpGet]
        public IActionResult Quick(string? ReturnUrl = null)
        {
            if (Function.IsLogin(HttpContext.Session))
            {
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("Checkout", "Cart");
            }

            ViewData["ReturnUrl"] = ReturnUrl ?? Url.Action("Checkout", "Cart");
            return View();
        }

        [HttpPost]
        public IActionResult Index(Customer c, string? ReturnUrl = null)
        {
            if (c == null)
            {
                return NotFound();
            }

            // Kiểm tra rỗng để tránh lỗi và yêu cầu nhập đầy đủ thông tin
            if (string.IsNullOrWhiteSpace(c.Name) ||
                string.IsNullOrWhiteSpace(c.Email) ||
                string.IsNullOrWhiteSpace(c.Password))
            {
                Function._MessageEmail = "Vui lòng nhập đầy đủ Họ tên, Email và Mật khẩu.";
                return RedirectToAction("Index", "Register", new { ReturnUrl });
            }

            // Kiểm tra sự tồn tại trong DB
            var check = _context.Customers.FirstOrDefault(m => m.Email == c.Email);
            if (check != null)
            {
                Function._MessageEmail = "Tài khoản tồn tại";
                return RedirectToAction("Index", "Register", new { ReturnUrl });
            }

            // Nếu không tồn tại thì thêm vào CSDL
            Function._MessageEmail = string.Empty;
            c.FullName = string.IsNullOrWhiteSpace(c.FullName) ? c.Name.Trim() : c.FullName.Trim();
            c.Username = c.Email.Trim();
            c.RoleId = 2;
            c.IsActive = true;
            c.Password = Function.MD5Password(c.Password);
            _context.Add(c);
            _context.SaveChanges();
            return RedirectToAction("Index", "Login", new { ReturnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> QuickRegister(
            string? FullName,
            string? Email,
            string? PhoneNumber,
            string? Address,
            string? LicenseNumber,
            IFormFile? LicenseFrontFile,
            IFormFile? LicenseBackFile)
        {
            var fullName = FullName?.Trim() ?? string.Empty;
            var email = Email?.Trim() ?? string.Empty;
            var phone = PhoneNumber?.Trim() ?? string.Empty;
            var address = Address?.Trim() ?? string.Empty;
            var licenseNo = LicenseNumber?.Trim();

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ Họ tên, Email, Số điện thoại và Địa chỉ." });
            }

            if (!email.Contains('@') || !email.Contains('.'))
            {
                return Json(new { success = false, message = "Địa chỉ Email không hợp lệ." });
            }

            var existed = await _context.Customers.AnyAsync(c => c.Email != null && c.Email.ToLower() == email.ToLower());
            if (existed)
            {
                return Json(new
                {
                    success = false,
                    emailExists = true,
                    email,
                    message = "Email này đã được đăng ký tài khoản ReCar. Vui lòng chuyển sang đăng nhập!"
                });
            }

            var tempPassword = GenerateRandomPassword(8);
            var (mailOk, mailMsg) = await _emailService.SendQuickRegisterPasswordAsync(email, fullName, tempPassword);
            if (!mailOk)
            {
                return Json(new { success = false, message = mailMsg });
            }

            string? frontPath = await SaveLicenseImageAsync(LicenseFrontFile, "front");
            string? backPath = await SaveLicenseImageAsync(LicenseBackFile, "back");

            var customer = new Customer
            {
                FullName = fullName,
                Name = fullName,
                Username = email,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                LicenseNumber = string.IsNullOrWhiteSpace(licenseNo) ? null : licenseNo,
                LicenseImage = frontPath,
                LicenseBackImage = backPath,
                Password = Function.MD5Password(tempPassword),
                RoleId = 2,
                IsActive = true
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                email,
                message = $"Đã gửi thông tin tài khoản và mật khẩu về email {email}. Vui lòng kiểm tra hộp thư để đăng nhập và tiếp tục thanh toán!"
            });
        }

        private async Task<string?> SaveLicenseImageAsync(IFormFile? file, string side)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext))
            {
                return null;
            }

            var folder = Path.Combine(_env.WebRootPath, "files", "license");
            Directory.CreateDirectory(folder);

            var fileName = $"gplx_{side}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
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
