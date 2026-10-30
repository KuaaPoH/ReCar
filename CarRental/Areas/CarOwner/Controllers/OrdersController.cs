using CarRental.Hubs;
using CarRental.Models;
using CarRental.Utilities;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Areas.CarOwner.Controllers
{
    [Area("CarOwner")]
    public class OrdersController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly IHubContext<ReCarHub> _hubContext;

        public OrdersController(DbRenalCarContext context, IHubContext<ReCarHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

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

        public IActionResult Index(int? filterStatusId, string? q)
        {
            var owner = GetCurrentCarOwner();
            if (owner == null || owner.RoleId != 3)
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Home", new { area = "CarOwner" });
            }

            var ownerId = owner.AccountId;

            // Lấy danh sách CarId của chủ xe này
            var carIds = _context.Cars
                .Where(c => c.OwnerId == ownerId)
                .Select(c => c.CarId)
                .ToList();

            // Lấy danh sách OrderId từ các OrderDetails có CarId trong danh sách trên
            var orderIds = _context.OrderDetails
                .Where(od => carIds.Contains(od.CarId))
                .Select(od => od.OrderId)
                .Distinct()
                .ToList();

            // Truy vấn cơ sở toàn bộ đơn hàng của chủ xe
            var baseQuery = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .Where(o => orderIds.Contains(o.OrderId));

            // Thống kê số lượng theo từng Tab nghiệp vụ
            int countAll = baseQuery.Count();
            int countPendingHandover = baseQuery.Count(o => o.StatusId == 3 || o.StatusId == 1);
            int countInRental = baseQuery.Count(o => o.StatusId == 4);
            int countCompleted = baseQuery.Count(o => o.StatusId == 5);
            int countCancelled = baseQuery.Count(o => o.StatusId == 2);

            var query = baseQuery.AsQueryable();

            // Áp dụng bộ lọc Tab trạng thái
            if (filterStatusId.HasValue && filterStatusId.Value > 0)
            {
                if (filterStatusId.Value == 3)
                {
                    // Chờ bàn giao: Đã cọc 30% (Status 3) hoặc Đã đặt (Status 1)
                    query = query.Where(o => o.StatusId == 3 || o.StatusId == 1);
                }
                else
                {
                    query = query.Where(o => o.StatusId == filterStatusId.Value);
                }
            }

            // Tìm kiếm nhanh đa năng: Mã đơn, Khách hàng, SĐT, Tên xe, Biển số
            if (!string.IsNullOrWhiteSpace(q))
            {
                string searchKey = q.Trim().ToLower();
                query = query.Where(o => o.OrderId.ToString().Contains(searchKey)
                    || (o.Customer != null && o.Customer.Name != null && o.Customer.Name.ToLower().Contains(searchKey))
                    || (o.Customer != null && o.Customer.PhoneNumber != null && o.Customer.PhoneNumber.Contains(searchKey))
                    || o.OrderDetails.Any(od => od.Car != null && (
                        (od.Car.CarName != null && od.Car.CarName.ToLower().Contains(searchKey)) ||
                        (od.Car.LicensePlate != null && od.Car.LicensePlate.ToLower().Contains(searchKey))
                    ))
                );
            }

            var orders = query.OrderByDescending(o => o.OrderId).ToList();

            // Lọc OrderDetails chỉ hiển thị xe thuộc chủ xe này
            foreach (var o in orders)
            {
                o.OrderDetails = o.OrderDetails.Where(od => od.Car != null && od.Car.OwnerId == ownerId).ToList();
            }

            ViewBag.FilterStatusId = filterStatusId;
            ViewBag.FilterQ = q;
            ViewBag.CountAll = countAll;
            ViewBag.CountPendingHandover = countPendingHandover;
            ViewBag.CountInRental = countInRental;
            ViewBag.CountCompleted = countCompleted;
            ViewBag.CountCancelled = countCancelled;

            return View(orders);
        }

        public IActionResult Details(int? id)
        {
            var owner = GetCurrentCarOwner();
            if (owner == null || owner.RoleId != 3)
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            if (id == null)
            {
                return NotFound();
            }

            var ownerId = owner.AccountId;

            var order = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .FirstOrDefault(o => o.OrderId == id && 
                    o.OrderDetails.Any(od => od.Car != null && od.Car.OwnerId == ownerId));

            if (order == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy đơn hàng hoặc đơn không thuộc quyền quản lý của bạn");
                return RedirectToAction("Index");
            }

            // Chỉ lấy các chi tiết đơn hàng của xe thuộc chủ xe này
            order.OrderDetails = order.OrderDetails
                .Where(od => od.Car != null && od.Car.OwnerId == ownerId)
                .ToList();

            var firstCar = order.OrderDetails.FirstOrDefault()?.Car;
            ViewBag.CarPolicy = CarPolicy.Parse(firstCar);

            try
            {
                var inspections = _context.VehicleInspections
                    .Where(vi => vi.OrderId == order.OrderId)
                    .ToList();
                ViewBag.HandoverInspection = inspections.FirstOrDefault(vi => vi.InspectionType == VehicleInspection.TypeHandover);
                ViewBag.ReturnInspection = inspections.FirstOrDefault(vi => vi.InspectionType == VehicleInspection.TypeReturn);
            }
            catch
            {
                EnsureInspectionTableSafe();
            }

            return View(order);
        }

        private void EnsureInspectionTableSafe()
        {
            try
            {
                _context.Database.ExecuteSqlRaw(@"CREATE TABLE IF NOT EXISTS `vehicle_inspections` (
                    `InspectionId` INT NOT NULL AUTO_INCREMENT,
                    `OrderId` INT NOT NULL,
                    `CarId` INT NOT NULL,
                    `InspectionType` VARCHAR(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                    `Odometer` INT NOT NULL DEFAULT 0,
                    `FuelLevel` INT NOT NULL DEFAULT 0,
                    `FrontImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `RearImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `LeftImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `RightImage` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `Notes` VARCHAR(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    `CreatedBy` INT NULL,
                    PRIMARY KEY (`InspectionId`),
                    UNIQUE INDEX `UX_Inspection_Order_Type` (`OrderId`, `InspectionType`),
                    INDEX `IX_Inspection_Car` (`CarId`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;");
            }
            catch { }
        }

        private async Task<VehicleInspection?> UpsertInspectionAsync(int orderId, int carId, int ownerId, string inspectionType, InspectionSubmitVM vm)
        {
            EnsureInspectionTableSafe();
            bool hasAnyInspectionInput = vm.Odometer.HasValue || vm.FuelLevel.HasValue ||
                                         !string.IsNullOrWhiteSpace(vm.Notes) ||
                                         vm.FrontImage != null || vm.RearImage != null ||
                                         vm.LeftImage != null || vm.RightImage != null;
            if (!hasAnyInspectionInput) return null;

            var record = await _context.VehicleInspections
                .FirstOrDefaultAsync(vi => vi.OrderId == orderId && vi.InspectionType == inspectionType);

            if (record == null)
            {
                record = new VehicleInspection
                {
                    OrderId = orderId,
                    CarId = carId,
                    InspectionType = inspectionType,
                    CreatedAt = DateTime.Now,
                    CreatedBy = ownerId
                };
                _context.VehicleInspections.Add(record);
            }

            if (vm.Odometer.HasValue && vm.Odometer.Value >= 0) record.Odometer = vm.Odometer.Value;
            if (vm.FuelLevel.HasValue) record.FuelLevel = Math.Clamp(vm.FuelLevel.Value, 0, 100);
            if (!string.IsNullOrWhiteSpace(vm.Notes)) record.Notes = vm.Notes.Trim();

            if (vm.FrontImage != null && vm.FrontImage.Length > 0)
                record.FrontImage = "/files/inspections/" + Function.UploadImage(vm.FrontImage, "inspections");
            if (vm.RearImage != null && vm.RearImage.Length > 0)
                record.RearImage = "/files/inspections/" + Function.UploadImage(vm.RearImage, "inspections");
            if (vm.LeftImage != null && vm.LeftImage.Length > 0)
                record.LeftImage = "/files/inspections/" + Function.UploadImage(vm.LeftImage, "inspections");
            if (vm.RightImage != null && vm.RightImage.Length > 0)
                record.RightImage = "/files/inspections/" + Function.UploadImage(vm.RightImage, "inspections");

            record.CreatedAt = DateTime.Now;
            record.CreatedBy = ownerId;
            return record;
        }

        // POST: /CarOwner/Orders/SaveInspection (Lưu/cập nhật biên bản bàn giao hoặc nhận xe độc lập)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveInspection([FromForm] InspectionSubmitVM vm)
        {
            var owner = GetCurrentCarOwner();
            if (owner == null || owner.RoleId != 3)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var ownerId = owner.AccountId;
            var order = _context.CarRentalOrders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .FirstOrDefault(o => o.OrderId == vm.id &&
                    o.OrderDetails.Any(od => od.Car != null && od.Car.OwnerId == ownerId));

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            var firstCar = order.OrderDetails.FirstOrDefault(od => od.Car != null && od.Car.OwnerId == ownerId)?.Car;
            if (firstCar == null)
            {
                return Json(new { success = false, message = "Không tìm thấy phương tiện trong đơn." });
            }

            string inspType = string.Equals(vm.InspectionType, VehicleInspection.TypeReturn, StringComparison.OrdinalIgnoreCase)
                ? VehicleInspection.TypeReturn
                : VehicleInspection.TypeHandover;

            await UpsertInspectionAsync(order.OrderId, firstCar.CarId, ownerId, inspType, vm);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = inspType == VehicleInspection.TypeReturn
                    ? "Đã lưu biên bản nhận lại xe (Check-out) thành công!"
                    : "Đã lưu biên bản bàn giao xe (Check-in) thành công!",
                redirectUrl = Url.Action("Details", new { id = order.OrderId })
            });
        }

        // POST: /CarOwner/Orders/ConfirmHandover (Bàn giao xe cho khách -> Đơn sang Đang thuê, Xe sang Đang bận + Lưu biên bản Check-in)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmHandover([FromForm] InspectionSubmitVM vm)
        {
            int id = vm.id;
            var owner = GetCurrentCarOwner();
            if (owner == null || owner.RoleId != 3)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var ownerId = owner.AccountId;
            var order = _context.CarRentalOrders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .FirstOrDefault(o => o.OrderId == id &&
                    o.OrderDetails.Any(od => od.Car != null && od.Car.OwnerId == ownerId));

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng hoặc đơn không thuộc quyền quản lý của bạn." });
            }

            if (order.StatusId != 1 && order.StatusId != 3)
            {
                return Json(new { success = false, message = "Đơn hàng không ở trạng thái chờ bàn giao." });
            }

            // Cập nhật trạng thái đơn hàng sang 4: Đang thuê / Đang giao
            order.StatusId = 4;

            Car? firstCar = null;
            // Cập nhật trạng thái xe sang "Đang bận" và tạm ẩn khỏi sàn
            foreach (var detail in order.OrderDetails.Where(od => od.Car != null && od.Car.OwnerId == ownerId))
            {
                if (detail.Car != null)
                {
                    firstCar ??= detail.Car;
                    detail.Car.CarStatus = "Đang bận";
                    detail.Car.IsActive = false;
                }
            }

            if (firstCar != null)
            {
                var record = await UpsertInspectionAsync(order.OrderId, firstCar.CarId, ownerId, VehicleInspection.TypeHandover, vm);
                if (record != null)
                {
                    string checkInLog = $"[Check-in {DateTime.Now:dd/MM/yyyy HH:mm}]: ODO {record.Odometer:N0} km, Nhiên liệu {record.FuelLevel}%";
                    order.Notes = string.IsNullOrEmpty(order.Notes) ? checkInLog : $"{order.Notes} | {checkInLog}";
                }
            }

            await _context.SaveChangesAsync();

            // Phát thông báo Real-time cho Admin
            try
            {
                _ = _hubContext.Clients.Group("Role_Admin").SendAsync("ReceiveOrderStatusUpdated", new
                {
                    orderId = order.OrderId,
                    statusId = 4,
                    statusText = "Đang thuê"
                });
            }
            catch { }

            Function.SetMessage(HttpContext.Session, $"Đã xác nhận bàn giao xe & lưu biên bản Check-in cho đơn #{order.OrderId}! Xe đã chuyển sang trạng thái 'Đang có khách'.");
            return Json(new { success = true, message = "Xác nhận bàn giao xe & lưu biên bản Check-in thành công!", redirectUrl = Url.Action("Details", new { id }) });
        }

        // POST: /CarOwner/Orders/ConfirmReturn (Nhận lại xe -> Đơn sang Hoàn tất, Xe sang Sẵn sàng, Trips + 1 + Lưu biên bản Check-out)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReturn([FromForm] InspectionSubmitVM vm)
        {
            int id = vm.id;
            try
            {
                var owner = GetCurrentCarOwner();
                if (owner == null || owner.RoleId != 3)
                {
                    return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
                }

                var ownerId = owner.AccountId;
                var order = _context.CarRentalOrders
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Car)
                    .FirstOrDefault(o => o.OrderId == id &&
                        o.OrderDetails.Any(od => od.Car != null && od.Car.OwnerId == ownerId));

                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng hoặc đơn không thuộc quyền quản lý của bạn." });
                }

                if (order.StatusId != 4)
                {
                    return Json(new { success = false, message = "Chỉ có thể hoàn tất khi xe đang trong trạng thái cho thuê." });
                }

                // Cập nhật trạng thái đơn hàng sang 5: Hoàn tất
                order.StatusId = 5;
                order.ReturnDate = DateOnly.FromDateTime(DateTime.Now);

                Car? firstCar = null;
                // Cập nhật trạng thái xe trở về "Sẵn sàng", mở lại trên sàn và tăng Trips + 1
                foreach (var detail in order.OrderDetails.Where(od => od.Car != null && od.Car.OwnerId == ownerId))
                {
                    if (detail.Car != null)
                    {
                        firstCar ??= detail.Car;
                        detail.Car.CarStatus = "Sẵn sàng";
                        detail.Car.IsActive = true;
                        detail.Car.Trips = (detail.Car.Trips ?? 0) + 1;
                    }
                }

                string summaryExtra = "";
                if (firstCar != null)
                {
                    var record = await UpsertInspectionAsync(order.OrderId, firstCar.CarId, ownerId, VehicleInspection.TypeReturn, vm);
                    if (record != null)
                    {
                        var handoverRecord = await _context.VehicleInspections
                            .FirstOrDefaultAsync(vi => vi.OrderId == order.OrderId && vi.InspectionType == VehicleInspection.TypeHandover);

                        var firstDetail = order.OrderDetails.FirstOrDefault();
                        int rentalDays = 1;
                        if (firstDetail?.PickupDate.HasValue == true && firstDetail?.ReturnDate.HasValue == true)
                        {
                            rentalDays = Math.Max(1, (int)(firstDetail.ReturnDate.Value.Date - firstDetail.PickupDate.Value.Date).TotalDays);
                        }
                        var policy = CarPolicy.Parse(firstCar);
                        int allowedKm = rentalDays * policy.MileageLimit;

                        if (handoverRecord != null && record.Odometer >= handoverRecord.Odometer && handoverRecord.Odometer > 0)
                        {
                            int drivenKm = record.Odometer - handoverRecord.Odometer;
                            int overKm = Math.Max(0, drivenKm - allowedKm);
                            decimal extraFee = overKm * policy.ExtraFeePerKm;
                            summaryExtra = overKm > 0
                                ? $" (Đã đi {drivenKm:N0} km, vượt {overKm:N0} km — Phụ phí: {extraFee:N0}đ)"
                                : $" (Đã đi {drivenKm:N0} km, trong hạn mức {allowedKm:N0} km)";
                        }

                        string checkOutLog = $"[Check-out {DateTime.Now:dd/MM/yyyy HH:mm}]: ODO {record.Odometer:N0} km, Nhiên liệu {record.FuelLevel}%{summaryExtra}";
                        order.Notes = string.IsNullOrEmpty(order.Notes) ? checkOutLog : $"{order.Notes} | {checkOutLog}";
                    }
                }

                await _context.SaveChangesAsync();

                // Phát thông báo Real-time cho Admin
                try
                {
                    _ = _hubContext.Clients.Group("Role_Admin").SendAsync("ReceiveOrderStatusUpdated", new
                    {
                        orderId = order.OrderId,
                        statusId = 5,
                        statusText = "Hoàn tất"
                    });
                }
                catch { }

                Function.SetMessage(HttpContext.Session, $"Đã nhận lại xe & hoàn tất đơn #{order.OrderId}{summaryExtra}! Xe đã sẵn sàng đón khách mới (+1 chuyến đi thành công).");
                return Json(new { success = true, message = $"Đã nhận lại xe & hoàn tất chuyến đi thành công{summaryExtra}!", redirectUrl = Url.Action("Details", new { id }) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi xử lý hoàn tất đơn: {ex.Message}" });
            }
        }

        // POST: /CarOwner/Orders/CancelOrder (Hủy đơn thuê)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id, string? reason)
        {
            var owner = GetCurrentCarOwner();
            if (owner == null || owner.RoleId != 3)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var ownerId = owner.AccountId;
            var order = _context.CarRentalOrders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .FirstOrDefault(o => o.OrderId == id &&
                    o.OrderDetails.Any(od => od.Car != null && od.Car.OwnerId == ownerId));

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            if (order.StatusId == 4 || order.StatusId == 5)
            {
                return Json(new { success = false, message = "Không thể hủy đơn khi xe đang chạy hoặc chuyến đi đã hoàn tất." });
            }

            order.StatusId = 2; // Đã Huỷ
            string cancelNote = string.IsNullOrWhiteSpace(reason) ? "Theo thỏa thuận giữa hai bên" : reason.Trim();
            order.Notes = string.IsNullOrEmpty(order.Notes) 
                ? $"[Chủ xe hủy đơn lúc {DateTime.Now:dd/MM/yyyy HH:mm}: {cancelNote}]"
                : $"{order.Notes} | [Chủ xe hủy đơn lúc {DateTime.Now:dd/MM/yyyy HH:mm}: {cancelNote}]";

            // Giải phóng xe về "Sẵn sàng"
            foreach (var detail in order.OrderDetails.Where(od => od.Car != null && od.Car.OwnerId == ownerId))
            {
                if (detail.Car != null)
                {
                    detail.Car.CarStatus = "Sẵn sàng";
                    detail.Car.IsActive = true;
                }
            }

            await _context.SaveChangesAsync();

            Function.SetMessage(HttpContext.Session, $"Đã hủy đơn thuê xe #{order.OrderId}.");
            return Json(new { success = true, message = "Đã hủy đơn thuê xe thành công.", redirectUrl = Url.Action("Details", new { id }) });
        }

        public IActionResult Edit(int? id)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var ownerId = Function.GetAccountId(HttpContext.Session);

            var order = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .FirstOrDefault(o => o.OrderId == id && 
                    o.OrderDetails.Any(od => od.Car != null && od.Car.OwnerId == ownerId));

            if (order == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy đơn hàng hoặc bạn không có quyền chỉnh sửa");
                return RedirectToAction("Index");
            }

            var Odstatus = (from o in _context.OrderStatuses
                            select new SelectListItem()
                            {
                                Text = o.StatusDescription,
                                Value = o.StatusId.ToString(),
                            }).ToList();
            Odstatus.Insert(0, new SelectListItem()
            {
                Text = "--Chọn trạng thái--",
                Value = "0",
            });
            ViewBag.OrderStatus = Odstatus;

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CarRentalOrder o)
        {
            if (!CheckCarOwnerPermission())
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            if (ModelState.IsValid)
            {
                var ownerId = Function.GetAccountId(HttpContext.Session);
                var existingOrder = _context.CarRentalOrders
                    .Include(order => order.Customer)
                    .Include(c => c.OrderDetails)
                        .ThenInclude(od => od.Car)
                    .FirstOrDefault(order => order.OrderId == o.OrderId &&
                        order.OrderDetails.Any(od => od.Car != null && od.Car.OwnerId == ownerId));

                if (existingOrder == null)
                {
                    Function.SetMessage(HttpContext.Session, "Không tìm thấy đơn hàng hoặc bạn không có quyền chỉnh sửa");
                    return RedirectToAction("Index");
                }

                existingOrder.StatusId = o.StatusId;

                // Nếu trạng thái là "đã hủy" (2) hoặc "hoàn tất" (5), giải phóng xe về Sẵn sàng
                if (o.StatusId == 2 || o.StatusId == 5)
                {
                    foreach (var detail in existingOrder.OrderDetails.Where(od => od.Car != null && od.Car.OwnerId == ownerId))
                    {
                        var car = _context.Cars.FirstOrDefault(c => c.CarId == detail.CarId);
                        if (car != null)
                        {
                            car.CarStatus = "Sẵn sàng";
                            car.IsActive = true;
                            if (o.StatusId == 5) car.Trips = (car.Trips ?? 0) + 1;
                            _context.Cars.Update(car);
                        }
                    }
                }
                else if (o.StatusId == 4) // Đang thuê
                {
                    foreach (var detail in existingOrder.OrderDetails.Where(od => od.Car != null && od.Car.OwnerId == ownerId))
                    {
                        var car = _context.Cars.FirstOrDefault(c => c.CarId == detail.CarId);
                        if (car != null)
                        {
                            car.CarStatus = "Đang bận";
                            car.IsActive = false;
                            _context.Cars.Update(car);
                        }
                    }
                }

                _context.CarRentalOrders.Update(existingOrder);
                _context.SaveChanges();

                Function.SetMessage(HttpContext.Session, "Cập nhật trạng thái đơn hàng thành công");
                return RedirectToAction("Index");
            }

            var Odstatus = (from status in _context.OrderStatuses
                            select new SelectListItem()
                            {
                                Text = status.StatusDescription,
                                Value = status.StatusId.ToString(),
                            }).ToList();
            Odstatus.Insert(0, new SelectListItem()
            {
                Text = "--Chọn trạng thái--",
                Value = "0",
            });
            ViewBag.OrderStatus = Odstatus;

            return View(o);
        }
    }
}
