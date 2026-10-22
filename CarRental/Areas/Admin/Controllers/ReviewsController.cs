using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Utilities;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReviewsController : Controller
    {
        private readonly DbRenalCarContext _context;

        public ReviewsController(DbRenalCarContext context)
        {
            _context = context;
        }

        // GET: Admin/Reviews
        public async Task<IActionResult> Index(string tab = "cars")
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.ActiveTab = tab;
            ViewBag.CarReviews = await _context.CarReviews
                .Include(r => r.Car)
                .OrderByDescending(r => r.CreatedDate ?? DateTime.MinValue)
                .ToListAsync();

            ViewBag.BlogComments = await _context.BlogComments
                .Include(c => c.Blog)
                .OrderByDescending(c => c.CreatedDate ?? DateTime.MinValue)
                .ToListAsync();

            return View();
        }

        // POST: Admin/Reviews/ToggleCarReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCarReview(int id)
        {
            if (!Function.IsLogin()) return RedirectToAction("Index", "Login");

            var review = await _context.CarReviews.FindAsync(id);
            if (review != null)
            {
                review.IsActive = !(review.IsActive ?? false);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã {(review.IsActive == true ? "phê duyệt hiển thị" : "tạm ẩn")} đánh giá của {review.Name}!";
            }
            return RedirectToAction(nameof(Index), new { tab = "cars" });
        }

        // POST: Admin/Reviews/DeleteCarReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCarReview(int id)
        {
            if (!Function.IsLogin()) return RedirectToAction("Index", "Login");

            var review = await _context.CarReviews.FindAsync(id);
            if (review != null)
            {
                _context.CarReviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa đánh giá thành công!";
            }
            return RedirectToAction(nameof(Index), new { tab = "cars" });
        }

        // POST: Admin/Reviews/ToggleBlogComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBlogComment(int id)
        {
            if (!Function.IsLogin()) return RedirectToAction("Index", "Login");

            var comment = await _context.BlogComments.FindAsync(id);
            if (comment != null)
            {
                comment.IsActive = !comment.IsActive;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã {(comment.IsActive ? "phê duyệt hiển thị" : "tạm ẩn")} bình luận của {comment.Name}!";
            }
            return RedirectToAction(nameof(Index), new { tab = "blogs" });
        }

        // POST: Admin/Reviews/DeleteBlogComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlogComment(int id)
        {
            if (!Function.IsLogin()) return RedirectToAction("Index", "Login");

            var comment = await _context.BlogComments.FindAsync(id);
            if (comment != null)
            {
                _context.BlogComments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa bình luận thành công!";
            }
            return RedirectToAction(nameof(Index), new { tab = "blogs" });
        }
    }
}
