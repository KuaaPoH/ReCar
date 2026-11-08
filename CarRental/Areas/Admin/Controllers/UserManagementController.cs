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

            ViewBag.RoleId = new SelectList(_context.Roles.ToList(), "RoleId", "RoleName", 2);
            return View();
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
                customer.LastLogin = Function.NowVn.ToString("yyyy-MM-dd HH:mm:ss");

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Tạo tài khoản {customer.FullName ?? customer.Username} thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoleId = new SelectList(_context.Roles.ToList(), "RoleId", "RoleName", customer.RoleId);
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

            // Không cho phép xem chi tiết Admin
            if (customer.RoleId == 1)
            {
                TempData["ErrorMessage"] = "Không thể xem chi tiết tài khoản Admin.";
                return RedirectToAction(nameof(Index));
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

            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName", customer.RoleId);
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

            // Không cho phép sửa Admin
            if (customer.RoleId == 1)
            {
                TempData["ErrorMessage"] = "Không thể sửa tài khoản Admin.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(a => a.CustomerId == id);
                if (existingCustomer == null)
                {
                    return NotFound();
                }

                // Nếu password mới được nhập và khác với password cũ thì mã hóa
                if (!string.IsNullOrEmpty(customer.Password) && customer.Password != existingCustomer.Password)
                {
                    existingCustomer.Password = Function.MD5Password(customer.Password);
                }

                existingCustomer.FullName = customer.FullName;
                existingCustomer.Username = customer.Username;
                existingCustomer.PhoneNumber = customer.PhoneNumber ?? customer.Phone;
                existingCustomer.Email = customer.Email;
                existingCustomer.RoleId = customer.RoleId;
                existingCustomer.IsActive = customer.IsActive;

                await _context.SaveChangesAsync();
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

            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName", customer.RoleId);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Không cho phép khóa/mở khóa Admin
            if (customer.RoleId == 1)
            {
                TempData["ErrorMessage"] = "Không thể khóa/mở khóa tài khoản Admin.";
                return RedirectToAction(nameof(Index));
            }

            // Đảo ngược trạng thái IsActive
            customer.IsActive = !customer.IsActive;
            
            _context.Update(customer);
            await _context.SaveChangesAsync();

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
