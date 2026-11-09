using System;
using System.Collections.Generic;

namespace CarRental.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public string? Role { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? Type { get; set; } = "System";

    public string? LinkUrl { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

