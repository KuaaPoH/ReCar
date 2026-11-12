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

        private bool CheckAdmin()
        {
            return Function.IsAdmin(HttpContext.Session);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // Kiểm tra trạng thái đăng nhập Admin
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var id = Function.GetAdminAccountId(HttpContext.Session);
            if (id == 0)
            {
                id = Function.GetAccountId(HttpContext.Session);
            }

            var user = await _context.Customers.FirstOrDefaultAsync(a => a.CustomerId == id);

            if (user == null)
            {
                // Fallback theo email admin trong session
                var adminEmail = Function.GetEmail(HttpContext.Session);
                if (!string.IsNullOrEmpty(adminEmail))
                {
                    user = await _context.Customers.FirstOrDefaultAsync(a => a.Email == adminEmail && a.RoleId == 1);
                }
            }

            if (user == null)
            {
                // Fallback lấy tài khoản admin đầu tiên
                user = await _context.Customers.FirstOrDefaultAsync(a => a.RoleId == 1);
            }

            if (user == null)
            {
                return NotFound("Không tìm thấy tài khoản Quản trị viên.");
            }

            // Đồng bộ lại avatar vào session nếu session đang thiếu
            if (!string.IsNullOrEmpty(user.Avartar))
            {
                Function.SetAdminAvatar(HttpContext.Session, user.Avartar);
            }

            return View(user);
        }

        // Cập nhật thông tin cá nhân & Ảnh đại diện Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfo(string fullName, string username, string email, string? phoneNumber, IFormFile? avatarFile)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var id = Function.GetAdminAccountId(HttpContext.Session);
            if (id == 0) id = Function.GetAccountId(HttpContext.Session);

            var user = await _context.Customers.FirstOrDefaultAsync(u => u.CustomerId == id) 
                       ?? await _context.Customers.FirstOrDefaultAsync(u => u.RoleId == 1);

            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra trùng Email
            if (!string.IsNullOrEmpty(email) && email.Trim().ToLower() != (user.Email ?? "").Trim().ToLower())
            {
                var emailExists = await _context.Customers.AnyAsync(p => p.Email == email.Trim() && p.CustomerId != user.CustomerId);
                if (emailExists)
                {
                    TempData["ErrorMessage"] = "Địa chỉ Email này đã được sử dụng bởi một tài khoản khác!";
                    return RedirectToAction(nameof(Edit));
                }
                user.Email = email.Trim();
            }

            // Cập nhật thông tin
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                user.FullName = fullName.Trim();
                user.Name = fullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                user.Username = username.Trim();
            }

            user.PhoneNumber = phoneNumber?.Trim();
            user.Phone = phoneNumber?.Trim();

            // Xử lý tải lên Avatar mới nếu có
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploadedAvatarPath = Function.UploadAvatar(avatarFile);
                if (!string.IsNullOrEmpty(uploadedAvatarPath))
                {
                    user.Avartar = uploadedAvatarPath;
                    Function.SetAdminAvatar(HttpContext.Session, uploadedAvatarPath);
                }
                else
                {
                    TempData["ErrorMessage"] = "Ảnh đại diện không hợp lệ. Vui lòng chọn tệp .jpg, .png hoặc .webp!";
                }
            }

            await _context.SaveChangesAsync();

            // Cập nhật Session Admin ngay lập tức
            Function.SetAdminSession(HttpContext.Session, user.CustomerId, user.FullName ?? user.Username ?? "Admin", user.Email ?? "", user.Avartar ?? "");
            TempData["SuccessMessage"] = "Cập nhật hồ sơ Quản trị viên thành công!";

            return RedirectToAction(nameof(Edit));
        }

        // Đổi mật khẩu Admin - BẮT BUỘC NHẬP MẬT KHẨU CŨ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                TempData["PasswordError"] = "Vui lòng nhập mật khẩu hiện tại!";
                return RedirectToAction(nameof(Edit));
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["PasswordError"] = "Mật khẩu mới phải có độ dài tối thiểu từ 6 ký tự trở lên!";
                return RedirectToAction(nameof(Edit));
            }

            if (newPassword != confirmPassword)
            {
                TempData["PasswordError"] = "Mật khẩu xác nhận không trùng khớp với mật khẩu mới!";
                return RedirectToAction(nameof(Edit));
            }

            var id = Function.GetAdminAccountId(HttpContext.Session);
            if (id == 0) id = Function.GetAccountId(HttpContext.Session);

            var user = await _context.Customers.FirstOrDefaultAsync(u => u.CustomerId == id)
                       ?? await _context.Customers.FirstOrDefaultAsync(u => u.RoleId == 1);

            if (user == null)
            {
                return NotFound();
            }

            // Xác thực mật khẩu cũ
            if (!Function.VerifyPassword(oldPassword, user.Password))
            {
                TempData["PasswordError"] = "Mật khẩu hiện tại không chính xác. Vui lòng kiểm tra lại!";
                return RedirectToAction(nameof(Edit));
            }

            // Cập nhật mật khẩu mới mã hóa MD5
            user.Password = Function.MD5Password(newPassword);
            await _context.SaveChangesAsync();

            TempData["PasswordSuccess"] = "Đổi mật khẩu bảo mật thành công! Mật khẩu mới đã có hiệu lực.";
            return RedirectToAction(nameof(Edit));
        }

        // Action tương thích ngược
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer userModel, IFormFile? avatarFile)
        {
            return await UpdateInfo(userModel.FullName ?? userModel.Name ?? "", userModel.Username ?? "", userModel.Email ?? "", userModel.PhoneNumber ?? userModel.Phone, avatarFile);
        }
    }
}
