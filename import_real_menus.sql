SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

DELETE FROM `Menu`;

-- ====================================================================
-- 1. PHÂN HỆ QUẢN TRỊ VIÊN (ADMIN) - Khớp 100% Sidebar Admin gốc
-- ====================================================================
-- Header 1: TỔNG QUAN
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (1, 'TỔNG QUAN', 'Admin', '#', 'bi-grid-fill', 1, 1, NULL, 'Nhóm Tổng quan');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (2, 'Tổng quan', 'Admin', '/Admin/Home/Index', 'bi-grid-fill', 1, 1, 1, 'Trang tổng quan hệ thống Admin');

-- Header 2: XE & ĐƠN HÀNG
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (3, 'XE & ĐƠN HÀNG', 'Admin', '#', 'bi-car-front', 2, 1, NULL, 'Nhóm Xe & Đơn hàng');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(4, 'Phê duyệt xe', 'Admin', '/Admin/Cars/Pending', 'bi-shield-check', 1, 1, 3, 'Phê duyệt xe mới đăng ký'),
(5, 'Duyệt hồ sơ Chủ xe', 'Admin', '/Admin/CarOwners/Pending', 'bi-person-check-fill', 2, 1, 3, 'Duyệt hồ sơ đối tác chủ xe'),
(6, 'Quản lý đội xe', 'Admin', '/Admin/Cars/Index', 'bi-car-front-fill', 3, 1, 3, 'Quản lý toàn bộ danh sách xe'),
(7, 'Quản lý kiểu xe', 'Admin', '/Admin/CarTypes/Index', 'bi-collection-fill', 4, 1, 3, 'Quản lý danh mục kiểu dáng xe'),
(8, 'Đơn thuê xe', 'Admin', '/Admin/Orders/Index', 'bi-clipboard-check-fill', 5, 1, 3, 'Quản lý hợp đồng & đơn thuê xe'),
(9, 'Tài chính & đối soát', 'Admin', '/Admin/Finance/Index', 'bi-wallet2', 6, 1, 3, 'Tài chính hoa hồng & đối soát sàn');

-- Header 3: NỘI DUNG & TƯƠNG TÁC
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (10, 'NỘI DUNG & TƯƠNG TÁC', 'Admin', '#', 'bi-chat-square-text', 3, 1, NULL, 'Nhóm Nội dung & Tương tác');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(11, 'Bài viết & tin tức', 'Admin', '/Admin/Blogs/Index', 'bi-newspaper', 1, 1, 10, 'Biên tập bài viết & tin tức'),
(12, 'Quản lý Menu', 'Admin', '/Admin/Menus/Index', 'bi-menu-button-wide-fill', 2, 1, 10, 'Cấu hình menu 3 phân hệ'),
(13, 'Đánh giá & Bình luận', 'Admin', '/Admin/Reviews/Index', 'bi-star-half', 3, 1, 10, 'Kiểm duyệt đánh giá & bình luận');

-- Header 4: HỆ THỐNG
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (14, 'HỆ THỐNG', 'Admin', '#', 'bi-gear-fill', 4, 1, NULL, 'Nhóm Cài đặt hệ thống');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(15, 'Địa điểm nhận xe', 'Admin', '/Admin/Locations/Index', 'bi-geo-alt-fill', 1, 1, 14, 'Quản lý các điểm giao nhận xe'),
(16, 'Người dùng & đối tác', 'Admin', '/Admin/UserManagement/Index', 'bi-people-fill', 2, 1, 14, 'Quản lý tài khoản người dùng & đối tác'),
(17, 'Cấu hình hệ thống', 'Admin', '/Admin/Settings/Index', 'bi-gear-fill', 3, 1, 14, 'Cấu hình tỷ lệ cọc, hoa hồng và hệ thống');


-- ====================================================================
-- 2. PHÂN HỆ CHỦ XE (CAROWNER) - Khớp 100% Sidebar Chủ xe gốc
-- ====================================================================
-- Header 1: QUẢN TRỊ PHƯƠNG TIỆN
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (20, 'QUẢN TRỊ PHƯƠNG TIỆN', 'CarOwner', '#', 'bi-speedometer2', 1, 1, NULL, 'Nhóm Quản trị phương tiện');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(21, 'Tổng quan', 'CarOwner', '/CarOwner/Home/Index', 'bi-grid-fill', 1, 1, 20, 'Bảng điều khiển chủ xe'),
(22, 'Quản lý xe', 'CarOwner', '/CarOwner/Cars/Index', 'bi-car-front-fill', 2, 1, 20, 'Danh sách xe cho thuê'),
(23, 'Thêm xe', 'CarOwner', '/CarOwner/Cars/Create', 'bi-plus-circle-fill', 3, 1, 20, 'Đăng ký thêm xe mới'),
(24, 'Lịch xe', 'CarOwner', '/CarOwner/Calendar/Index', 'bi-calendar3', 4, 1, 20, 'Lịch trình bận & bảo dưỡng xe');

-- Header 2: KINH DOANH & THU NHẬP
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES (25, 'KINH DOANH & THU NHẬP', 'CarOwner', '#', 'bi-cash-coin', 2, 1, NULL, 'Nhóm Kinh doanh & Thu nhập');

INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(26, 'Lịch trình & Đơn xe', 'CarOwner', '/CarOwner/Orders/Index', 'bi-calendar2-range-fill', 1, 1, 25, 'Quản lý đơn thuê & lịch giao nhận xe'),
(27, 'Phân tích hiệu suất', 'CarOwner', '/CarOwner/Analytics/Index', 'bi-bar-chart-line-fill', 2, 1, 25, 'Thống kê doanh thu & hiệu suất hoạt động'),
(28, 'Ví đối tác & Đối soát', 'CarOwner', '/CarOwner/Wallet/Index', 'bi-wallet2', 3, 1, 25, 'Quản lý số dư, tiền cọc & rút tiền'),
(29, 'Hồ sơ pháp lý & Ngân hàng', 'CarOwner', '/CarOwner/Register/Pending', 'bi-file-earmark-text-fill', 4, 1, 25, 'Hồ sơ pháp lý, CCCD/GPLX & tài khoản ngân hàng');


-- ====================================================================
-- 3. PHÂN HỆ KHÁCH HÀNG (CUSTOMER) - Khớp 100% Navbar Header gốc
-- ====================================================================
INSERT INTO `Menu` (`MenuId`, `Title`, `Subsystem`, `Url`, `Icon`, `Position`, `IsActive`, `ParentId`, `Description`)
VALUES 
(31, 'Trang chủ', 'Customer', '/', 'bi-house-door', 1, 1, NULL, 'Trang chủ ReCar'),
(32, 'Thuê xe', 'Customer', '/Product', 'bi-car-front', 2, 1, NULL, 'Danh mục tìm kiếm và thuê xe ô tô'),
(33, 'Cẩm nang', 'Customer', '/Blog', 'bi-journal-bookmark', 3, 1, NULL, 'Cẩm nang kinh nghiệm lái xe & du lịch'),
(34, 'Tin tức', 'Customer', '/News', 'bi-newspaper', 4, 1, NULL, 'Tin tức & sự kiện ReCar'),
(35, 'Liên hệ', 'Customer', '/Contact', 'bi-telephone', 5, 1, NULL, 'Liên hệ hỗ trợ 24/7');

