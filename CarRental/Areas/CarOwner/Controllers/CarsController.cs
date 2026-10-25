using CarRental.Models;
using CarRental.Utilities;
using CarRental.ViewModels;
using CarRental.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.CarOwner.Controllers
{
    [Area("CarOwner")]
    public class CarsController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly IHubContext<ReCarHub> _hubContext;

        public CarsController(DbRenalCarContext context, IHubContext<ReCarHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Kiểm tra quyền chủ xe
        private Account? GetCurrentCarOwner()
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return null;
            }

            var accountId = Function.GetAccountId(HttpContext.Session);
            var carOwnerId = Function.GetCarOwnerAccountId(HttpContext.Session);
            var userEmail = Function.GetEmail(HttpContext.Session);

            return _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefault(a => (carOwnerId > 0 && a.AccountId == carOwnerId) ||
                                     a.AccountId == accountId ||
                                     (!string.IsNullOrEmpty(userEmail) && a.Email == userEmail && a.RoleId == 3));
        }

        private bool CheckCarOwnerPermission()
        {
            var owner = GetCurrentCarOwner();
            return owner != null && owner.RoleId == 3; // RoleId 3 = Chủ xe
        }

        public IActionResult Index()
        {
            var owner = GetCurrentCarOwner();
            if (owner == null || owner.RoleId != 3)
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Home", new { area = "CarOwner" });
            }

            var ownerId = owner.AccountId;
            var cars = _context.Cars
                .Include(c => c.Type)
                .Where(c => c.OwnerId == ownerId)
                .OrderByDescending(c => c.CarId)
                .ToList();

            return View(cars);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            ViewBag.CarBrands = _context.CarBrands
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ToList();

            ViewBag.CarTypes = _context.CarTypes
                .Select(c => new SelectListItem
                {
                    Text = c.CarTypeName,
                    Value = c.TypeId.ToString()
                }).ToList();

            // Khởi tạo model mặc định (để trống các lựa chọn để người dùng tự chọn/nhập)
            var model = new CarCreateVM
            {
                Price = 850000,
                MileageLimit = 350,
                ExtraFeePerKm = 3000,
                DeliverySupported = true,
                DeliveryFee = 100000,
                Features = new List<string>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CarCreateVM model)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            if (ModelState.IsValid)
            {
                var currentOwner = GetCurrentCarOwner();
                int ownerId = currentOwner?.AccountId ?? Function.GetAccountId(HttpContext.Session);

                // Xử lý upload ảnh đại diện
                string avatarPath = "/images/cars/car-sedan.jpg";
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string uploadedName = Function.UploadImage(model.ImageFile, "cars");
                    avatarPath = "/files/cars/" + uploadedName;
                }
                else if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    avatarPath = model.ImageUrl.Trim();
                }

                // Xây dựng chuỗi tiện nghi & mô tả
                var featuresStr = model.Features != null && model.Features.Any()
                    ? string.Join(", ", model.Features)
                    : "Đang cập nhật";

                var fullDetails = $"[Tiện nghi]: {featuresStr}\n" +
                                  $"[Quy định quãng đường]: Tối đa {model.MileageLimit} km/ngày (Phụ phí vượt: {model.ExtraFeePerKm:N0}đ/km)\n" +
                                  $"[Giao nhận xe]: {(model.DeliverySupported ? $"Có hỗ trợ giao xe tận nơi (Phí: {model.DeliveryFee:N0}đ)" : "Nhận xe trực tiếp tại bãi")}\n" +
                                  $"[Mô tả bổ sung]: {model.Details}";

                // Xử lý upload ảnh giấy tờ pháp lý của xe
                string? regDocPath = null;
                if (model.RegistrationImageFile != null && model.RegistrationImageFile.Length > 0)
                {
                    string uploaded = Function.UploadImage(model.RegistrationImageFile, "cars");
                    regDocPath = "/files/cars/" + uploaded;
                }

                string? inspDocPath = null;
                if (model.InspectionImageFile != null && model.InspectionImageFile.Length > 0)
                {
                    string uploaded = Function.UploadImage(model.InspectionImageFile, "cars");
                    inspDocPath = "/files/cars/" + uploaded;
                }

                string? insDocPath = null;
                if (model.InsuranceImageFile != null && model.InsuranceImageFile.Length > 0)
                {
                    string uploaded = Function.UploadImage(model.InsuranceImageFile, "cars");
                    insDocPath = "/files/cars/" + uploaded;
                }

                var car = new Car
                {
                    CarName = model.CarName.Trim(),
                    CarBrand = model.CarBrand.Trim(),
                    TypeId = model.TypeId,
                    Model = model.ModelYear,
                    LicensePlate = model.LicensePlate.Trim().ToUpper(),
                    Color = model.Color.Trim(),
                    Seat = model.Seat,
                    Transmission = model.Transmission,
                    Fuel = model.Fuel,
                    Price = model.Price,
                    SalePrice = (model.SalePrice.HasValue && model.SalePrice.Value > 0) ? model.SalePrice : null,
                    IsSale = (model.SalePrice.HasValue && model.SalePrice.Value > 0 && model.SalePrice < model.Price),
                    Location = model.Location.Trim(),
                    Image = avatarPath,
                    RegistrationImage = regDocPath,
                    InspectionImage = inspDocPath,
                    InsuranceImage = insDocPath,
                    Details = fullDetails,
                    Descriptions = !string.IsNullOrEmpty(model.FuelConsumption) ? $"Tiêu hao: {model.FuelConsumption} · {featuresStr}" : featuresStr,
                    CarStatus = "Chờ duyệt",
                    IsActive = false,
                    OwnerId = ownerId,
                    Trips = 0,
                    Rate = 5.0,
                    Alias = Function.TitleSlugGenerationAlias(model.CarName)
                };

                _context.Cars.Add(car);
                _context.SaveChanges();

                // Lưu các ảnh chi tiết vào CarImage
                if (model.DetailImages != null && model.DetailImages.Any())
                {
                    foreach (var imgFile in model.DetailImages)
                    {
                        if (imgFile.Length > 0)
                        {
                            string detailName = Function.UploadImage(imgFile, "cars");
                            _context.CarImages.Add(new CarImage
                            {
                                CarId = car.CarId,
                                Image1 = "/files/cars/" + detailName
                            });
                        }
                    }
                    _context.SaveChanges();
                }

                Function.SetMessage(HttpContext.Session, $"Đăng ký xe '{car.CarName}' thành công! Hồ sơ đang ở trạng thái 'Chờ duyệt' để Ban Quản trị ReCar phê duyệt.");

                // Bắn thông báo Realtime SignalR cho Admin
                try
                {
                    _ = _hubContext.Clients.All.SendAsync("ReceiveNewCarPendingNotification", new
                    {
                        carId = car.CarId,
                        carName = car.CarName,
                        licensePlate = car.LicensePlate,
                        ownerName = currentOwner?.FullName ?? "Đối tác Chủ xe"
                    });
                }
                catch (Exception sigEx)
                {
                    Console.WriteLine($"SignalR New Car Broadcast Error: {sigEx.Message}");
                }

                TempData["SuccessMessage"] = $"Đăng ký xe {car.CarName} ({car.LicensePlate}) thành công! Xe đang được chuyển tới Ban Quản trị ReCar thẩm định.";
                return RedirectToAction("Index");
            }

            ViewBag.CarBrands = _context.CarBrands
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ToList();

            ViewBag.CarTypes = _context.CarTypes
                .Select(c => new SelectListItem
                {
                    Text = c.CarTypeName,
                    Value = c.TypeId.ToString()
                }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            if (id == null)
            {
                return NotFound();
            }

            var currentOwner = GetCurrentCarOwner();
            var ownerId = currentOwner?.AccountId ?? Function.GetAccountId(HttpContext.Session);
            var car = _context.Cars.FirstOrDefault(c => c.CarId == id && c.OwnerId == ownerId);
            if (car == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy xe hoặc bạn không có quyền chỉnh sửa");
                return RedirectToAction("Index");
            }

            ViewBag.CarBrands = _context.CarBrands
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ToList();

            ViewBag.CarTypes = _context.CarTypes
                .Select(c => new SelectListItem
                {
                    Text = c.CarTypeName,
                    Value = c.TypeId.ToString(),
                    Selected = c.TypeId == car.TypeId
                }).ToList();

            // Trích xuất danh sách tiện nghi từ Details nếu có
            var featuresList = new List<string>();
            var allKnownFeatures = new[]
            {
                "Bản đồ dẫn đường GPS", "Camera lùi / Camera 360", "Camera hành trình", "Cảm biến áp suất lốp",
                "Apple CarPlay & Android Auto", "Cửa sổ trời Panorama", "Thu phí tự động ETC", "Ghế da cao cấp chỉnh điện",
                "Ghế trẻ em ISOFIX", "Sạc không dây Qi", "Cảm biến va chạm trước/sau", "Cảnh báo điểm mù BSM",
                "Khóa thông minh SmartKey", "Cốp điện thông minh"
            };

            if (!string.IsNullOrEmpty(car.Details))
            {
                foreach (var f in allKnownFeatures)
                {
                    if (car.Details.Contains(f, StringComparison.OrdinalIgnoreCase))
                    {
                        featuresList.Add(f);
                    }
                }
            }

            var model = new CarEditVM
            {
                CarId = car.CarId,
                CarName = car.CarName ?? string.Empty,
                CarStatus = car.CarStatus ?? "Sẵn sàng",
                CarBrand = car.CarBrand ?? string.Empty,
                TypeId = car.TypeId ?? 0,
                ModelYear = car.Model,
                LicensePlate = car.LicensePlate ?? string.Empty,
                Color = car.Color ?? string.Empty,
                Seat = car.Seat,
                Transmission = car.Transmission,
                Fuel = car.Fuel,
                Price = car.Price ?? 0,
                SalePrice = car.SalePrice,
                Location = car.Location ?? string.Empty,
                ExistingImage = car.Image,
                Details = car.Details,
                Features = featuresList,
                Alias = car.Alias,
                RegistrationImage = car.RegistrationImage,
                InspectionImage = car.InspectionImage,
                InsuranceImage = car.InsuranceImage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CarEditVM model)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var currentOwner = GetCurrentCarOwner();
            var ownerId = currentOwner?.AccountId ?? Function.GetAccountId(HttpContext.Session);
            var existingCar = _context.Cars.FirstOrDefault(c => c.CarId == model.CarId && c.OwnerId == ownerId);
            if (existingCar == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy xe hoặc bạn không có quyền chỉnh sửa");
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                // Cập nhật thông tin cơ bản
                existingCar.CarName = model.CarName.Trim();
                existingCar.CarBrand = model.CarBrand.Trim();
                existingCar.TypeId = model.TypeId;
                existingCar.Model = model.ModelYear;
                existingCar.LicensePlate = model.LicensePlate.Trim().ToUpper();
                existingCar.Color = model.Color.Trim();
                existingCar.Seat = model.Seat;
                existingCar.Transmission = model.Transmission;
                existingCar.Fuel = model.Fuel;
                existingCar.Price = model.Price;
                existingCar.SalePrice = (model.SalePrice.HasValue && model.SalePrice.Value > 0) ? model.SalePrice : null;
                existingCar.IsSale = (model.SalePrice.HasValue && model.SalePrice.Value > 0 && model.SalePrice < model.Price);
                existingCar.Location = model.Location.Trim();
                
                // Nếu xe đang ở trạng thái 'Từ chối' thì khi chủ xe cập nhật bổ sung sẽ tự động chuyển về 'Chờ duyệt'
                if (existingCar.CarStatus == "Từ chối" || existingCar.CarStatus == "rejected")
                {
                    existingCar.CarStatus = "Chờ duyệt";
                }
                
                existingCar.Alias = Function.TitleSlugGenerationAlias(model.CarName);

                // Cập nhật ảnh đại diện nếu có tải file mới
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string uploadedName = Function.UploadImage(model.ImageFile, "cars");
                    existingCar.Image = "/files/cars/" + uploadedName;
                }
                else if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    existingCar.Image = model.ImageUrl.Trim();
                }

                // Cập nhật ảnh giấy tờ nếu có tải file mới
                if (model.RegistrationImageFile != null && model.RegistrationImageFile.Length > 0)
                {
                    string uploaded = Function.UploadImage(model.RegistrationImageFile, "cars");
                    existingCar.RegistrationImage = "/files/cars/" + uploaded;
                }
                if (model.InspectionImageFile != null && model.InspectionImageFile.Length > 0)
                {
                    string uploaded = Function.UploadImage(model.InspectionImageFile, "cars");
                    existingCar.InspectionImage = "/files/cars/" + uploaded;
                }
                if (model.InsuranceImageFile != null && model.InsuranceImageFile.Length > 0)
                {
                    string uploaded = Function.UploadImage(model.InsuranceImageFile, "cars");
                    existingCar.InsuranceImage = "/files/cars/" + uploaded;
                }

                // Xây dựng chuỗi tiện nghi & mô tả
                var featuresStr = model.Features != null && model.Features.Any()
                    ? string.Join(", ", model.Features)
                    : "Đang cập nhật";

                existingCar.Details = $"[Tiện nghi]: {featuresStr}\n" +
                                      $"[Quy định quãng đường]: Tối đa {model.MileageLimit} km/ngày (Phụ phí vượt: {model.ExtraFeePerKm:N0}đ/km)\n" +
                                      $"[Giao nhận xe]: {(model.DeliverySupported ? $"Có hỗ trợ giao xe tận nơi (Phí: {model.DeliveryFee:N0}đ)" : "Nhận xe trực tiếp tại bãi")}\n" +
                                      $"[Mô tả bổ sung]: {model.Details}";

                existingCar.Descriptions = !string.IsNullOrEmpty(model.FuelConsumption) ? $"Tiêu hao: {model.FuelConsumption} · {featuresStr}" : featuresStr;

                _context.Cars.Update(existingCar);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Cập nhật thông tin phương tiện '{existingCar.CarName}' thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.CarBrands = _context.CarBrands
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ToList();

            ViewBag.CarTypes = _context.CarTypes
                .Select(c => new SelectListItem
                {
                    Text = c.CarTypeName,
                    Value = c.TypeId.ToString(),
                    Selected = c.TypeId == model.TypeId
                }).ToList();

            model.ExistingImage = existingCar.Image;
            return View(model);
        }

        public IActionResult Delete(int? id)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            if (id == null)
            {
                return NotFound();
            }

            var ownerId = Function.GetAccountId(HttpContext.Session);
            var car = _context.Cars
                .Include(c => c.Type)
                .FirstOrDefault(c => c.CarId == id && c.OwnerId == ownerId);

            if (car == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy xe hoặc bạn không có quyền xóa");
                return RedirectToAction("Index");
            }

            return View(car);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var car = _context.Cars.FirstOrDefault(c => c.CarId == id && c.OwnerId == Function.GetAccountId(HttpContext.Session));
            if (car == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy xe hoặc bạn không có quyền xóa";
                return RedirectToAction("Index");
            }

            // Xóa các ảnh liên quan
            var carImages = _context.CarImages.Where(ci => ci.CarId == id).ToList();
            if (carImages.Any())
            {
                _context.CarImages.RemoveRange(carImages);
            }

            _context.Cars.Remove(car);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Xóa xe thành công";
            return RedirectToAction("Index");
        }

        public IActionResult Details(int? id)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            if (id == null)
            {
                return NotFound();
            }

            var ownerId = Function.GetAccountId(HttpContext.Session);
            var car = _context.Cars
                .Include(c => c.Type)
                .Include(c => c.CarReviews)
                .FirstOrDefault(c => c.CarId == id && c.OwnerId == ownerId);

            if (car == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy xe");
                return RedirectToAction("Index");
            }

            return View(car);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            if (!CheckCarOwnerPermission())
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập" });
            }

            var ownerId = Function.GetAccountId(HttpContext.Session);
            var car = _context.Cars.FirstOrDefault(c => c.CarId == id && c.OwnerId == ownerId);
            if (car == null)
            {
                return Json(new { success = false, message = "Không tìm thấy xe" });
            }

            car.IsActive = !car.IsActive;
            _context.Cars.Update(car);
            _context.SaveChanges();

            return Json(new { success = true, isActive = car.IsActive, message = car.IsActive ? "Đã kích hoạt xe" : "Đã tạm dừng xe" });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult ToggleSale(int id)
        {
            if (!CheckCarOwnerPermission())
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập" });
            }

            var ownerId = Function.GetAccountId(HttpContext.Session);
            var car = _context.Cars.FirstOrDefault(c => c.CarId == id && c.OwnerId == ownerId);
            if (car == null)
            {
                return Json(new { success = false, message = "Không tìm thấy xe" });
            }

            car.IsSale = !car.IsSale;
            _context.Cars.Update(car);
            _context.SaveChanges();

            return Json(new { success = true, isSale = car.IsSale, message = car.IsSale ? "Đã bật giảm giá" : "Đã tắt giảm giá" });
        }
    }
}

