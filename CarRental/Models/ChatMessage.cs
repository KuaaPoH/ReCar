using System;
using System.Collections.Generic;

namespace CarRental.Models;

public partial class ChatMessage
{
    public int MessageId { get; set; }

    public int? CarId { get; set; }

    public int? SenderId { get; set; }

    public string SenderName { get; set; } = "Khách hàng";

    public string SenderRole { get; set; } = "Customer";

    public int? ReceiverId { get; set; }

    public string? ReceiverName { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime SentAt { get; set; } = DateTime.Now;

    public bool IsRead { get; set; } = false;

    public virtual Car? Car { get; set; }

    public virtual Account? Sender { get; set; }

    public virtual Account? Receiver { get; set; }
}
