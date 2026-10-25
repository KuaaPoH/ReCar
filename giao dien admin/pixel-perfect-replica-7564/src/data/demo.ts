/**
 * DỮ LIỆU DEMO — chỉ dùng cho prototype giao diện.
 * Không phải dữ liệu thật trong MySQL. Khi tích hợp, thay bằng API của
 * ASP.NET Core (Areas/Admin/Controllers/*). Xem src/data/api.ts.
 */
import sedan from "@/assets/car-sedan.jpg";
import suv from "@/assets/car-suv.jpg";
import mpv from "@/assets/car-mpv.jpg";
import interior from "@/assets/car-interior.jpg";

export const IMAGES = { sedan, suv, mpv, interior };

export type CarStatus = "ready" | "rented" | "maintenance" | "pending" | "suspended" | "rejected";
export type Segment = "Sedan" | "SUV" | "MPV" | "Bán tải" | "Hatchback";

export interface Car {
  id: string;
  name: string;
  brand: string;
  year: number;
  plate: string;
  segment: Segment;
  seats: number;
  gearbox: "Số tự động" | "Số sàn";
  fuel: "Xăng" | "Dầu" | "Điện" | "Hybrid";
  pricePerDay: number;
  driverFee?: number;
  ownerId: string;
  city: string;
  handover: string;
  trips: number;
  status: CarStatus;
  submittedAt: string;
  docsVerified: boolean;
  image: string;
  gallery: string[];
  hasActiveOrder?: boolean;
  rejectReason?: string;
}

export interface Owner {
  id: string;
  name: string;
  email: string;
  phone: string;
  cars: number;
  trips: number;
  revenue: number;
  rating: number;
  profileStatus: "verified" | "pending" | "rejected";
  accountStatus: "active" | "locked";
  bank: { name: string; account: string; holder: string };
  createdAt: string;
}

export interface Customer {
  id: string;
  name: string;
  email: string;
  phone: string;
  registeredAt: string;
  orders: number;
  accountStatus: "active" | "locked";
  violations: number;
}

export interface Admin {
  id: string;
  name: string;
  email: string;
  role: "Super Admin" | "Kiểm duyệt viên" | "Kế toán" | "Biên tập viên";
  level: number;
  accountStatus: "active" | "locked";
  lastLogin: string;
}

export type OrderStatus =
  | "created" | "awaiting_deposit" | "deposited" | "awaiting_handover" | "renting"
  | "awaiting_completion" | "completed" | "cancelled" | "rejected" | "refunding" | "refunded";
export type PaymentStatus = "unpaid" | "deposit_paid" | "paid" | "refunding" | "refunded";

export interface Order {
  id: string;
  carId: string;
  customerId: string;
  pickup: string;
  dropoff: string;
  total: number;
  status: OrderStatus;
  payment: PaymentStatus;
  method: "VNPay" | "Chuyển khoản" | "MoMo";
  note?: string;
  cancelReason?: string;
  history: { status: OrderStatus; at: string; by: string; reason?: string }[];
}

export interface Settlement {
  id: string;
  ownerId: string;
  period: string;
  trips: number;
  gross: number;
  adjustment: number;
  status: "pending" | "processing" | "settled" | "discrepancy";
  settledAt?: string;
  ref?: string;
  orderIds: string[];
}

export interface Post {
  id: string;
  title: string;
  category: string;
  author: string;
  createdAt: string;
  updatedAt: string;
  status: "draft" | "published" | "hidden";
  cover: string;
  content: string;
}

export const owners: Owner[] = [
  { id: "O1", name: "Nguyễn Văn Hùng", email: "hung.nv@gmail.com", phone: "0903 245 118", cars: 6, trips: 142, revenue: 186_500_000, rating: 4.9, profileStatus: "verified", accountStatus: "active", bank: { name: "Vietcombank", account: "0071001234567", holder: "NGUYEN VAN HUNG" }, createdAt: "2025-03-12" },
  { id: "O2", name: "Công ty TNHH Xe Việt", email: "lienhe@xeviet.vn", phone: "028 3822 4455", cars: 14, trips: 318, revenue: 412_000_000, rating: 4.8, profileStatus: "verified", accountStatus: "active", bank: { name: "Techcombank", account: "19034567891011", holder: "CONG TY TNHH XE VIET" }, createdAt: "2024-11-02" },
  { id: "O3", name: "Trần Thị Mai", email: "mai.tran@outlook.com", phone: "0912 778 903", cars: 3, trips: 76, revenue: 92_300_000, rating: 4.7, profileStatus: "verified", accountStatus: "active", bank: { name: "MB Bank", account: "0350112233", holder: "TRAN THI MAI" }, createdAt: "2025-06-20" },
  { id: "O4", name: "Lê Quốc Bảo", email: "bao.le@gmail.com", phone: "0988 112 667", cars: 2, trips: 41, revenue: 58_900_000, rating: 4.6, profileStatus: "pending", accountStatus: "active", bank: { name: "ACB", account: "2468013579", holder: "LE QUOC BAO" }, createdAt: "2026-01-08" },
  { id: "O5", name: "Phạm Minh Tuấn", email: "tuanpm@yahoo.com", phone: "0934 556 210", cars: 4, trips: 97, revenue: 121_700_000, rating: 4.8, profileStatus: "verified", accountStatus: "locked", bank: { name: "BIDV", account: "31410001234", holder: "PHAM MINH TUAN" }, createdAt: "2025-01-15" },
];

const g = (main: string) => [main, IMAGES.interior, main === sedan ? suv : sedan, mpv];

export const cars: Car[] = [
  { id: "C1", name: "Toyota Vios G", brand: "Toyota", year: 2023, plate: "51K-382.19", segment: "Sedan", seats: 5, gearbox: "Số tự động", fuel: "Xăng", pricePerDay: 750_000, ownerId: "O1", city: "TP. Hồ Chí Minh", handover: "Quận 7, TP.HCM", trips: 48, status: "ready", submittedAt: "2026-05-02T09:12:00", docsVerified: true, image: sedan, gallery: g(sedan) },
  { id: "C2", name: "Mazda CX-5 Premium", brand: "Mazda", year: 2024, plate: "30K-738.24", segment: "SUV", seats: 5, gearbox: "Số tự động", fuel: "Xăng", pricePerDay: 1_200_000, driverFee: 400_000, ownerId: "O2", city: "Hà Nội", handover: "Cầu Giấy, Hà Nội", trips: 63, status: "rented", submittedAt: "2026-04-11T14:30:00", docsVerified: true, image: suv, gallery: g(suv), hasActiveOrder: true },
  { id: "C3", name: "Mitsubishi Xpander AT", brand: "Mitsubishi", year: 2022, plate: "43A-551.08", segment: "MPV", seats: 7, gearbox: "Số tự động", fuel: "Xăng", pricePerDay: 950_000, ownerId: "O3", city: "Đà Nẵng", handover: "Hải Châu, Đà Nẵng", trips: 37, status: "maintenance", submittedAt: "2026-03-28T08:00:00", docsVerified: true, image: mpv, gallery: g(mpv) },
  { id: "C4", name: "Toyota Vios E", brand: "Toyota", year: 2021, plate: "51H-927.44", segment: "Sedan", seats: 5, gearbox: "Số sàn", fuel: "Xăng", pricePerDay: 620_000, ownerId: "O2", city: "TP. Hồ Chí Minh", handover: "Tân Bình, TP.HCM", trips: 81, status: "rented", submittedAt: "2025-12-01T10:00:00", docsVerified: true, image: sedan, gallery: g(sedan), hasActiveOrder: true },
  { id: "C5", name: "Mazda CX-5 Luxury", brand: "Mazda", year: 2023, plate: "29A-104.66", segment: "SUV", seats: 5, gearbox: "Số tự động", fuel: "Xăng", pricePerDay: 1_100_000, ownerId: "O5", city: "Hà Nội", handover: "Đống Đa, Hà Nội", trips: 29, status: "suspended", submittedAt: "2025-10-19T16:45:00", docsVerified: true, image: suv, gallery: g(suv) },
  { id: "C6", name: "Mitsubishi Xpander Cross", brand: "Mitsubishi", year: 2024, plate: "51L-218.73", segment: "MPV", seats: 7, gearbox: "Số tự động", fuel: "Xăng", pricePerDay: 1_050_000, driverFee: 350_000, ownerId: "O1", city: "TP. Hồ Chí Minh", handover: "Thủ Đức, TP.HCM", trips: 22, status: "ready", submittedAt: "2026-06-05T11:20:00", docsVerified: true, image: mpv, gallery: g(mpv) },
  // Chờ duyệt
  { id: "C7", name: "Toyota Vios GR-S", brand: "Toyota", year: 2025, plate: "51K-660.35", segment: "Sedan", seats: 5, gearbox: "Số tự động", fuel: "Xăng", pricePerDay: 850_000, ownerId: "O4", city: "TP. Hồ Chí Minh", handover: "Bình Thạnh, TP.HCM", trips: 0, status: "pending", submittedAt: "2026-10-18T08:42:00", docsVerified: true, image: sedan, gallery: g(sedan) },
  { id: "C8", name: "Mazda CX-5 Signature", brand: "Mazda", year: 2025, plate: "30L-412.90", segment: "SUV", seats: 5, gearbox: "Số tự động", fuel: "Xăng", pricePerDay: 1_350_000, driverFee: 450_000, ownerId: "O2", city: "Hà Nội", handover: "Tây Hồ, Hà Nội", trips: 0, status: "pending", submittedAt: "2026-10-17T15:10:00", docsVerified: false, image: suv, gallery: g(suv) },
  { id: "C9", name: "Mitsubishi Xpander MT", brand: "Mitsubishi", year: 2023, plate: "43C-098.12", segment: "MPV", seats: 7, gearbox: "Số sàn", fuel: "Xăng", pricePerDay: 800_000, ownerId: "O3", city: "Đà Nẵng", handover: "Sơn Trà, Đà Nẵng", trips: 0, status: "pending", submittedAt: "2026-10-16T10:05:00", docsVerified: true, image: mpv, gallery: g(mpv) },
  { id: "C10", name: "Mazda CX-5 Deluxe", brand: "Mazda", year: 2022, plate: "51G-775.01", segment: "SUV", seats: 5, gearbox: "Số tự động", fuel: "Xăng", pricePerDay: 1_000_000, ownerId: "O1", city: "TP. Hồ Chí Minh", handover: "Quận 1, TP.HCM", trips: 0, status: "pending", submittedAt: "2026-10-15T19:30:00", docsVerified: false, image: suv, gallery: g(suv) },
];

export const customers: Customer[] = [
  { id: "U1", name: "Hoàng Anh Tú", email: "tu.hoang@gmail.com", phone: "0901 234 567", registeredAt: "2025-08-14", orders: 7, accountStatus: "active", violations: 0 },
  { id: "U2", name: "Vũ Thị Lan", email: "lanvu@gmail.com", phone: "0912 345 678", registeredAt: "2025-11-03", orders: 3, accountStatus: "active", violations: 0 },
  { id: "U3", name: "Đặng Quang Huy", email: "huy.dq@outlook.com", phone: "0987 654 321", registeredAt: "2026-02-21", orders: 12, accountStatus: "active", violations: 1 },
  { id: "U4", name: "Bùi Ngọc Hân", email: "han.bui@yahoo.com", phone: "0938 111 222", registeredAt: "2026-04-09", orders: 1, accountStatus: "locked", violations: 2 },
  { id: "U5", name: "Ngô Thanh Sơn", email: "son.ngo@gmail.com", phone: "0976 333 444", registeredAt: "2026-07-30", orders: 4, accountStatus: "active", violations: 0 },
  { id: "U6", name: "Đỗ Minh Châu", email: "chau.do@gmail.com", phone: "0965 777 888", registeredAt: "2026-09-12", orders: 2, accountStatus: "active", violations: 0 },
];

export const admins: Admin[] = [
  { id: "A1", name: "Trần Hải", email: "hai.tran@recar.vn", role: "Super Admin", level: 4, accountStatus: "active", lastLogin: "2026-10-19T16:20:00" },
  { id: "A2", name: "Lý Thu Hà", email: "ha.ly@recar.vn", role: "Kiểm duyệt viên", level: 2, accountStatus: "active", lastLogin: "2026-10-19T09:02:00" },
  { id: "A3", name: "Phan Đức Long", email: "long.phan@recar.vn", role: "Kế toán", level: 2, accountStatus: "active", lastLogin: "2026-10-18T17:45:00" },
  { id: "A4", name: "Mai Khánh Linh", email: "linh.mai@recar.vn", role: "Biên tập viên", level: 1, accountStatus: "active", lastLogin: "2026-10-17T11:12:00" },
];

export const CURRENT_ADMIN = admins[0];

const h = (steps: [OrderStatus, string][]): Order["history"] =>
  steps.map(([status, at]) => ({ status, at, by: status === "created" ? "Khách hàng" : "Hệ thống" }));

export const orders: Order[] = [
  { id: "RC-8902", carId: "C2", customerId: "U1", pickup: "2026-10-17T08:00:00", dropoff: "2026-10-21T18:00:00", total: 4_800_000, status: "renting", payment: "deposit_paid", method: "VNPay", history: h([["created", "2026-10-12T10:00:00"], ["awaiting_deposit", "2026-10-12T10:00:10"], ["deposited", "2026-10-12T10:06:00"], ["awaiting_handover", "2026-10-16T09:00:00"], ["renting", "2026-10-17T08:15:00"]]) },
  { id: "RC-8901", carId: "C4", customerId: "U3", pickup: "2026-10-18T07:30:00", dropoff: "2026-10-20T19:00:00", total: 1_240_000, status: "renting", payment: "deposit_paid", method: "MoMo", history: h([["created", "2026-10-15T20:11:00"], ["awaiting_deposit", "2026-10-15T20:11:10"], ["deposited", "2026-10-15T20:20:00"], ["awaiting_handover", "2026-10-17T09:00:00"], ["renting", "2026-10-18T07:40:00"]]) },
  { id: "RC-8899", carId: "C1", customerId: "U2", pickup: "2026-10-22T09:00:00", dropoff: "2026-10-24T09:00:00", total: 1_500_000, status: "awaiting_deposit", payment: "unpaid", method: "VNPay", history: h([["created", "2026-10-19T14:00:00"], ["awaiting_deposit", "2026-10-19T14:00:10"]]) },
  { id: "RC-8897", carId: "C6", customerId: "U5", pickup: "2026-10-25T06:00:00", dropoff: "2026-10-28T20:00:00", total: 3_150_000, status: "deposited", payment: "deposit_paid", method: "Chuyển khoản", history: h([["created", "2026-10-18T09:00:00"], ["awaiting_deposit", "2026-10-18T09:00:10"], ["deposited", "2026-10-18T11:30:00"]]) },
  { id: "RC-8890", carId: "C3", customerId: "U1", pickup: "2026-10-05T08:00:00", dropoff: "2026-10-08T08:00:00", total: 2_850_000, status: "completed", payment: "paid", method: "VNPay", history: h([["created", "2026-09-30T10:00:00"], ["awaiting_deposit", "2026-09-30T10:00:10"], ["deposited", "2026-09-30T10:05:00"], ["awaiting_handover", "2026-10-04T10:00:00"], ["renting", "2026-10-05T08:10:00"], ["awaiting_completion", "2026-10-08T08:20:00"], ["completed", "2026-10-08T09:00:00"]]) },
  { id: "RC-8884", carId: "C1", customerId: "U6", pickup: "2026-10-01T08:00:00", dropoff: "2026-10-03T08:00:00", total: 1_500_000, status: "completed", payment: "paid", method: "MoMo", history: h([["created", "2026-09-25T10:00:00"], ["deposited", "2026-09-25T10:05:00"], ["renting", "2026-10-01T08:10:00"], ["completed", "2026-10-03T09:00:00"]]) },
  { id: "RC-8879", carId: "C5", customerId: "U4", pickup: "2026-09-28T08:00:00", dropoff: "2026-09-30T08:00:00", total: 2_200_000, status: "cancelled", payment: "refunded", method: "VNPay", cancelReason: "Khách hủy trước 48 giờ, hoàn 100% tiền cọc.", history: [...h([["created", "2026-09-20T10:00:00"], ["deposited", "2026-09-20T10:05:00"]]), { status: "cancelled", at: "2026-09-24T09:00:00", by: "Khách hàng", reason: "Thay đổi lịch trình" }, { status: "refunded", at: "2026-09-25T15:00:00", by: "Phan Đức Long" }] },
  { id: "RC-8872", carId: "C4", customerId: "U2", pickup: "2026-09-20T08:00:00", dropoff: "2026-09-22T08:00:00", total: 1_240_000, status: "awaiting_completion", payment: "deposit_paid", method: "Chuyển khoản", history: h([["created", "2026-09-15T10:00:00"], ["deposited", "2026-09-15T10:05:00"], ["renting", "2026-09-20T08:10:00"], ["awaiting_completion", "2026-09-22T08:30:00"]]) },
  { id: "RC-8865", carId: "C2", customerId: "U5", pickup: "2026-09-10T08:00:00", dropoff: "2026-09-14T08:00:00", total: 4_800_000, status: "completed", payment: "paid", method: "VNPay", history: h([["created", "2026-09-01T10:00:00"], ["deposited", "2026-09-01T10:05:00"], ["renting", "2026-09-10T08:10:00"], ["completed", "2026-09-14T09:00:00"]]) },
  { id: "RC-8860", carId: "C6", customerId: "U3", pickup: "2026-09-05T08:00:00", dropoff: "2026-09-06T08:00:00", total: 1_050_000, status: "refunding", payment: "refunding", method: "MoMo", cancelReason: "Xe gặp sự cố kỹ thuật trước giờ bàn giao.", history: h([["created", "2026-09-01T10:00:00"], ["deposited", "2026-09-01T10:05:00"], ["cancelled", "2026-09-04T18:00:00"], ["refunding", "2026-09-04T18:30:00"]]) },
  { id: "RC-8851", carId: "C3", customerId: "U6", pickup: "2026-08-28T08:00:00", dropoff: "2026-08-31T08:00:00", total: 2_850_000, status: "completed", payment: "paid", method: "Chuyển khoản", history: h([["created", "2026-08-20T10:00:00"], ["deposited", "2026-08-20T10:05:00"], ["completed", "2026-08-31T09:00:00"]]) },
  { id: "RC-8843", carId: "C1", customerId: "U1", pickup: "2026-08-15T08:00:00", dropoff: "2026-08-17T08:00:00", total: 1_500_000, status: "rejected", payment: "refunded", method: "VNPay", cancelReason: "Chủ xe từ chối do trùng lịch bảo dưỡng.", history: h([["created", "2026-08-10T10:00:00"], ["rejected", "2026-08-10T12:00:00"], ["refunded", "2026-08-11T09:00:00"]]) },
];

export const settlements: Settlement[] = [
  { id: "DS-2610-01", ownerId: "O2", period: "01/10 – 15/10/2026", trips: 24, gross: 38_400_000, adjustment: 0, status: "pending", orderIds: ["RC-8865"] },
  { id: "DS-2610-02", ownerId: "O1", period: "01/10 – 15/10/2026", trips: 11, gross: 15_750_000, adjustment: -250_000, status: "processing", orderIds: ["RC-8884"] },
  { id: "DS-2610-03", ownerId: "O3", period: "01/10 – 15/10/2026", trips: 6, gross: 8_550_000, adjustment: 0, status: "discrepancy", orderIds: ["RC-8890"] },
  { id: "DS-2609-01", ownerId: "O2", period: "16/09 – 30/09/2026", trips: 29, gross: 44_100_000, adjustment: 0, status: "settled", settledAt: "2026-10-03T10:00:00", ref: "VCB-FT26276-88123", orderIds: [] },
  { id: "DS-2609-02", ownerId: "O1", period: "16/09 – 30/09/2026", trips: 13, gross: 18_200_000, adjustment: 300_000, status: "settled", settledAt: "2026-10-03T10:20:00", ref: "VCB-FT26276-88124", orderIds: [] },
  { id: "DS-2609-03", ownerId: "O5", period: "16/09 – 30/09/2026", trips: 8, gross: 10_400_000, adjustment: 0, status: "pending", orderIds: [] },
];

export const postCategories = ["Kinh nghiệm thuê xe", "Tin tức ReCar", "Khuyến mãi", "Hướng dẫn chủ xe"];

export const posts: Post[] = [
  { id: "P1", title: "5 lưu ý khi thuê xe tự lái dịp cuối năm", category: "Kinh nghiệm thuê xe", author: "Mai Khánh Linh", createdAt: "2026-10-10", updatedAt: "2026-10-12", status: "published", cover: suv, content: "<h2>Đặt xe sớm</h2><p>Nhu cầu thuê xe tăng mạnh vào cuối năm…</p>" },
  { id: "P2", title: "ReCar mở rộng dịch vụ tại Đà Nẵng", category: "Tin tức ReCar", author: "Trần Hải", createdAt: "2026-09-28", updatedAt: "2026-09-28", status: "published", cover: mpv, content: "<p>Từ tháng 10, ReCar chính thức có mặt tại Đà Nẵng…</p>" },
  { id: "P3", title: "Giảm 15% cho chuyến đi đầu tiên", category: "Khuyến mãi", author: "Mai Khánh Linh", createdAt: "2026-10-15", updatedAt: "2026-10-18", status: "draft", cover: sedan, content: "<p>Áp dụng cho khách hàng mới…</p>" },
  { id: "P4", title: "Hướng dẫn chụp ảnh xe để được duyệt nhanh", category: "Hướng dẫn chủ xe", author: "Lý Thu Hà", createdAt: "2026-08-20", updatedAt: "2026-09-02", status: "hidden", cover: interior, content: "<ul><li>Chụp đủ 4 góc ngoại thất</li><li>Chụp nội thất rõ nét</li></ul>" },
];

export const revenue6m = [
  { month: "T5", gmv: 812_000_000, commission: 81_200_000, trips: 412 },
  { month: "T6", gmv: 905_000_000, commission: 90_500_000, trips: 455 },
  { month: "T7", gmv: 1_120_000_000, commission: 112_000_000, trips: 538 },
  { month: "T8", gmv: 1_064_000_000, commission: 106_400_000, trips: 507 },
  { month: "T9", gmv: 1_198_000_000, commission: 119_800_000, trips: 571 },
  { month: "T10", gmv: 1_286_000_000, commission: 128_600_000, trips: 604 },
];

export const notifications = [
  { id: "N1", title: "Hồ sơ xe mới chờ duyệt", body: "Toyota Vios GR-S · 51K-660.35", to: "/admin/cars/pending", at: "10 phút trước", read: false },
  { id: "N2", title: "Đối soát có sai lệch", body: "DS-2610-03 · Trần Thị Mai", to: "/admin/finance", at: "1 giờ trước", read: false },
  { id: "N3", title: "Yêu cầu hoàn tiền", body: "Đơn RC-8860 đang chờ xử lý", to: "/admin/orders", at: "3 giờ trước", read: false },
  { id: "N4", title: "Bài viết đã xuất bản", body: "5 lưu ý khi thuê xe tự lái…", to: "/admin/blog", at: "Hôm qua", read: true },
];

export const ownerById = (id: string) => owners.find((o) => o.id === id)!;
export const customerById = (id: string) => customers.find((c) => c.id === id)!;
