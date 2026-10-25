using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.ViewModels
{
    public class CarOwnerRegistrationVM
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email liên hệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ liên hệ/bàn giao xe")]
        [Display(Name = "Địa chỉ liên hệ")]
        public string Address { get; set; } = string.Empty;

        // --- 1. Định danh cá nhân & Pháp lý (KYC) ---
        [Required(ErrorMessage = "Vui lòng nhập số CCCD / CMND")]
        [Display(Name = "Số CCCD / CMND")]
        public string IdCard { get; set; } = string.Empty;

        [Display(Name = "Ngày cấp CCCD")]
        [DataType(DataType.Date)]
        public DateTime? IdCardIssueDate { get; set; }

        [Display(Name = "Nơi cấp CCCD")]
        public string? IdCardIssuePlace { get; set; }

        public string? ExistingIdCardFrontImage { get; set; }
        public string? ExistingIdCardBackImage { get; set; }

        [Display(Name = "Ảnh mặt trước CCCD")]
        public IFormFile? IdCardFrontFile { get; set; }

        [Display(Name = "Ảnh mặt sau CCCD")]
        public IFormFile? IdCardBackFile { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số Giấy phép lái xe")]
        [Display(Name = "Số Giấy phép lái xe (GPLX)")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Display(Name = "Hạng GPLX")]
        public string LicenseClass { get; set; } = "B2";

        [Display(Name = "Ngày hết hạn GPLX")]
        [DataType(DataType.Date)]
        public DateTime? LicenseExpiryDate { get; set; }

        public string? ExistingLicenseImage { get; set; }

        [Display(Name = "Ảnh chụp Giấy phép lái xe")]
        public IFormFile? LicenseImageFile { get; set; }

        // --- 2. Thông tin Tài khoản Ngân hàng (Payout Details) ---
        [Required(ErrorMessage = "Vui lòng chọn ngân hàng thụ hưởng")]
        [Display(Name = "Tên ngân hàng")]
        public string BankName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản ngân hàng")]
        [Display(Name = "Số tài khoản")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản ngân hàng (in hoa)")]
        [Display(Name = "Tên chủ tài khoản")]
        public string AccountHolder { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bạn cần đồng ý với điều khoản hợp tác")]
        public bool AcceptTerms { get; set; } = false;
    }
}
