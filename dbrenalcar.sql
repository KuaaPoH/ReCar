-- MariaDB dump 10.19  Distrib 10.4.32-MariaDB, for Win64 (AMD64)
--
-- Host: localhost    Database: DbRenalCar
-- ------------------------------------------------------
-- Server version	10.4.32-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `account`
--

DROP TABLE IF EXISTS `account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `account` (
  `AccountId` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) DEFAULT NULL,
  `Password` varchar(50) DEFAULT NULL,
  `FullName` varchar(50) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  `LastLogin` char(10) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`AccountId`),
  KEY `FK_Account_Role` (`RoleId`),
  CONSTRAINT `FK_Account_Role` FOREIGN KEY (`RoleId`) REFERENCES `role` (`RoleId`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `account`
--

LOCK TABLES `account` WRITE;
/*!40000 ALTER TABLE `account` DISABLE KEYS */;
INSERT INTO `account` VALUES (5,'binh','d0afd43f169f68af6d56cf454d74b167','bình','4234234','binh@gmail.com',3,'2025-12-21',1),(6,'tranbaocar','d0afd43f169f68af6d56cf454d74b167','Trần Quốc Bảo','0912.345.678','tranbao.carowner@gmail.com',3,'2025-12-21',1),(7,'Admin','93893acf0c705336677073e806d09e5c','Administrator',NULL,'admin@system.local',1,'2025-12-22',1),(8,'minhanh_auto','d0afd43f169f68af6d56cf454d74b167','Nguyễn Minh Anh','0988.223.344','minhanh.carrental@gmail.com',3,'2025-12-22',1),(9,'quan1','123','thuy','12334','quan@gmail.com',2,NULL,1),(10,'dauquan','d0afd43f169f68af6d56cf454d74b167','Đậu Công Quân','12334','dauquan@gmail.com',2,NULL,1),(11,'thuy','d0afd43f169f68af6d56cf454d74b167','thuy','12334','thuy@gmail.com',2,NULL,1),(12,'Q','d0afd43f169f68af6d56cf454d74b167','Nguyễn Hồng Quân','0353675457','Q@gmail.com',2,NULL,1),(13,'Huyen','ca3ea7a65775384c0aa2249769adb543','Hoàng Thanh Huyền cute','0123345678','Huyen@gmail.com',2,NULL,1),(14,'Dau','d0afd43f169f68af6d56cf454d74b167','Doan thanh thuy','1234353445','Dau@gmailcom',2,NULL,1),(15,'T','d0afd43f169f68af6d56cf454d74b167','Doãn thị thanh thuỷ','131243243','T@gmail.com',2,NULL,1),(16,'anh','d0afd43f169f68af6d56cf454d74b167','Minh Anh',NULL,'anh@gmail.com',2,NULL,1),(17,'tt','d0afd43f169f68af6d56cf454d74b167',NULL,NULL,'tt@gmail.com',2,NULL,1),(19,'bintest','d411cf68bd7f71d13fb053154f455651','binhtest1','','bintest@gmail.com',2,NULL,1),(20,'thuha_luxury','d0afd43f169f68af6d56cf454d74b167','Lê Thu Hà','0339.772.881','thuha.luxurycars@gmail.com',3,NULL,1),(21,'t12','d0afd43f169f68af6d56cf454d74b167','ẻgeergge',NULL,'t12@gmail.com',2,NULL,1),(23,'trankenn04','d0afd43f169f68af6d56cf454d74b167','Trần Kenn','0987654321','trankenn04@gmail.com',2,NULL,1),(24,'hoangnam_suv','d0afd43f169f68af6d56cf454d74b167','Phạm Hoàng Nam','0977.889.900','hoangnam.suv@gmail.com',3,NULL,1),(25,'dinhdung_rent','d0afd43f169f68af6d56cf454d74b167','Vũ Đình Dũng','0905.112.233','dinhdung.rentcar@gmail.com',3,NULL,1);
/*!40000 ALTER TABLE `account` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `blog`
--

DROP TABLE IF EXISTS `blog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `blog` (
  `BlogId` int(11) NOT NULL AUTO_INCREMENT,
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
  `AccountId` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`BlogId`),
  KEY `FK_Blog_Account` (`AccountId`),
  CONSTRAINT `FK_Blog_Account` FOREIGN KEY (`AccountId`) REFERENCES `account` (`AccountId`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `blog`
--

LOCK TABLES `blog` WRITE;
/*!40000 ALTER TABLE `blog` DISABLE KEYS */;
INSERT INTO `blog` VALUES (1,'Kinh nghiệm thuê xe tự lái lần đầu cho người mới','kinh-nghiem-thue-xe-tu-lai-lan-dau',NULL,'Những điều cần kiểm tra trước khi nhận xe để chuyến đi của bạn suôn sẻ và an toàn nhất.','Khi nhận xe tự lái, bạn cần quay video xung quanh thân xe, kiểm tra mức xăng/pin, chạy thử 500m để cảm nhận phanh và vô lăng, kiểm tra giấy tờ đăng kiểm xe...','/images/cars/car-sedan.jpg',NULL,NULL,NULL,'2026-10-13 20:39:31','ReCar Team',NULL,NULL,NULL,1),(2,'Giấy tờ cần chuẩn bị khi thuê xe ô tô tự lái','giay-to-can-chuan-bi-khi-thue-o-to',NULL,'CCCD gắn chip, Giấy phép lái xe hạng B trở lên và các giấy tờ cần thiết để ký hợp đồng.','Để hoàn tất thủ tục thuê xe tại ReCar, bạn chỉ cần tải ảnh chụp CCCD và Giấy phép lái xe hạng B1/B2 lên tài khoản và đối chiếu bản gốc lúc nhận bàn giao xe...','/images/cars/car-suv.jpg',NULL,NULL,NULL,'2026-10-13 20:39:31','ReCar Team',NULL,NULL,NULL,1),(3,'5 cung đường đẹp quanh Đà Lạt đáng trải nghiệm nhất','5-cung-duong-dep-quanh-da-lat',NULL,'Gợi ý lộ trình du lịch cuối tuần lý tưởng cho những ai yêu thích tự lái xe qua các cung đèo hùng vĩ.','Đà Lạt nổi tiếng với những cung đường uốn lượn quanh rừng thông như đèo Tà Nung, đèo Mimosa, cung đường hoa dã quỳ Cầu Đất...','/images/cars/hero.jpg',NULL,NULL,NULL,'2024-12-01 11:34:04','thanh','2024-12-01 11:47:04','thanh',NULL,1),(7,'Phiên bản cuối cùng của Porsche 911 Dakar: Đỉnh cao của sự đặc biệt','phien-ban-cuoi-cung-cua-porsche-911-dakar-dinh-cao-cua-su-dac-biet',NULL,'Đỉnh cao của sự đặc biệt','<h2 id=\"Thi+t+k++ngo+i+th+t+c+a+Porsche+911+Dakar\" name=\"Thi+t+k++ngo+i+th+t+c+a+Porsche+911+Dakar\" style=\"font-family: Roboto, sans-serif; font-weight: 700; color: rgb(34, 34, 34); font-size: 20px;\">Thiết kế ngoại thất của Porsche 911 Dakar</h2><p style=\"color: rgb(34, 34, 34); font-family: Roboto, sans-serif;\">Chiếc&nbsp;<a href=\"https://www.carmudi.vn/blog-xe-hoi/porsche-911-gt3-2022-mau-la-mat-trang-bi-so-san-dau-tien-tai-viet-nam/\">Porsche 911</a>&nbsp;Dakar cuối cùng xuất xưởng được thiết kế ngoại thất độc đáo, tôn vinh chiến thắng tại giải đua Paris-Dakar năm 1984. Phần trên của xe được sơn màu vàng Signal, trong khi phần dưới mang sắc xanh Gentian Blue Metallic, tạo nên sự tương phản nổi bật. Hai tông màu này được ngăn cách bởi đường kẻ xanh dương nhạt Lampedusa Blue, lấy cảm hứng từ hòn đảo Lampedusa ở Địa Trung Hải.</p><p style=\"color: rgb(34, 34, 34); font-family: Roboto, sans-serif;\"><img src=\"/files/Blog/blog1.jpeg\" style=\"width: 1038px;\"></p><p style=\"color: rgb(34, 34, 34); font-family: Roboto, sans-serif;\">Đặc biệt, bộ mâm xe cũng được sơn màu vàng Signal, kết hợp với viền xanh Lampedusa Blue, tạo nên sự hài hòa và tinh tế. Các chi tiết bằng thép không gỉ ở cản trước, cản sau và ngưỡng cửa bên hông được sơn đen, tăng thêm vẻ mạnh mẽ cho xe. Ngoài ra, viền đèn pha cũng được trang trí bằng màu xanh Lampedusa Blue, tạo điểm nhấn độc đáo cho phần đầu xe.</p><p style=\"color: rgb(34, 34, 34); font-family: Roboto, sans-serif;\">Toàn bộ quá trình sơn ngoại thất được thực hiện thủ công, thể hiện sự tỉ mỉ và đẳng cấp của Porsche trong việc cá nhân hóa sản phẩm. Thiết kế này không chỉ tôn vinh lịch sử huy hoàng của hãng mà còn mang đến cho chủ sở hữu một chiếc xe độc nhất vô nhị, kết hợp giữa di sản và phong cách hiện đại.</p>','/files/Blog/blog1.jpeg',NULL,NULL,NULL,'2024-12-01 11:34:04','thanh','2024-12-01 11:34:08','',NULL,1);
/*!40000 ALTER TABLE `blog` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `blogcomment`
--

DROP TABLE IF EXISTS `blogcomment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `blogcomment` (
  `CommentId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  `Detail` varchar(200) DEFAULT NULL,
  `BlogId` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  PRIMARY KEY (`CommentId`),
  KEY `FK_BlogComment_Blog` (`BlogId`),
  CONSTRAINT `FK_BlogComment_Blog` FOREIGN KEY (`BlogId`) REFERENCES `blog` (`BlogId`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `blogcomment`
--

LOCK TABLES `blogcomment` WRITE;
/*!40000 ALTER TABLE `blogcomment` DISABLE KEYS */;
INSERT INTO `blogcomment` VALUES (2,'nguyễn Hồng quân','Phone','quan@gmail.com','2014-11-22 00:00:00','this is blog details',3,1);
/*!40000 ALTER TABLE `blogcomment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `carimages`
--

DROP TABLE IF EXISTS `carimages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `carimages` (
  `ImageID` int(11) NOT NULL AUTO_INCREMENT,
  `Image1` longtext DEFAULT NULL,
  `CarID` int(11) DEFAULT NULL,
  `Image2` longtext DEFAULT NULL,
  `Image3` longtext DEFAULT NULL,
  `Image4` longtext DEFAULT NULL,
  PRIMARY KEY (`ImageID`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `carimages`
--

LOCK TABLES `carimages` WRITE;
/*!40000 ALTER TABLE `carimages` DISABLE KEYS */;
INSERT INTO `carimages` VALUES (1,'/files/Blog/blog2-2_1.jpg',14,NULL,NULL,NULL),(2,'/files/Blog/blog1.jpeg',15,NULL,NULL,NULL),(3,'/files/Blog/blog1.jpeg',16,NULL,NULL,NULL),(6,'/files/Carimage/car_img2.png',17,NULL,NULL,NULL),(7,'/files/Carimage/car_img3.png',18,NULL,NULL,NULL),(8,'/files/Blog/blog2-2_1.jpg',19,NULL,NULL,NULL);
/*!40000 ALTER TABLE `carimages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `carrentalorders`
--

DROP TABLE IF EXISTS `carrentalorders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `carrentalorders` (
  `OrderID` int(11) NOT NULL AUTO_INCREMENT,
  `CustomerID` int(11) NOT NULL,
  `OrderDate` date DEFAULT NULL,
  `Deposit` decimal(18,2) DEFAULT NULL,
  `Payment` decimal(18,2) DEFAULT NULL,
  `StatusID` int(11) DEFAULT NULL,
  `ReturnDate` date DEFAULT NULL,
  `Notes` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`OrderID`),
  KEY `FK_CarRentalOrders_Customer` (`CustomerID`),
  KEY `FK_CarRentalOrders_Status` (`StatusID`),
  CONSTRAINT `FK_CarRentalOrders_Customer` FOREIGN KEY (`CustomerID`) REFERENCES `customers` (`CustomerID`),
  CONSTRAINT `FK_CarRentalOrders_Status` FOREIGN KEY (`StatusID`) REFERENCES `orderstatus` (`StatusID`),
  CONSTRAINT `FK__CarRental__Custo__2D27B809` FOREIGN KEY (`CustomerID`) REFERENCES `customers` (`CustomerID`),
  CONSTRAINT `FK__CarRental__Statu__2E1BDC42` FOREIGN KEY (`StatusID`) REFERENCES `orderstatus` (`StatusID`)
) ENGINE=InnoDB AUTO_INCREMENT=1044 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `carrentalorders`
--

LOCK TABLES `carrentalorders` WRITE;
/*!40000 ALTER TABLE `carrentalorders` DISABLE KEYS */;
INSERT INTO `carrentalorders` VALUES (1030,1,'2025-12-21',240000.00,560000.00,2,NULL,''),(1031,6,'2025-12-21',120.00,280.00,2,NULL,'th'),(1032,6,'2025-12-21',1500.00,3500.00,2,NULL,'ưg'),(1033,5,'2025-12-21',18000.00,42000.00,2,NULL,''),(1034,5,'2025-12-22',9000.00,21000.00,2,NULL,''),(1035,5,'2025-12-22',60000.00,140000.00,1,NULL,''),(1036,1,'2025-12-22',96.90,226.10,2,NULL,''),(1037,1017,'2025-12-22',193.80,452.20,2,NULL,''),(1038,1017,'2025-12-22',96.90,226.10,2,NULL,''),(1039,1017,'2025-12-22',290.70,678.30,1,NULL,''),(1040,1,'2025-12-22',3000.00,7000.00,1,NULL,''),(1041,1,'2025-12-22',720.00,1680.00,1,NULL,''),(1042,8,'2025-12-22',90000.00,210000.00,1,NULL,''),(1043,1020,'2026-10-16',390000.00,910000.00,1,NULL,'Hình thức: Tự lái');
/*!40000 ALTER TABLE `carrentalorders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `carreviews`
--

DROP TABLE IF EXISTS `carreviews`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `carreviews` (
  `CarReviewID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  `Detail` varchar(200) DEFAULT NULL,
  `CarID` int(11) DEFAULT NULL,
  `Star` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`CarReviewID`),
  KEY `FK_CarReviews_Cars` (`CarID`),
  CONSTRAINT `FK_CarReviews_Cars` FOREIGN KEY (`CarID`) REFERENCES `cars` (`CarID`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `carreviews`
--

LOCK TABLES `carreviews` WRITE;
/*!40000 ALTER TABLE `carreviews` DISABLE KEYS */;
INSERT INTO `carreviews` VALUES (1,'NQuân','1234','Q@gmail.com','2024-12-22 00:00:00','sản phẩm tốt',1,5,1),(2,'Nthanh','123','thnh@gmai.com','2023-12-22 00:00:00','xe thơm',1,3,1),(3,'quan',NULL,NULL,'2024-12-02 16:09:09','123',1,NULL,NULL),(9,'ef',NULL,NULL,'2024-12-02 19:58:47','ưqđư',13,2,NULL),(10,'quan',NULL,'quan@gmail.com','2024-12-02 20:09:08','sản phẩm 100 điểm',13,0,NULL),(11,'q',NULL,'q','2024-12-02 20:10:43','q',13,5,NULL),(12,'đư',NULL,'dưqdư','2024-12-02 20:12:43','qđư',12,4,NULL),(13,'ưđ',NULL,'ưdqưd','2024-12-02 20:14:37','qưđ',12,0,NULL),(14,'dfvdfv',NULL,'vdfvdf','2024-12-02 20:14:49','vvdf',12,0,NULL),(15,'dfdf',NULL,'fsdf','2024-12-02 20:15:51','sdfsdf',12,5,NULL),(16,'q',NULL,'q','2024-12-02 20:37:49','q',13,0,NULL),(17,'nguyễn hồng quân',NULL,'nguyenquandc37@gmail.com','2024-12-02 20:38:27','bình luận thật là có đầu óc',13,0,NULL),(18,'ưdf',NULL,'fwe','2024-12-02 20:38:41','few',13,0,NULL),(19,'tung',NULL,'tung@gmail.com','2024-12-07 19:54:59','xe sạch sẽ ,xe quay lại thuê nấu cần',11,5,NULL),(20,'hanh',NULL,'hanh@gmail.com','2024-12-14 12:03:37','hanh123',11,0,NULL),(21,'1',NULL,'1','2024-12-14 12:11:57','1',11,0,NULL),(22,'2',NULL,'2','2024-12-14 12:15:31','2',11,0,NULL),(23,'3',NULL,'3','2024-12-14 12:15:58','3',11,0,NULL),(24,'binh',NULL,'binh@gmail.com','2024-12-14 13:01:28','binh',9,5,NULL),(25,'thuy',NULL,'thuy@gmail.com','2024-12-14 13:32:32','xe thom tho sach se',10,0,NULL);
/*!40000 ALTER TABLE `carreviews` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cars`
--

DROP TABLE IF EXISTS `cars`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cars` (
  `CarID` int(11) NOT NULL AUTO_INCREMENT,
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
  PRIMARY KEY (`CarID`),
  KEY `FK_Cars_CarTypes` (`TypeID`),
  KEY `idx_cars_owner` (`OwnerID`),
  CONSTRAINT `FK_Cars_CarTypes` FOREIGN KEY (`TypeID`) REFERENCES `cartypes` (`TypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cars`
--

LOCK TABLES `cars` WRITE;
/*!40000 ALTER TABLE `cars` DISABLE KEYS */;
INSERT INTO `cars` VALUES (1,'VinFast VF 5 Plus',5,'30A-952.18',650000.00,'Đỏ nóc trắng',2024,4.9,'VinFast','/images/cars/car-mini.jpg',NULL,'vinfast-vf-5-plus',0,1,'Tự động · Điện · Cầu Giấy, Hà Nội · 86 đánh giá','Xe điện',1,6,'Tự động','Điện','Cầu Giấy, Hà Nội',86,'Xe điện','Đang bận'),(2,'Toyota Vios G 2023',5,'51K-834.12',850000.00,'Trắng ngọc trai',2023,4.8,'Toyota','/images/cars/car-sedan.jpg',NULL,'toyota-vios-g-2023',0,1,'Tự động · Xăng · Quận 7, TP. Hồ Chí Minh · 124 đánh giá','Nổi bật',2,8,'Tự động','Xăng','Quận 7, TP. Hồ Chí Minh',124,'Nổi bật','Sẵn sàng'),(3,'Hyundai Santa Fe 2024',7,'43A-688.99',1850000.00,'Xanh đen',2024,4.7,'Hyundai','/images/cars/car-suv.jpg',1650000.00,'hyundai-santa-fe-2024',1,1,'Tự động · Dầu · Hải Châu, Đà Nẵng · 58 đánh giá','Ưu đãi',3,24,'Tự động','Dầu','Hải Châu, Đà Nẵng',58,'Ưu đãi','Sẵn sàng'),(4,'Mazda 3 Luxury',5,'30H-456.78',950000.00,'Trắng',2023,4.8,'Mazda','/images/cars/car-sedan.jpg',NULL,'mazda-3-luxury',0,1,'Tự động · Xăng · Đống Đa, Hà Nội · 72 đánh giá','Tiêu chuẩn',2,20,'Tự động','Xăng','Đống Đa, Hà Nội',72,NULL,'Sẵn sàng'),(5,'Kia Carnival Signature',7,'51L-999.88',2400000.00,'Xanh đen',2024,4.9,'Kia','/images/cars/car-suv.jpg',NULL,'kia-carnival-signature',0,1,'Tự động · Dầu · Quận 1, TP. Hồ Chí Minh · 41 đánh giá','Nổi bật',4,25,'Tự động','Dầu','Quận 1, TP. Hồ Chí Minh',41,'Nổi bật','Sẵn sàng'),(6,'Ford Ranger Wildtrak',5,'49C-333.21',1400000.00,'Cam Cyber',2023,4.6,'Ford','/images/cars/car-suv.jpg',NULL,'ford-ranger-wildtrak',0,1,'Tự động · Dầu · Phường 1, Đà Lạt · 33 đánh giá','Tiêu chuẩn',5,24,'Tự động','Dầu','Phường 1, Đà Lạt',33,NULL,'Bảo trì'),(7,'Honda City RS',5,'79A-567.89',950000.00,'Trắng',2023,4.7,'Honda','/images/cars/car-sedan.jpg',800000.00,'honda-city-rs',1,1,'Tự động · Xăng · Lộc Thọ, Nha Trang · 65 đánh giá','Ưu đãi',2,6,'Tự động','Xăng','Lộc Thọ, Nha Trang',65,'Ưu đãi','Sẵn sàng'),(8,'Toyota Fortuner Legender',7,'30F-777.66',1500000.00,'Bạc',2023,4.5,'Toyota','/images/cars/car-suv.jpg',NULL,'toyota-fortuner-legender',0,1,'Số sàn · Dầu · Tây Hồ, Hà Nội · 27 đánh giá','Tiêu chuẩn',3,8,'Số sàn','Dầu','Tây Hồ, Hà Nội',27,NULL,'Sẵn sàng');
/*!40000 ALTER TABLE `cars` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cartypes`
--

DROP TABLE IF EXISTS `cartypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cartypes` (
  `TypeID` int(11) NOT NULL AUTO_INCREMENT,
  `CarTypeName` varchar(100) DEFAULT NULL,
  `Seats` int(11) DEFAULT NULL,
  `Manufacturer` varchar(100) DEFAULT NULL,
  `Image` longtext DEFAULT NULL,
  PRIMARY KEY (`TypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cartypes`
--

LOCK TABLES `cartypes` WRITE;
/*!40000 ALTER TABLE `cartypes` DISABLE KEYS */;
INSERT INTO `cartypes` VALUES (1,'Xe mini đô thị',4,'VinFast, Kia','/images/cars/car-mini.jpg'),(2,'Sedan 5 chỗ',5,'Toyota, Mazda, Honda','/images/cars/car-sedan.jpg'),(3,'Crossover/SUV 7 chỗ',7,'Hyundai, Toyota, Ford','/images/cars/car-suv.jpg'),(4,'MPV đa dụng',7,'Kia, Mitsubishi','/images/cars/car-suv.jpg'),(5,'Xe bán tải',5,'Ford, Toyota','/images/cars/car-suv.jpg');
/*!40000 ALTER TABLE `cartypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chatmessages`
--

DROP TABLE IF EXISTS `chatmessages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chatmessages` (
  `MessageId` int(11) NOT NULL AUTO_INCREMENT,
  `CarId` int(11) DEFAULT NULL,
  `SenderId` int(11) DEFAULT NULL,
  `SenderName` varchar(100) NOT NULL DEFAULT 'Khách hàng',
  `SenderRole` varchar(50) NOT NULL DEFAULT 'Customer',
  `ReceiverId` int(11) DEFAULT NULL,
  `ReceiverName` varchar(100) DEFAULT NULL,
  `MessageText` text NOT NULL,
  `SentAt` datetime NOT NULL DEFAULT current_timestamp(),
  `IsRead` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`MessageId`),
  KEY `idx_chat_car` (`CarId`),
  KEY `idx_chat_receiver` (`ReceiverId`),
  KEY `idx_chat_sender` (`SenderId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chatmessages`
--

LOCK TABLES `chatmessages` WRITE;
/*!40000 ALTER TABLE `chatmessages` DISABLE KEYS */;
INSERT INTO `chatmessages` VALUES (1,1,6,'Trần Quốc Bảo','CarOwner',NULL,'Khách hàng','Chào bạn! Cảm ơn bạn đã quan tâm đến mẫu xe VinFast VF 5 Plus. Xe hiện đang có khách thuê, dự kiến về vào sáng mai. Bạn cần tư vấn lịch thuê từ ngày nào ạ?','2026-10-16 20:55:38',1),(2,2,8,'Nguyễn Minh Anh','CarOwner',NULL,'Khách hàng','Chào bạn! Chiếc Toyota Vios G 2023 bên mình luôn sẵn sàng, xe mới bảo dưỡng sạch sẽ, giao nhận tận nơi. Bạn cần nhận xe ở khu vực nào?','2026-10-16 21:55:38',1),(3,4,20,'Lê Thu Hà','CarOwner',NULL,'Khách hàng','Dạ chào anh/chị, Mazda 3 Luxury bên em đã sẵn sàng phục vụ. Xe chạy cực êm và tiết kiệm xăng. Anh/chị dự kiến thuê mấy ngày ạ?','2026-10-16 22:25:38',1),(4,3,NULL,'Khách hàng','Customer',24,'Phạm Hoàng Nam','Cho mình hỏi thủ tục nhận xe cần những gì vậy chủ xe?','2026-10-16 23:07:44',0),(5,3,24,'Phạm Hoàng Nam','CarOwner',NULL,'Khách hàng','Phạm Hoàng Nam: Dạ mình có hỗ trợ giao nhận xe tận nơi tại các quận nội thành hoặc bạn có thể qua trực tiếp bãi xe để nhận xe nha.','2026-10-16 23:07:45',0);
/*!40000 ALTER TABLE `chatmessages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contact`
--

DROP TABLE IF EXISTS `contact`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `contact` (
  `ContactId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(150) DEFAULT NULL,
  `Message` longtext DEFAULT NULL,
  `IsRead` int(11) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  `CreatedBy` varchar(150) DEFAULT NULL,
  `ModifiedDate` datetime DEFAULT NULL,
  `ModifiedBy` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`ContactId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contact`
--

LOCK TABLES `contact` WRITE;
/*!40000 ALTER TABLE `contact` DISABLE KEYS */;
INSERT INTO `contact` VALUES (1,'quan','123',NULL,'123',NULL,'2024-12-02 12:21:50',NULL,NULL,NULL),(2,'hanh','1234',NULL,'1244',NULL,'2024-12-02 12:24:18',NULL,NULL,NULL),(3,'quan','03534682',NULL,'liên hệ với tôi ngay',NULL,'2024-12-02 14:21:59',NULL,NULL,NULL);
/*!40000 ALTER TABLE `contact` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contract`
--

DROP TABLE IF EXISTS `contract`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `contract` (
  `ContractID` int(11) NOT NULL AUTO_INCREMENT,
  `CustomerID` int(11) NOT NULL,
  `Date` date DEFAULT NULL,
  `Content` longtext DEFAULT NULL,
  `GeneralTerms` longtext DEFAULT NULL,
  `SpecificTerms` longtext DEFAULT NULL,
  `AdvancePayment` decimal(18,2) DEFAULT NULL,
  PRIMARY KEY (`ContractID`),
  KEY `FK_Contract_Customer` (`CustomerID`),
  CONSTRAINT `FK_Contract_Customer` FOREIGN KEY (`CustomerID`) REFERENCES `customers` (`CustomerID`),
  CONSTRAINT `FK__Contracts__Custo__30F848ED` FOREIGN KEY (`CustomerID`) REFERENCES `customers` (`CustomerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contract`
--

LOCK TABLES `contract` WRITE;
/*!40000 ALTER TABLE `contract` DISABLE KEYS */;
/*!40000 ALTER TABLE `contract` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contractdetails`
--

DROP TABLE IF EXISTS `contractdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `contractdetails` (
  `ContractDetailID` int(11) NOT NULL AUTO_INCREMENT,
  `ContractID` int(11) NOT NULL,
  `CarTypeID` int(11) NOT NULL,
  `Quantity` int(11) DEFAULT NULL,
  `Price` decimal(18,2) DEFAULT NULL,
  `ReceiveDate` date DEFAULT NULL,
  `ReturnDate` date DEFAULT NULL,
  `Notes` longtext DEFAULT NULL,
  PRIMARY KEY (`ContractDetailID`),
  KEY `FK_ContractDetails_Contract` (`ContractID`),
  KEY `FK_ContractDetails_CarType` (`CarTypeID`),
  CONSTRAINT `FK_ContractDetails_CarType` FOREIGN KEY (`CarTypeID`) REFERENCES `cartypes` (`TypeID`),
  CONSTRAINT `FK_ContractDetails_Contract` FOREIGN KEY (`ContractID`) REFERENCES `contract` (`ContractID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contractdetails`
--

LOCK TABLES `contractdetails` WRITE;
/*!40000 ALTER TABLE `contractdetails` DISABLE KEYS */;
/*!40000 ALTER TABLE `contractdetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contractsettlementdetails`
--

DROP TABLE IF EXISTS `contractsettlementdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `contractsettlementdetails` (
  `SettlementDetailID` int(11) NOT NULL AUTO_INCREMENT,
  `SettlementID` int(11) NOT NULL,
  `CarID` int(11) NOT NULL,
  `ReceiveDate` date DEFAULT NULL,
  `ReturnDate` date DEFAULT NULL,
  `Price` decimal(18,2) DEFAULT NULL,
  `Total` decimal(18,2) DEFAULT NULL,
  PRIMARY KEY (`SettlementDetailID`),
  KEY `FK_ContractSettlementDetails_Settlement` (`SettlementID`),
  CONSTRAINT `FK_ContractSettlementDetails_Settlement` FOREIGN KEY (`SettlementID`) REFERENCES `contractsettlements` (`SettlementID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contractsettlementdetails`
--

LOCK TABLES `contractsettlementdetails` WRITE;
/*!40000 ALTER TABLE `contractsettlementdetails` DISABLE KEYS */;
/*!40000 ALTER TABLE `contractsettlementdetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contractsettlements`
--

DROP TABLE IF EXISTS `contractsettlements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `contractsettlements` (
  `SettlementID` int(11) NOT NULL AUTO_INCREMENT,
  `ContractID` int(11) NOT NULL,
  `Date` date DEFAULT NULL,
  `PaymentMethod` varchar(50) DEFAULT NULL,
  `TotalCarsRented` int(11) DEFAULT NULL,
  `AdvancePayment` decimal(18,2) DEFAULT NULL,
  `TotalPayment` decimal(18,2) DEFAULT NULL,
  `PaidAmount` decimal(18,2) DEFAULT NULL,
  `Notes` longtext DEFAULT NULL,
  PRIMARY KEY (`SettlementID`),
  KEY `FK_ContractSettlements_Contract` (`ContractID`),
  CONSTRAINT `FK_ContractSettlements_Contract` FOREIGN KEY (`ContractID`) REFERENCES `contract` (`ContractID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contractsettlements`
--

LOCK TABLES `contractsettlements` WRITE;
/*!40000 ALTER TABLE `contractsettlements` DISABLE KEYS */;
/*!40000 ALTER TABLE `contractsettlements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customers`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `customers` (
  `CustomerID` int(11) NOT NULL AUTO_INCREMENT,
  `FullName` varchar(50) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `Password` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
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
  PRIMARY KEY (`CustomerID`)
) ENGINE=InnoDB AUTO_INCREMENT=1027 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customers`
--

LOCK TABLES `customers` WRITE;
/*!40000 ALTER TABLE `customers` DISABLE KEYS */;
INSERT INTO `customers` VALUES (1,'bình','binh','d411cf68bd7f71d13fb053154f455651','binh@gmail.com','2025-12-21 00:00:00','2342','0123456789101','z7312564865026_72b1ba7dd1662b7e127ff0b18e5f4fc5_20251218235607389.jpg',NULL,'4234234',NULL,NULL,NULL,NULL),(2,NULL,'thuy','123','quan@gmail.com',NULL,'nghệ an',NULL,NULL,NULL,'12334',NULL,NULL,NULL,NULL),(3,'Đậu Công Quân','dauquan','d0afd43f169f68af6d56cf454d74b167','dauquan@gmail.com','2004-04-03 00:00:00','nghệ an','1123456789','A2_20251218114951560.jpg',NULL,'12334',NULL,NULL,NULL,NULL),(4,NULL,'thuy','d0afd43f169f68af6d56cf454d74b167','thuy@gmail.com',NULL,'nghệ an',NULL,NULL,NULL,'12334',NULL,NULL,NULL,NULL),(5,'Đậu Công Quân','quan','d0afd43f169f68af6d56cf454d74b167','quan123@gmail.com','2025-12-14 00:00:00','nghệ an, diễn châu, quyết thành1234567','121212321343','delete_20251221232537688.png',NULL,'12334',NULL,NULL,NULL,NULL),(6,'Nguyễn Hồng Quân','Nguyễn Quân 37dc','d0afd43f169f68af6d56cf454d74b167','Q@gmail.com','2004-01-12 00:00:00','Xóm Quyết Thành, Xã Diễn Bich, Huyển Diễn châu, tỉnh Nghệ An','123412341231','adc_20250108223225438.jpg',NULL,'0353675457',NULL,NULL,NULL,'0.00032029000144429.jpg'),(7,NULL,'Hoàng Thanh Huyền cute','ca3ea7a65775384c0aa2249769adb543','Huyen@gmail.com',NULL,'Thị xã Hoàng mai, Nghệ an',NULL,NULL,NULL,'0123345678',NULL,NULL,NULL,NULL),(8,'Doan thanh thuy','Đậu Công Quân','d0afd43f169f68af6d56cf454d74b167','Dau@gmailcom','2002-04-08 00:00:00','nghệ an, diễn châu, quyết thành1234567','123456789021','OIP_20251222214507362.jpg',NULL,'1234353445',NULL,NULL,NULL,NULL),(9,NULL,'Doãn thị thanh thuỷ','d0afd43f169f68af6d56cf454d74b167','T@gmail.com',NULL,'Nghi lộc nghệ an',NULL,NULL,NULL,'131243243',NULL,NULL,NULL,NULL),(10,NULL,'Minh Anh','d0afd43f169f68af6d56cf454d74b167','anh@gmail.com',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),(1010,'Nguyễn Hồng Quân',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),(1011,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),(1012,'Nguyễn Hồng Quân',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),(1013,'Nguyễn Hồng Quân',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),(1014,NULL,NULL,'d0afd43f169f68af6d56cf454d74b167','tt@gmail.com',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),(1015,NULL,'ẻgeergge','d0afd43f169f68af6d56cf454d74b167','t12@gmail.com',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),(1016,NULL,'binhtest1','d411cf68bd7f71d13fb053154f455651','bintest@gmail.com',NULL,'',NULL,NULL,NULL,'',NULL,NULL,NULL,NULL),(1017,'Bình Hidro','binhtest1','d411cf68bd7f71d13fb053154f455651','binhtest1@gmail.com','2025-12-22 00:00:00','Hoàng Mai, Hà Nội','11111111111','flutter-app-development-roadmap-by-tarikul_20251222140115648.png',NULL,'0339772881',NULL,NULL,NULL,NULL),(1018,'Doan thanh thuy','thủy','72631d8a190e188b4e116163d0c04ab5','thuy123@gmail.com','2004-11-08 00:00:00',NULL,'123456789012','get_20251222213520492.png',NULL,NULL,NULL,NULL,NULL,NULL),(1020,'Trần Kenn','Trần Kenn','d0afd43f169f68af6d56cf454d74b167','trankenn04@gmail.com','2004-10-10 00:00:00','Hà Nội','717254718131','images (1)_20261016002810148.jpg',NULL,'0987654321',NULL,NULL,NULL,NULL),(1021,NULL,'trankenn','d0afd43f169f68af6d56cf454d74b167','trankenn04hoyo@gmail.com',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),(1022,NULL,'Trần Quốc Bảo','d0afd43f169f68af6d56cf454d74b167','tranbao.carowner@gmail.com',NULL,'Cầu Giấy, Hà Nội',NULL,NULL,NULL,'0912.345.678',NULL,NULL,NULL,NULL),(1023,NULL,'Nguyễn Minh Anh','d0afd43f169f68af6d56cf454d74b167','minhanh.carrental@gmail.com',NULL,'Quận 7, TP.HCM',NULL,NULL,NULL,'0988.223.344',NULL,NULL,NULL,NULL),(1024,NULL,'Phạm Hoàng Nam','d0afd43f169f68af6d56cf454d74b167','hoangnam.suv@gmail.com',NULL,'Hải Châu, Đà Nẵng',NULL,NULL,NULL,'0977.889.900',NULL,NULL,NULL,NULL),(1025,NULL,'Lê Thu Hà','d0afd43f169f68af6d56cf454d74b167','thuha.luxurycars@gmail.com',NULL,'Đống Đa, Hà Nội',NULL,NULL,NULL,'0339.772.881',NULL,NULL,NULL,NULL),(1026,NULL,'Vũ Đình Dũng','d0afd43f169f68af6d56cf454d74b167','dinhdung.rentcar@gmail.com',NULL,'Quận 1, TP.HCM',NULL,NULL,NULL,'0905.112.233',NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu`
--

DROP TABLE IF EXISTS `menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `menu` (
  `MenuID` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(150) DEFAULT NULL,
  `Alias` varchar(150) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `Levels` int(11) DEFAULT NULL,
  `ParentId` int(11) DEFAULT NULL,
  `Position` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`MenuID`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu`
--

LOCK TABLES `menu` WRITE;
/*!40000 ALTER TABLE `menu` DISABLE KEYS */;
INSERT INTO `menu` VALUES (1,'Trang chủ','home',NULL,1,NULL,1,1),(2,'Sản phẩm','product',NULL,1,NULL,2,1),(3,'Blog','blog',NULL,1,NULL,3,1),(4,'Contact','contact',NULL,1,NULL,4,1),(6,'Cart','cart',NULL,1,NULL,5,1),(14,'New','news','tin tức',1,0,7,1);
/*!40000 ALTER TABLE `menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `news`
--

DROP TABLE IF EXISTS `news`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `news` (
  `NewsId` int(11) NOT NULL AUTO_INCREMENT,
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
  PRIMARY KEY (`NewsId`)
) ENGINE=InnoDB AUTO_INCREMENT=1003 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `news`
--

LOCK TABLES `news` WRITE;
/*!40000 ALTER TABLE `news` DISABLE KEYS */;
INSERT INTO `news` VALUES (2,'VINFAST THÔNG BÁO THAY ĐỔI TÊN GỌI PHIÊN BẢN CỦA MỘT SỐ DÒNG XE','vinfast-thong-bao-thay-doi-ten-goi-phien-ban-cua-mot-so-dong-xe',NULL,'Nhằm phù hợp hơn với định hướng phát triển sản phẩm và giá trị mà từng dòng xe mang lại, VinFast trân trọng thông báo việc thay đổi tên gọi một số phiên bản xe ô tô điện. Đây không chỉ là sự thay đổi về tên gọi, mà còn là bước chuyển mình trong chiến lược, cam kết mang đến những giá trị vượt trội trên hành trình kiến tạo tương lai xanh bền vững của VinFast.','<p><span style=\"color: rgb(60, 60, 60); font-family: Mulish, serif; font-style: italic; letter-spacing: -0.32px;\">Nhằm phù hợp hơn với định hướng phát triển sản phẩm và giá trị mà từng dòng xe mang lại, VinFast trân trọng thông báo việc thay đổi tên gọi một số phiên bản xe ô tô điện. Đây không chỉ là sự thay đổi về tên gọi, mà còn là bước chuyển mình trong chiến lược, cam kết mang đến những giá trị vượt trội trên hành trình kiến tạo tương lai xanh bền vững của VinFast.</span></p>','/files/News/xedo.jpg',NULL,NULL,NULL,'2025-01-12 14:59:33','thanh',NULL,NULL,1),(1002,'VINFAST THÔNG BÁO THAY ĐỔI TÊN GỌI PHIÊN BẢN CỦA MỘT SỐ DÒNG XE','vinfast-thong-bao-thay-doi-ten-goi-phien-ban-cua-mot-so-dong-xe',NULL,'NNNhằm phù hợp hơn với định hướng phát triển sản phẩm và giá trị mà từng dòng xe mang lại, VinFast trân trọng thông báo việc thay đổi tên gọi một số phiên bản xe ô tô điện. Đây không chỉ là sự thay đổi về tên gọi, mà còn là bước chuyển mình trong chiến lược, cam kết mang đến những giá trị vượt trội trên hành trình kiến tạo tương lai xanh bền vững của VinFast','<p><span style=\"color: rgb(60, 60, 60); font-family: Mulish, serif; font-style: italic; letter-spacing: -0.32px;\">Nhằm phù hợp hơn với định hướng phát triển sản phẩm và giá trị mà từng dòng xe mang lại, VinFast trân trọng thông báo việc thay đổi tên gọi một số phiên bản xe ô tô điện. Đây không chỉ là sự thay đổi về tên gọi, mà còn là bước chuyển mình trong chiến lược, cam kết mang đến những giá trị vượt trội trên hành trình kiến tạo tương lai xanh bền vững của VinFast.</span></p><p style=\"margin-right: 0px; margin-bottom: 10px; margin-left: 0px; color: rgb(60, 60, 60); font-family: Mulish, serif; letter-spacing: -0.32px;\"><span style=\"font-size: 12pt;\"><span style=\"font-family: &quot;times new roman&quot;, serif;\"><span style=\"color: rgb(0, 0, 0);\">Theo đó, các phiên bản sẽ có sự thay đổi tên bao gồm: VinFast VF 6 S, VinFast VF 7 S, VinFast VF 8 S Lux, VinFast VF 8 Lux Plus, VinFast VF e34. Chi tiết thay đổi như sau:</span></span></span></p><ol style=\"list-style: initial; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; padding: 0px 0px 0px 15px; color: rgb(60, 60, 60); font-family: Mulish, serif; letter-spacing: -0.32px;\"><li><span style=\"font-size: 12pt;\"><span style=\"font-family: &quot;times new roman&quot;, serif;\"><span style=\"color: rgb(0, 0, 0);\">VF 6 S đổi tên thành VF 6 Eco</span></span></span></li></ol><p style=\"margin-right: 0px; margin-bottom: 10px; margin-left: 0px; color: rgb(60, 60, 60); font-family: Mulish, serif; letter-spacing: -0.32px;\"><span style=\"font-size: 12pt;\"><span style=\"font-family: &quot;times new roman&quot;, serif;\"><span style=\"color: rgb(0, 0, 0);\">VinFast VF 6 S - Lựa chọn phù hợp cho gia đình trẻ hoặc cá nhân yêu thích sự tiện nghi với mức giá hợp lý nhất phân khúc xe gầm cao hạng B, sẽ đổi tên thành VinFast VF 6 Eco. Mẫu xe với thiết kế nội thất lấy cảm hứng từ ngôi nhà thứ hai của gia đình cùng không gian rộng rãi, đáp ứng nhu cầu sử dụng hằng ngày mà vẫn tối ưu về chi phí vận hành.</span></span></span></p><ol start=\"2\" style=\"list-style: initial; margin-right: 0px; margin-bottom: 0px; margin-left: 0px; padding: 0px 0px 0px 15px; color: rgb(60, 60, 60); font-family: Mulish, serif; letter-spacing: -0.32px;\"><li><span style=\"font-size: 12pt;\"><span style=\"font-family: &quot;times new roman&quot;, serif;\"><span style=\"color: rgb(0, 0, 0);\">VF 7 S đổi tên thành VF 7 Eco&nbsp;</span></span></span></li></ol><p style=\"margin-right: 0px; margin-bottom: 10px; margin-left: 0px; color: rgb(60, 60, 60); font-family: Mulish, serif; letter-spacing: -0.32px;\"><span style=\"font-size: 12pt;\"><span style=\"font-family: &quot;times new roman&quot;, serif;\"><span style=\"color: rgb(0, 0, 0);\">VinFast VF 7 S – Mẫu SUV hạng C đẳng cấp, với thiết kế sắc sảo, mạnh mẽ sẽ được đổi tên thành VinFast VF 7 Eco. Chiếc xe được trang bị hiệu suất vận hành vượt trội, cùng hệ thống an toàn tối đa, mang lại trải nghiệm lái đầy thú vị trên mọi nẻo đường bạn đi tới.&nbsp;</span></span></span></p>','/files/Blog/blog2-1_1.jpg',NULL,NULL,NULL,NULL,NULL,'2025-12-18 10:37:11','thanh',1);
/*!40000 ALTER TABLE `news` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `newscomment`
--

DROP TABLE IF EXISTS `newscomment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `newscomment` (
  `CommentId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) DEFAULT NULL,
  `Phone` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  `Detail` varchar(200) DEFAULT NULL,
  `NewsId` int(11) DEFAULT NULL,
  `isActive` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`CommentId`),
  KEY `FK_NewsComment_News` (`NewsId`),
  CONSTRAINT `FK_NewsComment_News` FOREIGN KEY (`NewsId`) REFERENCES `news` (`NewsId`)
) ENGINE=InnoDB AUTO_INCREMENT=1003 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `newscomment`
--

LOCK TABLES `newscomment` WRITE;
/*!40000 ALTER TABLE `newscomment` DISABLE KEYS */;
INSERT INTO `newscomment` VALUES (2,'thuy',NULL,'thuy@gjt','2025-01-12 15:00:31','tgsdgvhdm',2,NULL),(1002,'tươi',NULL,'heo@tuoi','2025-01-13 12:02:40','fyuk',2,NULL);
/*!40000 ALTER TABLE `newscomment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orderdetails`
--

DROP TABLE IF EXISTS `orderdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `orderdetails` (
  `OrderDetailID` int(11) NOT NULL AUTO_INCREMENT,
  `OrderID` int(11) NOT NULL,
  `CarID` int(11) NOT NULL,
  `Description` longtext DEFAULT NULL,
  `Price` decimal(18,2) DEFAULT NULL,
  `Quantity` int(11) DEFAULT NULL,
  `PickupDate` datetime DEFAULT NULL,
  `ReturnDate` datetime DEFAULT NULL,
  PRIMARY KEY (`OrderDetailID`),
  KEY `FK_OrderDetails_Order` (`OrderID`),
  CONSTRAINT `FK_OrderDetails_Order` FOREIGN KEY (`OrderID`) REFERENCES `carrentalorders` (`OrderID`)
) ENGINE=InnoDB AUTO_INCREMENT=1051 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orderdetails`
--

LOCK TABLES `orderdetails` WRITE;
/*!40000 ALTER TABLE `orderdetails` DISABLE KEYS */;
INSERT INTO `orderdetails` VALUES (1037,1030,2,NULL,800000.00,2,'2025-12-21 00:00:00','2025-12-22 00:00:00'),(1038,1031,14,NULL,400.00,1,'2025-12-21 00:00:00','2025-12-23 00:00:00'),(1039,1032,15,NULL,5000.00,1,'2025-12-26 00:00:00','2025-12-31 00:00:00'),(1040,1033,16,NULL,60000.00,1,'2025-12-21 00:00:00','2025-12-23 00:00:00'),(1041,1034,16,NULL,30000.00,1,'2025-12-22 00:00:00','2025-12-23 00:00:00'),(1042,1035,10,NULL,200000.00,1,'2025-12-22 00:00:00','2025-12-23 00:00:00'),(1043,1036,17,NULL,323.00,1,'2025-12-22 00:00:00','2025-12-23 00:00:00'),(1044,1037,18,NULL,646.00,1,'2025-12-22 00:00:00','2025-12-24 00:00:00'),(1045,1038,18,NULL,323.00,1,'2025-12-22 00:00:00','2025-12-23 00:00:00'),(1046,1039,18,NULL,969.00,1,'2025-12-22 00:00:00','2025-12-25 00:00:00'),(1047,1040,15,NULL,10000.00,1,'2025-12-22 00:00:00','2026-01-01 00:00:00'),(1048,1041,14,NULL,2400.00,2,'2025-12-22 00:00:00','2025-12-25 00:00:00'),(1049,1042,19,NULL,300000.00,1,'2025-12-25 00:00:00','2025-12-26 00:00:00'),(1050,1043,1,NULL,1300000.00,1,'2026-10-16 00:00:00','2026-10-18 00:00:00');
/*!40000 ALTER TABLE `orderdetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orderstatus`
--

DROP TABLE IF EXISTS `orderstatus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `orderstatus` (
  `StatusID` int(11) NOT NULL AUTO_INCREMENT,
  `StatusDescription` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`StatusID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orderstatus`
--

LOCK TABLES `orderstatus` WRITE;
/*!40000 ALTER TABLE `orderstatus` DISABLE KEYS */;
INSERT INTO `orderstatus` VALUES (1,' Đã đặt'),(2,'Đã Huỷ'),(3,'Đã thanh toán'),(4,'Đang giao'),(5,'Hoàn tất');
/*!40000 ALTER TABLE `orderstatus` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role`
--

DROP TABLE IF EXISTS `role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `role` (
  `RoleId` int(11) NOT NULL,
  `RoleName` varchar(50) DEFAULT NULL,
  `Description` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role`
--

LOCK TABLES `role` WRITE;
/*!40000 ALTER TABLE `role` DISABLE KEYS */;
INSERT INTO `role` VALUES (1,'Admin','quanli'),(2,'guest','khách'),(3,'Chủ xe','Đối tác chủ xe cung cấp phương tiện cho hệ thống R');
/*!40000 ALTER TABLE `role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `UserID` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `Password` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `amenities`
--

DROP TABLE IF EXISTS `amenities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `amenities` (
  `AmenityId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `Icon` varchar(100) NOT NULL DEFAULT 'bi-check-circle',
  `IconColor` varchar(50) DEFAULT 'text-primary',
  `DisplayOrder` int(11) NOT NULL DEFAULT 0,
  `IsActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`AmenityId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `amenities`
--

LOCK TABLES `amenities` WRITE;
/*!40000 ALTER TABLE `amenities` DISABLE KEYS */;
INSERT INTO `amenities` (`AmenityId`, `Name`, `Icon`, `IconColor`, `DisplayOrder`, `IsActive`) VALUES
(1,'Bản đồ định vị GPS','bi-geo-alt-fill','text-danger',1,1),
(2,'Camera lùi / Cảm biến va chạm','bi-camera-video-fill','text-primary',2,1),
(3,'Camera hành trình HD','bi-webcam-fill','text-info',3,1),
(4,'Kết nối Bluetooth & Apple CarPlay','bi-bluetooth','text-primary',4,1),
(5,'Cửa sổ trời toàn cảnh (Panorama)','bi-brightness-high-fill','text-warning',5,1),
(6,'Sạc điện thoại không dây Qi','bi-battery-charging','text-success',6,1),
(7,'Cảm biến áp suất lốp (TPMS)','bi-speedometer2','text-danger',7,1),
(8,'Hệ thống túi khí an toàn 360','bi-shield-fill-check','text-success',8,1),
(9,'Hệ thống phanh ABS / Cân bằng ESC','bi-shield-shaded','text-secondary',9,1),
(10,'Ghế bọc da chỉnh điện','bi-car-front-fill','text-dark',10,1),
(11,'Cổng sạc nhanh USB / Type-C','bi-usb-symbol','text-info',11,1),
(12,'Lốp xe dự phòng & Bộ cứu hộ','bi-tools','text-warning',12,1),
(13,'Cốp mở điện / Đá cốp thông minh','bi-box-seam-fill','text-primary',13,1),
(14,'Thiết bị thu phí tự động ETC (VETC/ePass)','bi-tag-fill','text-success',14,1);
/*!40000 ALTER TABLE `amenities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `car_amenities`
--

DROP TABLE IF EXISTS `car_amenities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `car_amenities` (
  `CarId` int(11) NOT NULL,
  `AmenityId` int(11) NOT NULL,
  PRIMARY KEY (`CarId`,`AmenityId`),
  KEY `FK_car_amenities_amenity` (`AmenityId`),
  CONSTRAINT `FK_car_amenities_car` FOREIGN KEY (`CarId`) REFERENCES `cars` (`CarID`) ON DELETE CASCADE,
  CONSTRAINT `FK_car_amenities_amenity` FOREIGN KEY (`AmenityId`) REFERENCES `amenities` (`AmenityId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-10-28

