using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CarRental.ViewModels
{
    public class CarCreateVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên phương tiện")]
        [Display(Name = "Tên xe")]
        public string CarName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn hãng sản xuất")]
        [Display(Name = "Hãng xe")]
        public string CarBrand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn phân khúc xe")]
        [Display(Name = "Phân khúc xe")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn năm sản xuất")]
        [Range(2015, 2030, ErrorMessage = "Năm sản xuất từ 2015 trở lại đây")]
        [Display(Name = "Năm sản xuất")]
        public int? ModelYear { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập biển số xe kiểm định")]
        [Display(Name = "Biển số xe")]
        public string LicensePlate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập màu sắc xe")]
        [Display(Name = "Màu sắc xe")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn số chỗ ngồi")]
        [Display(Name = "Số chỗ")]
        public int? Seat { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại hộp số")]
        [Display(Name = "Hộp số")]
        public string? Transmission { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại nhiên liệu")]
        [Display(Name = "Nhiên liệu")]
        public string? Fuel { get; set; }

        [Display(Name = "Mức tiêu hao nhiên liệu")]
        public string? FuelConsumption { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá niêm yết theo ngày")]
        [Range(200000, 50000000, ErrorMessage = "Giá thuê tối thiểu từ 200.000đ/ngày")]
        [Display(Name = "Giá thuê niêm yết (VNĐ/ngày)")]
        public decimal Price { get; set; } = 800000;

        [Display(Name = "Giá ưu đãi khuyến mãi")]
        public decimal? SalePrice { get; set; }

        [Display(Name = "Giới hạn quãng đường (km/ngày)")]
        public int MileageLimit { get; set; } = 350;

        [Display(Name = "Phụ phí quá km (VNĐ/km)")]
        public decimal ExtraFeePerKm { get; set; } = 3000;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao nhận xe")]
        [Display(Name = "Địa chỉ nhận xe")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Hỗ trợ giao xe tận nơi")]
        public bool DeliverySupported { get; set; } = true;

        [Display(Name = "Phí giao xe tận nơi (VNĐ)")]
        public decimal DeliveryFee { get; set; } = 100000;

        [Display(Name = "Tiện nghi trang bị")]
        public List<string> Features { get; set; } = new List<string>();

        // Danh sách ID tiện ích được chọn từ CSDL
        public List<int> AmenityIds { get; set; } = new List<int>();

        [Display(Name = "Mô tả chi tiết và nội quy")]
        public string? Details { get; set; }

        [Display(Name = "Ảnh đại diện chính")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Đường dẫn ảnh trực tuyến (nếu không tải file)")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Album ảnh chi tiết xe")]
        public List<IFormFile>? DetailImages { get; set; }

        [Display(Name = "Giấy đăng ký xe (cà vẹt)")]
        public IFormFile? RegistrationImageFile { get; set; }

        [Display(Name = "Giấy chứng nhận đăng kiểm")]
        public IFormFile? InspectionImageFile { get; set; }

        [Display(Name = "Bảo hiểm TNDS bắt buộc")]
        public IFormFile? InsuranceImageFile { get; set; }
    }
}
