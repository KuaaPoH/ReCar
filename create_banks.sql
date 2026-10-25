USE DbRenalCar;

SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

DROP TABLE IF EXISTS `banks`;

CREATE TABLE `banks` (
    `BankId` INT AUTO_INCREMENT PRIMARY KEY,
    `BankCode` VARCHAR(20) NOT NULL,
    `BankName` VARCHAR(150) NOT NULL,
    `ShortName` VARCHAR(50) NOT NULL,
    `Bin` VARCHAR(10) DEFAULT NULL,
    `Logo` VARCHAR(255) DEFAULT NULL,
    `DisplayOrder` INT DEFAULT 0,
    `IsActive` TINYINT(1) DEFAULT 1,
    INDEX `idx_bank_code` (`BankCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `banks` (`BankCode`, `ShortName`, `BankName`, `Bin`, `DisplayOrder`, `IsActive`) VALUES
('VCB', 'Vietcombank', 'Ngân hàng TMCP Ngoại thương Việt Nam', '970436', 1, 1),
('MB', 'MB Bank', 'Ngân hàng TMCP Quân đội', '970422', 2, 1),
('TCB', 'Techcombank', 'Ngân hàng TMCP Kỹ thương Việt Nam', '970407', 3, 1),
('BIDV', 'BIDV', 'Ngân hàng TMCP Đầu tư và Phát triển Việt Nam', '970418', 4, 1),
('CTG', 'VietinBank', 'Ngân hàng TMCP Công thương Việt Nam', '970415', 5, 1),
('VPB', 'VPBank', 'Ngân hàng TMCP Việt Nam Thịnh Vượng', '970432', 6, 1),
('ACB', 'ACB', 'Ngân hàng TMCP Á Châu', '970416', 7, 1),
('TPB', 'TPBank', 'Ngân hàng TMCP Tiên Phong', '970423', 8, 1),
('STB', 'Sacombank', 'Ngân hàng TMCP Sài Gòn Thương Tín', '970403', 9, 1),
('HDB', 'HDBank', 'Ngân hàng TMCP Phát triển TP.HCM', '970437', 10, 1),
('VBA', 'Agribank', 'Ngân hàng Nông nghiệp & PTNT Việt Nam', '970405', 11, 1),
('VIB', 'VIB', 'Ngân hàng TMCP Quốc tế Việt Nam', '970441', 12, 1),
('SHB', 'SHB', 'Ngân hàng TMCP Sài Gòn - Hà Nội', '970443', 13, 1),
('MSB', 'MSB', 'Ngân hàng TMCP Hàng Hải Việt Nam', '970426', 14, 1),
('OCB', 'OCB', 'Ngân hàng TMCP Phương Đông', '970448', 15, 1),
('SSB', 'SeABank', 'Ngân hàng TMCP Đông Nam Á', '970440', 16, 1),
('LPB', 'LPBank', 'Ngân hàng TMCP Lộc Phát Việt Nam', '970449', 17, 1),
('NAB', 'Nam A Bank', 'Ngân hàng TMCP Nam Á', '970428', 18, 1),
('BVB', 'BVBank', 'Ngân hàng TMCP Bản Việt', '970454', 19, 1),
('BAB', 'Bac A Bank', 'Ngân hàng TMCP Bắc Á', '970409', 20, 1);
