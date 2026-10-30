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

            ViewBag.Locations = _context.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.DisplayOrder)
                .ToList();

            ViewBag.Amenities = _context.Amenities
                .Where(a => a.IsActive)
                .OrderBy(a => a.DisplayOrder)
                .ToList();

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
                var createAmenityNames = model.AmenityIds != null && model.AmenityIds.Any() ? _context.Amenities.Where(a => model.AmenityIds.Contains(a.AmenityId)).Select(a => a.Name).ToList() : (model.Features ?? new List<string>());
                var featuresStr = createAmenityNames.Any()
                    ? string.Join(", ", createAmenityNames)
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

                // Lưu liên kết tiện ích vào CarAmenities
                if (model.AmenityIds != null && model.AmenityIds.Any())
                {
                    foreach (var aId in model.AmenityIds.Distinct())
                    {
                        _context.CarAmenities.Add(new CarAmenity
                        {
                            CarId = car.CarId,
                            AmenityId = aId
                        });
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

            ViewBag.Locations = _context.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.DisplayOrder)
                .ToList();

            ViewBag.Amenities = _context.Amenities
                .Where(a => a.IsActive)
                .OrderBy(a => a.DisplayOrder)
                .ToList();

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

            var allAmenities = _context.Amenities
                .Where(a => a.IsActive)
                .OrderBy(a => a.DisplayOrder)
                .ToList();
            ViewBag.Amenities = allAmenities;

            // Lấy danh sách AmenityId đã lưu trong bảng CarAmenities
            var selectedAmenityIds = _context.CarAmenities
                .Where(ca => ca.CarId == car.CarId)
                .Select(ca => ca.AmenityId)
                .ToList();

            // Nếu xe cũ chưa có bản ghi trong CarAmenities, tự động map từ chuỗi Details
            var featuresList = new List<string>();
            if (selectedAmenityIds.Any())
            {
                featuresList = allAmenities
                    .Where(a => selectedAmenityIds.Contains(a.AmenityId))
                    .Select(a => a.Name)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(car.Details))
            {
                foreach (var amen in allAmenities)
                {
                    if (car.Details.Contains(amen.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedAmenityIds.Add(amen.AmenityId);
                        featuresList.Add(amen.Name);
                    }
                }
            }

            var detailImages = _context.CarImages
                .Where(ci => ci.CarId == car.CarId)
                .Select(ci => ci.Image1)
                .ToList();

            var policy = CarPolicy.Parse(car);
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
                AmenityIds = selectedAmenityIds,
                Alias = car.Alias,
                RegistrationImage = car.RegistrationImage,
                InspectionImage = car.InspectionImage,
                InsuranceImage = car.InsuranceImage,
                DetailImageUrls = detailImages,
                FuelConsumption = policy.FuelConsumption,
                MileageLimit = policy.MileageLimit,
                ExtraFeePerKm = policy.ExtraFeePerKm,
                DeliverySupported = policy.DeliverySupported,
                DeliveryFee = policy.DeliveryFee
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
                string currentStatus = existingCar.CarStatus ?? "Sẵn sàng";
                bool isBusy = currentStatus == "Đang bận" || currentStatus == "Đang có khách" || currentStatus == "busy";

                // Kiểm tra xem chủ xe có sửa đổi các thông tin định danh nhạy cảm (cần Admin duyệt lại) hay không
                bool isSensitiveChanged = 
                    (!string.Equals(existingCar.CarName?.Trim(), model.CarName?.Trim(), StringComparison.OrdinalIgnoreCase)) ||
                    (!string.Equals(existingCar.CarBrand?.Trim(), model.CarBrand?.Trim(), StringComparison.OrdinalIgnoreCase)) ||
                    (existingCar.TypeId != model.TypeId) ||
                    (existingCar.Model != model.ModelYear) ||
                    (!string.Equals(existingCar.LicensePlate?.Trim().ToUpper(), model.LicensePlate?.Trim().ToUpper(), StringComparison.OrdinalIgnoreCase)) ||
                    (existingCar.Seat != model.Seat) ||
                    (model.ImageFile != null && model.ImageFile.Length > 0) ||
                    (!string.IsNullOrEmpty(model.ImageUrl) && !string.Equals(existingCar.Image, model.ImageUrl.Trim(), StringComparison.OrdinalIgnoreCase)) ||
                    (model.RegistrationImageFile != null && model.RegistrationImageFile.Length > 0) ||
                    (model.InspectionImageFile != null && model.InspectionImageFile.Length > 0) ||
                    (model.InsuranceImageFile != null && model.InsuranceImageFile.Length > 0);

                // Cập nhật thông tin cơ bản
                existingCar.CarName = model.CarName.Trim();
                existingCar.CarBrand = model.CarBrand.Trim();
                existingCar.TypeId = model.TypeId;
                existingCar.Model = model.ModelYear;
                
                // Nếu xe đang bận, giữ nguyên biển số và địa điểm để bảo vệ hợp đồng đang chạy
                if (!isBusy)
                {
                    existingCar.LicensePlate = model.LicensePlate.Trim().ToUpper();
                    existingCar.Location = model.Location.Trim();
                    existingCar.Price = model.Price;
                }
                
                existingCar.Color = model.Color.Trim();
                existingCar.Seat = model.Seat;
                existingCar.Transmission = model.Transmission;
                existingCar.Fuel = model.Fuel;
                existingCar.SalePrice = (model.SalePrice.HasValue && model.SalePrice.Value > 0) ? model.SalePrice : null;
                existingCar.IsSale = (model.SalePrice.HasValue && model.SalePrice.Value > 0 && model.SalePrice < existingCar.Price);
                existingCar.Alias = Function.TitleSlugGenerationAlias(model.CarName);

                string statusMessage = "";

                // Xử lý chuyển đổi trạng thái xe thông minh
                if (currentStatus == "Từ chối" || currentStatus == "rejected")
                {
                    existingCar.CarStatus = "Chờ duyệt";
                    existingCar.IsActive = false;
                    statusMessage = $"Hồ sơ xe '{existingCar.CarName}' đã được gửi lại cho Admin thẩm định!";
                }
                else if (isBusy)
                {
                    // Xe đang chạy chuyến, giữ nguyên trạng thái bận
                    statusMessage = $"Đã cập nhật thông tin xe '{existingCar.CarName}'! Đơn giá và biển số được giữ nguyên bảo vệ chuyến đi.";
                }
                else if (isSensitiveChanged)
                {
                    // Xe sửa đổi thông tin định danh/ảnh giấy tờ -> Chuyển sang 'Chờ cập nhật' để Admin duyệt lại
                    existingCar.CarStatus = "Chờ cập nhật";
                    existingCar.IsActive = false;
                    statusMessage = $"Đã lưu thay đổi! Vì bạn đã sửa đổi thông tin định danh/ảnh giấy tờ, xe '{existingCar.CarName}' được chuyển sang trạng thái 'Chờ cập nhật' để Admin phê duyệt lại.";

                    // Bắn thông báo Realtime SignalR cho Admin
                    try
                    {
                        _ = _hubContext.Clients.All.SendAsync("ReceiveNewCarPendingNotification", new
                        {
                            carId = existingCar.CarId,
                            carName = existingCar.CarName,
                            licensePlate = existingCar.LicensePlate,
                            ownerName = currentOwner?.FullName ?? "Đối tác Chủ xe (Cập nhật thông tin)"
                        });
                    }
                    catch { }
                }
                else
                {
                    // Chỉ sửa giá thuê, tiện nghi, mô tả -> Áp dụng ngay lập tức
                    statusMessage = $"Cập nhật giá thuê và thông tin xe '{existingCar.CarName}' thành công! Thay đổi có hiệu lực ngay.";
                }

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

                // Cập nhật độc lập các góc ảnh chi tiết trong bộ sưu tập 4 ảnh
                var existingDetails = _context.CarImages
                    .Where(ci => ci.CarId == existingCar.CarId)
                    .OrderBy(ci => ci.ImageId)
                    .ToList();

                // Slot 2: Đuôi xe
                if (model.DetailImage2 != null && model.DetailImage2.Length > 0)
                {
                    string uploaded = Function.UploadImage(model.DetailImage2, "cars");
                    if (existingDetails.Count >= 1)
                    {
                        existingDetails[0].Image1 = "/files/cars/" + uploaded;
                    }
                    else
                    {
                        _context.CarImages.Add(new CarImage
                        {
                            CarId = existingCar.CarId,
                            Image1 = "/files/cars/" + uploaded
                        });
                    }
                }

                // Slot 3: Khoang lái / Nội thất
                if (model.DetailImage3 != null && model.DetailImage3.Length > 0)
                {
                    string uploaded = Function.UploadImage(model.DetailImage3, "cars");
                    if (existingDetails.Count >= 2)
                    {
                        existingDetails[1].Image1 = "/files/cars/" + uploaded;
                    }
                    else
                    {
                        _context.CarImages.Add(new CarImage
                        {
                            CarId = existingCar.CarId,
                            Image1 = "/files/cars/" + uploaded
                        });
                    }
                }

                // Slot 4: Thân xe / Góc nghiêng
                if (model.DetailImage4 != null && model.DetailImage4.Length > 0)
                {
                    string uploaded = Function.UploadImage(model.DetailImage4, "cars");
                    if (existingDetails.Count >= 3)
                    {
                        existingDetails[2].Image1 = "/files/cars/" + uploaded;
                    }
                    else
                    {
                        _context.CarImages.Add(new CarImage
                        {
                            CarId = existingCar.CarId,
                            Image1 = "/files/cars/" + uploaded
                        });
                    }
                }

                // Bổ sung các ảnh chi tiết khác nếu có
                if (model.DetailImages != null && model.DetailImages.Any())
                {
                    foreach (var imgFile in model.DetailImages)
                    {
                        if (imgFile.Length > 0)
                        {
                            string detailName = Function.UploadImage(imgFile, "cars");
                            _context.CarImages.Add(new CarImage
                            {
                                CarId = existingCar.CarId,
                                Image1 = "/files/cars/" + detailName
                            });
                        }
                    }
                }

                // Cập nhật bảng liên kết tiện ích CarAmenities
                var existingCarAmenities = _context.CarAmenities.Where(ca => ca.CarId == existingCar.CarId).ToList();
                _context.CarAmenities.RemoveRange(existingCarAmenities);

                if (model.AmenityIds != null && model.AmenityIds.Any())
                {
                    foreach (var aId in model.AmenityIds.Distinct())
                    {
                        _context.CarAmenities.Add(new CarAmenity
                        {
                            CarId = existingCar.CarId,
                            AmenityId = aId
                        });
                    }
                }
                _context.SaveChanges();

                // Lấy tên tiện ích để đồng bộ vào chuỗi Details & Descriptions tương thích ngược
                var selectedAmenityNames = model.AmenityIds != null && model.AmenityIds.Any()
                    ? _context.Amenities.Where(a => model.AmenityIds.Contains(a.AmenityId)).Select(a => a.Name).ToList()
                    : (model.Features ?? new List<string>());

                var featuresStr = selectedAmenityNames.Any()
                    ? string.Join(", ", selectedAmenityNames)
                    : "Đang cập nhật";

                existingCar.Details = $"[Tiện nghi]: {featuresStr}\n" +
                                      $"[Quy định quãng đường]: Tối đa {model.MileageLimit} km/ngày (Phụ phí vượt: {model.ExtraFeePerKm:N0}đ/km)\n" +
                                      $"[Giao nhận xe]: {(model.DeliverySupported ? $"Có hỗ trợ giao xe tận nơi (Phí: {model.DeliveryFee:N0}đ)" : "Nhận xe trực tiếp tại bãi")}\n" +
                                      $"[Mô tả bổ sung]: {model.Details}";

                existingCar.Descriptions = !string.IsNullOrEmpty(model.FuelConsumption) ? $"Tiêu hao: {model.FuelConsumption} · {featuresStr}" : featuresStr;

                _context.Cars.Update(existingCar);
                _context.SaveChanges();

                TempData["SuccessMessage"] = statusMessage;
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

            ViewBag.Amenities = _context.Amenities
                .Where(a => a.IsActive)
                .OrderBy(a => a.DisplayOrder)
                .ToList();

            model.ExistingImage = existingCar.Image;
            return View(model);
        }

        [HttpGet]
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
                TempData["ErrorMessage"] = "Không tìm thấy xe hoặc bạn không có quyền thao tác";
                return RedirectToAction("Index");
            }

            // 1. Kiểm tra xe có đang bận/phục vụ khách hay không
            bool isBusy = car.CarStatus == "Đang bận" || car.CarStatus == "Đang có khách" || car.CarStatus == "busy";

            // 2. Kiểm tra xem xe có đơn đặt thuê nào chưa hoàn thành không
            var hasActiveOrders = _context.OrderDetails.Any(od => od.CarId == id && od.ReturnDate >= DateTime.Now);

            // 3. Kiểm tra xe đã từng phát sinh lịch sử giao dịch (đơn hàng, đánh giá, số chuyến) hay chưa
            int orderCount = _context.OrderDetails.Count(od => od.CarId == id);
            int reviewCount = _context.CarReviews.Count(cr => cr.CarId == id);
            int trips = car.Trips ?? 0;
            bool hasHistory = orderCount > 0 || reviewCount > 0 || trips > 0;

            ViewBag.IsBusy = isBusy;
            ViewBag.HasActiveOrders = hasActiveOrders;
            ViewBag.HasHistory = hasHistory;
            ViewBag.OrderCount = orderCount;
            ViewBag.ReviewCount = reviewCount;
            ViewBag.Trips = trips;

            var deleteAmenityItems = _context.CarAmenities
                .Include(ca => ca.Amenity)
                .Where(ca => ca.CarId == id && ca.Amenity != null)
                .OrderBy(ca => ca.Amenity!.DisplayOrder)
                .Select(ca => ca.Amenity!)
                .ToList();
            if (!deleteAmenityItems.Any() && !string.IsNullOrEmpty(car.Details))
            {
                var allAm = _context.Amenities.Where(a => a.IsActive).OrderBy(a => a.DisplayOrder).ToList();
                deleteAmenityItems = allAm.Where(a => car.Details.Contains(a.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            ViewBag.AmenityItems = deleteAmenityItems;

            return View(car);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id, string? actionType)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var ownerId = Function.GetAccountId(HttpContext.Session);
            var car = _context.Cars.FirstOrDefault(c => c.CarId == id && c.OwnerId == ownerId);
            if (car == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy xe hoặc bạn không có quyền thao tác";
                return RedirectToAction("Index");
            }

            // 1. Chặn tuyệt đối nếu xe đang bận
            if (car.CarStatus == "Đang bận" || car.CarStatus == "Đang có khách" || car.CarStatus == "busy")
            {
                TempData["ErrorMessage"] = "Xe đang trong hành trình đón khách, không thể ngừng hoạt động hoặc xóa!";
                return RedirectToAction("Index");
            }

            // 2. Chặn nếu có đơn đặt xe chưa hoàn tất
            var hasActiveOrders = _context.OrderDetails.Any(od => od.CarId == id && od.ReturnDate >= DateTime.Now);
            if (hasActiveOrders)
            {
                TempData["ErrorMessage"] = "Xe đang có đơn đặt thuê chưa hoàn thành. Vui lòng hoàn thành chuyến đi trước khi gỡ xe!";
                return RedirectToAction("Index");
            }

            // 3. Nếu chủ xe chỉ muốn Tạm ngưng đón khách
            if (actionType == "pause")
            {
                car.CarStatus = "Tạm ngưng";
                car.IsActive = false;
                _context.Cars.Update(car);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Đã chuyển xe '{car.CarName}' sang trạng thái 'Tạm ngưng đón khách'. Bạn có thể bật lại bất cứ lúc nào.";
                return RedirectToAction("Index");
            }

            // 4. Kiểm tra lịch sử giao dịch: Soft Delete vs Hard Delete
            int orderCount = _context.OrderDetails.Count(od => od.CarId == id);
            int reviewCount = _context.CarReviews.Count(cr => cr.CarId == id);
            int trips = car.Trips ?? 0;
            bool hasHistory = orderCount > 0 || reviewCount > 0 || trips > 0;

            if (hasHistory)
            {
                // Xóa mềm (Soft Delete / Ngừng kinh doanh): Bảo toàn dữ liệu đơn hàng & kế toán
                car.CarStatus = "Ngừng hoạt động";
                car.IsActive = false;
                _context.Cars.Update(car);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Đã rút xe '{car.CarName}' khỏi sàn ReCar (Ngừng kinh doanh). Toàn bộ lịch sử {trips} chuyến đi và doanh thu được bảo lưu an toàn.";
            }
            else
            {
                // Xe mới tạo hoặc chưa có đơn: Xóa hoàn toàn (Hard Delete)
                var carAmenities = _context.CarAmenities.Where(ca => ca.CarId == id).ToList();
                if (carAmenities.Any())
                {
                    _context.CarAmenities.RemoveRange(carAmenities);
                }

                var carImages = _context.CarImages.Where(ci => ci.CarId == id).ToList();
                if (carImages.Any())
                {
                    _context.CarImages.RemoveRange(carImages);
                }

                _context.Cars.Remove(car);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Đã xóa hoàn toàn hồ sơ xe '{car.CarName}' khỏi hệ thống.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            if (car.CarStatus == "Đang bận" || car.CarStatus == "Đang có khách")
            {
                return Json(new { success = false, message = "Xe đang phục vụ khách, không thể đổi trạng thái lúc này" });
            }

            if (car.CarStatus == "Sẵn sàng" || car.CarStatus == "active")
            {
                car.CarStatus = "Tạm ngưng";
                car.IsActive = false;
            }
            else if (car.CarStatus == "Tạm ngưng" || car.CarStatus == "Ngừng hoạt động")
            {
                car.CarStatus = "Sẵn sàng";
                car.IsActive = true;
            }
            else
            {
                return Json(new { success = false, message = $"Xe đang ở trạng thái '{car.CarStatus}', không thể tự chuyển đổi" });
            }

            _context.Cars.Update(car);
            _context.SaveChanges();

            return Json(new { success = true, newStatus = car.CarStatus, message = $"Đã cập nhật trạng thái xe thành '{car.CarStatus}'" });
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

            var owner = GetCurrentCarOwner();
            var ownerId = owner?.AccountId ?? Function.GetAccountId(HttpContext.Session);
            var car = _context.Cars
                .Include(c => c.Type)
                .Include(c => c.CarReviews)
                .FirstOrDefault(c => c.CarId == id && c.OwnerId == ownerId);

            if (car == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy xe");
                return RedirectToAction("Index");
            }

            // Lấy album ảnh chi tiết
            var galleryImages = _context.CarImages
                .Where(ci => ci.CarId == id && !string.IsNullOrEmpty(ci.Image1))
                .Select(ci => ci.Image1!)
                .ToList();
            ViewBag.GalleryImages = galleryImages;

            // Lấy danh sách tiện nghi
            var amenities = _context.CarAmenities
                .Include(ca => ca.Amenity)
                .Where(ca => ca.CarId == id && ca.Amenity != null)
                .Select(ca => ca.Amenity.Name)
                .ToList();
            if (!amenities.Any() && !string.IsNullOrEmpty(car.Details))
            {
                var allAm = _context.Amenities.Where(a => a.IsActive).Select(a => a.Name).ToList();
                amenities = allAm.Where(a => car.Details.Contains(a, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var amenityItems = _context.CarAmenities
                .Include(ca => ca.Amenity)
                .Where(ca => ca.CarId == id && ca.Amenity != null)
                .OrderBy(ca => ca.Amenity!.DisplayOrder)
                .Select(ca => ca.Amenity!)
                .ToList();
            if (!amenityItems.Any() && !string.IsNullOrEmpty(car.Details))
            {
                var allAm = _context.Amenities.Where(a => a.IsActive).OrderBy(a => a.DisplayOrder).ToList();
                amenityItems = allAm.Where(a => car.Details.Contains(a.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            ViewBag.AmenityItems = amenityItems;
            ViewBag.Amenities = amenities;
            ViewBag.CarPolicy = CarPolicy.Parse(car);

            // Lấy lịch sử đơn thuê của xe
            var carOrders = _context.OrderDetails
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Status)
                .Where(od => od.CarId == id)
                .OrderByDescending(od => od.OrderDetailId)
                .Take(10)
                .ToList();
            ViewBag.CarOrders = carOrders;

            return View(car);
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

