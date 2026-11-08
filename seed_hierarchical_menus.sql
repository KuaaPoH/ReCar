SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

DELETE FROM `Menu`;

-- =============================================
-- 1. PHÂN HỆ ADMIN: CẤU TRÚC NHÓM CHA - CON
-- =============================================
-- Nhóm 1: TỔNG QUAN
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (100, 'TỔNG QUAN', 'Admin', '#', 'bi-grid-fill', 1, 1, NULL, 'Nhóm điều hướng tổng quan sàn');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (101, 'Tổng quan Dashboard', 'Admin', '/Admin/Home/Index', 'bi-speedometer2', 1, 1, 100, 'Báo cáo doanh số, đơn đặt và biểu đồ tăng trưởng');

-- Nhóm 2: XE & ĐƠN HÀNG
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (110, 'XE & ĐƠN HÀNG', 'Admin', '#', 'bi-car-front', 2, 1, NULL, 'Nhóm quản lý vận hành đội xe và đơn thuê');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(111, 'Phê duyệt xe', 'Admin', '/Admin/Cars/Pending', 'bi-shield-check', 1, 1, 110, 'Kiểm duyệt phương tiện mới đăng ký trước khi mở bán'),
(112, 'Duyệt hồ sơ Chủ xe', 'Admin', '/Admin/CarOwners/Pending', 'bi-person-check-fill', 2, 1, 110, 'Thẩm định CCCD, GPLX và hồ sơ đối tác chủ xe'),
(113, 'Quản lý Đội xe', 'Admin', '/Admin/Cars/Index', 'bi-car-front-fill', 3, 1, 110, 'Kiểm soát trạng thái hoạt động, tạm ngưng, thông tin xe toàn sàn'),
(114, 'Quản lý kiểu xe', 'Admin', '/Admin/CarTypes/Index', 'bi-collection-fill', 4, 1, 110, 'Danh mục phân loại sedan, SUV, MPV, bán tải...'),
(115, 'Đơn thuê xe', 'Admin', '/Admin/Orders/Index', 'bi-clipboard-check-fill', 5, 1, 110, 'Giám sát toàn bộ đơn hàng, cọc sàn thu, tiến độ bàn giao xe'),
(116, 'Tài chính & Đối soát', 'Admin', '/Admin/Finance/Index', 'bi-wallet2', 6, 1, 110, 'Bóc tách hoa hồng sàn và quyết toán thu nhập đối tác');

-- Nhóm 3: NỘI DUNG & TƯƠNG TÁC
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (120, 'NỘI DUNG & TƯƠNG TÁC', 'Admin', '#', 'bi-chat-square-text', 3, 1, NULL, 'Nhóm quản lý truyền thông, menu và phản hồi khách hàng');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(121, 'Bài viết & tin tức', 'Admin', '/Admin/Blogs/Index', 'bi-newspaper', 1, 1, 120, 'Biên tập tin tức, bài viết cẩm nang du lịch và khuyến mãi'),
(122, 'Quản lý Menu', 'Admin', '/Admin/Menus/Index', 'bi-menu-button-wide-fill', 2, 1, 120, 'Cấu hình hệ thống menu điều hướng 3 phân hệ Khách hàng, Chủ xe, Admin'),
(123, 'Đánh giá & Bình luận', 'Admin', '/Admin/Reviews/Index', 'bi-star-half', 3, 1, 120, 'Kiểm duyệt đánh giá sao và phản hồi từ khách thuê');

-- Nhóm 4: HỆ THỐNG
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (130, 'HỆ THỐNG', 'Admin', '#', 'bi-gear-wide-connected', 4, 1, NULL, 'Nhóm cài đặt tài khoản, cấu hình sàn và địa điểm');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(131, 'Địa điểm nhận xe', 'Admin', '/Admin/Locations/Index', 'bi-geo-alt-fill', 1, 1, 130, 'Quản lý sân bay, bến xe, quận huyện giao xe linh hoạt'),
(132, 'Người dùng & Đối tác', 'Admin', '/Admin/UserManagement/Index', 'bi-people-fill', 2, 1, 130, 'Quản lý tài khoản Admin, Khách hàng, Chủ xe'),
(133, 'Cấu hình Sàn ReCar', 'Admin', '/Admin/Settings/Index', 'bi-sliders', 3, 1, 130, 'Thiết lập tỷ lệ cọc, hoa hồng sàn, thông tin thương hiệu ReCar');


-- =============================================
-- 2. PHÂN HỆ CHỦ XE (HOST PRO): CẤU TRÚC NHÓM CHA - CON
-- =============================================
-- Nhóm 1: QUẢN TRỊ PHƯƠNG TIỆN
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (200, 'QUẢN TRỊ PHƯƠNG TIỆN', 'CarOwner', '#', 'bi-speedometer2', 1, 1, NULL, 'Nhóm công cụ quản lý xe của chủ xe');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(201, 'Tổng quan', 'CarOwner', '/CarOwner/Home/Index', 'bi-grid-fill', 1, 1, 200, 'Bảng điều khiển tổng hợp đơn đặt và trạng thái xe'),
(202, 'Quản lý xe', 'CarOwner', '/CarOwner/Cars/Index', 'bi-car-front-fill', 2, 1, 200, 'Danh sách xe sở hữu, bật tắt trạng thái mở cho thuê'),
(203, 'Thêm xe', 'CarOwner', '/CarOwner/Cars/Create', 'bi-plus-circle-fill', 3, 1, 200, 'Đăng ký thêm xe mới đưa lên sàn ReCar'),
(204, 'Lịch xe', 'CarOwner', '/CarOwner/Calendar/Index', 'bi-calendar3', 4, 1, 200, 'Lịch trình bận, ngày khách thuê và bảo dưỡng');

-- Nhóm 2: KINH DOANH & THU NHẬP
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (210, 'KINH DOANH & THU NHẬP', 'CarOwner', '#', 'bi-graph-up-arrow', 2, 1, NULL, 'Nhóm quản lý dòng tiền, đơn thuê và hồ sơ ngân hàng');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(211, 'Lịch trình & Đơn xe', 'CarOwner', '/CarOwner/Orders/Index', 'bi-calendar2-range-fill', 1, 1, 210, 'Theo dõi tiến độ đơn, hợp đồng và biên bản bàn giao xe'),
(212, 'Phân tích hiệu suất', 'CarOwner', '/CarOwner/Analytics/Index', 'bi-bar-chart-line-fill', 2, 1, 210, 'Báo cáo doanh thu, tỷ lệ lấp đầy lịch và hiệu quả xe'),
(213, 'Ví đối tác & Đối soát', 'CarOwner', '/CarOwner/Wallet/Index', 'bi-wallet2', 3, 1, 210, 'Theo dõi tiền cọc sàn thu, hoa hồng và rút tiền về ngân hàng'),
(214, 'Hồ sơ pháp lý & Ngân hàng', 'CarOwner', '/CarOwner/Register/Pending', 'bi-file-earmark-text-fill', 4, 1, 210, 'Cập nhật tài khoản thụ hưởng và giấy tờ định danh');


-- =============================================
-- 3. PHÂN HỆ KHÁCH HÀNG: CẤU TRÚC HEADER/FOOTER
-- =============================================
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(301, 'Trang chủ', 'Customer', '/', 'bi-house-door-fill', 1, 1, NULL, 'Trang chủ sàn xe ReCar'),
(302, 'DỊCH VỤ THUÊ XE', 'Customer', '#', 'bi-car-front-fill', 2, 1, NULL, 'Menu xổ xuống danh mục thuê xe'),
(303, 'Thuê xe tự lái', 'Customer', '/Product?driveMode=self', 'bi-person-workspace', 1, 1, 302, 'Trải nghiệm tự do khám phá với xe tự lái'),
(304, 'Thuê xe có tài xế', 'Customer', '/Product?driveMode=driver', 'bi-person-badge-fill', 2, 1, 302, 'Dịch vụ có tài xế chuyên nghiệp, chu đáo'),
(305, 'Trở thành Chủ xe', 'Customer', '/CarOwner/Register', 'bi-cash-coin', 3, 1, NULL, 'Đăng ký chia sẻ xe và tạo thu nhập cùng ReCar'),
(306, 'Tin tức & Cẩm nang', 'Customer', '/Blog', 'bi-newspaper', 4, 1, NULL, 'Kinh nghiệm du lịch, lái xe an toàn'),
(307, 'VỀ RECAR & TRỢ GIÚP', 'Customer', '#', 'bi-info-circle-fill', 5, 1, NULL, 'Menu thông tin và trung tâm trợ giúp'),
(308, 'Giới thiệu về ReCar', 'Customer', '/Home/About', 'bi-buildings', 1, 1, 307, 'Câu chuyện thương hiệu và sứ mệnh ReCar'),
(309, 'Liên hệ hỗ trợ', 'Customer', '/Home/Contact', 'bi-headset', 2, 1, 307, 'Hotline 1900 633 633 và hỗ trợ 24/7');

