import { describe, expect, it } from "vitest";
import { canHandover, canToggleActive, completeOrder, financeBreakdown, removeMode } from "./rules";
import { cars, orders } from "./mock-data";

const car = (id: number) => cars.find((c) => c.id === id)!;
const order = (id: string) => orders.find((o) => o.id === id)!;

describe("host rules", () => {
  it("splits 30% deposit / 70% remaining and deducts 10% fee once", () => {
    const f = financeBreakdown(1_000_000);
    expect(f.deposit).toBe(300_000);
    expect(f.remaining).toBe(700_000);
    expect(f.platformFee).toBe(100_000);
    expect(f.ownerNet).toBe(900_000);
    expect(f.payoutFromPlatform).toBe(200_000);
  });
  it("blocks handover without confirmed deposit", () => {
    expect(canHandover(order("RC-24088"))).toBe(false);
    expect(canHandover(order("RC-24091"))).toBe(true);
  });
  it("cannot activate a busy or pending car", () => {
    expect(canToggleActive(car(101), orders).ok).toBe(false);
    expect(canToggleActive(car(104), orders).ok).toBe(false);
  });
  it("trip count increments exactly once", () => {
    const o = order("RC-24075"), c = car(101);
    const r1 = completeOrder(o, c, false);
    const r2 = completeOrder(r1.order, r1.car, false);
    expect(r1.car.completedTrips).toBe(c.completedTrips + 1);
    expect(r2.car.completedTrips).toBe(c.completedTrips + 1);
    expect(r1.car.status).toBe("available");
  });
  it("blocks removing a car with an active rental, soft-deletes cars with history", () => {
    expect(removeMode(car(101), orders).mode).toBe("blocked");
    expect(removeMode(car(104), orders).mode).toBe("hard");
  });
});
