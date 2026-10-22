using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CarRental.Hubs
{
    public class ReCarHub : Hub
    {
        private readonly ILogger<ReCarHub> _logger;

        public ReCarHub(ILogger<ReCarHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Đăng ký client vào nhóm tương ứng (User_{AccountId}, Role_Admin, Role_CarOwner, Car_{CarId})
        /// </summary>
        public async Task RegisterClient(int? accountId, string? role, int? carId)
        {
            if (accountId.HasValue && accountId.Value > 0)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{accountId.Value}");
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role.Trim()}");
            }

            if (carId.HasValue && carId.Value > 0)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Car_{carId.Value}");
            }
        }

        /// <summary>
        /// Gửi thông báo đang gõ tin nhắn (Typing Indicator)
        /// </summary>
        public async Task SendTyping(int? carId, int? receiverId, string senderName, bool isTyping)
        {
            if (receiverId.HasValue && receiverId.Value > 0)
            {
                await Clients.Group($"User_{receiverId.Value}").SendAsync("UserTyping", new
                {
                    carId = carId,
                    senderName = senderName,
                    isTyping = isTyping
                });
            }
            else if (carId.HasValue && carId.Value > 0)
            {
                await Clients.Group($"Car_{carId.Value}").SendAsync("UserTyping", new
                {
                    carId = carId,
                    senderName = senderName,
                    isTyping = isTyping
                });
            }
        }
    }
}
