using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Utilities;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CarTypesController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarTypesController(DbRenalCarContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/CarTypes
        public async Task<IActionResult> Index()
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            var carTypes = await _context.CarTypes
                .Include(t => t.Cars)
                .OrderBy(t => t.TypeId)
                .ToListAsync();

            return View(carTypes);
        }

        // GET: Admin/CarTypes/Create
        public IActionResult Create()
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        // POST: Admin/CarTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarType carType, IFormFile? imageFile)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadedPath = await ProcessUploadedImageAsync(imageFile);
                if (!string.IsNullOrEmpty(uploadedPath))
                {
                    carType.Image = uploadedPath;
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(carType);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã tạo mới kiểu xe '{carType.CarTypeName}' thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(carType);
        }

        // GET: Admin/CarTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null) return NotFound();

            var carType = await _context.CarTypes
                .Include(t => t.Cars)
                .FirstOrDefaultAsync(t => t.TypeId == id);
            if (carType == null) return NotFound();

            return View(carType);
        }

        // POST: Admin/CarTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarType carType, IFormFile? imageFile)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != carType.TypeId) return NotFound();

            var existingCarType = await _context.CarTypes.AsNoTracking().FirstOrDefaultAsync(t => t.TypeId == id);
            if (existingCarType == null) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadedPath = await ProcessUploadedImageAsync(imageFile);
                if (!string.IsNullOrEmpty(uploadedPath))
                {
                    carType.Image = uploadedPath;
                }
            }
            else if (string.IsNullOrWhiteSpace(carType.Image))
            {
                // Giữ lại ảnh cũ nếu người dùng không upload file mới và không dán link mới
                carType.Image = existingCarType.Image;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carType);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã cập nhật kiểu xe '{carType.CarTypeName}' thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.CarTypes.Any(e => e.TypeId == carType.TypeId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(carType);
        }

        // POST: Admin/CarTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            var carType = await _context.CarTypes.Include(t => t.Cars).FirstOrDefaultAsync(t => t.TypeId == id);
            if (carType == null) return NotFound();

            if (carType.Cars != null && carType.Cars.Any())
            {
                TempData["Error"] = $"Không thể xóa kiểu xe '{carType.CarTypeName}' vì đang có {carType.Cars.Count} xe thuộc loại này!";
                return RedirectToAction(nameof(Index));
            }

            _context.CarTypes.Remove(carType);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã xóa kiểu xe '{carType.CarTypeName}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/CarTypes/GetCarsByType?typeId=1
        [HttpGet]
        public async Task<IActionResult> GetCarsByType(int typeId)
        {
            var carType = await _context.CarTypes.FindAsync(typeId);
            if (carType == null)
            {
                return Json(new { success = false, message = "Không tìm thấy kiểu xe này" });
            }

            var cars = await _context.Cars
                .Include(c => c.Owner)
                .Include(c => c.Type)
                .Where(c => c.TypeId == typeId)
                .OrderByDescending(c => c.CarId)
                .Select(c => new
                {
                    id = c.CarId,
                    name = c.CarName,
                    brand = c.CarBrand ?? "ReCar",
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
                typeId = carType.TypeId,
                typeName = carType.CarTypeName,
                total = cars.Count,
                cars
            });
        }

        // GET: Admin/CarTypes/GetCarDetail?id=26
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

        /// <summary>
        /// Xử lý tải ảnh kiểu xe lên từ thư viện máy tính và lưu vào wwwroot/images/cartypes
        /// </summary>
        private async Task<string?> ProcessUploadedImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            try
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    return null;
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "cartypes");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"cartype_{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid().ToString("N")[..6]}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return $"/images/cartypes/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CarTypesController] Lỗi lưu ảnh kiểu xe: {ex.Message}");
                return null;
            }
        }
    }
}
