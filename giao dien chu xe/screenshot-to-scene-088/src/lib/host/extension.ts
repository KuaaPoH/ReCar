import { addDays, format, startOfMonth, startOfWeek } from "date-fns";
import type { Order, Car, Review } from "./types";
import { financeBreakdown } from "./rules";
export const isoDate = (d: Date) => format(d, "yyyy-MM-dd");
export const daysBetween = (start: string, end: string) =>
  Math.max(0, (Date.parse(end.slice(0, 10)) - Date.parse(start.slice(0, 10))) / 86400000);
export function dateRange(start: string, end: string) {
  if (!start || !end || !Number.isFinite(Date.parse(start)) || !Number.isFinite(Date.parse(end)))
    return [];
  return Array.from({ length: Math.min(730, Math.ceil(daysBetween(start, end))) }, (_, i) =>
    isoDate(addDays(new Date(start + "T12:00:00"), i)),
  );
}
export function calendarDays(month: string) {
  const first = startOfWeek(startOfMonth(new Date(month + "-01T12:00:00")), { weekStartsOn: 1 });
  return Array.from({ length: 42 }, (_, i) => isoDate(addDays(first, i)));
}
export interface HolidayRule {
  id: string;
  name: string;
  start: string;
  end: string;
  percent: number;
  carIds: number[];
  enabled: boolean;
}
export interface DiscountRule {
  id: string;
  days: number;
  percent: number;
  enabled: boolean;
}
export interface PricingConfig {
  enabled: boolean;
  weekdays: number[];
  discounts: DiscountRule[];
}
export interface CalendarBlock {
  id: string;
  carId: number;
  start: string;
  end: string;
  kind: "personal" | "maintenance" | "other";
  note: string;
}
export const defaultPricing = (): PricingConfig => ({
  enabled: true,
  weekdays: [0, 0, 0, 0, 15, 25, 20],
  discounts: [
    { id: "d3", days: 3, percent: 5, enabled: true },
    { id: "d7", days: 7, percent: 10, enabled: true },
    { id: "d14", days: 14, percent: 15, enabled: false },
  ],
});
export const blockLabels = { personal: "Cá nhân", maintenance: "Bảo dưỡng", other: "Khác" };
export function overlaps(a: string, b: string, c: string, d: string) {
  return a < d && c < b;
}
export function blockConflict(block: CalendarBlock, orders: Order[], blocks: CalendarBlock[]) {
  if (!block.start || !block.end || block.start >= block.end)
    return "Thời gian kết thúc phải sau thời gian bắt đầu.";
  const order = orders.find(
    (o) =>
      o.carId === block.carId &&
      (o.status === 3 || o.status === 4) &&
      overlaps(block.start, block.end, o.pickupAt, o.returnAt),
  );
  if (order)
    return `Trùng đơn ${order.id} (${order.status === 4 ? "đang cho thuê" : order.depositConfirmed ? "đã xác nhận cọc" : "đang chờ xác nhận cọc"}).`;
  const other = blocks.find(
    (b) =>
      b.carId === block.carId &&
      b.id !== block.id &&
      overlaps(block.start, block.end, b.start, b.end),
  );
  return other ? "Trùng với một lịch chặn hiện có." : undefined;
}
export function validDiscount(days: number, percent: number) {
  return (
    Number.isInteger(days) &&
    days >= 1 &&
    Number.isFinite(percent) &&
    percent >= 0 &&
    percent <= 100
  );
}
export function dayPrice(car: Car, date: string, config: PricingConfig, holidays: HolidayRule[]) {
  const holiday = holidays
    .filter((h) => h.enabled && h.carIds.includes(car.id) && h.start <= date && h.end >= date)
    .sort((a, b) => b.percent - a.percent || a.id.localeCompare(b.id))[0];
  const weekday = (new Date(date + "T12:00:00").getDay() + 6) % 7;
  const percent = holiday?.percent ?? (config.enabled ? (config.weekdays[weekday] ?? 0) : 0);
  return {
    price: Math.round(car.pricePerDay * (1 + percent / 100)),
    percent,
    kind: holiday ? "holiday" : percent ? "weekend" : "normal",
    label: holiday?.name ?? (percent ? "Giá theo thứ" : "Giá cơ bản"),
  };
}
export function quotePrice(
  car: Car,
  start: string,
  end: string,
  config: PricingConfig,
  holidays: HolidayRule[],
) {
  const dates = dateRange(start, end);
  const lines = dates.map((date) => ({ date, ...dayPrice(car, date, config, holidays) }));
  const subtotal = lines.reduce((s, l) => s + l.price, 0);
  const discount = config.discounts
    .filter((d) => d.enabled && validDiscount(d.days, d.percent) && dates.length >= d.days)
    .sort((a, b) => b.days - a.days || b.percent - a.percent)[0];
  const reduction = Math.round((subtotal * (discount?.percent ?? 0)) / 100);
  return {
    lines,
    days: dates.length,
    subtotal,
    reduction,
    total: subtotal - reduction,
    discount: discount?.percent ?? 0,
  };
}
export function vehiclePerformance(
  car: Car,
  orders: Order[],
  reviews: Review[],
  blocks: CalendarBlock[],
  start: string,
  end: string,
) {
  const dates = dateRange(start, end).filter((d) => d >= car.createdAt);
  const eligible =
    car.status !== "pending" &&
    car.status !== "rejected" &&
    car.status !== "paused" &&
    car.status !== "maintenance";
  const available = eligible
    ? dates.filter(
        (d) =>
          !blocks.some(
            (b) =>
              b.carId === car.id &&
              overlaps(
                d + "T00:00",
                isoDate(addDays(new Date(d + "T12:00"), 1)) + "T00:00",
                b.start,
                b.end,
              ),
          ),
      )
    : [];
  const mine = orders.filter((o) => o.carId === car.id);
  const rented = available.filter((d) =>
    mine.some(
      (o) =>
        (o.status === 4 || o.status === 5) &&
        overlaps(
          d + "T00:00",
          isoDate(addDays(new Date(d + "T12:00"), 1)) + "T00:00",
          o.pickupAt,
          o.returnAt,
        ),
    ),
  );
  const completed = mine.filter(
    (o) => o.status === 5 && o.returnAt.slice(0, 10) >= start && o.returnAt.slice(0, 10) < end,
  );
  const gross = completed.reduce((s, o) => s + o.total, 0);
  const rs = reviews.filter((r) => r.carId === car.id && r.date >= start && r.date < end);
  return {
    car,
    availableDays: available.length,
    rentedDays: rented.length,
    occupancy: available.length ? (rented.length / available.length) * 100 : 0,
    trips: completed.length,
    gross,
    net: financeBreakdown(gross).ownerNet,
    reviews: rs,
    avg: rs.length ? rs.reduce((s, r) => s + r.stars, 0) / rs.length : 0,
  };
}
export type PayoutStatus = "pending" | "processing" | "paid" | "failed" | "review";
export const payoutLabels: Record<PayoutStatus, string> = {
  pending: "Chờ quyết toán",
  processing: "Đang xử lý",
  paid: "Đã chuyển khoản",
  failed: "Thất bại",
  review: "Cần kiểm tra",
};
export interface Payout {
  id: string;
  orderIds: string[];
  status: PayoutStatus;
  period: string;
  date?: string;
  reference?: string;
}
export function payoutAmount(payout: Payout, orders: Order[]) {
  return orders
    .filter((o) => o.status === 5 && payout.orderIds.includes(o.id))
    .reduce((s, o) => s + financeBreakdown(o.total).payoutFromPlatform, 0);
}
export function walletBalances(orders: Order[], payouts: Payout[]) {
  const completed = orders.filter((o) => o.status === 5);
  const assigned = new Set(
    payouts
      .filter((p) => p.status === "paid" || p.status === "processing")
      .flatMap((p) => p.orderIds),
  );
  return {
    gross: completed.reduce((s, o) => s + o.total, 0),
    net: completed.reduce((s, o) => s + financeBreakdown(o.total).ownerNet, 0),
    pending: completed
      .filter((o) => !assigned.has(o.id))
      .reduce((s, o) => s + financeBreakdown(o.total).payoutFromPlatform, 0),
    paid: payouts
      .filter((p) => p.status === "paid")
      .reduce((s, p) => s + payoutAmount(p, orders), 0),
    processing: payouts
      .filter((p) => p.status === "processing")
      .reduce((s, p) => s + payoutAmount(p, orders), 0),
  };
}
