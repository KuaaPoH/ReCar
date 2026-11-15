using System.Net;
using System.Net.Mail;
using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactsController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly IConfiguration _config;

        public ContactsController(DbRenalCarContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private bool IsAdminAuthenticated() => Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session);

        public async Task<IActionResult> Index(string filter = "all")
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var allContacts = await _context.Contacts
                .OrderBy(c => c.IsRead ?? 0)
                .ThenByDescending(c => c.CreatedDate)
                .ThenByDescending(c => c.ContactId)
                .ToListAsync();

            ViewBag.TotalCount = allContacts.Count;
            ViewBag.UnreadCount = allContacts.Count(c => (c.IsRead ?? 0) == 0);
            ViewBag.RepliedCount = allContacts.Count(c => (c.IsRead ?? 0) == 1);
            ViewBag.Filter = filter;

            var filtered = filter switch
            {
                "unread" => allContacts.Where(c => (c.IsRead ?? 0) == 0).ToList(),
                "replied" => allContacts.Where(c => (c.IsRead ?? 0) == 1).ToList(),
                _ => allContacts
            };

            return View(filtered);
        }

        [HttpPost]
        public async Task<IActionResult> ReplyEmail(int id, string subject, string replyContent)
        {
            if (!IsAdminAuthenticated())
            {
                return Json(new { success = false, message = "Phiên đăng nhập quản trị đã hết hạn." });
            }

            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.ContactId == id);
            if (contact == null)
            {
                return Json(new { success = false, message = "Không tìm thấy liên hệ." });
            }

            if (string.IsNullOrWhiteSpace(contact.Email))
            {
                return Json(new { success = false, message = "Liên hệ này không có địa chỉ Email." });
            }

            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(replyContent))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ tiêu đề và nội dung phản hồi." });
            }

            bool smtpSent = false;
            string smtpError = "";
            var smtpHost = _config["Smtp:Host"];
            var smtpUser = _config["Smtp:Username"];
            var smtpPass = _config["Smtp:Password"];
            int smtpPort = _config.GetValue<int>("Smtp:Port", 587);
            bool smtpSsl = _config.GetValue<bool>("Smtp:EnableSsl", true);

            if (!string.IsNullOrWhiteSpace(smtpHost) && !string.IsNullOrWhiteSpace(smtpUser) && !string.IsNullOrWhiteSpace(smtpPass))
            {
                try
                {
                    using var client = new SmtpClient(smtpHost, smtpPort)
                    {
                        Credentials = new NetworkCredential(smtpUser, smtpPass),
                        EnableSsl = smtpSsl
                    };
                    using var mail = new MailMessage(smtpUser, contact.Email.Trim(), subject.Trim(), replyContent.Trim());
                    await client.SendMailAsync(mail);
                    smtpSent = true;
                }
                catch (Exception ex)
                {
                    smtpError = ex.Message;
                }
            }

            string adminName = Function.GetAdminUserName(HttpContext.Session);
            if (string.IsNullOrWhiteSpace(adminName)) adminName = "Admin";

            contact.IsRead = 1;
            contact.ModifiedDate = Function.NowVn;
            contact.ModifiedBy = smtpSent
                ? $"Đã gửi Email ({adminName}): {subject.Trim()}"
                : $"Đã xử lý phản hồi ({adminName}): {subject.Trim()}";
            await _context.SaveChangesAsync();

            string mailtoUrl = $"mailto:{Uri.EscapeDataString(contact.Email.Trim())}?subject={Uri.EscapeDataString(subject.Trim())}&body={Uri.EscapeDataString(replyContent.Trim())}";

            return Json(new
            {
                success = true,
                smtpSent,
                smtpError,
                mailtoUrl,
                message = smtpSent
                    ? $"Đã gửi email phản hồi thành công tới {contact.Email}!"
                    : "Đã đánh dấu đã phản hồi! Do chưa cấu hình SMTP trên server, bạn có thể bấm nút bên dưới để mở ứng dụng Email gửi trực tiếp cho khách."
            });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return Json(new { success = false, message = "Không có quyền." });
            }

            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.ContactId == id);
            if (contact == null) return Json(new { success = false, message = "Không tìm thấy." });

            string adminName = Function.GetAdminUserName(HttpContext.Session);
            contact.IsRead = 1;
            contact.ModifiedDate = Function.NowVn;
            if (string.IsNullOrWhiteSpace(contact.ModifiedBy))
            {
                contact.ModifiedBy = $"Đã đọc ({(string.IsNullOrWhiteSpace(adminName) ? "Admin" : adminName)})";
            }
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return Json(new { success = false, message = "Không có quyền." });
            }

            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.ContactId == id);
            if (contact == null) return Json(new { success = false, message = "Không tìm thấy." });

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
