# BÁO CÁO TIẾN TRÌNH CÔNG VIỆC DỰ ÁN RECAR (ĐỒ ÁN TỐT NGHIỆP)
**Thời gian thực hiện:** Ngày 13 - 17 tháng 10 năm 2026  
**Dự án:** ReCar – Nền tảng chia sẻ & cho thuê xe ô tô trực tuyến (Car-Sharing Platform)  
**Nền tảng công nghệ:** ASP.NET Core 9 (MVC), Entity Framework Core, MySQL (`DbRenalCar`), Bootstrap 5, Vanilla CSS Design System (Pixel-Perfect theo chuẩn mẫu Lovable `pixel-perfect-render-09837`).

---

## 1. MỤC TIÊU & TỔNG QUAN CÔNG VIỆC
- Nâng cấp toàn diện hệ thống từ giao diện cũ (FloSun) sang hệ thống thuê xe ô tô hiện đại **ReCar**, đạt chuẩn thẩm mỹ cao cấp **Pixel-Perfect 100%** theo mẫu template Lovable (`pixel-perfect-render-09837`).
- Chuẩn hóa toàn bộ cơ sở dữ liệu MySQL, bổ sung đầy đủ các trường nghiệp vụ thực tế (Hộp số, Nhiên liệu, Địa điểm nhận xe, Đánh giá/Số chuyến, Nhãn ưu đãi, Trạng thái xe, Bảng Chat lưu trữ).
- Thiết lập hệ thống tài khoản Chủ xe thật cho toàn bộ 8 xe mẫu hiện có, tích hợp trang quản lý chủ xe chuyên biệt (`/CarOwner`).
- Tích hợp hệ thống Chat trực tuyến 2 chiều giữa Khách hàng và Chủ xe có lưu trữ cơ sở dữ liệu (`ChatMessages`) và cơ chế phản hồi tự động thông minh.
- Hoàn thiện luồng đặt xe, hợp đồng điện tử 3 bên, xử lý thanh toán VNPay Sandbox & chế độ giả lập kiểm thử (Bypass Test Mode).
- Chuẩn bị kế hoạch phát triển tương lai và danh sách gợi ý đột phá để hoàn thiện xuất sắc đồ án tốt nghiệp.

---

## 2. CHI TIẾT CÁC CÔNG VIỆC ĐÃ HOÀN THÀNH

### 2.1. Chuẩn hóa & Nâng cấp Cơ sở dữ liệu MySQL (`DbRenalCar`)
1. **Bổ sung các trường dữ liệu thực tế cho bảng `cars`:**
   - `Transmission`: Hộp số (*Tự động*, *Số sàn*).
   - `Fuel`: Loại nhiên liệu (*Điện*, *Xăng*, *Dầu*).
   - `Location`: Địa điểm bàn giao xe thực tế (*Cầu Giấy, Hà Nội*, *Quận 7, TP.HCM*, *Hải Châu, Đà Nẵng*, *Đà Lạt*, *Nha Trang*...).
   - `Trips`: Số chuyến thuê thành công / lượt đánh giá.
   - `Badge`: Nhãn tiếp thị nổi bật (*Xe điện*, *Nổi bật*, *Ưu đãi*).
   - `CarStatus`: Trạng thái khả dụng của xe (*Sẵn sàng*, *Đang bận*, *Bảo trì*).
2. **Làm sạch và đồng bộ dữ liệu mẫu chuẩn (8 xe thực tế):**
   - Xe 1: **VinFast VF 5 Plus** (5 chỗ | Tự động | Điện | Cầu Giấy, Hà Nội | 650.000đ/ngày | Nhãn: Xe điện | Chủ xe: Trần Quốc Bảo).
   - Xe 2: **Toyota Vios G 2023** (5 chỗ | Tự động | Xăng | Quận 7, TP.HCM | 850.000đ/ngày | Nhãn: Nổi bật | Chủ xe: Nguyễn Minh Anh).
   - Xe 3: **Hyundai Santa Fe 2024** (7 chỗ | Tự động | Dầu | Hải Châu, Đà Nẵng | 1.650.000đ/ngày | Nhãn: Ưu đãi | Chủ xe: Phạm Hoàng Nam).
   - Xe 4: **Mazda 3 Luxury** (5 chỗ | Tự động | Xăng | Đống Đa, Hà Nội | 950.000đ/ngày | Chủ xe: Lê Thu Hà).
   - Xe 5: **Kia Carnival Signature** (7 chỗ | Tự động | Dầu | Quận 1, TP.HCM | 2.400.000đ/ngày | Nhãn: Nổi bật | Chủ xe: Vũ Đình Dũng).
   - Xe 6: **Ford Ranger Wildtrak** (5 chỗ | Tự động | Dầu | Phường 1, Đà Lạt | 1.400.000đ/ngày | Chủ xe: Phạm Hoàng Nam).
   - Xe 7: **Honda City RS** (5 chỗ | Tự động | Xăng | Lộc Thọ, Nha Trang | 800.000đ/ngày | Nhãn: Ưu đãi | Chủ xe: Trần Quốc Bảo).
   - Xe 8: **Toyota Fortuner Legender** (7 chỗ | Số sàn | Dầu | Tây Hồ, Hà Nội | 1.500.000đ/ngày | Chủ xe: Nguyễn Minh Anh).
3. **Dọn dẹp bản ghi rác & chuẩn hóa cấu trúc:**
   - Xóa bỏ các danh mục tiếng Anh thừa, chuẩn hóa 5 phân khúc xe: *Xe mini đô thị*, *Sedan 5 chỗ*, *Crossover/SUV 7 chỗ*, *MPV đa dụng*, *Xe bán tải*.
   - Khắc phục triệt để lỗi font chữ tiếng Việt (chuyển đổi 100% sang `utf8mb4_unicode_ci`).
   - Xuất bản tệp sao lưu dữ liệu toàn diện duy nhất tại [dbrenalcar.sql](file:///d:/CarRental_TTCN/dbrenalcar.sql).

---

### 2.2. Trạng thái xe (`CarStatus`) & Hiệu ứng Thị giác Thông minh
- **Trực quan hóa trạng thái xe:**
  - `Sẵn sàng`: Hiển thị ảnh sáng rõ nét tự nhiên, nút thuê xe và đặt cọc hoạt động bình thường.
  - `Đang bận` (*Xe đang có khách thuê*): Áp dụng hiệu ứng phủ bóng tối mờ (grayscale + brightness filter), đính kèm huy hiệu màu cam hổ phách `Đang có khách`, vô hiệu hóa nút đặt cọc và đưa ra khuyến nghị chọn ngày khác hoặc liên hệ chủ xe.
  - `Bảo trì` (*Xe đang kiểm tra định kỳ*): Phủ bóng mờ, đính kèm huy hiệu cảnh báo màu đỏ/xám `Đang bảo trì`, ngăn chặn phát sinh đơn thuê mới.
- **Đồng bộ hóa 2 chiều:** Chủ xe có thể chủ động chuyển trạng thái xe từ trang quản lý xe `/CarOwner/Cars/Edit` để phản ánh đúng lịch trình cá nhân.

---

### 2.3. Hệ thống Chat Trực tuyến Khách hàng - Chủ xe Lưu trữ CSDL
1. **Thiết kế Cơ sở dữ liệu Bảng `ChatMessages`:**
   - `MessageId` (Primary Key, Auto Increment).
   - `CarId` (Liên kết xe đang trao đổi).
   - `SenderId`, `SenderName`, `SenderRole` (*Customer* hoặc *CarOwner*).
   - `ReceiverId`, `ReceiverName`.
   - `MessageText` (Nội dung tin nhắn).
   - `SentAt` (Thời gian gửi).
   - `IsRead` (Trạng thái đã đọc).
2. **API Backend chuyên nghiệp ([ChatController.cs](file:///d:/CarRental_TTCN/CarRental/Controllers/ChatController.cs)):**
   - `POST /Chat/SendMessage`: Tiếp nhận tin nhắn từ giao diện khách, lưu vào CSDL, tự động kích hoạt bot chủ xe mô phỏng phản hồi tức thì thông minh dựa trên thông tin xe và lưu trữ câu trả lời vào CSDL.
   - `GET /Chat/GetMessages`: Truy vấn toàn bộ lịch sử trao đổi giữa khách và chủ xe của từng chiếc xe.
   - `GET /Chat/GetRecentConversations`: Lấy danh sách các cuộc trao đổi gần nhất để đưa lên Dropdown Header.
3. **Giao diện Chat Khách hàng ([Views/Shared/_Layout.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Shared/_Layout.cshtml)):**
   - Cửa sổ chat nổi góc phải màn hình với phong cách kính mờ hiện đại, hỗ trợ thu nhỏ/mở rộng mượt mà.
   - Menu tin nhắn dạng chuông/hộp thư trên thanh Header, hiển thị các đoạn chat gần nhất kèm thời gian.
   - Nút "Chat với chủ xe" nổi bật tại trang chi tiết xe ([Product/Details.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Product/Details.cshtml)), tự động điền sẵn tên xe và kết nối trực tiếp đến chủ xe phụ trách.

---

### 2.4. Phân hệ Quản lý Chủ xe (`Areas/CarOwner`) Chuẩn Pixel-Perfect (`pixel-perfect-render-09837`)
Chuyển đổi toàn diện từ giao diện màu tím cũ sang thiết kế **DashboardShell** hiện đại với bảng màu Navy cao cấp (`#0f172a`), điểm nhấn xanh ngọc (`#0284c7`), font chữ Plus Jakarta Sans:
1. **Cấu trúc Shell & Sidebar ([CarOwner/Views/Shared/_Layout.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Shared/_Layout.cshtml)):**
   - Sidebar Navy cố định, logo ReCar sang trọng kèm nhãn phụ đề `Phân hệ Chủ xe`.
   - Menu điều hướng dạng viên thuốc (Pill Navigation): *Tổng quan*, *Quản lý xe*, *Thêm xe mới*, *Đơn thuê xe*, *Đăng xuất* và nút quay về *Sàn ReCar*.
   - Topbar hiển thị tên chủ xe đang đăng nhập kèm chữ cái đại diện Avatar tròn và huy hiệu `Đối tác ReCar`.
2. **Bảng điều khiển Tổng quan ([CarOwner/Views/Home/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Home/Index.cshtml)):**
   - **4 Stat Cards:** Tổng doanh thu thực nhận (+12% so với tháng trước), Số xe đang sở hữu trên sàn, Tổng đơn thuê xe đã tiếp nhận, Điểm uy tín từ khách hàng (4.9 / 5.0 ★).
   - **Biểu đồ Cột Doanh thu 6 tháng (`Bars` component):** Mô phỏng trực quan biểu đồ doanh thu bằng Vanilla CSS Gradients theo phong cách Lovable.
   - **Bảng danh sách xe đang quản lý & Bảng đơn thuê xe gần đây:** Trình bày tinh gọn với đầy đủ biển số xe, ngày đặt và `StatusBadge` chuẩn màu.
3. **Quản lý Danh sách Xe ([CarOwner/Views/Cars/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Index.cshtml)):**
   - Bảng quản lý xe trực quan: Ảnh xe bo tròn, biển số font monospace nổi bật, khu vực bàn giao, thông số động cơ/hộp số, giá thuê/ngày, badge trạng thái (*Sẵn sàng*, *Đang có khách*, *Bảo trì*).
   - Cụm nút thao tác nhanh: Sửa xe, Xem trực tiếp bài đăng trên Sàn ReCar, Tạm dừng hoạt động.
4. **Biểu mẫu Thêm xe & Chỉnh sửa xe ([Cars/Create.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Create.cshtml) & [Cars/Edit.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Edit.cshtml)):**
   - Form 2 cột bo góc lớn `rounded-xl`, trường nhập liệu `recar-input` với hiệu ứng focus viền xanh sang trọng.
   - Hỗ trợ cập nhật đầy đủ: Tên xe, Hãng, Dòng xe, Biển số, Giá thuê, Trạng thái hoạt động, Nhiên liệu, Hộp số, Địa chỉ bàn giao, Link ảnh và Mô tả xe.
5. **Quản lý Đơn đặt xe ([CarOwner/Views/Orders/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Index.cshtml) & [Details.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Details.cshtml)):**
   - Danh sách đơn hàng với bộ lọc trạng thái thông minh.
   - Trang chi tiết đơn phân rã tài chính minh bạch: Tiền thuê, Tiền cọc 30% giữ trên sàn, Số tiền 70% còn lại khách sẽ thanh toán khi nhận xe.
   - Tích hợp nút Xác nhận bàn giao, Từ chối, và Gọi điện thoại trực tiếp cho người thuê.

---

### 2.5. Tinh chỉnh Luồng Khách hàng & Cổng Thanh toán
1. **Trang chủ & Bộ lọc xe ([Product/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Product/Index.cshtml)):**
   - Thanh lọc khoảng giá kép 2 đầu kéo (Slider Min - Max từ 500.000đ đến 5.000.000đ/ngày).
   - Hệ thống Chip Pills đa tiêu chí: Hãng xe, Dòng xe, Số chỗ, Nhiên liệu, Hình thức thuê (Tự lái / Có tài xế).
   - Đã gỡ bỏ hoàn toàn nút và banner "Trở thành chủ xe" trên trang khách hàng theo yêu cầu.
2. **Trang chi tiết xe ([Product/Details.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Product/Details.cshtml)):**
   - Khối thông tin Chủ xe chuẩn mực: Tên chủ xe thật, Huy hiệu đối tác 5 sao, Tỷ lệ phản hồi nhanh, Số điện thoại hỗ trợ và Nút kích hoạt cửa sổ chat.
   - Bóc tách minh bạch gói thuê: *Tự lái* (yêu cầu GPLX) hoặc *Có tài xế* (+500.000đ/ngày do chủ xe trực tiếp lái phục vụ).
3. **Cổng thanh toán & Chế độ Giả lập (Bypass Mode):**
   - Khắc phục triệt để lỗi mã hóa IP (`vnp_IpAddr`), map đúng mã đơn hàng `vnp_TxnRef` và xử lý callback tự động kích hoạt `StatusId = 3` (*Đã thanh toán cọc*).
   - Tích hợp cờ `"BypassPayment": true` trong [appsettings.json](file:///d:/CarRental_TTCN/CarRental/appsettings.json): Cho phép xác nhận đặt cọc thành công ngay lập tức bằng SweetAlert2 phục vụ quá trình chấm điểm và demo đồ án mượt mà mà không lo cổng sandbox hết hạn.

---

### 2.6. Nâng cấp Toàn diện Phân hệ Quản trị Toàn sàn (`Areas/Admin`) Chuẩn SaaS B2B Pixel-Perfect
Hoàn thành chuyển đổi 100% từ giao diện Mazer cũ sang hệ thống quản trị hiện đại theo đúng bản thiết kế Lovable ([giao dien admin](file:///d:/CarRental_TTCN/giao%20dien%20admin)):
1. **Khung Layout chuẩn mực ([Areas/Admin/Views/Shared/_Layout.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Shared/_Layout.cshtml)):**
   - **Sidebar Deep Navy (`#0f172a`):** Logo ReCar trắng, nhãn `ADMIN PORTAL`, phân quyền `Quản trị viên`.
   - **8 Mục Menu điều hướng chính:** *Tổng quan*, *Phê duyệt xe* (tự động gắn badge đếm số lượng xe chờ duyệt lấy trực tiếp từ CSDL), *Quản lý đội xe*, *Đơn thuê xe*, *Tài chính & đối soát*, *Người dùng & đối tác*, *Bài viết & tin tức*, *Cấu hình hệ thống*.
   - **Topbar thông minh:** Breadcrumb tự động, thanh tìm kiếm toàn sàn, chuông thông báo (nhảy popup xe chờ duyệt), Avatar Super Admin kèm dropdown hồ sơ và đăng xuất.
2. **Bảng điều khiển Tổng quan ([Areas/Admin/Views/Home/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Home/Index.cshtml)):**
   - **4 Thẻ chỉ số KPI:** Tổng giá trị giao dịch (GMV +8.4%), Hoa hồng sàn thực thu (10%), Xe đang hoạt động (tỷ lệ lấp đầy %), Xe chờ phê duyệt (nút dẫn sang duyệt xe).
   - **Biểu đồ Cột 6 tháng (Chart.js):** Chuyển đổi linh hoạt giữa xem Doanh thu (GMV vs Hoa hồng) và Lượt thuê xe thành công.
   - **Biểu đồ Tròn cơ cấu đội xe:** Phân bổ trực quan số lượng *Sẵn sàng*, *Đang có khách*, *Bảo dưỡng*, *Chờ duyệt*.
   - **Bảng dữ liệu tóm tắt:** 6 đơn thuê xe mới nhất (mã monospace `#RC-...`, ảnh xe, tiền cọc, status) và Top 5 chủ xe có doanh thu cao.
3. **Module Phê duyệt xe mới ([Cars/Pending.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Pending.cshtml) & [CarsController.cs](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarsController.cs)):**
   - Nghiệp vụ thực tế: Khi chủ xe thêm xe mới từ `/CarOwner/Cars/Create`, xe tự động mang trạng thái `Chờ duyệt` và **không xuất hiện ngoài trang chủ**.
   - Admin kiểm tra hồ sơ, thông số xe, ảnh xe, giấy tờ đăng kiểm.
   - Hỗ trợ chuyển đổi chế độ xem **Dạng thẻ (Grid)** và **Dạng bảng (Table)** kèm bộ lọc đa tiêu chí (Hãng xe, Thành phố, Chủ xe).
   - Thao tác: **[Phê duyệt xe]** (cập nhật CSDL sang `Sẵn sàng` để xuất hiện trên sàn) hoặc **[Từ chối duyệt]** (nhập lý do từ chối gửi thông báo cho chủ xe).
4. **Quản lý Đội xe toàn sàn ([Cars/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Index.cshtml)):**
   - Danh sách toàn bộ xe trên hệ thống, biển số font Monospace, link xem bài đăng công khai, thao tác chuyển nhanh trạng thái *Sẵn sàng / Bảo trì / Tạm ngừng*.
5. **Quản lý Đơn thuê xe ([Orders/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Orders/Index.cshtml) & [Edit.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Orders/Edit.cshtml)):**
   - Bảng đơn thuê bóc tách minh bạch: Tổng tiền thuê, Tiền cọc 30% giữ qua sàn, Tiền còn lại 70%, StatusBadge chuẩn màu.
6. **Quản lý Tài chính & Đối soát ([FinanceController.cs](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/FinanceController.cs) & [Finance/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Finance/Index.cshtml)):**
   - Bóc tách dòng tiền: Tổng GMV, Tiền cọc sàn giữ hộ (30%), Phí hoa hồng sàn thực thu (10%), Tiền thanh toán cho chủ xe (90%).
   - Bảng lệnh đối soát từng đối tác chủ xe kèm nút **[Xác nhận đã chuyển khoản quyết toán]** có lưu dấu vết kiểm toán.
7. **Người dùng, Bài viết & Cấu hình sàn:**
   - Hoàn thiện trang [UserManagement](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/UserManagement/Index.cshtml), [Blogs](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Blogs/Index.cshtml) và [Settings](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Settings/Index.cshtml).

---

## 3. DANH SÁCH TÀI KHOẢN MẪU PHỤC VỤ KIỂM THỬ HỆ THỐNG

Tất cả tài khoản dưới đây đã được đồng bộ chuẩn hóa vào CSDL MySQL, mật khẩu mặc định đều là `123`:

| STT | Họ và Tên | Vai trò | Email đăng nhập | Tên đăng nhập | Mật khẩu | Xe phụ trách / Ghi chú |
|:---:|:---|:---|:---|:---|:---:|:---|
| **1** | **Trần Kenn** | Khách thuê xe | `trankenn04@gmail.com` | `trankenn04` | `123` | Tài khoản khách hàng đầy đủ lịch sử đơn thuê và chat |
| **2** | **Trần Quốc Bảo** | Chủ xe đối tác | `tranbao.carowner@gmail.com` | `tranbaocar` | `123` | Quản lý: *VinFast VF 5 Plus* & *Honda City RS* |
| **3** | **Nguyễn Minh Anh**| Chủ xe đối tác | `minhanh.carrental@gmail.com` | `minhanh_auto` | `123` | Quản lý: *Toyota Vios G 2023* & *Toyota Fortuner Legender* |
| **4** | **Phạm Hoàng Nam** | Chủ xe đối tác | `hoangnam.suv@gmail.com` | `hoangnam_suv` | `123` | Quản lý: *Hyundai Santa Fe 2024* & *Ford Ranger Wildtrak* |
| **5** | **Lê Thu Hà** | Chủ xe đối tác | `thuha.luxurycars@gmail.com` | `thuha_luxury` | `123` | Quản lý: *Mazda 3 Luxury* |
| **6** | **Vũ Đình Dũng** | Chủ xe đối tác | `dinhdung.rentcar@gmail.com` | `dinhdung_rent` | `123` | Quản lý: *Kia Carnival Signature* |
| **7** | **Quản Trị Viên** | Admin hệ thống | `admin@gmail.com` | `admin` | `123` | Quản trị toàn sàn, duyệt xe, phân quyền hệ thống |

> **Ghi chú đăng nhập:** Chủ xe có thể đăng nhập tại cả `/Login` hoặc `/CarOwner/Login`. Sau khi đăng nhập, hệ thống sẽ tự động hiển thị mục **"Trang chủ xe"** trên Avatar Menu để truy cập trực tiếp vào `/CarOwner/Home/Index`.

---

## 4. KẾ HOẠCH & NHỮNG CÔNG VIỆC CẦN LÀM TRONG TƯƠNG LAI (ROADMAP)

Dưới đây là lộ trình các hạng mục trọng tâm nhằm hoàn thiện sản phẩm ở mức độ thương mại hóa cao:

### Giai đoạn 1: Nâng cấp Phân hệ Quản trị Viên (`Areas/Admin`) — [x] ĐÃ HOÀN THÀNH (19 - 20/10/2026)
1. **Áp dụng Design System theo mẫu `admin.tsx` / `AdminLayout.tsx` của Lovable:**
   - Hoàn thành Layout Deep Navy (`#0f172a`), 8 Menu quản trị, topbar tìm kiếm, chuông thông báo và avatar quản trị viên.
   - Thống kê 4 KPI toàn sàn (GMV, hoa hồng 10%, xe hoạt động, xe chờ duyệt), biểu đồ BarChart 6 tháng (doanh thu/chuyến đi), biểu đồ Donut cơ cấu đội xe.
2. **Chức năng Kiểm duyệt xe mới (Car Moderation):**
   - Xe do chủ xe đăng ký mới tự động mang trạng thái `Chờ duyệt` và không xuất hiện công khai.
   - Màn hình duyệt xe với bộ lọc đa tiêu chí, chuyển đổi dạng thẻ/bảng, luồng Duyệt và Từ chối kèm lý do gửi cho chủ xe.
3. **Quản lý Dòng tiền & Hoa hồng sàn (Commission & Payouts):**
   - Tính toán hoa hồng sàn 10%, tiền cọc 30% giữ hộ và tiền quyết toán cho chủ xe 90%.
   - Bảng lệnh đối soát từng chủ xe kèm nút [Xác nhận đã chuyển khoản quyết toán] có lưu vết kiểm toán.
4. **Chuẩn hóa các phân hệ Đơn thuê xe, Người dùng, Bài viết và Cấu hình hệ thống.**

---

### Giai đoạn 2: Nâng cấp Công nghệ Giao tiếp Thời gian thực (Real-time Communication với SignalR) — [x] ĐÃ HOÀN THÀNH (21/10/2026)
1. **Tích hợp ASP.NET Core SignalR cho Hệ thống Chat ([ReCarHub.cs](file:///d:/CarRental_TTCN/CarRental/Hubs/ReCarHub.cs) & [recar-realtime.js](file:///d:/CarRental_TTCN/CarRental/wwwroot/assets/js/recar-realtime.js)):**
   - Đã nâng cấp toàn diện Chat giữa Khách hàng và Chủ xe lên WebSockets thời gian thực (Real-time Two-Way Chat qua `/recarHub`). Tin nhắn hiển thị tức thì không có độ trễ, không cần reload trang.
   - Bổ sung hiệu ứng âm thanh thông báo du dương bằng Web Audio API (chuẩn âm sắc piano ping, hoạt động 100% offline).
   - Tích hợp chỉ báo đối phương đang gõ phím (`Typing Indicator`: *"Chủ xe đang soạn tin... 💬"* với spinner nhịp đập) có cơ chế debounce tự động tắt sau 2 giây.
2. **Hệ thống Thông báo Đẩy Thời gian thực (Live Push Notifications):**
   - **Khi có khách đặt xe mới ([CartController.cs](file:///d:/CarRental_TTCN/CarRental/Controllers/CartController.cs)):** Chủ xe và Ban Quản trị nhận được âm thanh chuông chime và Toast thông báo nổi góc phải màn hình, đồng thời chấm đỏ thông báo trên Topbar nhảy số tức thời.
   - **Khi Chủ xe thêm xe mới ([CarOwner/CarsController.cs](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/CarsController.cs)):** Ban Quản trị nhận ngay popup thông báo *"Hồ sơ xe mới chờ duyệt"* và Badge số lượng xe chờ duyệt trên thanh Sidebar Admin (`#pendingCarsBadge`) tự động nhảy số theo thời gian thực mà không cần F5.
   - **Khi Admin duyệt hoặc từ chối xe ([Admin/CarsController.cs](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarsController.cs)):** Chủ xe nhận được popup thông báo kết quả phê duyệt xe hoặc lý do từ chối kèm nút dẫn trực tiếp đến trang chỉnh sửa hồ sơ.
3. **Thành phần Toast Thông báo SaaS Hiện Đại:**
   - Toast dạng card nổi góc phải màn hình bo góc 14px, đổ bóng mềm, viền màu phân biệt theo sự kiện (Xanh lá: Đơn mới/Duyệt xe, Vàng: Chờ duyệt, Xanh ngọc: Chat, Đỏ: Từ chối), có progress bar tự động đếm ngược và thu gọn sau 6 giây.

---

### Giai đoạn 3: Tích hợp Cổng thanh toán Mở rộng & Ký kết Hợp đồng số (E-Signature)
1. **Kết nối Đa cổng thanh toán chính thức:**
   - Tích hợp thêm MoMo QR, ZaloPay và thẻ tín dụng quốc tế (Visa/Mastercard qua cổng PayOS / Stripe).
   - Tự động hoàn tiền cọc (Refund API) khi khách hủy chuyến hợp lệ theo đúng quy chế sàn.
2. **Ký số Hợp đồng điện tử (E-Signature):**
   - Tích hợp khung vẽ chữ ký điện tử trực tiếp trên màn hình điện thoại hoặc máy tính trước khi bấm "Nhận xe".
   - Tự động xuất file PDF hợp đồng thuê xe có mã Hash xác thực và chữ ký 3 bên gửi về email của khách và chủ xe.

---

### Giai đoạn 4: Bản đồ Tương tác Tìm Xe Gần Bạn (Station-Based Nearby Car Map View) — [x] ĐÃ HOÀN THÀNH & KIỂM THỬ XUẤT SẮC (21/10/2026)
1. **API Tính toán Khoảng cách GPS Thực tế ([ProductController.cs](file:///d:/CarRental_TTCN/CarRental/Controllers/ProductController.cs)):**
   - Endpoint `GET /Product/GetNearbyCars?userLat=...&userLng=...&radiusKm=...`: Ánh xạ tọa độ các bãi xe/điểm nhận xe thực tế (Hà Nội, TP.HCM, Đà Nẵng, Nha Trang, Đà Lạt, và đặc biệt cụm xe mới tại **TP. Vinh, Nghệ An**: Quảng trường Hồ Chí Minh, Lê Mao, Trường Thi, Hưng Dũng).
   - Thêm 4 mẫu xe biển số 37 Nghệ An (VinFast VF 8 Plus, Hyundai Tucson Turbo, Toyota Corolla Cross, Mazda CX-5 Premium).
   - Áp dụng công thức hình học cầu **Haversine** tính chính xác khoảng cách đường chim bay từ vị trí người dùng đến từng chiếc xe (làm tròn đến 0.1 km).
   - Tự động lọc theo bán kính (10km, 25km, 50km, Toàn quốc) và sắp xếp xe theo thứ tự từ gần nhất đến xa nhất.
2. **Giao diện Modal Bản đồ Split-View Cao cấp ([Product/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Product/Index.cshtml)):**
   - **Nút "Bản đồ xe gần bạn"** trên thanh toolbar danh sách xe và liên kết trực tiếp trên Navbar chính.
   - **Tích hợp bản đồ Leaflet.js** kết hợp Google Maps Street Tiles (nhanh, 100% tiếng Việt, hiển thị rõ ràng đường sá).
   - **Marker vị trí người dùng (User GPS):** Chấm xanh dương với hiệu ứng sóng radar nhịp đập (`recar-user-pulse`) kèm vòng tròn nét đứt thể hiện bán kính tìm kiếm.
   - **Marker xe dạng Viên thuốc Giá tiền (Pill Badges phong cách Airbnb / Mioto):** Hiển thị rõ giá thuê trọn gói trên từng điểm ghim (`650.000đ`, `1.200.000đ`...), hover vào đổi sang màu Navy sang trọng.
   - **Tương tác 2 chiều mượt mà (Bi-directional Sync):** Hover vào thẻ xe danh sách bên trái ➜ Bản đồ tự động xoay tâm (flyTo) và nảy marker; Bấm vào ghim trên bản đồ ➜ Hiện popup ảnh xe thật, tên xe, địa điểm, khoảng cách và nút "Xem xe / Thuê ngay".
   - **Định vị thông minh đa tầng & Kiểm thử di động:** Tự động bắt GPS vệ tinh chính xác cao trên điện thoại thông minh qua HTTPS tunnel, hỗ trợ click trực tiếp lên bản đồ để đổi vị trí, và dropdown chọn nhanh các phường tại TP. Vinh khi thuyết trình đồ án trên máy tính.

---

### Giai đoạn 5: Tự động Quét & Xác thực Bằng lái xe bằng AI (AI OCR License Verification)
- Khách hàng tải ảnh CCCD và Giấy phép lái xe (GPLX B1/B2) lên hệ thống.
- Sử dụng mô hình OCR / AI Vision tự động trích xuất: Họ tên, Số bằng lái, Ngày hết hạn, Hạng xe.
- Tự động đối chiếu thông tin và cấp huy hiệu `Đã xác thực danh tính ✓`, tăng độ uy tín và bảo vệ an toàn cho chủ xe.

---

## 5. ĐỀ XUẤT & GỢI Ý ĐỘT PHÁ NÂNG CAO TRẢI NGHIỆM NGƯỜI DÙNG (UX/UI ENHANCEMENTS)

Nhằm giúp đồ án tạo được ấn tượng mạnh mẽ với hội đồng phản biện và mang lại trải nghiệm tiện nghi vượt trội như các ứng dụng công nghệ hàng đầu (Mioto, Zoomcar, Turo, Airbnb):

### 1. Bản đồ Tương tác Tìm xe Xung quanh (Interactive Map View - Leaflet/Mapbox)
- **Ý tưởng:** Bên cạnh giao diện lưới danh sách xe truyền thống, bổ sung nút chuyển đổi `Dạng danh sách` ⟷ `Bản đồ`.
- **Trải nghiệm:** Khách hàng thấy vị trí các xe đang sẵn sàng hiển thị bằng pin giá tiền trên bản đồ thành phố (Hà Nội, TP.HCM, Đà Nẵng). Khi rê chuột vào pin sẽ hiện xem trước ảnh xe, bấm vào pin sẽ cuộn đến xe tương ứng hoặc xem chi tiết.
- **Giá trị:** Cực kỳ trực quan, giúp người thuê nhanh chóng tìm được xe ở gần nhà mình nhất.

### 2. Trình Xem Nội thất Xe 360° (360° Interior Panorama Viewer)
- **Ý tưởng:** Tích hợp thư viện ảnh toàn cảnh 360 độ (như Pannellum JS) vào trang chi tiết xe.
- **Trải nghiệm:** Người dùng có thể dùng chuột kéo xoay 360 độ để ngắm nhìn toàn bộ khoang lái, vô lăng, ghế sau của chiếc xe trước khi quyết định thuê.
- **Giá trị:** Tạo điểm nhấn công nghệ đồ họa vượt trội, gây ấn tượng thị giác mạnh mẽ trong buổi thuyết trình.

### 3. Tự động Quét & Xác thực Bằng lái xe bằng AI/OCR (AI License Verification)
- **Ý tưởng:** Khi khách hàng tải ảnh GPLX (Bằng lái xe B1/B2) và CCCD lên hệ thống, sử dụng mô hình OCR (hoặc Gemini Vision / Tesseract) để tự động đọc: Họ tên, Số bằng lái, Ngày hết hạn, Hạng xe được phép điều khiển.
- **Trải nghiệm:** Khách hàng không phải gõ tay thông tin, hệ thống tự động điền form và cấp huy hiệu `Đã xác thực danh tính ✓`.
- **Giá trị:** Giúp tăng tính bảo mật, chuyên nghiệp và giảm thiểu rủi ro pháp lý cho các chủ xe đối tác.

### 4. Lịch Giá Thông minh cho Chủ xe (Dynamic Smart Calendar)
- **Ý tưởng:** Cho phép chủ xe tùy chỉnh giá thuê linh hoạt theo từng ngày trong lịch:
  - Tự động tăng giá 20 - 30% vào các dịp Lễ, Tết hoặc cuối tuần (Thứ 7, Chủ Nhật).
  - Khuyến mãi giảm giá 10% nếu khách thuê dài ngày (từ 3 ngày trở lên).
- **Trải nghiệm:** Khách chọn ngày trên DatePicker sẽ thấy tổng tiền được tính toán minh bạch dựa trên từng mức giá ngày cụ thể.

### 5. Biên bản Bàn giao Xe Điện tử có Ảnh chụp Hiện trạng (Digital Check-in Inspection)
- **Ý tưởng:** Trước khi bàn giao xe, ứng dụng cung cấp giao diện chụp nhanh 4 góc xe, mức xăng/pin hiện tại và số km trên đồng hồ công tơ mét (ODO).
- **Trải nghiệm:** Hai bên khách và chủ xe cùng bấm xác nhận hiện trạng xe lúc nhận. Khi trả xe, đối chiếu lại để tránh tranh chấp xước xát xe.
- **Giá trị:** Giải quyết bài toán nhức nhối nhất trong nghiệp vụ cho thuê xe ô tô thực tế.

### 6. Chương trình Tích điểm Khách hàng Thân thiết (ReCar Rewards & Gamification)
- **Ý tưởng:** Khách hàng hoàn thành mỗi chuyến đi sẽ được tích điểm (100.000đ = 1 điểm).
- **Trải nghiệm:** Đạt hạng Bạc, Vàng, Kim Cương để nhận mã giảm giá 5 - 15%, ưu đãi miễn phụ phí tài xế hoặc miễn tiền cọc thế chấp.

---

## 6. HƯỚNG DẪN KÍCH HOẠT VNPAY SANDBOX THẬT (KHI CẦN)
1. Truy cập trang đăng ký thử nghiệm miễn phí của VNPay: [https://sandbox.vnpayment.vn/devreg](https://sandbox.vnpayment.vn/devreg)
2. Điền thông tin cá nhân/dự án, nhận ngay cặp mã `vnp_TmnCode` và `vnp_HashSecret` gửi qua email.
3. Cập nhật vào [appsettings.json](file:///d:/CarRental_TTCN/CarRental/appsettings.json):
   ```json
   "VnPay": {
     "TmnCode": "MÃ_TMN_MỚI",
     "HashSecret": "KHÓA_HASH_SECRET_MỚI",
     "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
     "Version": "2.1.0",
     "Command": "pay",
     "CurrCode": "VND",
     "Locale": "vn",
     "PaymentBackReturnUrl": "http://localhost:5163/Cart/PaymentCallBack",
     "BypassPayment": false
   }
   ```
4. Lưu tệp và tiến hành kiểm thử thanh toán với các thẻ test mẫu do VNPay cung cấp.

---

## 7. CẬP NHẬT CÔNG VIỆC MỚI NHẤT (NGÀY 21 - 22 THÁNG 10 NĂM 2026)

### 7.1. Chuẩn hóa Khung Tìm kiếm Xe Trang Chủ
- **Khắc phục lỗi cắt chữ ô "Giờ trả":** 
  - Trước đây hệ thống Bootstrap chia cột `col-md-1` (~75px) làm trường `input[type="time"]` bị co cụt thành `08:0` (mất chữ số cuối và icon đồng hồ).
  - Tái cấu trúc khung tìm kiếm sang CSS Grid `.recar-search-grid` (`2.2fr 1.6fr 1.3fr 1.6fr 1.3fr 1.8fr`).
  - Phân bổ độ rộng ~155px cho cả **Giờ nhận** và **Giờ trả**, hiển thị trọn vẹn `08:00 AM` kèm biểu tượng đồng hồ, tương thích hoàn hảo từ màn hình laptop (1366px) đến mobile.

### 7.2. Tích hợp Bảng CSDL Địa điểm Nhận xe (`locations`) & Module Quản trị
- **Cơ sở dữ liệu:** Tạo bảng `locations` trong CSDL MySQL `DbRenalCar`, nạp 17 tỉnh thành trọng điểm tại Việt Nam (Hà Nội, TP.HCM, TP. Vinh - Nghệ An, Đà Nẵng, Hải Phòng, Nha Trang, Đà Lạt, Phú Quốc...) với đầy đủ phân vùng miền Bắc - Trung - Nam, nhãn phổ biến và thứ tự ưu tiên.
- **Dữ liệu động:** Đưa danh sách từ bảng `locations` vào Dropdown địa điểm ở thanh tìm kiếm trang chủ và trang bộ lọc xe người dùng.
- **Module Admin Quản lý Địa điểm (`LocationsController`):** Cung cấp đầy đủ tính năng Thêm mới, Sửa, Xóa và Bật/Tắt trạng thái hiển thị địa điểm nhận xe trên sàn.

### 7.3. Nâng cấp Toàn diện Giao diện Admin Portal & Sidebar Công thái học
1. **Xây dựng & Chuẩn hóa các Module Quản trị:**
   - **Quản lý Kiểu xe & Phân khúc (`CarTypesController`):** Hiển thị danh mục kiểu xe, số chỗ ngồi, hãng đại diện, số lượng xe thực tế đang mở thuê trên sàn, thêm/sửa phân khúc.
   - **Quản lý Menu sàn ReCar (`MenusController`):** Loại bỏ 100% template Mazer cũ, chuyển sang thiết kế ReCar Design System với panel bo góc, badge trạng thái và bảng dữ liệu hiện đại.
   - **Quản lý Đánh giá & Bình luận (`ReviewsController`):** Giám sát phản hồi, số sao đánh giá (1 - 5 sao), nhận xét từ khách thuê và kiểm duyệt bình luận.
2. **Khắc phục triệt để các lỗi Thanh Sidebar Admin:**
   - **Khoảng cách Icon - Menu:** Sửa lỗi class `gap-2.5` không hợp lệ, thiết lập khoảng cách cố định 12-14px giữa icon và chữ, đều đặn và không bao giờ dính sát nhau.
   - **Logic Active chuẩn xác:** Định tuyến highlight theo tên Controller cụ thể (`isCars`, `isCarTypes`, `isMenus`, `isLocations`, `isReviews`, `isOrders`, `isUsers`...), xóa bỏ lỗi highlight nhầm vào "Tổng quan".
   - **Triệt tiêu hiện tượng "Thu nhỏ to ra" khi chuyển trang:** Gỡ bỏ `transition` mặc định trên `.recar-admin-sidebar`, chỉ kích hoạt `.sidebar-animating` khi người dùng chủ động bấm nút Toggle. Chuyển trang tức thì, không giật FOUC.
   - **Khắc phục triệt tiêu hiện tượng "Dài ngắn":** Khóa cố định chiều cao `height: 100vh !important; max-height: 100vh !important; align-self: flex-start !important; flex-shrink: 0 !important;` và cố định chiều rộng `width: 260px` (hoặc `72px` khi thu gọn). Ngăn Flexbox kéo dãn sidebar theo chiều cao nội dung trang dài/ngắn.
   - **Lưu & Khôi phục vị trí cuộn (`scrollTop`):** Tự động lưu vị trí cuộn của sidebar vào `sessionStorage` và khôi phục lại khi sang trang mới. Đồng thời tích hợp `scrollIntoView` tự động cuộn nhẹ để mục đang active luôn nằm rõ trong tầm mắt người dùng.

### 7.4. Sửa Lỗi Bộ Lọc Xe Phía Người Dùng (User Filter)
- **Khắc phục lỗi HTTP 500 khi lọc theo Thành phố:**
  - Thay thế cú pháp LINQ `.Any()` trong `ProductController.FilterProducts` (vốn sinh câu lệnh `FROM (VALUES ROW(...))` gây lỗi cú pháp trên MySQL/MariaDB) bằng điều kiện SQL `LIKE` an toàn `(c.Location.Contains(t1) || c.Location.Contains(t2))`.
  - Tương thích 100% với mọi phiên bản MariaDB / MySQL.
- **Lọc Khoảng giá Thời gian thực (Live Debounce):**
  - Tích hợp debounce 300ms trên sự kiện kéo thanh trượt `minPriceSlider` và `maxPriceSlider`, người dùng kéo đến đâu danh sách xe lọc mượt mà đến đó mà không cần thả chuột.
  - Mở rộng thanh giá tối thiểu từ `0đ` đến `5.000.000đ/ngày`.
- **Cập nhật số lượng xe và trạng thái rỗng:**
  - Đồng bộ số lượng xe tìm thấy trên banner đầu trang theo thời gian thực.
  - Hiển thị thông báo trạng thái rỗng thân thiện kèm nút nới rộng bộ lọc khi không có xe nào khớp tiêu chí.

---

## 8. CẬP NHẬT TIẾN TRÌNH PHIÊN LÀM VIỆC (NGÀY 25 - 26 THÁNG 10 NĂM 2026)

### 8.1. Hoàn Thiện Quy Trình Đăng Ký Đối Tác Chủ Xe (KYC & Ngân Hàng)
1. **Mở rộng Cơ sở dữ liệu:**
   - Tạo bảng `car_owner_registrations`: Lưu trữ thông tin đăng ký đối tác chủ xe gồm họ tên, số điện thoại, email, địa chỉ, ảnh CCCD mặt trước/sau, ảnh chân dung cầm CCCD, thông tin ngân hàng nhận tiền (`BankName`, `AccountNumber`, `AccountHolder`), lý do từ chối và trạng thái (`pending`, `approved`, `rejected`).
   - Tạo bảng `banks`: Danh mục 35+ ngân hàng thương mại tại Việt Nam (Vietcombank, MBBank, Techcombank, BIDV, VPBank, TPBank...) hỗ trợ nạp tự động vào dropdown.
   - Tạo bảng `car_brands`: Danh mục các hãng xe hơi phổ biến (Toyota, Hyundai, VinFast, Honda, Mazda, Kia, Ford, BMW, Mercedes...).
2. **Luồng người dùng phía Chủ xe (`/CarOwner/Register`):**
   - Hỗ trợ quay lại chỉnh sửa và bổ sung hồ sơ khi bị từ chối mà không làm mất thông tin cũ.
   - Giao diện đăng ký KYC chuẩn chỉnh, upload CCCD và liên kết ngân hàng thông suốt.
3. **Module Phê duyệt Hồ sơ Chủ xe phía Admin (`/Admin/CarOwners`):**
   - Admin kiểm tra hồ sơ, xem ảnh CCCD phóng to (Lightbox Zoom), duyệt hoặc từ chối kèm lý do phản hồi cho chủ xe.

### 8.2. Nâng Cấp Luồng Đăng Xe & Giấy Tờ Pháp Lý Xe Hơi
1. **Bổ sung 3 Giấy tờ Bắt buộc vào CSDL (`cars`):**
   - `RegistrationImage`: Giấy đăng ký xe (Cà vẹt xe).
   - `InspectionImage`: Giấy chứng nhận kiểm định an toàn kỹ thuật & bảo vệ môi trường (Đăng kiểm).
   - `InsuranceImage`: Bảo hiểm TNDS bắt buộc của chủ xe cơ giới.
2. **Biểu mẫu Thêm xe mới (`/CarOwner/Cars/Create`) & Sửa xe (`/CarOwner/Cars/Edit`):**
   - Nạp hãng xe động từ CSDL `car_brands`.
   - Cho phép nhập màu sắc xe tự do.
   - Bổ sung 3 trường upload tài liệu pháp lý xe, lưu trữ tại thư mục `/wwwroot/files/cars/`.
   - **Bảo mật trạng thái:** Khóa quyền sửa trạng thái xe của chủ xe tại trang `Edit`. Chủ xe không thể tự ý chuyển xe sang "Sẵn sàng" khi chưa được Admin duyệt. Nếu xe bị từ chối sửa lại sẽ tự động chuyển về "Chờ duyệt".

### 8.3. Tái Thiết Kế Trang Phê Duyệt Xe Admin Chuẩn Mẫu 100% Pixel-Perfect
1. **Đồng bộ hóa tuyệt đối với Template Shadcn/Tailwind (`pixel-perfect-replica-7564`):**
   - Căn lề chuẩn `px-4 pb-6` (16px), loại bỏ hiện tượng co cụm hoặc tràn viền.
   - 4 ảnh xe tỷ lệ chuẩn `aspect-ratio: 4/3`, bo góc `8px`, hiệu ứng hover zoom. Đã xóa bỏ toàn bộ nhãn góc text theo đúng yêu cầu người dùng.
   - Bảng thông số 2 cột `<dl class="sheet-spec-grid">`: Nền trắng tinh `#ffffff`, viền mỏng xám `#e2e8f0`, nhãn xám 12px `sheet-field-dt`, giá trị đen 14px `font-weight: 500` `sheet-field-dd` thanh thoát, biển số badge mono, giá tiền có gạch chân `<u>đ</u>`.
   - Khối Giấy tờ xác minh: Tích hợp icon vector **SVG Lucide `FileCheck2`** xanh lá nét mảnh và `FileWarning` chuẩn từng pixel của template mẫu.
2. **Giải quyết triệt để vấn đề Thanh nút che nội dung (Sticky Footer):**
   - Di chuyển 2 nút **"Từ chối"** (viền xám chữ đỏ icon Lucide `XCircle`) và **"Phê duyệt"** (nền xanh ReCar chữ trắng icon Lucide `CheckCircle2`) vào thẳng bên trong luồng cuộn nội dung, đặt ngay sau khối Giấy tờ xe.
   - Xóa bỏ hoàn toàn thanh footer dính ở đáy, giúp toàn bộ Drawer cuộn mượt mà từ trên xuống dưới tự nhiên, không che khuất bất kỳ thông số nào.

### 8.4. Khắc Phục Lỗi Dính Chữ Trong Bảng Quản Lý Xe Của Chủ Xe
- Trong giao diện Dạng bảng (`#tableCarContainer table.recar-table`) tại `/CarOwner/Cars/Index`:
  - Trước đây thiếu CSS cho `.recar-table` dẫn đến trình duyệt render table mặc định, các cột dính sát vào nhau (*Địa điểm Thông số*, *30H7271141📍 abc*...).
  - Đã bổ sung toàn diện hệ thống CSS `.recar-table` trong `CarOwner/Views/Shared/_Layout.cshtml` với `padding: 1.1rem 1.25rem`, `border-collapse: separate`, hiệu ứng hover hàng và thiết lập `min-width` tối thiểu rõ ràng cho cả 7 cột: Phương tiện (250px), Biển số (130px), Địa điểm (170px), Thông số (170px), Giá niêm yết (140px), Trạng thái (150px), Thao tác (160px). Bảng hiển thị rộng rãi, chuyên nghiệp.

---

## 10. CẬP NHẬT TIẾN TRÌNH PHIÊN LÀM VIỆC (NGÀY 27 THÁNG 10 NĂM 2026)

### 10.1. Tái Cấu Trúc Biểu Mẫu Đăng Xe Mới (`/CarOwner/Cars/Create`) Thành Multi-Step Wizard 5 Bước Rộng Rãi
1. **Khắc phục triệt để hiện tượng form cuộn dài gò bó:**
   - Thay thế biểu mẫu 2 cột cuộn dọc dài cũ bằng **Quy trình Multi-step Stepper 5 bước** (`max-width: 1040px`) hiện đại, công thái học, có thanh đo tiến trình (%) và số bước tương tác rõ ràng.
   - Cơ chế điều hướng thông minh: Tự động kiểm tra (validate) từng bước trước khi cho phép người dùng bấm "Tiếp tục".
2. **Thiết kế Lưới 4 Ô Chụp Ảnh Xe Trực Quan (4 Visual Photo Slots):**
   - **Ô 1 (Bắt buộc - Ảnh chính sàn):** Đầu xe / Mặt trước (`ImageFile`) - Hướng dẫn chụp rõ nét logo và biển số.
   - **Ô 2:** Đuôi xe / Mặt sau (`DetailImages`) - Chụp rõ cụm đèn hậu và cốp.
   - **Ô 3:** Nội thất khoang lái (`DetailImages`) - Chụp toàn cảnh taplo, vô lăng và ghế.
   - **Ô 4:** Góc nghiêng 3/4 thân xe (`DetailImages`) - Chụp toàn cảnh sườn xe và mâm lốp.
   - Hỗ trợ xem trước ảnh tức thì (Live Preview), nút xóa/thay đổi ảnh và ô nhập link URL dự phòng.
3. **Tích hợp Danh mục Địa điểm (`locations`):**
   - Dropdown chọn Tỉnh/Thành phố động nạp từ CSDL `locations` kết hợp ô nhập địa chỉ chi tiết, tự động ghép nối thành địa chỉ hoàn chỉnh.
4. **Màn hình Bước 5: Xem Trước Dạng Landing Page (Product Preview Showcase):**
   - Đột phá trải nghiệm: Trước khi bấm "Gửi kiểm duyệt", người dùng được chiêm ngưỡng chiếc xe của mình dưới dạng một **Landing Page sang trọng**:
     - Hero banner tối màu Navy với tên xe, năm sản xuất, hãng xe và biển số xe monospace.
     - Khối trưng bày 4 ảnh xe (1 ảnh lớn + 3 ảnh nhỏ).
     - Bảng thông số vận hành (chỗ ngồi, hộp số, nhiên liệu, tiêu hao), bảng phân rã giá & cọc 30%, địa điểm nhận xe và chip pills tiện nghi đã chọn.
     - Nút "✏️ Sửa" ở từng khối cho phép quay lại chỉnh sửa ngay lập tức.
     - Nút **"🚀 Xác nhận & Gửi hồ sơ kiểm duyệt"** lớn nổi bật kèm hiệu ứng loading.

### 10.2. Tối Ưu Hóa Thanh Header Topbar Chuẩn SaaS & Triệt Tiêu Lỗi Trùng Lặp Header (Double Header)
1. **Khắc phục hiện tượng Double Header tại trang Đăng ký xe (`Create.cshtml`) và Chỉnh sửa xe (`Edit.cshtml`):**
   - Trước đây: Layout đã hiển thị Topbar với tiêu đề to + breadcrumb và nút "+ Đăng xe cho thuê" (kể cả khi đang ở chính trang Create). Ngay bên dưới, view con lại render thêm một khối tiêu đề to thứ 2 kèm nút "Hủy và quay lại", làm giao diện bị đè 2 tầng header cồng kềnh, chiếm hết không gian màn hình.
   - Giải pháp:
     - Xóa bỏ triệt để khối header trùng lặp bên trong các view con (`Create.cshtml`, `Edit.cshtml`).
     - Tích hợp cơ chế `@section TopbarActions`: View con có thể đưa nút bấm tùy chỉnh (ví dụ: nút "Hủy & Thoát", "Xem trên sàn") lên thẳng Topbar của Layout.
     - Tự động ẩn nút "+ Đăng xe cho thuê" trên Topbar khi người dùng đang ở trang tạo xe (`currentController == "Cars" && currentAction == "Create"`).
2. **Thiết kế lại Topbar Layout (`_Layout.cshtml`) theo tỷ lệ chuẩn SaaS cao cấp:**
   - Thu nhỏ kích thước tiêu đề trang từ `1.75rem` xuống `1.35rem`, font-weight 800 sắc nét, không còn choán diện tích.
   - Breadcrumb tinh gọn, liên kết xám nhã nhặn.
   - Chuyển đổi nút "Thông báo" từ dạng viên thuốc bành trướng sang nút **Icon chuông tròn 38px** thanh lịch chuẩn SaaS hiện đại, có chấm đỏ Real-time nhấp nháy tinh tế.
   - User Profile dạng viên thuốc bo tròn mỏng nhẹ với avatar gradient tròn.
   - Bổ sung đường phân cách mỏng (`border-bottom: 1px solid #eef2f6`), mang lại tổng thể thoáng đãng, sang trọng và chuẩn mực.

### 10.3. Tái Cấu Trúc Toàn Diện Màn Hình Chỉnh Sửa Xe (`Cars/Edit.cshtml`) Phân Quyền Theo Trạng Thái Phương Tiện
1. **Status Context Banner thông minh trên đầu trang:**
   - Tự động nhận diện 5 trạng thái phương tiện (`Chờ duyệt`, `Sẵn sàng`, `Đang bận / Đang có khách`, `Từ chối`, `Tạm ngưng`).
   - Hiển thị thông báo ngữ cảnh và hướng dẫn rõ ràng cho Chủ xe về những quyền hạn chỉnh sửa được phép ở trạng thái đó.
2. **Quy tắc phân quyền trường dữ liệu chặt chẽ (Data Integrity):**
   - **Khi xe đang "Sẵn sàng" (Đã duyệt lên sàn):** Biển số xe và Giấy tờ pháp lý chuyển sang chế độ `readonly` kèm huy hiệu `[✓ Đã xác thực]`, ngăn chặn việc tráo đổi biển số/giấy tờ chưa kiểm duyệt. Chủ xe vẫn được tự do đổi Giá thuê, Khuyến mãi, Địa chỉ và Tiện nghi.
   - **Khi xe đang "Đang bận" (Khách đang thuê):** Tự động khóa `readonly` trường Đơn giá niêm yết, Biển số và Địa điểm giao nhận nhằm bảo vệ quyền lợi hợp đồng với khách thuê đang chạy trên đường.
   - **Khi xe "Chờ duyệt" hoặc "Từ chối":** Mở khóa toàn bộ các trường để chủ xe chủ động sửa sai hoặc bổ sung hồ sơ. Khi bấm Lưu ở xe bị từ chối, hệ thống tự động đưa về hàng đợi "Chờ duyệt" để Admin thẩm định lại.
3. **Cải tiến công thái học (Ergonomics):**
   - Đưa hộp thao tác Lưu lên trên cùng của cột phải với hiệu ứng `sticky-top`, giúp chủ xe bấm lưu ngay lập tức ở bất kỳ vị trí cuộn nào mà không phải cuộn chuột dài mệt mỏi.
   - Nút hành động thông minh: Tự đổi nhãn thành *"Cập nhật & Gửi duyệt lại"* khi xe đang bị từ chối.

### 10.4. Tinh Chỉnh Giao Diện Chỉnh Sửa Xe: Status Banner Thanh Mảnh & Bộ Sưu Tập 4 Góc Ảnh Ô Tô
1. **Tối ưu hóa Status Context Banner (Ảnh 1):**
   - Thay thế khối thông báo cồng kềnh bằng thanh thông báo mỏng gọn (Slim notification strip): Chiều cao tinh gọn, đường viền 1px bo góc 12px trang nhã.
   - Khắc phục triệt để lỗi icon hình tròn bị méo phình to: Cố định kích thước chuẩn `width: 36px; height: 36px; min-width: 36px; border-radius: 50%` với màu nền pastel dịu mắt.
   - Badge trạng thái phương tiện được bố trí thẳng hàng cùng hàng với dòng thông báo ở góc phải, không bị rớt xuống dòng.
2. **Nâng cấp Bộ sưu tập 4 góc ảnh ô tô dạng lưới 2x2 (Ảnh 2):**
   - Xóa bỏ hoàn toàn Card "Ảnh đại diện" 1 ảnh đơn điệu trước đây.
   - Xây dựng **Bộ sưu tập 4 góc ảnh xe (Lưới 2x2)** chuẩn kiểm định:
     - **Slot 1 (Đầu xe - Ảnh đại diện chính):** Hiển thị ảnh đại diện hiện tại, cho phép thay đổi qua `ImageFile`.
     - **Slot 2 (Đuôi xe):** Hiển thị ảnh chi tiết 1, cập nhật qua `DetailImage2`.
     - **Slot 3 (Khoang lái / Nội thất):** Hiển thị ảnh chi tiết 2, cập nhật qua `DetailImage3`.
     - **Slot 4 (Thân xe / Toàn cảnh):** Hiển thị ảnh chi tiết 3, cập nhật qua `DetailImage4`.
   - Mỗi slot có tỷ lệ 4:3, bo góc 10px, nhãn badge góc trên mờ kính, overlay đổi ảnh khi hover và trạng thái placeholder nét đứt nếu ô chưa có ảnh.
   - JavaScript `previewSlot` cho phép xem trước tức thì khi người dùng chọn file và tự động đánh dấu tích xanh `✓`.
   - Backend `CarsController.cs` và `CarEditVM.cs` xử lý lưu và ghi đè chính xác từng slot ảnh chi tiết vào CSDL `CarImages`.




### 10.5. Chuẩn Hóa Quản Lý Tiện Ích Phương Tiện Trong Cơ Sở Dữ Liệu (Database-Driven Amenities)
1. **Thiết kế & Tạo bảng CSDL mới trong MySQL:**
   - **Bảng amenities:** Lưu danh mục tiện ích chuẩn (AmenityId, Name, Icon, IconColor, DisplayOrder, IsActive).
   - **Bảng car_amenities:** Bảng trung gian liên kết nhiều - nhiều (N - N) giữa xe và tiện ích (CarId, AmenityId) với khóa chính phức hợp và ràng buộc khóa ngoại CASCADE.
   - **Seed dữ liệu & Tự động khởi tạo:** Khai báo cấu trúc bảng và seed sẵn 14 tiện nghi mặc định trong Program.cs, đồng thời đã di trú (migrate) dữ liệu tiện ích của các xe cũ sang bảng mới.
2. **Cập nhật Backend & Model:**
   - Tạo Amenity.cs và CarAmenity.cs.
   - Đăng ký DbSet trong DbRenalCarContext.cs.
   - Thêm AmenityIds vào CarCreateVM và CarEditVM.
   - Cập nhật CarsController.cs: Cả 2 action Create và Edit đều nạp danh mục tiện ích động từ CSDL, lưu liên kết vào car_amenities, đồng thời vẫn duy trì cập nhật chuỗi Details/Descriptions để tương thích ngược 100%.
3. **Cập nhật Giao diện:**
   - Cả Create.cshtml và Edit.cshtml được chuyển đổi sang duyệt danh sách tiện ích từ CSDL, hỗ trợ toggle linh hoạt qua AmenityIds.

### 10.6. Tái Cấu Trúc Khối Bảng Giá & Quy Định Hành Trình (Tách Hàng Thoáng Đãng)
- **Khắc phục lỗi chèn ép giao diện:** Trước đây các trường Giới hạn km, Phụ phí vượt km và Giao xe tận nơi bị nhét chung 3 cột col-md-4 làm ô phí giao xe bị bẹp dúm thành '100('.
- **Phân tách bố cục đa tầng thoáng đãng:**
  - Hàng 1: Đơn giá niêm yết & Giá khuyến mãi (2 cột rộng rãi), bỏ số đuôi .00 thừa của decimal.
  - Hàng 2: Xuống dòng riêng cho Giới hạn quãng đường & Phụ phí quá km (2 cột col-md-6), bổ sung ghi chú giải thích.
  - Hàng 3: Tách Giao xe tận nơi thành Panel Card riêng biệt có Switch bật/tắt (Toggle switch) và ô nhập phí độc lập rộng rãi.
- **Đồng bộ:** Áp dụng hoàn chỉnh cho cả Edit.cshtml và Create.cshtml.

### 10.7. Chuẩn Hóa Cơ Chế Vòng Đời Phương Tiện & Xóa Xe An Toàn (Safety Guard & Soft Delete)
1. **Nghiệp vụ thực tế:**
   - Trong mô hình sàn thương mại điện tử / chia sẻ xe (như Airbnb, Mioto), một chiếc xe **tuyệt đối không được phép "xóa cái rụp" (Hard Delete)** nếu đã từng phát sinh đơn hàng, hợp đồng thuê xe hoặc đánh giá nhằm tránh vi phạm toàn vẹn khóa ngoại (Foreign Key Restrict) và làm mất dữ liệu hóa đơn, kế toán thuế.
2. **Triển khai Cơ chế 3 tầng bảo vệ dữ liệu:**
   - **Tầng 1 - Kiểm tra an toàn (Safety Guard):** Khóa xóa và hiển thị cảnh báo đỏ nguy cấp nếu xe đang ở trạng thái `Đang bận`, `Đang có khách` hoặc có đơn đặt thuê chưa hoàn thành (`ReturnDate >= DateTime.Now`).
   - **Tầng 2 - Xóa mềm (Soft Delete / Ngừng kinh doanh):** Áp dụng cho xe đã có lịch sử (`Trips > 0` hoặc có `OrderDetails` / `CarReviews`). Xe được chuyển sang `CarStatus = "Ngừng hoạt động"`, `IsActive = false` để ẩn khỏi sàn khách thuê nhưng bảo toàn trọn vẹn số liệu thống kê và lịch sử doanh thu.
   - **Tầng 3 - Xóa cứng (Hard Delete):** Chỉ cho phép đối với xe mới tạo nhầm hoặc chưa từng phát sinh bất kỳ chuyến đi/hợp đồng nào. Đồng thời dọn sạch các bản ghi liên quan trong `car_amenities` và `car_images`.
3. **Cung cấp Tùy chọn "Tạm ngưng đón khách" (Pause Listing):**
   - Hỗ trợ chủ xe tạm ẩn xe khỏi sàn khi cần bảo dưỡng, sửa chữa hoặc bận việc cá nhân mà không phải xóa xe. Có thể mở lại bất cứ lúc nào chỉ với 1 thao tác.
4. **Nâng cấp Giao diện Xác nhận (`Delete.cshtml` & `Index.cshtml`):**
   - Thiết kế lại toàn bộ view [Delete.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Delete.cshtml) theo phong cách ReCar SaaS hiện đại (thẻ xe nổi bật, biển số xe chuẩn mono, 3 thống kê Chuyến đi - Đơn thuê - Đánh giá, thông báo ngữ cảnh tương ứng với 3 kịch bản bảo vệ).
   - Loại bỏ alert popup thô của trình duyệt tại [Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Index.cshtml), bổ sung bộ lọc trạng thái `Tạm ngưng` trên thanh công cụ và chuẩn hóa badge `Tạm ngưng`, `Ngừng KD`.

---

## 11. CẬP NHẬT TIẾN TRÌNH PHIÊN LÀM VIỆC (NGÀY 28 THÁNG 10 NĂM 2026)

### 11.1. Nâng Cấp Toàn Diện Phân Hệ Quản Lý Đơn Thuê Xe Chủ Xe (`Areas/CarOwner/Orders`)
1. **Khép kín Vòng đời Trạng thái Đơn hàng & Cơ sở dữ liệu:**
   - Bổ sung và seed tự động trạng thái đơn hàng thứ 5: `StatusId = 5` (*"Hoàn tất"*) trong [Program.cs](file:///d:/CarRental_TTCN/CarRental/Program.cs).
   - Chuẩn hóa luồng nghiệp vụ 4 bước: `[Đã cọc 30% (Status 3)]` ➜ Bấm *"Bàn giao xe"* chuyển sang `[Đang cho thuê (Status 4)]` ➜ Bấm *"Nhận lại xe & Hoàn tất"* chuyển sang `[Đã hoàn tất (Status 5)]`. Đồng thời hỗ trợ nút *"Hủy đơn (Status 2)"* kèm ghi chú lý do.
2. **Tự động Đồng bộ Hóa 2 Chiều giữa Trạng thái Đơn và Trạng thái Phương tiện:**
   - **Khi xác nhận Bàn giao xe (`ConfirmHandover`):** Đơn chuyển sang `Đang cho thuê` (Status 4), xe liên quan tự động chuyển sang `CarStatus = "Đang bận"` và `IsActive = false` (tạm ẩn khỏi sàn khách thuê để tránh bị đặt trùng lịch khi xe đang trên đường).
   - **Khi xác nhận Nhận lại xe (`ConfirmReturn`):** Đơn chuyển sang `Đã hoàn tất` (Status 5), xe tự động quay về `CarStatus = "Sẵn sàng"`, `IsActive = true` và tự động tăng số chuyến thành công `Trips = Trips + 1`.
   - **Khi Hủy đơn (`CancelOrder`):** Đơn chuyển sang `Đã hủy` (Status 2), xe được giải phóng trở lại trạng thái `Sẵn sàng` đón khách mới, ghi nhận lý do hủy vào `Notes`.
   - Tích hợp phát thông báo thời gian thực Real-time SignalR qua [ReCarHub.cs](file:///d:/CarRental_TTCN/CarRental/Hubs/ReCarHub.cs) gửi tới Ban Quản trị khi trạng thái đơn thay đổi.

### 11.2. Tái Thiết Kế Trang Danh Sách Đơn Thuê Xe ([Orders/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Index.cshtml)) Chuẩn SaaS Hiện Đại
1. **Thanh Tab Phân Loại Trạng Thái (Pill Tabs):**
   - 5 Tab viên thuốc bo tròn: *Tất cả*, *Chờ bàn giao (Đã cọc 30%)*, *Đang cho thuê*, *Đã hoàn tất*, *Đã hủy*.
   - Tự động hiển thị số lượng đơn tương ứng trên từng tab (`badge-count`).
2. **Thanh Tìm Kiếm Đa Năng:**
   - Tìm kiếm tức thì theo: Mã đơn `#RC-...`, Tên khách hàng, Số điện thoại, Tên xe, Biển số xe.
3. **Bảng Dữ Liệu SaaS Rộng Rãi & Trực Quan:**
   - **Cột Phương tiện:** Ảnh xe mini bo góc (58x42px), Tên xe in đậm, Biển số font monospace sang trọng kèm hộp số.
   - **Cột Khách hàng:** Avatar tròn chữ cái gradient, Tên khách, SĐT.
   - **Cột Lịch trình:** Ngày nhận - Ngày trả, badge số ngày thuê.
   - **Cột Tài chính:** Bóc tách rõ Tổng tiền thuê (xanh ReCar), Tiền cọc sàn đã thu 30% (xanh lá), Số tiền khách trả trực tiếp 70% khi nhận xe.
   - **Cột Trạng thái:** Badge viên thuốc chuẩn màu theo trạng thái (Vàng hổ phách: Chờ bàn giao, Xanh ngọc: Đang thuê, Xanh lá: Hoàn tất, Đỏ: Đã hủy).
   - **Cụm Thao Tác Thông Minh:** Nút *Chi tiết*, Nút thao tác nhanh *Giao xe* / *Nhận xe* và Nút gọi điện nhanh tích hợp Modal Popup SweetAlert2 tương tác mượt mà không cần F5.

### 11.3. Nâng Cấp Toàn Diện Trang Chi Tiết Đơn Thuê ([Orders/Details.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Details.cshtml))
1. **Banner Phương Tiện Bàn Giao Lớn:**
   - Trưng bày ảnh xe lớn sắc nét, thông số vận hành (Hộp số, Nhiên liệu, Địa điểm bàn giao xe), đơn giá niêm yết/ngày.
   - Bảng thông tin lịch trình thời gian nhận - trả và thời lượng thuê rõ ràng.
2. **Khối Thông Tin Khách Hàng:**
   - Tên người thuê, số điện thoại, email, địa chỉ, huy hiệu *Đã xác thực danh tính CCCD* và nút gọi điện thoại nhanh.
3. **Quy Trình & Checklist Bàn Giao An Toàn (Safety Checklist):**
   - 4 bước kiểm tra thiết thực: Đối chiếu CCCD & GPLX bản gốc; Thu 70% số tiền thuê còn lại; Ghi nhận hiện trạng ODO và mức nhiên liệu/pin; Kích hoạt bàn giao xe.
4. **Hộp Thao Tác Nghiệp Vụ Ngữ Cảnh (Action Command Card):**
   - Hiển thị nút hành động phù hợp theo từng trạng thái: *Xác nhận bàn giao xe*, *Hủy đơn thuê*, *Nhận lại xe & Hoàn tất*.
   - Popup xác nhận thông minh hiển thị số tiền 70% cần thu và lưu ý kiểm tra trước khi bàn giao chìa khóa.
5. **Bảng Phân Rã Tài Chính 3 Bên Minh Bạch:**
   - Tổng giá trị hợp đồng thuê xe.
   - Tiền cọc 30% sàn giữ hộ qua cổng VNPay.
   - Tiền khách thanh toán trực tiếp khi nhận xe (70% - hiển thị màu đỏ to nổi bật).
   - Phí hoa hồng sàn 10% và **Thu nhập thực nhận 90% của chủ xe**.
   - Hướng dẫn đối soát định kỳ chuyển khoản vào tài khoản ngân hàng.

---

## 12. CẬP NHẬT GIAO DIỆN PHÂN HỆ CHỦ XE THEO BỘ TEMPLATE MỚI (NGÀY 29/10/2026)

Đã đồng bộ toàn bộ giao diện từ bộ template clone `giao dien chu xe/screenshot-to-scene-088` vào phân hệ Chủ xe (`Areas/CarOwner`):

1. **Khung Layout & Sidebar ([_Layout.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Shared/_Layout.cshtml)):**
   - Chuẩn hóa Sidebar 260px nền Deep Navy `#0f172a` theo đúng `HostShell.tsx`.
   - Logo thương hiệu ReCar Host Pro với icon xe nổi bật, loại bỏ profile card cứng "Trần Quốc Bảo" theo yêu cầu người dùng.
   - Thêm nút thu gọn Sidebar tròn tinh tế (Collapse Sidebar) trên Topbar và Header, ghi nhớ trạng thái bằng `localStorage`.
   - Chỉnh sửa padding container sidebar (`16px 8px 12px 8px`) và `margin: 1px 6px` cho `.sidebar-nav-link` để khối active màu cyan `#00a8e8` thụt lề vào trong ~14px, không tràn viền.
   - Xóa bỏ nhãn `+ Mới` tại mục "Thêm xe mới" theo yêu cầu người dùng.
   - Thiết lập hệ thống thẻ `card-surface`, biển số monospace `plate`, và bộ huy hiệu `status-badge`.
   - Bổ sung các class CSS tương thích Tailwind: `.gap-1\.5`, `.gap-2\.5`, `.gap-3\.5`, `.place-items-center`, `.icon-box-fixed`.

2. **Trang chủ / Tổng quan ([Home/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Home/Index.cshtml)):**
   - Banner chào mừng cá nhân hóa, loại bỏ nút "Thêm mới xe" thừa ở banner.
   - 4 Thẻ KPI: Thu nhập thực nhận (90%), Xe đang quản lý (Sẵn sàng / Đang thuê / Chờ duyệt), Chuyến hoàn thành, Đánh giá trung bình.
   - Biểu đồ tăng trưởng 6 tháng Area Chart (Chart.js) với 2 đường: Doanh thu giao dịch vs Thu nhập thực nhận.
   - Thẻ *Host Insights - Mẹo tối ưu doanh thu* với 6 gợi ý thiết thực, căn chỉnh icon ngay ngắn, chuẩn khoảng cách `gap: 12px;` thoáng đãng, không dùng emoji.
   - Bảng Đơn thuê mới nhất trực quan, phân rã cọc 30% và tiền mặt 70%.

3. **Quản lý Đội xe ([Cars/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Index.cshtml)):**
   - Thanh công cụ tìm kiếm và lọc: Lọc theo Hãng, Phân khúc, Trạng thái và Sắp xếp linh hoạt.
   - Bảng danh sách xe chuẩn mẫu template: Ảnh xe, Tên xe, Phân khúc, Biển số `plate`, Đơn giá/ngày, Địa điểm nhận xe, Số chuyến, Đánh giá sao, Badge trạng thái.
   - Hai công tắc Toggle Switch tức thời: Bật/Tắt trạng thái hoạt động (`available` <-> `paused`) và Bật/Tắt giá khuyến mãi (`ToggleSale`).
   - Dropdown thao tác 3 chấm: Chi tiết xe, Chỉnh sửa, Xem trên sàn, Gỡ xe.

4. **Thêm xe mới ([Cars/Create.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Create.cshtml)):**
   - Tái cấu trúc sang bố cục Stepper 2 cột hiện đại theo `CarForm.tsx`: Cột trái 260px điều hướng 6 bước dọc có tick xanh khi hoàn thành; Cột phải là card form từng bước.
   - 6 bước tuần tự: Thông tin cơ bản -> Giá thuê -> Quy định vận hành -> Tiện nghi (đã kết nối với CSDL qua bảng `Amenity` và bảng liên kết `CarAmenity`) -> Hình ảnh & giấy tờ -> Kiểm tra & gửi duyệt.

5. **Chỉnh sửa xe & Xem chi tiết xe ([Cars/Edit.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Edit.cshtml), [Cars/Details.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Details.cshtml)):**
   - Thiết kế giao diện xem chi tiết xe chuẩn template: Banner gallery ảnh xe, thông số kỹ thuật, tiện nghi, lịch trình và báo cáo.
   - Giao diện chỉnh sửa xe theo 6 bước Stepper đồng bộ với trang Tạo mới.
   - Xây dựng cơ chế kiểm soát chỉnh sửa thông minh: Phân quyền theo trạng thái xe; nếu xe đã duyệt hoạt động khi chỉnh sửa các thông tin quan trọng sẽ chuyển sang trạng thái "Chờ admin duyệt cập nhật" để đảm bảo tính an toàn dữ liệu trên sàn.

6. **Lịch trình & Đơn xe ([Orders/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Index.cshtml)):**
   - Thanh Tab 5 nhóm trạng thái (Tất cả, Chờ bàn giao, Đang cho thuê, Đã hoàn tất, Đã hủy) kèm badge đếm số lượng.
   - Thanh tìm kiếm đơn xe tức thời.
   - Cột Khách thuê: Avatar tròn chứa chữ cái viết tắt (`TR`, `TK`) được căn giữa tâm tuyệt đối (`d-inline-flex align-items-center justify-content-center`), số điện thoại gọi nhanh.
   - Bảng dữ liệu SaaS chuẩn theo `CarOwner.Orders.Index.tsx` tích hợp đầy đủ SweetAlert2 cho thao tác *Bàn giao xe* và *Nhận xe*.

7. **Trang Chi tiết Đơn thuê ([Orders/Details.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Details.cshtml)):**
   - Banner 2 cột trực quan: thông tin xe và lịch trình chi tiết.
   - 3 Card thông tin nghiệp vụ:
     - *Khách thuê:* Avatar căn giữa chuẩn xác, sđt gọi nhanh, nút bật/tắt xem số CCCD và GPLX đầy đủ để đối chiếu lúc bàn giao.
     - *Checklist bàn giao an toàn:* 5 mục kiểm tra an toàn với icon tick xanh sắc nét, khoảng cách rộng rãi thoáng mắt (`gap: 14px; mb-3`).
     - *Phân rã tài chính:* Minh bạch tiền thuê, cọc 30% sàn thu, còn lại 70% khách trả trực tiếp cho chủ xe, trừ hoa hồng sàn 10%.
   - Timeline tiến trình 5 bước theo dõi vòng đời đơn thuê từ lúc cọc đến khi hoàn tất.
   - Sửa lỗi tương thích biên dịch liên quan đến kiểu dữ liệu `DateOnly`.

---

## 13. TỔNG KẾT & TRẠNG THÁI HỆ THỐNG
- **Dự án ASP.NET Core (.NET 9):** Biên dịch sạch sẽ `0 Error(s)`.
- **Cơ sở dữ liệu SQL Server (`dbrenalcar.sql`):** Đã cập nhật cấu trúc bảng và dữ liệu mẫu đồng bộ.
- **Dọn dẹp mã nguồn:** Đã gỡ bỏ các tệp rác `.git` thừa trong thư mục template clone, giữ repository gọn gàng chuẩn mực.
- **Sẵn sàng:** Toàn bộ mã nguồn đã được commit và đồng bộ lên remote GitHub `origin/main`.



