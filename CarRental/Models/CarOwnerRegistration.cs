using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    [Table("carowner_registrations")]
    public class CarOwnerRegistration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Address { get; set; }

        // --- 1. Định danh cá nhân & Pháp lý (KYC) ---
        [Required(ErrorMessage = "Vui lòng nhập số CCCD / Hộ chiếu")]
        [StringLength(25)]
        public string IdCard { get; set; } = string.Empty;

        public DateTime? IdCardIssueDate { get; set; }

        [StringLength(100)]
        public string? IdCardIssuePlace { get; set; }

        [StringLength(255)]
        public string? IdCardFrontImage { get; set; }

        [StringLength(255)]
        public string? IdCardBackImage { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số Giấy phép lái xe (GPLX)")]
        [StringLength(25)]
        public string LicenseNumber { get; set; } = string.Empty;

        [StringLength(20)]
        public string? LicenseClass { get; set; } = "B2"; // B1, B2, C...

        public DateTime? LicenseExpiryDate { get; set; }

        [StringLength(255)]
        public string? LicenseImage { get; set; }

        // --- 2. Tài khoản Ngân hàng (Payout Details) ---
        [Required(ErrorMessage = "Vui lòng chọn ngân hàng thụ hưởng")]
        [StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản ngân hàng")]
        [StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản (in hoa không dấu)")]
        [StringLength(100)]
        public string AccountHolder { get; set; } = string.Empty;

        // --- 3. Trạng thái xét duyệt ---
        // Pending: Chờ duyệt, Approved: Đã duyệt thành chủ xe, Rejected: Bị từ chối
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        public string? RejectReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ApprovedAt { get; set; }

        [StringLength(50)]
        public string? ApprovedBy { get; set; }
    }
}
