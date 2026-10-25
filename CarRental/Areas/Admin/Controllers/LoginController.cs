using CarRental.Models;
using CarRental.Utilities;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            const string adminUsername = "Admin";
            const string adminPasswordPlain = "admin123456a@";

            // Nếu chưa có tài khoản Admin thì tạo mới
            var admin = _context.Accounts.FirstOrDefault(a => a.Username == adminUsername);
            if (admin == null)
            {
                admin = new Account
                {
                    Username = adminUsername,
                    Password = Function.MD5Password(adminPasswordPlain),
                    FullName = "Administrator",
                    Email = "admin@system.local",
                    RoleId = 1, // giả định RoleId = 1 là Admin
                    IsActive = true,
                    LastLogin = DateTime.Now.ToString("yyyy-MM-dd")
                };

                _context.Accounts.Add(admin);
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
        [Area("Admin")]

        [HttpPost]
        public IActionResult Index(AdminAccountVM model)
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
            // Chỉ cho phép tài khoản Admin đăng nhập vào khu vực Admin
            // Ở đây dùng trường Email trong form như là "Tài khoản" (Username)
            var check = _context.Accounts
                .FirstOrDefault(m => m.Username == "Admin"
                                     && m.Username == model.Email
                                     && m.Password == pw
                                     && m.IsActive == true);

            if (check == null) {
                //hin thi thong bao neu sau mk
                Function.SetMessage(HttpContext.Session, "Sai tài khoản hoặc mật khẩu Admin");
                return RedirectToAction("Index", "Login");
            }
            // vao trang admin neu dung usernam va pasw - lưu vào Session
            Function.SetMessage(HttpContext.Session, string.Empty);
            Function.SetAdminSession(HttpContext.Session, check.AccountId, string.IsNullOrEmpty(check.Username) ? "Admin" : check.Username, string.IsNullOrEmpty(check.Email) ? string.Empty : check.Email);
            // Luôn đưa về Trang chủ Admin sau khi đăng nhập
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}

