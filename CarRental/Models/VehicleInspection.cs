using System;

namespace CarRental.Models;

/// <summary>
/// Biên bản bàn giao xe điện tử (Digital Check-in / Check-out).
/// Mỗi đơn thuê có tối đa 2 biên bản: lúc giao xe ("handover") và lúc nhận lại xe ("return").
/// </summary>
public partial class VehicleInspection
{
    public const string TypeHandover = "handover";
    public const string TypeReturn = "return";

    public int InspectionId { get; set; }

    public int OrderId { get; set; }

    public int CarId { get; set; }

    /// <summary>"handover" (lúc giao xe) hoặc "return" (lúc nhận lại xe)</summary>
    public string InspectionType { get; set; } = TypeHandover;

    /// <summary>Số km trên đồng hồ công tơ mét (ODO)</summary>
    public int Odometer { get; set; }

    /// <summary>Mức xăng / pin tính theo % (0 - 100)</summary>
    public int FuelLevel { get; set; }

    public string? FrontImage { get; set; }

    public string? RearImage { get; set; }

    public string? LeftImage { get; set; }

    public string? RightImage { get; set; }

    /// <summary>Ghi nhận hiện trạng: trầy xước, móp méo, phụ kiện kèm theo...</summary>
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int? CreatedBy { get; set; }

    public virtual CarRentalOrder? Order { get; set; }

    public virtual Car? Car { get; set; }
}

