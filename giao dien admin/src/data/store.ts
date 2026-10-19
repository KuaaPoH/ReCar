/**
 * Kho trạng thái demo trong bộ nhớ trình duyệt (mất khi tải lại trang).
 * Mỗi hàm mutate tương ứng một API dự kiến — xem src/data/api.ts.
 */
import { useSyncExternalStore } from "react";
import * as demo from "./demo";

export interface AuditEntry { at: string; by: string; action: string; target: string; reason?: string }

export interface Settings {
  depositPct: number;
  commissionPct: number;
  pricingRule: string;
  refundPolicy: string;
  cancelPolicy: string;
  settlementRule: string;
  hotline: string;
  supportEmail: string;
  contact: string;
  systemNotice: string;
  brand: string;
  policy: string;
  maintenance: boolean;
  maintenanceMsg: string;
  updatedBy: string;
  updatedAt: string;
}

interface State {
  cars: demo.Car[];
  orders: demo.Order[];
  settlements: demo.Settlement[];
  customers: demo.Customer[];
  owners: demo.Owner[];
  admins: demo.Admin[];
  posts: demo.Post[];
  categories: string[];
  notifications: typeof demo.notifications;
  audit: AuditEntry[];
  settings: Settings;
}

let state: State = {
  cars: demo.cars,
  orders: demo.orders,
  settlements: demo.settlements,
  customers: demo.customers,
  owners: demo.owners,
  admins: demo.admins,
  posts: demo.posts,
  categories: demo.postCategories,
  notifications: demo.notifications,
  audit: [],
  settings: {
    depositPct: 30,
    commissionPct: 10,
    pricingRule: "Giá thuê tính theo ngày (24 giờ). Quá giờ dưới 4 giờ tính 10%/giờ, trên 4 giờ tính thêm 1 ngày.",
    refundPolicy: "Hủy trước 48 giờ: hoàn 100% tiền cọc. Trong vòng 48 giờ: hoàn 50%. Sau giờ nhận xe: không hoàn.",
    cancelPolicy: "Chủ xe hủy đơn đã cọc sẽ bị trừ điểm uy tín và hoàn 100% cho khách.",
    settlementRule: "Đối soát 2 kỳ/tháng (ngày 1 và 16). Chuyển khoản trong 3 ngày làm việc sau khi chốt kỳ.",
    hotline: "1900 6868",
    supportEmail: "hotro@recar.vn",
    contact: "Tầng 8, 123 Nguyễn Văn Linh, Quận 7, TP.HCM",
    systemNotice: "",
    brand: "ReCar",
    policy: "Điều khoản sử dụng và chính sách bảo mật của ReCar…",
    maintenance: false,
    maintenanceMsg: "Hệ thống bảo trì từ 00:00 đến 02:00.",
    updatedBy: "Trần Hải",
    updatedAt: "2026-10-01T09:00:00",
  },
};

const listeners = new Set<() => void>();
const emit = () => listeners.forEach((l) => l());
const subscribe = (l: () => void) => (listeners.add(l), () => listeners.delete(l));

export function useStore<T>(sel: (s: State) => T): T {
  return useSyncExternalStore(subscribe, () => sel(state), () => sel(state));
}
export const getState = () => state;

export function update(fn: (s: State) => Partial<State>, audit?: Omit<AuditEntry, "at" | "by">) {
  const patch = fn(state);
  state = { ...state, ...patch };
  if (audit) state.audit = [{ ...audit, at: new Date().toISOString(), by: demo.CURRENT_ADMIN.name }, ...state.audit];
  emit();
}

/** Mô phỏng độ trễ mạng để hiển thị trạng thái loading. */
export const simulate = (ms = 700) => new Promise((r) => setTimeout(r, ms));
