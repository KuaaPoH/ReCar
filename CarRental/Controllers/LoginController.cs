using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbRenalCarContext _context;
        public LoginController (DbRenalCarContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index(string? ReturnUrl = null)
        {
            // Nếu đã đăng nhập thì không cho vào lại trang Login, chuyển thẳng về Trang chủ
            if (Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Home");
            }

            // Không cho trình duyệt cache trang Login để tránh quay lại bằng nút Back
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(Customer c, string? ReturnUrl = null)
        {
            if (c == null)
            {
                return NotFound();
            }
            
            // Kiểm tra rỗng để tránh lỗi khi chưa nhập gì đã bấm Login
            if (string.IsNullOrWhiteSpace(c.Email) || string.IsNullOrWhiteSpace(c.Password))
            {
                Function.SetMessage(HttpContext.Session, "Vui lòng nhập đầy đủ tài khoản và mật khẩu.");
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                //mã hoá mật khẩu trc khi kiểm tra
                string p = Function.MD5Password(c.Password);
                //kiểm tra tài khoản có tồn tại trong cớ sở dữ liêu
                var check = _context.Customers.Where(m => (m.Email == c.Email) && (m.Password == p)).FirstOrDefault();
                if (check == null)
                {
                    //hiên thị thông báo
                    Function.SetMessage(HttpContext.Session, "Sai thông tin đăng nhập");
                    return RedirectToAction("Index", "Login");
                }

                // Kiểm tra xem Customer có Account tương ứng không và kiểm tra trạng thái khóa
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Email == check.Email);
                
                if (account != null && account.IsActive == false)
                {
                    // Tài khoản bị khóa
                    Function.SetMessage(HttpContext.Session, "LOCKED");
                    return RedirectToAction("Index", "Login");
                }

                //đăng nhập thành công - lưu vào Session
                Function.SetMessage(HttpContext.Session, string.Empty);
                Function.SetAccountId(HttpContext.Session, check.CustomerId);
                Function.SetUserName(HttpContext.Session, string.IsNullOrEmpty(check.Name) ? string.Empty : check.Name);
                Function.SetEmail(HttpContext.Session, string.IsNullOrEmpty(check.Email) ? string.Empty : check.Email);
                Function.SetAddress(HttpContext.Session, string.IsNullOrEmpty(check.Address) ? string.Empty : check.Address);
                Function.SetPhone(HttpContext.Session, string.IsNullOrEmpty(check.PhoneNumber) ? string.Empty : check.PhoneNumber);

                // Kiểm tra xem Customer có Account tương ứng và có phải RoleId=3 (Chủ xe) không
                var carOwnerAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Email == check.Email && a.RoleId == 3);
                
                if (carOwnerAccount != null)
                {
                    Function.SetCarOwnerAccountId(HttpContext.Session, carOwnerAccount.AccountId);
                }
                else
                {       
                    Function.SetCarOwnerAccountId(HttpContext.Session, 0);
                }

                // Luôn lưu AccountId là check.CustomerId của bảng Customers để các tính năng giỏ hàng, đặt xe hoạt động chính xác
                Function.SetAccountId(HttpContext.Session, check.CustomerId);

                // Luôn chuyển về Trang chủ sau khi đăng nhập, bỏ qua ReturnUrl
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}

