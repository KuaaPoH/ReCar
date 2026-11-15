using System;
using System.Collections.Generic;
using System.Linq;

namespace CarRental.Utilities
{
    public class VietnamProvinceInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool IsPopular { get; set; }
        public string[] Aliases { get; set; } = Array.Empty<string>();
    }

    public static class VietnamLocations
    {
        // Danh sách 34 tỉnh/thành phố trực thuộc Trung ương của Việt Nam (đơn vị hành chính mới từ 07/2025)
        public static readonly List<VietnamProvinceInfo> Provinces = new()
        {
            new() { Name = "Hà Nội", Region = "Miền Bắc", Lat = 21.0285, Lng = 105.8542, IsPopular = true, Aliases = new[] { "hà nội", "ha noi", "cầu giấy", "đống đa", "tây hồ", "hoàn kiếm", "ba đình", "thanh xuân", "hà đông", "long biên" } },
            new() { Name = "TP. Hồ Chí Minh", Region = "Miền Nam", Lat = 10.7769, Lng = 106.7009, IsPopular = true, Aliases = new[] { "hồ chí minh", "ho chi minh", "tp.hcm", "tphcm", "sài gòn", "sai gon", "bình dương", "binh duong", "vũng tàu", "vung tau", "bà rịa", "ba ria", "thủ đức", "quận 1", "quận 7" } },
            new() { Name = "Hải Phòng", Region = "Miền Bắc", Lat = 20.8449, Lng = 106.6881, IsPopular = true, Aliases = new[] { "hải phòng", "hai phong", "hải dương", "hai duong" } },
            new() { Name = "Đà Nẵng", Region = "Miền Trung", Lat = 16.0544, Lng = 108.2022, IsPopular = true, Aliases = new[] { "đà nẵng", "da nang", "hải châu", "quảng nam", "quang nam", "hội an", "hoi an", "tam kỳ" } },
            new() { Name = "Cần Thơ", Region = "Miền Nam", Lat = 10.0452, Lng = 105.7469, IsPopular = true, Aliases = new[] { "cần thơ", "can tho", "sóc trăng", "soc trang", "hậu giang", "hau giang" } },
            new() { Name = "Thành phố Huế", Region = "Miền Trung", Lat = 16.4637, Lng = 107.5909, IsPopular = true, Aliases = new[] { "huế", "hue", "thừa thiên huế", "thua thien hue" } },

            // Miền Bắc
            new() { Name = "Quảng Ninh", Region = "Miền Bắc", Lat = 20.9599, Lng = 107.0425, IsPopular = true, Aliases = new[] { "quảng ninh", "quang ninh", "hạ long", "ha long", "cẩm phả", "móng cái" } },
            new() { Name = "Bắc Ninh", Region = "Miền Bắc", Lat = 21.1861, Lng = 106.0763, IsPopular = false, Aliases = new[] { "bắc ninh", "bac ninh", "bắc giang", "bac giang" } },
            new() { Name = "Phú Thọ", Region = "Miền Bắc", Lat = 21.3227, Lng = 105.4020, IsPopular = false, Aliases = new[] { "phú thọ", "phu tho", "việt trì", "vĩnh phúc", "vinh phuc", "vĩnh yên", "hòa bình", "hoa binh" } },
            new() { Name = "Thái Nguyên", Region = "Miền Bắc", Lat = 21.5672, Lng = 105.8252, IsPopular = false, Aliases = new[] { "thái nguyên", "thai nguyen", "bắc kạn", "bac kan" } },
            new() { Name = "Hưng Yên", Region = "Miền Bắc", Lat = 20.6464, Lng = 106.0511, IsPopular = false, Aliases = new[] { "hưng yên", "hung yen", "thái bình", "thai binh" } },
            new() { Name = "Ninh Bình", Region = "Miền Bắc", Lat = 20.2506, Lng = 105.9745, IsPopular = true, Aliases = new[] { "ninh bình", "ninh binh", "hà nam", "ha nam", "phủ lý", "nam định", "nam dinh" } },
            new() { Name = "Lào Cai", Region = "Miền Bắc", Lat = 22.4809, Lng = 103.9755, IsPopular = true, Aliases = new[] { "lào cai", "lao cai", "sa pa", "sapa", "yên bái", "yen bai" } },
            new() { Name = "Tuyên Quang", Region = "Miền Bắc", Lat = 21.8236, Lng = 105.2142, IsPopular = false, Aliases = new[] { "tuyên quang", "tuyen quang", "hà giang", "ha giang" } },
            new() { Name = "Cao Bằng", Region = "Miền Bắc", Lat = 22.6663, Lng = 106.2578, IsPopular = false, Aliases = new[] { "cao bằng", "cao bang" } },
            new() { Name = "Lạng Sơn", Region = "Miền Bắc", Lat = 21.8537, Lng = 106.7615, IsPopular = false, Aliases = new[] { "lạng sơn", "lang son" } },
            new() { Name = "Điện Biên", Region = "Miền Bắc", Lat = 21.3860, Lng = 103.0230, IsPopular = false, Aliases = new[] { "điện biên", "dien bien" } },
            new() { Name = "Lai Châu", Region = "Miền Bắc", Lat = 22.3862, Lng = 103.4703, IsPopular = false, Aliases = new[] { "lai châu", "lai chau" } },
            new() { Name = "Sơn La", Region = "Miền Bắc", Lat = 21.3256, Lng = 103.9188, IsPopular = false, Aliases = new[] { "sơn la", "son la", "mộc châu", "moc chau" } },

            // Miền Trung
            new() { Name = "Thanh Hóa", Region = "Miền Trung", Lat = 19.8067, Lng = 105.7852, IsPopular = false, Aliases = new[] { "thanh hóa", "thanh hoa", "sầm sơn" } },
            new() { Name = "Nghệ An", Region = "Miền Trung", Lat = 18.6737, Lng = 105.6813, IsPopular = true, Aliases = new[] { "nghệ an", "nghe an", "tp. vinh", "vinh", "cửa lò", "lê mao", "trường thi", "hưng dũng", "quảng trường" } },
            new() { Name = "Hà Tĩnh", Region = "Miền Trung", Lat = 18.3560, Lng = 105.8877, IsPopular = false, Aliases = new[] { "hà tĩnh", "ha tinh" } },
            new() { Name = "Quảng Trị", Region = "Miền Trung", Lat = 16.8163, Lng = 107.1003, IsPopular = false, Aliases = new[] { "quảng trị", "quang tri", "quảng bình", "quang binh", "đồng hới", "dong hoi" } },
            new() { Name = "Quảng Ngãi", Region = "Miền Trung", Lat = 15.1214, Lng = 108.8044, IsPopular = false, Aliases = new[] { "quảng ngãi", "quang ngai", "kon tum", "lý sơn" } },
            new() { Name = "Gia Lai", Region = "Miền Trung", Lat = 13.9833, Lng = 108.0000, IsPopular = false, Aliases = new[] { "gia lai", "pleiku", "bình định", "binh dinh", "quy nhơn", "quy nhon" } },
            new() { Name = "Đắk Lắk", Region = "Miền Trung", Lat = 12.6667, Lng = 108.0500, IsPopular = false, Aliases = new[] { "đắk lắk", "dak lak", "buôn ma thuột", "buon ma thuot", "phú yên", "phu yen", "tuy hòa" } },
            new() { Name = "Khánh Hòa", Region = "Miền Trung", Lat = 12.2388, Lng = 109.1967, IsPopular = true, Aliases = new[] { "khánh hòa", "khanh hoa", "nha trang", "cam ranh", "ninh thuận", "ninh thuan", "phan rang", "lộc thọ" } },
            new() { Name = "Lâm Đồng", Region = "Miền Trung", Lat = 11.9404, Lng = 108.4583, IsPopular = true, Aliases = new[] { "lâm đồng", "lam dong", "đà lạt", "da lat", "bảo lộc", "bình thuận", "binh thuan", "phan thiết", "phan thiet", "mũi né", "đắk nông", "dak nong" } },

            // Miền Nam
            new() { Name = "Đồng Nai", Region = "Miền Nam", Lat = 10.9574, Lng = 106.8427, IsPopular = false, Aliases = new[] { "đồng nai", "dong nai", "biên hòa", "bien hoa", "bình phước", "binh phuoc", "đồng xoài" } },
            new() { Name = "Tây Ninh", Region = "Miền Nam", Lat = 11.3100, Lng = 106.0983, IsPopular = false, Aliases = new[] { "tây ninh", "tay ninh", "long an", "tân an" } },
            new() { Name = "Đồng Tháp", Region = "Miền Nam", Lat = 10.4547, Lng = 105.6331, IsPopular = false, Aliases = new[] { "đồng tháp", "dong thap", "cao lãnh", "tiền giang", "tien giang", "mỹ tho" } },
            new() { Name = "Vĩnh Long", Region = "Miền Nam", Lat = 10.2537, Lng = 105.9722, IsPopular = false, Aliases = new[] { "vĩnh long", "vinh long", "bến tre", "ben tre", "trà vinh", "tra vinh" } },
            new() { Name = "An Giang", Region = "Miền Nam", Lat = 10.3864, Lng = 105.4352, IsPopular = true, Aliases = new[] { "an giang", "long xuyên", "châu đốc", "kiên giang", "kien giang", "phú quốc", "phu quoc", "rạch giá" } },
            new() { Name = "Cà Mau", Region = "Miền Nam", Lat = 9.1768, Lng = 105.1524, IsPopular = false, Aliases = new[] { "cà mau", "ca mau", "bạc liêu", "bac lieu" } }
        };

        public static (double lat, double lng) ResolveCoordinates(string? location, int carId = 0)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return (21.0285, 105.8542);
            }

            string loc = location.ToLower().Trim();

            // Chi tiết quận/phường phổ biến để giữ độ chính xác cao cho dữ liệu mẫu cũ
            if (loc.Contains("cầu giấy") || loc.Contains("cau giay")) return (21.0333 + (carId % 5) * 0.002, 105.7833 + (carId % 3) * 0.002);
            if (loc.Contains("đống đa") || loc.Contains("dong da")) return (21.0180 + (carId % 4) * 0.002, 105.8290 + (carId % 3) * 0.002);
            if (loc.Contains("tây hồ") || loc.Contains("tay ho")) return (21.0667 + (carId % 3) * 0.002, 105.8167 + (carId % 2) * 0.002);
            if (loc.Contains("lê mao") || loc.Contains("le mao")) return (18.6705 + (carId % 3) * 0.002, 105.6765 + (carId % 2) * 0.002);
            if (loc.Contains("trường thi") || loc.Contains("truong thi")) return (18.6752 + (carId % 3) * 0.002, 105.6908 + (carId % 2) * 0.002);
            if (loc.Contains("hưng dũng") || loc.Contains("hung dung")) return (18.6815 + (carId % 3) * 0.002, 105.7020 + (carId % 2) * 0.002);
            if (loc.Contains("quận 7") || loc.Contains("quan 7")) return (10.7327 + (carId % 3) * 0.002, 106.7167 + (carId % 4) * 0.002);
            if (loc.Contains("quận 1") || loc.Contains("quan 1")) return (10.7769 + (carId % 4) * 0.002, 106.7009 + (carId % 3) * 0.002);
            if (loc.Contains("phú quốc") || loc.Contains("phu quoc")) return (10.2270 + (carId % 3) * 0.002, 103.9570 + (carId % 3) * 0.002);
            if (loc.Contains("vũng tàu") || loc.Contains("vung tau")) return (10.3460 + (carId % 3) * 0.002, 107.0843 + (carId % 3) * 0.002);
            if (loc.Contains("quy nhơn") || loc.Contains("quy nhon")) return (13.7820 + (carId % 3) * 0.002, 109.2197 + (carId % 3) * 0.002);
            if (loc.Contains("phan thiết") || loc.Contains("phan thiet")) return (10.9289 + (carId % 3) * 0.002, 108.1021 + (carId % 3) * 0.002);

            foreach (var p in Provinces)
            {
                if (loc.Contains(p.Name.ToLower()) || p.Aliases.Any(a => loc.Contains(a)))
                {
                    return (p.Lat + (carId % 5) * 0.003, p.Lng + (carId % 4) * 0.003);
                }
            }

            return (21.0285, 105.8542);
        }
    }
}
