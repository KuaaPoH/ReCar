using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models;

[Table("wallet_transactions")]
public class WalletTransaction
{
    [Key]
    public int TransactionId { get; set; }

    public int CustomerId { get; set; }

    public int? OrderId { get; set; }

    /// <summary>
    /// refund (hoàn tiền cọc), payment (thanh toán đơn), withdraw (rút tiền ngân hàng)
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string Type { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAfter { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? BankName { get; set; }

    [MaxLength(50)]
    public string? BankAccount { get; set; }

    [MaxLength(150)]
    public string? AccountHolder { get; set; }

    /// <summary>
    /// completed, pending, rejected
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "completed";

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }
}
