using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly DbRenalCarContext _context;

        public SettingsController(DbRenalCarContext context)
        {
            _context = context;
        }

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

            var setting = _context.SystemSettings.FirstOrDefault();
            if (setting == null)
            {
                setting = new SystemSetting
                {
                    DepositPct = 30,
                    CommissionPct = 10,
                    DefaultKmPerDay = 350,
                    ExtraKmPrice = 3000m,
                    Hotline = "1900 8888",
                    SupportEmail = "hotro@recar.vn",
                    PlatformName = "ReCar — Car-Sharing Platform",
                    Address = "Tòa tháp Keangnam Landmark 72, Mễ Trì, Nam Từ Liêm, Hà Nội",
                    UpdatedAt = null
                };
                _context.SystemSettings.Add(setting);
                _context.SaveChanges();
            }

            return View(setting);
        }

        [HttpPost]
        public IActionResult SaveSettings(int depositPct, int commissionPct, int defaultKmPerDay, decimal extraKmPrice, string hotline, string supportEmail, string platformName, string address)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            if (depositPct < 10 || depositPct > 50)
            {
                return Json(new { success = false, message = "Tỷ lệ đặt cọc phải nằm trong khoảng từ 10% đến 50%." });
            }

            if (commissionPct < 5 || commissionPct > 30)
            {
                return Json(new { success = false, message = "Tỷ lệ hoa hồng nền tảng phải nằm trong khoảng từ 5% đến 30%." });
            }

            var setting = _context.SystemSettings.FirstOrDefault();
            if (setting == null)
            {
                setting = new SystemSetting();
                _context.SystemSettings.Add(setting);
            }

            setting.DepositPct = depositPct;
            setting.CommissionPct = commissionPct;
            setting.DefaultKmPerDay = defaultKmPerDay > 0 ? defaultKmPerDay : 350;
            setting.ExtraKmPrice = extraKmPrice > 0 ? extraKmPrice : 3000m;
            setting.Hotline = string.IsNullOrWhiteSpace(hotline) ? "1900 8888" : hotline.Trim();
            setting.SupportEmail = string.IsNullOrWhiteSpace(supportEmail) ? "hotro@recar.vn" : supportEmail.Trim();
            setting.PlatformName = string.IsNullOrWhiteSpace(platformName) ? "ReCar — Car-Sharing Platform" : platformName.Trim();
            setting.Address = string.IsNullOrWhiteSpace(address) ? "Tòa tháp Keangnam Landmark 72, Mễ Trì, Nam Từ Liêm, Hà Nội" : address.Trim();
            setting.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return Json(new { 
                success = true, 
                message = "Cấu hình hệ thống đã được lưu thành công! Các đơn đặt xe và tính toán tài chính phát sinh từ thời điểm này sẽ tự động áp dụng chính sách mới." 
            });
        }
    }
}
