using System.Linq;
using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserManagementController : Controller
    {
        private readonly DbRenalCarContext _context;

        public UserManagementController(DbRenalCarContext context)
        {
            _context = context;
        }

        // GET: Admin/UserManagement
        public IActionResult Index(string searchString, string filterType)
        {
            // Kiểm tra trạng thái đăng nhập
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login");
            }

            // Quản lý người dùng thống nhất 100% từ bảng Customers
            var customers = _context.Customers
                .Include(a => a.Role)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(a =>
                    (a.Username != null && a.Username.Contains(searchString)) ||
                    (a.FullName != null && a.FullName.Contains(searchString)) ||
                    (a.Name != null && a.Name.Contains(searchString)) ||
                    (a.Email != null && a.Email.Contains(searchString)) ||
                    (a.PhoneNumber != null && a.PhoneNumber.Contains(searchString))
                );
            }

            // Lọc theo loại
            switch (filterType)
            {
                case "customer":
                    // Khách hàng: RoleId = 2
                    customers = customers.Where(a => a.RoleId == 2);
                    break;
                case "carowner":
                    // Chủ xe: RoleId = 3
                    customers = customers.Where(a => a.RoleId == 3);
                    break;
                case "banned":
                    // Tài khoản bị khóa: IsActive = false
                    customers = customers.Where(a => a.IsActive == false);
                    break;
            }

            var customerList = customers.OrderBy(a => a.CustomerId).ToList();

            ViewBag.SearchString = searchString;
            ViewBag.FilterType = filterType;

            return View(customerList);
        }

        // GET: Admin/UserManagement/Create
        public IActionResult Create()
        {
            if (!Function.IsLogin(HttpContext.Session) || !Function.IsAdmin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.RoleId = BuildRoleSelectList(2);
            return View();
        }

        private SelectList BuildRoleSelectList(int? selectedRoleId)
        {
            var items = _context.Roles
                .OrderBy(r => r.RoleId)
                .ToList()
                .Select(r => new
                {
                    r.RoleId,
                    RoleName = r.RoleId switch
                    {
                        1 => "Quản trị viên (Admin)",
                        2 => "Khách hàng (Người thuê xe)",
                        3 => "Chủ xe đối tác (Host)",
                        _ => r.RoleName
                    }
                });
            return new SelectList(items, "RoleId", "RoleName", selectedRoleId);
        }

        // POST: Admin/UserManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer, string? rawPassword)
        {
            if (!Function.IsLogin(HttpContext.Session) || !Function.IsAdmin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                ModelState.AddModelError("Email", "Vui lòng nhập email.");
            }
            else if (_context.Customers.Any(c => c.Email == customer.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng bởi một tài khoản khác.");
            }

            if (!string.IsNullOrWhiteSpace(customer.Username) && _context.Customers.Any(c => c.Username == customer.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại.");
            }

            if (string.IsNullOrWhiteSpace(rawPassword) || rawPassword.Length < 3)
            {
                ModelState.AddModelError("Password", "Mật khẩu phải có ít nhất 3 ký tự.");
            }

            if (ModelState.IsValid)
            {
                customer.Password = Function.MD5Password(rawPassword!);
                customer.IsActive = true;
                customer.Name = string.IsNullOrWhiteSpace(customer.Name) ? customer.FullName : customer.Name;
                customer.FullName = string.IsNullOrWhiteSpace(customer.FullName) ? customer.Name : customer.FullName;
                customer.Username = string.IsNullOrWhiteSpace(customer.Username) ? customer.Email : customer.Username;
                customer.LastLogin = Function.NowVn.ToString("yyyy-MM-dd HH:mm:ss");

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Tạo tài khoản {customer.FullName ?? customer.Username} thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoleId = BuildRoleSelectList(customer.RoleId);
            return View(customer);
        }

        // GET: Admin/UserManagement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Nạp thông tin đăng ký đối tác (nếu có)
            var reg = await _context.CarOwnerRegistrations
                .FirstOrDefaultAsync(r => r.CustomerId == customer.CustomerId || (customer.Email != null && r.Email == customer.Email));
            ViewBag.Registration = reg;

            // Nạp dữ liệu liên quan tùy theo vai trò
            if (customer.RoleId == 3) // Chủ xe đối tác
            {
                var ownedCars = await _context.Cars
                    .Include(c => c.Type)
                    .Where(c => c.OwnerId == customer.CustomerId)
                    .OrderByDescending(c => c.CarId)
                    .ToListAsync();
                ViewBag.OwnedCars = ownedCars;

                var carIds = ownedCars.Select(c => c.CarId).ToList();
                var hostOrders = await _context.OrderDetails
                    .Include(od => od.Order)
                        .ThenInclude(o => o.Status)
                    .Include(od => od.Order)
                        .ThenInclude(o => o.Customer)
                    .Include(od => od.Car)
                    .Where(od => carIds.Contains(od.CarId))
                    .OrderByDescending(od => od.OrderDetailId)
                    .Take(10)
                    .ToListAsync();
                ViewBag.HostOrders = hostOrders;
            }
            else // Khách hàng (Role 2) hoặc Admin (Role 1)
            {
                var customerOrders = await _context.CarRentalOrders
                    .Include(o => o.Status)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Car)
                    .Where(o => o.CustomerId == customer.CustomerId)
                    .OrderByDescending(o => o.OrderId)
                    .Take(10)
                    .ToListAsync();
                ViewBag.CustomerOrders = customerOrders;
            }

            return View(customer);
        }

        // GET: Admin/UserManagement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Không cho phép sửa Admin
            if (customer.RoleId == 1)
            {
                TempData["ErrorMessage"] = "Không thể sửa tài khoản Admin.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = BuildRoleSelectList(customer.RoleId);
            ViewBag.RoleId = ViewBag.Roles;
            return View(customer);
        }

        // POST: Admin/UserManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            try
            {
                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(a => a.CustomerId == id);
                if (existingCustomer == null)
                {
                    return NotFound();
                }

                // Không cho phép sửa tài khoản Admin
                if (existingCustomer.RoleId == 1)
                {
                    TempData["ErrorMessage"] = "Không thể sửa tài khoản Admin.";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra trùng Email / Username với tài khoản khác
                if (!string.IsNullOrWhiteSpace(customer.Email) &&
                    await _context.Customers.AnyAsync(c => c.CustomerId != id && c.Email == customer.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng bởi một tài khoản khác.");
                    ViewBag.Roles = BuildRoleSelectList(customer.RoleId);
                    ViewBag.RoleId = ViewBag.Roles;
                    return View(customer);
                }

                if (!string.IsNullOrWhiteSpace(customer.Username) &&
                    await _context.Customers.AnyAsync(c => c.CustomerId != id && c.Username == customer.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại.");
                    ViewBag.Roles = BuildRoleSelectList(customer.RoleId);
                    ViewBag.RoleId = ViewBag.Roles;
                    return View(customer);
                }

                // Nếu password mới được nhập và khác với password cũ thì mã hóa
                if (!string.IsNullOrEmpty(customer.Password) && customer.Password != existingCustomer.Password)
                {
                    existingCustomer.Password = Function.MD5Password(customer.Password);
                }

                existingCustomer.FullName = customer.FullName;
                existingCustomer.Name = string.IsNullOrWhiteSpace(customer.FullName) ? existingCustomer.Name : customer.FullName;
                if (!string.IsNullOrWhiteSpace(customer.Username))
                {
                    existingCustomer.Username = customer.Username;
                }
                existingCustomer.PhoneNumber = customer.PhoneNumber ?? customer.Phone;
                existingCustomer.Email = customer.Email;
                existingCustomer.Idcard = customer.Idcard;
                existingCustomer.Address = customer.Address;
                int oldRoleId = existingCustomer.RoleId;
                existingCustomer.RoleId = customer.RoleId;
                existingCustomer.IsActive = customer.IsActive;

                // Nếu Admin hạ quyền từ Chủ xe (Role 3) xuống Khách hàng (Role != 3),
                // cập nhật hồ sơ CarOwnerRegistration cũ để tránh xung đột/vòng lặp redirect khi đăng ký lại.
                if (oldRoleId == 3 && customer.RoleId != 3)
                {
                    var approvedRegs = await _context.CarOwnerRegistrations
                        .Where(r => (r.CustomerId == existingCustomer.CustomerId || (!string.IsNullOrEmpty(existingCustomer.Email) && r.Email == existingCustomer.Email))
                                    && r.Status == "Approved")
                        .ToListAsync();
                    foreach (var reg in approvedRegs)
                    {
                        reg.Status = "Revoked";
                        reg.RejectReason = "Quản trị viên đã điều chỉnh vai trò tài khoản về Khách hàng.";
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã cập nhật tài khoản {existingCustomer.FullName ?? existingCustomer.Username} thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.CustomerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            ViewBag.Roles = BuildRoleSelectList(customer.RoleId);
            ViewBag.RoleId = ViewBag.Roles;
            return View(customer);
        }

        // GET: Admin/UserManagement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Không cho phép xóa Admin
            if (customer.RoleId == 1)
            {
                TempData["ErrorMessage"] = "Không thể xóa tài khoản Admin.";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // POST: Admin/UserManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                // Không cho phép xóa Admin
                if (customer.RoleId == 1)
                {
                    TempData["ErrorMessage"] = "Không thể xóa tài khoản Admin.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/UserManagement/ToggleLock/5
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Không tìm thấy người dùng." });
                return NotFound();
            }

            // Không cho phép khóa/mở khóa Admin
            if (customer.RoleId == 1)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Không thể thao tác trên tài khoản Quản trị viên tối cao." });
                TempData["ErrorMessage"] = "Không thể khóa/mở khóa tài khoản Admin.";
                return RedirectToAction(nameof(Index));
            }

            // Đảo ngược trạng thái IsActive
            customer.IsActive = !customer.IsActive;
            
            _context.Update(customer);
            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    isActive = customer.IsActive,
                    message = customer.IsActive ? "Đã mở khóa tài khoản thành công." : "Đã khóa tài khoản thành công."
                });
            }

            TempData["SuccessMessage"] = customer.IsActive 
                ? "Đã mở khóa tài khoản thành công." 
                : "Đã khóa tài khoản thành công.";

            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
