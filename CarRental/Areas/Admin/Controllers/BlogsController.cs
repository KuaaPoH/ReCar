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
    public class BlogsController : Controller
    {
        private readonly DbRenalCarContext _context;

        public BlogsController(DbRenalCarContext context)
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

            var blogs = _context.Blogs.OrderByDescending(b => b.BlogId).ToList();
            return View(blogs);
        }

        public IActionResult Create()
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            return View(new Blog { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Blog b, IFormFile? ImageFile)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "blogs");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + Path.GetFileName(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }
                    b.Image = "/images/blogs/" + uniqueFileName;
                }
                else if (string.IsNullOrEmpty(b.Image))
                {
                    b.Image = "/images/cars/hero.jpg";
                }

                b.Alias = Function.TitleSlugGenerationAlias(b.Title ?? "tin-tuc");
                b.CreatedBy = Function.GetUserName(HttpContext.Session) ?? "Admin";
                b.CreatedDate = DateTime.Now;
                b.CustomerId = Function.GetAccountId(HttpContext.Session);
                b.Views = 0;
                b.IsFeatured = b.IsFeatured ?? false;

                _context.Blogs.Add(b);
                _context.SaveChanges();
                TempData["Success"] = "Thêm bài viết mới thành công!";
                return RedirectToAction("Index");
            }
            return View(b);
        }

        public IActionResult Edit(int? id)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var blog = _context.Blogs.Find(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Blog b, IFormFile? ImageFile)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (ModelState.IsValid)
            {
                var existingBlog = _context.Blogs.Find(b.BlogId);
                if (existingBlog == null)
                {
                    return NotFound();
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "blogs");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + Path.GetFileName(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }
                    existingBlog.Image = "/images/blogs/" + uniqueFileName;
                }
                else if (!string.IsNullOrEmpty(b.Image))
                {
                    existingBlog.Image = b.Image;
                }

                existingBlog.Title = b.Title;
                existingBlog.Alias = Function.TitleSlugGenerationAlias(b.Title ?? "tin-tuc");
                existingBlog.Description = b.Description;
                existingBlog.Detail = b.Detail;
                existingBlog.IsActive = b.IsActive;
                existingBlog.IsFeatured = b.IsFeatured ?? false;
                existingBlog.Badge = b.Badge;
                existingBlog.SeoKeywords = b.SeoKeywords;

                existingBlog.ModifiedBy = Function.GetUserName(HttpContext.Session) ?? "Admin";
                existingBlog.ModifiedDate = DateTime.Now;

                _context.Blogs.Update(existingBlog);
                _context.SaveChanges();
                TempData["Success"] = "Cập nhật bài viết thành công!";
                return RedirectToAction("Index");
            }
            return View(b);
        }

        public IActionResult Delete(int? id)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var blog = _context.Blogs.Find(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var blog = _context.Blogs.Find(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa bài viết thành công!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAjax(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var blog = _context.Blogs.Find(id);
            if (blog == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết cần xóa." });
            }

            _context.Blogs.Remove(blog);
            _context.SaveChanges();
            return Json(new { success = true, message = "Đã xóa bài viết thành công!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleActive(int id)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var blog = _context.Blogs.Find(id);
            if (blog == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết." });
            }

            blog.IsActive = !blog.IsActive;
            blog.ModifiedDate = DateTime.Now;
            blog.ModifiedBy = Function.GetUserName(HttpContext.Session) ?? "Admin";
            _context.SaveChanges();

            return Json(new { 
                success = true, 
                isActive = blog.IsActive, 
                message = blog.IsActive ? "Đã xuất bản bài viết lên sàn ReCar." : "Đã chuyển bài viết về dạng Bản nháp." 
            });
        }

        // POST: Admin/Blogs/UploadEditorImage
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

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "blogs", "content");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = $"blog_content_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..6]}{ext}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string fileUrl = $"/images/blogs/content/{uniqueFileName}";
                return Json(new { success = true, url = fileUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Admin/Blogs/GetBlogPreview?id=5
        [HttpGet]
        public async Task<IActionResult> GetBlogPreview(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết này." });
            }

            return Json(new
            {
                success = true,
                blog = new
                {
                    id = blog.BlogId,
                    title = blog.Title,
                    alias = blog.Alias,
                    description = blog.Description,
                    detail = blog.Detail,
                    image = string.IsNullOrEmpty(blog.Image) ? "/images/cars/hero.jpg" : blog.Image,
                    createdBy = blog.CreatedBy ?? "Admin",
                    createdDate = blog.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? "Mới cập nhật",
                    isActive = blog.IsActive,
                    views = blog.Views ?? 0,
                    isFeatured = blog.IsFeatured ?? false,
                    badge = blog.Badge ?? "",
                    seoKeywords = string.IsNullOrEmpty(blog.SeoKeywords) ? "Kinh nghiệm thuê xe" : blog.SeoKeywords
                }
            });
        }
    }
}
