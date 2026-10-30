using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class BlogController : Controller
    {
        private readonly DbRenalCarContext _context;
        public BlogController(DbRenalCarContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/blog/{alias}-{id}.html")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(i => i.BlogComments)
                .FirstOrDefaultAsync(i => i.BlogId == id);

            if (blog == null)
            {
                blog = GetCuratedBlogById(id.Value);
            }

            if (blog == null)
            {
                return NotFound();
            }

            ViewBag.blogComment = await _context.BlogComments
                .Where(i => i.BlogId == id && (i.IsActive == null || i.IsActive == true))
                .OrderByDescending(i => i.CreatedDate)
                .ToListAsync();

            var dbRelated = await _context.Blogs
                .Where(i => i.IsActive && i.BlogId != id)
                .OrderByDescending(i => i.CreatedDate)
                .Take(4)
                .ToListAsync();

            foreach (var item in GetCuratedBlogs())
            {
                if (item.BlogId != id && dbRelated.Count < 4 && !dbRelated.Any(x => x.BlogId == item.BlogId))
                {
                    dbRelated.Add(item);
                }
            }
            ViewBag.RelatedPosts = dbRelated;

            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> AddBlogComment(string name, string email, string detail, int blogId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(detail))
                {
                    return Json(new { status = false, message = "Vui lòng nhập đầy đủ họ tên và nội dung bình luận." });
                }

                BlogComment c = new BlogComment
                {
                    Name = name.Trim(),
                    Email = email?.Trim(),
                    Detail = detail.Trim(),
                    CreatedDate = DateTime.Now,
                    BlogId = blogId > 0 && await _context.Blogs.AnyAsync(b => b.BlogId == blogId) ? blogId : null,
                    IsActive = true
                };

                _context.BlogComments.Add(c);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    status = true,
                    name = c.Name,
                    email = c.Email,
                    detail = c.Detail,
                    createdDate = c.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        public static List<Blog> GetCuratedBlogs()
        {
            return new List<Blog>
            {
                new Blog
                {
                    BlogId = 901,
                    Title = "Kinh nghiệm thuê xe tự lái lần đầu: 7 lưu ý quan trọng",
                    Alias = "kinh-nghiem-thue-xe-tu-lai-lan-dau",
                    Description = "Những điều cần kiểm tra trước khi nhận xe, cách quay video đối chiếu ngoại thất và lưu ý hợp đồng để chuyến đi suôn sẻ, an toàn.",
                    Image = "/images/cars/car-sedan.jpg",
                    CreatedBy = "Ban biên tập ReCar",
                    CreatedDate = new DateTime(2026, 10, 8),
                    SeoKeywords = "Kinh nghiệm thuê xe",
                    IsActive = true,
                    Detail = @"
                        <p class=""lead fw-medium text-dark"">Thuê xe ô tô tự lái đang trở thành lựa chọn ưu tiên của nhiều gia đình và nhóm bạn trẻ tại Việt Nam nhờ sự riêng tư, chủ động lịch trình và chi phí hợp lý. Nếu đây là lần đầu tiên bạn thuê xe tự lái trên ReCar, hãy lưu lại ngay 7 kinh nghiệm thực tế dưới đây.</p>
                        <h4>1. Chọn dòng xe phù hợp với cung đường và số lượng thành viên</h4>
                        <p>Trước khi đặt cọc, bạn nên xác định rõ lộ trình di chuyển:</p>
                        <ul>
                            <li><strong>Di chuyển nội đô hoặc nhóm 2–4 người:</strong> Các mẫu Sedan hoặc Hatchback 4–5 chỗ (như VinFast VF 5, Mazda 3, Toyota Vios) sẽ giúp bạn dễ dàng xoay trở và tiết kiệm nhiên liệu.</li>
                            <li><strong>Đi đường đèo dốc hoặc gia đình 5–7 người:</strong> Ưu tiên SUV gầm cao hoặc MPV rộng rãi (như Mitsubishi Xpander, Ford Everest, Hyundai SantaFe) để có khoang hành lý thoải mái.</li>
                        </ul>
                        <h4>2. Chuẩn bị đầy đủ giấy tờ pháp lý trước giờ nhận xe</h4>
                        <p>Khi nhận bàn giao phương tiện từ chủ xe đối tác ReCar, bạn cần xuất trình bản gốc để đối chiếu:</p>
                        <ul>
                            <li>Căn cước công dân (CCCD) gắn chip hoặc Hộ chiếu còn hiệu lực.</li>
                            <li>Giấy phép lái xe (GPLX) hạng B1 hoặc B2 trở lên còn thời hạn sử dụng.</li>
                        </ul>
                        <h4>3. Kiểm tra kỹ ngoại thất và nội thất khi bàn giao (Digital Check-in)</h4>
                        <p>Đừng bỏ qua bước đồng kiểm cùng chủ xe. Hãy sử dụng điện thoại chụp lại 4 góc xe (trước, sau, trái, phải) và quay một đoạn video ngắn quanh thân xe để ghi nhận các vết xước sẵn có (nếu có). Trên hệ thống ReCar, mọi thông tin này đều được lưu trữ minh bạch trong <strong>Biên bản bàn giao điện tử</strong>.</p>
                        <h4>4. Đối chiếu chỉ số ODO và mức nhiên liệu / dung lượng Pin</h4>
                        <p>Hãy chụp ảnh màn hình đồng hồ công-tơ-mét (ODO) lúc nhận xe để làm cơ sở tính giới hạn quãng đường di chuyển mỗi ngày (thường từ 300–400 km/ngày) và mức xăng/pin thực tế để hoàn trả đúng định mức khi kết thúc chuyến đi.</p>
                        <h4>5. Làm quen với các thao tác điều khiển cơ bản trên xe</h4>
                        <p>Mỗi dòng xe có thiết kế phanh tay điện tử, cần số, đèn pha, gạt mưa và nắp bình xăng/cổng sạc khác nhau. Hãy dành 3–5 phút nhờ chủ xe hướng dẫn nhanh và kết nối sẵn Apple CarPlay / Bluetooth trước khi lăn bánh.</p>"
                },
                new Blog
                {
                    BlogId = 902,
                    Title = "Giấy tờ cần chuẩn bị khi thuê ô tô tự lái & Quy trình ký hợp đồng",
                    Alias = "giay-to-can-chuan-bi-khi-thue-o-to",
                    Description = "Tổng hợp chi tiết về CCCD gắn chip, giấy phép lái xe hạng B1/B2, quy định đặt cọc 30% và các điều khoản đối chiếu khi ký hợp đồng thuê xe.",
                    Image = "/images/cars/car-suv.jpg",
                    CreatedBy = "Chuyên gia Pháp lý ReCar",
                    CreatedDate = new DateTime(2026, 10, 2),
                    SeoKeywords = "Thủ tục & Giấy tờ",
                    IsActive = true,
                    Detail = @"
                        <p class=""lead fw-medium text-dark"">Một trong những ưu điểm lớn nhất khi đặt xe qua nền tảng ReCar là thủ tục giấy tờ đã được chuẩn hóa 100%, minh bạch và không phát sinh các khoản thế chấp rườm rà.</p>
                        <h4>1. Bộ giấy tờ tiêu chuẩn dành cho người thuê xe</h4>
                        <p>Để đảm bảo tuân thủ Luật Giao thông đường bộ và bảo vệ quyền lợi bảo hiểm chuyến đi, khách hàng đứng tên hợp đồng cần có:</p>
                        <ul>
                            <li><strong>CCCD gắn chip / VNeID định danh mức 2:</strong> Dùng để xác thực danh tính người ký hợp đồng thuê xe.</li>
                            <li><strong>Giấy phép lái xe ô tô (B1, B2, C...):</strong> Phù hợp với loại hộp số (Tự động hoặc Số sàn) của chiếc xe bạn chọn thuê.</li>
                        </ul>
                        <h4>2. Trường hợp thuê xe có tài xế thì cần giấy tờ gì?</h4>
                        <p>Nếu bạn lựa chọn hình thức <strong>Thuê xe có tài xế</strong> trên ReCar, bạn <strong>không cần có Giấy phép lái xe ô tô</strong>. Bạn chỉ cần cung cấp thông tin liên hệ và CCCD để xác lập hợp đồng dịch vụ vận chuyển hành khách.</p>
                        <h4>3. Quy trình thanh toán đặt cọc 30% và ký hợp đồng</h4>
                        <ol>
                            <li><strong>Bước 1 – Giữ xe trực tuyến:</strong> Thanh toán trước 30% tổng giá trị đơn thuê qua cổng VNPay. Hệ thống ngay lập tức khóa lịch xe cho riêng bạn.</li>
                            <li><strong>Bước 2 – Nhận bàn giao xe:</strong> Gặp chủ xe tại địa điểm hẹn, đối chiếu giấy tờ bản gốc, kiểm tra tình trạng phương tiện và ký Hợp đồng thuê xe.</li>
                            <li><strong>Bước 3 – Thanh toán 70% còn lại:</strong> Hoàn tất phần chi phí thuê xe còn lại trực tiếp hoặc chuyển khoản khi nhận chìa khóa xe.</li>
                        </ol>"
                },
                new Blog
                {
                    BlogId = 903,
                    Title = "Bí quyết lái xe an toàn trên đường đèo dốc và cao tốc dịp cuối tuần",
                    Alias = "bi-quyet-lai-xe-an-toan-duong-deo-cao-toc",
                    Description = "Hướng dẫn kỹ thuật đổ đèo bằng số tay (L/S/B), giữ khoảng cách an toàn trên cao tốc và cách xử lý khi gặp thời tiết sương mù, mưa lớn.",
                    Image = "/images/cars/car-mpv.jpg",
                    CreatedBy = "Đội ngũ Kỹ thuật ReCar",
                    CreatedDate = new DateTime(2026, 9, 28),
                    SeoKeywords = "Lái xe an toàn",
                    IsActive = true,
                    Detail = @"
                        <p class=""lead fw-medium text-dark"">Những chuyến dã ngoại cuối tuần tới Đà Lạt, Sa Pa, Tam Đảo hay Mộc Châu luôn mang lại trải nghiệm tuyệt vời, nhưng cũng đòi hỏi người cầm lái nắm vững kỹ năng vận hành xe trên cao tốc và đường đèo.</p>
                        <h4>1. Nguyên tắc vàng khi đổ đèo: Lên số nào – Xuống số đó</h4>
                        <p>Tuyệt đối không rà phanh liên tục khi xuống dốc dài vì sẽ làm má phanh quá nhiệt, dẫn đến mất phanh. Ngay cả với xe số tự động, hãy chuyển sang chế độ bán tự động (<strong>S, M, L hoặc lẫy chuyển số +/- trên vô lăng</strong>) để tận dụng lực hãm của động cơ.</p>
                        <h4>2. Giữ khoảng cách an toàn theo quy tắc 3 giây trên cao tốc</h4>
                        <p>Khi di chuyển ở tốc độ 80–120 km/h trên đường cao tốc, hãy luôn duy trì khoảng cách tối thiểu từ 70m – 100m với xe phía trước và bật đèn báo rẽ sớm trước khi chuyển làn.</p>
                        <h4>3. Kiểm tra lốp xe và hệ thống chiếu sáng trước khi khởi hành</h4>
                        <p>Trước khi bắt đầu hành trình dài, hãy kiểm tra áp suất lốp (bao gồm cả lốp dự phòng), nước rửa kính và hệ thống đèn sương mù để luôn chủ động trong mọi điều kiện thời tiết.</p>"
                }
            };
        }

        private static Blog? GetCuratedBlogById(int id)
        {
            return GetCuratedBlogs().FirstOrDefault(b => b.BlogId == id);
        }
    }
}
