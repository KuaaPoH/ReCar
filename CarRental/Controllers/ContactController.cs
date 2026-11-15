using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    public class ContactController : Controller
    {
        private readonly DbRenalCarContext _context;
        public ContactController(DbRenalCarContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Settings = _context.SystemSettings.FirstOrDefault();

            int accountId = Function.GetAccountId(HttpContext.Session);
            bool isLoggedIn = Function.IsLogin(HttpContext.Session) && accountId > 0;
            ViewBag.IsLoggedIn = isLoggedIn;

            if (isLoggedIn)
            {
                ViewBag.Customer = _context.Customers.FirstOrDefault(c => c.CustomerId == accountId);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string? phone, string message)
        {
            int accountId = Function.GetAccountId(HttpContext.Session);
            if (!Function.IsLogin(HttpContext.Session) || accountId <= 0)
            {
                return Json(new
                {
                    status = false,
                    requireLogin = true,
                    message = "Vui lòng đăng nhập tài khoản để gửi liên hệ."
                });
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return Json(new
                {
                    status = false,
                    message = "Vui lòng nhập nội dung cần hỗ trợ."
                });
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == accountId);
            if (customer == null)
            {
                return Json(new
                {
                    status = false,
                    requireLogin = true,
                    message = "Không tìm thấy thông tin tài khoản. Vui lòng đăng nhập lại."
                });
            }

            try
            {
                var con = new Contact
                {
                    Name = !string.IsNullOrWhiteSpace(customer.FullName) ? customer.FullName : (customer.Username ?? "Khách hàng"),
                    Phone = !string.IsNullOrWhiteSpace(phone) ? phone.Trim() : (customer.PhoneNumber ?? ""),
                    Email = customer.Email ?? "",
                    CreatedDate = Function.NowVn,
                    CreatedBy = customer.CustomerId.ToString(),
                    Message = message.Trim(),
                    IsRead = 0
                };

                _context.Contacts.Add(con);
                await _context.SaveChangesAsync();

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
    }
}
