using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using CarRental.Models;
using CarRental.Hubs;
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

        public class SendMessageRequest
        {
            public int? CarId { get; set; }
            public int? OwnerId { get; set; }
            public string MessageText { get; set; } = string.Empty;
            public string? SenderName { get; set; }
            public string? CarName { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.MessageText))
            {
                return Json(new { success = false, message = "Tin nhắn không được để trống" });
            }

            // 1. Xác định người gửi (nếu có đăng nhập từ Session)
            int? senderId = null;
            string senderName = !string.IsNullOrWhiteSpace(req.SenderName) ? req.SenderName.Trim() : "Khách hàng";

            var sessionAccountId = HttpContext.Session.GetString("AccountId");
            if (!string.IsNullOrEmpty(sessionAccountId) && int.TryParse(sessionAccountId, out int accId))
            {
                senderId = accId;
                var currentAcc = await _context.Accounts.FindAsync(accId);
                if (currentAcc != null && !string.IsNullOrWhiteSpace(currentAcc.FullName))
                {
                    senderName = currentAcc.FullName;
                }
            }

            // 2. Tìm thông tin Chủ xe nhận tin nhắn
            string receiverName = "Chủ xe ReCar";
            if (req.OwnerId.HasValue && req.OwnerId.Value > 0)
            {
                var owner = await _context.Accounts.FindAsync(req.OwnerId.Value);
                if (owner != null && !string.IsNullOrWhiteSpace(owner.FullName))
                {
                    receiverName = owner.FullName;
                }
            }

            // 3. Lưu tin nhắn của khách vào CSDL
            var customerMsg = new ChatMessage
            {
                CarId = req.CarId,
                SenderId = senderId,
                SenderName = senderName,
                SenderRole = "Customer",
                ReceiverId = req.OwnerId,
                ReceiverName = receiverName,
                MessageText = req.MessageText.Trim(),
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.ChatMessages.Add(customerMsg);
            await _context.SaveChangesAsync();

            // 4. Tạo phản hồi tự động thông minh từ Chủ xe và lưu vào CSDL
            string autoReplyText = GenerateOwnerReply(req.MessageText, receiverName, req.CarName);
            var ownerReplyMsg = new ChatMessage
            {
                CarId = req.CarId,
                SenderId = req.OwnerId,
                SenderName = receiverName,
                SenderRole = "CarOwner",
                ReceiverId = senderId,
                ReceiverName = senderName,
                MessageText = autoReplyText,
                SentAt = DateTime.Now.AddSeconds(1),
                IsRead = false
            };

            _context.ChatMessages.Add(ownerReplyMsg);
            await _context.SaveChangesAsync();

            // 5. Phát tín hiệu thời gian thực qua SignalR
            try
            {
                var payloadCustomerMsg = new
                {
                    messageId = customerMsg.MessageId,
                    carId = customerMsg.CarId,
                    senderId = customerMsg.SenderId,
                    senderName = customerMsg.SenderName,
                    senderRole = customerMsg.SenderRole,
                    receiverId = customerMsg.ReceiverId,
                    receiverName = customerMsg.ReceiverName,
                    messageText = customerMsg.MessageText,
                    sentAt = customerMsg.SentAt.ToString("HH:mm - dd/MM"),
                    isFromOwner = false
                };

                var payloadOwnerReply = new
                {
                    messageId = ownerReplyMsg.MessageId,
                    carId = ownerReplyMsg.CarId,
                    senderId = ownerReplyMsg.SenderId,
                    senderName = ownerReplyMsg.SenderName,
                    senderRole = ownerReplyMsg.SenderRole,
                    receiverId = ownerReplyMsg.ReceiverId,
                    receiverName = ownerReplyMsg.ReceiverName,
                    messageText = ownerReplyMsg.MessageText,
                    sentAt = ownerReplyMsg.SentAt.ToString("HH:mm - dd/MM"),
                    isFromOwner = true
                };

                // Gửi tin nhắn của khách cho Chủ xe
                if (req.OwnerId.HasValue && req.OwnerId.Value > 0)
                {
                    await _hubContext.Clients.Group($"User_{req.OwnerId.Value}").SendAsync("ReceiveChatMessage", payloadCustomerMsg);
                }
                if (req.CarId.HasValue && req.CarId.Value > 0)
                {
                    await _hubContext.Clients.Group($"Car_{req.CarId.Value}").SendAsync("ReceiveChatMessage", payloadCustomerMsg);
                }

                // Gửi tin phản hồi của chủ xe cho khách hàng
                if (senderId.HasValue && senderId.Value > 0)
                {
                    await _hubContext.Clients.Group($"User_{senderId.Value}").SendAsync("ReceiveChatMessage", payloadOwnerReply);
                }
                else if (req.CarId.HasValue && req.CarId.Value > 0)
                {
                    await _hubContext.Clients.Group($"Car_{req.CarId.Value}").SendAsync("ReceiveChatMessage", payloadOwnerReply);
                }
            }
            catch (Exception hubEx)
            {
                // SignalR broadcast không làm gián đoạn luồng chính
                Console.WriteLine($"SignalR Chat Broadcast Error: {hubEx.Message}");
            }

            return Json(new
            {
                success = true,
                customerMessage = new
                {
                    messageId = customerMsg.MessageId,
                    senderName = customerMsg.SenderName,
                    messageText = customerMsg.MessageText,
                    sentAt = customerMsg.SentAt.ToString("HH:mm")
                },
                ownerReply = new
                {
                    messageId = ownerReplyMsg.MessageId,
                    senderName = ownerReplyMsg.SenderName,
                    messageText = ownerReplyMsg.MessageText,
                    sentAt = ownerReplyMsg.SentAt.ToString("HH:mm")
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
                else if (req.CarId.HasValue && req.CarId.Value > 0)
                {
                    await _hubContext.Clients.Group($"Car_{req.CarId.Value}").SendAsync("UserTyping", new
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
        public async Task<IActionResult> GetMessages(int? carId, int? ownerId)
        {
            var query = _context.ChatMessages.AsQueryable();

            if (carId.HasValue && carId.Value > 0)
            {
                query = query.Where(m => m.CarId == carId.Value);
            }
            else if (ownerId.HasValue && ownerId.Value > 0)
            {
                query = query.Where(m => m.SenderId == ownerId.Value || m.ReceiverId == ownerId.Value);
            }

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
                    receiverName = m.ReceiverName,
                    messageText = m.MessageText,
                    isFromOwner = m.SenderRole == "CarOwner",
                    sentAt = m.SentAt.ToString("HH:mm - dd/MM")
                })
                .ToListAsync();

            return Json(new { success = true, messages });
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentConversations()
        {
            // Lấy danh sách tin nhắn mới nhất theo từng xe hoặc chủ xe
            var recentMessages = await _context.ChatMessages
                .Include(m => m.Car)
                .OrderByDescending(m => m.SentAt)
                .Take(20)
                .ToListAsync();

            var grouped = recentMessages
                .GroupBy(m => m.CarId ?? (m.SenderRole == "CarOwner" ? m.SenderId : m.ReceiverId) ?? 0)
                .Select(g =>
                {
                    var last = g.First();
                    string title = last.Car?.CarName ?? (last.SenderRole == "CarOwner" ? last.SenderName : last.ReceiverName) ?? "Chủ xe ReCar";
                    string partnerName = last.SenderRole == "CarOwner" ? last.SenderName : (last.ReceiverName ?? "Chủ xe ReCar");
                    int? carId = last.CarId;
                    int? ownerId = last.SenderRole == "CarOwner" ? last.SenderId : last.ReceiverId;
                    return new
                    {
                        carId = carId,
                        ownerId = ownerId,
                        title = title,
                        partnerName = partnerName,
                        lastMessage = last.MessageText,
                        time = last.SentAt.ToString("HH:mm - dd/MM"),
                        carImage = last.Car?.Image ?? "/assets/images/cars/default.png"
                    };
                })
                .Take(5)
                .ToList();

            return Json(new { success = true, conversations = grouped });
        }

        private string GenerateOwnerReply(string userMsg, string ownerName, string? carName)
        {
            var text = userMsg.ToLower();
            string car = !string.IsNullOrWhiteSpace(carName) ? carName : "xe";

            if (text.Contains("giá") || text.Contains("chi phí") || text.Contains("bao nhiêu") || text.Contains("tiền"))
            {
                return $"{ownerName}: Giá thuê niêm yết trên ứng dụng là giá trọn gói đã gồm bảo hiểm cơ bản. Bạn thuê nhiều ngày sẽ có chiết khấu thêm nhé!";
            }
            if (text.Contains("địa chỉ") || text.Contains("nhận xe") || text.Contains("ở đâu") || text.Contains("giao xe") || text.Contains("tận nơi"))
            {
                return $"{ownerName}: Dạ mình có hỗ trợ giao nhận xe tận nơi tại các quận nội thành hoặc bạn có thể qua trực tiếp bãi xe để nhận xe nha.";
            }
            if (text.Contains("giấy tờ") || text.Contains("thủ tục") || text.Contains("căn cước") || text.Contains("bằng lái") || text.Contains("gplx"))
            {
                return $"{ownerName}: Thủ tục thuê xe rất nhanh gọn: Bạn chỉ cần CCCD gắn chip (hoặc VNeID định danh mức 2) và GPLX hợp lệ là nhận xe được ngay.";
            }
            if (text.Contains("bận") || text.Contains("bảo trì") || text.Contains("khi nào") || text.Contains("lịch"))
            {
                return $"{ownerName}: Cảm ơn bạn đã hỏi thăm! Chiếc {car} này mình đang cho khách thuê / kiểm tra kỹ thuật định kỳ. Bạn dự kiến đi ngày nào để mình sắp xếp giữ lịch ưu tiên cho bạn nhé!";
            }

            return $"{ownerName}: Chào bạn! Cảm ơn bạn đã gửi tin nhắn về chiếc {car}. Mình đã ghi nhận yêu cầu và sẽ hỗ trợ chi tiết cho bạn ngay đây ạ!";
        }
    }
}
