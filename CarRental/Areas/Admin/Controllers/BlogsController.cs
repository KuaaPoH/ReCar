using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

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

                b.Alias = Function.TitleSlugGenerationAlias(b.Title ?? "tin-tuc");
                b.CreatedBy = Function.GetUserName(HttpContext.Session) ?? "Admin";
                b.CreatedDate = DateTime.Now;
                b.CustomerId = Function.GetAccountId(HttpContext.Session);

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
    }
}
