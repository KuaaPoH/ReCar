// Types mirror the expected ASP.NET Core MVC view models (Areas/CarOwner).
// Order status codes follow the existing ReCar database mapping — do not change.

export type CarStatus = "available" | "busy" | "pending" | "rejected" | "paused" | "maintenance";

/** Existing DB codes: 2 = Đã hủy, 3 = Chờ bàn giao, 4 = Đang cho thuê, 5 = Hoàn tất */
export type OrderStatus = 2 | 3 | 4 | 5;

export interface Car {
  id: number;
  ownerId: number;
  name: string;
  brand: string;
  segment: string;
  year: number;
  plate: string;
  color: string;
  seats: number;
  transmission: "Tự động" | "Số sàn";
  fuel: "Xăng" | "Dầu" | "Điện" | "Hybrid";
  consumption: string;
  pricePerDay: number;
  salePrice: number | null;
  saleEnabled: boolean;
  location: string;
  completedTrips: number;
  status: CarStatus;
  rejectReason?: string | undefined;
  image: string;
  amenities: string[];
  kmLimitPerDay: number;
  overKmFee: number;
  delivery: boolean;
  deliveryFee: number;
  createdAt: string;
  hasTransactions: boolean;
}

export interface Review {
  id: number;
  carId: number;
  guest: string;
  stars: number;
  comment: string;
  date: string;
}

export interface OrderHistory {
  at: string;
  label: string;
  note?: string;
}

export interface Order {
  id: string;
  carId: number;
  createdAt: string;
  guestName: string;
  guestPhone: string;
  guestEmail: string;
  guestIdNumber: string;
  guestLicense: string;
  pickupAt: string;
  returnAt: string;
  days: number;
  total: number;
  depositConfirmed: boolean;
  status: OrderStatus;
  tripCounted: boolean;
  cancelReason?: string;
  history: OrderHistory[];
}

export interface OwnerProfile {
  id: number;
  fullName: string;
  initials: string;
  verified: "approved" | "pending" | "rejected";
  trusted: boolean;
  idNumber: string;
  licenseNumber: string;
  licenseClass: string;
  submittedAt: string;
  bankName: string;
  bankAccount: string;
  bankHolder: string;
  rejectReason?: string;
  missingItems?: string[];
}

export interface HostNotification {
  id: number;
  title: string;
  body: string;
  at: string;
  read: boolean;
  orderId?: string;
}
