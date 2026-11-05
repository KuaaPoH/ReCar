using CarRental.Hubs;
using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CarOwnersController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly IHubContext<ReCarHub> _hubContext;

        public CarOwnersController(DbRenalCarContext context, IHubContext<ReCarHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        private bool CheckAdmin()
        {
            return Function.IsAdmin(HttpContext.Session);
        }

        // GET: /Admin/CarOwners/Pending (Danh sách hồ sơ đăng ký chủ xe chờ duyệt)
        public async Task<IActionResult> Pending(string? q)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var query = _context.CarOwnerRegistrations.AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                q = q.Trim().ToLower();
                query = query.Where(r => r.FullName.ToLower().Contains(q) ||
                                         r.Phone.Contains(q) ||
                                         r.Email.ToLower().Contains(q) ||
                                         r.IdCard.Contains(q) ||
                                         r.LicenseNumber.Contains(q));
            }

            // Ưu tiên hiển thị hồ sơ Pending lên đầu, sau đó sắp xếp theo thời gian mới nhất
            var list = await query
                .OrderBy(r => r.Status == "Pending" ? 0 : (r.Status == "Rejected" ? 1 : 2))
                .ThenByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.PendingCount = await _context.CarOwnerRegistrations.CountAsync(r => r.Status == "Pending");
            ViewBag.SearchQuery = q;

            return View(list);
        }

        // POST: /Admin/CarOwners/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var reg = await _context.CarOwnerRegistrations.FindAsync(id);
            if (reg == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
            }

            // 1. Cập nhật trạng thái hồ sơ thành Approved
            reg.Status = "Approved";
            reg.ApprovedAt = DateTime.Now;
            reg.ApprovedBy = Function.GetAdminUserName(HttpContext.Session);
            reg.RejectReason = null;

            // 2. Cập nhật quyền Chủ xe (RoleId = 3) trực tiếp trên Customer
            var customer = await _context.Customers.FindAsync(reg.CustomerId)
                        ?? await _context.Customers.FirstOrDefaultAsync(c => c.Email == reg.Email);
            if (customer != null)
            {
                customer.RoleId = 3; // Nâng cấp lên Chủ xe
                customer.IsActive = true;
                customer.FullName = reg.FullName;
                customer.PhoneNumber = reg.Phone;
                customer.Idcard = reg.IdCard;
                customer.LicenseNumber = reg.LicenseNumber;
                customer.Bank = reg.BankName;
                customer.AccountNumber = reg.AccountNumber;
                customer.CompanyName = reg.AccountHolder;
            }

            await _context.SaveChangesAsync();

            // Gửi thông báo Real-time cho người dùng
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveOwnerApprovedNotification", new
                {
                    customerId = reg.CustomerId,
                    email = reg.Email,
                    fullName = reg.FullName,
                    message = "Chúc mừng! Hồ sơ đối tác Chủ xe của bạn đã được phê duyệt thành công. Bạn có thể truy cập Trang chủ xe ngay bây giờ."
                });
            }
            catch { }

            return Json(new { success = true, message = $"Đã phê duyệt thành công đối tác: {reg.FullName}!" });
        }

        // POST: /Admin/CarOwners/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var reg = await _context.CarOwnerRegistrations.FindAsync(id);
            if (reg == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
            }

            reg.Status = "Rejected";
            reg.RejectReason = !string.IsNullOrWhiteSpace(reason) ? reason.Trim() : "Thông tin chưa đáp ứng tiêu chuẩn quy định của Sàn ReCar.";
            reg.ApprovedAt = DateTime.Now;
            reg.ApprovedBy = Function.GetAdminUserName(HttpContext.Session);

            await _context.SaveChangesAsync();

            // Gửi thông báo Real-time cho người dùng
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveOwnerRejectedNotification", new
                {
                    customerId = reg.CustomerId,
                    email = reg.Email,
                    fullName = reg.FullName,
                    reason = reg.RejectReason
                });
            }
            catch { }

            return Json(new { success = true, message = $"Đã từ chối hồ sơ của: {reg.FullName}." });
        }
    }
}
