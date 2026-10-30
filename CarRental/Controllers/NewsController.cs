using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class NewsController : Controller
    {
        private readonly DbRenalCarContext _context;
        public NewsController(DbRenalCarContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/news/{alias}-{id}.html")]
        public async Task<IActionResult> Details(string alias, int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FirstOrDefaultAsync(m => m.NewsId == id);
            if (news == null)
            {
                news = GetCuratedNewsById(id.Value);
            }

            if (news == null)
            {
                return NotFound();
            }

            ViewBag.newsComment = await _context.NewsComments
                .Where(r => r.NewsId == news.NewsId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            var dbRelated = await _context.News
                .Where(n => n.IsActive && n.NewsId != id)
                .OrderByDescending(n => n.CreatedDate)
                .Take(4)
                .ToListAsync();

            foreach (var item in GetCuratedNews())
            {
                if (item.NewsId != id && dbRelated.Count < 4 && !dbRelated.Any(x => x.NewsId == item.NewsId))
                {
                    dbRelated.Add(item);
                }
            }
            ViewBag.RelatedNews = dbRelated;

            return View(news);
        }

        [HttpPost]
        public async Task<IActionResult> Comment(string name, string email, string detail, int id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(detail))
                {
                    return Json(new { status = false, message = "Vui lòng nhập đầy đủ họ tên và nội dung bình luận." });
                }

                NewsComment r = new NewsComment
                {
                    Name = name.Trim(),
                    Email = email?.Trim(),
                    CreatedDate = DateTime.Now,
                    Detail = detail.Trim(),
                    NewsId = id > 0 && await _context.News.AnyAsync(n => n.NewsId == id) ? id : null,
                    IsActive = true
                };

                _context.NewsComments.Add(r);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    status = true,
                    name = r.Name,
                    email = r.Email,
                    detail = r.Detail,
                    createdDate = r.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        public static List<News> GetCuratedNews()
        {
            return new List<News>
            {
                new News
                {
                    NewsId = 901,
                    Title = "5 cung đường đẹp quanh Đà Lạt dành cho tín đồ mê cầm lái",
                    Alias = "5-cung-duong-dep-quanh-da-lat",
                    Description = "Gợi ý lộ trình cuối tuần tuyệt vời qua đèo Tà Nung, Cầu Đất, hồ Tuyền Lâm cho những ai yêu thích trải nghiệm lái xe ngắm cảnh cao nguyên.",
                    Image = "/images/cars/hero.jpg",
                    CreatedBy = "Ban biên tập ReCar",
                    CreatedDate = new DateTime(2026, 9, 25),
                    SeoKeywords = "Hành trình du lịch",
                    IsActive = true,
                    Detail = @"
                        <p class=""lead fw-medium text-dark"">Đà Lạt luôn là điểm đến lý tưởng cho những chuyến road-trip tự lái nhờ khí hậu mát mẻ quanh năm và những cung đường đèo uốn lượn giữa rừng thông bạt ngàn. Dưới đây là 5 cung đường đẹp nhất mà bạn không nên bỏ lỡ khi thuê xe tự lái tại Đà Lạt.</p>
                        <h4>1. Cung đường Đèo Prenn – Hồ Tuyền Lâm</h4>
                        <p>Sau khi nâng cấp mở rộng mặt đường, đèo Prenn trở nên êm ái và rộng rãi hơn bao giờ hết. Từ giữa đèo, bạn có thể rẽ vào khu du lịch quốc gia Hồ Tuyền Lâm để tận hưởng không khí trong lành và mặt hồ phẳng lặng như gương.</p>
                        <h4>2. Cung đường săn mây Trại Mát – Đồi chè Cầu Đất</h4>
                        <p>Cách trung tâm Đà Lạt khoảng 25 km về phía Đông Nam, cung đường dẫn tới Đồi chè Cầu Đất và cánh đồng quạt gió là địa điểm hoàn hảo để khởi hành lúc 5h00 sáng săn biển mây bồng bềnh.</p>
                        <h4>3. Cung đường Đèo Tà Nung – Thác Voi</h4>
                        <p>Nếu bạn yêu thích cảm giác cầm lái một chiếc SUV gầm cao băng qua những khúc cua ngoạn mục ngập tràn hoa dã quỳ và những trang trại cà phê chồn, đèo Tà Nung chắc chắn sẽ làm bạn say đắm.</p>
                        <h4>4. Cung đường Suối Vàng – Cây thông cô đơn</h4>
                        <p>Tuyến đường ven hồ Suối Vàng (Đankia) với những thảm cỏ hồng trải dài là nơi lý tưởng để dừng xe chụp ảnh kỷ niệm và cắm trại dã ngoại trong ngày cùng gia đình.</p>
                        <h4>5. Cung đường Đèo Khánh Lê (Nha Trang – Đà Lạt)</h4>
                        <p>Cung đường nối liền biển và hoa dài 130 km đưa bạn đi từ nắng gió vịnh biển Nha Trang lên tới độ cao 1.500m của đỉnh Hòn Giao mờ sương chỉ trong hơn 2 giờ cầm lái.</p>"
                },
                new News
                {
                    NewsId = 902,
                    Title = "Xu hướng thuê ô tô điện VinFast tự lái tăng trưởng mạnh năm 2026",
                    Alias = "xu-huong-thue-o-to-dien-vinfast-tu-lai-tang-truong-manh",
                    Description = "Nhờ chi phí vận hành tiết kiệm, khả năng vận hành êm ái không mùi xăng và mạng lưới trạm sạc phủ rộng toàn quốc, các dòng xe VF 5, VF 6, VF 8 ngày càng được ưa chuộng.",
                    Image = "/images/cars/car-electric.jpg",
                    CreatedBy = "Chuyên mục Thị trường Xe",
                    CreatedDate = new DateTime(2026, 10, 12),
                    SeoKeywords = "Công nghệ & Xe điện",
                    IsActive = true,
                    Detail = @"
                        <p class=""lead fw-medium text-dark"">Trong năm 2026, lượng đơn đặt thuê các dòng xe điện thông minh trên nền tảng ReCar ghi nhận mức tăng trưởng hơn 65% so với cùng kỳ, đặc biệt ở phân khúc xe đô thị và C-SUV gia đình.</p>
                        <h4>1. Trải nghiệm êm ái, hạn chế say xe tối đa cho gia đình</h4>
                        <p>Ưu điểm vượt trội của động cơ thuần điện là khả năng tăng tốc mượt mà, không có tiếng ồn động cơ đốt trong và hoàn toàn không có mùi nhiên liệu trong khoang cabin, giúp trẻ nhỏ và người lớn tuổi thoải mái suốt hành trình.</p>
                        <h4>2. Hệ thống hỗ trợ lái nâng cao ADAS thông minh</h4>
                        <p>Các mẫu xe điện thế hệ mới trên ReCar đều được trang bị gói hỗ trợ lái thông minh như giữ làn đường, kiểm soát hành trình thích ứng (Adaptive Cruise Control), cảnh báo điểm mù và camera 360 độ sắc nét.</p>"
                },
                new News
                {
                    NewsId = 903,
                    Title = "ReCar ra mắt tính năng Biên bản bàn giao xe điện tử (Digital Check-in)",
                    Alias = "recar-ra-mat-bien-ban-ban-giao-xe-dien-tu-digital-checkin",
                    Description = "Chuẩn hóa quy trình nhận và trả xe bằng hình ảnh 4 góc thực tế, đối soát ODO và mức nhiên liệu tự động nhằm bảo vệ quyền lợi cho cả khách thuê và chủ xe.",
                    Image = "/images/cars/car-luxury.jpg",
                    CreatedBy = "Tin tức ReCar",
                    CreatedDate = new DateTime(2026, 10, 18),
                    SeoKeywords = "Tin tức ReCar",
                    IsActive = true,
                    Detail = @"
                        <p class=""lead fw-medium text-dark"">Nhằm xóa bỏ hoàn toàn những tranh chấp không đáng có về tình trạng ngoại thất hay phụ phí vượt km sau chuyến đi, ReCar chính thức tích hợp hệ thống Biên bản bàn giao xe điện tử ngay trên tài khoản khách hàng và chủ xe.</p>
                        <h4>1. Minh bạch 100% chỉ số ODO và nhiên liệu</h4>
                        <p>Khi bàn giao xe, chủ xe cập nhật chỉ số công-tơ-mét (ODO), phần trăm nhiên liệu/pin kèm hình ảnh 4 góc xe. Khách hàng có thể xem trực tiếp biên bản này trong mục <strong>Đơn thuê của tôi</strong>.</p>
                        <h4>2. Tự động tính toán quãng đường di chuyển khi trả xe</h4>
                        <p>Ngay khi hoàn tất Check-out lúc trả xe, hệ thống tự động đối chiếu số km thực tế đã đi so với hạn mức ngày và hiển thị chi tiết trên Hợp đồng điện tử.</p>"
                }
            };
        }

        private static News? GetCuratedNewsById(int id)
        {
            return GetCuratedNews().FirstOrDefault(n => n.NewsId == id);
        }
    }
}
