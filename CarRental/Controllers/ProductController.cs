using CarRental.Extensions;
using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarRental.Controllers
{
    public class ProductController : Controller
    {
        private readonly DbRenalCarContext _context;
        public ProductController(DbRenalCarContext context)
        {
            _context = context;
        }
        /*[Route("/Product/Index.html")]*/
        public IActionResult Index(int? TypeId, string? city)
        {
            ViewBag.TypeId = TypeId;
            ViewBag.SelectedCity = city;
            ViewBag.Locations = _context.Locations.Where(l => l.IsActive).OrderBy(l => l.DisplayOrder).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult FilterProducts(decimal? minPrice, decimal? maxPrice, string? brands, int? TypeId, int? seats, string? fuel, string? sort, string? city)
        {
            var query = _context.Cars.Include(c => c.Type).Where(c => c.IsActive && c.CarStatus != "Chờ duyệt" && c.CarStatus != "Từ chối");

            if (!string.IsNullOrEmpty(city))
            {
                var searchTerms = new List<string>();
                string raw = city.Trim();
                
                // Trích xuất các từ khóa cốt lõi (VD: "TP. Vinh (Nghệ An)" -> "Vinh", "Nghệ An")
                if (raw.Contains("(") && raw.Contains(")"))
                {
                    int start = raw.IndexOf('(');
                    int end = raw.IndexOf(')');
                    string inside = raw.Substring(start + 1, end - start - 1).Trim();
                    string outside = raw.Substring(0, start).Replace("TP.", "").Replace("Tỉnh", "").Trim();
                    if (!string.IsNullOrEmpty(outside)) searchTerms.Add(outside);
                    if (!string.IsNullOrEmpty(inside)) searchTerms.Add(inside);
                }
                else
                {
                    string cleaned = raw.Replace("TP.", "").Replace("Thành phố", "").Replace("Tỉnh", "").Trim();
                    if (!string.IsNullOrEmpty(cleaned)) searchTerms.Add(cleaned);
                }

                // Lọc trực tiếp SQL an toàn không dùng .Any() để tương thích 100% MariaDB & MySQL
                if (searchTerms.Count == 1)
                {
                    string t1 = searchTerms[0];
                    query = query.Where(c => c.Location != null && c.Location.Contains(t1));
                }
                else if (searchTerms.Count >= 2)
                {
                    string t1 = searchTerms[0];
                    string t2 = searchTerms[1];
                    query = query.Where(c => c.Location != null && (c.Location.Contains(t1) || c.Location.Contains(t2)));
                }
            }

            if (minPrice.HasValue && minPrice.Value > 0)
            {
                query = query.Where(c => (c.IsSale && c.SalePrice != null && c.SalePrice > 0 && c.SalePrice < c.Price ? c.SalePrice : c.Price) >= minPrice.Value);
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(c => (c.IsSale && c.SalePrice != null && c.SalePrice > 0 && c.SalePrice < c.Price ? c.SalePrice : c.Price) <= maxPrice.Value);
            }

            if (!string.IsNullOrEmpty(brands))
            {
                var brandList = brands.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(b => b.Trim().ToLower())
                    .ToList();
                query = query.Where(c => c.CarBrand != null && brandList.Contains(c.CarBrand.ToLower()));
            }

            if (TypeId.HasValue && TypeId.Value > 0)
            {
                query = query.Where(c => c.TypeId == TypeId.Value);
            }

            if (seats.HasValue && seats.Value > 0)
            {
                query = query.Where(c => c.Seat == seats.Value);
            }

            if (!string.IsNullOrEmpty(fuel))
            {
                query = query.Where(c => (c.Fuel != null && c.Fuel.Contains(fuel)) || (c.Details != null && c.Details.Contains(fuel)));
            }

            if (sort == "price-asc")
            {
                query = query.OrderBy(c => c.IsSale && c.SalePrice != null && c.SalePrice > 0 && c.SalePrice < c.Price ? c.SalePrice : c.Price);
            }
            else if (sort == "price-desc")
            {
                query = query.OrderByDescending(c => c.IsSale && c.SalePrice != null && c.SalePrice > 0 && c.SalePrice < c.Price ? c.SalePrice : c.Price);
            }
            else if (sort == "rating")
            {
                query = query.OrderByDescending(c => c.Rate);
            }
            else
            {
                query = query.OrderBy(c => c.CarId);
            }

            var list = query.ToList();
            return ViewComponent("Product", new { items = list });
        }

        [HttpGet]
        public IActionResult GetNearbyCars(double? userLat, double? userLng, double? radiusKm)
        {
            // Mặc định trung tâm Hà Nội (Cầu Giấy) nếu người dùng chưa cấp quyền GPS
            double centerLat = userLat ?? 21.0333;
            double centerLng = userLng ?? 105.7833;
            double maxRadius = (radiusKm.HasValue && radiusKm.Value > 0) ? radiusKm.Value : 999999;

            var cars = _context.Cars
                .Include(c => c.Type)
                .Include(c => c.Owner)
                .Where(c => c.IsActive && c.CarStatus != "Chờ duyệt" && c.CarStatus != "Từ chối")
                .ToList();

            var result = cars.Select(car =>
            {
                var coords = ResolveCoordinates(car.Location, car.CarId);
                double dist = CalculateHaversineDistance(centerLat, centerLng, coords.lat, coords.lng);
                decimal price = (car.IsSale && car.SalePrice.HasValue && car.SalePrice > 0 && car.SalePrice < (car.Price ?? 0))
                    ? car.SalePrice.Value
                    : (car.Price ?? 0);

                return new
                {
                    carId = car.CarId,
                    carName = car.CarName ?? "Xe ReCar",
                    carBrand = car.CarBrand ?? "",
                    licensePlate = car.LicensePlate ?? "",
                    price = price,
                    priceFormatted = price.ToString("N0") + "đ",
                    image = car.Image ?? "/assets/images/cars/default.png",
                    location = car.Location ?? "Toàn quốc",
                    transmission = car.Transmission ?? "Tự động",
                    fuel = car.Fuel ?? "Xăng",
                    seat = car.Seat ?? 5,
                    rate = car.Rate ?? 5.0,
                    trips = car.Trips ?? 0,
                    badge = car.Badge ?? "",
                    carStatus = car.CarStatus ?? "Sẵn sàng",
                    ownerName = car.Owner?.FullName ?? "Đối tác Chủ xe",
                    lat = coords.lat,
                    lng = coords.lng,
                    distanceKm = Math.Round(dist, 1),
                    detailUrl = $"/product/{car.Alias}-{car.CarId}.html"
                };
            })
            .Where(c => c.distanceKm <= maxRadius)
            .OrderBy(c => c.distanceKm)
            .ToList();

            return Json(new 
            { 
                success = true, 
                center = new { lat = centerLat, lng = centerLng }, 
                totalFound = result.Count,
                cars = result 
            });
        }

        private (double lat, double lng) ResolveCoordinates(string? location, int carId)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return (21.0333, 105.7833);
            }

            string loc = location.ToLower();

            if (loc.Contains("cầu giấy")) return (21.0333 + (carId % 5) * 0.003, 105.7833 + (carId % 3) * 0.003);
            if (loc.Contains("đống đa")) return (21.0180 + (carId % 4) * 0.003, 105.8290 + (carId % 3) * 0.003);
            if (loc.Contains("tây hồ")) return (21.0667 + (carId % 3) * 0.003, 105.8167 + (carId % 2) * 0.003);
            if (loc.Contains("hà nội")) return (21.0285 + (carId % 5) * 0.004, 105.8542 + (carId % 4) * 0.004);

            if (loc.Contains("lê mao")) return (18.6705 + (carId % 3) * 0.002, 105.6765 + (carId % 2) * 0.002);
            if (loc.Contains("trường thi")) return (18.6752 + (carId % 3) * 0.002, 105.6908 + (carId % 2) * 0.002);
            if (loc.Contains("hưng dũng")) return (18.6815 + (carId % 3) * 0.002, 105.7020 + (carId % 2) * 0.002);
            if (loc.Contains("quảng trường") || (loc.Contains("vinh") && loc.Contains("hồ chí minh"))) return (18.6737 + (carId % 3) * 0.002, 105.6813 + (carId % 2) * 0.002);
            if (loc.Contains("vinh") || loc.Contains("nghệ an")) return (18.6737 + (carId % 5) * 0.003, 105.6813 + (carId % 4) * 0.003);

            if (loc.Contains("quận 7")) return (10.7327 + (carId % 3) * 0.003, 106.7167 + (carId % 4) * 0.003);
            if (loc.Contains("quận 1")) return (10.7769 + (carId % 4) * 0.003, 106.7009 + (carId % 3) * 0.003);
            if (loc.Contains("tp.hcm") || loc.Contains("hồ chí minh")) return (10.7769 + (carId % 5) * 0.005, 106.7009 + (carId % 4) * 0.005);

            if (loc.Contains("đà nẵng") || loc.Contains("hải châu")) return (16.0544 + (carId % 4) * 0.003, 108.2208 + (carId % 3) * 0.003);
            if (loc.Contains("đà lạt")) return (11.9404 + (carId % 3) * 0.003, 108.4373 + (carId % 4) * 0.003);
            if (loc.Contains("nha trang") || loc.Contains("lộc thọ")) return (12.2388 + (carId % 3) * 0.003, 109.1967 + (carId % 4) * 0.003);

            return (18.6737, 105.6813);
        }

        private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Bán kính Trái đất (km)
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        [Route("/product/{alias}-{id}.html")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }
            var product = await _context.Cars
                .Include(i => i.Type)
                .Include(i => i.Owner)
                .FirstOrDefaultAsync(m => m.CarId == id);
            //var product = await _context.Cars.FirstOrDefaultAsync(m => m.CarId == id);

            if (product == null)
            {
                return NotFound();
            }

            //ViewBag.productFeatured = _context.CarTypes.OrderByDescending(i=>i.CarTypeId).ToList();


            //ViewBag.productRelated = _context.Cars.Include(i=>i.Type).Where(i => i.TypeId == product.TypeId && i.CarId != id).ToList();
            ViewBag.productRelated = _context.Cars.Where(i => i.TypeId == product.TypeId && i.CarId != id).ToList();
            ViewBag.productRviews = _context.CarReviews.Where(r => r.CarId == product.CarId).ToList();
          
            // Lấy danh sách sản phẩm đã xem gần đây từ session
            var recentProducts = HttpContext.Session.Get<List<int>>("RecentProducts") ?? new List<int>();

            // Thêm ID sản phẩm vào danh sách nếu chưa có
            if (!recentProducts.Contains(id.Value))
            {
                recentProducts.Add(id.Value);

                // Giữ danh sách giới hạn số lượng sản phẩm (ví dụ: 5 sản phẩm gần đây)
                if (recentProducts.Count > 5)
                {
                    recentProducts.RemoveAt(0);
                }

                // Cập nhật session
                HttpContext.Session.Set("RecentProducts", recentProducts);
            }
            // Lấy danh sách ngày thuê liên quan đến xe
            var rentalDates = _context.OrderDetails
                .Where(od => od.CarId == id)
                .Select(od => new
                {
                    od.PickupDate,
                    od.ReturnDate
                })
                .ToList();

            // Truyền dữ liệu sang View
            ViewBag.RentalDates = rentalDates;
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Reviews(string name,string email, string detail, int id, int star)
        {
            try
            {
                // Tạo đối tượng review mới
                CarReview r = new CarReview
                {
                    Name = name,
                    
                    Email = email,
                    CreatedDate = DateTime.Now,
                    Detail= detail,
                    CarId = id,
                    Star = star

                };

                // Thêm vào DbSet và lưu vào cơ sở dữ liệu
                _context.CarReviews.Add(r);
                await _context.SaveChangesAsync(); // Sử dụng await để đảm bảo dữ liệu được lưu


                // Trả về dữ liệu của review vừa được thêm
                return Json(new
                {
                    //status = true,
                    name = r.Name,
                    email = r.Email,
                    detail = r.Detail,
                    star = r.Star
                   
                });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần) và trả về trạng thái thất bại
                return Json(new { status = false, message = ex.Message });
            }
        }
                 

    }
}
