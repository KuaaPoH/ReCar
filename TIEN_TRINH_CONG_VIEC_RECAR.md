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

### Giai đoạn 2: Nâng cấp Công nghệ Giao tiếp Thời gian thực (Real-time Communication với SignalR)
1. **Tích hợp ASP.NET Core SignalR cho Hệ thống Chat:**
   - Nâng cấp Chat giữa Khách hàng và Chủ xe từ AJAX Polling hiện tại lên WebSockets thời gian thực (Real-time Two-Way Chat). Tin nhắn hiển thị tức thì không có độ trễ.
   - Bổ sung hiệu ứng âm thanh thông báo (Sound Ping) và chỉ báo "đối phương đang soạn tin..." (`typing indicator`).
2. **Hệ thống Thông báo Đẩy Thời gian thực (Live Push Notifications):**
   - Khi có khách đặt xe mới: Chủ xe và Admin nhận được chuông thông báo và popup đẩy ngay trên góc màn hình mà không cần F5.
   - Khi chủ xe thêm xe mới: Badge số xe chờ duyệt trên thanh Sidebar Admin tự động nhảy số thời gian thực.
   - Khi Admin duyệt xe hoặc Chủ xe giao xe: Khách hàng nhận được thông báo trạng thái cập nhật tức thời.

---

### Giai đoạn 3: Tích hợp Cổng thanh toán Mở rộng & Ký kết Hợp đồng số (E-Signature)
1. **Kết nối Đa cổng thanh toán chính thức:**
   - Tích hợp thêm MoMo QR, ZaloPay và thẻ tín dụng quốc tế (Visa/Mastercard qua cổng PayOS / Stripe).
   - Tự động hoàn tiền cọc (Refund API) khi khách hủy chuyến hợp lệ theo đúng quy chế sàn.
2. **Ký số Hợp đồng điện tử (E-Signature):**
   - Tích hợp khung vẽ chữ ký điện tử trực tiếp trên màn hình điện thoại hoặc máy tính trước khi bấm "Nhận xe".
   - Tự động xuất file PDF hợp đồng thuê xe có mã Hash xác thực và chữ ký 3 bên gửi về email của khách và chủ xe.

---

### Giai đoạn 4: Bản đồ Tương tác Tìm xe Xung quanh (Interactive Map View - Leaflet/Mapbox)
- Bên cạnh giao diện danh sách xe, bổ sung nút chuyển đổi `Dạng danh sách` ⟷ `Bản đồ`.
- Hiển thị vị trí các xe đang sẵn sàng bằng pin giá tiền trên bản đồ Hà Nội, TP.HCM, Đà Nẵng, Đà Lạt.
- Khi rê chuột vào pin hiển thị popup ảnh xe, bấm vào xem chi tiết và đặt xe nhanh chóng.

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
