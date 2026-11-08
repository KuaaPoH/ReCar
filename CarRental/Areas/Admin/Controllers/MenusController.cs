using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Utilities;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenusController : Controller
    {
        private readonly DbRenalCarContext _context;

        public static readonly List<(string Icon, string Name)> AvailableIcons = new()
        {
            ("bi-house-door", "Trang chủ"),
            ("bi-car-front", "Xe hơi"),
            ("bi-car-front-fill", "Đội xe"),
            ("bi-speedometer2", "Bảng điều khiển (Dashboard)"),
            ("bi-receipt", "Đơn hàng"),
            ("bi-receipt-cutoff", "Giám sát đơn"),
            ("bi-wallet2", "Ví & Tài chính"),
            ("bi-cash-coin", "Doanh thu & Đối soát"),
            ("bi-people-fill", "Người dùng & Khách"),
            ("bi-person-badge", "Chủ xe / Tài xế"),
            ("bi-person-check", "Duyệt hồ sơ"),
            ("bi-calendar-check", "Lịch bận & Bảo dưỡng"),
            ("bi-chat-dots", "Tin nhắn / Trao đổi"),
            ("bi-star-fill", "Đánh giá & Review"),
            ("bi-newspaper", "Tin tức & Cẩm nang"),
            ("bi-info-circle", "Thông tin & Giới thiệu"),
            ("bi-headset", "Hỗ trợ & Hotline"),
            ("bi-shield-check", "Bảo mật & Đăng ký"),
            ("bi-sliders2", "Cấu hình sàn"),
            ("bi-gear", "Cài đặt"),
            ("bi-menu-button-wide", "Quản lý Menu"),
            ("bi-graph-up-arrow", "Báo cáo phân tích"),
            ("bi-plus-circle", "Thêm mới"),
            ("bi-box-arrow-up-right", "Mở liên kết"),
            ("bi-link-45deg", "Liên kết mặc định")
        };

        public MenusController(DbRenalCarContext context)
        {
            _context = context;
        }

        private bool CheckAdmin()
        {
            return Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session);
        }

        // GET: Admin/Menus
        public async Task<IActionResult> Index(string? tab)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            string activeTab = string.IsNullOrWhiteSpace(tab) ? "Customer" : tab;
            if (activeTab != "Customer" && activeTab != "CarOwner" && activeTab != "Admin")
            {
                activeTab = "Customer";
            }

            var allMenus = await _context.Menus
                .OrderBy(m => m.Subsystem)
                .ThenBy(m => m.ParentId)
                .ThenBy(m => m.Position)
                .ToListAsync();

            ViewBag.ActiveTab = activeTab;
            ViewBag.CustomerCount = allMenus.Count(m => m.Subsystem == "Customer");
            ViewBag.CarOwnerCount = allMenus.Count(m => m.Subsystem == "CarOwner");
            ViewBag.AdminCount = allMenus.Count(m => m.Subsystem == "Admin");

            return View(allMenus);
        }

        // GET: Admin/Menus/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            return RedirectToAction(nameof(Edit), new { id });
        }

        // GET: Admin/Menus/Create
        public IActionResult Create(string? subsystem, int? parentId)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            string targetSubsystem = string.IsNullOrWhiteSpace(subsystem) ? "Customer" : subsystem;
            if (targetSubsystem != "Customer" && targetSubsystem != "CarOwner" && targetSubsystem != "Admin")
            {
                targetSubsystem = "Customer";
            }

            // Danh sách các menu cha tiềm năng (cùng phân hệ và là cấp 1)
            var parentCandidates = _context.Menus
                .Where(m => m.Subsystem == targetSubsystem && (m.ParentId == null || m.ParentId == 0))
                .OrderBy(m => m.Position)
                .ToList();

            ViewBag.ParentCandidates = parentCandidates;
            ViewBag.AvailableIcons = AvailableIcons;
            ViewBag.TargetSubsystem = targetSubsystem;
            ViewBag.SelectedParentId = parentId ?? 0;

            var newMenu = new Menu
            {
                Subsystem = targetSubsystem,
                ParentId = parentId ?? 0,
                Levels = (parentId.HasValue && parentId.Value > 0) ? 2 : 1,
                Position = 1,
                IsActive = true,
                Icon = "bi-link-45deg"
            };

            return View(newMenu);
        }

        // POST: Admin/Menus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Menu menu)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (string.IsNullOrWhiteSpace(menu.Title))
            {
                ModelState.AddModelError("Title", "Vui lòng nhập tiêu đề Menu.");
            }

            if (ModelState.IsValid)
            {
                menu.Title = menu.Title!.Trim();
                menu.Alias = !string.IsNullOrWhiteSpace(menu.Alias) 
                    ? Function.TitleSlugGenerationAlias(menu.Alias) 
                    : Function.TitleSlugGenerationAlias(menu.Title);

                menu.Url = !string.IsNullOrWhiteSpace(menu.Url) ? menu.Url.Trim() : ("/" + menu.Alias);
                menu.Icon = !string.IsNullOrWhiteSpace(menu.Icon) ? menu.Icon.Trim() : "bi-link-45deg";
                
                if (menu.ParentId.HasValue && menu.ParentId.Value > 0)
                {
                    menu.Levels = 2;
                }
                else
                {
                    menu.ParentId = 0;
                    menu.Levels = 1;
                }

                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đã tạo mới menu '{menu.Title}' cho phân hệ {menu.Subsystem}.";
                return RedirectToAction(nameof(Index), new { tab = menu.Subsystem });
            }

            ViewBag.ParentCandidates = _context.Menus
                .Where(m => m.Subsystem == menu.Subsystem && (m.ParentId == null || m.ParentId == 0))
                .OrderBy(m => m.Position)
                .ToList();
            ViewBag.AvailableIcons = AvailableIcons;
            ViewBag.TargetSubsystem = menu.Subsystem;
            ViewBag.SelectedParentId = menu.ParentId ?? 0;

            return View(menu);
        }

        // GET: Admin/Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (id == null) return NotFound();

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            // Menu cha phải cùng phân hệ và không phải chính nó
            var parentCandidates = await _context.Menus
                .Where(m => m.Subsystem == menu.Subsystem && m.MenuId != menu.MenuId && (m.ParentId == null || m.ParentId == 0))
                .OrderBy(m => m.Position)
                .ToListAsync();

            ViewBag.ParentCandidates = parentCandidates;
            ViewBag.AvailableIcons = AvailableIcons;
            ViewBag.TargetSubsystem = menu.Subsystem;
            ViewBag.SelectedParentId = menu.ParentId ?? 0;

            return View(menu);
        }

        // POST: Admin/Menus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Menu menu)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (id != menu.MenuId) return NotFound();

            if (string.IsNullOrWhiteSpace(menu.Title))
            {
                ModelState.AddModelError("Title", "Vui lòng nhập tiêu đề Menu.");
            }

            if (ModelState.IsValid)
            {
                var existing = await _context.Menus.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Title = menu.Title!.Trim();
                existing.Alias = !string.IsNullOrWhiteSpace(menu.Alias) 
                    ? Function.TitleSlugGenerationAlias(menu.Alias) 
                    : Function.TitleSlugGenerationAlias(menu.Title);

                existing.Url = !string.IsNullOrWhiteSpace(menu.Url) ? menu.Url.Trim() : ("/" + existing.Alias);
                existing.Icon = !string.IsNullOrWhiteSpace(menu.Icon) ? menu.Icon.Trim() : "bi-link-45deg";
                existing.Subsystem = menu.Subsystem;
                existing.Description = menu.Description;
                existing.Position = menu.Position ?? 1;
                existing.IsActive = menu.IsActive;

                if (menu.ParentId.HasValue && menu.ParentId.Value > 0)
                {
                    existing.ParentId = menu.ParentId.Value;
                    existing.Levels = 2;
                }
                else
                {
                    existing.ParentId = 0;
                    existing.Levels = 1;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đã cập nhật menu '{existing.Title}' thành công.";
                return RedirectToAction(nameof(Index), new { tab = existing.Subsystem });
            }

            ViewBag.ParentCandidates = await _context.Menus
                .Where(m => m.Subsystem == menu.Subsystem && m.MenuId != menu.MenuId && (m.ParentId == null || m.ParentId == 0))
                .OrderBy(m => m.Position)
                .ToListAsync();
            ViewBag.AvailableIcons = AvailableIcons;
            ViewBag.TargetSubsystem = menu.Subsystem;
            ViewBag.SelectedParentId = menu.ParentId ?? 0;

            return View(menu);
        }

        // POST: Admin/Menus/ToggleActive
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return Json(new { success = false, message = "Không tìm thấy menu." });
            }

            menu.IsActive = !(menu.IsActive ?? false);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                isActive = menu.IsActive.Value, 
                message = $"Đã {(menu.IsActive.Value ? "kích hoạt" : "tạm ẩn")} menu '{menu.Title}'." 
            });
        }

        // POST: Admin/Menus/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return Json(new { success = false, message = "Không tìm thấy menu." });
            }

            // Nếu menu này có các menu con, chuyển các con về cấp 1 (ParentId = 0)
            var children = await _context.Menus.Where(m => m.ParentId == id).ToListAsync();
            foreach (var child in children)
            {
                child.ParentId = 0;
                child.Levels = 1;
            }

            string sub = menu.Subsystem;
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();

            return Json(new { success = true, subsystem = sub, message = $"Đã xóa menu '{menu.Title}' thành công." });
        }
    }
}
