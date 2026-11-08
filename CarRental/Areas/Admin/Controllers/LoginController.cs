using CarRental.Models;
using CarRental.Utilities;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly DbRenalCarContext _context;
        public LoginController(DbRenalCarContext context)
        {
            _context = context;
        }

        private void EnsureAdminAccount()
        {
            // Kiểm tra xem hệ thống đã có tài khoản Quản trị viên (RoleId = 1) chưa
            bool hasAdmin = _context.Customers.Any(a => a.RoleId == 1);
            if (!hasAdmin)
            {
                var admin = new Customer
                {
                    Username = "admin",
                    Password = Function.MD5Password("123"),
                    FullName = "Quản Trị Viên",
                    Name = "Quản Trị Viên",
                    Email = "admin@gmail.com",
                    RoleId = 1,
                    IsActive = true,
                    LastLogin = Function.NowVn.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _context.Customers.Add(admin);
                _context.SaveChanges();
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Đảm bảo luôn có tài khoản Admin
            EnsureAdminAccount();

            // Nếu đã đăng nhập (trong phiên này) thì chuyển thẳng về trang chủ Admin
            if (Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            // Không cho cache trang Login để tránh quay lại bằng nút Back
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AdminAccountVM model)
        {
            if (model == null)
            {
                return NotFound();
            }

            // Kiểm tra rỗng để tránh lỗi khi chưa nhập gì đã bấm Login
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                Function.SetMessage(HttpContext.Session, "Vui lòng nhập đầy đủ tài khoản và mật khẩu Admin.");
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            // Mã hoá mật khẩu trước khi kiểm tra
            string pw = Function.MD5Password(model.Password);
            
            // Tìm tài khoản Admin trong bảng Customers
            var check = await _context.Customers
                .FirstOrDefaultAsync(m => (m.Username == model.Email || m.Email == model.Email)
                                     && m.Password == pw
                                     && m.RoleId == 1
                                     && m.IsActive == true);

            if (check == null) {
                Function.SetMessage(HttpContext.Session, "Sai tài khoản hoặc mật khẩu Admin");
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            // Cập nhật thời điểm đăng nhập
            check.LastLogin = Function.NowVn.ToString("yyyy-MM-dd HH:mm:ss");
            await _context.SaveChangesAsync();

            // Lưu vào Session với CustomerId
            Function.SetMessage(HttpContext.Session, string.Empty);
            Function.SetAdminSession(HttpContext.Session, check.CustomerId, string.IsNullOrEmpty(check.Username) ? "Admin" : check.Username, string.IsNullOrEmpty(check.Email) ? string.Empty : check.Email);
            
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}
