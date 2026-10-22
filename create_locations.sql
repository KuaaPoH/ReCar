USE DbRenalCar;

DROP TABLE IF EXISTS locations;

CREATE TABLE locations (
    LocationID INT AUTO_INCREMENT PRIMARY KEY,
    CityName VARCHAR(100) NOT NULL,
    Region VARCHAR(50) DEFAULT 'Miền Bắc',
    IsPopular TINYINT(1) DEFAULT 1,
    DisplayOrder INT DEFAULT 0,
    IsActive TINYINT(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO locations (CityName, Region, IsPopular, DisplayOrder, IsActive) VALUES
('Hà Nội', 'Miền Bắc', 1, 1, 1),
('TP. Hồ Chí Minh', 'Miền Nam', 1, 2, 1),
('TP. Vinh (Nghệ An)', 'Miền Trung', 1, 3, 1),
('Đà Nẵng', 'Miền Trung', 1, 4, 1),
('Hải Phòng', 'Miền Bắc', 1, 5, 1),
('Cần Thơ', 'Miền Nam', 1, 6, 1),
('Nha Trang (Khánh Hòa)', 'Miền Trung', 1, 7, 1),
('Đà Lạt (Lâm Đồng)', 'Miền Trung', 1, 8, 1),
('Vũng Tàu (Bà Rịa)', 'Miền Nam', 1, 9, 1),
('Hạ Long (Quảng Ninh)', 'Miền Bắc', 1, 10, 1),
('Huế (Thừa Thiên Huế)', 'Miền Trung', 0, 11, 1),
('Quy Nhơn (Bình Định)', 'Miền Trung', 0, 12, 1),
('Phan Thiết (Bình Thuận)', 'Miền Nam', 0, 13, 1),
('Thanh Hóa', 'Miền Bắc', 0, 14, 1),
('Bắc Ninh', 'Miền Bắc', 0, 15, 1),
('Buôn Ma Thuột (Đắk Lắk)', 'Miền Trung', 0, 16, 1),
('Phú Quốc (Kiên Giang)', 'Miền Nam', 1, 17, 1);
