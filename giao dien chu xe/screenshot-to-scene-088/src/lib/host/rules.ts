// Business rules for the CarOwner area. In production every rule here MUST be
// re-checked server-side (ASP.NET Core); the frontend only uses them for UI gating.
import type { Car, CarStatus, Order, OrderStatus } from "./types";

/** Current business configuration — read from backend config in production. */
export const DEPOSIT_RATE = 0.3;
export const PLATFORM_FEE_RATE = 0.1;

export interface FinanceBreakdown {
  total: number;
  deposit: number; // 30% thu qua sàn
  remaining: number; // 70% khách trả trực tiếp cho chủ xe
  platformFee: number; // 10% tổng tiền thuê, khấu trừ một lần
  ownerNet: number; // thu nhập thực nhận
  payoutFromPlatform: number; // phần cọc sàn chuyển lại sau khi trừ phí
}

export function financeBreakdown(total: number): FinanceBreakdown {
  const deposit = Math.round(total * DEPOSIT_RATE);
  const remaining = total - deposit;
  const platformFee = Math.round(total * PLATFORM_FEE_RATE);
  const ownerNet = total - platformFee; // fee deducted exactly once
  return {
    total,
    deposit,
    remaining,
    platformFee,
    ownerNet,
    payoutFromPlatform: deposit - platformFee,
  };
}

export const ORDER_STATUS_LABEL: Record<OrderStatus, string> = {
  2: "Đã hủy",
  3: "Chờ bàn giao",
  4: "Đang cho thuê",
  5: "Hoàn tất",
};

export const CAR_STATUS_LABEL: Record<CarStatus, string> = {
  available: "Sẵn sàng",
  busy: "Đang bận",
  pending: "Chờ duyệt",
  rejected: "Bị từ chối",
  paused: "Tạm ngừng",
  maintenance: "Bảo dưỡng",
};

export function canHandover(o: Order) {
  return o.status === 3 && o.depositConfirmed;
}
export function canReceiveBack(o: Order) {
  return o.status === 4;
}
export function canCancel(o: Order) {
  return o.status === 3;
}

/** Owner may only toggle between Sẵn sàng and Tạm ngừng. */
export function canToggleActive(car: Car, orders: Order[]): { ok: boolean; reason?: string } {
  if (car.status === "pending") return { ok: false, reason: "Xe đang chờ Admin phê duyệt." };
  if (car.status === "rejected") return { ok: false, reason: "Hồ sơ bị từ chối, cần chỉnh sửa và gửi lại." };
  if (car.status === "maintenance") return { ok: false, reason: "Xe đang bảo dưỡng." };
  if (car.status === "busy" || orders.some((o) => o.carId === car.id && o.status === 4))
    return { ok: false, reason: "Xe đang có khách thuê." };
  return { ok: true };
}

export type RemoveMode = "blocked" | "soft" | "hard";
export function removeMode(car: Car, orders: Order[]): { mode: RemoveMode; reason?: string } {
  const mine = orders.filter((o) => o.carId === car.id);
  if (mine.some((o) => o.status === 4)) return { mode: "blocked", reason: "Xe đang có khách thuê." };
  if (mine.some((o) => o.status === 3))
    return { mode: "blocked", reason: "Xe còn đơn đặt trước chưa hoàn tất." };
  if (car.hasTransactions || mine.length > 0) return { mode: "soft" };
  return { mode: "hard" };
}

/** Completing an order increments the trip count exactly once. */
export function completeOrder(order: Order, car: Car, hasNextBooking: boolean) {
  if (order.status !== 4) return { order, car };
  const nextOrder: Order = { ...order, status: 5, tripCounted: true };
  const nextCar: Car = {
    ...car,
    completedTrips: order.tripCounted ? car.completedTrips : car.completedTrips + 1,
    status: hasNextBooking ? car.status : car.status === "busy" ? "available" : car.status,
  };
  return { order: nextOrder, car: nextCar };
}

export function isTrustedHost(avgStars: number, reviewCount: number, trips: number, verified: boolean) {
  return verified && avgStars >= 4.8 && reviewCount >= 10 && trips >= 20;
}
