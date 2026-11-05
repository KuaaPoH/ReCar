using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    // Alias tương thích ngược cho AccountId (không map vào DB)
    [NotMapped]
    public int AccountId 
    { 
        get => CustomerId; 
        set => CustomerId = value; 
    }

    public string? FullName { get; set; }

    public string? Name { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public int RoleId { get; set; } = 2; // 1: Admin, 2: Customer, 3: CarOwner

    public bool IsActive { get; set; } = true;

    public string? LastLogin { get; set; }

    public DateTime? DateofBirth { get; set; }

    public string? Address { get; set; }

    public string? LicenseNumber { get; set; }

    public string? LicenseImage { get; set; }

    public string? Idcard { get; set; }

    public string? PhoneNumber { get; set; }

    // Alias tương thích ngược cho Phone (không map vào DB)
    [NotMapped]
    public string? Phone 
    { 
        get => PhoneNumber; 
        set => PhoneNumber = value; 
    }

    public string? AccountNumber { get; set; }

    public string? Bank { get; set; }

    public string? CompanyName { get; set; }

    public string? Avartar { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<CarRentalOrder> CarRentalOrders { get; set; } = new List<CarRentalOrder>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<ChatMessage> ChatMessagesSent { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ChatMessage> ChatMessagesReceived { get; set; } = new List<ChatMessage>();

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
