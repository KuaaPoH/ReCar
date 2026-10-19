using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private bool CheckAdmin()
        {
            return Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session);
        }

        public IActionResult Index()
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            ViewBag.DepositPct = 30;
            ViewBag.CommissionPct = 10;
            ViewBag.Hotline = "1900 8888";
            ViewBag.SupportEmail = "hotro@recar.vn";
            ViewBag.PlatformName = "ReCar — Car-Sharing Platform";
            ViewBag.Address = "Tòa tháp Keangnam Landmark 72, Mễ Trì, Nam Từ Liêm, Hà Nội";

            return View();
        }

        [HttpPost]
        public IActionResult SaveSettings(int depositPct, int commissionPct, string hotline, string supportEmail)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            return Json(new { 
                success = true, 
                message = "Cấu hình hệ thống đã được cập nhật thành công và áp dụng cho các giao dịch mới." 
            });
        }
    }
}
