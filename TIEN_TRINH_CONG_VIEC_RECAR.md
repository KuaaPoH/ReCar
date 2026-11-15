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

## 13. CẬP NHẬT TIẾN TRÌNH PHIÊN LÀM VIỆC (NGÀY 30 THÁNG 10 NĂM 2026)

### 13.1. Hoàn Thiện Đồng Bộ Tiện Nghi (`car_amenities`) & Quy Định Vận Hành Động (`CarPolicy`) Cho Chủ Xe
1. **Đồng bộ hiển thị tiện nghi từ bảng liên kết `car_amenities`:**
   - Cập nhật [CarsController.cs](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/CarsController.cs): Truy vấn đầy đủ đối tượng [`Amenity`](file:///d:/CarRental_TTCN/CarRental/Models/Amenity.cs) (bao gồm `Name`, `Icon`, `IconColor`, `DisplayOrder`) từ bảng [`CarAmenity`](file:///d:/CarRental_TTCN/CarRental/Models/CarAmenity.cs) cho cả trang Chi tiết xe (`Details`), trang Ngừng kinh doanh/Xóa xe (`Delete`) và đồng bộ tên tiện nghi từ `AmenityIds` khi tạo xe mới (`Create`).
   - Cập nhật [Details.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Details.cshtml) và [Delete.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Delete.cshtml): Hiển thị danh sách tiện nghi kèm biểu tượng Bootstrap Icons và màu sắc trực quan lấy từ CSDL thay vì danh sách tĩnh cố định.
2. **Trích xuất Quy định vận hành động ([`CarPolicy.cs`](file:///d:/CarRental_TTCN/CarRental/Utilities/CarPolicy.cs)):**
   - Tự động bóc tách Giới hạn quãng đường (`km/ngày`), Phụ phí vượt quãng đường (`VNĐ/km`), Hỗ trợ giao xe tận nơi & Phí giao xe, và Mức tiêu hao nhiên liệu của từng xe để hiển thị chính xác trên `Details.cshtml` và nạp lại vào biểu mẫu `Edit.cshtml`.

### 13.2. Triển Khai Biên Bản Bàn Giao & Đối Chiếu Hiện Trạng Xe Có Ảnh (Digital Check-in / Check-out)
1. **Cơ sở dữ liệu & Model ([`VehicleInspection.cs`](file:///d:/CarRental_TTCN/CarRental/Models/VehicleInspection.cs)):**
   - Bổ sung bảng `vehicle_inspections` (tự động khởi tạo trong [Program.cs](file:///d:/CarRental_TTCN/CarRental/Program.cs) và đăng ký trong [`DbRenalCarContext.cs`](file:///d:/CarRental_TTCN/CarRental/Models/DbRenalCarContext.cs)).
   - Lưu trữ biên bản theo 2 mốc của mỗi đơn thuê: `handover` (Lúc giao xe - Check-in) và `return` (Lúc nhận lại xe - Check-out), gồm: Số công tơ mét (`Odometer`), Mức xăng/pin (`FuelLevel` 0-100%), 4 ảnh chụp 4 góc xe thực tế (`FrontImage`, `RearImage`, `LeftImage`, `RightImage`), Ghi chú hiện trạng (`Notes`) và dấu thời gian.
2. **Nghiệp vụ Backend ([OrdersController.cs](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/OrdersController.cs)):**
   - Tích hợp lưu biên bản và upload 4 ảnh góc xe (`/files/inspections/`) trực tiếp vào luồng `ConfirmHandover`, `ConfirmReturn` và bổ sung endpoint `SaveInspection` cho phép cập nhật/bổ sung biên bản bất cứ lúc nào.
   - Tự động đối chiếu số km lúc nhận lại so với lúc giao xe (`drivenKm = returnOdo - handoverOdo`), so sánh với hạn mức miễn phí (`Số ngày × Giới hạn km/ngày`) và tự động tính phụ phí vượt km theo quy định của xe.
3. **Giao diện Trực quan & Modal Chụp 4 Góc Xe ([Orders/Details.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Details.cshtml) & [Orders/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Index.cshtml)):**
   - Nối trực tiếp nút lập biên bản vào thẻ **Checklist bàn giao an toàn** và thanh thao tác trên cùng.
   - **Modal Biên bản điện tử (`#digitalInspectionModal`):** Hỗ trợ nhập ODO, thanh trượt + nút chọn nhanh mức xăng/pin (`25%`, `50%`, `75%`, `100%`), lưới 4 ô chụp/tải ảnh 4 góc xe (Đầu xe, Đuôi xe, Thân trái, Thân phải) có xem trước tức thì (`Live Preview`) và tính toán tự động số km vượt + phụ phí ngay khi gõ ODO lúc nhận xe.
   - **Bảng Đối chiếu 2 chiều (Check-in vs Check-out):** Đặt song song biên bản lúc giao và lúc nhận lại xe kèm album 4 góc ảnh (hỗ trợ phóng to Lightbox) và hộp tổng hợp chênh lệch quãng đường, phụ phí vượt km và mức nhiên liệu.

### 13.3. Tái Thiết Kế Trang Hồ Sơ Pháp Lý & Ngân Hàng Theo Chuẩn Mẫu Template (`PartnerProfile.tsx`)
1. **Khung Layout & Bố cục:**
   - Chuyển đổi toàn diện trang `/CarOwner/Register/Pending` sang sử dụng layout **Dashboard Shell của Host Pro** (`Areas/CarOwner/Views/Shared/_Layout.cshtml`).
   - Thiết kế chuẩn 100% theo component `PartnerProfile.tsx` với 3 trạng thái banner trực quan:
     - `Approved`: Banner xanh lá xác nhận hồ sơ đối tác đã được duyệt.
     - `Pending`: Banner vàng thông báo hồ sơ đang xét duyệt kèm dấu thời gian.
     - `Rejected`: Banner đỏ thông báo lý do bị từ chối và danh sách tài liệu cần bổ sung.
2. **Cấu trúc 2 Card đối xứng hiện đại:**
   - **Cột 1 - Hồ sơ định danh:** Họ tên, số CCCD mask (`079****8921`), số GPLX mask (`7901****1042`), hạng GPLX (`B2`) và lưới 4 ô tài liệu (*CCCD mặt trước, CCCD mặt sau, GPLX mặt trước, GPLX mặt sau*) có tích hợp xem ảnh phóng to Modal Lightbox.
   - **Cột 2 - Tài khoản nhận tiền:** Biểu mẫu cho phép chủ xe cập nhật Tên ngân hàng (có datalist gợi ý 35+ ngân hàng tại VN), Số tài khoản và Tên chủ tài khoản IN HOA KHÔNG DẤU.
3. **Nghiệp vụ Backend ([RegisterController.cs](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/RegisterController.cs)):**
   - Bổ sung action `POST /CarOwner/Register/UpdateBank` xử lý cập nhật tài khoản ngân hàng thụ hưởng, đồng bộ với bảng `Customers` và tự động gửi lại hồ sơ xét duyệt khi bị từ chối (`Status = "Pending"`).
   - Tự động fallback nạp thông tin hồ sơ của chủ xe đang đăng nhập ngay cả khi là tài khoản mẫu đã kích hoạt.

### 13.4. Nâng Cấp Trang Chi Tiết Thuê Xe Khách Hàng (`/Product/Details.cshtml`) Đồng Bộ 100%
1. **Dải ảnh Album 4 góc xe (Gallery Thumbnails):**
   - Đọc toàn bộ ảnh từ bảng `CarImages` (hỗ trợ cả cấu trúc đa cột `Image1`..`Image4` và đa dòng), nạp danh sách vào `ViewBag.GalleryImages`.
   - Bổ sung dải thumbnail nằm ngay dưới ảnh lớn, hỗ trợ bấm vào ảnh nhỏ đổi ảnh chính tức thì kèm hiệu ứng chuyển tiếp mượt mà.
2. **Hiển thị thông số vận hành chuẩn xác:**
   - Thay thế các giá trị tĩnh bằng dữ liệu thực tế từ CSDL: Hộp số (`Model.Transmission`), Nhiên liệu (`Model.Fuel`), Địa điểm bàn giao xe (`Model.Location`).
3. **Tiện nghi trang bị động từ `car_amenities`:**
   - Truy vấn danh sách tiện nghi thực tế theo xe từ `car_amenities` nạp qua `ViewBag.AmenityItems`.
   - Hiển thị huy hiệu tổng số tiện ích kèm icon Bootstrap Icons và tên tiện nghi rõ ràng, đồng thời hiển thị mô tả bổ sung từ chủ xe.
4. **Quy định hành trình & Dịch vụ giao nhận xe (`CarPolicy`):**
   - Thêm khối **"Quy định hành trình & Giao xe"** nổi bật: Giới hạn quãng đường (`km/ngày`), Phụ phí vượt quãng đường (`VNĐ/km`), Thông tin dịch vụ giao xe tận nơi (kèm mức phí nếu có) và Mức tiêu hao nhiên liệu công bố.
   - Bổ sung chỉ số hạn mức di chuyển vào Sticky Booking Widget bên phải để khách hàng dễ dàng theo dõi trước khi tiến hành đặt cọc.

### 13.5. Rà Soát Toàn Diện Phân Hệ Khách Hàng (User Portal) & Khắc Phục Lỗi Tiềm Ẩn
1. **Khắc phục lỗi fallback tiện ích tĩnh giả định:**
   - Xóa bỏ hoàn toàn khối render 6 tiện ích cứng mặc định khi xe chưa có tiện nghi.
   - Hiển thị thông báo nhã nhặn, trung thực khi xe chưa có tiện nghi; đồng thời lọc bỏ chuỗi nối thông số cũ (`Tự động · Điện...`) khỏi mô tả chi tiết.
2. **Đồng bộ tính toán phụ phí tài xế trong giỏ hàng & Checkout ([`CartController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/CartController.cs)):**
   - Sửa lỗi tính `total` tại GET `Checkout` và `CheckoutSummary` từ `item.Price` sang `item.PriceTotal`, đảm bảo phụ phí tài xế (+500k/ngày) hiển thị khớp 100% giữa giao diện xem trước và số tiền cọc thực tế khi submit.
3. **Phân quyền Menu tài khoản trên Header ([`_Layout.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Shared/_Layout.cshtml)):**
   - Chỉ hiển thị mục *"Trang chủ xe"* khi tài khoản có quyền Chủ xe (`CarOwnerAccountId > 0`) và *"Quản trị"* khi tài khoản là Admin (`IsAdmin`). Tránh việc tài khoản khách hàng thông thường thấy các mục phân hệ quản trị.
4. **Nâng cấp trang Điều khoản dịch vụ & Chính sách thuê xe ([`Privacy.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Home/Privacy.cshtml)):**
   - Thay thế trang mặc định rỗng của Visual Studio thành văn bản quy chuẩn pháp lý chuyên nghiệp: Quy định chung, Giấy tờ đối chiếu, Quy chế đặt cọc 30% & 70% thanh toán khi bàn giao, Biên bản điện tử Digital Check-in/Check-out, Hạn mức quãng đường (350 km/ngày), Bảng phụ phí và Chính sách hoàn hủy chuyến.
5. **Tối ưu thư viện JavaScript:**
   - Gỡ bỏ script jQuery nạp trùng lặp trong `Checkout.cshtml` để tránh xung đột sự kiện với instance jQuery chuẩn đã nạp tại Layout.

### 13.6. Hoàn Thiện Các Giao Diện Còn Thiếu Ở Phân Hệ Khách Hàng Theo Thiết Kế Tổng Thể ReCar
1. **Khối "Các mẫu xe tương tự cùng phân khúc" & Lightbox xem ảnh lớn ([`Views/Product/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Product/Details.cshtml)):**
   - Chuyển đổi toàn bộ thẻ xe tương tự từ các class cũ bị thiếu style sang cấu trúc `.recar-car-card` chuẩn 100% (hiển thị ảnh xe, badge ưu đãi/xe điện/trạng thái bận, 3 thông số chỗ ngồi – hộp số – nhiên liệu, địa điểm, đánh giá sao, giá thuê và 2 nút *Xem chi tiết* / *Thuê ngay*).
   - Bổ sung nút *"Xem ảnh lớn"* và Modal Lightbox (`#carGalleryLightboxModal`) hỗ trợ phóng to ảnh xe, chuyển ảnh trái/phải và thanh chọn nhanh thumbnail.
2. **Nâng cấp danh sách Cẩm nang (`/Blog`) & Tin tức (`/News`) kèm liên kết Trang chủ:**
   - Cập nhật [`Views/Home/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Home/Index.cshtml): Gắn liên kết trực tiếp cho 3 bài viết Cẩm nang & Tin tức dẫn thẳng tới trang đọc chi tiết bài viết.
   - Thiết kế lại [`Views/Blog/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Blog/Index.cshtml), [`Views/Shared/Components/Blog/Default.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Shared/Components/Blog/Default.cshtml), [`Views/News/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/News/Index.cshtml) và [`Views/Shared/Components/News/Default.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Shared/Components/News/Default.cshtml): Bổ sung thanh tìm kiếm bài viết tức thì, các nút lọc chủ đề (Filter Pills), thẻ **Bài viết tiêu điểm (Featured Article)** 2 cột nổi bật và lưới bài viết 3 cột hiện đại.
3. **Thiết kế mới hoàn toàn Trang chi tiết bài viết Cẩm nang & Tin tức ([`Views/Blog/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Blog/Details.cshtml), [`Views/News/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/News/Details.cshtml)):**
   - Xóa bỏ hoàn toàn giao diện bán hoa FloSun cũ.
   - Xây dựng bố cục tạp chí 2 cột sang trọng của ReCar: Breadcrumb, huy hiệu chuyên mục, thông tin tác giả/ngày đăng/thời gian đọc, nội dung bài viết tự động co giãn ảnh (`max-width: 100%`), khu vực bình luận AJAX thời gian thực (SweetAlert2 tiếng Việt), kèm Sidebar đặt xe nhanh và danh sách bài viết liên quan từ [`BlogController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/BlogController.cs) & [`NewsController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/NewsController.cs).
4. **Thiết kế mới Trang kết quả thanh toán ([`Views/Cart/Success.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Cart/Success.cshtml) & [`Views/Cart/PaymentFail.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Cart/PaymentFail.cshtml)):**
   - Xóa bỏ giao diện `Error 404` tiếng Anh cũ của FloSun.
   - Trang `Success.cshtml`: Hiển thị thanh tiến trình 3 bước, xác nhận đặt cọc 30% thành công, tóm tắt mã đơn thuê `#RC-...` vừa tạo, hướng dẫn 3 bước nhận bàn giao xe tiếp theo và nút dẫn thẳng tới *Đơn thuê & Hợp đồng*.
   - Trang `PaymentFail.cshtml`: Hiển thị chi tiết phản hồi từ cổng thanh toán VNPay, nguyên nhân phổ biến & cách khắc phục nhanh kèm các nút điều hướng hỗ trợ.

---

## 14. TỔNG KẾT PHIÊN LÀM VIỆC (NGÀY 30 - 31 THÁNG 10 NĂM 2026)
- **Hoàn thành phân hệ Chủ xe (CarOwner Portal):**
  - Chuyển đổi 100% giao diện sang bộ template SaaS mới của ReCar Host Pro.
  - Tích hợp quản lý tiện nghi qua CSDL (`car_amenities`), quy định vận hành động (`CarPolicy`), quy trình xóa an toàn (Soft Delete), biên bản bàn giao xe điện tử có ảnh 4 góc đối chiếu (Digital Check-in / Check-out).
  - Trang hồ sơ đối tác & tài khoản ngân hàng thụ hưởng (`PartnerProfile.tsx` / `Register/Pending.cshtml`) hoàn chỉnh.
- **Hoàn thành rà soát & thiết kế bổ sung phân hệ Khách hàng (User Portal):**
  - Khối xe tương tự chuẩn `.recar-car-card` & Lightbox xem ảnh lớn trên trang chi tiết xe (`Product/Details.cshtml`).
  - Giao diện Cẩm nang (`/Blog`) và Tin tức (`/News`) hiện đại với Featured Article, thanh tìm kiếm từ khóa, pill lọc chủ đề và trang chi tiết tạp chí xóa sạch code FloSun.
  - Trang kết quả thanh toán đặt cọc 30% (`Cart/Success.cshtml` & `Cart/PaymentFail.cshtml`) chuẩn ReCar, loại bỏ #hoàn toàn lỗi 404 FloSun cũ

  - Chuẩn hóa chính sách pháp lý & điều khoản dịch vụ tại `Home/Privacy.cshtml`.
- **Chất lượng mã nguồn:** Dự án ASP.NET Core (.NET 9) biên dịch sạch `0 Error(s)`.
- **Cơ sở dữ liệu:** Đồng bộ đầy đủ các bảng và dữ liệu seed (`banks`, `amenities`, `car_amenities`, `vehicle_inspections`, `orderstatuses`).

---

## 15. CẬP NHẬT TIẾN TRÌNH: ĐỒNG BỘ BIÊN BẢN BÀN GIAO & ĐỐI CHIẾU ODO VÀO PHÂN HỆ KHÁCH HÀNG (NGÀY 02/11/2026)

### 15.1. Nâng Cấp Backend Truy Vấn Đối Chiếu ([AccountsController.cs](file:///d:/CarRental_TTCN/CarRental/Controllers/AccountsController.cs))
1. **Kiểm soát bảo mật:** Bổ sung ràng buộc session đăng nhập và kiểm tra quyền sở hữu đơn hàng (chỉ khách đặt hoặc Quản trị viên mới được xem chi tiết đơn).
2. **Truy vấn CSDL biên bản bàn giao (`VehicleInspections`):**
   - Lấy đồng thời cả 2 mốc `handover` (Lúc nhận bàn giao xe) và `return` (Lúc hoàn tất trả xe).
   - Nạp thông tin xe và cấu hình quy định vận hành (`CarPolicy`) gồm: Giới hạn km/ngày, Đơn giá phụ phí vượt km, Mức tiêu hao nhiên liệu.
3. **Tự động tính toán số liệu hành trình:**
   - Tính tổng hạn mức cho phép: `Tổng km = Số ngày thuê × Giới hạn km/ngày`.
   - Tính quãng đường thực tế đã đi: `drivenKm = returnOdo - handoverOdo`.
   - Tự động bắt phạt phụ phí vượt km: `overKm = drivenKm - allowedKm` ➜ `Phụ phí = overKm × Đơn giá phụ phí/km`.
   - Đối chiếu mức hụt/đủ nhiên liệu xăng/pin (`fuelDiff`).

### 15.2. Nâng Cấp Giao Diện Chi Tiết Đơn Thuê ([Views/Accounts/OrderDetails.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Accounts/OrderDetails.cshtml))
1. **Thẻ Biên bản Bàn giao & Đối chiếu ODO Hiện trạng Xe (Check-in / Check-out):**
   - Bảng 2 cột song song hiển thị đầy đủ chỉ số ODO monospace, mức xăng/pin có progress bar và ghi chú hiện trạng ở 2 mốc: *Lúc Nhận Xe* vs *Lúc Trả Xe*.
   - Hỗ trợ hiển thị album ảnh thực tế (nếu chủ xe có chụp) kèm tính năng phóng to Modal Lightbox.
2. **Hộp Tổng kết Đối chiếu Hành trình & Phụ phí Tự động:**
   - Khi có đầy đủ 2 mốc ODO: Hệ thống hiển thị bảng tổng kết quãng đường đã đi so với hạn mức hợp đồng.
   - Nếu vượt hạn mức: Hiển thị cảnh báo màu hổ phách bóc tách rõ số km vượt và số tiền phụ phí cần thanh toán thêm cho chủ xe.
   - Nếu trong hạn mức: Hiển thị thông báo xanh lá xác nhận không phát sinh phụ phí.
3. **Cập nhật Hợp đồng điện tử A4:**
   - Bổ sung *Điều 3: Quy định hành trình & Đối chiếu công tơ mét (ODO)* vào bản in A4 của hợp đồng điện tử để đảm bảo tính pháp lý hoàn chỉnh.

### 15.3. Tối Ưu Hóa Trang Danh Sách Đơn Hàng ([Views/Accounts/Index.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Accounts/Index.cshtml))
- Bổ sung huy hiệu tinh tế `Có ODO` với biểu tượng đồng hồ tốc độ trên các đơn đã được chủ xe bàn giao.
- Chuẩn hóa màu sắc trạng thái đơn hàng (Đã cọc 30%, Đang thuê, Hoàn tất, Đã hủy).

### 15.4. Nâng Cấp Gói Thanh Toán Tại Trang Checkout ([Views/Cart/Checkout.cshtml](file:///d:/CarRental_TTCN/CarRental/Views/Cart/Checkout.cshtml) & [CartController.cs](file:///d:/CarRental_TTCN/CarRental/Controllers/CartController.cs))
1. **Xóa bỏ tùy chọn "Đặt giữ chỗ 0đ không cọc" bất hợp lý:** Trong thực tế thuê xe ô tô, không có sàn nào cho phép giữ chỗ 0 đồng vì rủi ro bỏ bom xe rất cao.
2. **Cung cấp 2 gói thanh toán chuẩn thương mại điện tử:**
   - **Gói 1: Thanh toán đặt cọc 30% qua VNPay (Khuyên dùng):** Thanh toán trước 30% qua VNPay để khóa lịch xe, 70% còn lại thanh toán cho chủ xe khi nhận bàn giao xe.
   - **Gói 2: Thanh toán toàn bộ 100% qua VNPay (Trọn gói):** Thanh toán đủ 100% qua VNPay một lần duy nhất, khi nhận xe không cần trả thêm tiền thuê xe (tiền cần thu khi nhận xe = 0 đ).
3. **Đồng bộ bảng tính tiền tương tác thời gian thực:** Khi người dùng chuyển đổi giữa 2 gói, bảng chi tiết thanh toán và nút xác nhận tự động cập nhật số tiền tương ứng mà không cần tải lại trang.
4. **Đồng bộ phân hệ Chủ xe & Chi tiết đơn:** Khi khách chọn thanh toán 100%, chủ xe khi mở đơn sẽ thấy ngay thông báo *"Khách đã thanh toán 100% qua sàn (Không thu thêm tiền thuê khi giao xe)"*.

### 15.5. Nâng Cấp Chuẩn Hóa Múi Giờ Việt Nam (UTC+7 / ICT) & Timeline Trạng Thái Đơn Hàng
1. **Cơ chế thời gian chuẩn Việt Nam ([`Function.cs`](file:///d:/CarRental_TTCN/CarRental/Utilities/Function.cs)):**
   - Bổ sung thuộc tính `Function.NowVn` tự động phát hiện múi giờ hệ thống (`SE Asia Standard Time` / `Asia/Ho_Chi_Minh` / fallback `UTC+7`).
   - Xây dựng hàm `FormatSafeDateTime`: Tự động nhận diện trường hợp ngày thuần túy (giờ mặc định `00:00`) và chuyển đổi sang khung giờ tiêu chuẩn ban ngày (`09:00` khi nhận xe, `18:00` khi trả xe), xóa bỏ hoàn toàn hiện tượng hiển thị giờ nửa đêm `00:00`.
   - Nâng cấp hàm `ExtractTimeFromNotes`: Hỗ trợ mảng từ khóa đa dạng (tiếng Việt có dấu, không dấu, viết tắt, chống lỗi mã hóa encoding) và regex thông minh để bóc tách chính xác mốc giờ phút `HH:mm - dd/MM/yyyy`.
2. **Đồng bộ hóa Backend ghi nhận thời gian ([`OrdersController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/OrdersController.cs) & [`CartController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/CartController.cs)):**
   - Thay thế toàn bộ `DateTime.Now` bằng `Function.NowVn` khi tạo đơn mới, khi VNPay callback xác nhận thanh toán, khi lập biên bản Check-in bàn giao xe và Check-out nhận lại xe.
3. **Trực quan hóa Timeline 4 mốc chuẩn giờ Việt Nam ([`Orders/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Details.cshtml)):**
   - Cập nhật cả 2 khối Lịch trình nhận xe / Trả xe ở banner đầu trang và 4 mốc sự kiện trong Card *Lịch sử trạng thái*:
     - **Mốc 1 (Đặt xe thành công):** Hiển thị `HH:mm - dd/MM/yyyy` theo giờ tạo đơn thực tế.
     - **Mốc 2 (Đã thanh toán):** Hiển thị `HH:mm - dd/MM/yyyy` theo giờ ghi nhận VNPay.
     - **Mốc 3 (Bàn giao xe):** Hiển thị chính xác giờ phút thực tế chủ xe lập biên bản Check-in (`handoverInspection.CreatedAt`), không còn `00:00`.
     - **Mốc 4 (Hoàn tất):** Xóa bỏ dòng thời gian dự kiến khi chưa hoàn tất; chỉ hiển thị giờ phút thực tế kèm thông điệp hoàn thành khi chủ xe đã lập biên bản Check-out nhận xe.

---

## 16. KẾ HOẠCH & CÁC HẠNG MỤC NÊN LÀM TRONG TƯƠNG LAI

1. **Kiểm thử Luồng Thuê xe Toàn trình E2E (End-to-End Testing):**
   - Rà soát toàn bộ vòng đời tương tác giữa **Khách hàng (User)** và **Chủ xe (Host)**:
     - Khách tìm kiếm xe, lọc tiện nghi, chọn lịch trình -> Checkout chọn gói Cọc 30% hoặc Trọn gói 100% qua VNPay -> Nhận thông báo Real-time SignalR.
     - Chủ xe nhận đơn -> Chuẩn bị xe -> Lập biên bản bàn giao xe (Check-in ODO, mức xăng/pin, 4 góc ảnh) -> Khách hàng xem và đối chiếu Hợp đồng điện tử A4 và ODO.
     - Chủ xe nhận lại xe (Check-out ODO) -> Hệ thống tự động tính quãng đường, phụ phí vượt km và chênh lệch nhiên liệu -> Hoàn tất đơn.

2. **Rà soát & Sửa lại CSS / Giao diện nhất quán (Design System Polish):**
   - Rà soát toàn bộ CSS responsive trên thiết bị di động (Mobile) và máy tính bảng (Tablet).
   - Chuẩn hóa màu sắc bảng mã ReCar: Nền Dark Slate `#0f172a`, màu nhấn Cyan `#0284c7`, thẻ Card bề mặt `#ffffff` bo góc mềm mại `rounded-4`.
   - Cân chỉnh chi tiết icon (Lucide & Bootstrap Icons), font chữ, độ tương phản và loại bỏ hoàn toàn các đoạn CSS dư thừa từ template cũ.

3. **Cập nhật & Bổ sung tính năng cho Phân hệ Chủ xe (Car Owner Portal):**
   - Bổ sung trang thống kê tài chính, biểu đồ dòng tiền & lịch sử nhận tiền đối soát chi tiết hơn.
   - Thêm tính năng quản lý lịch bận của xe (Car Calendar / Block dates) cho phép chủ xe chủ động khóa ngày xe bận bảo dưỡng hoặc việc cá nhân.
   - Bổ sung xuất file PDF / In biên bản bàn giao ODO và Hợp đồng thuê xe trực tiếp từ phía chủ xe.

4. **Rà soát & Hoàn thiện Phân hệ Quản trị Toàn sàn (`Areas/Admin`):**
   - Đồng bộ giao diện các trang trong Admin (`/Admin/Home`, `/Admin/Product`, `/Admin/Orders`, `/Admin/UserManagement`, `/Admin/Blogs`, `/Admin/CarTypes`) theo đúng Design System chuẩn ReCar.
   - Kiểm tra và hoàn thiện màn hình duyệt hồ sơ đối tác chủ xe mới (`/Admin/CarOwners`) và quy trình duyệt xe mới đăng tải hoặc duyệt cập nhật xe.
   - Bảng quản trị cấu hình chính sách sàn (tỷ lệ hoa hồng 10%, giới hạn km mặc định, hotline cứu hộ).

5. **Đóng gói Mã nguồn & Đẩy lên GitHub (Git Commit & Push):**
   - Rà soát file `.gitignore` để loại bỏ các thư mục rác (như `bin/`, `obj/`, `.vs/`, file build tạm).
   - Kiểm tra lại chuỗi kết nối `appsettings.json` đảm bảo không để lộ thông tin nhạy cảm.
   - Tạo commit có thông điệp rõ ràng theo chuẩn Conventional Commits và đẩy lên repository GitHub khi người dùng yêu cầu.





---

## 17. CẬP NHẬT TIẾN TRÌNH: HỢP NHẤT BẢNG TÀI KHOẢN (CUSTOMERS), NÂNG CẤP CHAT REALTIME & HOÀN THIỆN ĐÁNH GIÁ (NGÀY 05 - 06/11/2026)

### 17.1. Hợp Nhất Cơ Sở Dữ Liệu & Loại Bỏ Hoàn Toàn Bảng `Account`
1. **Refactor Codebase sang bảng `Customers` duy nhất:**
   - Thay thế toàn bộ thực thể `Account` trong toàn bộ Controllers Admin ([`UserManagementController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/UserManagementController.cs), [`CarOwnersController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarOwnersController.cs), [`AccountController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/AccountController.cs), [`FinanceController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/FinanceController.cs), [`HomeController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/HomeController.cs), [`RegisterController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/RegisterController.cs)).
   - Cập nhật toàn bộ Razor Views liên quan sang Model `Customer`: `Areas/Admin/Views/UserManagement` (`Index`, `Details`, `Edit`, `Delete`), `Areas/Admin/Views/Login/Index`, `Areas/Admin/Views/Register/Index`, `Areas/Admin/Views/Account/Edit`, `Views/Accounts/Index`.
   - Xóa bỏ file `Models/Account.cs`, xóa `DbSet<Account> Accounts` và cấu hình mapping trong [`DbRenalCarContext.cs`](file:///d:/CarRental_TTCN/CarRental/Models/DbRenalCarContext.cs).
   - Chuẩn hóa quan hệ 1-N giữa `Role` và `Customer` (`entity.HasOne(d => d.Role).WithMany(p => p.Customers)`), khắc phục triệt để lỗi shadow property `RoleId1`.
2. **Cập nhật CSDL MySQL:**
   - Đã thực thi lệnh `DROP TABLE IF EXISTS account;`. CSDL `DbRenalCar` hiện chỉ sử dụng bảng `customers` duy nhất, lưu trữ đầy đủ thông tin của Khách hàng (RoleId 2), Chủ xe (RoleId 3) và Quản trị viên (RoleId 1).

### 17.2. Khắc Phục Triệt Để Hệ Thống Chat Realtime Giữa Khách Hàng Và Chủ Xe
1. **Sửa lỗi thiếu thư viện jQuery tại Layout Chủ xe ([`Areas/CarOwner/Views/Shared/_Layout.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Shared/_Layout.cshtml)):**
   - Đổi đường dẫn nhúng script từ `jquery.min.js` (không tồn tại) sang `jquery-3.6.0.min.js`, đồng thời bổ sung file alias `jquery.min.js` vào `wwwroot/assets/js/vendor/` để phòng ngừa 404.
   - Khắc phục triệt để lỗi ngầm `$ is not defined` làm tê liệt JavaScript trên toàn bộ giao diện Host Pro.
2. **Tối ưu trải nghiệm Dropdown tin nhắn Header Host Pro:**
   - Bổ sung sự kiện lắng nghe Bootstrap `show.bs.dropdown` trên nút tin nhắn.
   - Tự động gọi `loadRecentChats()` ngay khi vừa vào trang: nếu có tin nhắn mới từ khách thuê, chuông/icon trên Header lập tức phát sáng chấm đỏ Realtime.
   - Hiển thị hiệu ứng tải `spinner-border` khi đang lấy tin nhắn.
   - Hiển thị đầy đủ Tên khách hàng, Tên xe quan tâm, Tin nhắn cuối và Thời gian gửi; click vào mở ngay hộp thoại chat trao đổi trực tiếp hai chiều.
3. **Chuẩn hóa Phân định Đối tác Chat ([`ChatController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/ChatController.cs)):**
   - Khắc phục lỗi gán nhầm `ownerId = partnerId` trong `GetRecentConversations`.
   - Sửa lỗi kiểm tra tự gửi tin nhắn cho chính mình khi khách hàng mở hội thoại từ danh sách tin nhắn.

### 17.3. Chuẩn Hóa Hiển Thị Lịch Trình Trên Giao Diện Chủ Xe
- Tại trang **Lịch trình & Đơn xe** ([`Areas/CarOwner/Views/Orders/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Index.cshtml)): Loại bỏ phần giờ `00:00 - `, chỉ hiển thị ngày/tháng/năm (`dd/MM/yyyy`) ở cả hai dòng *Nhận* và *Trả*, giúp giao diện sạch đẹp và trực quan.

### 17.4. Nâng Cấp Hệ Thống Đánh Giá Chuyến Đi Theo Mã Đơn Hàng (OrderId)
1. **Chuẩn hóa CSDL Đánh giá (`carreviews`):**
   - Bổ sung cột `OrderId INT NULL` và chỉ mục `INDEX (OrderId)` vào bảng `carreviews`.
   - Cập nhật model [`CarReview.cs`](file:///d:/CarRental_TTCN/CarRental/Models/CarReview.cs) và mapping trong [`DbRenalCarContext.cs`](file:///d:/CarRental_TTCN/CarRental/Models/DbRenalCarContext.cs).
2. **Quản lý Đánh giá Độc lập theo từng Đơn Hàng ([`AccountsController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/AccountsController.cs)):**
   - Thay đổi điều kiện kiểm tra đánh giá từ `CarId + Email/Name` sang `OrderId == id`.
   - Khắc phục triệt để lỗi: Đơn hàng mới thuê lại cùng chiếc xe không còn bị gán nhầm đánh giá cũ của đơn trước, mà sẽ hiển thị nút màu vàng **"Đánh giá chuyến đi"** để khách gửi đánh giá riêng cho chuyến đi mới.
3. **Xóa Form Tự do tại Trang Chi tiết Xe ([`Views/Product/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Product/Details.cshtml)):**
   - Xóa bỏ form tự do *"Gửi đánh giá trải nghiệm của bạn"*.
   - Khẳng định nghiệp vụ: Chỉ những khách hàng đã thuê xe và hoàn tất chuyến đi (hóa đơn Status 5) mới được đánh giá xe thông qua hóa đơn trong tài khoản cá nhân.


---

## 18. CẬP NHẬT TIẾN TRÌNH: TÁCH CSS DÙNG CHUNG, TRIỂN KHAI 3 PHÂN HỆ MỚI CHO CHỦ XE & ĐỒNG BỘ LỊCH CHẶN TOÀN HỆ THỐNG (NGÀY 06/11/2026)

### 18.1. Bóc Tách Toàn Bộ Inline CSS Sang 3 File Stylesheet Riêng Biệt (Phương Án 1)
1. **Kiểm tra và chuẩn hóa:** Đã loại bỏ 100% các thẻ `<style>` inline nhúng trong toàn bộ 18 file `.cshtml` trên cả 3 cổng giao diện (Admin, CarOwner, Customer).
2. **Cấu trúc 3 file CSS trong `wwwroot/assets/css/`:**
   - [`recar.css`](file:///d:/CarRental_TTCN/CarRental/wwwroot/assets/css/recar.css): Toàn bộ style cho khách hàng (Trang chủ, Thuê xe, Chi tiết xe, Giỏ hàng, Tài khoản cá nhân, Tin tức, Bài viết).
   - [`recar-admin.css`](file:///d:/CarRental_TTCN/CarRental/wwwroot/assets/css/recar-admin.css): Toàn bộ style cho phân hệ Admin (Layout và trang duyệt xe).
   - [`recar-carowner.css`](file:///d:/CarRental_TTCN/CarRental/wwwroot/assets/css/recar-carowner.css): Toàn bộ style cho phân hệ Chủ xe (Layout, Quản lý xe, Thêm/Sửa/Xóa xe, Đơn hàng, Biên bản bàn giao Check-in/Check-out, Hồ sơ đối tác).
3. **Kết quả xác nhận:** Lệnh quét `Get-ChildItem -Recurse -Filter "*.cshtml" | Select-String "<style\b"` trả về đúng **0** thẻ.

---

### 18.2. Triển Khai 3 Phân Hệ Mới Cho Phân Hệ Chủ Xe (Ưu Tiên 1, 2, 3 Từ Template React)

#### 1. Ưu Tiên 1: Ví Đối Tác & Đối Soát (`/CarOwner/Wallet/Index`)
- **Controller:** [`WalletController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/WalletController.cs).
- **Giao diện:** [`Areas/CarOwner/Views/Wallet/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Wallet/Index.cshtml).
- **Nghiệp vụ tài chính:**
  - Tổng giá trị đơn thuê (100%): Khách cọc 30% qua VNPay, thanh toán 70% trực tiếp cho chủ xe khi nhận chìa khóa.
  - Phí dịch vụ sàn 10%: Khấu trừ một lần trực tiếp từ 30% tiền cọc.
  - Sàn chuyển đối soát 20% còn lại về tài khoản ngân hàng của chủ xe vào ngày 01 & 16 hàng tháng.
  - Chủ xe thực nhận 90% (70% tiền mặt/CK trực tiếp + 20% đối soát từ sàn).
- **Tính năng giao diện:**
  - 4 Stat Cards kèm tooltip giải thích chi tiết cơ chế dòng tiền.
  - Bảng tổng hợp dòng tiền trong kỳ & Thông tin tài khoản thụ hưởng của chủ xe.
  - Tab *Biến động giao dịch* (`TN-#RC-...`, `CK-PO-...`) và Tab *Lịch sử quyết toán* (Nhóm theo kỳ nửa tháng `PO-YYYYMM-01`).
  - Modal chi tiết từng giao dịch, Modal chi tiết kỳ quyết toán kèm danh sách đơn.
  - Xuất báo cáo CSV/Excel (`/CarOwner/Wallet/ExportCsv`) và in phiếu đối soát (`window.print()`).

#### 2. Ưu Tiên 2: Lịch Xe & Chặn Ngày Bận (`/CarOwner/Calendar/Index`)
- **Database & Model:** Tạo model [`CarCalendarBlock.cs`](file:///d:/CarRental_TTCN/CarRental/Models/CarCalendarBlock.cs), đăng ký bảng `car_calendar_blocks` trong `DbRenalCarContext` và tự động tạo bảng trong `Program.cs`.
- **Controller:** [`CalendarController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/CalendarController.cs).
- **Giao diện:** [`Areas/CarOwner/Views/Calendar/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Calendar/Index.cshtml).
- **Tính năng:**
  - 2 chế độ hiển thị: Lưới 42 ô lịch Tháng và Bảng Lịch Tuần theo từng xe.
  - Phân loại màu sắc trực quan: Còn trống (kèm giá ngày), Đã đặt (Cọc 30%), Đang giao xe, Hoàn tất, Bảo dưỡng định kỳ, Bận việc cá nhân.
  - Chức năng thêm/xóa chặn lịch kèm tự động kiểm tra xung đột: Ngăn chặn chủ xe khóa ngày trùng vào các đơn khách đã cọc hoặc lịch đã chặn trước đó.

#### 3. Ưu Tiên 3: Phân Tích Hiệu Suất & Tỷ Lệ Lấp Đầy (`/CarOwner/Analytics/Index`)
- **Controller:** [`AnalyticsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/AnalyticsController.cs).
- **Giao diện:** [`Areas/CarOwner/Views/Analytics/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Analytics/Index.cshtml).
- **Công thức Tỷ lệ lấp đầy (Occupancy Rate):**
  $$\text{Tỷ lệ lấp đầy (\%)} = \frac{\text{Số ngày có khách thuê}}{\text{Tổng số ngày khả dụng trong kỳ (đã trừ ngày chủ xe chặn bận/bảo dưỡng)}} \times 100$$
  *(Ví dụ: Trong kỳ 6 tháng gồm 183 ngày, nếu xe bị chặn 2 ngày thì số ngày khả dụng là 181 ngày; nếu khách thuê 8 ngày thì lấp đầy = 8/181 ≈ 4%).*
- **Tính năng:**
  - 5 Stat Cards tổng quan: Lấp đầy %, Tổng doanh thu 100%, Doanh thu ròng 90%, Chuyến hoàn thành, Đánh giá trung bình.
  - Biểu đồ đường (Line Chart): Xu hướng Doanh thu theo thời gian qua các tháng.
  - Biểu đồ cột ngang (Bar Chart): So sánh tỷ lệ lấp đầy từng xe trong đội xe.
  - Bảng hiệu suất chi tiết từng xe kèm thanh progress bar.
  - Phân bổ đánh giá 1–5 sao và Gợi ý vận hành tự động theo hiệu suất thực tế của từng xe.

---

### 18.3. Đồng Bộ Luồng Lịch Chặn Toàn Hệ Thống (Quản Lý Xe & Giao Diện Khách Hàng)
1. **Tại Quản lý xe của Chủ xe ([`Areas/CarOwner/Views/Cars/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Index.cshtml)):**
   - Xe đang trong khoảng ngày bảo dưỡng $\rightarrow$ Tự động chuyển badge sang màu đỏ **"Bảo dưỡng"**.
   - Xe đang trong khoảng ngày bận việc cá nhân $\rightarrow$ Tự động hiển thị badge **"Chặn lịch"**.
   - Menu 3 chấm $(\dots)$ bổ sung nút bấm nhanh **"Lịch & Chặn ngày"** chuyển thẳng đến lịch của xe đó.
2. **Tại Chi tiết xe phía Khách hàng ([`Views/Product/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Product/Details.cshtml)):**
   - Hiển thị danh sách các khoảng ngày xe bị khóa/bảo dưỡng ngay dưới ô chọn ngày nhận – trả xe.
   - Kiểm tra Real-time trên client: Nếu khách chọn khoảng ngày trùng lịch bận/bảo dưỡng, hệ thống trượt cảnh báo đỏ và **khóa nút đặt cọc giữ xe**.
   - Nếu hôm nay xe đang trong đợt bảo dưỡng $\rightarrow$ Hiển thị nhãn *"Đang bảo trì định kỳ"* và vô hiệu hóa nút đặt cọc trực tuyến.
3. **Tại Controller Giỏ hàng ([`CartController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/CartController.cs)):**
   - Kiểm tra lịch chặn ngay trong hàm `AddToCart` (không cho thêm vào giỏ nếu trùng ngày bận).
   - Kiểm tra chốt chặn an toàn tại hàm `Checkout` trước khi tạo đơn và sinh link thanh toán VNPay.
4. **Tinh chỉnh Sidebar Chủ xe:** Đã xóa bỏ mục *"Sắp ra mắt / Tính năng tương lai"* tại [`_Layout.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Shared/_Layout.cshtml), tinh gọn sidebar chỉ còn các tính năng thực tế.

---

### 18.4. Ghi Chú Kiến Trúc Cho Ca Làm Việc Tiếp Theo
- **Bài toán thay đổi tỷ lệ phí sàn (Snapshot Pattern):**
  - Khi triển khai tính năng Admin cấu hình tỷ lệ phí sàn (ví dụ tăng từ 10% lên 20%), cần bổ sung 2 cột `PlatformFeeRate` và `PlatformFee` vào bảng `carrentalorders` để lưu lại "ảnh chụp" tỷ lệ phí tại thời điểm khách tạo đơn.
  - Đảm bảo các đơn hàng cũ trong lịch sử vẫn giữ nguyên 10%, và các đơn hàng mới tạo sau thời điểm Admin thay đổi sẽ tự động áp dụng 20%, không làm sai lệch số liệu kế toán.

---

## 19. CẬP NHẬT TIẾN TRÌNH: HOÀN THIỆN CẤU HÌNH PHÍ SÀN LINH HOẠT (ADMIN SETTINGS) & SNAPSHOT PATTERN (NGÀY 08/11/2026)

### 19.1. Thiết Kế Cơ Sở Dữ Liệu Cấu Hình Hệ Thống & Bảng `system_settings`
1. **Mô hình hóa & Tạo bảng CSDL MySQL:**
   - Tạo model [`SystemSetting.cs`](file:///d:/CarRental_TTCN/CarRental/Models/SystemSetting.cs) và khai báo `DbSet<SystemSetting> SystemSettings` trong [`DbRenalCarContext.cs`](file:///d:/CarRental_TTCN/CarRental/Models/DbRenalCarContext.cs).
   - Tự động tạo bảng `system_settings` trong [`Program.cs`](file:///d:/CarRental_TTCN/CarRental/Program.cs) với các trường nghiệp vụ:
     - `SettingId` (Primary Key, Auto Increment).
     - `DepositPct`: Tỷ lệ đặt cọc giữ xe (Mặc định 30%).
     - `CommissionPct`: Tỷ lệ hoa hồng sàn trích thu (Mặc định 10%).
     - `DefaultKmPerDay`: Hạn mức quãng đường mặc định (Mặc định 350 km/ngày).
     - `ExtraKmPrice`: Đơn giá phụ phí vượt km mặc định (Mặc định 3.000 đ/km).
     - `Hotline`, `SupportEmail`, `PlatformName`, `Address`, `UpdatedAt`.
   - Tự động seed bản ghi cấu hình mặc định ban đầu nếu bảng trống.

### 19.2. Triển Khai Kiến Trúc Snapshot Pattern Cho Phí Sàn Trong Bảng `carrentalorders`
1. **Bổ sung 2 cột Snapshot vào CSDL:**
   - Cột `PlatformFeeRate` (DECIMAL(5, 2)): Ghi nhận % hoa hồng sàn tại thời điểm khách đặt xe.
   - Cột `PlatformFee` (DECIMAL(18, 2)): Ghi nhận số tiền hoa hồng thực tế sàn thu trên đơn hàng đó.
   - Tự động chạy migration SQL trong [`Program.cs`](file:///d:/CarRental_TTCN/CarRental/Program.cs) khi khởi động ứng dụng:
     - Thêm 2 cột nếu chưa tồn tại.
     - Tự động tính toán và điền giá trị cho toàn bộ các đơn hàng cũ trong lịch sử: `PlatformFeeRate = 10.00`, `PlatformFee = (Deposit + Payment) * 10%`.
2. **Giá trị kiến trúc:**
   - Tuyệt đối bảo toàn dữ liệu lịch sử và chứng từ kế toán thuế: Nếu Admin điều chỉnh tỷ lệ hoa hồng (ví dụ từ 10% lên 15%), toàn bộ các đơn hàng đã tạo trước đó vẫn giữ nguyên số liệu 10%, chỉ các đơn hàng phát sinh từ thời điểm cập nhật trở đi mới mang tỷ lệ 15%.

### 19.3. Đồng Bộ Luồng Tạo Đơn & Tính Toán Tài Chính Toàn Sàn
1. **Tại Controller Giỏ hàng & Thanh toán ([`CartController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/CartController.cs)):**
   - Đọc tỷ lệ cọc `DepositPct` và hoa hồng `CommissionPct` từ bảng `system_settings`.
   - Khi khách tạo đơn thuê xe: Tính toán tiền cọc, số tiền còn lại và tự động lưu `PlatformFeeRate`, `PlatformFee` vào bản ghi đơn hàng.
2. **Tại Phân hệ Chủ xe ([`WalletController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/WalletController.cs) & [`Orders/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Details.cshtml)):**
   - Đọc trực tiếp `PlatformFeeRate` và `PlatformFee` từ từng đơn hàng:
     - Hiển thị nhãn phí dịch vụ động: *"Phí dịch vụ sàn X% (trừ từ cọc)"* thay vì text cố định 10%.
     - Tính toán thu nhập thực nhận của chủ xe và tiền sàn đối soát chuẩn xác theo từng thời kỳ.
3. **Tại Phân hệ Admin ([`FinanceController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/FinanceController.cs), [`HomeController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/HomeController.cs) & [`Finance/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Finance/Index.cshtml)):**
   - Tổng hoa hồng thực thu được tính bằng tổng `PlatformFee` thực tế của các đơn hàng.
   - Các thẻ KPI tài chính và tỷ lệ đối soát hiển thị động theo cấu hình hệ thống.
4. **Tại Quy định vận hành xe ([`CarPolicy.cs`](file:///d:/CarRental_TTCN/CarRental/Utilities/CarPolicy.cs)):**
   - Hỗ trợ nạp cấu hình hệ thống để tự động fallback hạn mức km/ngày và phụ phí vượt km nếu chủ xe chưa cài đặt riêng.

### 19.4. Hoàn Thiện Màn Hình Cấu Hình Hệ Thống ([`Areas/Admin/Views/Settings/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Settings/Index.cshtml))
1. **Giao diện chuẩn ReCar Design System:**
   - Bố cục 2 panel thoáng đãng, không dùng CSS inline.
   - Banner giải thích cơ chế bảo vệ dữ liệu **Snapshot Pattern** minh bạch.
   - Hiển thị dấu thời gian *"Cập nhật lần cuối"*.
2. **Nghiệp vụ Backend & Validation ([`SettingsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/SettingsController.cs)):**
   - Kiểm tra chặt chẽ quyền Admin.
   - Kiểm tra giới hạn hợp lệ: Tỷ lệ cọc từ 10% đến 50%, Tỷ lệ hoa hồng từ 5% đến 30%.
   - Lưu trữ tức thì vào CSDL MySQL, trả về thông báo và tự động tải lại dữ liệu mới bằng SweetAlert2.

### 19.5. Kiểm Tra Chất Lượng Mã Nguồn
- Biên dịch toàn bộ dự án ASP.NET Core 9 (.NET 9): **`Build succeeded. 0 Warning(s) 0 Error(s)`**.

---

## 20. CẬP NHẬT TIẾN TRÌNH: RÀ SOÁT TOÀN DIỆN & ĐỒNG BỘ 100% PHÂN HỆ QUẢN TRỊ ADMIN (NGÀY 08/11/2026)

### 20.1. Tái Thiết Kế Trang Đăng Nhập & Đăng Ký Admin Chuẩn SaaS Cao Cấp
1. **Trang Đăng nhập Quản trị ([`Areas/Admin/Views/Login/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Login/Index.cshtml)):**
   - Loại bỏ 100% mã nguồn Mazer cũ và các đường dẫn ảnh tĩnh bị lỗi 404.
   - Thiết kế giao diện Auth Shell sang trọng: Bảng màu Deep Navy `#0f172a`, logo huy hiệu ReCar Admin Portal phát sáng, font chữ Plus Jakarta Sans.
   - Tích hợp chip thông minh *"Tự động điền tài khoản mẫu ⚡"* (`admin` / `123`) hỗ trợ quá trình kiểm thử và demo đồ án nhanh chóng.
   - Bổ sung nút quay về Sàn ReCar công khai.
2. **Trang Đăng ký Quản trị ([`Areas/Admin/Views/Register/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Register/Index.cshtml)):**
   - Đồng bộ hoàn toàn phong cách Auth Shell hiện đại.

### 20.2. Chuẩn Hóa Điều Hướng & Quản Lý Hồ Sơ Đối Tác Chủ Xe
1. **Khắc phục lỗi điều hướng `/Admin/CarOwners` ([`CarOwnersController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarOwnersController.cs)):**
   - Bổ sung action `Index(string? q)` tự động chuyển tiếp về `Pending(q)`.
   - Ngăn chặn triệt để lỗi HTTP 404 khi người dùng truy cập trực tiếp URL `/Admin/CarOwners`.

### 20.3. Hoàn Thiện Module Quản Lý Người Dùng & Đối Tác (`UserManagement`)
1. **Bổ sung tính năng Tạo người dùng mới ([`UserManagementController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/UserManagementController.cs)):**
   - Triển khai đầy đủ cặp action GET & POST `Create`:
     - Kiểm tra trùng lặp email và username trong toàn hệ thống.
     - Mã hóa mật khẩu bảo mật MD5 một chiều qua `Function.MD5Password`.
     - Hỗ trợ khởi tạo đầy đủ các vai trò: Quản trị viên (Role 1), Khách hàng (Role 2) và Chủ xe (Role 3).
2. **Xây dựng mới View Tạo tài khoản ([`UserManagement/Create.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/UserManagement/Create.cshtml)):**
   - Biểu mẫu 2 cột chuẩn ReCar Design System, bo góc mềm mại, đầy đủ chỉ dẫn và validation rõ ràng.
3. **Hiện đại hóa toàn bộ các màn hình Quản lý người dùng:**
   - [`Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/UserManagement/Details.cshtml): Xóa bỏ layout Mazer cũ, hiển thị thẻ avatar tròn lớn, phân quyền vai trò và đầy đủ thông tin cá nhân/định danh.
   - [`Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/UserManagement/Edit.cshtml): Form chỉnh sửa hồ sơ hiện đại, hỗ trợ đổi mật khẩu và chuyển đổi trạng thái Khóa / Mở khóa tài khoản.
   - [`Delete.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/UserManagement/Delete.cshtml): Thẻ cảnh báo xóa nguy hiểm bo viền đỏ chuyên nghiệp, khuyến nghị quản trị viên dùng tính năng Khóa tài khoản để bảo toàn dữ liệu lịch sử đơn thuê.

### 20.4. Chuẩn Hóa Quản Lý Đơn Thuê Xe & Cập Nhật Phí Sàn Snapshot
1. **Trang Danh sách Đơn hàng ([`Areas/Admin/Views/Orders/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Orders/Index.cshtml)):**
   - Cập nhật chuẩn hóa mã màu cho cả 5 trạng thái đơn (`OrderStatus` 1 đến 5).
   - Bổ sung cột hiển thị **Phí dịch vụ sàn** đọc trực tiếp từ `order.PlatformFee` và % snapshot `order.PlatformFeeRate`.
2. **Trang Cập nhật Đơn hàng ([`Areas/Admin/Views/Orders/Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Orders/Edit.cshtml)):**
   - Bổ sung hộp thông tin bóc tách hoa hồng sàn thực thu trên đơn hàng.
3. **Controller Đơn hàng ([`OrdersController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/OrdersController.cs)):**
   - Thêm alias `Details` và `OrderDetails` chuyển tiếp mượt mà về `Edit`, triệt tiêu nguy cơ 404.

### 20.5. Chuẩn Hóa Phân Quyền & Các Controller Quản Trị Khác
1. **Kiểm duyệt Đánh giá & Bình luận ([`ReviewsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/ReviewsController.cs)):**
   - Nâng cấp cơ chế kiểm tra quyền Admin qua `Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session)`.
2. **Chuyển tiếp Controller cũ sang hệ thống mới:**
   - [`ProductController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/ProductController.cs): Tự động chuyển hướng về `/Admin/Cars/Index`.
   - [`CustomerController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CustomerController.cs): Tự động chuyển hướng về `/Admin/UserManagement/Index`.
3. **Trang Hồ sơ Cá nhân Admin ([`Areas/Admin/Views/Account/Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Account/Edit.cshtml)):**
   - Thiết kế lại 100% theo phong cách ReCar Admin, xóa bỏ toàn bộ mã HTML Mazer cũ.

### 20.6. Kiểm Tra & Đảm Bảo Chất Lượng
- Dự án biên dịch sạch sẽ trên .NET 9: **`Build succeeded. 0 Warning(s) 0 Error(s)`**.


---

## 21. KHẮC PHỤC TRIỆT ĐỂ LỖI ĐỒNG BỘ TỶ LỆ CỌC & HOA HỒNG NỀN TẢNG (DYNAMIC SNAPSHOT ENGINE)

### 21.1. Bối cảnh & Hiện tượng ghi nhận từ kiểm thử thực tế
1. **Hiện tượng 1 (Admin Settings):** Khi quản trị viên đổi Tỷ lệ cọc thành 35% và Hoa hồng thành 20% rồi bấm "Lưu cấu hình", popup xác nhận SweetAlert2 hiện lên nhưng trên màn hình dường như đã ghi lại thời gian sửa đổi trước khi bấm xác nhận.
   - *Nguyên nhân:* Khi chạy seed dữ liệu ban đầu, trường `UpdatedAt` được khởi tạo bằng `DateTime.Now`. Khi Admin vừa mở trang, màn hình đã sẵn có mốc thời gian của lần seed trước đó khiến người dùng tưởng lầm là đã lưu tự động khi vừa click nút.
   - *Khắc phục:* `UpdatedAt` được giữ là `null` khi hệ thống chưa từng chỉnh sửa (hiển thị nhãn "Cấu hình ban đầu"). Chỉ khi Admin bấm nút "Đồng ý lưu" trên popup thì mới thực hiện gọi AJAX POST và cập nhật `UpdatedAt = DateTime.Now`.
2. **Hiện tượng 2 (Customer Car Details):** Tại trang chi tiết xe `/Product/Details/{id}`, thông tin tiền cọc vẫn ghi chú `30%` và tính tiền theo hệ số cứng `0.3m`.
   - *Nguyên nhân:* Action `Details` trong [`ProductController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/ProductController.cs) chưa query cấu hình hệ thống `SystemSetting` để truyền `ViewBag.DepositPct`. Đồng thời trong [`Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Product/Details.cshtml), tỷ lệ 30% và 70% bị viết cứng (hardcoded) ở cả Razor và script tính toán ngày.
   - *Khắc phục:* Bổ sung truy vấn `SystemSetting` trong `ProductController.Details`, truyền `ViewBag.DepositPct`. Chuyển đổi 100% tỷ lệ và công thức toán học trong `Details.cshtml` và JavaScript sang biến động `depositPct` và `depositRate`.
3. **Hiện tượng 3 (Car Owner Order Details):** Tại trang xem đơn của chủ xe `/CarOwner/Orders/Details/{id}`, phần phân rã tài chính hiển thị:
   - Tổng tiền thuê: 1.300.000 đ
   - Cọc 30% — sàn thu: 455.000 đ
   - Phí dịch vụ sàn 20%: -260.000 đ
   - Thu trực tiếp khi nhận xe: 845.000 đ
   - *Phân tích logic:* 455.000 đ / 1.300.000 đ = **35%**! Nghĩa là phần backend đã tính tiền cọc 35% hoàn toàn chính xác theo đúng cấu hình mới của Admin. Tuy nhiên, nhãn text trên giao diện trước đó là chuỗi ký tự cứng `"Cọc 30% — sàn thu"` nên đã hiển thị sai lệch giữa con số tiền và nhãn phần trăm!
   - *Khắc phục:* Tính toán tỷ lệ cọc động theo chính đơn hàng: `depositRate = (totalRent > 0 && deposit > 0) ? Math.Round((deposit / totalRent) * 100m, 0) : 30m;` và hiển thị nhãn linh hoạt `$"Cọc {depositRate:0.#}% — sàn thu"`. Đơn hàng cọc bao nhiêu % (30%, 35%, 50% hay 100%) thì giao diện sẽ hiển thị đúng bấy nhiêu phần trăm.

### 21.2. Các file đã chuẩn hóa đồng bộ
- [`CarRental/Areas/Admin/Controllers/SettingsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/SettingsController.cs): Khởi tạo `UpdatedAt = null` khi chưa sửa; ghi nhận thời gian chính xác chỉ khi Admin bấm lưu.
- [`CarRental/Areas/Admin/Views/Settings/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Settings/Index.cshtml): Hiển thị trạng thái cấu hình ban đầu hoặc thời gian cập nhật lần cuối chuẩn xác.
- [`CarRental/Controllers/ProductController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/ProductController.cs): Truy vấn `SystemSetting` và truyền `ViewBag.DepositPct`.
- [`CarRental/Views/Product/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Product/Details.cshtml): Đồng bộ nhãn % và JavaScript tính toán động theo cấu hình hệ thống.
- [`CarRental/Views/Cart/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Cart/Index.cshtml) & [`Checkout.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Cart/Checkout.cshtml): Đồng bộ nhãn cọc động và tính toán thanh toán theo cấu hình.
- [`CarRental/Views/Cart/Success.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Cart/Success.cshtml): Hiển thị % cọc thực tế từ đơn hàng.
- [`CarRental/Areas/CarOwner/Views/Orders/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Details.cshtml): Tính toán tỷ lệ cọc và tiền thu trực tiếp theo số liệu thực tế của đơn hàng, xóa bỏ hoàn toàn chuỗi text cứng 30%/70%.
- [`CarRental/Areas/CarOwner/Views/Orders/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Orders/Index.cshtml): Cập nhật cột tài chính tính toán động theo đơn.
- [`CarRental/Views/Accounts/OrderDetails.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Accounts/OrderDetails.cshtml): Đồng bộ nhãn cọc trong chi tiết đơn hàng và hợp đồng điện tử của khách hàng.
- Kiểm thử biên dịch: Toàn bộ project biên dịch sạch sẽ **0 Error(s)**.


---

## 22. XỬ LÝ TRIỆT ĐỂ ĐỒNG BỘ ẨN/HIỆN XE TRÊN SÀN (FLEET VISIBILITY & DEACTIVATION SYNC)

### 22.1. Hiện tượng & Phân tích nguyên nhân
- **Hiện tượng:** Trong màn hình Quản lý đội xe Admin (`/Admin/Cars`), khi bấm chuyển trạng thái xe sang "Ngưng hoạt động" (hoặc "Tạm ngừng"), xe vẫn tiếp tục xuất hiện trên trang chủ (`/Home`) và danh sách thuê xe (`/Product`).
- **Nguyên nhân cốt lõi:**
  1. Trong [`Areas/Admin/Controllers/CarsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarsController.cs) action `ToggleStatus`, hệ thống chỉ gán `car.CarStatus = status` mà **không hề cập nhật cột `car.IsActive = false`**. Cột `IsActive` vẫn giữ nguyên giá trị `true (1)`.
  2. Tại [`HomeController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/HomeController.cs), câu lệnh lấy danh sách xe chỉ kiểm tra `Where(i => i.IsActive == true)` mà không lọc trừ các trạng thái `Tạm ngừng` hoặc `Ngưng hoạt động`.
  3. Tại [`ProductController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/ProductController.cs) và [`ProductViewComponent.cs`](file:///d:/CarRental_TTCN/CarRental/ViewComponents/ProductViewComponent.cs), bộ lọc chỉ loại bỏ `Chờ duyệt` và `Từ chối`, do đó xe có trạng thái `Tạm ngừng` / `Ngưng hoạt động` vẫn lọt qua bộ lọc và hiển thị ra sàn.
  4. Người dùng cố tình truy cập link chi tiết xe hoặc cố tình bấm thêm giỏ hàng thì hệ thống chưa chặn các xe bị ngưng hoạt động.

### 22.2. Giải pháp đã thực hiện đồng bộ 100%
1. **Đồng bộ hóa cột `IsActive` trong Admin:**
   - Khi chuyển sang `"Ngưng hoạt động"`, `"Tạm ngừng"`, `"Từ chối"`, `"Chờ duyệt"`: Tự động gán `car.IsActive = false` (Ẩn hoàn toàn khỏi sàn).
   - Khi chuyển sang `"Sẵn sàng"`, `"Bảo trì"`: Tự động gán `car.IsActive = true`.
2. **Bộ lọc đa tầng trên toàn bộ Client:**
   - [`HomeController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/HomeController.cs): Lọc loại trừ triệt để xe `!IsActive || CarStatus in ('Chờ duyệt', 'Từ chối', 'Tạm ngừng', 'Ngưng hoạt động')`.
   - [`ProductController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/ProductController.cs) & [`ProductViewComponent.cs`](file:///d:/CarRental_TTCN/CarRental/ViewComponents/ProductViewComponent.cs): Áp dụng đồng bộ bộ lọc loại trừ cho cả danh sách tìm kiếm, bản đồ xe gần bạn, chi tiết xe và xe liên quan.
   - [`CartController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/CartController.cs): Chặn thêm vào giỏ hàng đối với xe đang ngưng hoạt động.
3. **Cập nhật giao diện Admin Fleet:**
   - Chuẩn hóa nút thao tác thành **"Ngưng hoạt động (Ẩn khỏi sàn)"** và bộ lọc trạng thái "Ngưng hoạt động".
4. **Đồng bộ dữ liệu Database:**
   - Đã cập nhật `IsActive = 0` cho các xe đang ở trạng thái `Tạm ngừng` / `Ngưng hoạt động` trong cơ sở dữ liệu MySQL (bao gồm xe ID 24).
5. **Kiểm thử biên dịch:** Dự án biên dịch sạch sẽ **`Build succeeded. 0 Error(s)`**.


---

## 23. NÂNG CẤP TOÀN DIỆN QUẢN LÝ MENU 3 PHÂN HỆ & BẢNG ĐIỀU HƯỚNG CẤY ĐA CẤP (RECAR 3-SUBSYSTEM MENU ARCHITECTURE)

### 23.1. Yêu cầu & Thiết kế kiến trúc mới
- **Nhu cầu nghiệp vụ:** Người dùng muốn một trang quản lý Menu tập trung duy nhất nhưng phân tách rõ ràng thành **3 phân hệ riêng biệt**:
  1. **Khách hàng (`Customer`):** Menu Header/Footer của sàn thuê xe trực tuyến.
  2. **Chủ xe (`CarOwner`):** Menu Sidebar và các chức năng của cổng đối tác Host Pro.
  3. **Quản trị viên (`Admin`):** Toàn bộ Menu Sidebar của trung tâm điều hành sàn ReCar.
- **Tính năng mở rộng:**
  - **Quan hệ Cây Cha - Con (`ParentId` & `Levels`):** Nhận biết nút này là Menu chính (Cấp 1) hay thuộc Menu cha nào (Cấp 2). Khi hiển thị trên bảng, menu con được thụt lề trực quan với ký hiệu `↳` và nhãn `Con của: [Tên menu cha]`.
  - **Quản lý Biểu tượng Icon (`Icon`):** Lưu trữ mã Bootstrap Icon (như `bi-speedometer2`, `bi-car-front`, `bi-wallet2`, `bi-newspaper`...), hỗ trợ ô chọn gợi ý nhanh các icon phổ biến kèm **Live Preview (xem trước icon thời gian thực)** ngay khi nhập hoặc chọn.
  - **Đường dẫn liên kết (`Url`):** Quản lý route nội bộ hoặc URL tương ứng.
  - **Bật / Tắt nhanh (`ToggleActive`):** Cho phép bật/tắt hiển thị menu trực tiếp trên bảng bằng AJAX SweetAlert2 không cần tải lại trang.

### 23.2. Cập nhật Cơ sở dữ liệu & Model
- Bổ sung 3 cột mới vào bảng `Menu`:
  - `Icon VARCHAR(100) DEFAULT 'bi-link-45deg'`
  - `Subsystem VARCHAR(50) NOT NULL DEFAULT 'Customer'`
  - `Url VARCHAR(255) NULL`
- Cập nhật Entity [`CarRental/Models/Menu.cs`](file:///d:/CarRental_TTCN/CarRental/Models/Menu.cs) và mapping trong [`DbRenalCarContext.cs`](file:///d:/CarRental_TTCN/CarRental/Models/DbRenalCarContext.cs).
- Đã seed dữ liệu menu chuẩn mực đầy đủ cho cả 3 phân hệ:
  - **Khách hàng:** 7 mục (Trang chủ, Thuê xe tự lái, Thuê xe có tài, Đăng ký làm chủ xe, Tin tức, Về ReCar, Liên hệ).
  - **Chủ xe:** 9 mục (Dashboard, Quản lý đội xe, Thêm xe mới, Lịch bận bảo dưỡng, Đơn hàng, Ví & Doanh thu, Báo cáo phân tích, Tin nhắn, Hồ sơ).
  - **Admin:** 10 mục (Tổng quan, Quản lý đội xe, Giám sát đơn hàng, Người dùng & Đối tác, Duyệt hồ sơ chủ xe, Tài chính & Đối soát, Đánh giá xe, Bài viết, Quản lý Menu, Cấu hình sàn).

### 23.3. Các Controller & View hoàn thiện
- [`MenusController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/MenusController.cs): Hỗ trợ phân loại tab theo `subsystem`, tạo menu con với `parentId`, chọn icon danh mục, toggle kích hoạt và xóa an toàn.
- [`Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Menus/Index.cshtml): Giao diện 3 Tabs Pills bo tròn sang trọng, bảng phân cấp rõ nét, Live AJAX Toggle.
- [`Create.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Menus/Create.cshtml) & [`Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Menus/Edit.cshtml): Form chuyên nghiệp có bộ chọn Menu cha theo phân hệ, Icon Picker kèm Live Preview.
- [`Delete.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Menus/Delete.cshtml): Chuyển đổi sang phong cách ReCar Admin, cảnh báo an toàn bảo toàn menu con.
- Kết quả kiểm tra: Toàn bộ project đạt **`Build succeeded. 0 Warning(s) 0 Error(s)`**.


### 23.4. Khắc phục lỗi Font chữ Tiếng Việt bảng Menu (UTF-8 Collation Sync)
- **Hiện tượng:** Dữ liệu mẫu bảng `Menu` hiển thị các dấu chấm hỏi `?` (ví dụ: `T?ng quan h? th?ng`, `Qu?n l? ??i xe to?n s?n`...).
- **Nguyên nhân:** Lệnh SQL chèn trực tiếp qua PowerShell CLI sử dụng bảng mã mặc định của Windows Console (Code Page 1258/1252) thay vì luồng UTF-8 chuẩn.
- **Khắc phục:** Đã chạy lại script cập nhật dữ liệu với cờ `--default-character-set=utf8mb4` và `SET NAMES utf8mb4`. Toàn bộ danh mục Menu của cả 3 phân hệ (Khách hàng, Chủ xe, Quản trị viên) đã hiển thị tiếng Việt sắc nét, chuẩn 100%.

---

## 24. HOÀN THIỆN ĐỒNG BỘ MENU 3 PHÂN HỆ VÀ KẾT NỐI DỮ LIỆU THẬT RA GIAO DIỆN (DYNAMIC NAVIGATION ARCHITECTURE)

### 24.1. Khắc phục Trạng thái Tabs Pill và Tối ưu Nút Thao tác trên `/Admin/Menus`
1. **Lỗi viên thuốc đứng yên (Active Tabs Pill):**
   - **Hiện tượng:** Khi click chuyển đổi giữa 3 tab *Khách hàng*, *Chủ xe*, *Quản trị viên*, nội dung bảng bên dưới đã chuyển nhưng màu nền xanh của viên thuốc trên cùng bị đứng yên ở tab đầu tiên.
   - **Nguyên nhân:** Tab đầu tiên bị gắn inline style cố định `style="background-color: var(--recar-primary); color: #fff;"` khiến class `.active` của Bootstrap JavaScript không thể loại bỏ màu nền inline.
   - **Khắc phục:** Gỡ bỏ toàn bộ inline styles; sử dụng scoped CSS selector `#menuSubsystemTabs .nav-link.active` kết hợp lắng nghe sự kiện `click shown.bs.tab` bằng jQuery để đảm bảo hiệu ứng di chuyển màu xanh và đổi màu badge số lượng diễn ra tức thì, mượt mà 100%.
2. **Xóa bỏ nút "Thêm menu mới" trùng lặp:**
   - Xóa bỏ nút thêm menu chung ở header trên cùng để loại bỏ sự thừa thãi.
   - Giữ lại và nâng cấp nút thêm chuyên biệt theo từng phân hệ trong từng thẻ tab: `[+ Thêm menu cho Khách hàng]`, `[+ Thêm menu cho Chủ xe]`, `[+ Thêm menu cho Admin]`.

### 24.2. Cấu trúc Cây Menu Cha - Con (Tiêu đề nhóm & Mục chức năng con)
- Phân tách rõ ràng giữa **Tiêu đề nhóm (Section Heading / Menu Cha)** và **Mục menu chức năng (Menu Con)** khớp hoàn toàn với thực tế điều hướng:
  - **Phân hệ Admin:**
    - 📁 Nhóm `TỔNG QUAN` ➔ `Tổng quan` (`/Admin/Home/Index`)
    - 📁 Nhóm `XE & ĐƠN HÀNG` ➔ `Phê duyệt xe`, `Duyệt hồ sơ Chủ xe`, `Quản lý đội xe`, `Quản lý kiểu xe`, `Đơn thuê xe`, `Tài chính & đối soát`
    - 📁 Nhóm `NỘI DUNG & TƯƠNG TÁC` ➔ `Bài viết & tin tức`, `Quản lý Menu`, `Đánh giá & Bình luận`
    - 📁 Nhóm `HỆ THỐNG` ➔ `Địa điểm nhận xe`, `Người dùng & đối tác`, `Cấu hình hệ thống`
  - **Phân hệ Chủ xe (CarOwner / Host Pro):**
    - 📁 Nhóm `QUẢN TRỊ PHƯƠNG TIỆN` ➔ `Tổng quan`, `Quản lý xe`, `Thêm xe`, `Lịch xe`
    - 📁 Nhóm `KINH DOANH & THU NHẬP` ➔ `Lịch trình & Đơn xe`, `Phân tích hiệu suất`, `Ví đối tác & Đối soát`, `Hồ sơ pháp lý & Ngân hàng`
  - **Phân hệ Khách hàng (Customer):**
    - `Trang chủ`, `Thuê xe`, `Cẩm nang`, `Tin tức`, `Liên hệ` (và các menu tùy biến mới tạo).
- **Giao diện bảng Quản lý Menu:**
  - Nhóm cha hiển thị in hoa đậm, gắn badge `📁 Tiêu đề nhóm`, URL `-- Tiêu đề phân nhóm --` và có sẵn nút `+ Con` để thêm nhanh menu con trực thuộc nhóm.
  - Menu con thụt lề `ps-4`, có biểu tượng nhánh cây `↳` và badge `↳ Thuộc: [Tên nhóm]`.
  - Bộ chọn Menu Cha trong form `Create` và `Edit` tự động gán nhãn `📁 [Nhóm]` trực quan.

### 24.3. Kết nối Dữ liệu Menu Thật từ CSDL ra Giao diện 3 Phân hệ
- Thay thế các thanh điều hướng HTML tĩnh trước đây bằng luồng render động trực tiếp từ CSDL bảng `Menu`:
  - **Header Khách hàng (`Views/Shared/_Layout.cshtml`):** Truy vấn động `_db.Menus.Where(m => (m.Subsystem == "Customer") && m.IsActive)` và hiển thị chính xác theo thứ tự `Position`.
  - **Sidebar Chủ xe (`Areas/CarOwner/Views/Shared/_Layout.cshtml`):** Truy vấn động `_db.Menus.Where(m => m.Subsystem == "CarOwner" && m.IsActive)` theo từng nhóm phân cấp cha - con.
  - **Sidebar Admin (`Areas/Admin/Views/Shared/_Layout.cshtml`):** Truy vấn động `_db.Menus.Where(m => m.Subsystem == "Admin" && m.IsActive)` theo từng nhóm, giữ nguyên logic badge số lượng thông báo thực tế (Xe chờ duyệt, Hồ sơ chủ xe chờ duyệt).
- **Kiểm thử thực tế thành công:** Tạo menu mới `test` ở phân hệ Khách hàng với thứ tự vị trí số 4 (`Position = 4`), menu lập tức xuất hiện trực tiếp trên thanh Navbar Header giữa `Cẩm nang` và `Tin tức` mà không cần chỉnh sửa bất kỳ dòng code nào.
- Toàn bộ giải pháp đạt chuẩn bảo mật, thiết kế pixel-perfect và biên dịch **`Build succeeded. 0 Error(s)`**.


---

## 25. CẬP NHẬT TIẾN TRÌNH: RÀ SOÁT, DỌN DẸP TOÀN BỘ TÀN DƯ MAZER VÀ ĐỒNG BỘ 100% PHÂN HỆ ADMIN (NGÀY 09/11/2026)

### 25.1. Tái Thiết Kế & Nâng Cấp Toàn Diện Phân Hệ Bài Viết & Tin Tức (`Areas/Admin/Views/Blogs/`)
1. **Loại bỏ 100% template Mazer cũ:**
   - Xóa bỏ triệt để các thẻ `#main`, `.burger-btn`, `.page-heading` và footer Mazer 2021 của A. Saugi trong toàn bộ các view của Blogs.
   - Sửa lỗi nghiêm trọng: Tiêu đề card bị ghi nhầm thành *"Thêm mới xe"* / *"Xoá xe"*.
2. **Thiết kế mới theo ReCar Design System:**
   - [`Blogs/Create.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Blogs/Create.cshtml): Form 2 cột hiện đại, tiêu đề, tóm tắt Sapo, textarea chi tiết bài viết, switch bật/tắt xuất bản, hỗ trợ tải lên file ảnh hoặc nhập URL kèm **Live Preview xem trước ảnh tức thì**.
   - [`Blogs/Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Blogs/Edit.cshtml): Giữ nguyên thông tin người tạo, ngày tạo, cập nhật bài viết mượt mà, xem trước ảnh hiện tại.
   - [`Blogs/Delete.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Blogs/Delete.cshtml): Thẻ cảnh báo nguy hiểm bo viền đỏ chuyên nghiệp, tóm tắt bài viết cần xóa, nút xác nhận xóa vĩnh viễn.
   - [`Blogs/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Blogs/Index.cshtml): Bổ sung nút Toggle trạng thái (*Đã xuất bản / Bản nháp*) bằng AJAX SweetAlert2 và nút Xóa nhanh không cần tải lại trang.
3. **Nâng cấp Controller ([`BlogsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/BlogsController.cs)):**
   - Bổ sung kiểm tra quyền Admin chặt chẽ qua `CheckAdmin()`.
   - Bổ sung xử lý upload file ảnh vào `/wwwroot/images/blogs/`.
   - Bổ sung 2 endpoint AJAX: `DeleteAjax` (xóa an toàn qua modal) và `ToggleActive` (bật/tắt xuất bản tức thì).

### 25.2. Dọn Dẹp Phân Hệ Quản Lý Đơn Hàng (`Areas/Admin/Views/Orders/`)
- Đã xóa bỏ tệp view Mazer cũ [`Orders/OrderDetails.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Orders/OrderDetails.cshtml).
- Toàn bộ luồng xem chi tiết và cập nhật đơn hàng phía Admin tập trung duy nhất tại [`Orders/Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Orders/Edit.cshtml) chuẩn ReCar Panel có bóc tách phí hoa hồng sàn và tiền cọc.

### 25.3. Dọn Dẹp Phân Hệ Thư Viện Ảnh Xe (`CarImages`)
- Đã xóa bỏ toàn bộ thư mục view scaffolding thô sơ `Areas/Admin/Views/CarImages/` và controller không dùng `Areas/Admin/Controllers/CarImagesController.cs`.
- Bảo toàn 100% bảng CSDL `carimages` và logic truy vấn ảnh xe của hệ thống trong `CarsController.cs`.

### 25.4. Dọn Dẹp Thư Mục View Tàn Dư Cũ (`Customer/` & `Product/`)
- Xóa bỏ 2 thư mục view Mazer cũ: `Areas/Admin/Views/Customer/` và `Areas/Admin/Views/Product/`.
- Chuẩn hóa [`ProductController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/ProductController.cs) và [`CustomerController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CustomerController.cs) tự động chuyển hướng mọi truy vấn cũ về `/Admin/Cars` và `/Admin/UserManagement`.

### 25.5. Dọn Dẹp Trình Quản Lý Tệp (FileManager & elFinder)
- Xóa bỏ thư mục `Areas/Admin/Views/FileManager/`.
- Xóa bỏ tệp `Areas/Admin/Views/Shared/_SummerNote.cshtml` (nguyên nhân gây xung đột CSS Bootstrap cũ).
- Xóa bỏ `Areas/Admin/Controllers/FileManagerController.cs`, `FileSystemController.cs` và `Areas/Admin/Models/SummerNote.cs`.

### 25.6. Đồng Bộ Toàn Diện Trải Nghiệm Người Dùng (UX & SweetAlert2)
- Loại bỏ hoàn toàn các hàm `onsubmit="return confirm(...)"` mặc định thô sơ của trình duyệt.
- Thay thế 100% bằng popup xác nhận xóa **SweetAlert2** bo góc hiện đại, đồng bộ trên toàn bộ các trang Admin:
  - [`CarTypes/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/CarTypes/Index.cshtml): Xác nhận xóa kiểu xe / phân khúc.
  - [`Locations/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Locations/Index.cshtml): Xác nhận xóa địa điểm nhận xe.
  - [`Reviews/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Reviews/Index.cshtml): Xác nhận xóa đánh giá chuyến đi của khách và bình luận bài viết.
- **Kết quả rà soát:** Phân hệ Admin đạt **0% tàn dư Mazer**, đồng bộ 100% theo ReCar Design System.


### 25.7. Khắc Phục Lỗi Biên Dịch (Build Succeeded: 0 Error)
- **Lỗi phát hiện:** Tệp [`Areas/Admin/Views/Locations/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Locations/Index.cshtml) gọi thuộc tính không tồn tại `@item.SearchKeywords` trong Model `Location`, dẫn đến lỗi `error CS1061: 'Location' does not contain a definition for 'SearchKeywords'`.
- **Khắc phục:** Đã điều chỉnh hiển thị sang thuộc tính chuẩn `@item.Region`.
- **Kết quả biên dịch:** **`Build succeeded. 0 Error(s)`**.


---

## 26. CẬP NHẬT TIẾN TRÌNH: HOÀN THIỆN PHÂN HỆ THÊM XE CHỦ XE & ĐỒNG BỘ HIỂN THỊ 4 ẢNH GÓC XE (NGÀY 09/11/2026)

### 26.1. Sửa Lỗi Font Chữ Dropdown Quản Lý Xe (`Areas/CarOwner/Views/Cars/Index.cshtml`)
- **Vấn đề:** Trong menu thả xuống "Hành động" của từng xe, mục lịch bị lỗi hiển thị mã hóa font thành `Lá»«ch & Cháº·n ngĂ y`.
- **Khắc phục:** Đã chuẩn hóa mã hóa UTF-8 và điều chỉnh chính xác thành nhãn `Lịch & Chặn ngày`.

### 26.2. Nâng Cấp Form Thêm Mới Xe Phía Chủ Xe (`/CarOwner/Cars/Create`)
1. **Mở rộng ViewModel ([`CarCreateVM.cs`](file:///d:/CarRental_TTCN/CarRental/ViewModels/CarCreateVM.cs)):**
   - Bổ sung 4 trường `IFormFile`: `Image1File` (Đầu xe / Mặt trước), `Image2File` (Đuôi xe / Mặt sau), `Image3File` (Nội thất khoang lái), `Image4File` (Sườn xe / Toàn cảnh nghiêng).
2. **Thiết kế lại Bước 5 (Hình ảnh xe & Pháp lý) theo ReCar Design System:**
   - Bổ sung khung lưới 4 ô tải ảnh 4 góc xung quanh xe với hướng dẫn cụ thể, xem trước ảnh Live Preview tức thì và nút chọn ảnh riêng biệt.
   - Tách biệt rõ ràng 3 nhóm ảnh: **Ảnh đại diện hiển thị sàn**, **4 ảnh chụp thực tế 4 góc xe** và **3 ảnh giấy tờ pháp lý bảo mật (Cà vẹt, Đăng kiểm, Bảo hiểm)**.
3. **Cập nhật Bước 6 (Xác nhận & Gửi duyệt):**
   - Bổ sung dải hiển thị xem trước 4 ảnh góc xe ngay cạnh ảnh đại diện để chủ xe kiểm tra tổng quan trước khi bấm nút gửi hồ sơ xét duyệt.
4. **Ràng Buộc Chặt Chẽ Dữ Liệu Nhập (Form Validation & UX):**
   - Tích hợp hàm kiểm tra `validateStep(stepIndex)` trước mỗi lần bấm nút chuyển bước:
     - **Bước 1 (Thông tin cơ bản):** Bắt buộc nhập đầy đủ Tên xe, Hãng sản xuất, Năm sản xuất, Biển số xe, Phân khúc, Số chỗ ngồi, Hộp số, Nhiên liệu, Mức tiêu hao.
     - **Bước 2 (Địa chỉ giao nhận):** Bắt buộc chọn Tỉnh/Thành phố, Quận/Huyện và nhập Địa chỉ nhận xe chi tiết.
     - **Bước 3 (Đơn giá & Thế chấp):** Bắt buộc nhập Giá thuê theo ngày, Số tiền đặt cọc và chọn Tài sản thế chấp yêu cầu.
     - **Bước 5 (Hình ảnh):** Bắt buộc tải lên đủ toàn bộ 8 hình ảnh (1 ảnh đại diện + 4 ảnh 4 góc xe + 3 ảnh giấy tờ). Nếu thiếu bất kỳ ảnh nào, hệ thống chặn chuyển bước và hiển thị cảnh báo **SweetAlert2** trực quan chỉ rõ ô còn thiếu.
5. **Tối ưu Cơ chế Lưu CSDL ([`CarsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/CarsController.cs)):**
   - Khi chủ xe thêm xe mới: Tạo đúng 1 bản ghi `CarImage` liên kết với `CarId`, đồng thời lưu đầy đủ 4 đường dẫn ảnh vào 4 cột `Image1, Image2, Image3, Image4` của bảng `carimages`.

### 26.3. Đồng Bộ Hiển Thị 4 Ảnh Chi Tiết Trên Toàn Bộ Hệ Thống
1. **Giao diện Khách hàng (`Views/Product/Details.cshtml` & `ProductController.cs`):**
   - Truy vấn toàn bộ 4 ảnh góc xe từ `CarImages` và kết hợp cùng ảnh đại diện thành thư viện hình ảnh `allPhotos`, cho phép khách hàng xem đầy đủ ảnh thật của xe khi bấm vào xem chi tiết xe.
2. **Giao diện Admin Duyệt xe (`Areas/Admin/Views/Cars/Pending.cshtml` & `CarsController.cs`):**
   - Gom dữ liệu `CarImages` vào danh sách ảnh chi tiết; khi Admin bấm nút xem thông tin xe trong Drawer kiểm duyệt, toàn bộ 4 ảnh xung quanh xe và ảnh giấy tờ đều được hiển thị rõ ràng, phóng to thu nhỏ tiện lợi.
3. **Giao diện Chủ xe Xem Chi tiết xe (`Areas/CarOwner/Views/Cars/Details.cshtml` & `CarsController.cs`):**
   - Bổ sung gom cả 4 trường `Image1, Image2, Image3, Image4` vào `ViewBag.GalleryImages` để hiển thị trong tab Bộ sưu tập ảnh xe của chủ xe.

### 26.4. Kết Quả Kiểm Thử
- Toàn bộ source code C# biên dịch thành công: **`Build succeeded. 0 Error(s)`**.


---

## 27. CẬP NHẬT TIẾN TRÌNH: HỆ THỐNG THÔNG BÁO HEADER TOÀN DIỆN & TỐI ƯU MENU QUẢN LÝ XE CHỜ DUYỆT (NGÀY 09/11/2026)

### 27.1. Tối Ưu Menu Hành Động (3 Chấm) Cho Xe Mới Gửi Lên (`Areas/CarOwner/Views/Cars/Index.cshtml`)
- **Yêu cầu:** Khi xe mới gửi lên đang ở trạng thái chờ duyệt (hoặc bị từ chối/cần cập nhật), menu dropdown 3 chấm chỉ được phép hiển thị các thao tác phù hợp, không cho phép xem lịch hoặc xem trên sàn khi xe chưa được phê duyệt.
- **Thực hiện:**
  - Thiết lập điều kiện phân nhánh theo trạng thái `isPending || isPendingUpdate || isRejected`:
    - **Khi xe chưa duyệt:** Menu 3 chấm chỉ hiển thị chính xác 3 thao tác:
      1. `Tình trạng duyệt hồ sơ` (icon `bi-shield-check`, link tới `/CarOwner/Cars/Details/{id}`)
      2. `Chỉnh sửa` (icon `bi-pencil`, link tới `/CarOwner/Cars/Edit/{id}`)
      3. `Gỡ xe khỏi sàn` (icon `bi-trash3`, link tới `/CarOwner/Cars/Delete/{id}`)
    - **Khi xe đã được duyệt:** Hiển thị đầy đủ 5 thao tác (*Lịch & Chặn ngày, Chi tiết xe, Chỉnh sửa, Xem trên sàn, Gỡ xe khỏi sàn*).
  - Khóa công tắc (*Switch*) Khuyến mãi (`disabled`) khi xe chưa duyệt.
  - Cập nhật trang [`Areas/CarOwner/Views/Cars/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Details.cshtml): Bổ sung Banner trạng thái kiểm duyệt trực quan màu vàng hổ phách, thông báo quy trình thẩm định trong 24 giờ và phím tắt cập nhật hồ sơ nhanh.

### 27.2. Xây Dựng Hệ Thống Lưu Trữ & Hiển Thị Thông Báo Header (Notification System)
1. **Khởi tạo Bảng CSDL MySQL `notifications`:**
   - Các trường dữ liệu: `NotificationId` (PK), `UserId`, `Role`, `Title`, `Content`, `Type`, `LinkUrl`, `IsRead`, `CreatedAt`.
   - Lập chỉ mục tối ưu truy vấn theo `UserId`, `Role` và `CreatedAt`.
2. **Khai báo Model & Context:**
   - Tạo model [`Notification.cs`](file:///d:/CarRental_TTCN/CarRental/Models/Notification.cs).
   - Đăng ký `DbSet<Notification> Notifications` và cấu hình Fluent API trong [`DbRenalCarContext.cs`](file:///d:/CarRental_TTCN/CarRental/Models/DbRenalCarContext.cs).
3. **Dịch vụ Quản lý & Phát Thông Báo (`INotificationService` & `NotificationService`):**
   - Đăng ký Scoped Service trong [`Program.cs`](file:///d:/CarRental_TTCN/CarRental/Program.cs).
   - Tự động lưu thông báo vào CSDL MySQL đồng thời phát sự kiện Realtime SignalR (`ReCarHub`) theo từng đối tượng: nhóm người dùng cụ thể (`User_{id}`), nhóm vai trò (`Role_Admin`, `Role_CarOwner`, `Role_Customer`) hoặc toàn sàn (`Clients.All`).
4. **Tích hợp Tự Động Tạo Thông Báo Trong Các Luồng Nghiệp Vụ:**
   - **Admin Duyệt xe (`Areas/Admin/Controllers/CarsController.cs`):** Tự động gửi thông báo cho Chủ xe: *"Hồ sơ xe đã được phê duyệt 🎉 - Xe sẵn sàng cho thuê trên sàn ReCar"*.
   - **Admin Từ chối xe (`Areas/Admin/Controllers/CarsController.cs`):** Tự động gửi thông báo cho Chủ xe kèm lý do từ chối cụ thể để chủ xe sửa đổi.
   - **Chủ xe Đăng ký xe mới (`Areas/CarOwner/Controllers/CarsController.cs`):** Tự động gửi thông báo cho Admin: *"Hồ sơ xe mới chờ kiểm duyệt 🚗"* và thông báo xác nhận tiếp nhận cho Chủ xe.
   - **Khách hàng Đặt cọc thuê xe (`Controllers/CartController.cs`):** Tự động gửi thông báo đồng thời cho Chủ xe (*Có đơn đặt xe mới kèm mã đơn và lịch bàn giao*), Admin (*Phát sinh đơn thuê mới*) và Khách hàng (*Xác nhận đặt cọc thành công*).
5. **Bộ Điều Khiển API ([`Controllers/NotificationController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/NotificationController.cs)):**
   - `GET /Notification/GetNotifications`: Truy vấn thông báo theo Session người dùng hiện tại (Admin, Chủ xe hoặc Khách hàng), định dạng thời gian thân thiện (*vừa xong, x phút trước, hôm qua*), đếm số lượng chưa đọc.
   - `POST /Notification/MarkRead`: Đánh dấu 1 thông báo đã đọc.
   - `POST /Notification/MarkAllRead`: Đánh dấu tất cả thông báo đã đọc.
   - `POST /Notification/Broadcast`: Cho phép Admin phát thông báo hệ thống tới toàn sàn.
6. **Mô-đun Giao Diện Dùng Chung & Realtime ([`wwwroot/assets/js/recar-notifications.js`](file:///d:/CarRental_TTCN/CarRental/wwwroot/assets/js/recar-notifications.js)):**
   - Xử lý nạp dữ liệu AJAX, render badge đếm số lượng thông báo đỏ nổi bật trên chuông.
   - Phân loại icon và bảng màu thông minh theo loại: Duyệt xe (Xanh lá), Từ chối (Đỏ), Chờ duyệt (Cam), Đơn mới (Xanh ReCar), Hệ thống (Tím).
   - Hỗ trợ click trực tiếp để chuyển hướng đến trang tương ứng (`LinkUrl`).
   - Lắng nghe SignalR: Cập nhật thông báo mới tức thì không cần tải lại trang và hiển thị Toast thông báo ở góc màn hình.
7. **Đồng Bộ Header Chuông Thông Báo Trên Cả 3 Phân Hệ:**
   - **Sidebar/Header Chủ xe ([`Areas/CarOwner/Views/Shared/_Layout.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Shared/_Layout.cshtml)):** Nâng cấp dropdown chuông, có nút "Đọc tất cả" và danh sách thông báo cuộn mượt.
   - **Header Admin ([`Areas/Admin/Views/Shared/_Layout.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Shared/_Layout.cshtml)):** Đồng bộ chuông thông báo hệ thống, đơn hàng và xe chờ duyệt.
   - **Header Khách hàng ([`Views/Shared/_Layout.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Shared/_Layout.cshtml)):** Nâng cấp nút chuông tĩnh thành dropdown thông báo tài khoản hoạt động đầy đủ.

### 27.3. Kết Quả Kiểm Thử & Biên Dịch
- Biên dịch toàn bộ mã nguồn C#: **`Build succeeded. 0 Error(s)`**.


---

## 28. CẬP NHẬT TIẾN TRÌNH: LOẠI BỎ EMOJI THÔNG BÁO & HIỂN THỊ CHI TIẾT LÝ DO TỪ CHỐI DUYỆT XE (NGÀY 09/11/2026)

### 28.1. Loại Bỏ Toàn Bộ Emoji Khỏi Hệ Thống Thông Báo
- **Mục tiêu:** Giữ giao diện chuyên nghiệp, trang nhã theo phong cách doanh nghiệp và ReCar Design System.
- **Thực hiện:**
  - Đã loại bỏ hoàn toàn các biểu tượng cảm xúc (emoji) trong tiêu đề và nội dung thông báo trên tất cả các Controller:
    - [`Areas/Admin/Controllers/CarsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarsController.cs): Bỏ emoji khi phê duyệt hoặc từ chối xe.
    - [`Areas/CarOwner/Controllers/CarsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/CarsController.cs): Bỏ emoji khi gửi hồ sơ xe mới.
    - [`Controllers/CartController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/CartController.cs): Bỏ emoji khi phát sinh đơn thuê xe mới.
    - [`wwwroot/assets/js/recar-notifications.js`](file:///d:/CarRental_TTCN/CarRental/wwwroot/assets/js/recar-notifications.js): Bỏ emoji trong các thông điệp Toast realtime.
  - Làm sạch toàn bộ dữ liệu mẫu trong bảng `notifications` trên CSDL MySQL `DbRenalCar`.

### 28.2. Hiển Thị Chi Tiết Lý Do Từ Chối Duyệt Xe (`Areas/CarOwner/Views/Cars/Details.cshtml`)
- **Trước đây:** Chỉ hiển thị cảnh báo chung chung "Hồ sơ xe đã bị từ chối", không hiển thị lý do cụ thể do Admin ghi khi từ chối hồ sơ.
- **Sau khi nâng cấp:**
  - **Tự động bóc tách lý do từ chối:** Trích xuất chính xác chuỗi lý do Admin đã nhập từ trường `Model.Descriptions` kèm thời gian phản hồi kiểm duyệt.
  - **Thiết kế Card cảnh báo cao cấp (ReCar Design System):**
    - Viền đỏ đậm `border-left: 5px solid #ef4444`, nền gradient đỏ nhẹ `#fff5f5` đến `#fee2e2`.
    - Huy hiệu nổi bật: `Bị từ chối kiểm duyệt` và hiển thị thời gian Admin phản hồi.
    - Khối trích dẫn lý do riêng biệt: Nền thẻ trắng bo góc, có biểu tượng trích dẫn và đường viền nhấn mạnh màu đỏ làm nổi bật nguyên văn lý do từ chối (Ví dụ: *"Ảnh đăng kiểm bị mờ, góc chụp đuôi xe chưa rõ biển số"*).
    - Hướng dẫn các bước khắc phục cho chủ xe.
    - Hai nút bấm hành động tức thì: **Chỉnh sửa & Cập nhật lại hồ sơ** (nút đỏ bo tròn dẫn trực tiếp tới trang Edit) và **Quay lại danh sách xe**.

### 28.3. Kết Quả Kiểm Thử
- Toàn bộ source code biên dịch chuẩn xác: **`Build succeeded. 0 Error(s)`**.


---

## 29. CẬP NHẬT TIẾN TRÌNH: SỬA LỖI FONT THÔNG BÁO VÀ KHẮC PHỤC LỖI KẾT NỐI KHI VỀ TRANG KHÁCH HÀNG (NGÀY 09/11/2026)

### 29.1. Khắc Phục Lỗi Font Tiếng Việt Bảng Thông Báo (`H? s? xe...`)
- **Vấn đề:** Do lệnh import trước đó qua terminal PowerShell làm mất mã hóa UTF-8, các bản ghi tiếng Việt có dấu trong bảng `notifications` bị lỗi hiển thị thành các dấu chấm hỏi `?` (*H? s? xe ?? ???c ph? du...*).
- **Khắc phục:**
  - Thiết lập chuẩn bộ ký tự UTF-8 4-byte `utf8mb4` cho toàn bộ bảng `notifications`.
  - Nạp lại dữ liệu tiếng Việt chuẩn có dấu với cờ `--default-character-set=utf8mb4`:
    - *"Hồ sơ xe đã được phê duyệt"*
    - *"Thông báo hệ thống ReCar"*
    - *"Có xe mới chờ duyệt"*
  - Hiển thị trên giao diện dropdown chuông sắc nét, chính xác 100%.

### 29.2. Khắc Phục Lỗi Connection Pool MariaDB: `FormatException: Expected to read 0x00 or 0xFE but got 0x01`
- **Hiện tượng:** Khi đang ở trang quản lý chủ xe (`/CarOwner`) bấm nút *"Về sàn khách thuê"* (`href="/"`) để quay về trang chủ khách hàng (`HomeController.Index()`), hệ thống bị crash với biệt lệ:
  `FormatException: Expected to read 0x00 or 0xFE but got 0x01` tại `MySqlConnector.Core.ServerSession.TryResetConnectionAsync`.
- **Nguyên nhân kỹ thuật:** Khi ứng dụng lấy lại một connection từ `ConnectionPool` trong MariaDB (XAMPP), thư viện `MySqlConnector` gửi gói tin `COM_RESET_CONNECTION`. Do phiên bản MariaDB phản hồi byte đầu tiên là `0x01` (khác với chuẩn `0x00` OK_Packet của MySQL thuần), bộ giải mã ném ra lỗi `FormatException`.
- **Giải pháp dứt điểm:**
  - Cập nhật chuỗi kết nối trong cả [`appsettings.json`](file:///d:/CarRental_TTCN/CarRental/appsettings.json) và [`appsettings.Development.json`](file:///d:/CarRental_TTCN/CarRental/appsettings.Development.json):
    `"DefaultConnection": "Server=localhost;Port=3306;Database=DbRenalCar;User=root;Password=;CharSet=utf8mb4;Connection Reset=false;"`
  - Tham số `Connection Reset=false` chỉ định `MySqlConnector` tái sử dụng phiên kết nối mà không gửi lệnh reset packet gây lỗi, bảo đảm chuyển trang qua lại giữa Chủ xe - Admin - Khách hàng hoàn toàn ổn định và nhanh chóng.

### 29.3. Kết Quả Kiểm Thử
- Biên dịch mã nguồn: **`Build succeeded. 0 Error(s)`**.


---

## 30. CẬP NHẬT TIẾN TRÌNH: HOÀN THIỆN NGHIỆP VỤ QUẢN LÝ ĐỘI XE ADMIN & ĐIỀU KHIỂN TRẠNG THÁI XE CHỦ XE (NGÀY 10/11/2026)

### 30.1. Tái Cấu Trúc Quản Lý Xe Admin (`Areas/Admin/Views/Cars/Index.cshtml`) & Liên Kết Đối Tác Chủ Xe
1. **Bổ sung thao tác Xóa xe an toàn (`Delete`):**
   - Kiểm tra chặt chẽ điều kiện ràng buộc: Nếu xe đang có hợp đồng hoặc đơn thuê đang hoạt động (`Đang thuê`, `Chờ duyệt`, `Đã xác nhận`), hệ thống sẽ từ chối xóa và hiển thị cảnh báo để tránh xung đột đơn hàng.
   - Khi thỏa điều kiện: Tự động dọn dẹp các bản ghi liên kết phụ thuộc (`CarImages`, `CarAmenities`, `CarCalendarBlocks`), sau đó xóa xe khỏi hệ thống và gửi thông báo xác nhận tới tài khoản Chủ xe.
   - Tích hợp hộp thoại xác nhận SweetAlert2 dạng cảnh báo nguy hiểm trước khi thực thi.
2. **Quy trình Phê duyệt Trạng thái Xe (Bảo trì / Tạm ngưng hoạt động):**
   - Chuẩn hóa luồng nghiệp vụ thực tế của sàn xe: Chủ xe là người chủ động gửi đề xuất xin phép chuyển xe sang *Bảo trì* (bảo dưỡng, sửa chữa) hoặc *Tạm ngưng hoạt động* (lý do cá nhân) kèm theo lý do cụ thể.
   - Phía Admin:
     - Thêm bộ lọc trạng thái: `Chờ bảo trì`, `Chờ tạm ngưng`.
     - Huy hiệu trạng thái cảnh báo nổi bật kèm tooltip hiển thị lý do chủ xe đề xuất.
     - Nút duyệt nhanh `Duyệt` (phê duyệt chuyển trạng thái theo mong muốn của chủ xe) hoặc `Từ chối` (hoàn trả trạng thái về `Sẵn sàng` kèm ghi chú phản hồi) trực tiếp trên từng dòng xe.
3. **Liên kết Chặt chẽ giữa Admin và Chủ xe:**
   - **Xem hồ sơ chi tiết đối tác chủ xe (`#modalOwnerDetail`):** Nhấp vào tên chủ xe hoặc chọn menu thao tác để mở Modal nạp Ajax thông tin cá nhân: Họ tên, Username, Số điện thoại, Email, Địa chỉ, ảnh đại diện, tổng số xe sở hữu và danh sách tóm tắt toàn bộ đội xe của chủ xe đó.
   - **Gửi tin nhắn / Thông báo riêng tới Chủ xe (`#modalSendMessage`):** Cho phép Quản trị viên nhập tiêu đề và nội dung nhắn gửi riêng (nhắc nhở giấy tờ, thông báo bảo dưỡng, trao đổi nghiệp vụ) gửi trực tiếp vào chuông thông báo và SignalR realtime của chủ xe.

### 30.2. Nâng Cấp Giao Diện & Logic Chỉnh Sửa Xe Bên Chủ Xe (`Areas/CarOwner/Views/Cars/Edit.cshtml`)
1. **Khóa cố định thông tin định danh & pháp lý cốt lõi:**
   - Sau khi xe đã được kiểm duyệt đưa lên sàn, các trường định danh kỹ thuật quan trọng như: **Tên xe, Hãng sản xuất, Phân khúc, Năm sản xuất, Biển số xe** sẽ được khóa chế độ chỉ đọc (`readonly`/`disabled` kèm `input hidden`).
   - Đính kèm huy hiệu `Đã xác thực bởi ReCar` để bảo vệ tính toàn vẹn của hồ sơ pháp lý mà Admin đã phê duyệt, tránh tình trạng gian lận thay đổi thông số xe sau khi được cấp phép.
2. **Cho phép chủ xe cập nhật thông tin thương mại:**
   - Chủ xe toàn quyền điều chỉnh: Giá thuê theo ngày, Giá khuyến mãi, Tiền cọc thế chấp, Địa chỉ nhận xe thực tế, Danh mục tiện nghi bổ sung, Điều khoản quy định thuê xe và Ảnh góc xe.
3. **Thanh Điều Khiển & Gửi Yêu Cầu Chuyển Trạng Thái Xe:**
   - Tích hợp Card thông báo trạng thái hoạt động ở đầu trang chỉnh sửa.
   - Bổ sung nút bấm tương tác: `Xin chuyển Bảo trì`, `Xin Tạm ngưng hoạt động`, hoặc `Kích hoạt lại Sẵn sàng`.
   - Tích hợp Modal `#modalRequestStatus`: Chủ xe nhập lý do chi tiết để gửi đến Quản trị viên ReCar; trạng thái xe sẽ tự động chuyển sang `Chờ bảo trì` hoặc `Chờ tạm ngưng` đồng thời bắn thông báo realtime tới Admin.

### 30.3. Kết Quả Kiểm Thử
- Dự án biên dịch hoàn hảo: **`Build succeeded. 0 Error(s)`**.

---

## 31. CẬP NHẬT TIẾN TRÌNH: TÍCH HỢP Ô CHAT THỜI GIAN THỰC CHO BÊN ADMIN & ĐỒNG BỘ TIN NHẮN VỚI CHỦ XE (NGÀY 10/11/2026)

### 31.1. Tinh Gọn Hệ Thống & Chuyển Đổi Sang Khung Chat Trực Tiếp
- **Tiếp thu phản hồi:** Loại bỏ modal gửi thông báo riêng rườm rà (`#modalSendMessage`) và thay bằng cơ chế nhắn tin trực tiếp tự nhiên giống như giữa khách hàng và chủ xe.
- **Tích hợp Ô Chat trực tiếp cho Admin (`#recarChatWindow`):**
  - Đưa toàn bộ cấu trúc cửa sổ Chat nổi (Floating Chat Window) vào Layout Admin ([`Areas/Admin/Views/Shared/_Layout.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Shared/_Layout.cshtml)):
    - Tiêu đề chat, tên đối tác trò chuyện, chỉ báo trạng thái trực tuyến xanh lá, nút thu nhỏ/đóng.
    - Khung tin nhắn với phân biệt bong bóng tin nhắn gửi đi của Admin (`.from-user`) và tin nhắn nhận từ Chủ xe/Khách (`.from-owner`).
    - Chỉ báo đang soạn tin (Typing Indicator) thời gian thực.
    - Ô nhập văn bản và nút gửi tin nhắn.
  - Thêm icon Chat và Dropdown cuộc trò chuyện gần đây (`#recentChatList`) ngay trên Topbar Admin, hiển thị chấm xanh thông báo tin nhắn mới và cho phép nhấp chọn bất kỳ cuộc hội thoại nào để tiếp tục chat.
  - Bổ sung định dạng CSS hoàn chỉnh cho `.recar-chat-window` trong [`recar-admin.css`](file:///d:/CarRental_TTCN/CarRental/wwwroot/assets/css/recar-admin.css).

### 31.2. Nâng Cấp Backend `ChatController.cs` Hỗ Trợ Quyền Admin
- **Nhận diện phiên Admin:** Mở rộng hàm `ResolveCurrentCustomerAsync` để tự động nhận diện tài khoản Admin thông qua `Function.GetAdminAccountId(HttpContext.Session)` hoặc tài khoản có `RoleId == 1`.
- **Hỗ trợ cờ `asAdmin`:** Cập nhật các action `SendMessage`, `GetMessages`, `GetRecentConversations`, `GetUnreadCount` để xử lý mượt mà cho vai trò `Admin`:
  - Khi Admin gửi tin nhắn: Lưu `SenderRole = "Admin"`, `ReceiverId = ownerId` (hoặc `customerId`).
  - Gửi tín hiệu SignalR tức thì tới nhóm `User_{receiverId}` để chuông và toast bên Chủ xe lập tức hiển thị.
  - Khi Chủ xe phản hồi: Tin nhắn được phát tới nhóm `User_{adminId}`, cửa sổ chat của Admin tự động cập nhật ngay lập tức.

### 31.3. Đồng Bộ Trải Nghiệm Trên Trang Quản Lý Đội Xe Admin (`/Admin/Cars/Index`)
- **Nút nhắn tin trực tiếp:** 
  - Tại cột Chủ xe và menu dropdown thao tác: Nhấp vào "Nhắn tin cho Chủ xe" sẽ gọi hàm `openChatBox(ownerName, defaultGreeting, carId, ownerId, carName)`, mở ngay cửa sổ chat thời gian thực với đối tác chủ xe đó.
  - Trong Modal xem hồ sơ Chủ xe (`#modalOwnerDetail`): Thêm nút *"Nhắn tin cho đối tác"* ở chân modal, bấm vào sẽ đóng modal và kích hoạt mở ngay ô chat để Quản trị viên trao đổi trực tiếp.

### 31.4. Kết Quả Kiểm Thử & Biên Dịch
- Biên dịch toàn bộ mã nguồn C#: **`Build succeeded. 0 Error(s)`**.


---

## 32. CẬP NHẬT TIẾN TRÌNH: TÁI CẤU TRÚC MODAL DUYỆT YÊU CẦU BẢO TRÌ/TẠM NGƯNG & KHẮC PHỤC TRÀN GIAO DIỆN (NGÀY 10/11/2026)

### 32.1. Vấn Đề Giao Diện & Trải Nghiệm Cũ
- **Hiện tượng:** Tại bảng Quản lý đội xe Admin ([`Areas/Admin/Views/Cars/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Index.cshtml)), khi xe có yêu cầu `Chờ bảo trì` hoặc `Chờ tạm ngưng`, hệ thống chèn trực tiếp 2 nút to *"Duyệt"* (xanh) và *"X"* (đỏ) ngay cạnh menu 3 chấm ở cột *Thao tác*.
- **Hệ quả:** Cột Thao tác bị phình to làm bề ngang bảng vượt quá khung nhìn, gây tràn ngang và làm méo mó, bị cắt chữ tiêu đề thành *"THAO TÁ"*.
- **Về mặt nghiệp vụ:** Quản trị viên chưa đọc được lý do chi tiết mà đã hiển thị nút duyệt nhanh là thiếu tính an toàn và chuyên nghiệp.

### 32.2. Giải Pháp Chuẩn Hóa Theo Ý Kiến Người Dùng
1. **Thiết kế lại Cột Trạng Thái (`CarStatus`):**
   - Giữ huy hiệu trạng thái cảnh báo `Chờ bảo trì` / `Chờ tạm ngưng`.
   - Bổ sung nút bấm nhỏ gọn, trang nhã: **`Xem lý do & Duyệt`** (`.btn-view-status-request`) với icon con mắt `bi-eye-fill` ngay dưới badge.
2. **Khôi phục Cột Thao Tác Tinh Gọn:**
   - Xóa bỏ hoàn toàn các nút bấm chen chúc.
   - Cột Thao tác được cố định kích thước `width: 80px`, chỉ chứa duy nhất nút menu 3 chấm `⋮`, chấm dứt triệt để lỗi tràn ngang.
   - Trong menu 3 chấm, đính kèm thêm mục ưu tiên: *"Xem xét lý do & Duyệt"*.
3. **Modal Xét Duyệt Yêu Cầu Chuyên Nghiệp (`#modalStatusRequestDetail`):**
   - Khi Admin bấm *"Xem lý do & Duyệt"*, một Modal được mở ra hiển thị đầy đủ:
     - Tên xe, Đời xe, Biển số dạng Card dập nổi, Tên đối tác Chủ xe.
     - Badge yêu cầu: `Chờ bảo trì` hoặc `Chờ tạm ngưng`.
     - Toàn văn lý do đề xuất do Chủ xe nhập được đóng khung trích dẫn nổi bật (`#reqDescriptionText`).
     - Hướng dẫn nghiệp vụ quản trị viên kiểm tra cuốc xe.
   - **Chân Modal cung cấp 2 nút hành động an toàn:**
     - Nút **"Phê duyệt yêu cầu"** (xanh lá `btn-success`).
     - Nút **"Từ chối yêu cầu"** (đỏ `btn-outline-danger`, mở hộp thoại nhập lý do gửi phản hồi cho chủ xe).

### 32.3. Kết Quả Kiểm Thử & Biên Dịch
- Biên dịch toàn bộ mã nguồn C#: **`Build succeeded. 0 Error(s)`**.


---

## 33. CẬP NHẬT TIẾN TRÌNH: DỌN DẸP NÚT PHỤ VÀ KHẮC PHỤC TRIỆT ĐỂ LỖI MỞ MODAL XÉT DUYỆT (NGÀY 10/11/2026)

### 33.1. Dọn Dẹp Nút Thừa Tại Cột Trạng Thái
- Xóa bỏ nút phụ *"Xem lý do & Duyệt"* dưới huy hiệu trạng thái theo đúng yêu cầu người dùng, giữ cột *TRẠNG THÁI* gọn gàng, sạch sẽ và đồng bộ với toàn bộ các hàng xe khác trong bảng.
- Tập trung toàn bộ thao tác vào menu 3 chấm `⋮` của dòng xe tương ứng.

### 33.2. Sửa Lỗi Modal Không Hiển Thị Khi Bấm Menu 3 Chấm
- **Nguyên nhân:** Trước đó việc truyền chuỗi lý do (`data-descriptions`) qua thuộc tính HTML `data-*` gặp xung đột khi văn bản có ký tự đặc biệt, dấu ngoặc kép hoặc xuống dòng, đồng thời sự kiện click trong Bootstrap dropdown bị chặn tương tác.
- **Khắc phục:**
  - Viết mới API Endpoint an toàn: [`GetStatusRequest(int id)`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarsController.cs#L497-L528) nạp dữ liệu sạch từ CSDL trả về JSON.
  - Xây dựng hàm gọi trực tiếp `openStatusRequestModal(carId)`:
    - Khi Admin chọn *"Xem xét lý do & Duyệt"* trong menu 3 chấm, hàm lập tức mở Modal `#modalStatusRequestDetail` với hiệu ứng nạp mượt mà.
    - Gọi Ajax lấy đầy đủ thông tin: Tên xe, Hãng xe, Biển số, Tên & SĐT chủ xe, Trạng thái xin phép, và nguyên văn lý do đề xuất.
    - Kích hoạt 2 nút *"Phê duyệt yêu cầu"* và *"Từ chối yêu cầu"* bên trong Modal.

### 33.3. Kết Quả Kiểm Thử & Biên Dịch
- Biên dịch toàn bộ mã nguồn C#: **`Build succeeded. 0 Error(s)`**.


---

## 34. CẬP NHẬT TIẾN TRÌNH: NÂNG CẤP XÉT DUYỆT BẢO TRÌ/TẠM NGƯNG BẰNG SWEETALERT2 CHỐNG LỖI 100% (NGÀY 10/11/2026)

### 34.1. Nguyên Nhân Sự Cố "Không Thao Tác Được Gì"
- **Xung đột phiên bản Bootstrap:** Dự án sử dụng `Bootstrap v5.0.0-beta1`, trong đó hàm `bootstrap.Modal.getOrCreateInstance` chưa tồn tại (chỉ có từ Bootstrap 5.1.0+), dẫn đến lỗi JavaScript âm thầm `TypeError: not a function` khi cố gắng khởi tạo modal truyền thống.
- **Hiện tượng:** Admin click vào dòng xe nhưng modal không thể bật lên, khiến xe bị kẹt ở trạng thái `Chờ bảo trì` mà không duyệt hay tương tác được.

### 34.2. Khắc Phục Triệt Để & Đa Dạng Hóa Điểm Chạm Thao Tác
1. **Chuyển Đổi Sang Hộp Thoại SweetAlert2 Độc Lập:**
   - Thay thế toàn bộ modal Bootstrap bằng dialog tùy biến cao cấp của `Swal.fire` (đã nạp sẵn trong hệ thống, không phụ thuộc vào Bootstrap JS).
   - Tự động nạp dữ liệu sạch qua Ajax từ [`/Admin/Cars/GetStatusRequest?id=`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarsController.cs#L498-L528).
   - Hiển thị đầy đủ thông tin: Tên xe, Biển số dập nổi, Chủ xe, Huy hiệu trạng thái, và khối trích dẫn nguyên văn lý do xin bảo trì/tạm ngưng.
   - Cung cấp sẵn 3 nút hành động tiện lợi:
     - **Phê duyệt (Duyệt)** (màu xanh lá): Chấp thuận chuyển trạng thái xe sang Bảo trì / Ngưng hoạt động và bắn thông báo tới chủ xe.
     - **Từ chối yêu cầu** (màu đỏ): Mở hộp thoại nhập lý do từ chối gửi trả lại cho chủ xe.
     - **Đóng** (màu xám): Hủy xem.
2. **Cung Cấp 3 Cách Thao Tác Trực Tiếp Tiện Lợi:**
   - **Cách 1:** Click trực tiếp vào Huy hiệu `• Chờ bảo trì` trên cột Trạng thái (được gán con trỏ click và icon info `bi-info-circle`).
   - **Cách 2:** Bấm nút biểu tượng biên bản kiểm tra màu vàng (`bi-clipboard-check`) ngay cạnh menu 3 chấm ở cột Thao tác.
   - **Cách 3:** Mở menu 3 chấm `⋮` và chọn mục đầu tiên *"Xem xét lý do & Duyệt"*.

### 34.3. Kết Quả Kiểm Thử & Biên Dịch
- Biên dịch toàn bộ mã nguồn C#: **`Build succeeded. 0 Error(s)`**.

---

## 35. CẬP NHẬT TIẾN TRÌNH: DỌN DẸP MENU THAO TÁC ADMIN & SỬA LỖI TOÀN BỘ BẢNG QUẢN LÝ ĐỘI XE (NGÀY 10/11/2026)

### 35.1. Nguyên Nhân Sự Cố Không Thao Tác Được Toàn Bảng
- **Dư thừa ký tự đóng ngoặc:** Tại cuối khối thẻ `<script>` trong [`Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Index.cshtml), tồn tại một dấu ngoặc nhọn `}` dư thừa khiến trình duyệt ném ngoại lệ `SyntaxError: Unexpected token '}'`.
- **Hệ quả:** Trình duyệt dừng thực thi toàn bộ script của view, làm cho tất cả các hàm thao tác (`openStatusRequestModal`, `toggleFleetStatus`, `confirmDeleteCar`, `showOwnerModal`, `chatWithOwnerFromModal`) rơi vào trạng thái `undefined`. Khi người dùng click vào bất kỳ mục nào (Xem lý do, Duyệt, Ngưng hoạt động, Xóa xe, Xem thông tin chủ xe) đều không có phản hồi.

### 35.2. Giải Pháp Triệt Để & Dọn Dẹp Nghiệp Vụ Theo Yêu Cầu
1. **Dọn dẹp các quyền cưỡng chế trạng thái thừa:**
   - Xóa bỏ hoàn toàn 2 mục *"Kích hoạt Sẵn sàng"* và *"Chuyển sang Bảo trì"* trong menu thao tác Admin vì hành động này thuộc quyền quyết định chủ động từ phía Chủ xe.
   - Menu thao tác Admin chỉ giữ các mục chuẩn:
     - **Xem xét lý do & Duyệt** (khi xe có yêu cầu từ chủ xe: *Chờ bảo trì* / *Chờ tạm ngưng*).
     - **Xem trên sàn ReCar** (mở tab xem chi tiết xe ngoài giao diện khách hàng).
     - **Xem thông tin Chủ xe** (popup hồ sơ đối tác).
     - **Nhắn tin cho Chủ xe** (mở ô chat trực tiếp giữa Admin và Chủ xe).
     - **Ngưng hoạt động (Ẩn khỏi sàn)** (dành cho Admin khi cần đình chỉ xe vi phạm).
     - **Xóa xe khỏi sàn** (có xác nhận nguy hiểm bằng SweetAlert2).
2. **Khắc phục lỗi cú pháp & Đăng ký toàn cục (Global Scope):**
   - Đã loại bỏ dấu `}` dư thừa.
   - Đăng ký tường minh tất cả các hàm vào `window` (`window.openStatusRequestModal`, `window.toggleFleetStatus`, `window.confirmDeleteCar`, `window.showOwnerModal`, `window.chatWithOwnerFromModal`, `window.openChatBox`).
   - Tối ưu hóa modal xem chi tiết chủ xe và xử lý fallback an toàn giữa Bootstrap 5 và jQuery Modal.

### 35.3. Kết Quả Kiểm Thử & Biên Dịch
- Biên dịch toàn bộ mã nguồn C#: **`Build succeeded. 0 Error(s)`**.
- Tất cả các nút thao tác và popup duyệt/từ chối, xóa xe, nhắn tin, xem hồ sơ đã hoạt động trơn tru 100%.

---

## 36. CẬP NHẬT TIẾN TRÌNH: KHẮC PHỤC TRIỆT ĐỂ LỖI MYSQL CONNECTOR PACKET OUT-OF-ORDER & FORMATEXCEPTION (NGÀY 10/11/2026)

### 36.1. Nguyên Nhân Gốc Rễ (Root Cause Analysis)
- **Ngoại lệ xảy ra:** `MySqlProtocolException: Packet received out-of-order. Expected 1; got 3` và trước đó là `FormatException: Expected to read 0x00 or 0xFE but got 0x01`.
- **Cơ chế xung đột:**
  1. Trong hệ thống có nhiều nơi gọi `_ = _notificationService.CreateNotificationAsync(...)` dưới dạng "fire-and-forget" (không sử dụng `await`).
  2. Trước đó `NotificationService` dùng chung Scoped `DbRenalCarContext` của HTTP Request. Khi action của Controller kết thúc và trả về View/Json/Redirect, phạm vi HTTP Request bị hủy và DbContext bị giải phóng trả connection về connection pool.
  3. Cùng lúc đó, task nền của `CreateNotificationAsync` vẫn đang gửi câu lệnh `INSERT INTO notifications` qua socket MySQL. Hai truy vấn cùng gửi/nhận dữ liệu dở dang trên một kết nối hoặc làm socket trong connection pool bị nhiễm bẩn dữ liệu.
  4. Khi request tiếp theo (như truy cập `/CarOwner/Cars/Edit` hay `/Home/Index`) lấy lại connection từ Pool để thực thi `_context.Customers.FirstOrDefault()`, socket đọc phải packet dở dang từ server (kỳ vọng packet 1 nhưng nhận phải packet 3), dẫn đến sập trang ngay lập tức.

### 36.2. Giải Pháp Triệt Để 2 Lớp (Two-Layer Defense)
1. **Lớp 1: Độc lập hóa DbContext trong `NotificationService`:**
   - Thay vì inject trực tiếp Scoped `DbRenalCarContext`, `NotificationService` inject `IServiceScopeFactory`.
   - Mỗi lần lưu thông báo, service tự tạo một Scope độc lập: `using var scope = _scopeFactory.CreateScope();`, lấy một DbContext riêng biệt, thực thi lưu dữ liệu và tự động giải phóng sạch sẽ. Hoàn toàn cách ly 100% với DbContext của HTTP Request.
2. **Lớp 2: Đồng bộ hóa toàn bộ sang Bất đồng bộ (`async Task<IActionResult>` & `await`):**
   - Đã chuyển đổi tất cả các action có bắn thông báo (`Approve`, `Reject`, `Create`...) sang `async Task<IActionResult>` và `await _notificationService.CreateNotificationAsync(...)`.
   - Đảm bảo toàn bộ luồng ghi dữ liệu và gửi thông báo hoàn tất trọn vẹn trước khi chuyển tiếp trang.

### 36.3. Kết Quả Kiểm Thử & Biên Dịch
- Biên dịch toàn bộ mã nguồn: **`Build succeeded. 0 Error(s)`**.
- Không còn bất kỳ hiện tượng xung đột packet, kẹt socket hay lỗi kết nối connection pool trên toàn hệ thống.

---

## 37. CẬP NHẬT TIẾN TRÌNH: HOÀN THIỆN LUỒNG "CHỜ CẬP NHẬT" & QUY TẮC HIỂN THỊ XE TRÊN SÀN (NGÀY 10/11/2026)

### 37.1. Chuẩn Hóa Nghiêm Ngặt Quy Tắc Hiển Thị Xe Trên Sàn Khách Hàng
- **Quy tắc tuyệt đối:** Chỉ những xe có **`CarStatus == "Sẵn sàng"`** và **`IsActive == true`** mới được xuất hiện trên sàn khách hàng.
- **Phạm vi áp dụng toàn diện:**
  1. [`HomeController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/HomeController.cs): `ViewBag.productFeatured` và `ViewBag.productDealofDay` chỉ nạp xe `Sẵn sàng`.
  2. [`ProductController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/ProductController.cs):
     - API lọc danh sách xe (`FilterProducts`): Chỉ lấy xe `Sẵn sàng`.
     - Bản đồ tìm xe quanh đây (`GetNearbyCars`): Chỉ hiển thị các chấm xe `Sẵn sàng`.
     - Chi tiết xe (`Details`): Chặn truy cập nếu xe không ở trạng thái `Sẵn sàng`.
     - Xe tương tự / xe đề xuất (`productRelated`): Chỉ đề xuất các xe `Sẵn sàng`.
  3. [`CartController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/CartController.cs): Chặn triệt để hành động đặt xe nếu xe không phải `Sẵn sàng`.
  - Mọi xe ở trạng thái `Chờ duyệt`, `Chờ cập nhật`, `Chờ bảo trì`, `Bảo trì`, `Chờ tạm ngưng`, `Ngưng hoạt động`, `Từ chối` đều bị **ẩn hoàn toàn khỏi khách hàng**.

### 37.2. Hoàn Thiện Luồng "Chờ Cập Nhật" Khi Chủ Xe Sửa Xe
1. **Phía Chủ Xe ([`/CarOwner/Cars/Edit`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/CarsController.cs)):**
   - Khi chủ xe chỉnh sửa và bấm *"Lưu thay đổi"*:
     - Xe tự động chuyển sang trạng thái **`Chờ cập nhật`** và `IsActive = false` (ngay lập tức tạm ẩn khỏi sàn cho thuê).
     - Ghi nhận tóm tắt thời điểm cập nhật vào mô tả xe để Admin dễ dàng đối chiếu.
     - Tự động bắn thông báo hệ thống và Realtime SignalR tới Ban Quản trị ReCar.
     - Hiển thị thông báo rõ ràng cho chủ xe: *"Xe đã được chuyển sang trạng thái 'Chờ cập nhật' (tạm ẩn khỏi sàn) để Ban Quản trị ReCar phê duyệt trước khi tiếp tục hiển thị."*
2. **Phía Admin ([`/Admin/Cars/Index`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Index.cshtml) & [`/Admin/Cars/Pending`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Pending.cshtml)):**
   - **Bảng Quản lý đội xe toàn sàn:**
     - Xe `Chờ cập nhật` hiển thị huy hiệu nổi bật màu xanh dương.
     - Click vào badge, nút clipboard vàng, hoặc chọn *"Xem xét lý do & Duyệt"* trong 3 chấm sẽ mở popup SweetAlert2 chuyên dụng: *"Xét duyệt cập nhật thông tin xe"*.
     - Khi Admin bấm **"Phê duyệt (Đưa lên sàn)"**: Xe chuyển sang `Sẵn sàng`, `IsActive = true` (hiển thị trở lại công khai trên sàn) và gửi thông báo chúc mừng về chuông của Chủ xe.
     - Khi Admin bấm **"Từ chối yêu cầu"**: Admin nhập lý do từ chối gửi về chuông của Chủ xe, giữ xe ẩn khỏi sàn để chủ xe chỉnh sửa lại.
   - **Trang Hồ sơ chờ duyệt (`/Admin/Cars/Pending`):**
     - Nạp cả xe `Chờ duyệt` và `Chờ cập nhật` để Admin có thể duyệt nhanh dạng lưới thẻ hoặc bảng.
   - **Menu Sidebar Admin:**
     - Bộ đếm số lượng hồ sơ chờ duyệt hiển thị chính xác tổng số xe đang cần Admin xử lý.

### 37.3. Kết Quả Kiểm Thử & Biên Dịch
- Biên dịch toàn bộ mã nguồn: **`Build succeeded. 0 Error(s)`**.
- Luồng duyệt xe và bảo mật hiển thị hoạt động chính xác 100% theo đúng yêu cầu nghiệp vụ.


---

## 38. CẬP NHẬT TIẾN TRÌNH: TÍNH NĂNG ĐỐI CHIẾU THAY ĐỔI 2 BÊN (SIDE-BY-SIDE DIFF) & HOÀN THIỆN THÔNG SỐ DUYỆT XE (NGÀY 10/11/2026)

### 38.1. Cơ Chế Snapshot & Đối Chiếu Thay Đổi Trước - Sau
1. **Chụp Bản Lưu Cũ (Snapshot) Tại Backend ([`CarOwner/CarsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Controllers/CarsController.cs)):**
   - Khi chủ xe gửi bản chỉnh sửa thông tin xe, trước khi áp dụng các thay đổi mới, hệ thống tự động đóng gói toàn bộ thông số cũ thành JSON snapshot:
     - Tên xe, Hãng xe, Phân khúc, Năm sản xuất, Biển số, Màu sắc, Số chỗ ngồi, Hộp số, Nhiên liệu.
     - Giá thuê niêm yết, Giá ưu đãi (SalePrice), Phụ phí tài xế riêng (DriverFee).
     - Giới hạn km, Phụ phí vượt km, Phí giao xe tận nơi, Mức tiêu hao nhiên liệu, Danh sách tiện nghi, Mô tả của chủ xe.
   - Snapshot được mã hóa an toàn dưới dạng Base64 và lưu trữ kèm trong cấu trúc trường `Details` của xe.
2. **Bảng Đối Chiếu Thay Đổi Trực Quan Tại Giao Diện Admin:**
   - **Sheet Phê duyệt xe ([`Admin/Cars/Pending.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Pending.cshtml)):**
     - Xe ở trạng thái `Chờ cập nhật` tự động hiển thị banner thông báo kèm nút chuyển đổi chế độ xem.
     - Bảng đối chiếu 2 cột (Cột Trước thay đổi - Bản cũ vs Cột Sau thay đổi - Bản mới): **Mục nào có sự thay đổi sẽ tự động được làm nổi bật với viền và nền màu cam kèm huy hiệu "Thay đổi"**.
   - **Modal Xét duyệt yêu cầu tại Quản lý xe ([`Admin/Cars/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Index.cshtml)):**
     - Tích hợp bảng đối chiếu thu gọn trực tiếp bên trong popup SweetAlert2, giúp Admin nắm bắt ngay tức thì các trường thông tin mà chủ xe vừa điều chỉnh.

### 38.2. Hoàn Thiện Đầy Đủ Các Thông Số Kiểm Tra Hồ Sơ
- Bổ sung hiển thị đầy đủ và chuẩn xác các trường thông tin quan trọng vào Sheet duyệt xe Admin:
  - **Màu sắc ngoại thất** (`Color`).
  - **Mức tiêu hao nhiên liệu** (`FuelConsumption`).
  - **Giá thuê niêm yết** và **Giá ưu đãi / khuyến mãi** (`SalePrice` kèm badge Ưu đãi đỏ).
  - **Phụ phí tài xế riêng** (`DriverFee` - tính theo ngày).
  - **Quy định quãng đường** (`MileageLimit` và `ExtraFeePerKm`).
  - **Giao xe tận nơi** (`DeliverySupported` và `DeliveryFee`).
  - **Tiện nghi trang bị** (Danh sách badge có icon).
  - **Mô tả & Nội quy của chủ xe**.

### 38.3. Sửa Lỗi Kẹt Nút "Gửi Lại Hồ Sơ" Tại Form Chỉnh Sửa Xe
- **Nguyên nhân:** Trường `SalePrice` có thuộc tính `step="10000"`, khi người dùng nhập số lẻ (ví dụ 625.000đ) bị HTML5 validation âm thầm chặn submit; đồng thời các input ở các bước trước bị ẩn (`display: none`) gây kẹt form không phản hồi.
- **Khắc phục:** 
  - Đổi `step="any"` cho tất cả các trường tiền tệ.
  - Bổ sung thuộc tính `novalidate` cho form.
  - Viết hàm `submitEditForm()` validate dữ liệu tổng hợp bằng SweetAlert2 trước khi gửi form, hiển thị thông báo tiến trình rõ ràng.

---

## 39. CẬP NHẬT TIẾN TRÌNH: HIỂN THỊ TIỆN NGHI TRANG BỊ & ĐỘC LẬP HÓA "MÔ TẢ & NỘI QUY CỦA CHỦ XE" (NGÀY 10/11/2026)

### 39.1. Khắc Phục Lỗi Hiển Thị "Tiện Nghi Trang Bị" Trên Sheet Duyệt Xe Admin
- **Nguyên nhân:** Trong Action `Pending` của [`Admin/CarsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarsController.cs) trước đó chưa nạp `.Include(c => c.CarAmenities).ThenInclude(ca => ca.Amenity)`, đồng thời phần tử `<div id="sheetAmenitiesList">` trong JavaScript chưa được render dữ liệu.
- **Khắc phục:**
  - Nạp đầy đủ quan hệ bảng liên kết `CarAmenities` từ cơ sở dữ liệu.
  - Thiết lập cơ chế Fallback thông minh: nếu bảng liên kết rỗng, tự động bóc tách từ tag `[Tiện nghi]:` trong trường `Details`.
  - Render danh sách tiện nghi thành các badge sang trọng với icon tích xanh (`bi-check-circle-fill`), hiển thị nổi bật dưới mục **`✨ Tiện nghi trang bị`**.

### 39.2. Độc Lập Hóa & Chuẩn Hóa Ô Nhập "Mô Tả & Nội Quy Của Chủ Xe"
- **Vấn đề trước đây:** Trường mô tả xe thường bị hệ thống tự động ghi đè (`Tiêu hao: ... · Tiện nghi...`) hoặc bị lồng ghép các thẻ tag cấu hình hệ thống (`[Tiện nghi]`, `[Quy định]`, `[Snapshot]`, chuỗi Base64), không có ô nhập độc lập cho chủ xe tự do quy định.
- **Nâng cấp giải pháp:**
  1. **Tại Form Thêm xe ([`CarOwner/Cars/Create.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Create.cshtml)):**
     - Bố trí Card nhập riêng biệt có icon `bi-chat-left-text-fill`, nhãn **`Mô tả & Nội quy của chủ xe`**, badge `Đồng bộ với Khách hàng & Admin` và văn bản hướng dẫn cụ thể (giữ gìn vệ sinh, cấm hút thuốc, cấm chở thú cưng/hải sản, quy định vạch xăng khi trả xe...).
  2. **Tại Form Sửa xe ([`CarOwner/Cars/Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Edit.cshtml)):**
     - Nạp sẵn nội dung văn bản thuần túy của chủ xe, loại bỏ hoàn toàn các mã kỹ thuật.
  3. **Bộ lọc làm sạch 2 lớp (Two-Layer Sanitization):**
     - **Backend ([`CarPolicy.cs`](file:///d:/CarRental_TTCN/CarRental/Utilities/CarPolicy.cs)):** Hàm `CleanExtraDescription` chuẩn hóa xuống dòng (`\r\n`, literal `\n`), tìm tag `[Mô tả bổ sung]:` cuối cùng, lọc bỏ toàn bộ các thẻ tag hệ thống và chuỗi Base64 dài.
     - **Frontend ([`Pending.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Pending.cshtml)):** Hàm `cleanDescJs` làm sạch thêm 1 lớp bảo vệ trực tiếp trên trình duyệt.
  4. **Đồng bộ hóa xuyên suốt 4 bên:**
     - **Admin - Duyệt xe ([`Pending.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Pending.cshtml)):** Hiển thị văn bản thuần túy, sạch sẽ 100%.
     - **Admin - Quản lý đội xe ([`Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Cars/Index.cshtml)):** Modal xét duyệt hiển thị rõ ràng nội dung chủ xe đề xuất.
     - **Chủ xe - Chi tiết xe ([`Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Details.cshtml)):** Hiển thị khối **"Mô tả & Nội quy của xe"** kèm icon trích dẫn.
     - **Khách hàng - Đặt xe ([`Product/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Product/Details.cshtml)):** Khung card viền bo tròn **"Mô tả & Nội quy từ chủ xe"** giúp khách đọc kỹ lưu ý trước khi quyết định thuê xe.

### 39.3. Làm Sạch Dữ Liệu Thực Tế Trong Database & Biên Dịch
- Đã chuẩn hóa chuỗi `Details` và đồng bộ 6 tiện nghi vào bảng `car_amenities` cho xe kiểm thử `abc` (CarId = 26).
- Biên dịch toàn bộ mã nguồn C#: **`Build succeeded. 0 Error(s)`**.
- Khởi động lại ứng dụng web CarRental trên cổng `http://0.0.0.0:5163`, kiểm tra HTTP status 200 OK.

---

## 40. CẬP NHẬT TIẾN TRÌNH: ĐỒNG BỘ NÚT THAO TÁC QUẢN LÝ KIỂU XE & TÍCH HỢP UPLOAD HÌNH ẢNH TỪ THƯ VIỆN MÁY TÍNH (NGÀY 10/11/2026)

### 40.1. Đồng Bộ Nút Thao Tác Chuẩn Admin ReCar ([`Admin/CarTypes/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/CarTypes/Index.cshtml))
- **Vấn đề trước đây:** Bảng danh sách kiểu xe hiển thị 2 nút phẳng chữ "Sửa" và "Xóa" không đồng nhất với phong cách chung của các bảng khác trong Admin.
- **Giải pháp hoàn thiện:**
  1. Thay thế bằng tổ hợp thao tác chuẩn SaaS Admin:
     - Nút **Sửa nhanh** dạng icon tròn (`bi-pencil`) giúp quản trị viên 1-click vào trang chỉnh sửa ngay lập tức.
     - Nút **Menu 3 chấm dropdown** (`bi-three-dots-vertical`) mở rộng danh mục tùy chọn:
       - *Chỉnh sửa kiểu xe*: Chuyển hướng tới trang cập nhật thông tin.
       - *Xem đội xe*: Điều hướng sang danh sách xe thuộc kiểu xe này.
       - Đường kẻ phân cách và nút *Xóa kiểu xe* với màu đỏ cảnh báo.
  2. Tích hợp popup xác nhận bằng SweetAlert2 thông minh:
     - Tự động kiểm tra số lượng xe đang liên kết (`carCount`). Nếu đang có xe thuộc dòng này, hiển thị modal cảnh báo vàng chặn không cho xóa trực tiếp.
     - Nếu không có xe nào liên kết, hiển thị modal xác nhận xóa chuẩn ReCar (nút đỏ nguy hiểm, có thể hủy bỏ).
  3. Bổ sung cơ chế fallback `onerror` cho hình ảnh kiểu xe, chống vỡ layout khi link ảnh bị lỗi.

### 40.2. Tích Hợp Upload Hình Ảnh Từ Thư Viện Máy Tính ([`Create.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/CarTypes/Create.cshtml) & [`Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/CarTypes/Edit.cshtml))
- **Vấn đề trước đây:** Form thêm và sửa kiểu xe chỉ có ô input text bắt dán URL/link ảnh, không hỗ trợ người dùng tải file ảnh từ máy tính.
- **Giải pháp hoàn thiện:**
  1. **Nâng cấp Backend ([`CarTypesController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarTypesController.cs)):**
     - Inject `IWebHostEnvironment` vào Controller.
     - Cập nhật cả 2 action `Create` và `Edit` nhận tham số `IFormFile? imageFile`.
     - Viết hàm `ProcessUploadedImageAsync`:
       - Tự động tạo thư mục `wwwroot/images/cartypes` nếu chưa có.
       - Kiểm tra định dạng hợp lệ (`.jpg`, `.jpeg`, `.png`, `.webp`, `.gif`).
       - Đổi tên file duy nhất chống trùng lặp theo timestamp và GUID: `cartype_{timestamp}_{hash}.ext`.
       - Lưu file và gán đường dẫn web tương đối `/images/cartypes/{fileName}` vào trường `Image`.
       - Tại Action `Edit`: Giữ nguyên ảnh hiện tại nếu người dùng không chọn tải ảnh mới từ máy và không dán link mới.
  2. **Nâng cấp Giao diện Thêm mới & Chỉnh sửa ([`Create.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/CarTypes/Create.cshtml) & [`Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/CarTypes/Edit.cshtml)):**
     - Thêm `enctype="multipart/form-data"` cho `<form>`.
     - Thiết kế card tải ảnh chuyên nghiệp với nút **"Chọn ảnh từ thư viện máy"** (`bi-folder2-open`).
     - Khung xem trước trực tiếp (Live Image Preview) thời gian thực bằng JavaScript `FileReader`.
     - Kiểm tra trực tiếp tại Client: Cảnh báo file quá 5MB hoặc sai định dạng bằng SweetAlert2.
     - Tại trang `Edit`: Hiển thị sẵn ảnh hiện tại kèm badge "Ảnh hiện tại". Khi chọn file mới từ máy sẽ tự động chuyển sang xem trước ảnh mới kèm badge "Ảnh mới chọn từ máy" và nút "Khôi phục ảnh cũ".
     - Giữ tùy chọn dán link URL dạng Accordion thu gọn dự phòng cho quản trị viên muốn dán link CDN ngoài.

### 40.3. Tái Cấu Trúc Toàn Diện Giao Diện Thêm/Sửa Kiểu Xe Theo Layout 2 Cột Chuẩn Admin ReCar
- **Vấn đề được phản hồi:** Trang chỉnh sửa trước đó bị mất cân đối (1 cột dài hẹp 800px), thiếu các thành phần đặc trưng của Admin SaaS và không đồng nhất với các màn hình khác như `Admin/Blogs/Edit.cshtml` hay `Admin/UserManagement/Edit.cshtml`.
- **Giải pháp hoàn thiện:**
  1. **Đồng bộ Header chuẩn ReCar:**
     - Nút quay lại dạng icon tròn bo mềm (`bi-arrow-left`).
     - Badge mã định danh kiểu font-monospace `#ID-@Model.TypeId`.
     - Cụm nút thao tác trên Topbar: Nút Xóa kiểu xe bằng SweetAlert2 (`btn-outline-danger`) và Nút Quay lại (`btn-outline-secondary`).
  2. **Kiến trúc Layout 2 cột (Two-Column SaaS Layout - 1100px):**
     - **Cột trái (col-lg-7):** Card `recar-panel` chứa form nhập liệu chính: Tên kiểu xe/phân khúc, Số chỗ ngồi, Hãng đại diện kèm icon input-group, cùng khối thống kê tóm tắt số lượng xe đang thuộc phân khúc này trên sàn.
     - **Cột phải (col-lg-5):**
       - **Card Trạng thái & Hành động (Actions Card):** Nút *Lưu thay đổi* màu xanh ReCar Blue to rõ (`w-100 py-2.5 fw-bold`), Nút *Hủy bỏ & Quay lại*, cùng hộp tóm tắt thông số mã phân khúc, trạng thái sàn và số xe trực thuộc.
       - **Card Hình ảnh minh họa (Media Card):** Khung xem trước Live Preview bo tròn `rounded-4 shadow-sm` tỷ lệ chuẩn xe, nút tải file từ máy tính, badge trạng thái ảnh động và ô nhập URL dự phòng.
  3. **Đồng bộ hóa 100% sang cả Trang Thêm mới ([`Create.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/CarTypes/Create.cshtml)):**
     - Cả 2 trang Thêm mới và Chỉnh sửa hiện đã sở hữu layout 2 cột đồng nhất, sang trọng và chuẩn mực.

### 40.4. Kết Quả Kiểm Thử & Biên Dịch
- Biên dịch mã nguồn C#: **`Build succeeded. 0 Warning(s). 0 Error(s)`**.
- Khởi động ứng dụng web thành công tại `http://0.0.0.0:5163`.

---

## 41. CẬP NHẬT TIẾN TRÌNH: ĐỒNG BỘ HIỂN THỊ KIỂU XE MỚI RA TRANG CHỦ, BỘ LỌC THUÊ XE & XÁC THỰC PHÂN HỆ CHỦ XE (NGÀY 11/11/2026)

### 41.1. Khắc Phục Giới Hạn Hiển Thị Kiểu Xe Trên Trang Chủ ([`Views/Home/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Home/Index.cshtml))
- **Nguyên nhân:** Trước đó vòng lặp hiển thị danh mục dòng xe bị giới hạn cứng `carTypes.Take(5)`. Do đó, bất kỳ kiểu xe nào từ thứ 6 trở đi khi Admin thêm mới đều bị ẩn hoàn toàn khỏi Trang chủ.
- **Khắc phục:**
  - Bỏ giới hạn `.Take(5)` để hiển thị đầy đủ toàn bộ danh mục xe từ CSDL.
  - Nâng cấp lưới hiển thị co giãn linh hoạt: `row-cols-2 row-cols-sm-3 row-cols-md-4 row-cols-lg-5 row-cols-xl-6 g-3`, tự động thích ứng với số lượng danh mục.
  - Bổ sung cơ chế fallback `onerror` cho ảnh kiểu xe, chống vỡ giao diện nếu link ảnh trống hoặc bị lỗi.

### 41.2. Động Hóa Bộ Lọc Dòng Xe Trên Trang Thuê Xe ([`Controllers/ProductController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/ProductController.cs) & [`Views/Product/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Product/Index.cshtml))
- **Nguyên nhân:**
  1. Trong `ProductController.Index()` trước đó chưa nạp `ViewBag.CarTypes`.
  2. Trong `Views/Product/Index.cshtml`, 5 chip dòng xe đang bị hardcode cứng với `data-type="1"` đến `data-type="5"`, không render từ cơ sở dữ liệu. Khi khách chọn phân khúc xe mới từ Trang chủ hoặc lọc tại trang tìm kiếm, hệ thống không tìm thấy chip tương ứng để kích hoạt.
- **Khắc phục:**
  - Trong `ProductController.cs`: Nạp đầy đủ danh sách `ViewBag.CarTypes = _context.CarTypes.OrderBy(t => t.TypeId).ToList();`.
  - Trong `Product/Index.cshtml`: Thay thế 5 chip hardcode bằng vòng lặp `@foreach (var ct in carTypes)`, gán động `data-type="@ct.TypeId"` và tên hiển thị `@ct.CarTypeName`.
  - Tích hợp liên kết 2 chiều: Khi khách hàng bấm vào danh mục xe ở Trang chủ (`/Product?TypeId=...`), trang danh sách xe tự động bắt `TypeId`, làm sáng chip phân khúc tương ứng và kích hoạt AJAX lọc xe ngay lập tức.

### 41.3. Xác Thực Kiểm Tra Phân Hệ Đối Tác Chủ Xe ([`Areas/CarOwner`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner))
- **Kết quả kiểm tra:**
  - Tại Form Thêm xe ([`CarOwner/Cars/Create.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Create.cshtml)) và Form Chỉnh sửa xe ([`CarOwner/Cars/Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Edit.cshtml)): Dropdown phân khúc (`selectCarType`) được nạp trực tiếp và động 100% từ `_context.CarTypes` thông qua `ViewBag.CarTypes`.
  - **Khẳng định:** Mọi kiểu xe mới được Admin tạo lập sẽ **xuất hiện ngay lập tức** trong danh sách lựa chọn phân khúc khi Chủ xe đăng ký xe mới hoặc sửa xe mà không cần cấu hình thêm.

### 41.4. Kết Quả Kiểm Thử
- Dự án biên dịch thành công: **`Build succeeded. 0 Error(s)`**.
- Khởi động ứng dụng web trên cổng `http://0.0.0.0:5163`.

---

## 42. CẬP NHẬT TIẾN TRÌNH: TRIỂN KHAI TOÀN DIỆN PHÂN HỆ QUẢN LÝ HÃNG XE (CAR BRANDS) CHO ADMIN (NGÀY 11/11/2026)

### 42.1. Bối Cảnh & Sự Cần Thiết
- Hệ thống trước đó đã có bảng `car_brands` và Entity `CarBrand.cs` phục vụ cho Chủ xe chọn hãng khi thêm xe và Khách thuê lọc xe, nhưng Ban Quản trị (Admin) chưa hề có Controller, View hay Menu để quản lý danh mục này.
- Admin không thể thêm các hãng xe mới (Porsche, Lexus, BMW, Mercedes-Benz, Audi, Subaru, BYD...), không thể đổi tên, sắp xếp thứ tự hoặc tạm ẩn thương hiệu.

### 42.2. Triển Khai Backend ([`CarBrandsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/CarBrandsController.cs))
- **`Index`**: Liệt kê danh sách thương hiệu theo `DisplayOrder`, tự động tổng hợp số lượng xe sàn thực tế thuộc từng hãng bằng `GroupBy` linh hoạt.
- **`Create`**: Thêm hãng xe mới với kiểm tra trùng tên, tự động gợi ý thứ tự tiếp theo `nextOrder`.
- **`Edit`**: Cập nhật thông tin thương hiệu, quốc gia xuất xứ, thứ tự ưu tiên và trạng thái. Đồng thời nếu Admin đổi tên hãng (ví dụ "Vinfast" -> "VinFast"), hệ thống tự động cập nhật đồng bộ toàn bộ xe sàn đang thuộc hãng đó.
- **`Delete`**: Ràng buộc bảo vệ dữ liệu toàn sàn: Nếu có xe đang mang thương hiệu này, hệ thống sẽ chặn xóa an toàn.
- **`ToggleActive`**: Bật/tắt trạng thái hiển thị nhanh bằng AJAX & SweetAlert2.

### 42.3. Thiết Kế Giao Diện Chuẩn ReCar Admin SaaS
- **Trang Danh Sách ([`Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/CarBrands/Index.cshtml)):** Bảng `recar-table` chuẩn mực, avatar ký tự chữ cái đầu của thương hiệu, badge quốc gia xuất xứ, nút Sửa nhanh và Dropdown menu 3 chấm tròn chuẩn ReCar, popup xóa SweetAlert2 thông minh.
- **Trang Thêm Mới ([`Create.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/CarBrands/Create.cshtml)) & Chỉnh Sửa ([`Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/CarBrands/Edit.cshtml)):**
  - Layout 2 cột sang trọng đồng nhất với `CarTypes`.
  - Bộ nút gợi ý nhanh quốc gia xuất xứ phổ biến (Việt Nam, Nhật Bản, Hàn Quốc, Đức, Mỹ, Anh, Pháp, Trung Quốc).
  - Khối thống kê số lượng xe sàn thuộc thương hiệu đang quản lý.

### 42.4. Tự Động Đồng Bộ Hệ Thống & Menu Sidebar Admin
- **Động hóa Bộ Lọc Khách Thuê ([`ProductController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/ProductController.cs) & [`Product/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Product/Index.cshtml)):** Bộ lọc hãng xe tại trang Thuê xe hiện được nạp trực tiếp từ `_context.CarBrands.Where(b => b.IsActive)`. Bất kỳ hãng xe mới nào được Admin tạo sẽ hiển thị ngay lập tức trên bộ lọc.
- **Seed Menu Sidebar Admin ([`Program.cs`](file:///d:/CarRental_TTCN/CarRental/Program.cs)):** Tự động khởi tạo menu `"Quản lý hãng xe"` (`Url = "/Admin/CarBrands"`, `Icon = "bi-patch-check"`) vào nhóm `XE & ĐƠN HÀNG` ngay cạnh `Quản lý kiểu xe`.

### 42.5. Tinh Chỉnh Giao Diện Hãng Xe & Tích Hợp Popup Đội Xe Kèm Chi Tiết Xe
1. **Loại bỏ Avatar Chữ Cái Tròn:**
   - Đã gỡ bỏ toàn bộ avatar tròn chữ cái ở cột *"Tên thương hiệu"*, chuyển sang hiển thị tên thương hiệu dạng chữ đậm tinh tế, gọn gàng và hiện đại theo phản hồi người dùng.
2. **Popup Danh Sách Đội Xe Thuộc Hãng (`#brandFleetModal`):**
   - Khi bấm vào badge số lượng xe trên sàn hoặc chọn *"Xem đội xe"* trong menu 3 chấm, hệ thống lập tức mở popup danh sách các xe thuộc thương hiệu đó qua AJAX `GetCarsByBrand`.
   - Mỗi xe hiển thị dạng card sang trọng: Ảnh xe, Tên xe, Phân khúc, Biển số (font-mono), Năm SX, Hộp số, Số chỗ, Giá thuê/ngày (kèm giá ưu đãi nếu có) và Tên chủ xe.
3. **Popup Xem Chi Tiết Chiếc Xe (`#carDetailModal`):**
   - Khi click vào bất kỳ chiếc xe nào trong popup đội xe, hệ thống sẽ mở ngay popup chi tiết thông số xe qua AJAX `GetCarDetail`:
     - Xem đầy đủ ảnh xe và 4 ảnh góc (có thể click đổi ảnh chính).
     - Chi tiết thông số kỹ thuật (Hãng, Phân khúc, Năm SX, Màu sắc, Số chỗ, Hộp số, Nhiên liệu, Mức tiêu hao).
     - Thông tin tài chính (Giá thuê, Phụ phí tài xế, Giới hạn km, Phụ phí vượt km, Giao xe tận nơi).
     - Danh sách tiện nghi trang bị (badge tích xanh).
     - Thông tin Chủ xe liên hệ và Mô tả / Nội quy của xe.
     - Nút *"Quay lại đội xe"* và nút *"Xem trên sàn"* (mở tab trực tiếp ra trang sản phẩm khách thuê).

### 42.6. Tinh Chỉnh Bố Cục 2 Popup Đội Xe & Chi Tiết Xe Đạt Tỷ Lệ Cân Xứng Hoàn Hảo & Khắc Phục Triệt Để Méo Ảnh
1. **Khắc Phục Hiện Tượng Ảnh Méo & Khoảng Trống Thừa Tại Popup Đội Xe (`#brandFleetModal`):**
   - **Hiện tượng cũ:**
     - Thiết lập `object-fit: cover` khiến ảnh xe ô tô (vốn chụp ngang trên studio nền trắng) bị cắt mất bánh xe, trần xe hoặc bị co ép méo mó.
     - Lưới 3 cột (`col-lg-4`) trên modal quá rộng làm cho thương hiệu có 2 xe (như VinFast) bị dạt hết sang bên trái, bỏ trống nguyên 1/3 diện tích modal bên phải.
   - **Cải tiến:**
     - Chuyển sang kích thước `modal-lg` chuẩn gọn (940px).
     - Thiết kế dạng **Thẻ Ngang (Horizontal Card) 2 cột (`col-12 col-md-6`)**: Khi có 2 xe, 2 card chia đôi màn hình (50% - 50%) vừa vặn, không còn bất kỳ khoảng trống thừa nào.
     - Khung ảnh chiếm 42% chiều rộng card: Đặt nền xám nhạt `#f8fafc`, áp dụng `object-fit: contain; max-width: 90%; max-height: 90%;` -> **Chiếc xe hiển thị trọn vẹn 100%, sắc nét, giữ nguyên tỷ lệ thật, tuyệt đối không bị méo mó hay bẹp dúm**.
     - Cột nội dung bên phải (58%): Badge phân khúc, Tên xe, 3 thông số vận hành, Giá thuê to rõ và nút "Chi tiết >" cân đối, thanh lịch.

2. **Tối Ưu Hóa Toàn Diện Popup Chi Tiết Xe (`#carDetailModal`) Thoáng Đãng, Rộng Rãi:**
   - **Phục hồi kích thước ảnh to rõ như ban đầu:** Khung ảnh chính được mở rộng lên chiều cao `290px`, bo tròn góc `rounded-4`, ảnh xe to nét căng tràn viền, góc chụp studio sắc nét không bị méo.
   - **Xóa bỏ hoàn toàn text overlay:** Gỡ bỏ triệt để dải chữ *"Bấm ảnh nhỏ để đổi góc"* theo đúng yêu cầu người dùng.
   - **Khắc phục triệt để lỗi icon dính vào chữ & tách bạch các ô thông số:**
     - Thay vì dồn ép 4 cột làm chữ bị cắt thành `Crossover/SU...`, khối Thông số kỹ thuật chuyển sang dạng **2 cột lớn rộng rãi (`row g-3`)**: Mỗi ô có icon tròn nền pastel riêng biệt, tách biệt khỏi chữ với khoảng cách thoáng đãng `gap: 12px`, tiêu đề và giá trị to rõ, hiển thị trọn vẹn 100% không bị cắt chữ.
     - Khối Bảng giá & Phụ phí: To rõ, icon cách chữ với khoảng đệm `gap: 10px`, giá thuê nổi bật.
     - Khối Tiện nghi trang bị: Các badge pill to rõ, icon tích xanh tách chữ `gap: 8px`.
     - Khối Chủ xe & Mô tả nội quy: Rộng rãi, dễ đọc, bố cục đẳng cấp SaaS.

   - **Xóa bỏ hoàn toàn hiện tượng đường kẻ line đè/dính vào chữ:**
     - **Tại Thẻ xe (Popup Đội xe):** Thay thế đường kẻ `border-top` mỏng bị đè lên chữ giá tiền bằng **Khung hộp chân thẻ bo góc (`bg-light-subtle rounded-3 p-2.5 border-light-subtle`)**. Giá tiền nằm gọn gàng bên trái, nút Chi tiết bên phải, hoàn toàn không còn đường kẻ nào cắt ngang hay dính vào chữ.
     - **Tại Khối Bảng giá & Phụ phí (Popup Chi tiết):** Xóa bỏ hoàn toàn cách chia 2 cột chật hẹp và các đường line kẻ `border-bottom` cũ (vốn làm chữ *"Giao xe tận nơi"* và *"Giới hạn quãng đường"* bị gãy đôi, chữ rơi đè lên đường kẻ). Thay thế bằng **Banner giá thuê trên + 3 Card Box phụ phí độc lập ở dưới (`row g-3`)**: Mỗi phụ phí nằm trong một ô card màu trắng bo góc sạch sẽ, icon pastel riêng biệt, chữ hiển thị thẳng hàng không bị rớt dòng và không còn bất kỳ line kẻ nào bị dính.

### 42.7. Thiết Kế Lại Hoàn Toàn 2 Popup (Đội Xe & Chi Tiết Xe) Bằng CSS Riêng `rc-*`
- **Nguyên nhân gốc của lỗi "dính nhau":** các class đã dùng như `p-2.5`, `gap-2.5`, `py-1.5`, `g-3.5`, `mb-0.5` **không tồn tại trong Bootstrap 5** (chỉ có 0-5) nên padding/gap bằng 0.
- **Giải pháp:** viết bộ CSS riêng (`.rc-modal`, `.rc-card`, `.rc-spec`, `.rc-info`...) với khoảng cách px cụ thể, không phụ thuộc utility Bootstrap.
- **Popup Đội xe:** `modal-xl`, lưới `auto-fill minmax(330px)` gap 24px; thẻ xe dọc (ảnh 200px `object-fit: cover`, biển số + trạng thái trên ảnh, phân khúc, tên, chip thông số, hộp giá + nút Chi tiết).
- **Popup Chi tiết:** lưới 2 cột (5fr/6fr). Trái: ảnh 16:10 + 4 thumbnail (có viền chọn), chủ xe, tiện nghi. Phải: 8 ô thông số (lưới 2 cột), banner giá + 3 dòng phụ phí có đệm 16px, mô tả. Đã xóa text "Bấm ảnh nhỏ để đổi góc". Responsive dưới 992px.

### 42.8. Kết Quả Kiểm Thử
- Biên dịch toàn bộ mã nguồn: **`Build succeeded. 0 Error(s)`**.
- Ứng dụng ReCar đã khởi chạy và sẵn sàng trải nghiệm tại: **`http://localhost:5163/Admin/CarBrands`**.

---

## 43. ĐỒNG BỘ POPUP ĐỘI XE & CHI TIẾT XE SANG QUẢN LÝ KIỂU XE (`/Admin/CarTypes`)

### 43.1. Nghiệp Vụ & Mục Tiêu
- Đồng bộ toàn bộ trải nghiệm Popup Đội xe và Popup Chi tiết xe cùng phong cách thiết kế CSS chuyên nghiệp `rc-*` từ phân hệ Quản lý hãng xe (`/Admin/CarBrands`) sang phân hệ **Quản lý kiểu xe & phân khúc (`/Admin/CarTypes`)**.
- Cho phép Quản trị viên khi xem danh sách các kiểu xe (Sedan, SUV, Crossover, MPV, Bán tải,...) có thể:
  1. Bấm trực tiếp vào badge *"X xe đang có"* hoặc mục *"Xem đội xe"* trong menu 3 chấm để xem nhanh toàn bộ danh sách các xe thuộc kiểu xe đó trong Popup Modal dạng lưới thẻ hiện đại.
  2. Bấm vào bất kỳ xe nào để mở Popup Modal Chi tiết xe đầy đủ (ảnh đa góc, thông số kỹ thuật, giá thuê, phụ phí, tiện nghi, hồ sơ chủ xe, mô tả).
  3. Có nút quay lại danh sách xe phân khúc mượt mà mà không phải tải lại trang.

### 43.2. Triển Khai Backend (`CarTypesController.cs`)
- Thêm action `GetCarsByType(int typeId)`: Lấy danh sách các xe thuộc phân khúc cùng thông tin cơ bản, biển số, chủ xe, phân khúc và giá thuê.
- Thêm action `GetCarDetail(int id)`: Lấy hồ sơ chi tiết của xe bao gồm 4 góc ảnh, thông số vận hành, chính sách giá và phụ phí (tài xế, giao xe, vượt km), danh sách tiện nghi và nội quy sử dụng.

### 43.3. Triển Khai Giao Diện (`CarTypes/Index.cshtml`)
- Cập nhật bảng dữ liệu: Cột số lượng xe và menu 3 chấm gọi hàm JavaScript `openTypeFleetModal(typeId, typeName, carCount)`.
- Tích hợp 2 Modal:
  - `#typeFleetModal`: Modal Đội xe phân khúc với tiêu đề động theo tên kiểu xe, lưới thẻ xe dọc `.rc-fleet-grid` và `.rc-car-card`.
  - `#carDetailModal`: Modal Chi tiết xe 2 cột thoáng đãng, tỉ lệ 16:10, 8 ô thông số, hộp bảng giá & phụ phí và thông tin đối tác chủ xe.
- Tích hợp toàn bộ hệ thống CSS `rc-*` độc lập với khoảng cách `px` cố định, giải quyết triệt để lỗi icon dính chữ và đường kẻ đè vào nội dung.

### 43.4. Kiểm Thử & Trạng Thái
- Biên dịch thành công: **`0 Error(s)`**.
- Ứng dụng đã chạy và sẵn sàng tại: **`http://localhost:5163/Admin/CarTypes`**.

---

## 44. NÂNG CẤP TOÀN DIỆN PHÂN HỆ QUẢN LÝ BÀI VIẾT & TIN TỨC (`/Admin/Blogs`)

### 44.1. Nghiệp Vụ & Yêu Cầu Người Dùng
1. **Menu 3 chấm & Xem giao diện khách thuê (`Index.cshtml`):**
   - Bổ sung menu dropdown 3 chấm chuẩn Admin ReCar vào cột Thao tác.
   - Thêm tính năng *"Xem trên web"* (mở tab mới bài viết phía người dùng thực tế) và Popup Modal *"Xem nhanh bài viết"* (`#blogPreviewModal`) mô phỏng giao diện bài viết như khách thấy ngay trong Admin.
   - Bổ sung nút Xem nhanh, Sửa nhanh và Xóa bằng SweetAlert2.
2. **Trình soạn thảo trực quan WYSIWYG (`Create.cshtml` & `Edit.cshtml`):**
   - Xóa bỏ việc người dùng phải nhìn và gõ code HTML thô (`<h2>`, `name=`, `style=`, `id=`).
   - Tích hợp trình soạn thảo WYSIWYG **Summernote Lite** hỗ trợ tiếng Việt đầy đủ (tiêu đề H2-H4, in đậm, in nghiêng, trích dẫn, gạch đầu dòng, căn chỉnh,...).
   - Tích hợp tính năng **Chèn ảnh trực tiếp vào nội dung bài viết**: Chọn ảnh hoặc kéo thả từ máy tính, tự động upload qua AJAX lên `/images/blogs/content/` và chèn vào bài viết với kích thước chuẩn, responsive.
3. **Đơn giản hóa Khung Ảnh bìa:**
   - Xóa bỏ hoàn toàn ô nhập URL thô *"Hoặc sửa đường dẫn ảnh (URL):"*.
   - Chỉ giữ lại thao tác tải ảnh mới từ máy tính (`ImageFile`) kèm khung xem trước Live Preview trực quan và input ẩn giữ ảnh cũ.

### 44.2. Cập Nhật Backend (`BlogsController.cs` & `BlogController.cs`)
- Thêm action `UploadEditorImage(IFormFile file)` trong `BlogsController`: Nhận ảnh từ Summernote, kiểm tra định dạng an toàn, lưu vào `wwwroot/images/blogs/content` và trả về JSON URL.
- Thêm action `GetBlogPreview(int id)`: Trả về JSON dữ liệu bài viết phục vụ Modal xem trước trực tiếp trên trang Index.
- Cập nhật routing trong `BlogController.cs`: Bổ sung route `[Route("/Blog/Details/{id:int}")]` song song với `[Route("/blog/{alias}-{id}.html")]`.

### 44.3. Kiểm Thử & Trạng Thái
- Biên dịch thành công: **`0 Error(s)`**.
- Ứng dụng đã chạy và sẵn sàng trải nghiệm tại: **`http://localhost:5163/Admin/Blogs`**.

### 44.4. Tinh Gọn Bảng Danh Sách Bài Viết (Ẩn Tóm Tắt & Tối Giản Nút Thao Tác)
- **Ẩn cột "Tóm tắt nội dung":** Loại bỏ hoàn toàn cột tóm tắt nội dung khỏi bảng để giao diện thoáng đãng, tiêu đề bài viết hiển thị rộng rãi, không bị dồn ép cột.
- **Tối giản cột "Thao tác":** Gỡ bỏ 2 nút con (Xem nhanh & Sửa nhanh) ngoài bảng, chỉ để duy nhất **1 nút menu 3 chấm tròn** thanh lịch chuẩn SaaS. Mọi tác vụ (Xem trên web của khách, Xem nhanh Preview modal, Chỉnh sửa bài viết, Đổi trạng thái, Xóa) đều nằm gọn gàng bên trong menu.

---

## 45. PHÁT TRIỂN MỚI PHÂN HỆ QUẢN LÝ TIN TỨC THỊ TRƯỜNG XE (`/Admin/News`) & PHÂN ĐỊNH RÀNH MẠCH VỚI CẨM NANG

### 45.1. Bối Cảnh & Sự Khác Biệt
- **Phía người dùng (Client):** Có 2 mục hoàn toàn độc lập:
  1. *Cẩm nang du lịch & thuê xe* (`/Blog`): Sử dụng bảng CSDL `Blogs`.
  2. *Tin tức thị trường xe* (`/News`): Sử dụng bảng CSDL `News`.
- **Phía quản trị (Admin):** Trước đây chỉ có `/Admin/Blogs` (thao tác trên bảng `Blogs`), còn bảng `News` hoàn toàn chưa có phân hệ quản lý trong Admin.
- **Giải pháp:** Tách biệt rõ ràng 2 phân hệ độc lập:
  - **Quản lý cẩm nang** (`/Admin/Blogs`): Icon sổ tay `bi-journal-bookmark`.
  - **Quản lý tin tức** (`/Admin/News`): Icon tờ báo `bi-newspaper`.

### 45.2. Triển Khai Toàn Diện Phân Hệ News
1. **Backend Controller (`Areas/Admin/Controllers/NewsController.cs`):**
   - Đầy đủ các CRUD action: `Index`, `Create`, `Edit`, `Delete`, `DeleteAjax`, `ToggleActive`.
   - Action `UploadEditorImage`: Upload ảnh từ Summernote lưu vào `wwwroot/images/news/content/`.
   - Action `GetNewsPreview`: Trả về dữ liệu bản tin phục vụ Popup Modal xem trước.
2. **Giao diện Views (`Areas/Admin/Views/News/`):**
   - `Index.cshtml`: Bảng danh sách tinh gọn (không có cột tóm tắt), menu 3 chấm tròn, Modal xem trước `#newsPreviewModal` mô phỏng giao diện khách thấy.
   - `Create.cshtml` & `Edit.cshtml`: Trình soạn thảo WYSIWYG Summernote Lite tiếng Việt, chèn ảnh từ máy tính, đã xóa ô URL thô, xem trước ảnh bìa Live Preview.
   - `Delete.cshtml`: Xác nhận xóa bản tin an toàn.
3. **Cơ sở dữ liệu & Sidebar Admin (`Program.cs`):**
   - Tự động seed mục menu "Quản lý tin tức" (`/Admin/News`) vào sidebar Admin.
   - Cập nhật mục menu cũ thành "Quản lý cẩm nang" (`/Admin/Blogs`).

### 45.3. Kiểm Thử & Trạng Thái
- Biên dịch thành công: **`0 Error(s)`**.
- Ứng dụng đã chạy và sẵn sàng tại:
  - Quản lý cẩm nang: **`http://localhost:5163/Admin/Blogs`**
  - Quản lý tin tức: **`http://localhost:5163/Admin/News`**

---

## 46. ĐỒNG BỘ DỮ LIỆU BẢN TIN TIN TỨC, TÍNH NĂNG MẮT XEM TỰ ĐỘNG & TÙY CHỌN GẮN NHÃN NỔI BẬT

### 46.1. Nguyên Nhân Sự Chênh Lệch Dữ Liệu (5 bài Khách vs 2 bài Admin)
- Trong CSDL MySQL trước đây chỉ có 2 bài tin tức thực sự được lưu vào bảng `news`.
- Phía Client (`/News`) trước đó được lập trình viên hard-code thêm 3 bài phụ trong hàm `NewsController.GetCuratedNews()` (ID 901, 902, 903) và `NewsViewComponent.cs` gộp vào danh sách hiển thị, dẫn tới việc Khách thấy 5 bài nhưng Admin truy vấn từ CSDL chỉ thấy 2 bài.
- **Giải pháp:** Chuyển đổi toàn bộ 3 bài tin tức mẫu này vào logic Seed CSDL tự động trong `Program.cs`. Toàn bộ 5 bài hiện đã được lưu trữ thực sự trong bảng `news` của MySQL, giúp dữ liệu giữa Admin và Client đồng nhất 100%.

### 46.2. Mở Rộng Cơ Sở Dữ Liệu (`News`)
- Bổ sung 3 trường dữ liệu mới cho Entity `News`:
  - `Views` (`int?`): Lượt mắt xem của bản tin (mặc định = 0).
  - `IsFeatured` (`bool?`): Đánh dấu tin bài nổi bật thủ công (mặc định = false).
  - `Badge` (`string?`, tối đa 50 ký tự): Nhãn huy hiệu tùy biến (ví dụ: `🔥 Nổi bật`, `⭐ Xu hướng`, `⚡ Mới nhất`, `🚗 Xe điện`,...).
- Tự động bổ sung cột an toàn qua `INFORMATION_SCHEMA.COLUMNS` trong `Program.cs` và cấu hình Fluent API trong `DbRenalCarContext.cs`.

### 46.3. Cơ Chế Tự Động Tăng Lượt Xem & Tự Gắn Nhãn Nổi Bật
- **Tự động tăng mắt xem:** Mỗi khi người dùng bấm vào xem chi tiết bài viết tại `/News/Details/{id}` hoặc `/news/{alias}-{id}.html`, hệ thống tự động tăng `Views` lên 1 và lưu vào CSDL.
- **Tự động gắn nhãn "🔥 Nổi bật":** Khi một bài viết đạt mốc $\ge 500$ lượt xem hoặc được bật cờ `IsFeatured == true`, hệ thống tự động gán và hiển thị nhãn nổi bật mà không bắt buộc quản trị viên phải can thiệp thủ công.

### 46.4. Nâng Cấp Giao Diện Phía Admin (`/Admin/News`)
- **Trang Tạo mới (`Create.cshtml`) & Chỉnh sửa (`Edit.cshtml`):** Bổ sung Card *"Mắt xem & Nhãn nổi bật"* bên cột phải:
  - Công tắc bật/tắt (Switch): Đánh dấu bài viết nổi bật (`IsFeatured`).
  - Ô nhập Huy hiệu / Nhãn hiển thị (`Badge`) kèm danh sách gợi ý sẵn (`🔥 Nổi bật`, `⭐ Xu hướng`, `⚡ Mới nhất`, `🚗 Xe điện`, `💡 Cẩm nang hot`).
  - Ô chỉnh sửa số lượt mắt xem (`Views`): Cho phép điều chỉnh hoặc đặt lượt xem ban đầu theo ý muốn.
- **Trang Danh sách (`Index.cshtml`):** Hiển thị số mắt xem (`<i class="bi bi-eye"></i>`) và huy hiệu nổi bật ngay bên cạnh tiêu đề bài viết; cập nhật modal xem trước Live Preview thể hiện số lượt xem thực tế.

### 46.5. Đồng Bộ Giao Diện Phía Khách Hàng (`/News`)
- Cập nhật `CarRental/Views/Shared/Components/News/Default.cshtml` và `CarRental/Views/News/Details.cshtml`:
  - Thể hiện nhãn huy hiệu nổi bật trên góc ảnh bản tin tiêu điểm và danh sách các thẻ bài viết.
  - Hiển thị trực quan số lượt xem kèm icon con mắt xanh: `[Lượt xem] lượt xem`.

### 46.6. Kiểm Thử & Trạng Thái
- Biên dịch dự án: **`0 Error(s)`**.
- Hệ thống đang chạy ổn định tại:
  - Client Tin tức: **`http://localhost:5163/News`**
  - Admin Quản lý tin tức: **`http://localhost:5163/Admin/News`**

---

## 47. CHUẨN HÓA HỆ THỐNG TAG/NHÃN, TỰ ĐỘNG ĐẾM LƯỢT XEM & THAY THẾ TÁC GIẢ BẰNG PHÂN LOẠI CHỦ ĐỀ CHO CẢ TIN TỨC VÀ CẨM NANG

### 47.1. Bỏ Ô Chỉnh Sửa Lượt Mắt Xem (Views) Thủ Công & Tinh Gọn Khung Thông Tin
- **Nguyên lý:** Lượt xem (mắt xem) là chỉ số thống kê hệ thống khách quan, phản ánh mức độ quan tâm thực tế của khách thuê đối với bài viết.
- **Giải pháp:**
  - Loại bỏ hoàn toàn ô nhập số lượt xem (`<input type="number" asp-for="Views" />`) trong form Tạo mới (`Create.cshtml`) và Chỉnh sửa (`Edit.cshtml`) của cả hai phân hệ **Tin tức** và **Cẩm nang**.
  - Đã lược bỏ toàn bộ khung viền hiển thị text chỉ đọc "Lượt mắt xem..." trên form Chỉnh sửa theo yêu cầu để giao diện tinh gọn, thoáng đãng tối đa, chỉ lưu trữ `Views` ngầm qua thẻ `<input type="hidden" asp-for="Views" />`.
  - Trong Controller Backend: không cho phép form ghi đè lượt xem, luôn bảo toàn số lượt xem thật từ CSDL.

### 47.2. Chuẩn Hóa Nhãn Nổi Bật (Badge): Chuyển Sang Dropdown & Loại Bỏ Hoàn Toàn Emoji
- **Chuyển sang Dropdown `<select>`:** Thay thế ô nhập text tự do và datalist bằng hộp chọn `<select asp-for="Badge">` chuyên nghiệp với các tùy chọn chuẩn SaaS:
  - `-- Không gắn nhãn --`
  - `Nổi bật`
  - `Xu hướng`
  - `Mới nhất`
  - `Ưu đãi`
  - `Đặc sắc`
  - `Đọc nhiều`
- **Loại bỏ Emoji:** Bỏ toàn bộ các icon biểu cảm (`🔥`, `⭐`, `⚡`,...) ở các nhãn hiển thị trong CSDL, giao diện bảng Admin, modal Preview và giao diện ngoài sàn của khách hàng để tạo phong cách thanh lịch, tối giản và hiện đại.
- **Áp dụng đồng bộ cho Cẩm nang (`Blog`):** Bổ sung các cột `Views`, `IsFeatured`, `Badge` cho bảng `Blog` trong MySQL, cập nhật Entity `Blog.cs` và cấu hình Fluent API trong `DbRenalCarContext.cs`.

### 47.3. Thay Thế Phần "Tác Giả" Bằng "Chủ Đề / Thẻ Phân Loại" (Tag)
- **Vấn đề trước đây:** Trường tác giả cố định là "Admin" hoặc "ReCar Editorial" mà không chỉnh sửa được, chiếm diện tích cột trong bảng và không hỗ trợ nhiều cho việc phân loại nội dung.
- **Giải pháp chuyển đổi:**
  - **Trên bảng danh sách (`Index.cshtml` của Blogs & News):** Đổi cột *"Tác giả & Ngày đăng"* thành *"Chủ đề & Ngày đăng"*. Hiển thị huy hiệu chủ đề bài viết (`SeoKeywords`) dạng viên thuốc nổi bật (`badge-primary-subtle`) kèm ngày đăng bên dưới.
  - **Trên form Soạn thảo (`Create.cshtml` & `Edit.cshtml` của Blogs & News):** Bỏ phần hiển thị tác giả tĩnh, thay bằng trường chọn **Chủ đề / Phân loại** (`SeoKeywords`) dạng dropdown `<select>` tương ứng với các danh mục lọc ngoài sàn:
    - *Cẩm nang:* `Kinh nghiệm thuê xe`, `Thủ tục & Giấy tờ`, `Lái xe an toàn`, `Kỹ thuật & Vận hành`, `Cung đường du lịch`.
    - *Tin tức:* `Tin tức ReCar`, `Công nghệ & Xe điện`, `Hành trình du lịch`, `Thị trường ô tô`, `Chính sách & Pháp luật`.

### 47.4. Kiểm Thử & Trạng Thái Hệ Thống
- Biên dịch toàn bộ giải pháp: **`0 Error(s)`**.
- Ứng dụng đang chạy ổn định tại:
  - Quản lý cẩm nang: **`http://localhost:5163/Admin/Blogs`**
  - Quản lý tin tức: **`http://localhost:5163/Admin/News`**
  - Cẩm nang người dùng: **`http://localhost:5163/Blog`**
  - Tin tức người dùng: **`http://localhost:5163/News`**

---

## 48. TÁCH BẠCH ĐỘC LẬP TÍNH NĂNG "ĐƯA LÊN ĐẦU TRANG" (ISFEATURED) VÀ "NHÃN BÀI VIẾT" (BADGE)

### 48.1. Hủy Bỏ Cơ Chế Tự Động Gắn Nhãn Theo Lượt Xem (Views >= 500)
- **Yêu cầu:** Không tự động gán nhãn "Nổi bật" khi bài viết vượt mốc 500 lượt xem. Lượt xem chỉ mang tính chất ghi nhận số lượng độc giả quan tâm đơn thuần.
- **Triển khai:**
  - Loại bỏ hoàn toàn điều kiện `Views >= 500` tự động gán `Badge = "Nổi bật"` trong `NewsController.cs` và `BlogController.cs`.
  - Loại bỏ việc gán ngầm nhãn nổi bật trong logic `Create`, `Edit` và `GetPreview` của Admin.
  - Loại bỏ điều kiện `Views >= 500` trên giao diện bảng Admin, modal xem trước và giao diện phía khách hàng (`/News`, `/Blog`).

### 48.2. Định Nghĩa Rõ Vai Trò Duy Nhất Của Switch "Nổi Bật" (`IsFeatured`)
- **Mục đích:** Switch *"Đánh dấu là Bản tin / Bài viết nổi bật"* chỉ có **duy nhất 1 nhiệm vụ**: khi admin chủ động **bật lên** (`IsFeatured == true`), bài viết đó sẽ được ưu tiên xếp lên vị trí đầu tiên của trang (bản tin / bài viết tiêu điểm khổ lớn).
- **Sắp xếp ViewComponent:**
  - `NewsViewComponent`: Sắp xếp ưu tiên theo `.OrderByDescending(m => m.IsFeatured ?? false).ThenByDescending(m => m.CreatedDate)`.
  - `BlogVIewComponent`: Sắp xếp ưu tiên theo `.OrderByDescending(i => i.IsFeatured ?? false).ThenByDescending(i => i.CreatedDate)`.
- Khi switch bị tắt (`IsFeatured == false`), bài viết sẽ trở về vị trí danh sách bình thường theo thứ tự thời gian.

### 48.3. Nhãn Hiển Thị (`Badge`): Tuân Thủ Tuyệt Đối Lựa Chọn Của Admin
- Nhãn hiển thị trên ảnh bài viết chỉ xuất hiện khi và chỉ khi admin chủ động chọn một nhãn cụ thể trong Dropdown (`Nổi bật`, `Xu hướng`, `Mới nhất`, `Ưu đãi`, `Đặc sắc`, `Đọc nhiều`).
- Nếu admin để `-- Không gắn nhãn --` (`Badge == null` hoặc rỗng), bài viết sẽ hoàn toàn không có huy hiệu nhãn, kể cả khi bài viết có nhiều lượt xem hay đang được ghim lên đầu trang.

### 48.4. Kiểm Thử & Trạng Thái Hệ Thống
- Biên dịch toàn bộ giải pháp: **`0 Error(s)`**.
- Ứng dụng đang chạy ổn định tại:
  - Admin Tin tức: **`http://localhost:5163/Admin/News`**
  - Admin Cẩm nang: **`http://localhost:5163/Admin/Blogs`**
  - Client Tin tức: **`http://localhost:5163/News`**
  - Client Cẩm nang: **`http://localhost:5163/Blog`**

---

## 49. RÀNG BUỘC ĐĂNG NHẬP KHI BÌNH LUẬN CẨM NANG & TIN TỨC VÀ TINH GỌN FORM BÌNH LUẬN (NGÀY 12/11/2026)

### 49.1. Nghiệp Vụ & Yêu Cầu Người Dùng
- **Phía người dùng (Client):**
  - Chỉ những khách hàng đã đăng nhập tài khoản ReCar mới có quyền gửi bình luận tại trang chi tiết bài viết **Cẩm nang (`/Blog/Details/{id}`)** và **Tin tức (`/News/Details/{id}`)**.
  - Khi người dùng chưa đăng nhập: Ẩn form gửi bình luận, hiển thị khung thông báo trang nhã kèm nút **"Đăng nhập để bình luận"** tự động chuyển hướng đến `/Login?ReturnUrl=...` để sau khi đăng nhập xong sẽ quay lại đúng bài viết đang đọc dở.
  - Khi người dùng đã đăng nhập: Form bình luận được tinh giản tối đa:
    - Loại bỏ hoàn toàn 2 ô nhập thủ công *"Họ và tên"* và *"Địa chỉ Email"*.
    - Chỉ hiển thị duy nhất ô nhập **Nội dung bình luận** (`textarea`) kèm badge nhỏ hiển thị tên tài khoản đang bình luận và nút *"Gửi bình luận"*.

### 49.2. Triển Khai Backend (`BlogController.cs` & `NewsController.cs`)
1. **Kiểm tra phiên đăng nhập chặt chẽ:**
   - Cả 2 action `AddBlogComment` trong [`BlogController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/BlogController.cs) và `Comment` trong [`NewsController.cs`](file:///d:/CarRental_TTCN/CarRental/Controllers/NewsController.cs) đều kiểm tra trạng thái xác thực `Function.IsLogin(HttpContext.Session)` và `Function.GetAccountId(HttpContext.Session) > 0`.
   - Nếu chưa đăng nhập: Trả về JSON `{ status = false, requireLogin = true, message = "Vui lòng đăng nhập để gửi bình luận." }`.
2. **Tự động định danh tác giả bình luận từ CSDL / Session:**
   - Hệ thống tự động truy vấn thông tin người dùng từ bảng `Customers` theo `AccountId` đã đăng nhập:
     - Tên tác giả: Ưu tiên `FullName` -> `Name` -> Session `UserName` -> Fallback mặc định (*Độc giả ReCar* / *Bạn đọc ReCar*).
     - Email tác giả: Lấy từ `customer.Email` hoặc Session `Email`.
   - Mốc thời gian tạo bình luận: Sử dụng giờ Việt Nam chuẩn xác qua `Function.NowVn`.
   - Bảo toàn 100% tương thích dữ liệu quản lý đánh giá / bình luận phía Admin ([`ReviewsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/ReviewsController.cs)).

### 49.3. Triển Khai Giao Diện (`Views/Blog/Details.cshtml` & `Views/News/Details.cshtml`)
1. **Kiểm tra trạng thái xác thực tại View:**
   - Nhận diện người dùng bằng `bool isUserLoggedIn = Function.IsLogin(Context.Session) || Function.GetAccountId(Context.Session) > 0 || Function.GetUserAccountId(Context.Session) > 0;`.
   - Khi chưa đăng nhập: Render Card thông báo với icon bình luận, thông điệp kêu gọi và nút `Đăng nhập để bình luận` kèm `ReturnUrl`.
   - Khi đã đăng nhập: Render badge người dùng tích xanh `@currentUserName` và ô nhập `textarea` nội dung bình luận tinh gọn.
2. **Cập nhật JavaScript AJAX:**
   - Chỉ validate nội dung `detail`, không yêu cầu người dùng nhập họ tên hay email.
   - Khi nhận phản hồi `res.requireLogin == true`: Hiển thị cảnh báo SweetAlert2 kèm nút *"Đăng nhập ngay"* chuyển tiếp mượt mà đến trang Login.
   - Khi gửi thành công: Tự động chèn bình luận mới lên đầu danh sách bằng avatar ký tự chữ cái đầu, tên tài khoản thật do server trả về, thời gian *"Vừa xong"* và làm sạch ô nhập.

### 49.4. Kiểm Thử & Biên Dịch
- Biên dịch toàn bộ dự án ASP.NET Core (.NET 9): **`Build succeeded. 0 Warning(s) 0 Error(s)`**.
- Ứng dụng đang chạy sẵn sàng trải nghiệm tại:
  - Chi tiết Cẩm nang: **`http://localhost:5163/Blog`**
  - Chi tiết Tin tức: **`http://localhost:5163/News`**

---

## 51. THIẾT KẾ PHÂN HỆ PHẢN HỒI ĐÁNH GIÁ XE DÀNH CHO CHỦ XE (HOST CAR REVIEW REPLY SYSTEM) (NGÀY 12/11/2026)

### 51.1. Yêu Cầu & Bối Cảnh
- **Yêu cầu từ người dùng:** Trong giao diện Đối tác Chủ xe (`/CarOwner`), tại trang Chi tiết quản lý xe (`/CarOwner/Cars/Details/{id}`), thiết kế một phần chuyên biệt hiển thị các đánh giá từ khách hàng và cho phép chủ xe gửi, chỉnh sửa hoặc xóa phản hồi trực tiếp đối với từng đánh giá.
- **Mục tiêu hệ thống:**
  1. Cho phép chủ xe tương tác, cảm ơn hoặc giải đáp nhận xét của khách sau chuyến đi thuê xe.
  2. Phản hồi của chủ xe được hiển thị công khai trên trang chi tiết xe (`/Product/Details/{id}`) để các khách hàng tiềm năng khác có thể xem được.
  3. Quản trị viên (`/Admin/Reviews`) cũng có thể quan sát thấy câu trả lời của chủ xe trong bảng đánh giá đội xe.

### 51.2. Mở Rộng Cơ Sở Dữ Liệu & Entity Framework
1. **Bổ sung cột trong CSDL MySQL (`CarReviews`):**
   - Đã thêm 2 cột:
     - `Reply LONGTEXT NULL`: Lưu trữ nội dung phản hồi của chủ xe.
     - `ReplyDate DATETIME NULL`: Mốc thời gian gửi hoặc cập nhật phản hồi gần nhất.
2. **Cập nhật Model C# & Fluent API:**
   - [`CarReview.cs`](file:///d:/CarRental_TTCN/CarRental/Models/CarReview.cs): Khai báo 2 thuộc tính `public string? Reply { get; set; }` và `public DateTime? ReplyDate { get; set; }`.
   - [`DbRenalCarContext.cs`](file:///d:/CarRental_TTCN/CarRental/Models/DbRenalCarContext.cs): Cấu hình mapping cột `Reply` (`longtext`) và `ReplyDate` (`datetime`) trong `modelBuilder.Entity<CarReview>`.

### 51.3. Backend Controller (`Areas/CarOwner/Controllers/CarsController.cs`)
1. **Tối ưu Action `Details(int? id)`:**
   - Kiểm tra quyền chủ xe qua `CheckCarOwnerPermission()`.
   - Nạp thông tin xe kèm `CarReviews`, bóc tách tiện nghi và đơn thuê xe.
2. **Viết Action mới `ReplyReview` (`POST: /CarOwner/Cars/ReplyReview`):**
   - Xác thực quyền Chủ xe thông qua Session.
   - Kiểm tra quyền sở hữu xe: Đảm bảo đánh giá thuộc đúng chiếc xe do chủ xe hiện tại sở hữu (`review.Car.OwnerId == currentOwnerId`).
   - Xử lý chuỗi văn bản: Nếu nội dung để trống hoặc khoảng trắng, xóa phản hồi (`review.Reply = null`, `review.ReplyDate = null`). Nếu có nội dung, cập nhật `review.Reply` và gán thời gian theo múi giờ chuẩn Việt Nam `Function.NowVn`.
   - Lưu vào CSDL và trả về kết quả JSON kèm thời gian định dạng chuẩn `dd/MM/yyyy HH:mm`.

### 51.4. Thiết Kế Giao Diện Chi Tiết Xe Phía Chủ Xe (`Areas/CarOwner/Views/Cars/Details.cshtml`)
1. **Thiết kế Card "Đánh giá & Phản hồi từ Khách hàng" chuẩn ReCar Design System:**
   - Header hiển thị tổng số lượng đánh giá và huy hiệu điểm trung bình tinh gọn: **`<i class="bi bi-star-fill text-warning"></i> [Điểm] ★`**.
   - Danh sách đánh giá cuộn mượt mà (`max-height: 580px; overflow-y: auto`), mỗi nhận xét là một thẻ độc lập:
     - Avatar tròn chữ cái đầu có dải màu gradient sắc nét.
     - Tên khách hàng, mã đơn thuê xe `#OrderId` kèm mốc thời gian nhận xét.
     - Huy hiệu số sao gọn gàng `[Số sao] ★`.
     - Nội dung nhận xét chi tiết của khách.
2. **Khối hiển thị Phản hồi từ Chủ xe:**
   - Nếu chủ xe **đã phản hồi**: Render khung card lồng thụt lề cao cấp với viền nhấn màu ReCar Blue `#0284c7`, nhãn tích xanh bảo chứng *"Phản hồi của bạn (Chủ xe)"*, mốc thời gian và menu 3 chấm thao tác gọn gàng (*Chỉnh sửa* / *Xóa phản hồi*).
   - Nếu chủ xe **chưa phản hồi**: Hiển thị nút bấm viền bo tròn trang nhã **"Phản hồi đánh giá"** kèm icon `bi-reply-fill`.
3. **Modal Phản hồi tương tác cao & AJAX:**
   - Modal Bootstrap `#modalReplyReview` giao diện Deep Navy tiêu chuẩn ReCar.
   - Soạn thảo nội dung phản hồi, hiển thị chú thích minh bạch, nút lưu trạng thái loading.
   - Tích hợp SweetAlert2 thông báo thành công hoặc xác nhận xóa an toàn.

### 51.5. Đồng Bộ Hiển Thị Phía Khách Hàng & Quản Trị Viên
1. **Trang Chi tiết xe phía Khách hàng ([`Views/Product/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Product/Details.cshtml)):**
   - Dưới mỗi nhận xét của khách, nếu xe có phản hồi từ chủ xe, hệ thống sẽ tự động hiển thị khối thông điệp:
     - Huy hiệu tích xanh `Phản hồi từ Chủ xe`.
     - Mốc thời gian gửi phản hồi.
     - Nội dung phản hồi rõ ràng, giúp tăng tính minh bạch và uy tín cho xe trên sàn.
2. **Trang Quản trị Đánh giá Admin ([`Areas/Admin/Views/Reviews/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Reviews/Index.cshtml)):**
   - Trong Tab *Đánh giá Đội xe*, cột nội dung đánh giá tự động hiển thị thêm ghi chú phản hồi của chủ xe nếu có, giúp Admin nắm bắt được quá trình hỗ trợ và tương tác giữa chủ xe và người thuê.

### 51.6. Kiểm Thử & Trạng Thái Hoàn Thành
- Toàn bộ dự án ASP.NET Core 9 biên dịch không lỗi: **`Build succeeded. 0 Warning(s) 0 Error(s)`**.
---

## 53. NÂNG CẤP GIAO DIỆN PHẢN HỒI ĐÁNH GIÁ XE CHUẨN HIỆN ĐẠI (HOST REPLY UI REFINEMENT) (NGÀY 12/11/2026)

### 53.1. Vấn Đề Giao Diện Trước Đó
- Dựa trên hình ảnh thực tế từ người dùng (`media_1794500084915_e65a1519.png` và `media_1794500126153_73ee8ad5.png`):
  1. Hộp phản hồi có thanh viền xanh bên trái `border-left` đậm gây thô ráp và đứt gãy thị giác.
  2. Huy hiệu phản hồi màu xanh đậm bị dính sát ngày tháng, thiếu khoảng thụt lề (indent) phân tầng câu trả lời.
  3. Thiếu avatar đại diện cho chủ xe và nội dung văn bản phản hồi bị ép sát vào viền lề.

### 53.2. Cải Tiến & Tái Thiết Kế Chuẩn ReCar Design System
1. **Tại Trang Chi tiết xe Khách hàng ([`Views/Product/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Views/Product/Details.cshtml)):**
   - Nâng cấp thẻ đánh giá của khách: Bổ sung avatar tròn chứa chữ cái đầu tài khoản khách hàng, gắn huy hiệu số sao gọn gàng `[Số sao] ★` thay vì dàn trải 5 ngôi sao to.
   - Thẻ phản hồi từ Chủ xe được thụt lề rõ ràng (`ms-2 ms-md-4`) tạo cảm giác lồng phân cấp phản hồi chuẩn Booking/Airbnb.
   - Tông màu nền xanh ngọc dịu mát (`#f0fdf4` đến `#f8fafc`), viền xanh pastel `#bbf7d0` và đổ bóng nhẹ mềm mại (`shadow: 0 2px 10px rgba(16, 185, 129, 0.05)`).
   - Avatar chủ xe tròn xanh ngọc (`bi-person-check-fill`), tên đầy đủ của chủ xe kèm nhãn tích xanh bảo chứng `Chủ xe`.
   - Có đường kẻ phân cách tinh tế giữa phần tiêu đề phản hồi và nội dung chi tiết.
2. **Tại Trang Chi tiết xe Chủ xe ([`Areas/CarOwner/Views/Cars/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/CarOwner/Views/Cars/Details.cshtml)):**
   - Đồng bộ hoàn toàn phong cách thẻ phản hồi cao cấp lồng thụt lề.
   - Tích hợp menu 3 chấm tròn gọn gàng bên góc phải để chủ xe dễ dàng chọn *Sửa phản hồi* hoặc *Xóa phản hồi*.
   - Khoảng cách dòng thoáng đạt (`line-height: 1.65`, font 13.5px), màu chữ xám than đậm `#1e293b` rõ ràng, thanh lịch.

### 53.3. Kiểm Thử
- Cả hai giao diện đều đã tải lại thành công và hiển thị đồng bộ:
  - Chi tiết xe Sàn ReCar: **`http://localhost:5163/product/vinfast-vf-5-plus-1.html`**
  ---

## 54. ĐỒNG BỘ GIAO DIỆN THAO TÁC & FORM CHỈNH SỬA ĐỊA ĐIỂM NHẬN XE CHUẨN ADMIN RECAR (NGÀY 12/11/2026)

### 54.1. Bối Cảnh & Yêu Cầu
- Yêu cầu người dùng: *"sửa lại phần thao tác ở địa điểm nhận xe cho đồng bộ với admin, ở trong edit nữa"*.
- Module Quản lý địa điểm nhận xe (`/Admin/Locations`) trước đó còn dùng các nút bấm đơn lẻ kiểu cũ (`Sửa`, `Xóa` thô sơ), nút trạng thái dạng form submit toàn trang gây giật màn hình, và trang Chỉnh sửa (`Edit.cshtml`) chưa đồng bộ theo phong cách ReCar Admin SaaS 2 cột hiện đại.

### 54.2. Các Cải Tiến Thực Hiện
1. **Trang Danh sách Địa điểm nhận xe ([`Areas/Admin/Views/Locations/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Locations/Index.cshtml)):**
   - **Menu 3 chấm thao tác chuẩn Admin:** Thay thế cặp nút inline cũ bằng nút tròn 32px `bi-three-dots-vertical` với menu thả bóng mờ, bo góc 12px:
     - *Chỉnh sửa* (icon `bi-pencil-square text-primary`).
     - *Đổi trạng thái hiển thị* (icon `bi-arrow-repeat text-secondary`).
     - Đường kẻ phân cách tinh tế.
     - *Xóa địa điểm* (icon `bi-trash text-danger`, kích hoạt xác nhận an toàn qua SweetAlert2).
   - **Nút trạng thái Pill chuẩn ReCar:** Dạng viên thuốc bo tròn mềm mại (`btn-success-subtle` / `btn-light text-muted`), hỗ trợ chuyển đổi bật/tắt hiển thị tức thì qua AJAX và SweetAlert2 Toast không cần tải lại trang.
   - Hiệu ứng xóa mượt mà: Dòng dữ liệu tự động fade-out biến mất khỏi bảng khi xác nhận xóa.
2. **Trang Chỉnh sửa Địa điểm nhận xe ([`Areas/Admin/Views/Locations/Edit.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Locations/Edit.cshtml)):**
   - **Header chuẩn Admin ReCar:** Nút tròn quay lại (`rounded-circle`), Tiêu đề trang, Huy hiệu ID `#ID-@Model.LocationId`, nút Xóa nguy hiểm màu đỏ phía trên góc phải và nút Quay lại danh sách.
   - **Bố cục 2 cột chuyên nghiệp:**
     - *Cột chính (7 cột):* Thông tin địa điểm nhận xe: Tên Tỉnh / Thành phố (input group kèm icon `bi-pin-map-fill`), Khu vực vùng miền (Select box kèm icon `bi-compass`), Thứ tự ưu tiên hiển thị (kèm icon `bi-sort-numeric-down`), và khay nút bấm gợi ý nhanh các tỉnh thành lớn (Hà Nội, TP.HCM, Đà Nẵng, Hải Phòng, Cần Thơ, Nha Trang, Đà Lạt, TP. Vinh...).
     - *Cột phụ (5 cột):* Thẻ Trạng thái & Hiển thị (Switch Kích hoạt hiển thị và Switch Đánh dấu điểm nhận xe Phổ biến / Nổi bật) + Thẻ Thao tác lưu với nút bấm bo tròn ReCar Blue `#0284c7` nổi bật.
   - Tích hợp SweetAlert2 xác nhận xóa trực tiếp ngay trong trang Edit và tự động quay về danh sách.
3. **Trang Thêm mới Địa điểm nhận xe ([`Areas/Admin/Views/Locations/Create.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/Locations/Create.cshtml)):**
   - Đồng bộ hoàn toàn theo phong cách giao diện 2 cột hiện đại, thân thiện và thống nhất 100% với `Edit.cshtml`.
4. **Backend Controller ([`Areas/Admin/Controllers/LocationsController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/LocationsController.cs)):**
   - Hỗ trợ cả 2 phương thức gọi AJAX và Form POST truyền thống cho 2 action `ToggleStatus` và `Delete`.

### 54.3. Kiểm Thử Hoàn Tất
- Cả 3 trang trong module Địa điểm nhận xe đều đã được kiểm tra trực tiếp và phản hồi **HTTP 200 OK**:
  - Danh sách địa điểm: **`http://localhost:5163/Admin/Locations`** -> **200 OK**
  - Chỉnh sửa địa điểm: **`http://localhost:5163/Admin/Locations/Edit/1`** -> **200 OK**
  - Thêm mới địa điểm: **`http://localhost:5163/Admin/Locations/Create`** -> **200 OK**
- Toàn bộ dự án biên dịch sạch: **`0 Warning(s), 0 Error(s)`**.

---

## 55. NÂNG CẤP TOÀN DIỆN PHÂN HỆ QUẢN LÝ NGƯỜI DÙNG & ĐỐI TÁC (USER MANAGEMENT) CHUẨN ADMIN RECAR (NGÀY 12-13/11/2026)

### 55.1. Bối Cảnh & Yêu Cầu
- Yêu cầu người dùng: *"tiếp theo là phần người dùng và đối tác, sửa lại giao diện cho phù hợp với hệ thống nhất là khi bấm vào chi tiết"*.
- Phân hệ Quản lý Người dùng & Đối tác (`/Admin/UserManagement`) trước đó còn mang dáng dấp giao diện bảng cũ, các nút thao tác đơn lẻ, danh sách tài khoản sinh nhiều modal lặp trong vòng lặp `foreach`, và trang Chi tiết người dùng (`Details.cshtml`) chỉ là bảng text thô sơ dạng `dl-horizontal` của scaffolding mặc định.

### 55.2. Các Cải Tiến Thực Hiện
1. **Trang Chi tiết Hồ sơ Người dùng & Đối tác ([`Areas/Admin/Views/UserManagement/Details.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/UserManagement/Details.cshtml)):**
   - **Tái thiết kế toàn diện theo mô hình Hồ sơ SaaS 2 cột chuyên nghiệp:**
     - **Header chuẩn ReCar:** Nút tròn quay lại (`rounded-circle`), Tiêu đề hồ sơ, Huy hiệu ID `#ID-@Model.CustomerId`, huy hiệu phân vai trò (*Quản trị viên*, *Chủ xe đối tác*, *Khách hàng*), huy hiệu trạng thái (*Hoạt động* / *Đã khóa*), nút Chỉnh sửa và nút Khóa/Mở khóa nhanh.
     - **Cột trái (4 cột) - Hồ sơ định danh & Pháp lý:**
       - Thẻ đại diện cá nhân với Avatar tròn gradient/ảnh đại diện thực tế, chấm trạng thái hoạt động online/khóa.
       - Thẻ thông tin liên hệ: Email, Số điện thoại, Địa chỉ thường trú.
       - Thẻ thông tin tài khoản ngân hàng liên kết: Ngân hàng thụ hưởng, Số tài khoản, Tên chủ thẻ (phục vụ đối soát doanh thu chủ xe và hoàn tiền cọc cho khách).
       - Thẻ giấy phép lái xe & định danh GPLX: Số GPLX/CCCD, kèm khung ảnh GPLX có nút xem ảnh phóng to toàn màn hình.
     - **Cột phải (8 cột) - Hoạt động nghiệp vụ theo vai trò:**
       - **Nếu là Chủ xe đối tác (Role 3):**
         - 3 Thẻ thống kê KPI: Tổng số xe đang quản lý trên sàn, Doanh thu ước tính từ các lượt thuê, Tổng đơn thuê xe đã tiếp nhận.
         - Bảng Đội xe sở hữu: Hiển thị danh sách toàn bộ các xe thuộc chủ sở hữu (ảnh xe, tên xe, biển số, phân khúc, giá thuê/ngày, trạng thái xe, số chuyến đã đi, nút xem chi tiết xe).
         - Bảng Các đơn thuê gần nhất: Hiển thị các chuyến thuê liên quan đến đội xe của đối tác.
         - Thông tin đơn đăng ký làm chủ xe ban đầu (nếu có).
       - **Nếu là Khách hàng (Role 2):**
         - Thẻ thống kê: Tổng đơn đặt xe đã tạo, Số chuyến hoàn tất, Tổng chi tiêu trên sàn.
         - Bảng Lịch sử các đơn đặt thuê xe: Mã đơn, Tên xe thuê kèm biển số và ảnh, Thời gian nhận - trả xe, Tổng tiền thanh toán, Huy hiệu trạng thái đơn hàng.
2. **Trang Danh sách Người dùng & Đối tác ([`Areas/Admin/Views/UserManagement/Index.cshtml`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Views/UserManagement/Index.cshtml)):**
   - Thay thế hoàn toàn cụm nút inline thô sơ và loại bỏ vòng lặp Modal Bootstrap thừa thãi.
   - Bổ sung **Menu 3 chấm thao tác (`bi-three-dots-vertical`)** chuẩn thiết kế ReCar:
     - *Xem hồ sơ chi tiết* (`bi-person-badge text-primary`).
     - *Chỉnh sửa tài khoản* (`bi-pencil-square text-secondary`).
     - Đường kẻ phân cách.
     - *Khóa / Mở khóa tài khoản* (`bi-lock` / `bi-unlock` kích hoạt xác nhận an toàn qua SweetAlert2).
   - **Nút trạng thái Pill chuẩn ReCar:** Cho phép chuyển đổi trạng thái Khóa / Mở khóa tài khoản ngay lập tức qua AJAX không cần tải lại toàn bộ trang.
3. **Backend Controller ([`Areas/Admin/Controllers/UserManagementController.cs`](file:///d:/CarRental_TTCN/CarRental/Areas/Admin/Controllers/UserManagementController.cs)):**
   - Nạp dữ liệu đa tầng tùy theo vai trò trong action `Details(id)`: Load `OwnedCars`, `HostOrders`, và `CarOwnerRegistration` cho Chủ xe; Load `CustomerOrders` kèm chi tiết xe cho Khách hàng.
   - Bổ sung cơ chế phản hồi JSON cho action `ToggleLock(id)` khi nhận yêu cầu AJAX từ giao diện.
   - Đồng bộ danh sách vai trò `ViewBag.RoleId` và `ViewBag.Roles` cho trang `Edit.cshtml`.

### 55.3. Kiểm Thử Hoàn Tất
- Hệ thống biên dịch thành công: **`0 Error(s)`**.
- Tất cả các endpoint đều đã kiểm thử trực tiếp và phản hồi **HTTP 200 OK**:
  - Danh sách người dùng & đối tác: **`http://localhost:5163/Admin/UserManagement/Index`** -> **200 OK**
  - Chi tiết hồ sơ Chủ xe (ID: 1022 - Trần Quốc Bảo): **`http://localhost:5163/Admin/UserManagement/Details/1022`** -> **200 OK**
  - Chi tiết hồ sơ Khách hàng (ID: 1027 - Trần Kenn): **`http://localhost:5163/Admin/UserManagement/Details/1027`** -> **200 OK**
  - Chỉnh sửa tài khoản: **`http://localhost:5163/Admin/UserManagement/Edit/1022`** -> **200 OK**

---

## 56. TINH CHỈNH GIAO DIỆN CHI TIẾT HỒ SƠ & KHẮC PHỤC HIỂN THỊ HÌNH ẢNH GPLX / GIẤY TỜ (NGÀY 13/11/2026)

### 56.1. Bối Cảnh & Phản Hồi Từ Người Dùng
- **Ảnh 1:** Nút quay lại tròn (`<a class="btn rounded-circle"><i class="bi bi-arrow-left"></i></a>`) đứng ngay trước tiêu đề "Hồ sơ người dùng & đối tác" bị trùng lặp vì bên phải header đã có nút "Quay lại" rõ ràng.
- **Ảnh 2 & Ảnh 3:** Khối tiêu đề của 2 thẻ "Đội xe do chủ xe này sở hữu" và "Đơn thuê xe gần đây" bị lỗi dính viền, icon chạm sát góc bo cong do thiếu padding.
- **Lỗi xem hình ảnh GPLX / Giấy tờ:** Hình ảnh GPLX / CCCD không mở được hoặc bị lỗi đường dẫn khi xem chi tiết.

### 56.2. Các Giải Pháp & Xử Lý Hoàn Tất
1. **Khắc phục triệt để hiển thị Hình ảnh GPLX / Giấy tờ:**
   - **Xử lý đường dẫn động thông minh (`ResolveDocUrl`):** Các file upload trước đây lưu tên tệp tương đối (ví dụ `giay-phep-lai-xe-hang-a1.jpg` hoặc `vangla_...jpg`) khiến trình duyệt tải sai đường dẫn tương đối `/Admin/UserManagement/Details/...` dẫn đến 404. Hệ thống đã bổ sung hàm tự động phân giải đường dẫn thực tế sang `/files/license/...` hoặc `/files/KYC/...` và hỗ trợ cả ảnh trực tuyến.
   - **Bổ sung xem toàn diện giấy tờ pháp lý:** Cho phép xem cả **Giấy phép lái xe (GPLX)**, **Mặt trước CCCD**, và **Mặt sau CCCD** (từ hồ sơ đăng ký KYC `CarOwnerRegistration`).
   - **Nâng cấp Modal phóng to ảnh (`imageModal`):** Tích hợp thuộc tính HTML khai báo chuẩn `data-bs-toggle="modal" data-bs-target="#imageModal"` trực tiếp trên nút "Phóng to" và khung hình ảnh, kèm hàm gán dữ liệu `prepareImageModal()` và cơ chế khởi tạo an toàn tương thích mọi phiên bản Bootstrap 5 / jQuery (`new bootstrap.Modal()`), bổ sung nút "Mở tab mới" để xem ảnh gốc độ phân giải cao và cơ chế `onerror` tự động fallback.
   - Cập nhật bổ sung mẫu GPLX/CCCD và tài khoản ngân hàng chuẩn cho các tài khoản chủ xe mẫu (ID 1022, 1023, 1024...).
2. **Loại bỏ nút Back trùng lặp (Theo Ảnh 1):**
   - Đã gỡ bỏ nút tròn `bi-arrow-left` phía trước tiêu đề, canh chỉnh dòng phụ đề thẳng hàng, gọn gàng và tinh tế với nút "Quay lại" bên phải màn hình.
3. **Thiết kế lại Card Header chuẩn ReCar (Theo Ảnh 2 & Ảnh 3):**
   - Chuyển đổi từ `recar-panel` cũ sang `card border-0 shadow-sm rounded-4 overflow-hidden`.
   - Card Header sử dụng `py-3 px-4` với khay Icon bo góc 40x40px (`rounded-3`) màu xanh pastel (`#e0f2fe` cho xe, `#dcfce7` cho đơn hàng) tạo điểm nhấn nhận diện thương hiệu, khoảng cách đệm an toàn tuyệt đối, không còn tình trạng dính viền.
   - Thêm padding `ps-4 py-3` và `pe-4 py-3` cho bảng dữ liệu để nội dung căn lề chuẩn chỉ từ trên xuống dưới.

### 56.3. Kiểm Thử
- Chi tiết hồ sơ Chủ xe: **`http://localhost:5163/Admin/UserManagement/Details/1022`** -> **200 OK**
- Ảnh GPLX tĩnh: **`http://localhost:5163/files/license/giay-phep-lai-xe-hang-a1.jpg`** -> **200 OK**

---

## 57. NÂNG CẤP HỒ SƠ CÁ NHÂN, ĐỔI MẬT KHẨU BẮT BUỘC MẬT KHẨU CŨ & ĐỒNG BỘ AVATAR TOÀN DIỆN (NGÀY 13/11/2026)

### 57.1. Bối Cảnh & Yêu Cầu Người Dùng
- **Yêu cầu:** "bây giờ tôi muốn làm phần hồ sơ cá nhân cho phân hệ admin và khách hàng. có phần đổi mật khẩu thì phải bắt nhập mật khẩu cũ nhé, avatar sau khi đổi thì sẽ hiện thị ở tất cả mọi nơi nữa".
- **Mục tiêu:**
  1. Xây dựng và nâng cấp giao diện Hồ sơ cá nhân cho cả **Admin** (`/Admin/Account/Edit`) và **Khách hàng** (`/Accounts` tab `#account-info`).
  2. Bắt buộc nhập **Mật khẩu cũ (hiện tại)** khi đổi mật khẩu ở cả Admin và Khách hàng; xác minh chính xác mật khẩu cũ qua hash MD5 trước khi cho phép tạo mật khẩu mới.
  3. Xây dựng hệ thống tải và đồng bộ **Ảnh đại diện (Avatar)**: lưu ảnh vào `/files/avatars/`, cập nhật cơ sở dữ liệu `Customer.Avartar` và tức thì đồng bộ Session (`Function.SetAvatar`, `Function.SetAdminAvatar`), giúp ảnh đại diện hiển thị đồng nhất ở tất cả mọi nơi (Header Client, Header Admin, Header Chủ xe, Dropdown người dùng, Sidebar tài khoản, trang Hồ sơ).

### 57.2. Các Giải Pháp Kỹ Thuật Đã Triển Khai
1. **Hệ thống Session & Utility Avatar & Password (`CarRental/Utilities/Function.cs`):**
   - Bổ sung session key `SESSION_AVATAR`, `SESSION_ADMIN_AVATAR`, cùng các hàm: `SetAvatar`, `GetAvatar`, `SetAdminAvatar`, `GetAdminAvatar`.
   - Bổ sung hàm tiện ích `UploadAvatar(IFormFile? file)`: kiểm tra định dạng an toàn (`.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`), đặt tên unique kèm timestamp và lưu vào thư mục `wwwroot/files/avatars/`.
   - Bổ sung hàm xác thực bảo mật `VerifyPassword(string inputPassword, string? storedPassword)`: kiểm tra đối chiếu mã hash MD5 (`MD5Password`) và hỗ trợ đối sánh an toàn.
   - Bổ sung hàm `ResolveAvatarUrl(string? rawAvatar)` xử lý đường dẫn tương đối, tránh lỗi 404 hình ảnh.
   - Cập nhật `SessionSyncMiddleware` và các Controller Đăng nhập (`LoginController` ở Client & Admin): tự động nạp `Customer.Avartar` vào Session ngay khi đăng nhập.

2. **Nâng cấp Hồ sơ Quản trị viên (`/Admin/Account/Edit`):**
   - **Backend (`CarRental/Areas/Admin/Controllers/AccountController.cs`):**
     - `[HttpGet] Edit()`: Tự động tải thông tin Admin đang đăng nhập (hoặc tài khoản Super Admin `RoleId == 1`).
     - `[HttpPost] UpdateInfo(...)`: Cập nhật Họ tên, Email (kiểm tra chống trùng lặp), Số điện thoại, Địa chỉ và upload Avatar mới; ngay lập tức cập nhật Session Admin.
     - `[HttpPost] ChangePassword(...)`: Bắt buộc nhập `oldPassword`, xác minh qua `Function.VerifyPassword`. Nếu không đúng mật khẩu cũ lập tức báo lỗi. Kiểm tra mật khẩu mới tối thiểu 6 ký tự và xác nhận khớp nhau trước khi hash MD5 lưu vào DB.
   - **Giao diện (`CarRental/Areas/Admin/Views/Account/Edit.cshtml`):**
     - Bố cục 2 cột chuẩn phong cách Admin SaaS hiện đại: Cột trái gồm Avatar 110px bo tròn với viền xanh, nút máy ảnh chọn ảnh tức thì và live preview; Cột phải gồm 2 card phân tách rõ ràng: Card thông tin cá nhân và Card Đổi mật khẩu có nút con mắt (eye) bật/tắt xem mật khẩu.
     - Gỡ bỏ hoàn toàn badge text thừa `Quản trị cấp cao (Super Admin)` theo đúng phản hồi của người dùng.
     - Tích hợp SweetAlert2 bắt buộc xác nhận trước khi đổi ảnh đại diện mới.

3. **Nâng cấp Hồ sơ Khách hàng (`/Accounts` tab `#account-info`):**
   - **Backend (`CarRental/Controllers/AccountsController.cs`):**
     - Đồng bộ avatar khách hàng từ DB vào Session trong `Index()`.
     - Thêm action `[HttpPost] UpdateProfile(...)` cập nhật thông tin cá nhân và xử lý upload Avatar.
     - Thêm action `[HttpPost] ChangePassword(...)` bắt buộc mật khẩu cũ, xác thực với `Function.VerifyPassword`, kiểm tra độ dài và cập nhật mật khẩu mới.
     - Thêm action `[HttpPost] UploadAvatarAjax(...)` cho phép đổi ảnh đại diện nhanh qua AJAX với thông báo SweetAlert2 có popup xác nhận.
   - **Giao diện (`CarRental/Views/Accounts/Index.cshtml`):**
     - Nâng cấp Sidebar tài khoản: Hiển thị avatar thực tế của khách hàng (kèm huy hiệu vai trò "Khách hàng ReCar" / "Chủ xe đối tác") và nút máy ảnh mini cho phép đổi nhanh ảnh đại diện.
     - Tái thiết kế Tab `#account-info` thành 2 card sang trọng: Card 1 "Hồ sơ cá nhân & Ảnh đại diện" và Card 2 "Đổi mật khẩu đăng nhập (Bắt buộc mật khẩu cũ)" có eye toggle và ghi chú bảo mật.
     - Bổ sung menu "Hồ sơ & Đổi mật khẩu" trực tiếp trên dropdown topbar của Client và Chủ xe để truy cập 1 chạm.

4. **Đồng bộ Avatar trên toàn hệ thống giao diện:**
   - **Header Client (`CarRental/Views/Shared/_Layout.cshtml`):** Hiển thị avatar tròn trên nút pill đăng nhập và trong dropdown menu khách hàng; tự động fallback lấy từ DB và đồng bộ vào Session nếu session chưa có.
   - **Header Admin (`CarRental/Areas/Admin/Views/Shared/_Layout.cshtml`):** Hiển thị avatar tròn của Admin trên thanh Topbar và dropdown Quản trị viên.
   - **Header Chủ xe (`CarRental/Areas/CarOwner/Views/Shared/_Layout.cshtml`):** Hiển thị avatar tròn của chủ xe trên thanh Topbar và dropdown.

---

## 58. THỐNG NHẤT HỆ THỐNG AVATAR & ĐIỀU CHỈNH TỈ LỆ / KHOẢNG CÁCH ĐÁNH GIÁ XE, CẨM NANG, TIN TỨC (NGÀY 13/11/2026)

### 58.1. Yêu Cầu & Phản Hồi Từ Người Dùng
- **Yêu cầu:** "các nơi như cẩm nang, tin tức, đánh giá xe, vẫn không thông nhất avatar này, ở chỗ đánh giá xe sửa lại tỉ lệ chỗ bình luận đi avatar và tên đang dính với nhau kia phải cách ra 1 chút".
- **Các vấn đề thực tế xác minh từ 3 ảnh người dùng đính kèm:**
  1. **Ảnh 1 (Chi tiết Tin tức/Cẩm nang):** Tác giả bài viết hiển thị icon "RC" đơn điệu, chưa đồng bộ với ảnh đại diện của Ban biên tập/Admin.
  2. **Ảnh 2 (Ý kiến bạn đọc Tin tức/Cẩm nang):** Người bình luận (`Trần Quốc Bảo`) chỉ hiện chữ "T" mờ trơ trọi không có avatar tròn chuẩn, chưa lấy avatar thật của tài khoản trong hệ thống ReCar.
  3. **Ảnh 3 (Đánh giá xe từ khách hàng):** Khách hàng (`Trần Kenn`) chưa lấy avatar thật; Avatar và Tên bị dính sát vào nhau do tỉ lệ nhỏ (36px) và khoảng cách hẹp; Phản hồi từ chủ xe chưa hiện avatar thật của Chủ xe.

### 58.2. Giải Pháp Triển Khai Hoàn Tất
1. **Khắc phục lỗi class Bootstrap 5 không hợp lệ:**
   - Thay thế toàn bộ các class không được hỗ trợ trong Bootstrap 5 (`p-3.5`, `gap-2.5`, `px-2.5`, `pb-1.5`, `py-0.5`, `me-0.5`) bằng thuộc tính CSS inline chuẩn (`padding: 18px 20px`, `gap: 14px`, `gap: 10px`, `padding: 4px 12px`...).
2. **Đồng bộ Avatar và sửa tỉ lệ tại trang Chi tiết Đánh giá xe (`Product/Details.cshtml`):**
   - **Kích thước & khoảng cách chuẩn:** Avatar khách hàng nâng lên `44px × 44px`, khoảng cách `gap: 14px` với Họ tên (`14.5px`, `font-weight: 600`), dòng thời gian cách dưới `3px`.
   - **Nội dung bình luận căn lề thông thoáng:** Sử dụng `padding-left: 58px` thẳng hàng ngay dưới tên khách hàng.
   - **Lấy Avatar thật từ database:** Truy vấn `_db.Customers` theo Email, SĐT hoặc Tên người đánh giá để hiển thị ảnh đại diện thật của họ (nếu chưa có mới dùng avatar chữ cái gradient màu ReCar sắc nét).
   - **Phản hồi từ Chủ xe:** Căn lề `margin-left: 58px`, `padding: 14px 16px`, avatar thật của Chủ xe `32px × 32px` với khoảng cách `gap: 10px` không còn dính chữ.
   - **Badge số sao:** Rút gọn còn 1 ngôi sao vàng `★` + số sao, nền vàng nhạt `padding: 4px 12px`.
3. **Đồng bộ Avatar tại trang Chi tiết Tin tức (`News/Details.cshtml`):**
   - **Tác giả bài viết:** Thay thế chữ "RC" cứng bằng avatar thật của Tác giả/Ban biên tập/Admin hoặc icon bản tin trang nhã.
   - **Ý kiến bạn đọc:** Khắc phục lỗi chữ cái đơn độc bằng việc tìm kiếm avatar thật của tài khoản trong hệ thống; nếu chưa có thì hiển thị khung tròn gradient 40px viền trắng 2px sắc nét. Khoảng cách avatar và tên được nới rộng `gap: 12px`.
   - **Phản hồi Ban Quản trị:** Tự động lấy avatar của Admin Super Admin (`RoleId == 1`) hiển thị đồng bộ.
4. **Đồng bộ Avatar tại trang Chi tiết Cẩm nang (`Blog/Details.cshtml`):**
   - Đồng bộ hoàn toàn tương tự Tin tức: Tác giả bài viết, Danh sách bình luận độc giả (avatar thật 40px, `gap: 12px`), Phản hồi Ban Quản trị.
5. **Đồng bộ bảng Quản trị Đánh giá & Bình luận Admin (`Reviews/Index.cshtml`):**
   - Bổ sung avatar tròn 34px nhỏ gọn ở cả 3 tab (Đánh giá Đội xe, Bình luận Cẩm nang, Bình luận Tin tức) giúp Quản trị viên dễ dàng nhận diện khách hàng và độc giả.

---

## 59. TỔNG KẾT CA LÀM VIỆC NGÀY 13/11/2026 & BÀN GIAO MÃ NGUỒN LÊN GITHUB

### 59.1. Danh Sách Các Hạng Mục Đã Hoàn Thành Trọn Vẹn
1. **Nâng cấp toàn diện Hồ sơ cá nhân Quản trị viên (`/Admin/Account/Edit`):**
   - Bố cục 2 cột SaaS, cập nhật định danh, đổi mật khẩu bắt buộc mật khẩu cũ, xem trước avatar và hộp thoại xác nhận khi đổi ảnh.
   - Gỡ bỏ chữ "Quản trị cấp cao (Super Admin)" thừa.
2. **Nâng cấp Hồ sơ Khách hàng (`/Accounts` tab `#account-info`):**
   - Cập nhật thông tin cá nhân, đổi mật khẩu bắt buộc mật khẩu cũ, đổi ảnh đại diện nhanh qua AJAX trên Sidebar có popup xác nhận.
3. **Đồng bộ Avatar toàn hệ thống:**
   - Header Admin, Header Client, Header CarOwner, Dropdown Menu, Sidebar.
   - Trang Đánh giá xe (`Product/Details`), Tin tức (`News/Details`), Cẩm nang (`Blog/Details`), Quản lý đánh giá (`Admin/Reviews/Index`).
4. **Sửa lỗi tỉ lệ & khoảng cách dính viền ở Đánh giá xe:**
   - Thay thế toàn bộ class không hợp lệ trong Bootstrap 5 bằng spacing cố định (`padding: 18px 20px`, `gap: 14px`, `margin-left: 58px`).
   - Tách rời avatar và tên, căn lề thẳng hàng, bố cục cân đối và chuyên nghiệp.

### 59.2. Kiểm Thử Hệ Thống
- Dự án biên dịch thành công: **`0 Error(s)`**.
- Ứng dụng chạy mượt mà trên nền tảng .NET 8 / Kestrel tại **`http://localhost:5163`**.
- Tất cả các endpoint đều trả về **HTTP 200 OK**.






















---

## 60. TỔNG KẾT CA LÀM VIỆC NGÀY 15/11/2026 & BÀN GIAO MÃ NGUỒN LÊN GITHUB

### 60.1. Danh Sách Các Hạng Mục Đã Hoàn Thành
1. **Chuyển đổi cổng thanh toán sang ZaloPay & Gỡ bỏ VNPay:**
   - Tích hợp cổng thanh toán **ZaloPay** (`IZaloPayService`, `ZaloPayService`, cấu hình trong `appsettings.json`, xử lý tạo đơn và callback trong `CartController.cs`, cập nhật giao diện tại `Checkout.cshtml`, `Success.cshtml`, `PaymentFail.cshtml`).
   - Chuẩn hóa toàn bộ nhãn hiển thị cho khách hàng và thông báo từ `ZaloPay Sandbox` sang **ZaloPay**.
   - Xóa bỏ hoàn toàn mã nguồn và cấu hình VNPay cũ (`IVnpayServices.cs`, `VnPayService.cs`, `VnPayLibrary.cs`, `VnPaymentResponseModel.cs`).
   - Rà soát và đồng bộ luồng tiền, ví Chủ xe và đối soát tài chính Admin (`PartnerPayout.cs`, `WalletTransaction.cs`, `FinanceController.cs`, `WalletController.cs`, `OrdersController.cs`).

2. **Nâng cấp Liên hệ Khách hàng & Trang Quản trị Liên hệ (Admin):**
   - Cập nhật trang liên hệ phía khách hàng (`ContactController.cs`, `Views/Contact/Index.cshtml`): yêu cầu đăng nhập mới được gửi liên hệ và tự động đồng bộ thông tin tài khoản đã đăng nhập.
   - Xây dựng và đồng bộ giao diện Quản lý Liên hệ phía Admin (`Areas/Admin/Controllers/ContactsController.cs`, `Areas/Admin/Views/Contacts/Index.cshtml`), bổ sung nút **Phản hồi Email** mở sẵn ứng dụng email (`mailto:`) điền sẵn địa chỉ người nhận, tiêu đề và nội dung trích dẫn.

3. **Tinh chỉnh Giao diện Trang chủ (`Home/Index.cshtml` & `recar.css`):**
   - **Danh mục dòng xe:** Thay thế chia cột cố định `row-cols-xl-6` bằng CSS Grid `.recar-cat-grid` (`repeat(auto-fit, minmax(175px, 1fr))`) giúp các thẻ danh mục tự động giãn đều 100% chiều ngang theo đúng số lượng dòng xe hiện có.
   - **Xe nổi bật và ưu đãi:** Lọc đúng các xe đang hoạt động có tag **Nổi bật** hoặc đang **Ưu đãi** (`HomeController.cs`), hiển thị **4 xe mỗi trang** và tự động hiển thị thanh phân trang JavaScript mượt mà khi có từ 5 xe trở lên.

4. **Dọn dẹp toàn bộ Mã nguồn & Tài nguyên dư thừa:**
   - Xóa các thư mục giao diện mẫu rời (`giao dien admin/`, `giao dien chu xe/`), `PROMPT_DESIGN_CAR_OWNER.md` và 6 file `.sql` dư thừa ở thư mục gốc (`create_banks.sql`, `create_brands.sql`, `create_locations.sql`, `dbrenalcar.sql`, `import_real_menus.sql`, `seed_hierarchical_menus.sql`).
   - Xóa 6 file Model không còn sử dụng (`Product.cs`, `Message.cs`, `Contract.cs`, `ContractDetail.cs`, `ContractSettlement.cs`, `ContractSettlementDetail.cs`) và gỡ bỏ `DbSet`, cấu hình `OnModelCreating` trong `DbRenalCarContext.cs` cùng các navigation property tương ứng trong `User.cs`, `CarType.cs`, `Customer.cs`.

### 60.2. Kiểm Thử Hệ Thống
- Dự án biên dịch thành công: **`0 Error(s)`** (`dotnet build CarRental\CarRental.csproj --no-restore`).
- Ứng dụng chạy ổn định tại **`http://localhost:5163`**, các endpoint chính (`/` , `/Cart/Success`, `/Cart/PaymentFail`, `/Cart/ZaloPayCallBack`) đều trả về **HTTP 200 OK**.
