using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Utilities;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CarBrandsController : Controller
    {
        private readonly DbRenalCarContext _context;

        public CarBrandsController(DbRenalCarContext context)
        {
            _context = context;
        }

        // GET: Admin/CarBrands
        public async Task<IActionResult> Index()
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            var brands = await _context.CarBrands
                .OrderBy(b => b.DisplayOrder)
                .ThenBy(b => b.BrandId)
                .ToListAsync();

            // Đếm số lượng xe trên sàn thuộc từng hãng
            var allCars = await _context.Cars
                .Where(c => !string.IsNullOrEmpty(c.CarBrand))
                .Select(c => c.CarBrand!.Trim().ToLower())
                .ToListAsync();

            var carCounts = allCars
                .GroupBy(b => b)
                .ToDictionary(g => g.Key, g => g.Count());

            ViewBag.CarCounts = carCounts;

            return View(brands);
        }

        // GET: Admin/CarBrands/Create
        public IActionResult Create()
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            // Gợi ý thứ tự hiển thị tiếp theo
            int nextOrder = 1;
            if (_context.CarBrands.Any())
            {
                nextOrder = _context.CarBrands.Max(b => b.DisplayOrder) + 1;
            }

            var brand = new CarBrand
            {
                DisplayOrder = nextOrder,
                IsActive = true
            };

            return View(brand);
        }

        // POST: Admin/CarBrands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarBrand brand)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrWhiteSpace(brand.BrandName))
            {
                ModelState.AddModelError("BrandName", "Vui lòng nhập tên hãng xe.");
            }
            else
            {
                // Kiểm tra trùng tên hãng
                bool exists = await _context.CarBrands.AnyAsync(b => b.BrandName.ToLower() == brand.BrandName.Trim().ToLower());
                if (exists)
                {
                    ModelState.AddModelError("BrandName", $"Hãng xe '{brand.BrandName.Trim()}' đã tồn tại trong hệ thống.");
                }
            }

            if (ModelState.IsValid)
            {
                brand.BrandName = brand.BrandName.Trim();
                brand.Country = brand.Country?.Trim();

                _context.Add(brand);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đã thêm mới hãng xe '{brand.BrandName}' thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(brand);
        }

        // GET: Admin/CarBrands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null) return NotFound();

            var brand = await _context.CarBrands.FindAsync(id);
            if (brand == null) return NotFound();

            // Đếm số lượng xe thuộc hãng này
            int carCount = await _context.Cars
                .CountAsync(c => c.CarBrand != null && c.CarBrand.ToLower() == brand.BrandName.ToLower());
            ViewBag.CarCount = carCount;

            return View(brand);
        }

        // POST: Admin/CarBrands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarBrand brand)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != brand.BrandId) return NotFound();

            if (string.IsNullOrWhiteSpace(brand.BrandName))
            {
                ModelState.AddModelError("BrandName", "Vui lòng nhập tên hãng xe.");
            }
            else
            {
                // Kiểm tra trùng tên với hãng khác
                bool exists = await _context.CarBrands
                    .AnyAsync(b => b.BrandId != id && b.BrandName.ToLower() == brand.BrandName.Trim().ToLower());
                if (exists)
                {
                    ModelState.AddModelError("BrandName", $"Hãng xe '{brand.BrandName.Trim()}' đã tồn tại trong hệ thống.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.CarBrands.FindAsync(id);
                    if (existing == null) return NotFound();

                    string oldName = existing.BrandName;
                    existing.BrandName = brand.BrandName.Trim();
                    existing.Country = brand.Country?.Trim();
                    existing.DisplayOrder = brand.DisplayOrder;
                    existing.IsActive = brand.IsActive;

                    // Nếu đổi tên hãng, đồng bộ cập nhật tên hãng trên các xe tương ứng
                    if (!string.Equals(oldName, existing.BrandName, StringComparison.OrdinalIgnoreCase))
                    {
                        var affectedCars = await _context.Cars
                            .Where(c => c.CarBrand != null && c.CarBrand.ToLower() == oldName.ToLower())
                            .ToListAsync();

                        foreach (var car in affectedCars)
                        {
                            car.CarBrand = existing.BrandName;
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã cập nhật hãng xe '{existing.BrandName}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.CarBrands.Any(e => e.BrandId == brand.BrandId))
                        return NotFound();
                    else
                        throw;
                }
            }

            int count = await _context.Cars
                .CountAsync(c => c.CarBrand != null && c.CarBrand.ToLower() == brand.BrandName.ToLower());
            ViewBag.CarCount = count;

            return View(brand);
        }

        // POST: Admin/CarBrands/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            var brand = await _context.CarBrands.FindAsync(id);
            if (brand == null) return NotFound();

            // Kiểm tra ràng buộc xe sàn
            int carCount = await _context.Cars
                .CountAsync(c => c.CarBrand != null && c.CarBrand.ToLower() == brand.BrandName.ToLower());

            if (carCount > 0)
            {
                TempData["Error"] = $"Không thể xóa hãng xe '{brand.BrandName}' vì đang có {carCount} xe thuộc thương hiệu này trên sàn!";
                return RedirectToAction(nameof(Index));
            }

            _context.CarBrands.Remove(brand);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã xóa hãng xe '{brand.BrandName}' khỏi hệ thống thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/CarBrands/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            if (!Function.IsLogin())
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var brand = await _context.CarBrands.FindAsync(id);
            if (brand == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hãng xe" });
            }

            brand.IsActive = !brand.IsActive;
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                isActive = brand.IsActive,
                message = brand.IsActive ? $"Đã kích hoạt hãng xe '{brand.BrandName}'" : $"Đã ẩn hãng xe '{brand.BrandName}'"
            });
        }

        // GET: Admin/CarBrands/GetCarsByBrand?brandName=Toyota
        [HttpGet]
        public async Task<IActionResult> GetCarsByBrand(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
            {
                return Json(new { success = false, message = "Tên thương hiệu không hợp lệ" });
            }

            var cars = await _context.Cars
                .Include(c => c.Owner)
                .Include(c => c.Type)
                .Where(c => c.CarBrand != null && c.CarBrand.ToLower() == brandName.Trim().ToLower())
                .OrderByDescending(c => c.CarId)
                .Select(c => new
                {
                    id = c.CarId,
                    name = c.CarName,
                    brand = c.CarBrand,
                    plate = c.LicensePlate,
                    image = string.IsNullOrEmpty(c.Image) ? "/images/cars/hero.jpg" : c.Image,
                    price = c.Price,
                    salePrice = c.SalePrice,
                    seat = c.Seat,
                    model = c.Model,
                    status = c.CarStatus ?? "Sẵn sàng",
                    isActive = c.IsActive,
                    color = c.Color ?? "Chưa rõ",
                    transmission = c.Transmission ?? "Tự động",
                    fuel = c.Fuel ?? "Xăng",
                    ownerName = c.Owner != null ? (c.Owner.FullName ?? c.Owner.Name ?? "Chủ xe") : "Hệ thống",
                    ownerPhone = c.Owner != null ? c.Owner.PhoneNumber : "",
                    typeName = c.Type != null ? c.Type.CarTypeName : "Đa dụng"
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                brandName = brandName.Trim(),
                total = cars.Count,
                cars
            });
        }

        // GET: Admin/CarBrands/GetCarDetail?id=26
        [HttpGet]
        public async Task<IActionResult> GetCarDetail(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Owner)
                .Include(c => c.Type)
                .Include(c => c.CarAmenities).ThenInclude(ca => ca.Amenity)
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin xe này" });
            }

            var policy = CarPolicy.Parse(car);
            var amenities = car.CarAmenities != null && car.CarAmenities.Any()
                ? car.CarAmenities.Select(ca => ca.Amenity?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList()
                : new List<string>();

            if (!amenities.Any() && !string.IsNullOrEmpty(car.Details))
            {
                var matchAmen = System.Text.RegularExpressions.Regex.Match(car.Details, @"\[Tiện nghi\]:\s*([^\[\r\n]+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (matchAmen.Success)
                {
                    amenities = matchAmen.Groups[1].Value.Split(new[] { ',', '·' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();
                }
            }

            string cleanDesc = CarPolicy.CleanExtraDescription(car.Descriptions ?? car.Details);

            return Json(new
            {
                success = true,
                car = new
                {
                    id = car.CarId,
                    name = car.CarName,
                    brand = car.CarBrand,
                    plate = car.LicensePlate,
                    image = string.IsNullOrEmpty(car.Image) ? "/images/cars/hero.jpg" : car.Image,
                    model = car.Model,
                    seat = car.Seat,
                    color = car.Color ?? "Chưa rõ",
                    transmission = car.Transmission ?? "Tự động",
                    fuel = car.Fuel ?? "Xăng",
                    status = car.CarStatus ?? "Sẵn sàng",
                    isActive = car.IsActive,
                    trips = car.Trips ?? 0,
                    rating = car.Rate ?? 5.0,
                    price = car.Price,
                    salePrice = car.SalePrice,
                    driverFee = policy.DriverFee,
                    mileageLimit = policy.MileageLimit,
                    extraFeePerKm = policy.ExtraFeePerKm,
                    deliverySupported = policy.DeliverySupported,
                    deliveryFee = policy.DeliveryFee,
                    fuelConsumption = policy.FuelConsumption,
                    typeName = car.Type?.CarTypeName ?? "Tiêu chuẩn",
                    ownerName = car.Owner != null ? (car.Owner.FullName ?? car.Owner.Name ?? "Chủ xe") : "Hệ thống",
                    ownerPhone = car.Owner?.PhoneNumber ?? "Chưa cập nhật",
                    ownerEmail = car.Owner?.Email ?? "",
                    amenities = amenities,
                    description = cleanDesc,
                    image1 = car.Image ?? "/images/cars/hero.jpg",
                    image2 = !string.IsNullOrEmpty(car.RegistrationImage) ? car.RegistrationImage : car.Image,
                    image3 = !string.IsNullOrEmpty(car.InspectionImage) ? car.InspectionImage : car.Image,
                    image4 = !string.IsNullOrEmpty(car.InsuranceImage) ? car.InsuranceImage : car.Image
                }
            });
        }
    }
}

