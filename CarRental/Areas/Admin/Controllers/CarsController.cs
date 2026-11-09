using CarRental.Services;
using CarRental.Models;
using CarRental.Utilities;
using CarRental.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CarRental.Areas.Admin.Controllers
{
 [Area("Admin")]
 public class CarsController : Controller
 {
 private readonly DbRenalCarContext _context;
 private readonly IHubContext<ReCarHub> _hubContext;
 private readonly INotificationService _notificationService;

 public CarsController(DbRenalCarContext context, IHubContext<ReCarHub> hubContext, INotificationService notificationService)
 {
 _context = context;
 _hubContext = hubContext;
 _notificationService = notificationService;
 }

 private bool CheckAdmin()
 {
 return Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session);
 }

 // GET: /Admin/Cars/Pending (Phê duyệt xe mới)
 public IActionResult Pending(string? q, string? brand, string? city, string? owner, string? date)
 {
 if (!CheckAdmin())
 {
 return RedirectToAction("Index", "Login", new { area = "Admin" });
 }

            var query = _context.Cars
                .Include(c => c.Type)
                .Include(c => c.Owner)
                .Include(c => c.CarAmenities)
                    .ThenInclude(ca => ca.Amenity)
                .Where(c => c.CarStatus == "Chờ duyệt" || c.CarStatus == "Chờ cập nhật" || c.CarStatus == "pending_update" || c.CarStatus == "pending")
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
 var pendingIds = pendingCars.Select(c => c.CarId).ToList();

 // Nạp thư viện ảnh phụ cho từng xe
 var carImagesList = _context.CarImages.Where(ci => ci.CarId.HasValue && pendingIds.Contains(ci.CarId.Value)).ToList();
 var carImagesDict = carImagesList
 .GroupBy(ci => ci.CarId!.Value)
 .ToDictionary(
 g => g.Key,
 g => g.SelectMany(x => new[] { x.Image1, x.Image2, x.Image3, x.Image4 })
 .Where(img => !string.IsNullOrEmpty(img))
 .Select(img => img!)
 .ToList()
 );
 ViewBag.CarImagesDict = carImagesDict;

 // Danh sách hồ sơ đã từ chối gần đây
 var recentRejected = _context.Cars
 .Include(c => c.Owner)
 .Where(c => c.CarStatus == "Từ chối" || c.CarStatus == "rejected")
 .OrderByDescending(c => c.CarId)
 .Take(10)
 .ToList();
 ViewBag.RecentRejected = recentRejected;

 // Danh sách hãng và thành phố cho filter
 ViewBag.Brands = _context.Cars.Where(c => !string.IsNullOrEmpty(c.CarBrand)).Select(c => c.CarBrand!).Distinct().ToList();
 ViewBag.Cities = new[] { "Hà Nội", "TP. Hồ Chí Minh", "Đà Nẵng", "Đà Lạt", "Nha Trang", "Cần Thơ", "Hải Phòng", "Bình Dương" };
 ViewBag.FilterQ = q;
 ViewBag.FilterBrand = brand;
 ViewBag.FilterCity = city;
 ViewBag.FilterOwner = owner;
 ViewBag.FilterDate = date;

 return View(pendingCars);
 }

 // POST: /Admin/Cars/Approve (Phê duyệt xe)
 [HttpPost]
 public async Task<IActionResult> Approve(int id)
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

 // Gửi thông báo lưu vào CSDL cho Chủ xe
 if (car.OwnerId.HasValue && car.OwnerId.Value > 0)
 {
 await _notificationService.CreateNotificationAsync(
 car.OwnerId.Value,
 "CarOwner",
 "Hồ sơ xe đã được phê duyệt ",
 $"Chúc mừng bạn! Xe {car.CarName} ({car.LicensePlate}) đã được kiểm duyệt thành công và sẵn sàng đón khách thuê trên sàn ReCar.",
 "CarApproved",
 $"/CarOwner/Cars/Details/{car.CarId}"
 );
 }

 // Phát thông báo Realtime cho Chủ xe
 if (car.OwnerId.HasValue && car.OwnerId.Value > 0)
 {
 try
 {
 _ = _hubContext.Clients.Group($"User_{car.OwnerId.Value}").SendAsync("ReceiveCarModerationResult", new
 {
 carId = car.CarId,
 carName = car.CarName,
 licensePlate = car.LicensePlate,
 isApproved = true,
 reason = ""
 });
 }
 catch (Exception ex)
 {
 Console.WriteLine($"SignalR Approve Car Broadcast Error: {ex.Message}");
 }
 }

 return Json(new { success = true, message = $"Đã phê duyệt {car.CarName} ({car.LicensePlate}). Xe sẽ hiển thị công khai khi đủ điều kiện hoạt động." });
 }

 // POST: /Admin/Cars/Reject (Từ chối duyệt xe)
 [HttpPost]
 public async Task<IActionResult> Reject(int id, string reason, bool notify = true)
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

 // Gửi thông báo lưu vào CSDL cho Chủ xe
 if (notify && car.OwnerId.HasValue && car.OwnerId.Value > 0)
 {
 await _notificationService.CreateNotificationAsync(
 car.OwnerId.Value,
 "CarOwner",
 "Hồ sơ xe bị từ chối kiểm duyệt ",
 $"Hồ sơ xe {car.CarName} ({car.LicensePlate}) chưa được duyệt. Lý do: {reason.Trim()}. Vui lòng kiểm tra và chỉnh sửa lại thông tin.",
 "CarRejected",
 $"/CarOwner/Cars/Edit/{car.CarId}"
 );
 }

 // Phát thông báo Realtime cho Chủ xe nếu được yêu cầu
 if (notify && car.OwnerId.HasValue && car.OwnerId.Value > 0)
 {
 try
 {
 _ = _hubContext.Clients.Group($"User_{car.OwnerId.Value}").SendAsync("ReceiveCarModerationResult", new
 {
 carId = car.CarId,
 carName = car.CarName,
 licensePlate = car.LicensePlate,
 isApproved = false,
 reason = reason.Trim()
 });
 }
 catch (Exception ex)
 {
 Console.WriteLine($"SignalR Reject Car Broadcast Error: {ex.Message}");
 }
 }

 string notifyText = notify ? " và gửi thông báo yêu cầu chỉnh sửa tới chủ xe" : "";
 return Json(new { success = true, message = $"Đã từ chối hồ sơ {car.LicensePlate}{notifyText}." });
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
 if (status == "Ngưng hoạt động" || status == "Tạm ngừng")
 {
 query = query.Where(c => c.CarStatus == "Ngưng hoạt động" || c.CarStatus == "Tạm ngừng");
 }
 else
 {
 query = query.Where(c => c.CarStatus == status);
 }
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

 // Đồng bộ triệt để trạng thái hiển thị trên sàn ReCar
 if (status == "Ngưng hoạt động" || status == "Tạm ngừng" || status == "Từ chối" || status == "Chờ duyệt")
 {
 car.IsActive = false; // Ẩn hoàn toàn khỏi sàn
 }
 else if (status == "Sẵn sàng" || status == "Bảo trì" || status == "Đang bận")
 {
 car.IsActive = true;
 }

 _context.SaveChanges();

 string activeDesc = car.IsActive ? "(Đang hiển thị trên sàn)" : "(Đã ẩn khỏi sàn)";
 return Json(new { 
 success = true, 
 isActive = car.IsActive,
 carStatus = car.CarStatus,
 message = $"Đã cập nhật trạng thái xe {car.LicensePlate} sang '{status}' {activeDesc}." 
 });
 }

        // POST: /Admin/Cars/Delete (Xóa xe vĩnh viễn khỏi hệ thống)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var car = await _context.Cars
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
            {
                return Json(new { success = false, message = "Không tìm thấy xe." });
            }

            bool hasActiveOrders = await _context.OrderDetails
                .Include(od => od.Order)
                .AnyAsync(od => od.CarId == id && od.Order != null && 
                    (od.Order.StatusId == 1 || od.Order.StatusId == 2 || od.Order.StatusId == 3));

            if (hasActiveOrders)
            {
                return Json(new { success = false, message = "Xe đang có hợp đồng hoặc đơn thuê đang hoạt động. Không thể xóa xe lúc này." });
            }

            string carName = car.CarName ?? "Phương tiện";
            string licensePlate = car.LicensePlate ?? "";
            int? ownerId = car.OwnerId;

            var images = _context.CarImages.Where(ci => ci.CarId == id);
            _context.CarImages.RemoveRange(images);

            var amenities = _context.CarAmenities.Where(ca => ca.CarId == id);
            _context.CarAmenities.RemoveRange(amenities);

            var blocks = _context.CarCalendarBlocks.Where(cb => cb.CarId == id);
            _context.CarCalendarBlocks.RemoveRange(blocks);

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            if (ownerId.HasValue && ownerId.Value > 0)
            {
                await _notificationService.CreateNotificationAsync(
                    ownerId.Value,
                    "CarOwner",
                    "Xe đã bị xóa khỏi sàn ReCar",
                    $"Xe {carName} ({licensePlate}) đã được Quản trị viên xóa khỏi hệ thống.",
                    "System",
                    "/CarOwner/Cars"
                );
            }

            return Json(new { success = true, message = $"Đã xóa thành công xe {carName} ({licensePlate}) khỏi hệ thống." });
        }

        // POST: /Admin/Cars/ApproveStatusRequest (Duyệt hoặc từ chối yêu cầu chuyển Bảo trì / Tạm ngưng từ Chủ xe)
        [HttpPost]
        public async Task<IActionResult> ApproveStatusRequest(int id, bool isApproved, string? note)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == id);
            if (car == null)
            {
                return Json(new { success = false, message = "Không tìm thấy xe." });
            }

            string currentStatus = car.CarStatus ?? "";
            bool isUpdate = (currentStatus == "Chờ cập nhật" || currentStatus == "pending_update");

            if (isApproved)
            {
                if (currentStatus == "Chờ bảo trì")
                {
                    car.CarStatus = "Bảo trì";
                    car.IsActive = false;
                }
                else if (currentStatus == "Chờ tạm ngưng")
                {
                    car.CarStatus = "Ngưng hoạt động";
                    car.IsActive = false;
                }
                else
                {
                    // Chờ cập nhật hoặc yêu cầu kích hoạt lại -> Chuyển sang Sẵn sàng và hiển thị trên sàn
                    car.CarStatus = "Sẵn sàng";
                    car.IsActive = true;
                }
            }
            else
            {
                if (isUpdate)
                {
                    car.CarStatus = "Từ chối";
                    car.IsActive = false;
                }
                else
                {
                    car.CarStatus = "Sẵn sàng";
                    car.IsActive = true;
                }

                if (!string.IsNullOrWhiteSpace(note))
                {
                    car.Descriptions = $"Yêu cầu bị từ chối lúc {DateTime.Now:dd/MM/yyyy HH:mm}: {note.Trim()}";
                }
            }

            await _context.SaveChangesAsync();

            if (car.OwnerId.HasValue && car.OwnerId.Value > 0)
            {
                string title;
                string content;

                if (isUpdate)
                {
                    title = isApproved
                        ? "Hồ sơ cập nhật xe đã được phê duyệt"
                        : "Hồ sơ cập nhật xe bị từ chối";
                    content = isApproved
                        ? $"Ban Quản trị ReCar đã duyệt các thông tin cập nhật của xe {car.CarName} ({car.LicensePlate}). Xe đã kích hoạt Sẵn sàng đón khách trên sàn!"
                        : $"Yêu cầu cập nhật thông tin xe {car.CarName} ({car.LicensePlate}) không được chấp thuận. Lý do: {note ?? "Chưa phù hợp quy chuẩn sàn"}.";
                }
                else
                {
                    title = isApproved 
                        ? "Yêu cầu thay đổi trạng thái xe đã được phê duyệt" 
                        : "Yêu cầu thay đổi trạng thái xe bị từ chối";
                    content = isApproved
                        ? $"Ban Quản trị ReCar đã chấp thuận yêu cầu chuyển xe {car.CarName} ({car.LicensePlate}) sang trạng thái '{car.CarStatus}'."
                        : $"Yêu cầu chuyển trạng thái cho xe {car.CarName} ({car.LicensePlate}) không được chấp thuận. Lý do: {note ?? "Không đáp ứng điều kiện"}";
                }

                await _notificationService.CreateNotificationAsync(
                    car.OwnerId.Value,
                    "CarOwner",
                    title,
                    content,
                    isApproved ? "CarApproved" : "CarRejected",
                    $"/CarOwner/Cars/Details/{car.CarId}"
                );
            }

            return Json(new { 
                success = true, 
                newStatus = car.CarStatus, 
                isActive = car.IsActive, 
                message = isApproved 
                    ? $"Đã phê duyệt thành công. Xe '{car.CarName}' đã chuyển sang trạng thái '{car.CarStatus}'." 
                    : "Đã từ chối yêu cầu và thông báo tới chủ xe."
            });
        }

        // GET: /Admin/Cars/GetOwnerInfo (Lấy thông tin chủ xe phục vụ popup liên hệ)
        [HttpGet]
        public async Task<IActionResult> GetOwnerInfo(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var owner = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (owner == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin chủ xe." });
            }

            var ownerCars = await _context.Cars
                .Where(c => c.OwnerId == id)
                .Select(c => new
                {
                    c.CarId,
                    c.CarName,
                    c.LicensePlate,
                    c.CarStatus,
                    c.Image
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                owner = new
                {
                    customerId = owner.CustomerId,
                    fullName = owner.FullName ?? owner.Name ?? "Chủ xe ReCar",
                    username = owner.Username ?? "",
                    phone = owner.Phone ?? "Chưa cập nhật",
                    email = owner.Email ?? "Chưa cập nhật",
                    address = owner.Address ?? "Chưa cập nhật",
                    avatar = owner.Avatar ?? "/images/customers/customer-1.jpg",
                    carCount = ownerCars.Count,
                    cars = ownerCars
                }
            });
        }

        // GET: /Admin/Cars/GetStatusRequest?id={id} (Xem chi tiết yêu cầu chuyển trạng thái của Chủ xe)
        [HttpGet]
        public async Task<IActionResult> GetStatusRequest(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var car = await _context.Cars
                .Include(c => c.Owner)
                .Include(c => c.Type)
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin xe." });
            }

            var policy = CarPolicy.Parse(car);
            var amenities = await _context.CarAmenities
                .Where(ca => ca.CarId == car.CarId)
                .Join(_context.Amenities, ca => ca.AmenityId, a => a.AmenityId, (ca, a) => a.Name)
                .ToListAsync();

            string? oldSnapshotJson = CarPolicy.ExtractOldSnapshotJson(car.Details);

            bool isUpdate = (car.CarStatus == "Chờ cập nhật" || car.CarStatus == "pending_update");

            return Json(new
            {
                success = true,
                carId = car.CarId,
                carName = car.CarName,
                carBrand = car.CarBrand,
                licensePlate = car.LicensePlate,
                carStatus = car.CarStatus,
                descriptions = car.Descriptions ?? "Chủ xe không nhập ghi chú lý do chi tiết.",
                ownerName = car.Owner?.FullName ?? car.Owner?.Name ?? "Chủ xe ReCar",
                ownerPhone = car.Owner?.PhoneNumber ?? car.Owner?.Phone ?? "Chưa có",
                carImage = car.Image ?? "/images/cars/car-sedan.jpg",
                color = car.Color ?? "Chưa rõ",
                modelYear = car.Model ?? 2024,
                seat = car.Seat ?? 5,
                transmission = car.Transmission ?? "Tự động",
                fuel = car.Fuel ?? "Xăng",
                price = car.Price ?? 0,
                salePrice = car.SalePrice,
                driverFee = policy.DriverFee,
                location = car.Location ?? "Toàn quốc",
                mileageLimit = policy.MileageLimit,
                extraFeePerKm = policy.ExtraFeePerKm,
                deliverySupported = policy.DeliverySupported,
                deliveryFee = policy.DeliveryFee,
                fuelConsumption = policy.FuelConsumption ?? "Tiêu chuẩn",
                amenities = amenities,
                extraDescription = policy.ExtraDescription ?? "",
                isUpdate = isUpdate,
                oldSnapshotJson = oldSnapshotJson
            });
        }

        // POST: /Admin/Cars/SendMessageToOwner (Admin gửi tin nhắn/lời nhắc riêng tới chuông của chủ xe)
        [HttpPost]
        public async Task<IActionResult> SendMessageToOwner(int ownerId, string title, string message, int? carId)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
            {
                return Json(new { success = false, message = "Tiêu đề và nội dung tin nhắn không được để trống." });
            }

            string link = carId.HasValue ? $"/CarOwner/Cars/Details/{carId.Value}" : "/CarOwner/Dashboard";

            await _notificationService.CreateNotificationAsync(
                ownerId,
                "CarOwner",
                title.Trim(),
                message.Trim(),
                "System",
                link
            );

            return Json(new { success = true, message = "Đã gửi thông báo riêng tới Chủ xe thành công." });
        }
    }
}
