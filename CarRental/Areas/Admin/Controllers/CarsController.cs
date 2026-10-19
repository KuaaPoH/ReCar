using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CarsController : Controller
    {
        private readonly DbRenalCarContext _context;

        public CarsController(DbRenalCarContext context)
        {
            _context = context;
        }

        private bool CheckAdmin()
        {
            return Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session);
        }

        // GET: /Admin/Cars/Pending (Phê duyệt xe mới)
        public IActionResult Pending(string? q, string? brand, string? city, string? owner)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var query = _context.Cars
                .Include(c => c.Type)
                .Include(c => c.Owner)
                .Where(c => c.CarStatus == "Chờ duyệt")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                string searchKey = q.Trim().ToLower();
                query = query.Where(c => (c.CarName != null && c.CarName.ToLower().Contains(searchKey))
                                      || (c.LicensePlate != null && c.LicensePlate.ToLower().Contains(searchKey)));
            }

            if (!string.IsNullOrWhiteSpace(brand) && brand != "__all")
            {
                query = query.Where(c => c.CarBrand == brand);
            }

            if (!string.IsNullOrWhiteSpace(city) && city != "__all")
            {
                query = query.Where(c => c.Location != null && c.Location.Contains(city));
            }

            if (!string.IsNullOrWhiteSpace(owner))
            {
                string ownerKey = owner.Trim().ToLower();
                query = query.Where(c => (c.Owner != null && c.Owner.FullName != null && c.Owner.FullName.ToLower().Contains(ownerKey))
                                      || (c.Owner != null && c.Owner.Phone != null && c.Owner.Phone.Contains(ownerKey)));
            }

            var pendingCars = query.OrderByDescending(c => c.CarId).ToList();

            // Lấy danh sách hãng và thành phố cho filter
            ViewBag.Brands = _context.Cars.Where(c => !string.IsNullOrEmpty(c.CarBrand)).Select(c => c.CarBrand!).Distinct().ToList();
            ViewBag.Cities = new[] { "Hà Nội", "TP. Hồ Chí Minh", "Đà Nẵng", "Đà Lạt", "Nha Trang" };
            ViewBag.FilterQ = q;
            ViewBag.FilterBrand = brand;
            ViewBag.FilterCity = city;
            ViewBag.FilterOwner = owner;

            return View(pendingCars);
        }

        // POST: /Admin/Cars/Approve (Phê duyệt xe)
        [HttpPost]
        public IActionResult Approve(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var car = _context.Cars.FirstOrDefault(c => c.CarId == id);
            if (car == null)
            {
                return Json(new { success = false, message = "Không tìm thấy xe." });
            }

            car.CarStatus = "Sẵn sàng";
            car.IsActive = true;
            _context.SaveChanges();

            return Json(new { success = true, message = $"Đã phê duyệt xe {car.CarName} ({car.LicensePlate}). Xe hiện đã hiển thị công khai trên sàn ReCar." });
        }

        // POST: /Admin/Cars/Reject (Từ chối duyệt xe)
        [HttpPost]
        public IActionResult Reject(int id, string reason)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var car = _context.Cars.FirstOrDefault(c => c.CarId == id);
            if (car == null)
            {
                return Json(new { success = false, message = "Không tìm thấy xe." });
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return Json(new { success = false, message = "Vui lòng nhập lý do từ chối hồ sơ." });
            }

            car.CarStatus = "Từ chối";
            car.Descriptions = $"Lý do từ chối: {reason.Trim()} (Cập nhật lúc {DateTime.Now:dd/MM/yyyy HH:mm})";
            _context.SaveChanges();

            return Json(new { success = true, message = $"Đã từ chối duyệt xe {car.LicensePlate} và gửi thông báo yêu cầu chỉnh sửa đến chủ xe." });
        }

        // GET: /Admin/Cars (Quản lý toàn bộ đội xe)
        public IActionResult Index(string? q, string? brand, string? status, string? city)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var query = _context.Cars
                .Include(c => c.Type)
                .Include(c => c.Owner)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                string searchKey = q.Trim().ToLower();
                query = query.Where(c => (c.CarName != null && c.CarName.ToLower().Contains(searchKey))
                                      || (c.LicensePlate != null && c.LicensePlate.ToLower().Contains(searchKey)));
            }

            if (!string.IsNullOrWhiteSpace(brand) && brand != "__all")
            {
                query = query.Where(c => c.CarBrand == brand);
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "__all")
            {
                query = query.Where(c => c.CarStatus == status);
            }

            if (!string.IsNullOrWhiteSpace(city) && city != "__all")
            {
                query = query.Where(c => c.Location != null && c.Location.Contains(city));
            }

            var list = query.OrderByDescending(c => c.CarId).ToList();

            ViewBag.Brands = _context.Cars.Where(c => !string.IsNullOrEmpty(c.CarBrand)).Select(c => c.CarBrand!).Distinct().ToList();
            ViewBag.Cities = new[] { "Hà Nội", "TP. Hồ Chí Minh", "Đà Nẵng", "Đà Lạt", "Nha Trang" };
            ViewBag.FilterQ = q;
            ViewBag.FilterBrand = brand;
            ViewBag.FilterStatus = status;
            ViewBag.FilterCity = city;

            return View(list);
        }

        // POST: /Admin/Cars/ToggleStatus
        [HttpPost]
        public IActionResult ToggleStatus(int id, string status)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var car = _context.Cars.FirstOrDefault(c => c.CarId == id);
            if (car == null)
            {
                return Json(new { success = false, message = "Không tìm thấy xe." });
            }

            car.CarStatus = status;
            _context.SaveChanges();

            return Json(new { success = true, message = $"Đã cập nhật trạng thái xe {car.LicensePlate} sang '{status}'." });
        }
    }
}
