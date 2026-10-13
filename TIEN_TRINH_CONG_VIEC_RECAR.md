# BÁO CÁO TIẾN TRÌNH CÔNG VIỆC DỰ ÁN RECAr (ĐỒ ÁN TỐT NGHIỆP)
**Thời gian thực hiện:** Ngày 13 - 14 tháng 10 năm 2026  
**Dự án:** ReCar – Hệ thống cho thuê xe ô tô trực tuyến  
**Nền tảng công nghệ:** ASP.NET Core 9 (MVC), Entity Framework Core, MySQL (`DbRenalCar`), Bootstrap 5, Vanilla CSS Design System.

---

## 1. MỤC TIÊU CÔNG VIỆC TRONG NGÀY
- Nâng cấp toàn diện hệ thống từ giao diện cũ (FloSun) sang hệ thống thuê xe ô tô chuyên nghiệp **ReCar**, đạt chuẩn **Pixel-Perfect 100%** theo mẫu template Lovable (`pixel-perfect-render-09837`).
- Chuẩn hóa cơ sở dữ liệu MySQL, bổ sung đầy đủ các trường nghiệp vụ thực tế của xe ô tô (Hộp số, Nhiên liệu, Địa điểm nhận xe, Đánh giá/Số chuyến, Nhãn ưu đãi).
- Tối ưu hóa UI/UX toàn bộ các luồng: Trang chủ, Thuê xe & Bộ lọc nâng cao, Cẩm nang, Tin tức, Liên hệ, Xác thực người dùng (Auth).
- Dọn dẹp mã nguồn, giải quyết lỗi font chữ tiếng Việt (UTF-8mb4) và chuẩn bị đẩy code lên kho lưu trữ GitHub mới.

---

## 2. CHI TIẾT CÁC CÔNG VIỆC ĐÃ HOÀN THÀNH

### 2.1. Chuẩn hóa & Nâng cấp Cơ sở dữ liệu MySQL (`DbRenalCar`)
1. **Bổ sung các trường dữ liệu thực tế cho bảng `cars`:**
   - `Transmission`: Hộp số (*Tự động*, *Số sàn*).
   - `Fuel`: Loại nhiên liệu (*Điện*, *Xăng*, *Dầu*).
   - `Location`: Địa điểm nhận xe (*Cầu Giấy, Hà Nội*, *Quận 7, TP.HCM*, *Hải Châu, Đà Nẵng*, *Đà Lạt*, *Nha Trang*...).
   - `Trips`: Số chuyến thuê / lượt đánh giá.
   - `Badge`: Nhãn nổi bật (*Xe điện*, *Nổi bật*, *Ưu đãi*).
2. **Làm sạch và đồng bộ dữ liệu mẫu chuẩn (8 xe thực tế):**
   - Xe 1: VinFast VF 5 Plus (5 chỗ | Tự động | Điện | Cầu Giấy, Hà Nội | 650.000đ/ngày | Nhãn: Xe điện).
   - Xe 2: Toyota Vios G 2023 (5 chỗ | Tự động | Xăng | Quận 7, TP.HCM | 850.000đ/ngày | Nhãn: Nổi bật).
   - Xe 3: Hyundai Santa Fe 2024 (7 chỗ | Tự động | Dầu | Hải Châu, Đà Nẵng | 1.650.000đ/ngày | Nhãn: Ưu đãi).
   - Xe 4: Mazda 3 Luxury (5 chỗ | Tự động | Xăng | Đống Đa, Hà Nội | 950.000đ/ngày).
   - Xe 5: Kia Carnival Signature (7 chỗ | Tự động | Dầu | Quận 1, TP.HCM | 2.400.000đ/ngày | Nhãn: Nổi bật).
   - Xe 6: Ford Ranger Wildtrak (5 chỗ | Tự động | Dầu | Phường 1, Đà Lạt | 1.400.000đ/ngày).
   - Xe 7: Honda City RS (5 chỗ | Tự động | Xăng | Lộc Thọ, Nha Trang | 800.000đ/ngày | Nhãn: Ưu đãi).
   - Xe 8: Toyota Fortuner Legender (7 chỗ | Số sàn | Dầu | Tây Hồ, Hà Nội | 1.500.000đ/ngày).
3. **Dọn dẹp bản ghi rác:**
   - Xóa bỏ các dòng danh mục tiếng Anh thừa trong bảng `cartypes`, giữ đúng 5 dòng xe chuẩn: *Xe mini đô thị*, *Sedan 5 chỗ*, *Crossover/SUV 7 chỗ*, *MPV đa dụng*, *Xe bán tải*.
   - Tắt/xóa các xe rác test cũ (`CarID > 8`).
4. **Xử lý triệt để lỗi mã hóa tiếng Việt (Font `?`):**
   - Chuyển đổi toàn bộ các bảng sang `utf8mb4_unicode_ci`.
   - Nạp lại 100% dữ liệu tiếng Việt có dấu chuẩn xác vào CSDL.
5. **Tạo tài khoản khách hàng thử nghiệm:**
   - **Email:** `trankenn04@gmail.com`
   - **Mật khẩu:** `123` (băm MD5 theo thuật toán hệ thống `d0afd43f169f68af6d56cf454d74b167`).
   - **Họ và tên:** `Trần Kenn` (đã lưu cả bảng `customers` và `account`).
6. **Sao lưu CSDL mới nhất:**
   - Đã xuất bản dump hoàn chỉnh chuẩn UTF-8mb4 vào tệp [dbrenalcar.sql](file:///d:/CarRental_TTCN/dbrenalcar.sql).

---

### 2.2. Giao diện & Trải nghiệm Người dùng (UI/UX Pixel-Perfect)

#### A. Header & Điều hướng ([_Layout.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Shared/_Layout.cshtml))
- Logo ReCar hiện đại (Biểu tượng xe xanh bo tròn + chữ `ReCar`).
- Menu giữa dạng viên thuốc nền xám sáng (`Trang chủ`, `Thuê xe`, `Cẩm nang`, `Tin tức`, `Liên hệ`).
- **Luồng trạng thái Auth thông minh:**
  - **Khi chưa đăng nhập:** Hiển thị 2 nút viên thuốc gọn gàng: `Đăng nhập` và nút xanh `Đăng ký` (đã loại bỏ nút "Trở thành chủ xe" theo yêu cầu).
  - **Khi đã đăng nhập:** Tự động hiển thị chuông thông báo có chấm đỏ + Nút Avatar tròn với 2 chữ cái đầu viết tắt (`[TK] Trần Kenn ⌄`) mở Dropdown menu (Tài khoản của tôi, Đơn thuê xe, Trang chủ xe, Quản trị, Đăng xuất).

#### B. Trang chủ ([Home/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Home/Index.cshtml))
- **Hero Search Box nổi:** Đầy đủ 2 tab chuyển đổi `Tự lái` / `Có tài xế`, các trường chọn Địa điểm, Ngày giờ nhận/trả xe và nút tìm kiếm xanh dương `#0284c7`.
- **Thanh 6 lợi ích:** Nền xám nhạt bo góc (Thủ tục nhanh, Bảo hiểm đầy đủ, Hỗ trợ 24/7, Minh bạch, Giao xe tận nơi, Điều khoản rõ ràng).
- **Danh mục dòng xe (Đã sửa lỗi cao thấp):** 5 card tỷ lệ 4:3 đồng đều chiều cao 100%, không bị tràn chữ rớt dòng.
- **Xe nổi bật và ưu đãi:** Lưới 4 xe mẫu đầu tiên từ CSDL với đầy đủ thông số (Số chỗ, Hộp số, Nhiên liệu, Địa điểm, Đánh giá, Nút Xem chi tiết & Thuê ngay).
- **Thuê xe chỉ với 3 bước:** 01 Chọn xe & thời gian, 02 Đặt cọc 30% trực tuyến, 03 Nhận xe & ký hợp đồng.
- **Cẩm nang và tin tức:** Lưới 3 bài viết nổi bật chuẩn giao diện ReCar (`aspect-ratio: 16/9`, ngày đăng, tóm tắt, liên kết đọc thêm).
- **Đã gỡ bỏ:** Khối banner lớn quảng bá trở thành chủ xe theo yêu cầu người dùng.

#### C. Trang Thuê xe & Bộ lọc nâng cao ([Product/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Product/Index.cshtml))
- **Thanh lọc khoảng giá 2 đầu kéo (Min - Max):**
  - Thanh trượt kép hiện đại, thẩm mỹ, cho phép kéo từ mức giá bắt đầu (`Từ: 500k`) đến mức giá kết thúc (`Đến: 5tr/ngày`).
  - Hiển thị badge tóm tắt động và nhãn định dạng tiền tệ Việt Nam (`VNĐ`).
- **Bộ lọc Chip Pills đa tiêu chí:**
  - Hãng xe: VinFast, Toyota, Mazda, Hyundai, Kia, Honda, Ford.
  - Dòng xe: Tất cả, Mini, Sedan, SUV 7 chỗ, MPV, Bán tải.
  - Số chỗ: Tất cả, 5 chỗ, 7 chỗ.
  - Nhiên liệu: Tất cả, Xăng, Dầu.
- **Lọc động bằng AJAX:** Lọc xe không cần tải lại trang, cập nhật số lượng xe phù hợp theo thời gian thực.
- **Đọc trực tiếp từ CSDL:** Thẻ xe [Product/Default.cshtml](file:///d:/CarRental_TTCN/carrental/Views/Shared/Components/Product/Default.cshtml) hiển thị trực tiếp dữ liệu thật từ bảng `cars`.

#### D. Các trang phụ trợ
- **Cẩm nang ([Blog/Index.cshtml](file:///d:/CarRental_TTCN/carrental/Views/Blog/Index.cshtml)):** Xóa sạch template FloSun cũ, đồng bộ banner và lưới bài viết ReCar.
- **Tin tức ([News/Index.cshtml](file:///d:/CarRental_TTCN/carrental/Views/News/Index.cshtml)):** Banner tiêu đề và lưới tin tức công nghệ xe hơi mới nhất.
- **Liên hệ ([Contact/Index.cshtml](file:///d:/CarRental_TTCN/carrental/Views/Contact/Index.cshtml)):** Bố cục 2 cột chuyên nghiệp: 3 hộp thông tin liên hệ (Hotline 24/7 `1900 6868`, Email, Địa chỉ văn phòng) + Form gửi tin nhắn hỗ trợ SweetAlert2 (cho phép khách vãng lai gửi phản hồi).

---

## 3. DỌN DẸP MÃ NGUỒN VÀ TÀI NGUYÊN THỪA
- Đã xóa các tệp truy vấn SQL tạm: `sync_recar_data.sql`, `clean_and_sync_recar.sql`, `update_db_recar.sql`, `fix_utf8.sql`.
- Đã thêm cấu hình ignore `.tmb/` (các tệp ảnh thumbnail tạm sinh ra bởi trình quản lý file) vào [.gitignore](file:///d:/CarRental_TTCN/.gitignore) để giữ kho lưu trữ Git luôn sạch sẽ.

---

## 4. BƯỚC TIẾP THEO
1. Cấu hình Git remote trỏ sang repository mới: `https://github.com/KuaaPoH/ReCar.git`.
2. Kiểm tra tài khoản và commit toàn bộ thay đổi lên branch chính.
3. Tiếp tục hoàn thiện các chức năng nâng cao theo đề cương Đồ án tốt nghiệp:
   - Chi tiết xe ([Product/Details.cshtml](file:///d:/CarRental_TTCN/carrental/Views/Product/Details.cshtml)): widget tính tiền cọc 30% và lịch thuê.
   - Hợp đồng điện tử định dạng chuẩn A4 có chữ ký số.
   - Tích hợp cổng thanh toán trực tuyến VNPay Sandbox.
