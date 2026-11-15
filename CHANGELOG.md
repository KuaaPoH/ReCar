# Nhật ký cập nhật hệ thống ReCar

## Ca làm việc: 15/11/2026 - 16/11/2026

### 1. Thanh toán ZaloPay & Tài chính
- Thay thế tích hợp VNPay bằng **ZaloPay** (`IZaloPayService`, `ZaloPayService`, `CartController`, `Views/Cart/Checkout.cshtml`, `Success.cshtml`, `PaymentFail.cshtml`), chuẩn hóa nhãn hiển thị thành **ZaloPay**.
- Xóa bỏ hoàn toàn các file VNPay không còn dùng (`IVnpayServices.cs`, `VnPayService.cs`, `VnPayLibrary.cs`, `VnPaymentResponseModel.cs`) và cấu hình liên quan.
- Bổ sung và đồng bộ xử lý tài chính/đối soát ví chủ xe và Admin (`PartnerPayout.cs`, `WalletTransaction.cs`, `FinanceController`, `WalletController`, `OrdersController`).

### 2. Quản lý & Phản hồi Liên hệ khách hàng
- Yêu cầu khách hàng đăng nhập khi gửi liên hệ và tự động đồng bộ thông tin người dùng (`ContactController`, `Views/Contact/Index.cshtml`).
- Xây dựng trang quản lý liên hệ trong Admin (`Areas/Admin/Controllers/ContactsController.cs`, `Areas/Admin/Views/Contacts/Index.cshtml`) đồng bộ với giao diện Admin, hỗ trợ nút **Phản hồi Email** mở trình gửi mail (`mailto:`) điền sẵn người nhận, tiêu đề và nội dung.

### 3. Tinh chỉnh Trang chủ & Trải nghiệm người dùng
- **Danh mục dòng xe:** Chuyển sang CSS Grid `.recar-cat-grid` (`recar.css`, `Views/Home/Index.cshtml`) để các thẻ tự động giãn đều theo số lượng dòng xe hiện có.
- **Xe nổi bật và ưu đãi:** Lọc đúng các xe đang hoạt động có tag `Nổi bật` hoặc đang `Ưu đãi` (`HomeController.cs`), hiển thị 4 xe mỗi trang và tự động bật thanh phân trang khi có từ 5 xe trở lên (`Views/Home/Index.cshtml`).

### 4. Dọn dẹp dự án & Model
- Xóa các thư mục template mẫu không dùng (`giao dien admin/`, `giao dien chu xe/`), `PROMPT_DESIGN_CAR_OWNER.md` và 6 file `.sql` ở thư mục gốc (`create_banks.sql`, `create_brands.sql`, `create_locations.sql`, `dbrenalcar.sql`, `import_real_menus.sql`, `seed_hierarchical_menus.sql`).
- Xóa 6 file Model không còn sử dụng (`Product.cs`, `Message.cs`, `Contract.cs`, `ContractDetail.cs`, `ContractSettlement.cs`, `ContractSettlementDetail.cs`) và gỡ bỏ `DbSet`/navigation property tương ứng trong `DbRenalCarContext.cs`, `User.cs`, `CarType.cs`, `Customer.cs`.
