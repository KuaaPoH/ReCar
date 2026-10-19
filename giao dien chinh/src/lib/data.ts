// DỮ LIỆU MẪU — chỉ để minh họa giao diện, sẽ thay bằng dữ liệu từ Controller/ViewModel.
import sedan from "@/assets/car-sedan.jpg";
import suv from "@/assets/car-suv.jpg";
import mini from "@/assets/car-mini.jpg";
import hero from "@/assets/hero.jpg";

export const images = { sedan, suv, mini, hero };

export type Car = {
  id: number;
  alias: string;
  name: string;
  brand: string;
  type: string;
  seats: number;
  gear: "Số tự động" | "Số sàn";
  fuel: "Xăng" | "Dầu" | "Điện";
  city: string;
  district: string;
  price: number;
  rating: number;
  reviews: number;
  tag?: "Nổi bật" | "Xe điện" | "Ưu đãi";
  image: string;
  owner: string;
  status: "Sẵn sàng" | "Đang cho thuê" | "Chờ duyệt" | "Tạm ngưng";
  features: string[];
};

export const carTypes = [
  { key: "mini", label: "Xe mini đô thị", image: mini },
  { key: "sedan", label: "Sedan 5 chỗ", image: sedan },
  { key: "suv", label: "Crossover/SUV 7 chỗ", image: suv },
  { key: "mpv", label: "MPV đa dụng", image: suv },
  { key: "pickup", label: "Xe bán tải", image: suv },
];

export const brands = ["VinFast", "Toyota", "Mazda", "Hyundai", "Kia", "Honda", "Ford"];
export const cities = ["Hà Nội", "TP. Hồ Chí Minh", "Đà Nẵng", "Nha Trang", "Đà Lạt"];

const f = ["Bản đồ", "Camera lùi", "Bluetooth", "Cảm biến va chạm", "Định vị GPS", "Cửa sổ trời"];

export const cars: Car[] = [
  { id: 1, alias: "vinfast-vf5", name: "VinFast VF 5 Plus", brand: "VinFast", type: "mini", seats: 5, gear: "Số tự động", fuel: "Điện", city: "Hà Nội", district: "Cầu Giấy", price: 650000, rating: 4.9, reviews: 86, tag: "Xe điện", image: mini, owner: "Nguyễn Minh Anh", status: "Sẵn sàng", features: f.slice(0, 4) },
  { id: 2, alias: "toyota-vios", name: "Toyota Vios G 2023", brand: "Toyota", type: "sedan", seats: 5, gear: "Số tự động", fuel: "Xăng", city: "TP. Hồ Chí Minh", district: "Quận 7", price: 850000, rating: 4.8, reviews: 124, tag: "Nổi bật", image: sedan, owner: "Trần Quốc Bảo", status: "Sẵn sàng", features: f.slice(0, 5) },
  { id: 3, alias: "hyundai-santafe", name: "Hyundai Santa Fe 2024", brand: "Hyundai", type: "suv", seats: 7, gear: "Số tự động", fuel: "Dầu", city: "Đà Nẵng", district: "Hải Châu", price: 1650000, rating: 4.7, reviews: 58, tag: "Ưu đãi", image: suv, owner: "Lê Thu Hà", status: "Đang cho thuê", features: f },
  { id: 4, alias: "mazda-3", name: "Mazda 3 Luxury", brand: "Mazda", type: "sedan", seats: 5, gear: "Số tự động", fuel: "Xăng", city: "Hà Nội", district: "Đống Đa", price: 950000, rating: 4.8, reviews: 72, image: sedan, owner: "Phạm Đức Long", status: "Sẵn sàng", features: f.slice(0, 5) },
  { id: 5, alias: "kia-carnival", name: "Kia Carnival Signature", brand: "Kia", type: "mpv", seats: 7, gear: "Số tự động", fuel: "Dầu", city: "TP. Hồ Chí Minh", district: "Quận 1", price: 2400000, rating: 4.9, reviews: 41, tag: "Nổi bật", image: suv, owner: "Trần Quốc Bảo", status: "Sẵn sàng", features: f },
  { id: 6, alias: "ford-ranger", name: "Ford Ranger Wildtrak", brand: "Ford", type: "pickup", seats: 5, gear: "Số tự động", fuel: "Dầu", city: "Đà Lạt", district: "Phường 1", price: 1400000, rating: 4.6, reviews: 33, image: suv, owner: "Võ Hoàng Nam", status: "Sẵn sàng", features: f.slice(1, 5) },
  { id: 7, alias: "honda-city", name: "Honda City RS", brand: "Honda", type: "sedan", seats: 5, gear: "Số tự động", fuel: "Xăng", city: "Nha Trang", district: "Lộc Thọ", price: 800000, rating: 4.7, reviews: 65, tag: "Ưu đãi", image: sedan, owner: "Ngô Thanh Tâm", status: "Sẵn sàng", features: f.slice(0, 4) },
  { id: 8, alias: "toyota-fortuner", name: "Toyota Fortuner Legender", brand: "Toyota", type: "suv", seats: 7, gear: "Số sàn", fuel: "Dầu", city: "Hà Nội", district: "Tây Hồ", price: 1500000, rating: 4.5, reviews: 27, image: suv, owner: "Nguyễn Minh Anh", status: "Chờ duyệt", features: f.slice(0, 5) },
];

export const posts = [
  { id: 1, title: "Kinh nghiệm thuê xe tự lái lần đầu", excerpt: "Những điều cần kiểm tra trước khi nhận xe để chuyến đi suôn sẻ.", date: "08/10/2026", image: sedan, cat: "Cẩm nang" },
  { id: 2, title: "Giấy tờ cần chuẩn bị khi thuê ô tô", excerpt: "CCCD, giấy phép lái xe và các giấy tờ đối chiếu khi ký hợp đồng.", date: "02/10/2026", image: suv, cat: "Cẩm nang" },
  { id: 3, title: "5 cung đường đẹp quanh Đà Lạt", excerpt: "Gợi ý lộ trình cuối tuần cho những ai yêu thích lái xe đường đèo.", date: "25/09/2026", image: hero, cat: "Tin tức" },
];

export type Order = { id: string; car: string; customer: string; from: string; to: string; total: number; deposit: number; status: "Chờ xác nhận" | "Đã đặt cọc" | "Đang thuê" | "Hoàn thành" | "Đã hủy" };

export const orders: Order[] = [
  { id: "RC10241", car: "Toyota Vios G 2023", customer: "Hoàng Lan", from: "15/10/2026 08:00", to: "18/10/2026 08:00", total: 2550000, deposit: 765000, status: "Đã đặt cọc" },
  { id: "RC10238", car: "Kia Carnival Signature", customer: "Đỗ Văn Hùng", from: "12/10/2026 09:00", to: "14/10/2026 18:00", total: 4800000, deposit: 1440000, status: "Đang thuê" },
  { id: "RC10230", car: "VinFast VF 5 Plus", customer: "Mai Phương", from: "20/10/2026 07:00", to: "21/10/2026 20:00", total: 1300000, deposit: 390000, status: "Chờ xác nhận" },
  { id: "RC10219", car: "Mazda 3 Luxury", customer: "Bùi Gia Huy", from: "01/10/2026 08:00", to: "04/10/2026 08:00", total: 2850000, deposit: 855000, status: "Hoàn thành" },
  { id: "RC10202", car: "Honda City RS", customer: "Lý Ngọc Trâm", from: "28/09/2026 10:00", to: "29/09/2026 10:00", total: 800000, deposit: 240000, status: "Đã hủy" },
];

export const vnd = (n: number) => n.toLocaleString("vi-VN") + "đ";
export const DEPOSIT_RATE = 0.3;
