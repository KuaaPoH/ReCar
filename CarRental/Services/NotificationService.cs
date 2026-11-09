using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using CarRental.Hubs;
using CarRental.Models;

namespace CarRental.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ReCarHub> _hubContext;

        public NotificationService(IServiceScopeFactory scopeFactory, IHubContext<ReCarHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        public async Task CreateNotificationAsync(int? userId, string? role, string title, string content, string type = "System", string? linkUrl = null)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DbRenalCarContext>();

                var notif = new Notification
                {
                    UserId = userId,
                    Role = role,
                    Title = title,
                    Content = content,
                    Type = type,
                    LinkUrl = linkUrl,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                context.Notifications.Add(notif);
                await context.SaveChangesAsync();

                var payload = new
                {
                    notificationId = notif.NotificationId,
                    title = notif.Title,
                    content = notif.Content,
                    type = notif.Type,
                    linkUrl = notif.LinkUrl,
                    createdAt = notif.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                    timeAgo = "Vừa xong"
                };

                if (userId.HasValue && userId.Value > 0)
                {
                    await _hubContext.Clients.Group($"User_{userId.Value}").SendAsync("ReceiveNotification", payload);
                }
                else if (!string.IsNullOrWhiteSpace(role) && role != "All")
                {
                    await _hubContext.Clients.Group($"Role_{role.Trim()}").SendAsync("ReceiveNotification", payload);
                }
                else
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", payload);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NotificationService Exception: {ex.Message}");
            }
        }
    }
}

