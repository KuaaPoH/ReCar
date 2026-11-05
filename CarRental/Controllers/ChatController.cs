using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using CarRental.Models;
using CarRental.Hubs;
using CarRental.Utilities;
using System.Text.Json;

namespace CarRental.Controllers
{
    public class ChatController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly IHubContext<ReCarHub> _hubContext;

        public ChatController(DbRenalCarContext context, IHubContext<ReCarHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Xác định Customer thực tế trong bảng Customers đang đăng nhập (Khách hàng, Chủ xe hoặc Admin)
        /// Thống nhất 100% người dùng vào bảng Customers duy nhất.
        /// </summary>
        private async Task<Customer?> ResolveCurrentCustomerAsync(bool asOwner = false, int? explicitOwnerId = null)
        {
            // 1. Session CarOwner (nếu thao tác ở giao diện chủ xe)
            if (asOwner)
            {
                int ownerId = Function.GetCarOwnerAccountId(HttpContext.Session);
                if (ownerId > 0)
                {
                    var ownerCust = await _context.Customers.FindAsync(ownerId);
                    if (ownerCust != null && ownerCust.RoleId == 3) return ownerCust;
                }

                // Explicit owner id nếu client khu vực chủ xe gửi lên
                if (explicitOwnerId.HasValue && explicitOwnerId.Value > 0)
                {
                    var exp = await _context.Customers.FindAsync(explicitOwnerId.Value);
                    if (exp != null && exp.RoleId == 3) return exp;
                }

                // Fallback nếu là khu vực CarOwner mà chưa login hoặc ID không phải chủ xe
                var fallback = await _context.Customers.FirstOrDefaultAsync(c => c.RoleId == 3 && c.CustomerId == 1022) 
                             ?? await _context.Customers.FirstOrDefaultAsync(c => c.RoleId == 3);
                if (fallback != null) return fallback;
            }

            // 2. Session User / Customer (sàn khách hàng)
            int custId = Function.GetAccountId(HttpContext.Session);
            if (custId > 0)
            {
                var cust = await _context.Customers.FindAsync(custId);
                if (cust != null) return cust;
            }

            int userAccId = Function.GetUserAccountId(HttpContext.Session);
            if (userAccId > 0)
            {
                var cust = await _context.Customers.FindAsync(userAccId);
                if (cust != null) return cust;
            }

            // 3. Fallback theo Email trong Session
            string email = Function.GetEmail(HttpContext.Session);
            if (string.IsNullOrEmpty(email)) email = Function.GetCarOwnerEmail(HttpContext.Session);
            if (!string.IsNullOrEmpty(email))
            {
                var cust = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
                if (cust != null) return cust;
            }

            return null;
        }

        public class SendMessageRequest
        {
            public int? CarId { get; set; }
            public int? OwnerId { get; set; }
            public int? ReceiverId { get; set; }
            public string MessageText { get; set; } = string.Empty;
            public string? SenderName { get; set; }
            public string? CarName { get; set; }
            public bool AsOwner { get; set; } = false;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest req)
        {
            // 1. Kiểm tra người gửi (TUYỆT ĐỐI không lấy req.OwnerId để gán cho người gửi)
            var senderCust = await ResolveCurrentCustomerAsync(req?.AsOwner == true);
            if (senderCust == null)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập tài khoản để gửi tin nhắn!" });
            }

            if (req == null || string.IsNullOrWhiteSpace(req.MessageText))
            {
                return Json(new { success = false, message = "Nội dung tin nhắn không được để trống!" });
            }

            int currentUserId = senderCust.CustomerId;
            string senderName = !string.IsNullOrWhiteSpace(senderCust.FullName) ? senderCust.FullName : (senderCust.Name ?? "Khách hàng");
            string senderRole = senderCust.RoleId == 3 ? "CarOwner" : (senderCust.RoleId == 1 ? "Admin" : "Customer");

            // 2. Xác định người nhận
            // Nếu người gửi là khách hàng và có req.CarId: người nhận chính là chủ sở hữu của chiếc xe đó
            int receiverId = req.ReceiverId ?? 0;
            if (receiverId <= 0 && req.OwnerId.HasValue && req.OwnerId.Value != currentUserId)
            {
                receiverId = req.OwnerId.Value;
            }

            if (req.CarId.HasValue && req.CarId.Value > 0)
            {
                var car = await _context.Cars.FindAsync(req.CarId.Value);
                if (car != null && car.OwnerId.HasValue)
                {
                    // Nếu người gửi không phải chủ xe thì người nhận chắc chắn là chủ xe
                    if (car.OwnerId.Value != currentUserId)
                    {
                        receiverId = car.OwnerId.Value;
                    }
                }
            }

            // Nếu vẫn chưa có người nhận nhưng có req.OwnerId khác với người gửi
            if (receiverId <= 0 && req.OwnerId.HasValue && req.OwnerId.Value != currentUserId)
            {
                receiverId = req.OwnerId.Value;
            }

            if (receiverId <= 0)
            {
                return Json(new { success = false, message = "Không xác định được chủ xe/người nhận tin nhắn!" });
            }

            if (receiverId == currentUserId)
            {
                return Json(new { 
                    success = false, 
                    message = "Bạn không thể tự gửi tin nhắn cho chính mình! Tài khoản hiện tại đang là chủ sở hữu của chiếc xe này. Để thử nghiệm nhắn tin qua lại, vui lòng mở một cửa sổ Ẩn danh (Ctrl+Shift+N) đăng nhập bằng tài khoản Khách thuê riêng biệt." 
                });
            }

            var receiverCust = await _context.Customers.FindAsync(receiverId);
            string receiverName = !string.IsNullOrWhiteSpace(receiverCust?.FullName) 
                ? receiverCust.FullName 
                : (!string.IsNullOrWhiteSpace(receiverCust?.Name) ? receiverCust.Name : "Đối tác ReCar");

            // 3. Lưu tin nhắn vào CSDL
            var newMsg = new ChatMessage
            {
                CarId = req.CarId,
                SenderId = currentUserId,
                SenderName = senderName,
                SenderRole = senderRole,
                ReceiverId = receiverId,
                ReceiverName = receiverName,
                MessageText = req.MessageText.Trim(),
                SentAt = Function.NowVn,
                IsRead = false
            };

            _context.ChatMessages.Add(newMsg);
            await _context.SaveChangesAsync();

            // 4. Phát tín hiệu thời gian thực SignalR tới nhóm người nhận (User_{receiverId})
            try
            {
                var payload = new
                {
                    messageId = newMsg.MessageId,
                    carId = newMsg.CarId,
                    carName = req.CarName,
                    senderId = newMsg.SenderId,
                    senderName = newMsg.SenderName,
                    senderRole = newMsg.SenderRole,
                    receiverId = newMsg.ReceiverId,
                    receiverName = newMsg.ReceiverName,
                    messageText = newMsg.MessageText,
                    sentAt = newMsg.SentAt.ToString("HH:mm - dd/MM"),
                    isFromOwner = (senderRole == "CarOwner")
                };

                await _hubContext.Clients.Group($"User_{receiverId}").SendAsync("ReceiveChatMessage", payload);
            }
            catch (Exception hubEx)
            {
                Console.WriteLine($"SignalR Chat Broadcast Error: {hubEx.Message}");
            }

            return Json(new
            {
                success = true,
                message = new
                {
                    messageId = newMsg.MessageId,
                    senderId = newMsg.SenderId,
                    senderName = newMsg.SenderName,
                    senderRole = newMsg.SenderRole,
                    messageText = newMsg.MessageText,
                    sentAt = newMsg.SentAt.ToString("HH:mm")
                }
            });
        }

        public class TypingRequest
        {
            public int? CarId { get; set; }
            public int? ReceiverId { get; set; }
            public string SenderName { get; set; } = string.Empty;
            public bool IsTyping { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SendRealtimeTyping([FromBody] TypingRequest req)
        {
            if (req == null) return Json(new { success = false });

            try
            {
                if (req.ReceiverId.HasValue && req.ReceiverId.Value > 0)
                {
                    await _hubContext.Clients.Group($"User_{req.ReceiverId.Value}").SendAsync("UserTyping", new
                    {
                        carId = req.CarId,
                        senderName = req.SenderName,
                        isTyping = req.IsTyping
                    });
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int? carId, int? partnerId, int? ownerId, bool asOwner = false)
        {
            // Kiểm tra đăng nhập (chỉ dùng explicit ownerId khi asOwner == true)
            var currentCust = await ResolveCurrentCustomerAsync(asOwner, asOwner ? ownerId : null);
            if (currentCust == null)
            {
                return Json(new { success = false, requireLogin = true, messages = new List<object>() });
            }

            int currentUserId = currentCust.CustomerId;

            int targetId = partnerId ?? 0;
            if (targetId <= 0 && ownerId.HasValue && ownerId.Value != currentUserId)
            {
                targetId = ownerId.Value;
            }

            if (carId.HasValue && carId.Value > 0)
            {
                var car = await _context.Cars.FindAsync(carId.Value);
                if (car != null && car.OwnerId.HasValue)
                {
                    if (car.OwnerId.Value != currentUserId && targetId <= 0)
                    {
                        targetId = car.OwnerId.Value;
                    }
                }
            }

            if (targetId == currentUserId)
            {
                targetId = 0;
            }

            var query = _context.ChatMessages.AsQueryable();

            if (targetId > 0)
            {
                query = query.Where(m =>
                    (m.SenderId == currentUserId && m.ReceiverId == targetId) ||
                    (m.SenderId == targetId && m.ReceiverId == currentUserId)
                );
            }
            else
            {
                query = query.Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId);
            }

            if (carId.HasValue && carId.Value > 0)
            {
                query = query.Where(m => m.CarId == carId.Value || m.CarId == null);
            }

            // Đánh dấu các tin nhắn đối phương gửi cho mình là Đã đọc
            if (targetId > 0 && targetId != currentUserId)
            {
                var unreadIncoming = await _context.ChatMessages
                    .Where(m => m.ReceiverId == currentUserId && m.SenderId == targetId && !m.IsRead)
                    .ToListAsync();
                if (unreadIncoming.Any())
                {
                    foreach (var u in unreadIncoming)
                    {
                        u.IsRead = true;
                    }
                    await _context.SaveChangesAsync();
                }
            }

            int remainingUnread = await _context.ChatMessages
                .Where(m => m.ReceiverId == currentUserId && !m.IsRead)
                .CountAsync();

            var messages = await query
                .OrderBy(m => m.SentAt)
                .Take(50)
                .Select(m => new
                {
                    messageId = m.MessageId,
                    carId = m.CarId,
                    senderId = m.SenderId,
                    senderName = m.SenderName,
                    senderRole = m.SenderRole,
                    receiverId = m.ReceiverId,
                    receiverName = m.ReceiverName,
                    messageText = m.MessageText,
                    isMe = (m.SenderId == currentUserId),
                    isFromOwner = (m.SenderRole == "CarOwner"),
                    sentAt = m.SentAt.ToString("HH:mm - dd/MM")
                })
                .ToListAsync();

            return Json(new { success = true, currentUserId = currentUserId, unreadCount = remainingUnread, messages });
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentConversations(bool asOwner = false, int? ownerId = null)
        {
            var currentCust = await ResolveCurrentCustomerAsync(asOwner, asOwner ? ownerId : null);
            if (currentCust == null)
            {
                return Json(new { success = true, unreadCount = 0, conversations = new List<object>() });
            }

            int currentUserId = currentCust.CustomerId;

            // Danh sách xe do tài khoản này sở hữu (nếu là chủ xe)
            var myCarIds = await _context.Cars
                .Where(c => c.OwnerId == currentUserId)
                .Select(c => c.CarId)
                .ToListAsync();

            // Lấy tất cả tin nhắn liên quan: mình gửi, mình nhận HOẶC khách gửi vào xe của mình
            var recentMessages = await _context.ChatMessages
                .Include(m => m.Car)
                .Where(m => m.SenderId == currentUserId || 
                            m.ReceiverId == currentUserId || 
                            (m.CarId.HasValue && myCarIds.Contains(m.CarId.Value)))
                .OrderByDescending(m => m.SentAt)
                .Take(60)
                .ToListAsync();

            // Nhóm theo đối tác trò chuyện (PartnerId)
            var partnerIds = recentMessages
                .Select(m => (m.SenderId == currentUserId) ? (m.ReceiverId ?? 0) : (m.SenderId ?? 0))
                .Where(id => id > 0 && id != currentUserId)
                .Distinct()
                .ToList();

            var partnerDict = await _context.Customers
                .Where(c => partnerIds.Contains(c.CustomerId))
                .ToDictionaryAsync(c => c.CustomerId, c => !string.IsNullOrEmpty(c.FullName) ? c.FullName : (c.Name ?? "Khách hàng"));

            var grouped = recentMessages
                .GroupBy(m => {
                    if (m.SenderId == currentUserId) return m.ReceiverId ?? 0;
                    if (m.ReceiverId == currentUserId) return m.SenderId ?? 0;
                    return m.SenderId ?? 0;
                })
                .Where(g => g.Key > 0 && g.Key != currentUserId)
                .Select(g =>
                {
                    var last = g.First();
                    int partnerId = g.Key;
                    string partnerName = partnerDict.ContainsKey(partnerId) 
                        ? partnerDict[partnerId]
                        : ((last.SenderId == currentUserId) ? (last.ReceiverName ?? "Đối tác ReCar") : last.SenderName);

                    string title = last.Car?.CarName ?? partnerName;

                    return new
                    {
                        partnerId = partnerId,
                        ownerId = (last.Car?.OwnerId ?? ((currentCust.RoleId == 3) ? currentUserId : partnerId)),
                        carId = last.CarId,
                        title = title,
                        partnerName = partnerName,
                        lastMessage = last.MessageText,
                        time = last.SentAt.ToString("HH:mm - dd/MM"),
                        carImage = last.Car?.Image ?? "/assets/images/cars/default.png"
                    };
                })
                .Take(15)
                .ToList();

            int totalUnread = await _context.ChatMessages
                .Where(m => (m.ReceiverId == currentUserId || (m.CarId.HasValue && myCarIds.Contains(m.CarId.Value))) && !m.IsRead && m.SenderId != currentUserId)
                .CountAsync();

            return Json(new { success = true, unreadCount = totalUnread, conversations = grouped });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount(bool asOwner = false, int? ownerId = null)
        {
            var currentCust = await ResolveCurrentCustomerAsync(asOwner, asOwner ? ownerId : null);
            if (currentCust == null)
            {
                return Json(new { success = true, unreadCount = 0 });
            }

            int currentUserId = currentCust.CustomerId;

            var myCarIds = await _context.Cars
                .Where(c => c.OwnerId == currentUserId)
                .Select(c => c.CarId)
                .ToListAsync();

            int count = await _context.ChatMessages
                .Where(m => (m.ReceiverId == currentUserId || (m.CarId.HasValue && myCarIds.Contains(m.CarId.Value))) && !m.IsRead && m.SenderId != currentUserId)
                .CountAsync();

            return Json(new { success = true, unreadCount = count });
        }
    }
}
