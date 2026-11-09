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
        public const decimal DefaultDriverFee = 300000;

        public int MileageLimit { get; set; } = DefaultMileageLimit;
        public decimal ExtraFeePerKm { get; set; } = DefaultExtraFeePerKm;
        public bool DeliverySupported { get; set; } = true;
        public decimal DeliveryFee { get; set; } = DefaultDeliveryFee;
        public decimal DriverFee { get; set; } = DefaultDriverFee;
        public string? FuelConsumption { get; set; }
        public string? ExtraDescription { get; set; }

        /// <summary>
        /// Làm sạch mô tả bổ sung của chủ xe, loại bỏ tất cả các thẻ tag hệ thống và chuỗi Base64 nếu bị dính lồng
        /// </summary>
        public static string CleanExtraDescription(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";

            // Chuẩn hóa các dạng ký tự xuống dòng
            string text = raw.Replace("\\r\\n", "\n").Replace("\\n", "\n").Replace("\r\n", "\n").Replace("\r", "\n");

            // Nếu bị lồng nhiều lớp [Mô tả bổ sung]:, lấy nội dung ở tag cuối cùng
            int idx = text.LastIndexOf("[Mô tả bổ sung]:", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                text = text.Substring(idx + "[Mô tả bổ sung]:".Length);
            }

            // Loại bỏ tất cả các thẻ tag hệ thống
            text = Regex.Replace(text, @"\[(?:Tiện nghi|Quy định quãng đường|Giao nhận xe|Phụ phí tài xế|Snapshot|Bản lưu trước sửa|B\?n l\?u|Mô tả bổ sung)[^\]]*\]:[^\n]*", "", RegexOptions.IgnoreCase);

            // Loại bỏ chuỗi Base64 dài
            text = Regex.Replace(text, @"[A-Za-z0-9+/=]{40,}", "");

            // Lọc các dòng trống
            var lines = text.Split('\n').Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l));
            return string.Join("\n", lines).Trim();
        }

        public static CarPolicy Parse(Car? car, SystemSetting? setting = null)
        {
            var policy = new CarPolicy();
            if (setting != null)
            {
                if (setting.DefaultKmPerDay > 0) policy.MileageLimit = setting.DefaultKmPerDay;
                if (setting.ExtraKmPrice > 0) policy.ExtraFeePerKm = setting.ExtraKmPrice;
            }
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

            var driver = Regex.Match(details, @"\[Phụ phí tài xế\]:\s*([\d\.,]+)\s*đ", RegexOptions.IgnoreCase);
            if (driver.Success)
            {
                var dVal = ParseMoney(driver.Groups[1].Value);
                if (dVal.HasValue) policy.DriverFee = dVal.Value;
            }

            var extra = Regex.Match(details, @"\[Mô tả bổ sung\]:\s*(.*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (extra.Success && !string.IsNullOrWhiteSpace(extra.Groups[1].Value))
            {
                policy.ExtraDescription = CleanExtraDescription(extra.Groups[1].Value);
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

        /// <summary>
        /// Trích xuất JSON snapshot của xe trước khi sửa đổi (hỗ trợ cả Base64 an toàn và JSON thô)
        /// </summary>
        public static string? ExtractOldSnapshotJson(string? details)
        {
            if (string.IsNullOrEmpty(details))
            {
                return null;
            }

            // 1. Thử trích xuất chuỗi Base64
            var matchB64 = Regex.Match(details, @"\[(?:Snapshot|Bản lưu trước sửa|B\?n l\?u tr\?\?c s\?a)\]:\s*([A-Za-z0-9+/=]{20,})", RegexOptions.IgnoreCase);
            if (matchB64.Success)
            {
                try
                {
                    string rawB64 = matchB64.Groups[1].Value.Trim();
                    byte[] bytes = Convert.FromBase64String(rawB64);
                    string decoded = System.Text.Encoding.UTF8.GetString(bytes);
                    if (decoded.StartsWith("{") && decoded.EndsWith("}"))
                    {
                        return decoded;
                    }
                }
                catch
                {
                }
            }

            // 2. Thử trích xuất JSON thô nếu có
            var matchJson = Regex.Match(details, @"\[(?:Snapshot|Bản lưu trước sửa|B\?n l\?u tr\?\?c s\?a)\]:\s*(\{.*?\})(?:\s*\[|\r|\n|$)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (matchJson.Success)
            {
                string rawJson = matchJson.Groups[1].Value.Trim();
                if (rawJson.StartsWith("{") && rawJson.EndsWith("}"))
                {
                    return rawJson;
                }
            }

            return null;
        }

        /// <summary>Đọc số tiền bất kể định dạng "3,000" hay "3.000"</summary>
        private static decimal? ParseMoney(string raw)
        {
            var digits = Regex.Replace(raw ?? string.Empty, @"[^\d]", "");
            return decimal.TryParse(digits, out var value) ? value : null;
        }
    }
}

