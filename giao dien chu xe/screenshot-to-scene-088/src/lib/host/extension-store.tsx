import { createContext, useContext, useState, type ReactNode } from "react";
import {
  defaultPricing,
  blockConflict,
  type PricingConfig,
  type HolidayRule,
  type CalendarBlock,
} from "./extension";
import { useHost } from "./store";
interface ExtensionState {
  pricing: Record<number, PricingConfig>;
  holidays: HolidayRule[];
  blocks: CalendarBlock[];
  setPricing: (ids: number[], config: PricingConfig) => void;
  setHolidays: (rules: HolidayRule[]) => void;
  addBlock: (block: CalendarBlock) => string | undefined;
  removeBlock: (id: string) => string | undefined;
}
const Ctx = createContext<ExtensionState | null>(null);
export function HostExtensionProvider({ children }: { children: ReactNode }) {
  const { cars, orders } = useHost();
  const [pricing, updatePricing] = useState<Record<number, PricingConfig>>({});
  const [holidays, setHolidays] = useState<HolidayRule[]>([
    {
      id: "newyear2027",
      name: "Tết Dương lịch 2027",
      start: "2027-01-01",
      end: "2027-01-03",
      percent: 50,
      carIds: cars.map((c) => c.id),
      enabled: true,
    },
  ]);
  const [blocks, setBlocks] = useState<CalendarBlock[]>([
    {
      id: "maintenance-demo",
      carId: 102,
      start: "2026-10-23T08:00",
      end: "2026-10-24T18:00",
      kind: "maintenance",
      note: "Kiểm tra định kỳ",
    },
    {
      id: "personal-demo",
      carId: 103,
      start: "2026-11-04T08:00",
      end: "2026-11-06T18:00",
      kind: "personal",
      note: "Sử dụng xe gia đình",
    },
  ]);
  return (
    <Ctx.Provider
      value={{
        pricing,
        holidays,
        blocks,
        setHolidays,
        setPricing: (ids, config) =>
          updatePricing((prev) => ({
            ...prev,
            ...Object.fromEntries(
              ids
                .filter((id) => cars.some((c) => c.id === id))
                .map((id) => [id, structuredClone(config)]),
            ),
          })),
        addBlock: (block) => {
          if (!cars.some((c) => c.id === block.carId)) return "Không tìm thấy xe của bạn.";
          const error = blockConflict(block, orders, blocks);
          if (error) return error;
          setBlocks((bs) => [...bs, block]);
          return undefined;
        },
        removeBlock: (id) => {
          const b = blocks.find((x) => x.id === id);
          if (!b) return "Không tìm thấy lịch chặn.";
          const error = blockConflict(b, orders, []);
          if (error) return error;
          setBlocks((bs) => bs.filter((x) => x.id !== id));
          return undefined;
        },
      }}
    >
      {children}
    </Ctx.Provider>
  );
}
export function useHostExtension() {
  const value = useContext(Ctx);
  if (!value) throw new Error("HostExtensionProvider missing");
  return value;
}
export const configFor = (pricing: Record<number, PricingConfig>, id: number) =>
  pricing[id] ?? defaultPricing();
