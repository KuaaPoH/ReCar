using System;

namespace CarRental.Models;

public partial class SystemSetting
{
    public int SettingId { get; set; }

    public int DepositPct { get; set; } = 30; // 30% đặt cọc

    public int CommissionPct { get; set; } = 10; // 10% hoa hồng nền tảng sàn thu

    public int DefaultKmPerDay { get; set; } = 350; // Hạn mức di chuyển mặc định 350 km/ngày

    public decimal ExtraKmPrice { get; set; } = 3000m; // Phụ phí vượt km mặc định 3.000 đ/km

    public string Hotline { get; set; } = "1900 8888";

    public string SupportEmail { get; set; } = "hotro@recar.vn";

    public string PlatformName { get; set; } = "ReCar — Car-Sharing Platform";

    public string Address { get; set; } = "Tòa tháp Keangnam Landmark 72, Mễ Trì, Nam Từ Liêm, Hà Nội";

    public DateTime? UpdatedAt { get; set; }
}

