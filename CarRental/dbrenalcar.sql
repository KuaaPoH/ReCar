-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Máy chủ: 127.0.0.1
-- Thời gian đã tạo: Th10 16, 2026 lúc 02:50 PM
-- Phiên bản máy phục vụ: 10.4.32-MariaDB
-- Phiên bản PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Cơ sở dữ liệu: `dbrenalcar`
--

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `amenities`
--

CREATE TABLE `amenities` (
  `AmenityId` int(11) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Icon` varchar(50) DEFAULT NULL,
  `IconColor` varchar(30) DEFAULT NULL,
  `DisplayOrder` int(11) NOT NULL DEFAULT 0,
  `IsActive` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `amenities`
--

INSERT INTO `amenities` (`AmenityId`, `Name`, `Icon`, `IconColor`, `DisplayOrder`, `IsActive`) VALUES
(1, 'Bản đồ dẫn đường GPS', 'bi-geo-alt-fill', 'text-danger', 1, 1),
(2, 'Camera lùi / Camera 360', 'bi-camera-video-fill', 'text-primary', 2, 1),
(3, 'Camera hành trình', 'bi-record-circle-fill', 'text-danger', 3, 1),
(4, 'Cảm biến áp suất lốp', 'bi-speedometer2', 'text-warning', 4, 1),
(5, 'Apple CarPlay & Android Auto', 'bi-phone-fill', 'text-info', 5, 1),
(6, 'Cửa sổ trời Panorama', 'bi-brightness-high-fill', 'text-warning', 6, 1),
(7, 'Thu phí tự động ETC', 'bi-credit-card-2-front-fill', 'text-success', 7, 1),
(8, 'Ghế da cao cấp chỉnh điện', 'bi-award-fill', 'text-primary', 8, 1),
(9, 'Ghế trẻ em ISOFIX', 'bi-person-heart', 'text-danger', 9, 1),
(10, 'Sạc không dây Qi', 'bi-lightning-charge-fill', 'text-warning', 10, 1),
(11, 'Cảm biến va chạm trước/sau', 'bi-shield-check', 'text-success', 11, 1),
(12, 'Cảnh báo điểm mù BSM', 'bi-eye-fill', 'text-secondary', 12, 1),
(13, 'Khóa thông minh SmartKey', 'bi-key-fill', 'text-primary', 13, 1),
(14, 'Cốp điện thông minh', 'bi-box-seam-fill', 'text-secondary', 14, 1),
(19, 'Kết nối Bluetooth', 'bi-bluetooth', 'text-primary', 15, 1),
(21, 'Cổng sạc USB / Type-C', 'bi-usb-symbol', 'text-secondary', 17, 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `banks`
--

CREATE TABLE `banks` (
  `BankId` int(11) NOT NULL,
  `BankCode` varchar(20) NOT NULL,
  `BankName` varchar(150) NOT NULL,
  `ShortName` varchar(50) NOT NULL,
  `Bin` varchar(10) DEFAULT NULL,
  `Logo` varchar(255) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT 0,
  `IsActive` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `banks`
--

INSERT INTO `banks` (`BankId`, `BankCode`, `BankName`, `ShortName`, `Bin`, `Logo`, `DisplayOrder`, `IsActive`) VALUES
(1, 'VCB', 'Ngân hàng TMCP Ngoại thương Việt Nam', 'Vietcombank', '970436', NULL, 1, 1),
(2, 'MB', 'Ngân hàng TMCP Quân đội', 'MB Bank', '970422', NULL, 2, 1),
(3, 'TCB', 'Ngân hàng TMCP Kỹ thương Việt Nam', 'Techcombank', '970407', NULL, 3, 1),
(4, 'BIDV', 'Ngân hàng TMCP Đầu tư và Phát triển Việt Nam', 'BIDV', '970418', NULL, 4, 1),
(5, 'CTG', 'Ngân hàng TMCP Công thương Việt Nam', 'VietinBank', '970415', NULL, 5, 1),
(6, 'VPB', 'Ngân hàng TMCP Việt Nam Thịnh Vượng', 'VPBank', '970432', NULL, 6, 1),
(7, 'ACB', 'Ngân hàng TMCP Á Châu', 'ACB', '970416', NULL, 7, 1),
(8, 'TPB', 'Ngân hàng TMCP Tiên Phong', 'TPBank', '970423', NULL, 8, 1),
(9, 'STB', 'Ngân hàng TMCP Sài Gòn Thương Tín', 'Sacombank', '970403', NULL, 9, 1),
(10, 'HDB', 'Ngân hàng TMCP Phát triển TP.HCM', 'HDBank', '970437', NULL, 10, 1),
(11, 'VBA', 'Ngân hàng Nông nghiệp & PTNT Việt Nam', 'Agribank', '970405', NULL, 11, 1),
(12, 'VIB', 'Ngân hàng TMCP Quốc tế Việt Nam', 'VIB', '970441', NULL, 12, 1),
(13, 'SHB', 'Ngân hàng TMCP Sài Gòn - Hà Nội', 'SHB', '970443', NULL, 13, 1),
(14, 'MSB', 'Ngân hàng TMCP Hàng Hải Việt Nam', 'MSB', '970426', NULL, 14, 1),
(15, 'OCB', 'Ngân hàng TMCP Phương Đông', 'OCB', '970448', NULL, 15, 1),
(16, 'SSB', 'Ngân hàng TMCP Đông Nam Á', 'SeABank', '970440', NULL, 16, 1),
(17, 'LPB', 'Ngân hàng TMCP Lộc Phát Việt Nam', 'LPBank', '970449', NULL, 17, 1),
(18, 'NAB', 'Ngân hàng TMCP Nam Á', 'Nam A Bank', '970428', NULL, 18, 1),
(19, 'BVB', 'Ngân hàng TMCP Bản Việt', 'BVBank', '970454', NULL, 19, 1),
(20, 'BAB', 'Ngân hàng TMCP Bắc Á', 'Bac A Bank', '970409', NULL, 20, 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `blog`
--

CREATE TABLE `blog` (
  `BlogId` int(11) NOT NULL,
  `Title` varchar(250) DEFAULT NULL,
  `Alias` varchar(250) DEFAULT NULL,
  `CategoryId` int(11) DEFAULT NULL,
  `Description` varchar(4000) DEFAULT NULL,
  `Detail` longtext DEFAULT NULL,
  `Image` varchar(500) DEFAULT NULL,
  `SeoTitle` varchar(250) DEFAULT NULL,
  `SeoDescription` varchar(500) DEFAULT NULL,
  `SeoKeywords` varchar(250) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  `CreatedBy` varchar(150) DEFAULT NULL,
  `ModifiedDate` datetime DEFAULT NULL,
  `ModifiedBy` varchar(150) DEFAULT NULL,
  `CustomerId` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT 0,
  `Views` int(11) NOT NULL DEFAULT 0,
  `IsFeatured` tinyint(1) NOT NULL DEFAULT 0,
  `Badge` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `blog`
--

INSERT INTO `blog` (`BlogId`, `Title`, `Alias`, `CategoryId`, `Description`, `Detail`, `Image`, `SeoTitle`, `SeoDescription`, `SeoKeywords`, `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`, `CustomerId`, `IsActive`, `Views`, `IsFeatured`, `Badge`) VALUES
(1, 'Kinh nghiệm thuê xe tự lái lần đầu cho người mới', 'kinh-nghiem-thue-xe-tu-lai-lan-dau', NULL, 'Những điều cần kiểm tra trước khi nhận xe để chuyến đi của bạn suôn sẻ và an toàn nhất.', 'Khi nhận xe tự lái, bạn cần quay video xung quanh thân xe, kiểm tra mức xăng/pin, chạy thử 500m để cảm nhận phanh và vô lăng, kiểm tra giấy tờ đăng kiểm xe...', '/images/cars/car-sedan.jpg', NULL, NULL, NULL, '2026-10-13 20:39:31', 'ReCar Team', NULL, NULL, NULL, 1, 920, 0, NULL),
(2, 'Giấy tờ cần chuẩn bị khi thuê xe ô tô tự lái', 'giay-to-can-chuan-bi-khi-thue-o-to', NULL, 'CCCD gắn chip, Giấy phép lái xe hạng B trở lên và các giấy tờ cần thiết để ký hợp đồng.', 'Để hoàn tất thủ tục thuê xe tại ReCar, bạn chỉ cần tải ảnh chụp CCCD và Giấy phép lái xe hạng B1/B2 lên tài khoản và đối chiếu bản gốc lúc nhận bàn giao xe...', '/images/cars/car-suv.jpg', NULL, NULL, NULL, '2026-10-13 20:39:31', 'ReCar Team', NULL, NULL, NULL, 1, 925, 0, NULL),
(3, '5 cung đường đẹp quanh Đà Lạt đáng trải nghiệm nhất', '5-cung-duong-dep-quanh-da-lat', NULL, 'Gợi ý lộ trình du lịch cuối tuần lý tưởng cho những ai yêu thích tự lái xe qua các cung đèo hùng vĩ.', 'Đà Lạt nổi tiếng với những cung đường uốn lượn quanh rừng thông như đèo Tà Nung, đèo Mimosa, cung đường hoa dã quỳ Cầu Đất...', '/images/cars/hero.jpg', NULL, NULL, NULL, '2024-12-01 11:34:04', 'thanh', '2024-12-01 11:47:04', 'thanh', NULL, 1, 2890, 1, 'Nổi bật'),
(7, 'Phiên bản cuối cùng của Porsche 911 Dakar: Đỉnh cao của sự đặc biệt', 'phien-ban-cuoi-cung-cua-porsche-911-dakar-dinh-cao-cua-su-dac-biet', NULL, 'Đỉnh cao của sự đặc biệt', '<h2 id=\"Thi+t+k++ngo+i+th+t+c+a+Porsche+911+Dakar\" name=\"Thi+t+k++ngo+i+th+t+c+a+Porsche+911+Dakar\" style=\"font-family: Roboto, sans-serif; font-weight: 700; color: rgb(34, 34, 34); font-size: 20px;\">Thiết kế ngoại thất của Porsche 911 Dakar</h2><p style=\"color: rgb(34, 34, 34); font-family: Roboto, sans-serif;\">Chiếc&nbsp;<a href=\"https://www.carmudi.vn/blog-xe-hoi/porsche-911-gt3-2022-mau-la-mat-trang-bi-so-san-dau-tien-tai-viet-nam/\">Porsche 911</a>&nbsp;Dakar cuối cùng xuất xưởng được thiết kế ngoại thất độc đáo, tôn vinh chiến thắng tại giải đua Paris-Dakar năm 1984. Phần trên của xe được sơn màu vàng Signal, trong khi phần dưới mang sắc xanh Gentian Blue Metallic, tạo nên sự tương phản nổi bật. Hai tông màu này được ngăn cách bởi đường kẻ xanh dương nhạt Lampedusa Blue, lấy cảm hứng từ hòn đảo Lampedusa ở Địa Trung Hải.</p><p style=\"color: rgb(34, 34, 34); font-family: Roboto, sans-serif;\"><img src=\"/files/Blog/blog1.jpeg\" style=\"width: 1038px;\"></p><p style=\"color: rgb(34, 34, 34); font-family: Roboto, sans-serif;\">Đặc biệt, bộ mâm xe cũng được sơn màu vàng Signal, kết hợp với viền xanh Lampedusa Blue, tạo nên sự hài hòa và tinh tế. Các chi tiết bằng thép không gỉ ở cản trước, cản sau và ngưỡng cửa bên hông được sơn đen, tăng thêm vẻ mạnh mẽ cho xe. Ngoài ra, viền đèn pha cũng được trang trí bằng màu xanh Lampedusa Blue, tạo điểm nhấn độc đáo cho phần đầu xe.</p><p style=\"color: rgb(34, 34, 34); font-family: Roboto, sans-serif;\">Toàn bộ quá trình sơn ngoại thất được thực hiện thủ công, thể hiện sự tỉ mỉ và đẳng cấp của Porsche trong việc cá nhân hóa sản phẩm. Thiết kế này không chỉ tôn vinh lịch sử huy hoàng của hãng mà còn mang đến cho chủ sở hữu một chiếc xe độc nhất vô nhị, kết hợp giữa di sản và phong cách hiện đại.</p>', '/files/Blog/blog1.jpeg', NULL, NULL, NULL, '2024-12-01 11:34:04', 'thanh', '2026-11-11 20:54:37', 'Admin', NULL, 1, 920, 0, NULL);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `blogcomment`
--

CREATE TABLE `blogcomment` (
  `CommentId` int(11) NOT NULL,
  `Name` varchar(50) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  `Detail` varchar(200) DEFAULT NULL,
  `BlogId` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `Reply` longtext DEFAULT NULL,
  `ReplyDate` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `blogcomment`
--

INSERT INTO `blogcomment` (`CommentId`, `Name`, `Phone`, `Email`, `CreatedDate`, `Detail`, `BlogId`, `IsActive`, `Reply`, `ReplyDate`) VALUES
(2, 'nguyễn Hồng quân', 'Phone', 'quan@gmail.com', '2014-11-22 00:00:00', 'this is blog details', 3, 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `carimages`
--

CREATE TABLE `carimages` (
  `ImageID` int(11) NOT NULL,
  `Image1` longtext DEFAULT NULL,
  `CarID` int(11) DEFAULT NULL,
  `Image2` longtext DEFAULT NULL,
  `Image3` longtext DEFAULT NULL,
  `Image4` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `carowner_registrations`
--

CREATE TABLE `carowner_registrations` (
  `Id` int(11) NOT NULL,
  `CustomerId` int(11) NOT NULL,
  `FullName` varchar(100) NOT NULL,
  `Phone` varchar(20) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `IdCard` varchar(25) NOT NULL,
  `IdCardIssueDate` datetime DEFAULT NULL,
  `IdCardIssuePlace` varchar(100) DEFAULT NULL,
  `IdCardFrontImage` varchar(255) DEFAULT NULL,
  `IdCardBackImage` varchar(255) DEFAULT NULL,
  `LicenseNumber` varchar(25) NOT NULL,
  `LicenseClass` varchar(20) DEFAULT 'B2',
  `LicenseExpiryDate` datetime DEFAULT NULL,
  `LicenseImage` varchar(255) DEFAULT NULL,
  `BankName` varchar(100) NOT NULL,
  `AccountNumber` varchar(50) NOT NULL,
  `AccountHolder` varchar(100) NOT NULL,
  `Status` varchar(30) NOT NULL DEFAULT 'Pending',
  `RejectReason` longtext DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `ApprovedAt` datetime DEFAULT NULL,
  `ApprovedBy` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `carowner_registrations`
--

INSERT INTO `carowner_registrations` (`Id`, `CustomerId`, `FullName`, `Phone`, `Email`, `Address`, `IdCard`, `IdCardIssueDate`, `IdCardIssuePlace`, `IdCardFrontImage`, `IdCardBackImage`, `LicenseNumber`, `LicenseClass`, `LicenseExpiryDate`, `LicenseImage`, `BankName`, `AccountNumber`, `AccountHolder`, `Status`, `RejectReason`, `CreatedAt`, `ApprovedAt`, `ApprovedBy`) VALUES
(1, 1021, 'Trần Ngọc Hải', '0866459082', 'trankenn04hoyo@gmail.com', 'Nghệ An', '080208028372', '2004-10-10 00:00:00', 'Nghệ An', 'Screenshot 2026-10-20 211958_20261025223623076.png', 'test_20261025223623080.jpg', '717254718131', 'B2', '2027-10-10 00:00:00', 'vangla_20261025223623081.jpg', 'MB Bank (MB)', '225510108888', 'TRAN NGOC HAI', 'Approved', NULL, '2026-10-25 22:50:27', '2026-10-25 22:50:48', 'Admin');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `carrentalorders`
--

CREATE TABLE `carrentalorders` (
  `OrderID` int(11) NOT NULL,
  `CustomerID` int(11) NOT NULL,
  `OrderDate` date DEFAULT NULL,
  `Deposit` decimal(18,2) DEFAULT NULL,
  `Payment` decimal(18,2) DEFAULT NULL,
  `StatusID` int(11) DEFAULT NULL,
  `ReturnDate` date DEFAULT NULL,
  `Notes` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PlatformFeeRate` decimal(5,2) DEFAULT 10.00,
  `PlatformFee` decimal(18,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `carrentalorders`
--

INSERT INTO `carrentalorders` (`OrderID`, `CustomerID`, `OrderDate`, `Deposit`, `Payment`, `StatusID`, `ReturnDate`, `Notes`, `PlatformFeeRate`, `PlatformFee`) VALUES
(1030, 1, '2025-12-21', 240000.00, 560000.00, 2, NULL, '', 10.00, 80000.00),
(1043, 1020, '2026-10-16', 390000.00, 910000.00, 2, NULL, 'Hình thức: Tự lái | [Chủ xe hủy đơn lúc 28/10/2026', 10.00, 130000.00),
(1044, 1021, '2026-10-28', 480000.00, 1120000.00, 5, '2026-10-28', 'Hình thức: Tự lái | [VNPay Test] Đã thanh toán đặt', 10.00, 160000.00),
(1045, 1020, '2026-11-03', 390000.00, 910000.00, 5, '2026-11-03', 'Hình thức: Có tài xế (Phụ phí tài xế: 500,000đ) | [VNPay Test] Đã thanh toán đặt cọc 30% thành công | [Check-in 03/11/2026 00:20]: ODO 25,000 km, Nhiên liệu 85% | [Check-out 03/11/2026 00:22]: ODO 25,400 km, Nhiên liệu 50% (Đã đi 400 km, vượt 50 km — Phụ phí: 150,000đ)', 10.00, 130000.00),
(1046, 1020, '2026-11-03', 1600000.00, 0.00, 5, '2026-11-03', '[T?o ??n 03/11/2026 01:02] | [Thanh to?n 03/11/2026 01:04] | H?nh th?c: T? l?i | [G?i thanh to?n: Tr?n g?i 100% qua VNPay] | [VNPay Test] ?? thanh to?n tr?n g?i 100% qua VNPay th?nh c?ng | [Check-in 03/11/2026 01:07]: ODO 25,000 km, Nhi?n li?u 85% | [Check-out 03/11/2026 02:16]: ODO 25,180 km, Nhiên liệu 85% (Đã đi 180 km, trong hạn mức 700 km)', 10.00, 160000.00),
(1047, 1020, '2026-11-04', 390000.00, 910000.00, 5, '2026-11-04', '[Tạo đơn 04/11/2026 23:27] | Hình thức: Tự lái | [Gói thanh toán: Đặt cọc 30% qua VNPay] | [Thanh toán 04/11/2026 23:27] | [VNPay Test] Đã thanh toán đặt cọc 30% qua VNPay thành công | [Check-in 04/11/2026 23:31]: ODO 25,000 km, Nhiên liệu 85% | [Check-out 04/11/2026 23:33]: ODO 25,780 km, Nhiên liệu 75% (Đã đi 780 km, vượt 80 km — Phụ phí: 240,000đ)', 10.00, 130000.00),
(1048, 1020, '2026-11-05', 390000.00, 910000.00, 5, '2026-11-05', '[Tạo đơn 05/11/2026 23:26] | Hình thức: Tự lái | [Gói thanh toán: Đặt cọc 30% qua VNPay] | [Thanh toán 05/11/2026 23:26] | [VNPay Test] Đã thanh toán đặt cọc 30% qua VNPay thành công | [Check-in 05/11/2026 23:29]: ODO 25,000 km, Nhiên liệu 85% | [Check-out 05/11/2026 23:31]: ODO 25,180 km, Nhiên liệu 85% (Đã đi 180 km, trong hạn mức 700 km)', 10.00, 130000.00),
(1049, 1020, '2026-11-08', 455000.00, 845000.00, 2, NULL, '[Tạo đơn 08/11/2026 21:42] | Hình thức: Tự lái | [Gói thanh toán: Đặt cọc 35% qua VNPay] | [Thanh toán 08/11/2026 21:42] | [VNPay Test] Đã thanh toán đặt cọc 30% qua VNPay thành công | [Chủ xe hủy đơn lúc 15/11/2026 20:17: Theo thỏa thuận giữa hai bên | Đã hoàn 455,000 đ tiền cọc vào Ví ReCar của khách]', 20.00, 260000.00),
(1050, 1020, '2026-11-15', 455000.00, 845000.00, 2, NULL, '[T?o n 15/11/2026 20:20] | Hình thức: T? li | [Gói thanh toán: Đặt cọc 35% qua Ví ReCar] | [Thanh toán 15/11/2026 20:20] | Đã thanh toán đặt cọc 35% (455,000 đ) bằng Số dư Ví ReCar | [Chủ xe hủy đơn lúc 15/11/2026 20:20: Theo thỏa thuận giữa hai bên | Đã hoàn 455,000 đ tiền cọc vào Ví ReCar của khách]', 20.00, 260000.00),
(1051, 1022, '2026-11-15', 595000.00, 1105000.00, 1, NULL, '[T?o n 15/11/2026 21:53] | Hình thức: T? li | [Gói thanh toán: Đặt cọc 35% qua VNPay]', 20.00, 340000.00),
(1052, 1022, '2026-11-15', 1680000.00, 3120000.00, 2, NULL, '[T?o n 15/11/2026 22:39] | Hình thức: T? li | [Gói thanh toán: Đặt cọc 35% qua ZaloPay] | [Thanh toán 15/11/2026 22:42] | Đã thanh toán cọc qua ZaloPay Sandbox (Mã GD: 261115_1052_833) | [Khách hủy đơn lúc 15/11/2026 22:42 | Hoàn 1,680,000 đ tiền cọc vào Ví ReCar]', 20.00, 960000.00);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `carreviews`
--

CREATE TABLE `carreviews` (
  `CarReviewID` int(11) NOT NULL,
  `Name` varchar(50) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  `Detail` varchar(200) DEFAULT NULL,
  `CarID` int(11) DEFAULT NULL,
  `Star` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT NULL,
  `OrderId` int(11) DEFAULT NULL,
  `Reply` longtext DEFAULT NULL,
  `ReplyDate` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `carreviews`
--

INSERT INTO `carreviews` (`CarReviewID`, `Name`, `Phone`, `Email`, `CreatedDate`, `Detail`, `CarID`, `Star`, `IsActive`, `OrderId`, `Reply`, `ReplyDate`) VALUES
(1, 'NQuân', '1234', 'Q@gmail.com', '2024-12-22 00:00:00', 'sản phẩm tốt', 1, 5, 1, NULL, NULL, NULL),
(2, 'Nthanh', '123', 'thnh@gmai.com', '2023-12-22 00:00:00', 'xe thơm', 1, 3, 1, NULL, NULL, NULL),
(3, 'quan', NULL, NULL, '2024-12-02 16:09:09', '123', 1, NULL, NULL, NULL, NULL, NULL),
(26, 'Trần Kenn', '0987654321', 'trankenn04@gmail.com', '2026-11-05 00:53:31', 'xe thơm', 1, 5, 1, 1047, 'cảm ơn', '2026-11-12 22:00:20');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `cars`
--

CREATE TABLE `cars` (
  `CarID` int(11) NOT NULL,
  `CarName` varchar(100) DEFAULT NULL,
  `Seat` int(11) DEFAULT NULL,
  `LicensePlate` varchar(20) DEFAULT NULL,
  `Price` decimal(18,2) DEFAULT NULL,
  `Color` varchar(50) DEFAULT NULL,
  `Model` int(11) DEFAULT NULL,
  `Rate` double DEFAULT NULL,
  `CarBrand` varchar(50) DEFAULT NULL,
  `Image` longtext DEFAULT NULL,
  `SalePrice` decimal(18,2) DEFAULT NULL,
  `Alias` varchar(50) DEFAULT NULL,
  `IsSale` tinyint(1) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `Details` longtext DEFAULT NULL,
  `Descriptions` longtext DEFAULT NULL,
  `TypeID` int(11) DEFAULT NULL,
  `OwnerID` int(11) DEFAULT NULL,
  `Transmission` varchar(50) DEFAULT 'T??? ?????ng',
  `Fuel` varchar(50) DEFAULT 'X??ng',
  `Location` varchar(150) DEFAULT 'H?? N???i',
  `Trips` int(11) DEFAULT 50,
  `Badge` varchar(50) DEFAULT NULL,
  `CarStatus` varchar(50) DEFAULT 'Ready',
  `RegistrationImage` varchar(500) DEFAULT NULL,
  `InspectionImage` varchar(500) DEFAULT NULL,
  `InsuranceImage` varchar(500) DEFAULT NULL,
  `Latitude` double DEFAULT NULL,
  `Longitude` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `cars`
--

INSERT INTO `cars` (`CarID`, `CarName`, `Seat`, `LicensePlate`, `Price`, `Color`, `Model`, `Rate`, `CarBrand`, `Image`, `SalePrice`, `Alias`, `IsSale`, `IsActive`, `Details`, `Descriptions`, `TypeID`, `OwnerID`, `Transmission`, `Fuel`, `Location`, `Trips`, `Badge`, `CarStatus`, `RegistrationImage`, `InspectionImage`, `InsuranceImage`, `Latitude`, `Longitude`) VALUES
(1, 'VinFast VF 5 Plus', 5, '30A-952.18', 650000.00, 'Đỏ nóc trắng', 2024, 4.3, 'VinFast', '/images/cars/car-mini.jpg', NULL, 'vinfast-vf-5-plus', 0, 1, 'Tự động · Điện · Cầu Giấy, Hà Nội · 86 đánh giá', 'Xe điện', 1, 1022, 'Tự động', 'Điện', 'Cầu Giấy, Hà Nội', 88, 'Xe điện', 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL),
(2, 'Toyota Vios G 2023', 5, '51K-834.12', 850000.00, 'Trắng ngọc trai', 2023, 4.8, 'Toyota', '/images/cars/car-sedan.jpg', NULL, 'toyota-vios-g-2023', 0, 0, 'Tự động · Xăng · Quận 7, TP. Hồ Chí Minh · 124 đánh giá', 'Nổi bật', 2, 1023, 'Tự động', 'Xăng', 'Quận 7, TP. Hồ Chí Minh', 124, 'Nổi bật', 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL),
(3, 'Hyundai Santa Fe 2024', 7, '43A-688.99', 1850000.00, 'Xanh đen', 2024, 4.7, 'Hyundai', '/images/cars/car-suv.jpg', 1650000.00, 'hyundai-santa-fe-2024', 1, 1, 'Tự động · Dầu · Hải Châu, Đà Nẵng · 58 đánh giá', 'Ưu đãi', 3, 1024, 'Tự động', 'Dầu', 'Hải Châu, Đà Nẵng', 58, 'Ưu đãi', 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL),
(4, 'Mazda 3 Luxury', 5, '30H-456.78', 950000.00, 'Trắng', 2023, 4.8, 'Mazda', '/images/cars/car-sedan.jpg', NULL, 'mazda-3-luxury', 0, 1, 'Tự động · Xăng · Đống Đa, Hà Nội · 72 đánh giá', 'Tiêu chuẩn', 2, 1025, 'Tự động', 'Xăng', 'Đống Đa, Hà Nội', 72, NULL, 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL),
(5, 'Kia Carnival Signature', 7, '51L-999.88', 2400000.00, 'Xanh đen', 2024, 4.9, 'Kia', '/images/cars/car-suv.jpg', NULL, 'kia-carnival-signature', 0, 1, 'Tự động · Dầu · Quận 1, TP. Hồ Chí Minh · 41 đánh giá', 'Nổi bật', 4, 1026, 'Tự động', 'Dầu', 'Quận 1, TP. Hồ Chí Minh', 41, 'Nổi bật', 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL),
(6, 'Ford Ranger Wildtrak', 5, '49C-333.21', 1400000.00, 'Cam Cyber', 2023, 4.6, 'Ford', '/images/cars/car-suv.jpg', NULL, 'ford-ranger-wildtrak', 0, 1, 'Tự động · Dầu · Phường 1, Đà Lạt · 33 đánh giá', 'Tiêu chuẩn', 5, 1024, 'Tự động', 'Dầu', 'Phường 1, Đà Lạt', 33, NULL, 'Bảo trì', NULL, NULL, NULL, NULL, NULL),
(7, 'Honda City RS', 5, '79A-567.89', 950000.00, 'Trắng', 2023, 4.7, 'Honda', '/images/cars/car-sedan.jpg', 800000.00, 'honda-city-rs', 1, 1, '[Tiện nghi]: Cửa sổ trời Panorama, Thu phí tự động ETC, Sạc không dây Qi\n[Quy định quãng đường]: Tối đa 350 km/ngày (Phụ phí vượt: 3,000đ/km)\n[Giao nhận xe]: Có hỗ trợ giao xe tận nơi (Phí: 100,000đ)\n[Mô tả bổ sung]: [Tiện nghi]: Cửa sổ trời Panorama, Thu phí tự động ETC, Sạc không dây Qi\r\n[Quy định quãng đường]: Tối đa 350 km/ngày (Phụ phí vượt: 4,000đ/km)\r\n[Giao nhận xe]: Có hỗ trợ giao xe tận nơi (Phí: 100,000đ)\r\n[Mô tả bổ sung]: Tự động · Xăng · Lộc Thọ, Nha Trang · 65 đánh giá', 'Cửa sổ trời Panorama, Thu phí tự động ETC, Sạc không dây Qi', 2, 1022, 'Tự động', 'Xăng', 'Lộc Thọ, Nha Trang', 68, 'Ưu đãi', 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL),
(8, 'Toyota Fortuner Legender', 7, '30F-777.66', 1500000.00, 'Bạc', 2023, 4.5, 'Toyota', '/images/cars/car-suv.jpg', NULL, 'toyota-fortuner-legender', 0, 1, 'Số sàn · Dầu · Tây Hồ, Hà Nội · 27 đánh giá', 'Tiêu chuẩn', 3, 1023, 'Số sàn', 'Dầu', 'Tây Hồ, Hà Nội', 27, NULL, 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL),
(20, 'VinFast VF 8 Plus 2024', 5, '37A-888.66', 1200000.00, 'Trắng', 2024, 4.95, 'VinFast', '/images/cars/car-suv.jpg', NULL, 'vinfast-vf-8-plus-vinh', 0, 1, 'Tự động • Điện • Quảng trường Hồ Chí Minh, TP. Vinh • 42 chuyến', 'Xe điện cao cấp SUV hạng D sang trọng, vận hành êm ái, đầy đủ tiện nghi, hỗ trợ sạc nhanh tại các trạm VinFast TP. Vinh.', 3, 1022, 'Tự động', 'Điện', 'Quảng trường Hồ Chí Minh, TP. Vinh, Nghệ An', 42, 'Xe điện cao cấp', 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL),
(21, 'Hyundai Tucson 1.6 Turbo 2023', 5, '37K-123.45', 1100000.00, 'Đen Phantom', 2023, 4.88, 'Hyundai', '/images/cars/car-suv.jpg', NULL, 'hyundai-tucson-turbo-vinh', 0, 1, 'Tự động • Xăng • Lê Mao, TP. Vinh • 65 chuyến', 'Crossover phong cách thể thao, máy bốc, nội thất rộng rãi, cửa sổ trời toàn cảnh, thích hợp đi gia đình và công tác tại Nghệ An.', 3, 1023, 'Tự động', 'Xăng', 'Lê Mao, TP. Vinh, Nghệ An', 65, 'Ưa chuộng', 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL),
(22, 'Toyota Corolla Cross 1.8V 2024', 5, '37A-996.88', 1050000.00, 'Đỏ Ruby', 2024, 4.92, 'Toyota', '/images/cars/car-suv.jpg', 950000.00, 'toyota-corolla-cross-vinh', 1, 1, 'Tự động • Xăng • Trường Thi, TP. Vinh • 78 chuyến', 'Xe gầm cao đô thị nhập khẩu, gói công nghệ an toàn Toyota Safety Sense cao cấp, tiết kiệm nhiên liệu tối ưu.', 3, 1024, 'Tự động', 'Xăng', 'Trường Thi, TP. Vinh, Nghệ An', 78, 'Tiết kiệm xăng', 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL),
(23, 'Mazda CX-5 2.0 Premium 2023', 5, '37K-567.89', 1000000.00, 'Trắng ngọc trai', 2023, 4.9, 'Mazda', '/images/cars/car-suv.jpg', NULL, 'mazda-cx5-premium-vinh', 0, 1, 'Tự động • Xăng • Hưng Dũng, TP. Vinh • 52 chuyến', 'Mẫu SUV ăn khách nhất phân khúc, loa Bose âm thanh vòm, camera 360, giao xe tận nơi miễn phí trong bán kính 5km trung tâm Vinh.', 3, 1025, 'Tự động', 'Xăng', 'Hưng Dũng, TP. Vinh, Nghệ An', 52, 'Giao xe tận nơi', 'Sẵn sàng', NULL, NULL, NULL, NULL, NULL);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `cartypes`
--

CREATE TABLE `cartypes` (
  `TypeID` int(11) NOT NULL,
  `CarTypeName` varchar(100) DEFAULT NULL,
  `Seats` int(11) DEFAULT NULL,
  `Manufacturer` varchar(100) DEFAULT NULL,
  `Image` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `cartypes`
--

INSERT INTO `cartypes` (`TypeID`, `CarTypeName`, `Seats`, `Manufacturer`, `Image`) VALUES
(1, 'Xe mini đô thị', 4, 'VinFast, Kia', '/images/cars/car-mini.jpg'),
(2, 'Sedan 5 chỗ', 5, 'Toyota, Mazda, Honda', '/images/cars/car-sedan.jpg'),
(3, 'Crossover/SUV 7 chỗ', 7, 'Hyundai, Toyota, Ford', '/images/cars/car-suv.jpg'),
(4, 'MPV đa dụng', 7, 'Kia, Mitsubishi', '/images/cars/car-suv.jpg'),
(5, 'Xe bán tải', 5, 'Ford, Toyota', '/images/cars/car-suv.jpg');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `car_amenities`
--

CREATE TABLE `car_amenities` (
  `CarId` int(11) NOT NULL,
  `AmenityId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `car_amenities`
--

INSERT INTO `car_amenities` (`CarId`, `AmenityId`) VALUES
(7, 6),
(7, 7),
(7, 10);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `car_brands`
--

CREATE TABLE `car_brands` (
  `BrandId` int(11) NOT NULL,
  `BrandName` varchar(100) NOT NULL,
  `Country` varchar(100) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT 0,
  `IsActive` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `car_brands`
--

INSERT INTO `car_brands` (`BrandId`, `BrandName`, `Country`, `DisplayOrder`, `IsActive`) VALUES
(1, 'VinFast', 'Việt Nam', 1, 1),
(2, 'Toyota', 'Nhật Bản', 2, 1),
(3, 'Hyundai', 'Hàn Quốc', 3, 1),
(4, 'Kia', 'Hàn Quốc', 4, 1),
(5, 'Mazda', 'Nhật Bản', 5, 1),
(6, 'Honda', 'Nhật Bản', 6, 1),
(7, 'Ford', 'Mỹ', 7, 1),
(8, 'Mitsubishi', 'Nhật Bản', 8, 1),
(9, 'Mercedes-Benz', 'Đức', 9, 1),
(10, 'BMW', 'Đức', 10, 1),
(11, 'Lexus', 'Nhật Bản', 11, 1),
(12, 'Audi', 'Đức', 12, 1),
(13, 'Nissan', 'Nhật Bản', 13, 1),
(14, 'MG', 'Anh / Trung Quốc', 14, 1),
(15, 'Subaru', 'Nhật Bản', 15, 1),
(16, 'Peugeot', 'Pháp', 16, 1),
(17, 'Suzuki', 'Nhật Bản', 17, 1),
(18, 'Porsche', 'Đức', 18, 1),
(19, 'Land Rover', 'Anh', 19, 1),
(20, 'Volvo', 'Thụy Điển', 20, 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `car_calendar_blocks`
--

CREATE TABLE `car_calendar_blocks` (
  `BlockId` int(11) NOT NULL,
  `CarId` int(11) NOT NULL,
  `StartTime` datetime NOT NULL,
  `EndTime` datetime NOT NULL,
  `Kind` varchar(30) NOT NULL DEFAULT 'personal',
  `Note` varchar(500) DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `car_calendar_blocks`
--

INSERT INTO `car_calendar_blocks` (`BlockId`, `CarId`, `StartTime`, `EndTime`, `Kind`, `Note`, `CreatedAt`) VALUES
(3, 7, '2026-11-08 08:00:00', '2026-11-09 20:00:00', 'maintenance', 'Bảo dưỡng định kỳ', '2026-11-06 01:52:50'),
(4, 26, '2026-11-10 08:00:00', '2026-11-12 20:00:00', 'maintenance', 'Bảo dưỡng định kỳ', '2026-11-10 02:02:36');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `chatmessages`
--

CREATE TABLE `chatmessages` (
  `MessageId` int(11) NOT NULL,
  `CarId` int(11) DEFAULT NULL,
  `SenderId` int(11) DEFAULT NULL,
  `SenderName` varchar(100) NOT NULL DEFAULT 'Khách hàng',
  `SenderRole` varchar(50) NOT NULL DEFAULT 'Customer',
  `ReceiverId` int(11) DEFAULT NULL,
  `ReceiverName` varchar(100) DEFAULT NULL,
  `MessageText` text NOT NULL,
  `SentAt` datetime NOT NULL DEFAULT current_timestamp(),
  `IsRead` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `chatmessages`
--

INSERT INTO `chatmessages` (`MessageId`, `CarId`, `SenderId`, `SenderName`, `SenderRole`, `ReceiverId`, `ReceiverName`, `MessageText`, `SentAt`, `IsRead`) VALUES
(1, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'xin chào', '2026-11-04 22:39:12', 1),
(2, 1, 1022, 'Trần Quốc Bảo', 'CarOwner', 1020, 'Trần Kenn', 'hi', '2026-11-04 22:39:29', 1),
(3, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'skipidi', '2026-11-04 22:39:38', 1),
(4, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'grrr', '2026-11-04 22:39:40', 1),
(5, 1, 1022, 'Trần Quốc Bảo', 'CarOwner', 1020, 'Trần Kenn', 'grrrr', '2026-11-04 22:39:50', 1),
(6, 1, 1022, 'Trần Quốc Bảo', 'CarOwner', 1020, 'Trần Kenn', 'aaa', '2026-11-04 22:42:50', 1),
(7, 1, 1022, 'Trần Quốc Bảo', 'CarOwner', 1020, 'Trần Kenn', 'aaa', '2026-11-04 22:42:56', 1),
(8, 1, 1022, 'Trần Quốc Bảo', 'CarOwner', 1020, 'Trần Kenn', 'aaa', '2026-11-04 22:43:31', 1),
(9, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'aaaa', '2026-11-04 23:07:07', 1),
(10, 1, 1022, 'Trần Quốc Bảo', 'CarOwner', 1020, 'Trần Kenn', 'alol', '2026-11-04 23:07:26', 1),
(11, 1, 1022, 'Trần Quốc Bảo', 'CarOwner', 1020, 'Trần Kenn', 'aaa', '2026-11-04 23:07:33', 1),
(12, 1, 1022, 'Trần Quốc Bảo', 'CarOwner', 1020, 'Trần Kenn', 'aaa', '2026-11-04 23:07:50', 1),
(13, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'hi', '2026-11-04 23:28:59', 1),
(14, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'hi\'', '2026-11-05 00:54:20', 1),
(15, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'a', '2026-11-05 00:59:01', 1),
(16, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'a', '2026-11-05 01:29:57', 1),
(17, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'a', '2026-11-05 21:42:40', 1),
(18, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'hello', '2026-11-05 21:43:04', 1),
(19, 1, 1022, 'Trần Quốc Bảo', 'CarOwner', 1020, 'Trần Kenn', 'aaa', '2026-11-05 21:44:03', 1),
(20, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'xin chao', '2026-11-05 22:29:57', 1),
(21, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'hi', '2026-11-05 23:21:48', 0),
(22, 1, 1022, 'Trần Quốc Bảo', 'CarOwner', 1020, 'Trần Kenn', 'finally i can fix that', '2026-11-05 23:21:59', 1),
(23, 26, 1027, 'Administrator', 'Admin', 1022, 'Trần Quốc Bảo', 'xin chao', '2026-11-10 01:00:02', 1),
(24, 26, 1022, 'Trần Quốc Bảo', 'CarOwner', 1027, 'Administrator', 'test', '2026-11-10 01:00:12', 0),
(25, 1, 1020, 'Trần Kenn', 'Customer', 1022, 'Trần Quốc Bảo', 'a', '2026-11-15 23:44:27', 0);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `contact`
--

CREATE TABLE `contact` (
  `ContactId` int(11) NOT NULL,
  `Name` varchar(150) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(150) DEFAULT NULL,
  `Message` longtext DEFAULT NULL,
  `IsRead` int(11) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  `CreatedBy` varchar(150) DEFAULT NULL,
  `ModifiedDate` datetime DEFAULT NULL,
  `ModifiedBy` varchar(150) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `customers`
--

CREATE TABLE `customers` (
  `CustomerID` int(11) NOT NULL,
  `FullName` varchar(50) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `Username` varchar(50) DEFAULT NULL,
  `Password` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `RoleId` int(11) NOT NULL DEFAULT 2,
  `IsActive` tinyint(1) NOT NULL DEFAULT 1,
  `LastLogin` varchar(50) DEFAULT NULL,
  `DateofBirth` datetime DEFAULT NULL,
  `Address` longtext DEFAULT NULL,
  `LicenseNumber` varchar(50) DEFAULT NULL,
  `LicenseImage` longtext DEFAULT NULL,
  `IDCard` varchar(20) DEFAULT NULL,
  `PhoneNumber` varchar(20) DEFAULT NULL,
  `AccountNumber` varchar(50) DEFAULT NULL,
  `Bank` varchar(50) DEFAULT NULL,
  `CompanyName` varchar(100) DEFAULT NULL,
  `Avartar` varchar(50) DEFAULT NULL,
  `Balance` decimal(18,2) NOT NULL DEFAULT 0.00
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `customers`
--

INSERT INTO `customers` (`CustomerID`, `FullName`, `Name`, `Username`, `Password`, `Email`, `RoleId`, `IsActive`, `LastLogin`, `DateofBirth`, `Address`, `LicenseNumber`, `LicenseImage`, `IDCard`, `PhoneNumber`, `AccountNumber`, `Bank`, `CompanyName`, `Avartar`, `Balance`) VALUES
(1, 'bình', 'binh', 'binh', 'd411cf68bd7f71d13fb053154f455651', 'binh@gmail.com', 3, 1, '2025-12-21', '2025-12-21 00:00:00', '2342', '0123456789101', 'z7312564865026_72b1ba7dd1662b7e127ff0b18e5f4fc5_20251218235607389.jpg', NULL, '4234234', NULL, NULL, NULL, NULL, 0.00),
(2, 'thuy', 'thuy', 'quan1', '123', 'quan@gmail.com', 2, 1, NULL, NULL, 'nghệ an', NULL, NULL, NULL, '12334', NULL, NULL, NULL, NULL, 0.00),
(3, 'Đậu Công Quân', 'dauquan', 'dauquan', 'd0afd43f169f68af6d56cf454d74b167', 'dauquan@gmail.com', 2, 1, NULL, '2004-04-03 00:00:00', 'nghệ an', '1123456789', 'A2_20251218114951560.jpg', NULL, '12334', NULL, NULL, NULL, NULL, 0.00),
(4, 'thuy', 'thuy', 'thuy', 'd0afd43f169f68af6d56cf454d74b167', 'thuy@gmail.com', 2, 1, NULL, NULL, 'nghệ an', NULL, NULL, NULL, '12334', NULL, NULL, NULL, NULL, 0.00),
(5, 'Đậu Công Quân', 'quan', NULL, 'd0afd43f169f68af6d56cf454d74b167', 'quan123@gmail.com', 2, 1, NULL, '2025-12-14 00:00:00', 'nghệ an, diễn châu, quyết thành1234567', '121212321343', 'delete_20251221232537688.png', NULL, '12334', NULL, NULL, NULL, NULL, 0.00),
(6, 'Nguyễn Hồng Quân', 'Nguyễn Quân 37dc', 'Q', 'd0afd43f169f68af6d56cf454d74b167', 'Q@gmail.com', 2, 1, NULL, '2004-01-12 00:00:00', 'Xóm Quyết Thành, Xã Diễn Bich, Huyển Diễn châu, tỉnh Nghệ An', '123412341231', 'adc_20250108223225438.jpg', NULL, '0353675457', NULL, NULL, NULL, '0.00032029000144429.jpg', 0.00),
(7, 'Hoàng Thanh Huyền cute', 'Hoàng Thanh Huyền cute', 'Huyen', 'ca3ea7a65775384c0aa2249769adb543', 'Huyen@gmail.com', 2, 1, NULL, NULL, 'Thị xã Hoàng mai, Nghệ an', NULL, NULL, NULL, '0123345678', NULL, NULL, NULL, NULL, 0.00),
(9, 'Doãn thị thanh thuỷ', 'Doãn thị thanh thuỷ', 'T', 'd0afd43f169f68af6d56cf454d74b167', 'T@gmail.com', 2, 1, NULL, NULL, 'Nghi lộc nghệ an', NULL, NULL, NULL, '131243243', NULL, NULL, NULL, NULL, 0.00),
(10, 'Minh Anh', 'Minh Anh', 'anh', 'd0afd43f169f68af6d56cf454d74b167', 'anh@gmail.com', 2, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00),
(1017, 'Bình Hidro', 'binhtest1', NULL, 'd411cf68bd7f71d13fb053154f455651', 'binhtest1@gmail.com', 2, 1, NULL, '2025-12-22 00:00:00', 'Hoàng Mai, Hà Nội', '11111111111', 'flutter-app-development-roadmap-by-tarikul_20251222140115648.png', NULL, '0339772881', NULL, NULL, NULL, NULL, 0.00),
(1018, 'Doan thanh thuy', 'thủy', NULL, '72631d8a190e188b4e116163d0c04ab5', 'thuy123@gmail.com', 2, 1, NULL, '2004-11-08 00:00:00', NULL, '123456789012', 'get_20251222213520492.png', NULL, NULL, NULL, NULL, NULL, NULL, 0.00),
(1020, 'Trần Kenn', 'Trần Kenn', 'trankenn04', 'd0afd43f169f68af6d56cf454d74b167', 'trankenn04@gmail.com', 2, 1, '2026-11-15 23:43:12', '2004-10-10 00:00:00', 'Hà Nội', '717254718131', 'images (1)_20261016002810148.jpg', NULL, '0987654321', NULL, NULL, NULL, '/files/avatars/avatar_20261115201757902_4eed29.jpg', 455000.00),
(1021, 'Trần Ngọc Hải', 'trankenn', 'trankenn04hoyo', 'd0afd43f169f68af6d56cf454d74b167', 'trankenn04hoyo@gmail.com', 3, 1, '2026-11-15 18:17:24', NULL, 'Nghệ An', '717254718131', 'vangla_20261025223623081.jpg', '080208028372', '0866459082', '225510108888', 'MB Bank (MB)', 'TRAN NGOC HAI', NULL, 0.00),
(1022, 'Trần Quốc Bảo', 'Trần Quốc Bảo', 'tranbaocar', 'd0afd43f169f68af6d56cf454d74b167', 'tranbao.carowner@gmail.com', 3, 1, '2026-11-16 20:37:20', NULL, 'Cầu Giấy, Hà Nội', '079098001234', 'giay-phep-lai-xe-hang-a1.jpg', '079098001234', '0912.345.678', '1029384756', 'Vietcombank', NULL, '/files/avatars/avatar_20261113023002821_2d348b.jpg', 1680000.00),
(1023, 'Nguyễn Minh Anh', 'Nguyễn Minh Anh', 'minhanh_auto', 'd0afd43f169f68af6d56cf454d74b167', 'minhanh.carrental@gmail.com', 3, 1, '2025-12-22', NULL, 'Quận 7, TP.HCM', '079098005678', 'A2_20251218114951560.jpg', '079098005678', '0988.223.344', '190367890123', 'Techcombank', NULL, NULL, 0.00),
(1024, 'Phạm Hoàng Nam', 'Phạm Hoàng Nam', 'hoangnam_suv', 'd0afd43f169f68af6d56cf454d74b167', 'hoangnam.suv@gmail.com', 3, 1, NULL, NULL, 'Hải Châu, Đà Nẵng', '079098009999', 'vangla_20261025223623081.jpg', '079098009999', '0977.889.900', '0987654321', 'MB Bank', NULL, NULL, 0.00),
(1025, 'Lê Thu Hà', 'Lê Thu Hà', 'thuha_luxury', 'd0afd43f169f68af6d56cf454d74b167', 'thuha.luxurycars@gmail.com', 3, 1, NULL, NULL, 'Đống Đa, Hà Nội', NULL, NULL, NULL, '0339.772.881', NULL, NULL, NULL, NULL, 0.00),
(1026, 'Vũ Đình Dũng', 'Vũ Đình Dũng', 'dinhdung_rent', 'd0afd43f169f68af6d56cf454d74b167', 'dinhdung.rentcar@gmail.com', 3, 1, NULL, NULL, 'Quận 1, TP.HCM', NULL, NULL, NULL, '0905.112.233', NULL, NULL, NULL, NULL, 0.00),
(1027, 'Administrator', 'Administrator', 'Admin', '93893acf0c705336677073e806d09e5c', 'admin@system.local', 1, 1, '2026-11-16 20:44:02', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '/files/avatars/avatar_20261113022900792_33cd9c.jpg', 0.00);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `locations`
--

CREATE TABLE `locations` (
  `LocationID` int(11) NOT NULL,
  `CityName` varchar(100) NOT NULL,
  `Region` varchar(50) DEFAULT 'Miền Bắc',
  `IsPopular` tinyint(1) DEFAULT 1,
  `DisplayOrder` int(11) DEFAULT 0,
  `IsActive` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `locations`
--

INSERT INTO `locations` (`LocationID`, `CityName`, `Region`, `IsPopular`, `DisplayOrder`, `IsActive`) VALUES
(1, 'Hà Nội', 'Miền Bắc', 1, 1, 1),
(2, 'TP. Hồ Chí Minh', 'Miền Nam', 1, 2, 1),
(3, 'TP. Vinh (Nghệ An)', 'Miền Trung', 1, 3, 1),
(4, 'Đà Nẵng', 'Miền Trung', 1, 4, 1),
(5, 'Hải Phòng', 'Miền Bắc', 1, 5, 1),
(6, 'Cần Thơ', 'Miền Nam', 1, 6, 1),
(7, 'Nha Trang (Khánh Hòa)', 'Miền Trung', 1, 7, 1),
(8, 'Đà Lạt (Lâm Đồng)', 'Miền Trung', 1, 8, 1),
(9, 'Vũng Tàu (TP. Hồ Chí Minh)', 'Miền Nam', 1, 9, 1),
(10, 'Hạ Long (Quảng Ninh)', 'Miền Bắc', 1, 10, 1),
(11, 'Thành phố Huế', 'Miền Trung', 0, 11, 1),
(12, 'Quy Nhơn (Gia Lai)', 'Miền Trung', 0, 12, 1),
(13, 'Phan Thiết (Lâm Đồng)', 'Miền Nam', 0, 13, 1),
(14, 'Thanh Hóa', 'Miền Bắc', 0, 14, 1),
(15, 'Bắc Ninh', 'Miền Bắc', 0, 15, 1),
(16, 'Buôn Ma Thuột (Đắk Lắk)', 'Miền Trung', 0, 16, 1),
(17, 'Phú Quốc (An Giang)', 'Miền Nam', 1, 17, 1),
(19, 'Phú Thọ', 'Miền Bắc', 0, 18, 0),
(20, 'Thái Nguyên', 'Miền Bắc', 0, 19, 0),
(21, 'Hưng Yên', 'Miền Bắc', 0, 20, 0),
(22, 'Ninh Bình', 'Miền Bắc', 1, 21, 1),
(23, 'Lào Cai', 'Miền Bắc', 1, 22, 1),
(24, 'Tuyên Quang', 'Miền Bắc', 0, 23, 0),
(25, 'Cao Bằng', 'Miền Bắc', 0, 24, 0),
(26, 'Lạng Sơn', 'Miền Bắc', 0, 25, 0),
(27, 'Điện Biên', 'Miền Bắc', 0, 26, 0),
(28, 'Lai Châu', 'Miền Bắc', 0, 27, 0),
(29, 'Sơn La', 'Miền Bắc', 0, 28, 0),
(30, 'Hà Tĩnh', 'Miền Trung', 0, 29, 0),
(31, 'Quảng Trị', 'Miền Trung', 0, 30, 0),
(32, 'Quảng Ngãi', 'Miền Trung', 0, 31, 0),
(33, 'Đồng Nai', 'Miền Nam', 0, 32, 0),
(34, 'Tây Ninh', 'Miền Nam', 0, 33, 0),
(35, 'Đồng Tháp', 'Miền Nam', 0, 34, 0),
(36, 'Vĩnh Long', 'Miền Nam', 0, 35, 0),
(37, 'Cà Mau', 'Miền Nam', 0, 36, 0);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `menu`
--

CREATE TABLE `menu` (
  `MenuID` int(11) NOT NULL,
  `Title` varchar(150) DEFAULT NULL,
  `Alias` varchar(150) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `Levels` int(11) DEFAULT NULL,
  `ParentId` int(11) DEFAULT NULL,
  `Position` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT NULL,
  `Icon` varchar(100) DEFAULT 'bi-link-45deg',
  `Subsystem` varchar(50) NOT NULL DEFAULT 'Customer',
  `Url` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `menu`
--

INSERT INTO `menu` (`MenuID`, `Title`, `Alias`, `Description`, `Levels`, `ParentId`, `Position`, `IsActive`, `Icon`, `Subsystem`, `Url`) VALUES
(1, 'TỔNG QUAN', NULL, 'Nhóm Tổng quan', NULL, NULL, 1, 1, 'bi-grid-fill', 'Admin', '#'),
(2, 'Tổng quan', NULL, 'Trang tổng quan hệ thống Admin', NULL, 1, 1, 1, 'bi-grid-fill', 'Admin', '/Admin/Home/Index'),
(3, 'XE & ĐƠN HÀNG', NULL, 'Nhóm Xe & Đơn hàng', NULL, NULL, 2, 1, 'bi-car-front', 'Admin', '#'),
(4, 'Phê duyệt xe', NULL, 'Phê duyệt xe mới đăng ký', NULL, 3, 1, 1, 'bi-shield-check', 'Admin', '/Admin/Cars/Pending'),
(5, 'Duyệt hồ sơ Chủ xe', NULL, 'Duyệt hồ sơ đối tác chủ xe', NULL, 3, 2, 1, 'bi-person-check-fill', 'Admin', '/Admin/CarOwners/Pending'),
(6, 'Quản lý đội xe', NULL, 'Quản lý toàn bộ danh sách xe', NULL, 3, 3, 1, 'bi-car-front-fill', 'Admin', '/Admin/Cars/Index'),
(7, 'Quản lý kiểu xe', NULL, 'Quản lý danh mục kiểu dáng xe', NULL, 3, 4, 1, 'bi-collection-fill', 'Admin', '/Admin/CarTypes/Index'),
(8, 'Đơn thuê xe', NULL, 'Quản lý hợp đồng & đơn thuê xe', NULL, 3, 5, 1, 'bi-clipboard-check-fill', 'Admin', '/Admin/Orders/Index'),
(9, 'Tài chính & đối soát', NULL, 'Tài chính hoa hồng & đối soát sàn', NULL, 3, 6, 1, 'bi-wallet2', 'Admin', '/Admin/Finance/Index'),
(10, 'NỘI DUNG & TƯƠNG TÁC', NULL, 'Nhóm Nội dung & Tương tác', NULL, NULL, 3, 1, 'bi-chat-square-text', 'Admin', '#'),
(11, 'Quản lý cẩm nang', NULL, 'Biên tập bài viết & tin tức', NULL, 10, 1, 1, 'bi-journal-bookmark', 'Admin', '/Admin/Blogs'),
(12, 'Quản lý Menu', 'quan-ly-menu', 'Cấu hình menu 3 phân hệ', 2, 10, 4, 1, 'bi-menu-button-wide-fill', 'Admin', '/Admin/Menus/Index'),
(13, 'Đánh giá & Bình luận', NULL, 'Kiểm duyệt đánh giá & bình luận', NULL, 10, 3, 1, 'bi-star-half', 'Admin', '/Admin/Reviews/Index'),
(14, 'HỆ THỐNG', NULL, 'Nhóm Cài đặt hệ thống', NULL, NULL, 4, 1, 'bi-gear-fill', 'Admin', '#'),
(15, 'Địa điểm nhận xe', NULL, 'Quản lý các điểm giao nhận xe', NULL, 14, 1, 1, 'bi-geo-alt-fill', 'Admin', '/Admin/Locations/Index'),
(16, 'Người dùng & đối tác', NULL, 'Quản lý tài khoản người dùng & đối tác', NULL, 14, 2, 1, 'bi-people-fill', 'Admin', '/Admin/UserManagement/Index'),
(17, 'Cấu hình hệ thống', NULL, 'Cấu hình tỷ lệ cọc, hoa hồng và hệ thống', NULL, 14, 3, 1, 'bi-gear-fill', 'Admin', '/Admin/Settings/Index'),
(20, 'QUẢN TRỊ PHƯƠNG TIỆN', NULL, 'Nhóm Quản trị phương tiện', NULL, NULL, 1, 1, 'bi-speedometer2', 'CarOwner', '#'),
(21, 'Tổng quan', NULL, 'Bảng điều khiển chủ xe', NULL, 20, 1, 1, 'bi-grid-fill', 'CarOwner', '/CarOwner/Home/Index'),
(22, 'Quản lý xe', NULL, 'Danh sách xe cho thuê', NULL, 20, 2, 1, 'bi-car-front-fill', 'CarOwner', '/CarOwner/Cars/Index'),
(23, 'Thêm xe', NULL, 'Đăng ký thêm xe mới', NULL, 20, 3, 1, 'bi-plus-circle-fill', 'CarOwner', '/CarOwner/Cars/Create'),
(24, 'Lịch xe', NULL, 'Lịch trình bận & bảo dưỡng xe', NULL, 20, 4, 1, 'bi-calendar3', 'CarOwner', '/CarOwner/Calendar/Index'),
(25, 'KINH DOANH & THU NHẬP', NULL, 'Nhóm Kinh doanh & Thu nhập', NULL, NULL, 2, 1, 'bi-cash-coin', 'CarOwner', '#'),
(26, 'Lịch trình & Đơn xe', NULL, 'Quản lý đơn thuê & lịch giao nhận xe', NULL, 25, 1, 1, 'bi-calendar2-range-fill', 'CarOwner', '/CarOwner/Orders/Index'),
(27, 'Phân tích hiệu suất', NULL, 'Thống kê doanh thu & hiệu suất hoạt động', NULL, 25, 2, 1, 'bi-bar-chart-line-fill', 'CarOwner', '/CarOwner/Analytics/Index'),
(28, 'Ví đối tác & Đối soát', NULL, 'Quản lý số dư, tiền cọc & rút tiền', NULL, 25, 3, 1, 'bi-wallet2', 'CarOwner', '/CarOwner/Wallet/Index'),
(29, 'Hồ sơ pháp lý & Ngân hàng', NULL, 'Hồ sơ pháp lý, CCCD/GPLX & tài khoản ngân hàng', NULL, 25, 4, 1, 'bi-file-earmark-text-fill', 'CarOwner', '/CarOwner/Register/Pending'),
(31, 'Trang chủ', NULL, 'Trang chủ ReCar', NULL, NULL, 1, 1, 'bi-house-door', 'Customer', '/'),
(32, 'Thuê xe', NULL, 'Danh mục tìm kiếm và thuê xe ô tô', NULL, NULL, 2, 1, 'bi-car-front', 'Customer', '/Product'),
(33, 'Cẩm nang', NULL, 'Cẩm nang kinh nghiệm lái xe & du lịch', NULL, NULL, 3, 1, 'bi-journal-bookmark', 'Customer', '/Blog'),
(34, 'Tin tức', 'tin-tuc', 'Tin tức & sự kiện ReCar', 1, 0, 4, 1, 'bi-newspaper', 'Customer', '/News'),
(35, 'Liên hệ', 'lien-he', 'Liên hệ hỗ trợ 24/7', 1, 0, 5, 1, 'bi-telephone', 'Customer', '/Contact'),
(312, 'Quản lý hãng xe', 'Admin/CarBrands', NULL, 2, 3, 5, 1, 'bi-patch-check', 'Admin', '/Admin/CarBrands'),
(313, 'Quản lý tin tức', 'Admin/News', NULL, 2, 10, 2, 1, 'bi-newspaper', 'Admin', '/Admin/News'),
(314, 'Liên hệ khách hàng', 'lien-he-khach-hang', 'Quản lý liên hệ từ khách hàng', 2, 10, 14, 1, 'bi bi-envelope-paper', 'Admin', '/Admin/Contacts/Index');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `news`
--

CREATE TABLE `news` (
  `NewsId` int(11) NOT NULL,
  `Title` varchar(250) DEFAULT NULL,
  `Alias` varchar(250) DEFAULT NULL,
  `CategoryId` int(11) DEFAULT NULL,
  `Description` varchar(4000) DEFAULT NULL,
  `Detail` longtext DEFAULT NULL,
  `Image` varchar(500) DEFAULT NULL,
  `SeoTitle` varchar(250) DEFAULT NULL,
  `SeoDescription` varchar(500) DEFAULT NULL,
  `SeoKeywords` varchar(250) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  `CreatedBy` varchar(150) DEFAULT NULL,
  `ModifiedDate` datetime DEFAULT NULL,
  `ModifiedBy` varchar(150) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `Views` int(11) NOT NULL DEFAULT 0,
  `IsFeatured` tinyint(1) NOT NULL DEFAULT 0,
  `Badge` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `news`
--

INSERT INTO `news` (`NewsId`, `Title`, `Alias`, `CategoryId`, `Description`, `Detail`, `Image`, `SeoTitle`, `SeoDescription`, `SeoKeywords`, `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`, `IsActive`, `Views`, `IsFeatured`, `Badge`) VALUES
(2, 'VINFAST THÔNG BÁO THAY ĐỔI TÊN GỌI PHIÊN BẢN CỦA MỘT SỐ DÒNG XE', 'vinfast-thong-bao-thay-doi-ten-goi-phien-ban-cua-mot-so-dong-xe', NULL, 'Nhằm phù hợp hơn với định hướng phát triển sản phẩm và giá trị mà từng dòng xe mang lại, VinFast trân trọng thông báo việc thay đổi tên gọi một số phiên bản xe ô tô điện. Đây không chỉ là sự thay đổi về tên gọi, mà còn là bước chuyển mình trong chiến lược, cam kết mang đến những giá trị vượt trội trên hành trình kiến tạo tương lai xanh bền vững của VinFast.', '<p><span style=\"color: rgb(60, 60, 60); font-family: Mulish, serif; font-style: italic; letter-spacing: -0.32px;\">Nhằm phù hợp hơn với định hướng phát triển sản phẩm và giá trị mà từng dòng xe mang lại, VinFast trân trọng thông báo việc thay đổi tên gọi một số phiên bản xe ô tô điện. Đây không chỉ là sự thay đổi về tên gọi, mà còn là bước chuyển mình trong chiến lược, cam kết mang đến những giá trị vượt trội trên hành trình kiến tạo tương lai xanh bền vững của VinFast.</span></p>', '/files/News/xedo.jpg', NULL, NULL, NULL, '2025-01-12 14:59:33', 'thanh', NULL, NULL, 1, 680, 0, NULL),
(1002, 'VINFAST THÔNG BÁO THAY ĐỔI TÊN GỌI PHIÊN BẢN CỦA MỘT SỐ DÒNG XE', 'vinfast-thong-bao-thay-doi-ten-goi-phien-ban-cua-mot-so-dong-xe', NULL, 'NNNhằm phù hợp hơn với định hướng phát triển sản phẩm và giá trị mà từng dòng xe mang lại, VinFast trân trọng thông báo việc thay đổi tên gọi một số phiên bản xe ô tô điện. Đây không chỉ là sự thay đổi về tên gọi, mà còn là bước chuyển mình trong chiến lược, cam kết mang đến những giá trị vượt trội trên hành trình kiến tạo tương lai xanh bền vững của VinFast', '<p><span style=\"color: rgb(60, 60, 60); font-family: Mulish, serif; font-style: italic; letter-spacing: -0.32px;\">Nhằm phù hợp hơn với định hướng phát triển sản phẩm và giá trị mà từng dòng xe mang lại, VinFast trân trọng thông báo việc thay đổi tên gọi một số phiên bản xe ô tô điện. Đây không chỉ là sự thay đổi về tên gọi, mà còn là bước chuyển mình trong chiến lược, cam kết mang đến những giá trị vượt trội trên hành trình kiến tạo tương lai xanh bền vững của VinFast.</span></p><p style=\"margin-right: 0px; margin-bottom: 10px; margin-left: 0px; color: rgb(60, 60, 60); font-family: Mulish, serif; letter-spacing: -0.32px;\"><span style=\"font-size: 12pt;\"><span style=\"font-family: &quot;times new roman&quot;, serif;\"><span style=\"color: rgb(0, 0, 0);\">Theo đó, các phiên bản sẽ có sự thay đổi tên bao gồm: VinFast VF 6 S, VinFast VF 7 S, VinFast VF 8 S Lux, VinFast VF 8 Lux Plus, VinFast VF e34. Chi tiết thay đổi như sau:</span></span></span></p><ol style=\"list-style: initial; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; padding: 0px 0px 0px 15px; color: rgb(60, 60, 60); font-family: Mulish, serif; letter-spacing: -0.32px;\"><li><span style=\"font-size: 12pt;\"><span style=\"font-family: &quot;times new roman&quot;, serif;\"><span style=\"color: rgb(0, 0, 0);\">VF 6 S đổi tên thành VF 6 Eco</span></span></span></li></ol><p style=\"margin-right: 0px; margin-bottom: 10px; margin-left: 0px; color: rgb(60, 60, 60); font-family: Mulish, serif; letter-spacing: -0.32px;\"><span style=\"font-size: 12pt;\"><span style=\"font-family: &quot;times new roman&quot;, serif;\"><span style=\"color: rgb(0, 0, 0);\">VinFast VF 6 S - Lựa chọn phù hợp cho gia đình trẻ hoặc cá nhân yêu thích sự tiện nghi với mức giá hợp lý nhất phân khúc xe gầm cao hạng B, sẽ đổi tên thành VinFast VF 6 Eco. Mẫu xe với thiết kế nội thất lấy cảm hứng từ ngôi nhà thứ hai của gia đình cùng không gian rộng rãi, đáp ứng nhu cầu sử dụng hằng ngày mà vẫn tối ưu về chi phí vận hành.</span></span></span></p><ol start=\"2\" style=\"list-style: initial; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; padding: 0px 0px 0px 15px; color: rgb(60, 60, 60); font-family: Mulish, serif; letter-spacing: -0.32px;\"><li><span style=\"font-size: 12pt;\"><span style=\"font-family: &quot;times new roman&quot;, serif;\"><span style=\"color: rgb(0, 0, 0);\">VF 7 S đổi tên thành VF 7 Eco&nbsp;</span></span></span></li></ol><p style=\"margin-right: 0px; margin-bottom: 10px; margin-left: 0px; color: rgb(60, 60, 60); font-family: Mulish, serif; letter-spacing: -0.32px;\"><span style=\"font-size: 12pt;\"><span style=\"font-family: &quot;times new roman&quot;, serif;\"><span style=\"color: rgb(0, 0, 0);\">VinFast VF 7 S – Mẫu SUV hạng C đẳng cấp, với thiết kế sắc sảo, mạnh mẽ sẽ được đổi tên thành VinFast VF 7 Eco. Chiếc xe được trang bị hiệu suất vận hành vượt trội, cùng hệ thống an toàn tối đa, mang lại trải nghiệm lái đầy thú vị trên mọi nẻo đường bạn đi tới.&nbsp;</span></span></span></p>', '/files/Blog/blog2-1_1.jpg', NULL, NULL, NULL, NULL, NULL, '2025-12-18 10:37:11', 'thanh', 1, 680, 0, NULL),
(1003, '5 cung đường đẹp quanh Đà Lạt dành cho tín đồ mê cầm lái', '5-cung-duong-dep-quanh-da-lat', NULL, 'Gợi ý lộ trình cuối tuần tuyệt vời qua đèo Tà Nung, Cầu Đất, hồ Tuyền Lâm cho những ai yêu thích trải nghiệm lái xe ngắm cảnh cao nguyên.', '\n                        <p class=\"lead fw-medium text-dark\">Đà Lạt luôn là điểm đến lý tưởng cho những chuyến road-trip tự lái nhờ khí hậu mát mẻ quanh năm và những cung đường đèo uốn lượn giữa rừng thông bạt ngàn. Dưới đây là 5 cung đường đẹp nhất mà bạn không nên bỏ lỡ khi thuê xe tự lái tại Đà Lạt.</p>\n                        <h4>1. Cung đường Đèo Prenn – Hồ Tuyền Lâm</h4>\n                        <p>Sau khi nâng cấp mở rộng mặt đường, đèo Prenn trở nên êm ái và rộng rãi hơn bao giờ hết. Từ giữa đèo, bạn có thể rẽ vào khu du lịch quốc gia Hồ Tuyền Lâm để tận hưởng không khí trong lành và mặt hồ phẳng lặng như gương.</p>\n                        <h4>2. Cung đường săn mây Trại Mát – Đồi chè Cầu Đất</h4>\n                        <p>Cách trung tâm Đà Lạt khoảng 25 km về phía Đông Nam, cung đường dẫn tới Đồi chè Cầu Đất và cánh đồng quạt gió là địa điểm hoàn hảo để khởi hành lúc 5h00 sáng săn biển mây bồng bềnh.</p>\n                        <h4>3. Cung đường Đèo Tà Nung – Thác Voi</h4>\n                        <p>Nếu bạn yêu thích cảm giác cầm lái một chiếc SUV gầm cao băng qua những khúc cua ngoạn mục ngập tràn hoa dã quỳ và những trang trại cà phê chồn, đèo Tà Nung chắc chắn sẽ làm bạn say đắm.</p>\n                        <h4>4. Cung đường Suối Vàng – Cây thông cô đơn</h4>\n                        <p>Tuyến đường ven hồ Suối Vàng (Đankia) với những thảm cỏ hồng trải dài là nơi lý tưởng để dừng xe chụp ảnh kỷ niệm và cắm trại dã ngoại trong ngày cùng gia đình.</p>\n                        <h4>5. Cung đường Đèo Khánh Lê (Nha Trang – Đà Lạt)</h4>\n                        <p>Cung đường nối liền biển và hoa dài 130 km đưa bạn đi từ nắng gió vịnh biển Nha Trang lên tới độ cao 1.500m của đỉnh Hòn Giao mờ sương chỉ trong hơn 2 giờ cầm lái.</p>', '/images/cars/hero.jpg', NULL, NULL, 'Hành trình du lịch', '2026-09-25 00:00:00', 'Ban biên tập ReCar', NULL, NULL, 1, 1520, 1, 'Nổi bật'),
(1004, 'Xu hướng thuê ô tô điện VinFast tự lái tăng trưởng mạnh năm 2026', 'xu-huong-thue-o-to-dien-vinfast-tu-lai-tang-truong-manh', NULL, 'Nhờ chi phí vận hành tiết kiệm, khả năng vận hành êm ái không mùi xăng và mạng lưới trạm sạc phủ rộng toàn quốc, các dòng xe VF 5, VF 6, VF 8 ngày càng được ưa chuộng.', '\n                        <p class=\"lead fw-medium text-dark\">Trong năm 2026, lượng đơn đặt thuê các dòng xe điện thông minh trên nền tảng ReCar ghi nhận mức tăng trưởng hơn 65% so với cùng kỳ, đặc biệt ở phân khúc xe đô thị và C-SUV gia đình.</p>\n                        <h4>1. Trải nghiệm êm ái, hạn chế say xe tối đa cho gia đình</h4>\n                        <p>Ưu điểm vượt trội của động cơ thuần điện là khả năng tăng tốc mượt mà, không có tiếng ồn động cơ đốt trong và hoàn toàn không có mùi nhiên liệu trong khoang cabin, giúp trẻ nhỏ và người lớn tuổi thoải mái suốt hành trình.</p>\n                        <h4>2. Hệ thống hỗ trợ lái nâng cao ADAS thông minh</h4>\n                        <p>Các mẫu xe điện thế hệ mới trên ReCar đều được trang bị gói hỗ trợ lái thông minh như giữ làn đường, kiểm soát hành trình thích ứng (Adaptive Cruise Control), cảnh báo điểm mù và camera 360 độ sắc nét.</p>', '/images/cars/car-electric.jpg', NULL, NULL, 'Công nghệ & Xe điện', '2026-10-12 00:00:00', 'Chuyên mục Thị trường Xe', NULL, NULL, 1, 2343, 1, 'Xu hướng'),
(1005, 'ReCar ra mắt tính năng Biên bản bàn giao xe điện tử (Digital Check-in)', 'recar-ra-mat-tinh-nang-bien-ban-ban-giao-xe-dien-tu-digital-checkin', NULL, 'Chuẩn hóa quy trình nhận và trả xe bằng hình ảnh 4 góc thực tế, đối soát ODO và mức nhiên liệu tự động nhằm bảo vệ quyền lợi cho cả khách thuê và chủ xe.', '\r\n                        <p class=\"lead fw-medium text-dark\">Nhằm xóa bỏ hoàn toàn những tranh chấp không đáng có về tình trạng ngoại thất hay phụ phí vượt km sau chuyến đi, ReCar chính thức tích hợp hệ thống Biên bản bàn giao xe điện tử ngay trên tài khoản khách hàng và chủ xe.</p>\r\n                        <h4>1. Minh bạch 100% chỉ số ODO và nhiên liệu</h4>\r\n                        <p>Khi bàn giao xe, chủ xe cập nhật chỉ số công-tơ-mét (ODO), phần trăm nhiên liệu/pin kèm hình ảnh 4 góc xe. Khách hàng có thể xem trực tiếp biên bản này trong mục <strong>Đơn thuê của tôi</strong>.</p>\r\n                        <h4>2. Tự động tính toán quãng đường di chuyển khi trả xe</h4>\r\n                        <p>Ngay khi hoàn tất Check-out lúc trả xe, hệ thống tự động đối chiếu số km thực tế đã đi so với hạn mức ngày và hiển thị chi tiết trên Hợp đồng điện tử.</p>', '/images/cars/car-luxury.jpg', NULL, NULL, 'Tin tức ReCar', '2026-10-18 00:00:00', 'Tin tức ReCar', '2026-11-12 01:16:06', 'Admin', 1, 866, 0, 'Đặc sắc');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `newscomment`
--

CREATE TABLE `newscomment` (
  `CommentId` int(11) NOT NULL,
  `Name` varchar(50) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  `Detail` varchar(200) DEFAULT NULL,
  `NewsId` int(11) DEFAULT NULL,
  `isActive` tinyint(1) DEFAULT NULL,
  `Reply` longtext DEFAULT NULL,
  `ReplyDate` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `newscomment`
--

INSERT INTO `newscomment` (`CommentId`, `Name`, `Phone`, `Email`, `CreatedDate`, `Detail`, `NewsId`, `isActive`, `Reply`, `ReplyDate`) VALUES
(2, 'thuy', NULL, 'thuy@gjt', '2025-01-12 15:00:31', 'tgsdgvhdm', 2, NULL, NULL, NULL),
(1002, 'tươi', NULL, 'heo@tuoi', '2025-01-13 12:02:40', 'fyuk', 2, NULL, NULL, NULL),
(1003, 'Trần Quốc Bảo', NULL, 'tranbao.carowner@gmail.com', '2026-11-12 21:11:05', 'a', 1005, 1, 'test', '2026-11-12 21:34:05');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `notifications`
--

CREATE TABLE `notifications` (
  `NotificationId` int(11) NOT NULL,
  `UserId` int(11) DEFAULT NULL,
  `Role` varchar(50) DEFAULT NULL,
  `Title` varchar(255) NOT NULL,
  `Content` text DEFAULT NULL,
  `Type` varchar(50) DEFAULT 'System',
  `LinkUrl` varchar(255) DEFAULT NULL,
  `IsRead` tinyint(1) NOT NULL DEFAULT 0,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `notifications`
--

INSERT INTO `notifications` (`NotificationId`, `UserId`, `Role`, `Title`, `Content`, `Type`, `LinkUrl`, `IsRead`, `CreatedAt`) VALUES
(1, NULL, 'CarOwner', 'Hồ sơ xe đã được phê duyệt', 'Xe VinFast VF 5 Plus (30A-999.88) của bạn đã được kiểm duyệt hợp lệ và sẵn sàng hoạt động trên sàn ReCar.', 'CarApproved', '/CarOwner/Cars', 1, '2026-11-09 23:19:17'),
(2, NULL, 'CarOwner', 'Thông báo hệ thống ReCar', 'Chào mừng đối tác chủ xe đến với ReCar. Hãy cập nhật đầy đủ lịch trình và giá thuê để tối ưu số chuyến.', 'System', '/CarOwner/Dashboard', 1, '2026-11-09 21:29:17'),
(3, NULL, 'Admin', 'Có xe mới chờ duyệt', 'Chủ xe vừa gửi hồ sơ đăng ký xe mới lên hệ thống. Vui lòng kiểm tra và phê duyệt.', 'CarPending', '/Admin/Cars/Pending', 1, '2026-11-09 22:59:17'),
(5, NULL, 'Admin', 'Yêu cầu chuyển trạng thái: Bảo trì', 'Chủ xe Trần Quốc Bảo gửi yêu cầu chuyển xe abc (30H7271141) sang \'Bảo trì\'. Lý do: bảo trì.', 'StatusRequest', '/Admin/Cars', 1, '2026-11-10 00:19:18'),
(8, NULL, 'Admin', 'Hồ sơ xe chờ duyệt cập nhật', 'Chủ xe Trần Quốc Bảo vừa lưu chỉnh sửa thông tin xe abc (30H7271141). Vui lòng xem xét phê duyệt.', 'CarPending', '/Admin/Cars', 1, '2026-11-10 02:20:04'),
(9, 1022, 'CarOwner', 'Hồ sơ xe bị từ chối kiểm duyệt ', 'Hồ sơ xe abc (30H7271141) chưa được duyệt. Lý do: nah. Vui lòng kiểm tra và chỉnh sửa lại thông tin.', 'CarRejected', '/CarOwner/Cars/Edit/26', 1, '2026-11-10 02:58:26'),
(10, NULL, 'Admin', 'Hồ sơ xe chờ duyệt cập nhật', 'Chủ xe Trần Quốc Bảo vừa lưu chỉnh sửa thông tin xe abc (30H7271141). Vui lòng xem xét phê duyệt.', 'CarPending', '/Admin/Cars', 0, '2026-11-10 02:59:13'),
(11, 1022, 'CarOwner', 'Hồ sơ xe bị từ chối kiểm duyệt ', 'Hồ sơ xe abc (30H7271141) chưa được duyệt. Lý do: a. Vui lòng kiểm tra và chỉnh sửa lại thông tin.', 'CarRejected', '/CarOwner/Cars/Edit/26', 0, '2026-11-10 03:22:51'),
(12, NULL, 'Admin', 'Hồ sơ xe chờ duyệt cập nhật', 'Chủ xe Trần Quốc Bảo vừa lưu chỉnh sửa thông tin xe abc (30H7271141). Vui lòng xem xét phê duyệt.', 'CarPending', '/Admin/Cars', 0, '2026-11-10 03:33:30'),
(13, 1022, 'CarOwner', 'Hồ sơ xe bị từ chối kiểm duyệt ', 'Hồ sơ xe abc (30H7271141) chưa được duyệt. Lý do: a. Vui lòng kiểm tra và chỉnh sửa lại thông tin.', 'CarRejected', '/CarOwner/Cars/Edit/26', 0, '2026-11-10 03:42:05'),
(14, NULL, 'Admin', 'Hồ sơ xe chờ duyệt cập nhật', 'Chủ xe Trần Quốc Bảo vừa lưu chỉnh sửa thông tin xe abc (30H7271141). Vui lòng xem xét phê duyệt.', 'CarPending', '/Admin/Cars', 0, '2026-11-10 03:42:34'),
(15, 1022, 'CarOwner', 'Hồ sơ xe bị từ chối kiểm duyệt ', 'Hồ sơ xe abc (30H7271141) chưa được duyệt. Lý do: a. Vui lòng kiểm tra và chỉnh sửa lại thông tin.', 'CarRejected', '/CarOwner/Cars/Edit/26', 0, '2026-11-10 03:49:02'),
(16, NULL, 'Admin', 'Hồ sơ xe chờ duyệt cập nhật', 'Chủ xe Trần Quốc Bảo vừa lưu chỉnh sửa thông tin xe abc (30H7271141). Vui lòng xem xét phê duyệt.', 'CarPending', '/Admin/Cars', 0, '2026-11-10 03:49:25'),
(17, 1022, 'CarOwner', 'Hồ sơ xe bị từ chối kiểm duyệt ', 'Hồ sơ xe abc (30H7271141) chưa được duyệt. Lý do: a. Vui lòng kiểm tra và chỉnh sửa lại thông tin.', 'CarRejected', '/CarOwner/Cars/Edit/26', 0, '2026-11-10 04:17:53'),
(18, NULL, 'Admin', 'Hồ sơ xe chờ duyệt cập nhật', 'Chủ xe Trần Quốc Bảo vừa lưu chỉnh sửa thông tin xe abc (30H7271141). Vui lòng xem xét phê duyệt.', 'CarPending', '/Admin/Cars', 0, '2026-11-10 04:19:16'),
(19, 1022, 'CarOwner', 'Hồ sơ xe đã được phê duyệt ', 'Chúc mừng bạn! Xe abc (30H7271141) đã được kiểm duyệt thành công và sẵn sàng đón khách thuê trên sàn ReCar.', 'CarApproved', '/CarOwner/Cars/Details/26', 0, '2026-11-10 04:19:26'),
(20, NULL, 'Admin', 'Hồ sơ xe mới chờ kiểm duyệt ', 'Chủ xe Trần Ngọc Hải vừa gửi hồ sơ xe abc (30H7271141). Vui lòng kiểm tra và phê duyệt.', 'CarPending', '/Admin/Cars/Pending', 0, '2026-11-15 18:19:23'),
(21, 1021, 'CarOwner', 'Đã gửi hồ sơ xe thành công ', 'Hồ sơ đăng ký xe abc (30H7271141) đã được gửi tới Ban Quản trị ReCar và đang ở trạng thái \'Chờ duyệt\'.', 'CarPending', '/CarOwner/Cars/Details/27', 0, '2026-11-15 18:19:23'),
(22, 1021, 'CarOwner', 'Hồ sơ xe đã được phê duyệt ', 'Chúc mừng bạn! Xe abc (30H7271141) đã được kiểm duyệt thành công và sẵn sàng đón khách thuê trên sàn ReCar.', 'CarApproved', '/CarOwner/Cars/Details/27', 0, '2026-11-15 18:19:57'),
(23, 1021, 'CarOwner', 'Xe đã bị xóa khỏi sàn ReCar', 'Xe abc (30H7271141) đã được Quản trị viên xóa khỏi hệ thống.', 'System', '/CarOwner/Cars', 0, '2026-11-15 18:20:31'),
(24, 1020, 'Customer', 'Đơn thuê xe đã bị hủy', 'Chủ xe đã hủy đơn #1049 (Theo thỏa thuận giữa hai bên). Số tiền cọc 455,000 đ đã được hoàn vào Số dư Ví ReCar của bạn.', 'Order', '/Accounts/Index?tab=wallet', 0, '2026-11-15 20:17:22'),
(25, 1022, 'CarOwner', 'Đã quyết toán đối soát', 'Admin đã xác nhận chuyển khoản quyết toán mã PO-202611-01-1022 (6,340,000 đ). Kiểm tra ngay trong Ví đối tác.', 'Payout', '/CarOwner/Wallet?tab=payouts', 0, '2026-11-15 20:18:35'),
(26, NULL, 'Admin', 'n thu xe m?i ', 'Khch hng Trần Kenn v?a ?t xe VinFast VF 5 Plus. M? n: #1050, ti?n c?c 455,000.', 'NewOrder', '/Admin/Orders/Edit/1050', 0, '2026-11-15 20:20:26'),
(27, 1022, 'CarOwner', 'B?n c n ?t xe m?i! ', 'Khch hng Trần Kenn v?a ?t thu xe VinFast VF 5 Plus. M? n: #1050. Vui l?ng ki?m tra l?ch bn giao xe.', 'NewOrder', '/CarOwner/Orders/Details/1050', 0, '2026-11-15 20:20:26'),
(28, 1020, 'Customer', '?t c?c thu xe thnh cng ', 'B?n ? ?t c?c thnh cng xe VinFast VF 5 Plus. M? n #1050. Ch? xe s? lin h? bn giao xe.', 'NewOrder', '/Accounts/Orders', 0, '2026-11-15 20:20:26'),
(29, 1020, 'Customer', 'Đơn thuê xe đã bị hủy', 'Chủ xe đã hủy đơn #1050 (Theo thỏa thuận giữa hai bên). Số tiền cọc 455,000 đ đã được hoàn vào Số dư Ví ReCar của bạn.', 'Order', '/Accounts/Index?tab=wallet', 0, '2026-11-15 20:20:49'),
(30, NULL, 'Admin', 'n thu xe m?i ', 'Khch hng Trần Quốc Bảo v?a ?t xe Kia Carnival Signature. M? n: #1052, ti?n c?c 1,680,000.', 'NewOrder', '/Admin/Orders/Edit/1052', 0, '2026-11-15 22:42:32'),
(31, 1026, 'CarOwner', 'B?n c n ?t xe m?i! ', 'Khch hng Trần Quốc Bảo v?a ?t thu xe Kia Carnival Signature. M? n: #1052. Vui l?ng ki?m tra l?ch bn giao xe.', 'NewOrder', '/CarOwner/Orders/Details/1052', 0, '2026-11-15 22:42:32'),
(32, 1022, 'Customer', '?t c?c thu xe thnh cng ', 'B?n ? ?t c?c thnh cng xe Kia Carnival Signature. M? n #1052. Ch? xe s? lin h? bn giao xe.', 'NewOrder', '/Accounts/Orders', 0, '2026-11-15 22:42:32');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `orderdetails`
--

CREATE TABLE `orderdetails` (
  `OrderDetailID` int(11) NOT NULL,
  `OrderID` int(11) NOT NULL,
  `CarID` int(11) NOT NULL,
  `Description` longtext DEFAULT NULL,
  `Price` decimal(18,2) DEFAULT NULL,
  `Quantity` int(11) DEFAULT NULL,
  `PickupDate` datetime DEFAULT NULL,
  `ReturnDate` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `orderdetails`
--

INSERT INTO `orderdetails` (`OrderDetailID`, `OrderID`, `CarID`, `Description`, `Price`, `Quantity`, `PickupDate`, `ReturnDate`) VALUES
(1037, 1030, 2, NULL, 800000.00, 2, '2025-12-21 00:00:00', '2025-12-22 00:00:00'),
(1050, 1043, 1, NULL, 1300000.00, 1, '2026-10-16 00:00:00', '2026-10-18 00:00:00'),
(1051, 1044, 7, NULL, 1600000.00, 1, '2026-10-28 00:00:00', '2026-10-30 00:00:00'),
(1052, 1045, 7, NULL, 1300000.00, 1, '2026-11-03 00:00:00', '2026-11-04 00:00:00'),
(1053, 1046, 7, NULL, 1600000.00, 1, '2026-11-03 00:00:00', '2026-11-05 00:00:00'),
(1054, 1047, 1, NULL, 1300000.00, 1, '2026-11-04 00:00:00', '2026-11-06 00:00:00'),
(1055, 1048, 1, NULL, 1300000.00, 1, '2026-11-05 00:00:00', '2026-11-07 00:00:00'),
(1056, 1049, 1, NULL, 1300000.00, 1, '2026-11-08 00:00:00', '2026-11-10 00:00:00'),
(1057, 1050, 1, NULL, 1300000.00, 1, '2026-11-15 00:00:00', '2026-11-17 00:00:00'),
(1058, 1051, 2, NULL, 1700000.00, 1, '2026-11-15 00:00:00', '2026-11-17 00:00:00'),
(1059, 1052, 5, NULL, 4800000.00, 1, '2026-11-15 00:00:00', '2026-11-17 00:00:00');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `orderstatus`
--

CREATE TABLE `orderstatus` (
  `StatusID` int(11) NOT NULL,
  `StatusDescription` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `orderstatus`
--

INSERT INTO `orderstatus` (`StatusID`, `StatusDescription`) VALUES
(1, ' Đã đặt'),
(2, 'Đã Huỷ'),
(3, 'Đã thanh toán'),
(4, 'Đang thuê'),
(5, 'Hoàn tất');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `partner_payouts`
--

CREATE TABLE `partner_payouts` (
  `PayoutId` int(11) NOT NULL,
  `SettlementCode` varchar(50) NOT NULL,
  `OwnerId` int(11) NOT NULL,
  `CycleKey` varchar(30) DEFAULT NULL,
  `Period` varchar(100) DEFAULT NULL,
  `Amount` decimal(18,2) NOT NULL DEFAULT 0.00,
  `Status` varchar(20) NOT NULL DEFAULT 'paid',
  `PaidAt` datetime NOT NULL,
  `ConfirmedBy` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `partner_payouts`
--

INSERT INTO `partner_payouts` (`PayoutId`, `SettlementCode`, `OwnerId`, `CycleKey`, `Period`, `Amount`, `Status`, `PaidAt`, `ConfirmedBy`) VALUES
(1, 'PO-202611-01-1022', 1022, '2026-11-01', 'Tháng 11/2026 (Kỳ 1)', 6340000.00, 'paid', '2026-11-15 20:18:35', 'Admin');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `role`
--

CREATE TABLE `role` (
  `RoleId` int(11) NOT NULL,
  `RoleName` varchar(50) DEFAULT NULL,
  `Description` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `role`
--

INSERT INTO `role` (`RoleId`, `RoleName`, `Description`) VALUES
(1, 'Admin', 'quanli'),
(2, 'guest', 'khách'),
(3, 'Chủ xe', 'Đối tác chủ xe cung cấp phương tiện cho hệ thống R');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `system_settings`
--

CREATE TABLE `system_settings` (
  `SettingId` int(11) NOT NULL,
  `DepositPct` int(11) NOT NULL DEFAULT 30,
  `CommissionPct` int(11) NOT NULL DEFAULT 10,
  `DefaultKmPerDay` int(11) NOT NULL DEFAULT 350,
  `ExtraKmPrice` decimal(18,2) NOT NULL DEFAULT 3000.00,
  `Hotline` varchar(50) NOT NULL DEFAULT '1900 8888',
  `SupportEmail` varchar(100) NOT NULL DEFAULT 'hotro@recar.vn',
  `PlatformName` varchar(150) NOT NULL DEFAULT 'ReCar — Car-Sharing Platform',
  `Address` varchar(255) NOT NULL DEFAULT 'Tòa tháp Keangnam Landmark 72, Mễ Trì, Nam Từ Liêm, Hà Nội',
  `UpdatedAt` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `system_settings`
--

INSERT INTO `system_settings` (`SettingId`, `DepositPct`, `CommissionPct`, `DefaultKmPerDay`, `ExtraKmPrice`, `Hotline`, `SupportEmail`, `PlatformName`, `Address`, `UpdatedAt`) VALUES
(1, 35, 20, 350, 3000.00, '1900 8888', 'hotro@recar.vn', 'ReCar — Car-Sharing Platform', 'Tòa tháp Keangnam Landmark 72, Mễ Trì, Nam Từ Liêm, Hà Nội', '2026-11-08 21:41:48');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `vehicle_inspections`
--

CREATE TABLE `vehicle_inspections` (
  `InspectionId` int(11) NOT NULL,
  `OrderId` int(11) NOT NULL,
  `CarId` int(11) NOT NULL,
  `InspectionType` varchar(20) NOT NULL,
  `Odometer` int(11) NOT NULL DEFAULT 0,
  `FuelLevel` int(11) NOT NULL DEFAULT 0,
  `FrontImage` varchar(255) DEFAULT NULL,
  `RearImage` varchar(255) DEFAULT NULL,
  `LeftImage` varchar(255) DEFAULT NULL,
  `RightImage` varchar(255) DEFAULT NULL,
  `Notes` varchar(1000) DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `CreatedBy` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `vehicle_inspections`
--

INSERT INTO `vehicle_inspections` (`InspectionId`, `OrderId`, `CarId`, `InspectionType`, `Odometer`, `FuelLevel`, `FrontImage`, `RearImage`, `LeftImage`, `RightImage`, `Notes`, `CreatedAt`, `CreatedBy`) VALUES
(1, 1045, 7, 'handover', 25000, 85, '/files/inspections/images_20261103002000760.jpg', '/files/inspections/images (1)_20261103002000763.jpg', '/files/inspections/images_20261103002000764.jpg', '/files/inspections/images_20261103002000765.jpg', 'Xe sạch sẽ, ngoại thất nguyên vẹn, lốp căng đều, đầy đủ giấy tờ & bảo hiểm.', '2026-11-03 00:20:00', 6),
(2, 1045, 7, 'return', 25400, 50, NULL, NULL, NULL, NULL, 'Nhận lại xe đúng hiện trạng, không phát sinh hư hại.', '2026-11-03 00:22:44', 6),
(3, 1046, 7, 'handover', 25000, 85, '/files/inspections/images (1)_20261103010757166.jpg', '/files/inspections/vangla_20261103010757168.jpg', '/files/inspections/images (2)_20261103010757170.jpg', '/files/inspections/images (2)_20261103010757171.jpg', 'Xe sạch sẽ, ngoại thất nguyên vẹn, lốp căng đều, đầy đủ giấy tờ & bảo hiểm.', '2026-11-03 01:07:57', 6),
(4, 1046, 7, 'return', 25180, 85, NULL, NULL, NULL, NULL, 'Nhận lại xe đúng hiện trạng, không phát sinh hư hại.', '2026-11-03 02:16:07', 6),
(5, 1047, 1, 'handover', 25000, 85, '/files/inspections/images (2)_20261104233122694.jpg', '/files/inspections/images (2)_20261104233122701.jpg', NULL, NULL, 'Xe sạch sẽ, ngoại thất nguyên vẹn, lốp căng đều, đầy đủ giấy tờ & bảo hiểm.', '2026-11-04 23:31:22', 6),
(6, 1047, 1, 'return', 25780, 75, '/files/inspections/images_20261104233320425.jpg', '/files/inspections/vangla_20261104233320426.jpg', '/files/inspections/116_20261104233320427.jpg', '/files/inspections/images (1)_20261104233320429.jpg', 'Nhận lại xe đúng hiện trạng, không phát sinh hư hại.', '2026-11-04 23:33:20', 6),
(7, 1048, 1, 'handover', 25000, 85, NULL, NULL, NULL, NULL, 'Xe sạch sẽ, ngoại thất nguyên vẹn, lốp căng đều, đầy đủ giấy tờ & bảo hiểm.', '2026-11-05 23:29:06', 1022),
(8, 1048, 1, 'return', 25180, 85, NULL, NULL, NULL, NULL, 'Nhận lại xe đúng hiện trạng, không phát sinh hư hại.', '2026-11-05 23:31:22', 1022);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wallet_transactions`
--

CREATE TABLE `wallet_transactions` (
  `TransactionId` int(11) NOT NULL,
  `CustomerId` int(11) NOT NULL,
  `OrderId` int(11) DEFAULT NULL,
  `Type` varchar(30) NOT NULL,
  `Amount` decimal(18,2) NOT NULL DEFAULT 0.00,
  `BalanceAfter` decimal(18,2) NOT NULL DEFAULT 0.00,
  `Description` varchar(500) NOT NULL DEFAULT '',
  `BankName` varchar(100) DEFAULT NULL,
  `BankAccount` varchar(50) DEFAULT NULL,
  `AccountHolder` varchar(150) DEFAULT NULL,
  `Status` varchar(20) NOT NULL DEFAULT 'completed',
  `CreatedAt` datetime NOT NULL,
  `ProcessedAt` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wallet_transactions`
--

INSERT INTO `wallet_transactions` (`TransactionId`, `CustomerId`, `OrderId`, `Type`, `Amount`, `BalanceAfter`, `Description`, `BankName`, `BankAccount`, `AccountHolder`, `Status`, `CreatedAt`, `ProcessedAt`) VALUES
(1, 1020, 1049, 'refund', 455000.00, 455000.00, 'Hoàn tiền cọc đơn #1049 (Chủ xe hủy: Theo thỏa thuận giữa hai bên)', NULL, NULL, NULL, 'completed', '2026-11-15 20:17:21', '2026-11-15 20:17:21'),
(2, 1020, 1050, 'payment', 455000.00, 0.00, 'Thanh toán đặt cọc 35% đơn #1050 bằng Ví ReCar', NULL, NULL, NULL, 'completed', '2026-11-15 20:20:26', '2026-11-15 20:20:26'),
(3, 1020, 1050, 'refund', 455000.00, 455000.00, 'Hoàn tiền cọc đơn #1050 (Chủ xe hủy: Theo thỏa thuận giữa hai bên)', NULL, NULL, NULL, 'completed', '2026-11-15 20:20:49', '2026-11-15 20:20:49'),
(4, 1022, 1052, 'refund', 1680000.00, 1680000.00, 'Hoàn tiền cọc khi hủy đơn #1052', NULL, NULL, NULL, 'completed', '2026-11-15 22:42:51', '2026-11-15 22:42:51');

--
-- Chỉ mục cho các bảng đã đổ
--

--
-- Chỉ mục cho bảng `amenities`
--
ALTER TABLE `amenities`
  ADD PRIMARY KEY (`AmenityId`),
  ADD KEY `idx_amenity_order` (`DisplayOrder`);

--
-- Chỉ mục cho bảng `banks`
--
ALTER TABLE `banks`
  ADD PRIMARY KEY (`BankId`),
  ADD KEY `idx_bank_code` (`BankCode`);

--
-- Chỉ mục cho bảng `blog`
--
ALTER TABLE `blog`
  ADD PRIMARY KEY (`BlogId`),
  ADD KEY `FK_Blog_Account` (`CustomerId`);

--
-- Chỉ mục cho bảng `blogcomment`
--
ALTER TABLE `blogcomment`
  ADD PRIMARY KEY (`CommentId`),
  ADD KEY `FK_BlogComment_Blog` (`BlogId`);

--
-- Chỉ mục cho bảng `carimages`
--
ALTER TABLE `carimages`
  ADD PRIMARY KEY (`ImageID`);

--
-- Chỉ mục cho bảng `carowner_registrations`
--
ALTER TABLE `carowner_registrations`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `idx_reg_customer` (`CustomerId`),
  ADD KEY `idx_reg_status` (`Status`);

--
-- Chỉ mục cho bảng `carrentalorders`
--
ALTER TABLE `carrentalorders`
  ADD PRIMARY KEY (`OrderID`),
  ADD KEY `FK_CarRentalOrders_Customer` (`CustomerID`),
  ADD KEY `FK_CarRentalOrders_Status` (`StatusID`);

--
-- Chỉ mục cho bảng `carreviews`
--
ALTER TABLE `carreviews`
  ADD PRIMARY KEY (`CarReviewID`),
  ADD KEY `FK_CarReviews_Cars` (`CarID`),
  ADD KEY `OrderId` (`OrderId`);

--
-- Chỉ mục cho bảng `cars`
--
ALTER TABLE `cars`
  ADD PRIMARY KEY (`CarID`),
  ADD KEY `FK_Cars_CarTypes` (`TypeID`),
  ADD KEY `idx_cars_owner` (`OwnerID`);

--
-- Chỉ mục cho bảng `cartypes`
--
ALTER TABLE `cartypes`
  ADD PRIMARY KEY (`TypeID`);

--
-- Chỉ mục cho bảng `car_amenities`
--
ALTER TABLE `car_amenities`
  ADD PRIMARY KEY (`CarId`,`AmenityId`),
  ADD KEY `FK_CarAmenities_Amenities` (`AmenityId`);

--
-- Chỉ mục cho bảng `car_brands`
--
ALTER TABLE `car_brands`
  ADD PRIMARY KEY (`BrandId`);

--
-- Chỉ mục cho bảng `car_calendar_blocks`
--
ALTER TABLE `car_calendar_blocks`
  ADD PRIMARY KEY (`BlockId`),
  ADD KEY `IX_CalendarBlock_Car` (`CarId`,`StartTime`,`EndTime`);

--
-- Chỉ mục cho bảng `chatmessages`
--
ALTER TABLE `chatmessages`
  ADD PRIMARY KEY (`MessageId`),
  ADD KEY `idx_chat_car` (`CarId`),
  ADD KEY `idx_chat_receiver` (`ReceiverId`),
  ADD KEY `idx_chat_sender` (`SenderId`);

--
-- Chỉ mục cho bảng `contact`
--
ALTER TABLE `contact`
  ADD PRIMARY KEY (`ContactId`);

--
-- Chỉ mục cho bảng `customers`
--
ALTER TABLE `customers`
  ADD PRIMARY KEY (`CustomerID`),
  ADD KEY `FK_Customers_Role` (`RoleId`);

--
-- Chỉ mục cho bảng `locations`
--
ALTER TABLE `locations`
  ADD PRIMARY KEY (`LocationID`);

--
-- Chỉ mục cho bảng `menu`
--
ALTER TABLE `menu`
  ADD PRIMARY KEY (`MenuID`);

--
-- Chỉ mục cho bảng `news`
--
ALTER TABLE `news`
  ADD PRIMARY KEY (`NewsId`);

--
-- Chỉ mục cho bảng `newscomment`
--
ALTER TABLE `newscomment`
  ADD PRIMARY KEY (`CommentId`),
  ADD KEY `FK_NewsComment_News` (`NewsId`);

--
-- Chỉ mục cho bảng `notifications`
--
ALTER TABLE `notifications`
  ADD PRIMARY KEY (`NotificationId`),
  ADD KEY `idx_notifications_user` (`UserId`),
  ADD KEY `idx_notifications_role` (`Role`),
  ADD KEY `idx_notifications_created` (`CreatedAt`);

--
-- Chỉ mục cho bảng `orderdetails`
--
ALTER TABLE `orderdetails`
  ADD PRIMARY KEY (`OrderDetailID`),
  ADD KEY `FK_OrderDetails_Order` (`OrderID`);

--
-- Chỉ mục cho bảng `orderstatus`
--
ALTER TABLE `orderstatus`
  ADD PRIMARY KEY (`StatusID`);

--
-- Chỉ mục cho bảng `partner_payouts`
--
ALTER TABLE `partner_payouts`
  ADD PRIMARY KEY (`PayoutId`),
  ADD UNIQUE KEY `UX_partner_payouts_code` (`SettlementCode`),
  ADD KEY `IX_partner_payouts_owner` (`OwnerId`);

--
-- Chỉ mục cho bảng `role`
--
ALTER TABLE `role`
  ADD PRIMARY KEY (`RoleId`);

--
-- Chỉ mục cho bảng `system_settings`
--
ALTER TABLE `system_settings`
  ADD PRIMARY KEY (`SettingId`);

--
-- Chỉ mục cho bảng `vehicle_inspections`
--
ALTER TABLE `vehicle_inspections`
  ADD PRIMARY KEY (`InspectionId`),
  ADD UNIQUE KEY `UX_Inspection_Order_Type` (`OrderId`,`InspectionType`),
  ADD KEY `IX_Inspection_Car` (`CarId`);

--
-- Chỉ mục cho bảng `wallet_transactions`
--
ALTER TABLE `wallet_transactions`
  ADD PRIMARY KEY (`TransactionId`),
  ADD KEY `IX_wallet_transactions_customer` (`CustomerId`),
  ADD KEY `IX_wallet_transactions_order` (`OrderId`);

--
-- AUTO_INCREMENT cho các bảng đã đổ
--

--
-- AUTO_INCREMENT cho bảng `amenities`
--
ALTER TABLE `amenities`
  MODIFY `AmenityId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT cho bảng `banks`
--
ALTER TABLE `banks`
  MODIFY `BankId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT cho bảng `blog`
--
ALTER TABLE `blog`
  MODIFY `BlogId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT cho bảng `blogcomment`
--
ALTER TABLE `blogcomment`
  MODIFY `CommentId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT cho bảng `carimages`
--
ALTER TABLE `carimages`
  MODIFY `ImageID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT cho bảng `carowner_registrations`
--
ALTER TABLE `carowner_registrations`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT cho bảng `carrentalorders`
--
ALTER TABLE `carrentalorders`
  MODIFY `OrderID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1053;

--
-- AUTO_INCREMENT cho bảng `carreviews`
--
ALTER TABLE `carreviews`
  MODIFY `CarReviewID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- AUTO_INCREMENT cho bảng `cars`
--
ALTER TABLE `cars`
  MODIFY `CarID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=28;

--
-- AUTO_INCREMENT cho bảng `cartypes`
--
ALTER TABLE `cartypes`
  MODIFY `TypeID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT cho bảng `car_brands`
--
ALTER TABLE `car_brands`
  MODIFY `BrandId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT cho bảng `car_calendar_blocks`
--
ALTER TABLE `car_calendar_blocks`
  MODIFY `BlockId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT cho bảng `chatmessages`
--
ALTER TABLE `chatmessages`
  MODIFY `MessageId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT cho bảng `contact`
--
ALTER TABLE `contact`
  MODIFY `ContactId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT cho bảng `customers`
--
ALTER TABLE `customers`
  MODIFY `CustomerID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1028;

--
-- AUTO_INCREMENT cho bảng `locations`
--
ALTER TABLE `locations`
  MODIFY `LocationID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=38;

--
-- AUTO_INCREMENT cho bảng `menu`
--
ALTER TABLE `menu`
  MODIFY `MenuID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=315;

--
-- AUTO_INCREMENT cho bảng `news`
--
ALTER TABLE `news`
  MODIFY `NewsId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1006;

--
-- AUTO_INCREMENT cho bảng `newscomment`
--
ALTER TABLE `newscomment`
  MODIFY `CommentId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1004;

--
-- AUTO_INCREMENT cho bảng `notifications`
--
ALTER TABLE `notifications`
  MODIFY `NotificationId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=33;

--
-- AUTO_INCREMENT cho bảng `orderdetails`
--
ALTER TABLE `orderdetails`
  MODIFY `OrderDetailID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1060;

--
-- AUTO_INCREMENT cho bảng `orderstatus`
--
ALTER TABLE `orderstatus`
  MODIFY `StatusID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT cho bảng `partner_payouts`
--
ALTER TABLE `partner_payouts`
  MODIFY `PayoutId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT cho bảng `system_settings`
--
ALTER TABLE `system_settings`
  MODIFY `SettingId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT cho bảng `vehicle_inspections`
--
ALTER TABLE `vehicle_inspections`
  MODIFY `InspectionId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT cho bảng `wallet_transactions`
--
ALTER TABLE `wallet_transactions`
  MODIFY `TransactionId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Các ràng buộc cho các bảng đã đổ
--

--
-- Các ràng buộc cho bảng `blogcomment`
--
ALTER TABLE `blogcomment`
  ADD CONSTRAINT `FK_BlogComment_Blog` FOREIGN KEY (`BlogId`) REFERENCES `blog` (`BlogId`);

--
-- Các ràng buộc cho bảng `carrentalorders`
--
ALTER TABLE `carrentalorders`
  ADD CONSTRAINT `FK_CarRentalOrders_Customer` FOREIGN KEY (`CustomerID`) REFERENCES `customers` (`CustomerID`),
  ADD CONSTRAINT `FK_CarRentalOrders_Status` FOREIGN KEY (`StatusID`) REFERENCES `orderstatus` (`StatusID`),
  ADD CONSTRAINT `FK__CarRental__Custo__2D27B809` FOREIGN KEY (`CustomerID`) REFERENCES `customers` (`CustomerID`),
  ADD CONSTRAINT `FK__CarRental__Statu__2E1BDC42` FOREIGN KEY (`StatusID`) REFERENCES `orderstatus` (`StatusID`);

--
-- Các ràng buộc cho bảng `carreviews`
--
ALTER TABLE `carreviews`
  ADD CONSTRAINT `FK_CarReviews_Cars` FOREIGN KEY (`CarID`) REFERENCES `cars` (`CarID`);

--
-- Các ràng buộc cho bảng `cars`
--
ALTER TABLE `cars`
  ADD CONSTRAINT `FK_Cars_CarTypes` FOREIGN KEY (`TypeID`) REFERENCES `cartypes` (`TypeID`);

--
-- Các ràng buộc cho bảng `car_amenities`
--
ALTER TABLE `car_amenities`
  ADD CONSTRAINT `FK_CarAmenities_Amenities` FOREIGN KEY (`AmenityId`) REFERENCES `amenities` (`AmenityId`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `FK_CarAmenities_Cars` FOREIGN KEY (`CarId`) REFERENCES `cars` (`CarID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Các ràng buộc cho bảng `customers`
--
ALTER TABLE `customers`
  ADD CONSTRAINT `FK_Customers_Role` FOREIGN KEY (`RoleId`) REFERENCES `role` (`RoleId`);

--
-- Các ràng buộc cho bảng `newscomment`
--
ALTER TABLE `newscomment`
  ADD CONSTRAINT `FK_NewsComment_News` FOREIGN KEY (`NewsId`) REFERENCES `news` (`NewsId`);

--
-- Các ràng buộc cho bảng `orderdetails`
--
ALTER TABLE `orderdetails`
  ADD CONSTRAINT `FK_OrderDetails_Order` FOREIGN KEY (`OrderID`) REFERENCES `carrentalorders` (`OrderID`);

--
-- Các ràng buộc cho bảng `vehicle_inspections`
--
ALTER TABLE `vehicle_inspections`
  ADD CONSTRAINT `FK_Inspection_Car` FOREIGN KEY (`CarId`) REFERENCES `cars` (`CarID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `FK_Inspection_Order` FOREIGN KEY (`OrderId`) REFERENCES `carrentalorders` (`OrderID`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
