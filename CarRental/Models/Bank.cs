using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    [Table("banks")]
    public class Bank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BankId { get; set; }

        [Required]
        [StringLength(20)]
        public string BankCode { get; set; } = string.Empty; // VCB, MB, TCB, CTG, BIDV...

        [Required]
        [StringLength(150)]
        public string BankName { get; set; } = string.Empty; // Ngân hàng TMCP Ngoại thương Việt Nam

        [Required]
        [StringLength(50)]
        public string ShortName { get; set; } = string.Empty; // Vietcombank

        [StringLength(10)]
        public string? Bin { get; set; } // Mã định danh Napas/VietQR (VD: 970436)

        [StringLength(255)]
        public string? Logo { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
