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
        public IActionResult Index(int? TypeId)
        {
            ViewBag.TypeId = TypeId;
            return View();
        }

        [HttpGet]
        public IActionResult FilterProducts(decimal? minPrice, decimal? maxPrice, string? brands, int? TypeId, int? seats, string? fuel, string? sort)
        {
            var query = _context.Cars.Include(c => c.Type).Where(c => c.IsActive && c.CarStatus != "Chờ duyệt" && c.CarStatus != "Từ chối");

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
