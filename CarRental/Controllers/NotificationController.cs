using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Services;
using CarRental.Utilities;

namespace CarRental.Controllers
{
    public class NotificationController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly INotificationService _notificationService;

        public NotificationController(DbRenalCarContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET: /Notification/GetNotifications
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                int accountId = Function.GetAccountId(HttpContext.Session);
                int carOwnerId = Function.GetCarOwnerAccountId(HttpContext.Session);
                bool isAdmin = Function.IsAdmin(HttpContext.Session);
                int currentUserId = carOwnerId > 0 ? carOwnerId : accountId;

                IQueryable<Notification> query;

                if (isAdmin)
                {
                    query = _context.Notifications.Where(n => n.Role == "Admin" || (currentUserId > 0 && n.UserId == currentUserId) || n.Role == "All");
                }
                else if (carOwnerId > 0)
                {
                    query = _context.Notifications.Where(n => n.UserId == carOwnerId || (n.Role == "CarOwner" && n.UserId == null) || n.Role == "All");
                }
                else if (accountId > 0)
                {
                    query = _context.Notifications.Where(n => n.UserId == accountId || (n.Role == "Customer" && n.UserId == null) || n.Role == "All");
                }
                else
                {
                    return Json(new { success = true, unreadCount = 0, notifications = new List<object>() });
                }

                int unreadCount = await query.CountAsync(n => !n.IsRead);

                var list = await query
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(15)
                    .ToListAsync();

                var formattedList = list.Select(n => new
                {
                    notificationId = n.NotificationId,
                    title = n.Title,
                    content = n.Content,
                    type = n.Type ?? "System",
                    linkUrl = string.IsNullOrWhiteSpace(n.LinkUrl) ? "#" : n.LinkUrl,
                    isRead = n.IsRead,
                    timeAgo = FormatRelativeTime(n.CreatedAt),
                    createdAt = n.CreatedAt.ToString("HH:mm dd/MM/yyyy")
                }).ToList();

                return Json(new
                {
                    success = true,
                    unreadCount = unreadCount,
                    notifications = formattedList
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Notification/MarkRead
        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            try
            {
                var notif = await _context.Notifications.FindAsync(id);
                if (notif != null && !notif.IsRead)
                {
                    notif.IsRead = true;
                    await _context.SaveChangesAsync();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Notification/MarkAllRead
        [HttpPost]
        public async Task<IActionResult> MarkAllRead()
        {
            try
            {
                int accountId = Function.GetAccountId(HttpContext.Session);
                int carOwnerId = Function.GetCarOwnerAccountId(HttpContext.Session);
                bool isAdmin = Function.IsAdmin(HttpContext.Session);
                int currentUserId = carOwnerId > 0 ? carOwnerId : accountId;

                IQueryable<Notification> query;

                if (isAdmin)
                {
                    query = _context.Notifications.Where(n => (n.Role == "Admin" || (currentUserId > 0 && n.UserId == currentUserId) || n.Role == "All") && !n.IsRead);
                }
                else if (carOwnerId > 0)
                {
                    query = _context.Notifications.Where(n => (n.UserId == carOwnerId || (n.Role == "CarOwner" && n.UserId == null) || n.Role == "All") && !n.IsRead);
                }
                else if (accountId > 0)
                {
                    query = _context.Notifications.Where(n => (n.UserId == accountId || (n.Role == "Customer" && n.UserId == null) || n.Role == "All") && !n.IsRead);
                }
                else
                {
                    return Json(new { success = true });
                }

                var unreadList = await query.ToListAsync();
                foreach (var item in unreadList)
                {
                    item.IsRead = true;
                }
                await _context.SaveChangesAsync();

                return Json(new { success = true, updatedCount = unreadList.Count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Notification/Broadcast (Phát thông báo hệ thống - Chỉ dành cho Admin)
        [HttpPost]
        public async Task<IActionResult> Broadcast(string title, string content, string role = "All", string? linkUrl = null)
        {
            if (!Function.IsAdmin(HttpContext.Session))
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này." });
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Tiêu đề và nội dung thông báo không được để trống." });
            }

            await _notificationService.CreateNotificationAsync(null, role, title.Trim(), content.Trim(), "System", linkUrl);
            return Json(new { success = true, message = "Đã phát thông báo thành công." });
        }

        private static string FormatRelativeTime(DateTime dt)
        {
            var span = DateTime.Now - dt;
            if (span.TotalSeconds < 60) return "Vừa xong";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} phút trước";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} giờ trước";
            if (span.TotalDays < 2) return $"Hôm qua {dt:HH:mm}";
            return dt.ToString("dd/MM/yyyy HH:mm");
        }
    }
}

