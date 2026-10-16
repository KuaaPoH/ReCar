using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.CarOwner.Controllers
{
    [Area("CarOwner")]
    public class OrdersController : Controller
    {
        private readonly DbRenalCarContext _context;

        public OrdersController(DbRenalCarContext context)
        {
            _context = context;
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

        public IActionResult Index(int? filterStatusId)
        {
            var owner = GetCurrentCarOwner();
            if (owner == null || owner.RoleId != 3)
            {
                Function.SetMessage(HttpContext.Session, "Bạn không có quyền truy cập");
                return RedirectToAction("Index", "Login", new { area = "" });
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

            // Lấy các đơn hàng từ danh sách OrderId
            var orders = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Where(o => orderIds.Contains(o.OrderId))
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();

            // Áp dụng bộ lọc nếu có
            if (filterStatusId.HasValue)
            {
                orders = orders.Where(o => o.StatusId == filterStatusId.Value);
            }

            var status = (from c in _context.OrderStatuses
                           select new SelectListItem()
                           {
                               Text = c.StatusDescription,
                               Value = c.StatusId.ToString(),
                           }).ToList();
            status.Insert(0, new SelectListItem()
            {
                Text = "",
                Value = string.Empty,
            });
            ViewBag.orderstatus = status;

            return View(orders.ToList());
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

            var order = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .FirstOrDefault(o => o.OrderId == id && 
                    o.OrderDetails.Any(od => od.Car != null && od.Car.OwnerId == ownerId));

            if (order == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy đơn hàng");
                return RedirectToAction("Index");
            }

            // Chỉ lấy các chi tiết đơn hàng của xe thuộc chủ xe này
            order.OrderDetails = order.OrderDetails
                .Where(od => od.Car != null && od.Car.OwnerId == ownerId)
                .ToList();

            return View(order);
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

                // Nếu trạng thái là "đã hủy" hoặc "đã thanh lý", cập nhật IsActive của xe về true
                if (o.StatusId == 2 || o.StatusId == 5)
                {
                    foreach (var detail in existingOrder.OrderDetails.Where(od => od.Car != null && od.Car.OwnerId == ownerId))
                    {
                        var car = _context.Cars.FirstOrDefault(c => c.CarId == detail.CarId);
                        if (car != null)
                        {
                            car.IsActive = true;
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

