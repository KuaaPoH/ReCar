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

        private bool CheckAdmin()
        {
            return Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session);
        }

        // GET: Admin/Reviews
        public async Task<IActionResult> Index(string tab = "cars")
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
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

            ViewBag.NewsComments = await _context.NewsComments
                .Include(c => c.News)
                .OrderByDescending(c => c.CreatedDate ?? DateTime.MinValue)
                .ToListAsync();

            return View();
        }

        // POST: Admin/Reviews/ToggleCarReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCarReview(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Login", new { area = "Admin" });

            var review = await _context.CarReviews.FindAsync(id);
            if (review != null)
            {
                review.IsActive = !(review.IsActive ?? false);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã {(review.IsActive == true ? "phê duyệt hiển thị" : "tạm ẩn")} đánh giá của {review.Name}!";
            }
            return RedirectToAction(nameof(Index), new { tab = "cars" });
        }

        // AJAX Toggle CarReview
        [HttpPost]
        public async Task<IActionResult> ToggleCarReviewAjax(int id)
        {
            if (!CheckAdmin()) return Json(new { success = false, message = "Không có quyền thực hiện." });

            var review = await _context.CarReviews.FindAsync(id);
            if (review != null)
            {
                review.IsActive = !(review.IsActive ?? false);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isActive = review.IsActive ?? false, message = $"Đã {(review.IsActive == true ? "phê duyệt hiển thị" : "tạm ẩn")} đánh giá." });
            }
            return Json(new { success = false, message = "Không tìm thấy bản ghi." });
        }

        // POST: Admin/Reviews/DeleteCarReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCarReview(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Login", new { area = "Admin" });

            var review = await _context.CarReviews.FindAsync(id);
            if (review != null)
            {
                _context.CarReviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa đánh giá thành công!";
            }
            return RedirectToAction(nameof(Index), new { tab = "cars" });
        }

        // AJAX Delete CarReview
        [HttpPost]
        public async Task<IActionResult> DeleteCarReviewAjax(int id)
        {
            if (!CheckAdmin()) return Json(new { success = false, message = "Không có quyền thực hiện." });

            var review = await _context.CarReviews.FindAsync(id);
            if (review != null)
            {
                _context.CarReviews.Remove(review);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã xóa đánh giá thành công." });
            }
            return Json(new { success = false, message = "Không tìm thấy bản ghi." });
        }

        // POST: Admin/Reviews/ToggleBlogComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBlogComment(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Login", new { area = "Admin" });

            var comment = await _context.BlogComments.FindAsync(id);
            if (comment != null)
            {
                comment.IsActive = !comment.IsActive;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã {(comment.IsActive ? "phê duyệt hiển thị" : "tạm ẩn")} bình luận của {comment.Name}!";
            }
            return RedirectToAction(nameof(Index), new { tab = "blogs" });
        }

        // AJAX Toggle BlogComment
        [HttpPost]
        public async Task<IActionResult> ToggleBlogCommentAjax(int id)
        {
            if (!CheckAdmin()) return Json(new { success = false, message = "Không có quyền thực hiện." });

            var comment = await _context.BlogComments.FindAsync(id);
            if (comment != null)
            {
                comment.IsActive = !comment.IsActive;
                await _context.SaveChangesAsync();
                return Json(new { success = true, isActive = comment.IsActive, message = $"Đã {(comment.IsActive ? "phê duyệt hiển thị" : "tạm ẩn")} bình luận." });
            }
            return Json(new { success = false, message = "Không tìm thấy bản ghi." });
        }

        // POST: Admin/Reviews/DeleteBlogComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlogComment(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Login", new { area = "Admin" });

            var comment = await _context.BlogComments.FindAsync(id);
            if (comment != null)
            {
                _context.BlogComments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa bình luận thành công!";
            }
            return RedirectToAction(nameof(Index), new { tab = "blogs" });
        }

        // AJAX Delete BlogComment
        [HttpPost]
        public async Task<IActionResult> DeleteBlogCommentAjax(int id)
        {
            if (!CheckAdmin()) return Json(new { success = false, message = "Không có quyền thực hiện." });

            var comment = await _context.BlogComments.FindAsync(id);
            if (comment != null)
            {
                _context.BlogComments.Remove(comment);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã xóa bình luận thành công." });
            }
            return Json(new { success = false, message = "Không tìm thấy bản ghi." });
        }

        // POST: Admin/Reviews/ReplyBlogComment
        [HttpPost]
        public async Task<IActionResult> ReplyBlogComment(int id, string? replyText)
        {
            if (!CheckAdmin()) return Json(new { success = false, message = "Không có quyền thực hiện." });

            var comment = await _context.BlogComments.FindAsync(id);
            if (comment != null)
            {
                comment.Reply = string.IsNullOrWhiteSpace(replyText) ? null : replyText.Trim();
                comment.ReplyDate = string.IsNullOrWhiteSpace(replyText) ? null : Function.NowVn;
                await _context.SaveChangesAsync();
                return Json(new { 
                    success = true, 
                    reply = comment.Reply, 
                    replyDate = comment.ReplyDate?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    message = string.IsNullOrWhiteSpace(replyText) ? "Đã gỡ phản hồi." : "Đã lưu phản hồi thành công!"
                });
            }
            return Json(new { success = false, message = "Không tìm thấy bình luận." });
        }

        // POST: Admin/Reviews/ToggleNewsComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleNewsComment(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Login", new { area = "Admin" });

            var comment = await _context.NewsComments.FindAsync(id);
            if (comment != null)
            {
                comment.IsActive = !(comment.IsActive ?? false);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã {(comment.IsActive == true ? "phê duyệt hiển thị" : "tạm ẩn")} bình luận của {comment.Name}!";
            }
            return RedirectToAction(nameof(Index), new { tab = "news" });
        }

        // AJAX Toggle NewsComment
        [HttpPost]
        public async Task<IActionResult> ToggleNewsCommentAjax(int id)
        {
            if (!CheckAdmin()) return Json(new { success = false, message = "Không có quyền thực hiện." });

            var comment = await _context.NewsComments.FindAsync(id);
            if (comment != null)
            {
                comment.IsActive = !(comment.IsActive ?? false);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isActive = comment.IsActive ?? false, message = $"Đã {(comment.IsActive == true ? "phê duyệt hiển thị" : "tạm ẩn")} bình luận." });
            }
            return Json(new { success = false, message = "Không tìm thấy bản ghi." });
        }

        // POST: Admin/Reviews/DeleteNewsComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNewsComment(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Login", new { area = "Admin" });

            var comment = await _context.NewsComments.FindAsync(id);
            if (comment != null)
            {
                _context.NewsComments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa bình luận bản tin thành công!";
            }
            return RedirectToAction(nameof(Index), new { tab = "news" });
        }

        // AJAX Delete NewsComment
        [HttpPost]
        public async Task<IActionResult> DeleteNewsCommentAjax(int id)
        {
            if (!CheckAdmin()) return Json(new { success = false, message = "Không có quyền thực hiện." });

            var comment = await _context.NewsComments.FindAsync(id);
            if (comment != null)
            {
                _context.NewsComments.Remove(comment);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã xóa bình luận thành công." });
            }
            return Json(new { success = false, message = "Không tìm thấy bản ghi." });
        }

        // POST: Admin/Reviews/ReplyNewsComment
        [HttpPost]
        public async Task<IActionResult> ReplyNewsComment(int id, string? replyText)
        {
            if (!CheckAdmin()) return Json(new { success = false, message = "Không có quyền thực hiện." });

            var comment = await _context.NewsComments.FindAsync(id);
            if (comment != null)
            {
                comment.Reply = string.IsNullOrWhiteSpace(replyText) ? null : replyText.Trim();
                comment.ReplyDate = string.IsNullOrWhiteSpace(replyText) ? null : Function.NowVn;
                await _context.SaveChangesAsync();
                return Json(new { 
                    success = true, 
                    reply = comment.Reply, 
                    replyDate = comment.ReplyDate?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    message = string.IsNullOrWhiteSpace(replyText) ? "Đã gỡ phản hồi." : "Đã lưu phản hồi thành công!"
                });
            }
            return Json(new { success = false, message = "Không tìm thấy bình luận." });
        }
    }
}
