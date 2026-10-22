using CarRental.Models;
using CarRental.Utilities;
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var ownerId = owner.AccountId;
            var cars = _context.Cars
                .Include(c => c.Type)
                .Where(c => c.OwnerId == ownerId)
                .OrderByDescending(c => c.CarId)
                .ToList();

            return View(cars);
        }

        public IActionResult Create()
        {
            if (!CheckCarOwnerPermission())
            {
                Function._Message = "Bạn không có quyền truy cập";
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var carTypes = _context.CarTypes
                .Select(c => new SelectListItem
                {
                    Text = c.CarTypeName,
                    Value = c.TypeId.ToString()
                }).ToList();

            carTypes.Insert(0, new SelectListItem
            {
                Text = "--Chọn hạng xe--",
                Value = "0"
            });

            ViewBag.CarTypes = carTypes;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Car car, string filePaths)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            if (ModelState.IsValid)
            {
                var currentOwner = GetCurrentCarOwner();
                car.OwnerId = currentOwner?.AccountId ?? Function.GetAccountId(HttpContext.Session); // Gán chủ xe hiện tại
                car.Alias = Function.TitleSlugGenerationAlias(car.CarName ?? "");
                car.CarStatus = "Chờ duyệt"; // Xe mới cần Admin phê duyệt trước khi công khai

                _context.Cars.Add(car);
                _context.SaveChanges();

                // Xử lý ảnh xe
                if (!string.IsNullOrEmpty(filePaths))
                {
                    var filePathList = filePaths.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var filePath in filePathList)
                    {
                        var carImage = new CarImage
                        {
                            CarId = car.CarId,
                            Image1 = filePath.Trim()
                        };
                        _context.CarImages.Add(carImage);
                    }
                    _context.SaveChanges();
                }

                Function.SetMessage(HttpContext.Session, "Thêm xe thành công! Hồ sơ xe đang ở trạng thái 'Chờ duyệt' để Ban quản trị ReCar kiểm tra trước khi công khai lên sàn.");

                // Phát tín hiệu thời gian thực cho Admin
                try
                {
                    _ = _hubContext.Clients.Group("Role_Admin").SendAsync("ReceiveNewCarPendingNotification", new
                    {
                        carId = car.CarId,
                        carName = car.CarName,
                        licensePlate = car.LicensePlate,
                        ownerName = currentOwner?.FullName ?? "Đối tác Chủ xe"
                    });
                    _ = _hubContext.Clients.Group("Role_Quản trị viên").SendAsync("ReceiveNewCarPendingNotification", new
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

                return RedirectToAction("Index");
            }

            var carTypes = _context.CarTypes
                .Select(c => new SelectListItem
                {
                    Text = c.CarTypeName,
                    Value = c.TypeId.ToString()
                }).ToList();

            carTypes.Insert(0, new SelectListItem
            {
                Text = "--Chọn hạng xe--",
                Value = "0"
            });

            ViewBag.CarTypes = carTypes;
            return View(car);
        }

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

            var ownerId = Function.GetAccountId(HttpContext.Session);
            var car = _context.Cars.FirstOrDefault(c => c.CarId == id && c.OwnerId == ownerId);
            if (car == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy xe hoặc bạn không có quyền chỉnh sửa");
                return RedirectToAction("Index");
            }

            var carTypes = _context.CarTypes
                .Select(c => new SelectListItem
                {
                    Text = c.CarTypeName,
                    Value = c.TypeId.ToString(),
                    Selected = c.TypeId == car.TypeId
                }).ToList();

            carTypes.Insert(0, new SelectListItem
            {
                Text = "--Chọn hạng xe--",
                Value = "0"
            });

            ViewBag.CarTypes = carTypes;
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Car car, string filePaths)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var ownerId = Function.GetAccountId(HttpContext.Session);
            var existingCar = _context.Cars.FirstOrDefault(c => c.CarId == car.CarId && c.OwnerId == ownerId);
            if (existingCar == null)
            {
                Function._Message = "Không tìm thấy xe hoặc bạn không có quyền chỉnh sửa";
                return RedirectToAction("Index");
            }

            try
            {
                // Xóa validation error cho filePaths vì nó không bắt buộc khi edit
                if (ModelState.ContainsKey("filePaths"))
                {
                    ModelState["filePaths"].Errors.Clear();
                }
                
                // Xóa tất cả các key liên quan đến filePaths
                var keysToRemove = ModelState.Keys.Where(k => k.Contains("filePaths", StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                }
                
                if (ModelState.IsValid)
                {
                    existingCar.TypeId = car.TypeId;

                    existingCar.CarName = car.CarName;
                    existingCar.Seat = car.Seat;
                    existingCar.LicensePlate = car.LicensePlate;
                    existingCar.Price = car.Price;
                    existingCar.Color = car.Color;
                    existingCar.Model = car.Model;
                    existingCar.Rate = car.Rate;
                    existingCar.CarBrand = car.CarBrand;
                    existingCar.Image = car.Image;
                    existingCar.SalePrice = car.SalePrice;
                    existingCar.Alias = Function.TitleSlugGenerationAlias(car.CarName ?? "");
                    existingCar.IsSale = car.IsSale;
                    existingCar.IsActive = car.IsActive;
                    existingCar.Details = car.Details;
                    existingCar.Descriptions = car.Descriptions;

                    _context.Cars.Update(existingCar);
                    _context.SaveChanges();

                    // Xử lý ảnh xe nếu có thay đổi
                    if (!string.IsNullOrEmpty(filePaths))
                    {
                        // Xóa các ảnh cũ
                        var oldImages = _context.CarImages.Where(ci => ci.CarId == car.CarId).ToList();
                        _context.CarImages.RemoveRange(oldImages);
                        _context.SaveChanges();

                        // Thêm ảnh mới
                        var filePathList = filePaths.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var filePath in filePathList)
                        {
                            var carImage = new CarImage
                            {
                                CarId = car.CarId,
                                Image1 = filePath.Trim()
                            };
                            _context.CarImages.Add(carImage);
                        }
                        _context.SaveChanges();
                    }

                    TempData["SuccessMessage"] = "Cập nhật xe thành công";
                    return RedirectToAction("Index", new { success = true });
                }
                else
                {
                    // Có lỗi validation - thu thập tất cả lỗi
                    var errors = new List<string>();
                    foreach (var modelError in ModelState)
                    {
                        var fieldName = modelError.Key;
                        foreach (var error in modelError.Value.Errors)
                        {
                            if (!string.IsNullOrEmpty(error.ErrorMessage))
                            {
                                errors.Add($"{fieldName}: {error.ErrorMessage}");
                            }
                            else if (!string.IsNullOrEmpty(error.Exception?.Message))
                            {
                                errors.Add($"{fieldName}: {error.Exception.Message}");
                            }
                        }
                    }
                    
                    var errorMessage = errors.Count > 0 
                        ? string.Join("<br>", errors) 
                        : "Có lỗi xảy ra khi cập nhật xe. Vui lòng kiểm tra lại thông tin.";
                    
                    TempData["ErrorMessage"] = errorMessage;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật xe: " + ex.Message;
                return RedirectToAction("Index");
            }

            var carTypes = _context.CarTypes
                .Select(c => new SelectListItem
                {
                    Text = c.CarTypeName,
                    Value = c.TypeId.ToString(),
                    Selected = c.TypeId == car.TypeId
                }).ToList();

            carTypes.Insert(0, new SelectListItem
            {
                Text = "--Chọn hạng xe--",
                Value = "0"
            });

            ViewBag.CarTypes = carTypes;
            return View(car);
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

