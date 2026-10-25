# PROMPT THIẾT KẾ & TÁI CẤU TRÚC GIAO DIỆN PHÂN HỆ CHỦ XE (CAR OWNER PORTAL)
> **Mục tiêu:** Sử dụng prompt này để triển khai thiết kế lại toàn diện phân hệ Chủ xe (`/CarOwner`) ở các phiên làm việc tiếp theo, đạt chuẩn thẩm mỹ cao cấp **Pixel-Perfect, Modern SaaS Dashboard** (tương tự Lovable / Shadcn UI / Vercel Dashboard).

---

## 1. BỐI CẢNH & PHẠM VI DỰ ÁN
* **Dự án:** ReCar – Nền tảng chia sẻ & cho thuê xe ô tô trực tuyến (Car-Sharing Platform).
* **Phân hệ cần cải tổ:** Phân hệ Đối tác Chủ xe (`Areas/CarOwner`), bao gồm:
  1. `CarOwner/Views/Shared/_Layout.cshtml` (Khung Shell, Sidebar công thái học, Header Topbar).
  2. `CarOwner/Views/Home/Index.cshtml` (Dashboard Bảng điều khiển tổng quan, báo cáo doanh thu, thống kê xe).
  3. `CarOwner/Views/Cars/Index.cshtml` (Danh sách đội xe: Grid cards & Table view).
  4. `CarOwner/Views/Cars/Create.cshtml` (Biểu mẫu đăng ký xe mới đa bước - Multi-step Wizard).
  5. `CarOwner/Views/Cars/Edit.cshtml` (Biểu mẫu cập nhật thông tin xe & giấy tờ pháp lý).
  6. `CarOwner/Views/Orders/Index.cshtml` (Danh sách đơn thuê xe & lịch đặt).
  7. `CarOwner/Views/Orders/Details.cshtml` (Chi tiết đơn thuê, phân rã tài chính, biên bản bàn giao xe).
  8. `CarOwner/Views/Register/Index.cshtml` (Luồng đăng ký Đối tác Chủ xe KYC & Liên kết tài khoản ngân hàng).

---

## 2. NGUYÊN TẮC THIẾT KẾ & HỆ THỐNG THẨM MỸ (DESIGN SYSTEM)
### 2.1. Bảng màu chủ đạo (Color Palette):
* **Nền chính (Background):** Slate `#f8fafc` hoặc `#f1f5f9` sạch sẽ, thoáng mắt.
* **Thanh Sidebar (Navigation Shell):** Deep Navy cao cấp `#0f172a` hoặc Slate `#1e293b`, có phân cấp phân vùng tinh tế.
* **Màu thương hiệu (Primary Accent):** ReCar Blue `#0284c7` (hover: `#0369a1`), xanh ngọc nổi bật `#38bdf8`.
* **Màu hỗ trợ:**
  * Success (Sẵn sàng / Đã duyệt / Doanh thu): `#10b981` (soft: `#ecfdf5`).
  * Warning (Chờ duyệt / Đang bảo trì): `#f59e0b` (soft: `#fffbeb`).
  * Danger / Destructive (Từ chối / Tạm dừng / Hủy): `#ef4444` (soft: `#fef2f2`).
  * Info (Đang có khách thuê): `#3b82f6` (soft: `#eff6ff`).

### 2.2. Nghệ thuật chữ & Kiểu dáng (Typography & Geometry):
* **Font chữ:** `Plus Jakarta Sans` hoặc `Inter` hỗ trợ tiếng Việt đầy đủ, line-height thông thoáng `1.5 - 1.6`.
* **Bo góc (Border Radius):** Bo góc mềm mại, thống nhất `rounded-2xl` (16px) cho Cards và Panels; `rounded-lg` (8px) cho Inputs, Buttons và Badges.
* **Viền & Bóng đổ (Borders & Shadows):** Viền tinh tế `1px solid #e2e8f0`, bóng đổ nhiều lớp siêu nhẹ `box-shadow: 0 1px 3px 0 rgba(15, 23, 42, 0.05), 0 1px 2px -1px rgba(15, 23, 42, 0.05)`.
* **Tuyệt đối không dùng phong cách lỗi thời:** Loại bỏ hoàn toàn gradient gắt, đổ bóng thô đen, viền đậm 2px-3px hoặc màu tím không đồng bộ.

---

## 3. YÊU CẦU CHI TIẾT THEO TỪNG MÀN HÌNH

### 3.1. Shell Layout & Sidebar Điều Hướng (`Shared/_Layout.cshtml`)
* **Logo ReCar Host:** Logo xe phong cách tối giản kèm chữ `ReCar Host - Đối tác Chủ xe` với huy hiệu đối tác uy tín.
* **Hồ sơ chủ xe mini (Host Mini Profile):** Avatar chữ cái hoặc ảnh tròn, tên chủ xe, email, nhãn `Đã xác minh KYC`.
* **Nhóm điều hướng (Navigation Grouping):**
  * *Tổng quan & Thống kê:* Dashboard, Doanh thu & Báo cáo.
  * *Quản trị phương tiện:* Quản lý đội xe, Đăng ký xe mới (`+ Mới`).
  * *Kinh doanh & Khách hàng:* Đơn thuê xe, Lịch nhận xe, Tin nhắn Chat với khách.
  * *Hồ sơ & Tài khoản:* Hồ sơ pháp lý KYC, Tài khoản Ngân hàng nhận tiền.
  * *Lối tắt chân trang:* Xem sàn khách thuê (`/`), Đăng xuất.

### 3.2. Bảng Điều Khiển Tổng Quan (`Home/Index.cshtml`)
* **4 Khối thống kê (Stat Cards):**
  * Tổng thu nhập thực nhận (kèm % tăng trưởng tháng).
  * Tổng số xe đang quản lý (phân tách rõ: xe sẵn sàng, xe đang bận, xe chờ duyệt).
  * Đơn thuê tháng này & Tỷ lệ lấp đầy lịch xe (% Occupancy Rate).
  * Điểm đánh giá sao từ khách (★ 4.9/5.0).
* **Biểu đồ doanh thu 6 tháng:** Biểu đồ cột tương tác hoặc CSS Bar Chart phân bổ theo từng tháng với tooltip hiển thị số tiền chi tiết.
* **Khối Hoạt động sắp tới (Upcoming Handover):** Danh sách các chuyến xe sắp bàn giao trong 24h tới kèm đồng hồ đếm ngược và SĐT khách thuê.

### 3.3. Trang Quản Lý Đội Xe (`Cars/Index.cshtml`)
* **Thanh công cụ đỉnh cao:**
  * Ô tìm kiếm đa năng (theo tên xe, biển số, đời xe).
  * Bộ lọc trạng thái dạng Tab viên thuốc: *Tất cả, Sẵn sàng, Đang có khách, Chờ duyệt, Bảo trì*.
  * Chuyển đổi linh hoạt 2 chế độ: **Dạng lưới thẻ (Grid Cards)** và **Dạng bảng chi tiết (Table View)**.
* **Dạng lưới thẻ (Grid View):** Thẻ xe sang trọng, ảnh 16:10, nhãn trạng thái nổi trên ảnh, biển số font monospace, 3 thông số chính (chỗ, hộp số, nhiên liệu), giá thuê/ngày nổi bật và cụm nút thao tác gọn gàng (Sửa, Xem trên sàn, Tạm dừng).
* **Dạng bảng chi tiết (Table View):** Bảng `.recar-table` rộng rãi với `min-width` từng cột, padding thoáng mát, có hover effect, không bao giờ dính chữ vào nhau.

### 3.4. Form Đăng Xe Mới Đa Bước (`Cars/Create.cshtml`)
Chia quy trình thêm xe thành **Wizard 4 bước mượt mà**:
1. **Bước 1: Thông tin cơ bản & Dòng xe**
   * Tên xe, Hãng xe (Dropdown nạp từ CSDL `car_brands`), Phân khúc kiểu xe (`car_types`), Năm sản xuất, Màu sắc (viết tự do), Biển số xe.
2. **Bước 2: Thông số kỹ thuật & Vận hành**
   * Số chỗ ngồi, Hộp số (*Tự động / Số sàn*), Loại nhiên liệu (*Xăng / Dầu / Điện*), Mức tiêu hao nhiên liệu (lít/100km), Tính năng tiện ích dạng checkbox (Bản đồ, Camera lùi, Cảm biến lốp, Bluetooth, Cửa sổ trời...).
3. **Bước 3: Giá thuê & Địa điểm bàn giao**
   * Giá niêm yết 1 ngày (VNĐ), Phụ phí tài xế nếu có dịch vụ lái xe, Địa chỉ bàn giao xe cụ thể (Dropdown chọn Tỉnh/Thành phố liên kết CSDL `locations` + ô nhập địa chỉ chi tiết).
4. **Bước 4: Hồ sơ hình ảnh & Giấy tờ pháp lý KYC**
   * Khu vực upload 4 ảnh xe thực tế (Ngoại thất trước, Sau, Nội thất, Góc nghiêng) kèm xem trước ảnh (Preview) và nút xóa/thay đổi.
   * Khu vực upload 3 Giấy tờ bắt buộc: **Giấy đăng ký xe (Cà vẹt)**, **Giấy chứng nhận đăng kiểm**, **Bảo hiểm TNDS bắt buộc**.
   * Nút "Gửi hồ sơ kiểm duyệt" với hiệu ứng loading và thông báo xác nhận.

### 3.5. Form Chỉnh Sửa Thông Tin Xe (`Cars/Edit.cshtml`)
* Tuyệt đối không cho chủ xe tự ý sửa trạng thái xe sang "Sẵn sàng" khi đang chờ duyệt.
* Nếu xe bị Admin từ chối, hiển thị rõ khung lý do từ chối của Admin màu đỏ nhạt, kèm hướng dẫn chỉnh sửa và nút "Gửi lại hồ sơ duyệt".
* Xem và cập nhật lại ảnh xe cũng như các giấy tờ đã tải lên trước đó.

### 3.6. Quản Lý Đơn Thuê Xe & Lịch Trình (`Orders/Index.cshtml` & `Details.cshtml`)
* Bảng đơn xe minh bạch trạng thái: *Chờ thanh toán, Đã đặt cọc 30%, Chờ bàn giao, Đang thuê, Hoàn tất, Đã hủy*.
* Trang chi tiết đơn phân rã tài chính 3 bên:
  * Doanh thu của chủ xe (đã trừ phí sàn 10-15%).
  * Số tiền cọc khách đã trả qua VNPay.
  * Số tiền còn lại khách sẽ thanh toán trực tiếp khi nhận xe.
* Tích hợp nút Xác nhận bàn giao xe, Biên bản kiểm tra hiện trạng xe và Gọi điện thoại trực tiếp cho người thuê.

---

## 4. HƯỚNG DẪN THỰC THI CHO AI AGENT Ở PHIÊN TIẾP THEO
* Đọc kỹ file này và đối chiếu với cấu trúc Controller / View hiện có trong `CarRental/Areas/CarOwner/`.
* Sử dụng hoàn toàn **Vanilla CSS / Modern CSS Variables** trong các thẻ `<style>` hoặc file CSS riêng của phân hệ `CarOwner`, đảm bảo không phụ thuộc vào thư viện bên ngoài gây xung đột.
* Kiểm tra kỹ độ tương thích responsive trên các màn hình phổ biến: Desktop (1920x1080), Laptop (1366x768), Tablet và Mobile.
* Duy trì các quy tắc cốt lõi: Luôn trả lời bằng tiếng Việt, bảo toàn toàn bộ logic C# backend và database hiện có.
