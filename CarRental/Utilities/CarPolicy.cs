using System.Text.RegularExpressions;
using CarRental.Models;

namespace CarRental.Utilities
{
    /// <summary>
    /// Quy định vận hành của một xe (giới hạn km, phụ phí, giao xe tận nơi...).
    /// Dữ liệu được lưu dạng chuỗi có cấu trúc trong cột Cars.Details khi chủ xe tạo/sửa xe:
    ///   [Tiện nghi]: ...
    ///   [Quy định quãng đường]: Tối đa 350 km/ngày (Phụ phí vượt: 3,000đ/km)
    ///   [Giao nhận xe]: Có hỗ trợ giao xe tận nơi (Phí: 100,000đ)
    ///   [Mô tả bổ sung]: ...
    /// </summary>
    public class CarPolicy
    {
        public const int DefaultMileageLimit = 350;
        public const decimal DefaultExtraFeePerKm = 3000;
        public const decimal DefaultDeliveryFee = 100000;

        public int MileageLimit { get; set; } = DefaultMileageLimit;
        public decimal ExtraFeePerKm { get; set; } = DefaultExtraFeePerKm;
        public bool DeliverySupported { get; set; } = true;
        public decimal DeliveryFee { get; set; } = DefaultDeliveryFee;
        public string? FuelConsumption { get; set; }
        public string? ExtraDescription { get; set; }

        public static CarPolicy Parse(Car? car)
        {
            var policy = new CarPolicy();
            if (car == null) return policy;

            var details = car.Details ?? string.Empty;

            var mileage = Regex.Match(details, @"Tối đa\s*(\d+)\s*km/ngày.*?Phụ phí vượt:\s*([\d\.,]+)\s*đ", RegexOptions.IgnoreCase);
            if (mileage.Success)
            {
                if (int.TryParse(mileage.Groups[1].Value, out var limit) && limit > 0) policy.MileageLimit = limit;
                var fee = ParseMoney(mileage.Groups[2].Value);
                if (fee.HasValue) policy.ExtraFeePerKm = fee.Value;
            }

            var delivery = Regex.Match(details, @"\[Giao nhận xe\]:\s*(.+)", RegexOptions.IgnoreCase);
            if (delivery.Success)
            {
                var line = delivery.Groups[1].Value;
                policy.DeliverySupported = line.Contains("Có hỗ trợ", StringComparison.OrdinalIgnoreCase);
                var deliveryFee = Regex.Match(line, @"Phí:\s*([\d\.,]+)\s*đ", RegexOptions.IgnoreCase);
                if (deliveryFee.Success)
                {
                    var feeVal = ParseMoney(deliveryFee.Groups[1].Value);
                    if (feeVal.HasValue) policy.DeliveryFee = feeVal.Value;
                }
            }

            var extra = Regex.Match(details, @"\[Mô tả bổ sung\]:\s*(.*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (extra.Success && !string.IsNullOrWhiteSpace(extra.Groups[1].Value))
            {
                policy.ExtraDescription = extra.Groups[1].Value.Trim();
            }

            // Descriptions có dạng "Tiêu hao: 6.5L/100km · Tiện nghi A, Tiện nghi B"
            var desc = car.Descriptions ?? string.Empty;
            var fuel = Regex.Match(desc, @"Tiêu hao:\s*([^·]+)", RegexOptions.IgnoreCase);
            if (fuel.Success && !string.IsNullOrWhiteSpace(fuel.Groups[1].Value))
            {
                policy.FuelConsumption = fuel.Groups[1].Value.Trim();
            }

            return policy;
        }

        /// <summary>Đọc số tiền bất kể định dạng "3,000" hay "3.000"</summary>
        private static decimal? ParseMoney(string raw)
        {
            var digits = Regex.Replace(raw ?? string.Empty, @"[^\d]", "");
            return decimal.TryParse(digits, out var value) ? value : null;
        }
    }
}

