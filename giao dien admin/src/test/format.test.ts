import { describe, it, expect } from "vitest";
import { splitDeposit, payout } from "@/lib/format";

describe("business rules", () => {
  it("deposit is 30% and remaining 70% by default", () => {
    expect(splitDeposit(1_000_000)).toEqual({ deposit: 300_000, remaining: 700_000 });
  });
  it("platform commission is 10% by default", () => {
    expect(payout(2_000_000)).toEqual({ fee: 200_000, net: 1_800_000 });
  });
});
