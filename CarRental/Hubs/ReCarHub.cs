using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace CarRental.Hubs
{
    public class ReCarHub : Hub
    {
        private readonly ILogger<ReCarHub> _logger;

        // Quản lý trạng thái trực tuyến theo thời gian thực (Realtime Presence)
        private static readonly ConcurrentDictionary<int, HashSet<string>> _onlineUsers = new();
        private static readonly ConcurrentDictionary<string, int> _connectionToUser = new();

        public ReCarHub(ILogger<ReCarHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Kiểm tra xem một AccountId có đang kết nối trực tuyến (mở tab trình duyệt) hay không
        /// </summary>
        public static bool IsUserOnline(int? accountId)
        {
            if (!accountId.HasValue || accountId.Value <= 0) return false;
            return _onlineUsers.TryGetValue(accountId.Value, out var conns) && conns != null && conns.Count > 0;
        }

        private static void MarkOnline(int accountId, string connectionId)
        {
            _connectionToUser[connectionId] = accountId;
            _onlineUsers.AddOrUpdate(
                accountId,
                _ => new HashSet<string> { connectionId },
                (_, set) =>
                {
                    lock (set)
                    {
                        set.Add(connectionId);
                    }
                    return set;
                });
        }

        private static void MarkOffline(string connectionId)
        {
            if (_connectionToUser.TryRemove(connectionId, out int accountId))
            {
                if (_onlineUsers.TryGetValue(accountId, out var set))
                {
                    lock (set)
                    {
                        set.Remove(connectionId);
                    }
                    if (set.Count == 0)
                    {
                        _onlineUsers.TryRemove(accountId, out _);
                    }
                }
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            MarkOffline(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Đăng ký client vào nhóm tương ứng (User_{AccountId}, Role_Admin, Role_CarOwner, Car_{CarId})
        /// và ghi nhận trạng thái trực tuyến
        /// </summary>
        public async Task RegisterClient(int? accountId, string? role, int? carId)
        {
            if (accountId.HasValue && accountId.Value > 0)
            {
                MarkOnline(accountId.Value, Context.ConnectionId);
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
