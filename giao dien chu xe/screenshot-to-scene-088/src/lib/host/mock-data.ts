// DỮ LIỆU MẪU — chỉ phục vụ trình diễn prototype.
// Bản tích hợp thật: lấy từ ASP.NET Core MVC (MySQL) qua các endpoint trong api.ts.
import vios from "@/assets/car-vios.jpg";
import vf8 from "@/assets/car-vf8.jpg";
import xpander from "@/assets/car-xpander.jpg";
import cx5 from "@/assets/car-cx5.jpg";
import type { Car, HostNotification, Order, OwnerProfile, Review } from "./types";

export const DEMO_TODAY = "2026-10-28";
export const CURRENT_OWNER_ID = 1;

export const owner: OwnerProfile = {
  id: 1,
  fullName: "Trần Minh Khoa",
  initials: "MK",
  verified: "approved",
  trusted: false, // computed from rules.isTrustedHost in the layout
  idNumber: "079095012345",
  licenseNumber: "790123456789",
  licenseClass: "B2",
  submittedAt: "2026-03-02T09:15",
  bankName: "Vietcombank",
  bankAccount: "0071000123456",
  bankHolder: "TRAN MINH KHOA",
};

const base = {
  amenities: ["Bản đồ", "Bluetooth", "Camera lùi", "Điều hòa"],
  kmLimitPerDay: 300,
  overKmFee: 3000,
  delivery: true,
  deliveryFee: 150000,
  hasTransactions: true,
};

export const cars: Car[] = [
  { ...base, id: 101, ownerId: 1, name: "Toyota Vios G 2023", brand: "Toyota", segment: "Sedan hạng B", year: 2023, plate: "51K-238.45", color: "Trắng", seats: 5, transmission: "Tự động", fuel: "Xăng", consumption: "5.8L/100km", pricePerDay: 750000, salePrice: 690000, saleEnabled: true, location: "Quận 7, TP.HCM", completedTrips: 12, status: "busy", image: vios, createdAt: "2026-03-10" },
  { ...base, id: 102, ownerId: 1, name: "VinFast VF8 Plus", brand: "VinFast", segment: "SUV hạng D", year: 2024, plate: "51L-902.17", color: "Xanh dương", seats: 5, transmission: "Tự động", fuel: "Điện", consumption: "18 kWh/100km", pricePerDay: 1450000, salePrice: null, saleEnabled: false, location: "Thủ Đức, TP.HCM", completedTrips: 7, status: "available", image: vf8, amenities: [...base.amenities, "Cửa sổ trời", "Cảm biến lốp", "Camera hành trình"], createdAt: "2026-04-22" },
  { ...base, id: 103, ownerId: 1, name: "Mitsubishi Xpander AT", brand: "Mitsubishi", segment: "MPV 7 chỗ", year: 2022, plate: "51H-551.08", color: "Bạc", seats: 7, transmission: "Tự động", fuel: "Xăng", consumption: "6.9L/100km", pricePerDay: 900000, salePrice: 820000, saleEnabled: false, location: "Bình Thạnh, TP.HCM", completedTrips: 5, status: "available", image: xpander, delivery: false, deliveryFee: 0, createdAt: "2026-05-15" },
  { ...base, id: 104, ownerId: 1, name: "Mazda CX-5 Premium", brand: "Mazda", segment: "SUV hạng C", year: 2024, plate: "51K-774.32", color: "Đỏ", seats: 5, transmission: "Tự động", fuel: "Xăng", consumption: "7.2L/100km", pricePerDay: 1200000, salePrice: null, saleEnabled: false, location: "Quận 1, TP.HCM", completedTrips: 0, status: "pending", image: cx5, hasTransactions: false, createdAt: "2026-10-25" },
  { ...base, id: 105, ownerId: 1, name: "Toyota Vios E 2021", brand: "Toyota", segment: "Sedan hạng B", year: 2021, plate: "51G-118.90", color: "Trắng", seats: 5, transmission: "Số sàn", fuel: "Xăng", consumption: "6.1L/100km", pricePerDay: 600000, salePrice: null, saleEnabled: false, location: "Gò Vấp, TP.HCM", completedTrips: 0, status: "rejected", rejectReason: "Ảnh đăng kiểm bị mờ, không đọc được ngày hết hạn. Vui lòng chụp lại rõ nét.", image: vios, hasTransactions: false, createdAt: "2026-10-12" },
  // Xe của chủ xe khác — KHÔNG BAO GIỜ hiển thị (minh họa lọc theo chủ sở hữu)
  { ...base, id: 900, ownerId: 2, name: "Honda City RS", brand: "Honda", segment: "Sedan hạng B", year: 2023, plate: "30A-000.00", color: "Xám", seats: 5, transmission: "Tự động", fuel: "Xăng", consumption: "5.6L/100km", pricePerDay: 700000, salePrice: null, saleEnabled: false, location: "Hà Nội", completedTrips: 30, status: "available", image: vios, createdAt: "2026-01-01" },
];

const g = (name: string, phone: string, email: string) => ({
  guestName: name, guestPhone: phone, guestEmail: email,
  guestIdNumber: "079199" + phone.slice(-6), guestLicense: "79" + phone.slice(-10),
});

const done = (id: string, carId: number, created: string, pick: string, ret: string, days: number, total: number, guest: ReturnType<typeof g>): Order => ({
  id, carId, createdAt: created, pickupAt: pick, returnAt: ret, days, total, depositConfirmed: true, status: 5, tripCounted: true, ...guest,
  history: [
    { at: created, label: "Khách đặt xe & thanh toán cọc 30%" },
    { at: pick, label: "Chủ xe bàn giao xe" },
    { at: ret, label: "Chủ xe nhận lại xe — Hoàn tất" },
  ],
});

export const orders: Order[] = [
  { id: "RC-24091", carId: 102, createdAt: "2026-10-27T20:14", pickupAt: "2026-10-30T08:00", returnAt: "2026-11-02T08:00", days: 3, total: 4350000, depositConfirmed: true, status: 3, tripCounted: false, ...g("Lê Thu Hà", "0903456789", "thuha.le@gmail.com"), history: [{ at: "2026-10-27T20:14", label: "Khách đặt xe & thanh toán cọc 30%" }] },
  { id: "RC-24088", carId: 103, createdAt: "2026-10-27T11:02", pickupAt: "2026-10-29T07:30", returnAt: "2026-10-31T19:00", days: 3, total: 2700000, depositConfirmed: false, status: 3, tripCounted: false, ...g("Phạm Quốc Bảo", "0912334455", "bao.pq@outlook.com"), history: [{ at: "2026-10-27T11:02", label: "Khách đặt xe — chờ xác nhận cọc" }] },
  { id: "RC-24075", carId: 101, createdAt: "2026-10-24T09:40", pickupAt: "2026-10-26T09:00", returnAt: "2026-10-29T09:00", days: 3, total: 2070000, depositConfirmed: true, status: 4, tripCounted: false, ...g("Nguyễn Hoàng Nam", "0987112233", "nam.nh@gmail.com"), history: [{ at: "2026-10-24T09:40", label: "Khách đặt xe & thanh toán cọc 30%" }, { at: "2026-10-26T09:05", label: "Chủ xe bàn giao xe" }] },
  { id: "RC-24060", carId: 102, createdAt: "2026-10-18T15:20", pickupAt: "2026-10-20T08:00", returnAt: "2026-10-22T08:00", days: 2, total: 2900000, depositConfirmed: true, status: 2, tripCounted: false, cancelReason: "Khách đổi lịch công tác.", ...g("Võ Thanh Tâm", "0938776655", "tam.vo@gmail.com"), history: [{ at: "2026-10-18T15:20", label: "Khách đặt xe & thanh toán cọc 30%" }, { at: "2026-10-19T10:00", label: "Đơn bị hủy", note: "Khách đổi lịch công tác." }] },
  done("RC-24031", 101, "2026-10-05T10:00", "2026-10-08T08:00", "2026-10-11T08:00", 3, 2070000, g("Đặng Mỹ Linh", "0909887766", "linh.dang@gmail.com")),
  done("RC-23990", 102, "2026-09-20T18:00", "2026-09-25T08:00", "2026-09-28T08:00", 3, 4350000, g("Huỳnh Gia Huy", "0977665544", "huy.huynh@gmail.com")),
  done("RC-23952", 103, "2026-09-02T09:00", "2026-09-05T07:00", "2026-09-07T19:00", 3, 2700000, g("Trương Ngọc Ánh", "0966554433", "anh.truong@gmail.com")),
  done("RC-23901", 101, "2026-08-10T12:00", "2026-08-14T08:00", "2026-08-18T08:00", 4, 2760000, g("Bùi Văn Lộc", "0944332211", "loc.bui@gmail.com")),
  done("RC-23877", 102, "2026-08-01T08:30", "2026-08-03T08:00", "2026-08-05T08:00", 2, 2900000, g("Lý Hải Yến", "0933221100", "yen.ly@gmail.com")),
  done("RC-23810", 101, "2026-07-11T14:00", "2026-07-15T08:00", "2026-07-17T08:00", 2, 1380000, g("Phan Đức Trí", "0922110099", "tri.phan@gmail.com")),
  done("RC-23766", 103, "2026-06-05T10:00", "2026-06-08T07:00", "2026-06-12T19:00", 5, 4500000, g("Ngô Kim Chi", "0911009988", "chi.ngo@gmail.com")),
  done("RC-23702", 101, "2026-05-12T10:00", "2026-05-16T08:00", "2026-05-18T08:00", 2, 1500000, g("Đỗ Minh Tuấn", "0900998877", "tuan.do@gmail.com")),
  // Đơn của chủ xe khác — bị lọc
  done("RC-99999", 900, "2026-10-01T10:00", "2026-10-02T08:00", "2026-10-03T08:00", 1, 700000, g("Người khác", "0900000000", "x@x.com")),
];

export const reviews: Review[] = [
  { id: 1, carId: 101, guest: "Đặng Mỹ Linh", stars: 5, comment: "Xe sạch, chủ xe nhiệt tình, giao đúng giờ.", date: "2026-10-11" },
  { id: 2, carId: 102, guest: "Huỳnh Gia Huy", stars: 5, comment: "VF8 chạy êm, sạc đầy khi nhận.", date: "2026-09-28" },
  { id: 3, carId: 103, guest: "Trương Ngọc Ánh", stars: 4, comment: "Xe rộng rãi, phù hợp gia đình. Điều hòa hơi yếu hàng sau.", date: "2026-09-07" },
  { id: 4, carId: 101, guest: "Bùi Văn Lộc", stars: 5, comment: "Thủ tục nhanh gọn.", date: "2026-08-18" },
  { id: 5, carId: 102, guest: "Lý Hải Yến", stars: 4, comment: "Ổn, sẽ thuê lại.", date: "2026-08-05" },
];

export const notifications: HostNotification[] = [
  { id: 1, title: "Đơn mới RC-24091", body: "Lê Thu Hà đặt VinFast VF8 Plus từ 30/10.", at: "2026-10-27T20:14", read: false, orderId: "RC-24091" },
  { id: 2, title: "Đơn mới RC-24088", body: "Phạm Quốc Bảo đặt Mitsubishi Xpander — chờ xác nhận cọc.", at: "2026-10-27T11:02", read: false, orderId: "RC-24088" },
  { id: 3, title: "Hồ sơ xe bị từ chối", body: "Toyota Vios E 2021: ảnh đăng kiểm bị mờ.", at: "2026-10-14T09:00", read: true },
];
