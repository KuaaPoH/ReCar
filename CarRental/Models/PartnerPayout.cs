using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    [Table("partner_payouts")]
    public class PartnerPayout
    {
        [Key]
        public int PayoutId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SettlementCode { get; set; } = string.Empty;

        public int OwnerId { get; set; }

        [MaxLength(30)]
        public string? CycleKey { get; set; }

        [MaxLength(100)]
        public string? Period { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "paid";

        public DateTime PaidAt { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? ConfirmedBy { get; set; }
    }
}
