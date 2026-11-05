import { describe, it, expect } from "vitest";
import {
  blockConflict,
  defaultPricing,
  dayPrice,
  quotePrice,
  validDiscount,
  walletBalances,
  payoutAmount,
  vehiclePerformance,
  type CalendarBlock,
} from "./extension";
import { cars, orders, reviews, payouts } from "./mock-data";
const car = cars.find((c) => c.id === 101);
if (!car) throw new Error("Missing fixture");
const block: CalendarBlock = {
  id: "test",
  carId: 101,
  start: "2026-10-27T08:00",
  end: "2026-10-28T08:00",
  kind: "personal",
  note: "",
};
describe("host extension rules", () => {
  it("uses holiday instead of stacking weekday surcharges", () => {
    const c = { ...car, pricePerDay: 800000 };
    expect(
      dayPrice(c, "2026-10-31", defaultPricing(), [
        {
          id: "h",
          name: "Holiday",
          start: "2026-10-31",
          end: "2026-10-31",
          percent: 50,
          carIds: [101],
          enabled: true,
        },
      ]).price,
    ).toBe(1200000);
  });
  it("applies only the highest qualifying duration tier once", () => {
    const config = defaultPricing();
    config.enabled = false;
    const q = quotePrice({ ...car, pricePerDay: 800000 }, "2026-10-01", "2026-10-08", config, []);
    expect(q.days).toBe(7);
    expect(q.subtotal).toBe(5600000);
    expect(q.reduction).toBe(560000);
    expect(q.total).toBe(5040000);
  });
  it("validates discounts from zero through 100 percent", () => {
    expect(validDiscount(3, -1)).toBe(false);
    expect(validDiscount(3, 101)).toBe(false);
    expect(validDiscount(3, 100)).toBe(true);
    expect(validDiscount(0, 5)).toBe(false);
  });
  it("does not change confirmed order totals during a quote", () => {
    const o = orders.find((o) => o.id === "RC-24075");
    if (!o) throw new Error("Missing fixture");
    quotePrice(car, "2026-10-26", "2026-10-29", defaultPricing(), []);
    expect(o.total).toBe(2070000);
  });
  it("blocks overlaps with active rentals", () => {
    expect(blockConflict(block, orders, [])).toContain("RC-24075");
  });
  it("blocks confirmed pending orders", () => {
    expect(
      blockConflict(
        { ...block, carId: 102, start: "2026-10-31T08:00", end: "2026-11-01T08:00" },
        orders,
        [],
      ),
    ).toContain("RC-24091");
  });
  it("also protects orders awaiting deposit confirmation", () => {
    expect(
      blockConflict(
        { ...block, carId: 103, start: "2026-10-30T08:00", end: "2026-10-31T08:00" },
        orders,
        [],
      ),
    ).toContain("RC-24088");
  });
  it("rejects overlapping blocks but permits exact adjacent intervals", () => {
    expect(blockConflict({ ...block, id: "second" }, [], [block])).toContain("lịch chặn");
    expect(
      blockConflict(
        { ...block, id: "next", start: block.end, end: "2026-10-29T08:00" },
        [],
        [block],
      ),
    ).toBeUndefined();
  });
  it("never treats direct owner payments as platform payout and separates settlement states", () => {
    const mine = orders.filter((o) => cars.some((c) => c.id === o.carId && c.ownerId === 1));
    const b = walletBalances(mine, payouts);
    expect(b.gross).toBe(22160000);
    expect(b.net).toBe(19944000);
    expect(b.pending).toBe(414000);
    expect(b.processing).toBe(1410000);
    expect(b.paid).toBe(2608000);
    expect(b.pending + b.processing + b.paid).toBe(4432000);
    const firstPayout = payouts[0];
    if (!firstPayout) throw new Error("Missing payout fixture");
    expect(payoutAmount(firstPayout, mine)).toBe(300000);
  });
  it("counts only completed revenue and actual dated reviews", () => {
    const row = vehiclePerformance(car, orders, reviews, [], "2026-10-01", "2026-11-01");
    expect(row.gross).toBe(2070000);
    expect(row.net).toBe(1863000);
    expect(row.trips).toBe(1);
    expect(row.reviews.length).toBe(1);
    expect(row.avg).toBe(5);
  });
  it("removes blocked days from operating-day denominator", () => {
    const row = vehiclePerformance(
      { ...car, createdAt: "2026-01-01" },
      [],
      [],
      [{ ...block, start: "2026-10-01T00:00", end: "2026-10-03T00:00" }],
      "2026-10-01",
      "2026-10-08",
    );
    expect(row.availableDays).toBe(5);
    expect(row.occupancy).toBe(0);
  });
});
