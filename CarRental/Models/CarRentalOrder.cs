using System;
using System.Collections.Generic;

namespace CarRental.Models;

public partial class CarRentalOrder
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public decimal? Deposit { get; set; }

    public decimal? Payment { get; set; }

    public decimal? PlatformFeeRate { get; set; }

    public decimal? PlatformFee { get; set; }

    public int? StatusId { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public string? Notes { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual OrderStatus? Status { get; set; }
}
