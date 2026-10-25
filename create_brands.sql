-- Tạo bảng car_brands và nạp danh sách các hãng xe phổ biến
SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

CREATE TABLE IF NOT EXISTS `car_brands` (
  `BrandId` int(11) NOT NULL AUTO_INCREMENT,
  `BrandName` varchar(100) NOT NULL,
  `Country` varchar(100) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT 0,
  `IsActive` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`BrandId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `car_brands` (`BrandName`, `Country`, `DisplayOrder`, `IsActive`) VALUES
('VinFast', 'Việt Nam', 1, 1),
('Toyota', 'Nhật Bản', 2, 1),
('Hyundai', 'Hàn Quốc', 3, 1),
('Kia', 'Hàn Quốc', 4, 1),
('Mazda', 'Nhật Bản', 5, 1),
('Honda', 'Nhật Bản', 6, 1),
('Ford', 'Mỹ', 7, 1),
('Mitsubishi', 'Nhật Bản', 8, 1),
('Mercedes-Benz', 'Đức', 9, 1),
('BMW', 'Đức', 10, 1),
('Lexus', 'Nhật Bản', 11, 1),
('Audi', 'Đức', 12, 1),
('Nissan', 'Nhật Bản', 13, 1),
('MG', 'Anh / Trung Quốc', 14, 1),
('Subaru', 'Nhật Bản', 15, 1),
('Peugeot', 'Pháp', 16, 1),
('Suzuki', 'Nhật Bản', 17, 1),
('Porsche', 'Đức', 18, 1),
('Land Rover', 'Anh', 19, 1),
('Volvo', 'Thụy Điển', 20, 1)
ON DUPLICATE KEY UPDATE `BrandName` = VALUES(`BrandName`);
