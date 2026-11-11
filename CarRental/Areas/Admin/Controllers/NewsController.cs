using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsController : Controller
    {
        private readonly DbRenalCarContext _context;

        public NewsController(DbRenalCarContext context)
        {
            _context = context;
        }

        private bool CheckAdmin()
        {
            return Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session);
        }

        // GET: Admin/News
        public IActionResult Index()
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var newsList = _context.News.OrderByDescending(n => n.NewsId).ToList();
            return View(newsList);
        }

        // GET: Admin/News/Create
        public IActionResult Create()
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            return View(new News { IsActive = true });
        }

        // POST: Admin/News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News n, IFormFile? ImageFile)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "news");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = $"news_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..6]}{Path.GetExtension(ImageFile.FileName).ToLowerInvariant()}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    n.Image = "/images/news/" + uniqueFileName;
                }
                else if (string.IsNullOrEmpty(n.Image))
                {
                    n.Image = "/images/cars/hero.jpg";
                }

                n.Alias = Function.TitleSlugGenerationAlias(n.Title ?? "tin-tuc");
                n.CreatedBy = Function.GetUserName(HttpContext.Session) ?? "ReCar Editorial";
                n.CreatedDate = DateTime.Now;
                n.Views = 0;

                _context.News.Add(n);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm bản tin mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(n);
        }

        // GET: Admin/News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: Admin/News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(News n, IFormFile? ImageFile)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (ModelState.IsValid)
            {
                var existingNews = await _context.News.FindAsync(n.NewsId);
                if (existingNews == null)
                {
                    return NotFound();
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "news");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = $"news_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..6]}{Path.GetExtension(ImageFile.FileName).ToLowerInvariant()}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    existingNews.Image = "/images/news/" + uniqueFileName;
                }
                else if (!string.IsNullOrEmpty(n.Image))
                {
                    existingNews.Image = n.Image;
                }

                existingNews.Title = n.Title;
                existingNews.Alias = Function.TitleSlugGenerationAlias(n.Title ?? "tin-tuc");
                existingNews.Description = n.Description;
                existingNews.Detail = n.Detail;
                existingNews.IsActive = n.IsActive;
                existingNews.IsFeatured = n.IsFeatured ?? false;
                existingNews.Badge = n.Badge;
                existingNews.SeoKeywords = n.SeoKeywords;

                existingNews.ModifiedBy = Function.GetUserName(HttpContext.Session) ?? "ReCar Editorial";
                existingNews.ModifiedDate = DateTime.Now;

                _context.News.Update(existingNews);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật bản tin thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(n);
        }

        // GET: Admin/News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: Admin/News/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa bản tin thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/News/DeleteAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bản tin cần xóa." });
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã xóa bản tin thành công!" });
        }

        // POST: Admin/News/ToggleActive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bản tin." });
            }

            news.IsActive = !news.IsActive;
            news.ModifiedDate = DateTime.Now;
            news.ModifiedBy = Function.GetUserName(HttpContext.Session) ?? "ReCar Editorial";
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                isActive = news.IsActive,
                message = news.IsActive ? "Đã xuất bản bản tin lên sàn ReCar." : "Đã chuyển bản tin về dạng Bản nháp."
            });
        }

        // POST: Admin/News/UploadEditorImage
        [HttpPost]
        public async Task<IActionResult> UploadEditorImage(IFormFile file)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn tệp ảnh hợp lệ." });
            }

            try
            {
                var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExt.Contains(ext))
                {
                    return Json(new { success = false, message = "Chỉ chấp nhận các định dạng ảnh: JPG, PNG, WEBP, GIF." });
                }

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "news", "content");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = $"news_content_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..6]}{ext}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string fileUrl = $"/images/news/content/{uniqueFileName}";
                return Json(new { success = true, url = fileUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Admin/News/GetNewsPreview?id=5
        [HttpGet]
        public async Task<IActionResult> GetNewsPreview(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bản tin này." });
            }

            return Json(new
            {
                success = true,
                news = new
                {
                    id = news.NewsId,
                    title = news.Title,
                    alias = news.Alias,
                    description = news.Description,
                    detail = news.Detail,
                    image = string.IsNullOrEmpty(news.Image) ? "/images/cars/hero.jpg" : news.Image,
                    createdBy = news.CreatedBy ?? "ReCar Editorial",
                    createdDate = news.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? "Mới cập nhật",
                    isActive = news.IsActive,
                    views = news.Views ?? 0,
                    isFeatured = news.IsFeatured ?? false,
                    badge = news.Badge ?? "",
                    seoKeywords = string.IsNullOrEmpty(news.SeoKeywords) ? "Tin tức ReCar" : news.SeoKeywords
                }
            });
        }
    }
}

