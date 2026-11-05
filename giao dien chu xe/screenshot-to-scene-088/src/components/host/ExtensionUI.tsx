import type { ReactNode } from "react";
import { Info, ChevronLeft, ChevronRight } from "lucide-react";
import { addMonths } from "date-fns";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { Input } from "@/components/ui/input";
import { isoDate } from "@/lib/host/extension";
export function SelectField({
  label,
  value,
  onChange,
  options,
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
  options: { value: string; label: string }[];
}) {
  return (
    <Select value={value} onValueChange={onChange}>
      <SelectTrigger aria-label={label} className="w-full min-w-0 sm:w-52">
        <SelectValue placeholder={label} />
      </SelectTrigger>
      <SelectContent>
        {options.map((o) => (
          <SelectItem key={o.value} value={o.value}>
            {o.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
export function Stats({
  items,
}: {
  items: { label: string; value: ReactNode; note?: string; icon?: ReactNode }[];
}) {
  return (
    <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-5">
      {items.map((i) => (
        <div key={i.label} className="card-surface min-w-0 p-4">
          <div className="mb-3 flex items-center justify-between gap-2 text-muted-foreground">
            <span className="text-xs font-medium">{i.label}</span>
            {i.note ? (
              <TooltipProvider>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <Button
                      variant="ghost"
                      size="icon"
                      className="size-6 shrink-0"
                      aria-label={`Cách tính ${i.label}`}
                    >
                      <Info className="size-3.5" />
                    </Button>
                  </TooltipTrigger>
                  <TooltipContent className="max-w-xs">{i.note}</TooltipContent>
                </Tooltip>
              </TooltipProvider>
            ) : (
              i.icon
            )}
          </div>
          <div className="break-words text-xl font-bold">{i.value}</div>
        </div>
      ))}
    </div>
  );
}
export function Section({
  title,
  action,
  children,
}: {
  title: string;
  action?: ReactNode;
  children: ReactNode;
}) {
  return (
    <section className="space-y-4 border-t pt-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h2 className="text-lg font-bold">{title}</h2>
        {action}
      </div>
      {children}
    </section>
  );
}
export function MonthNav({ month, onChange }: { month: string; onChange: (v: string) => void }) {
  return (
    <div className="flex items-center gap-1">
      <Button
        variant="outline"
        size="icon"
        aria-label="Tháng trước"
        onClick={() => onChange(isoDate(addMonths(new Date(month + "-01T12:00"), -1)).slice(0, 7))}
      >
        <ChevronLeft />
      </Button>
      <Input
        type="month"
        aria-label="Chọn tháng"
        value={month}
        onChange={(e) => e.target.value && onChange(e.target.value)}
        className="w-40"
      />
      <Button
        variant="outline"
        size="icon"
        aria-label="Tháng sau"
        onClick={() => onChange(isoDate(addMonths(new Date(month + "-01T12:00"), 1)).slice(0, 7))}
      >
        <ChevronRight />
      </Button>
    </div>
  );
}
export const weekdayNames = ["Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật"];
export const tableClass =
  "w-full min-w-[740px] text-left text-sm [&_th]:whitespace-nowrap [&_th]:bg-muted [&_th]:px-4 [&_th]:py-3 [&_th]:text-xs [&_th]:font-semibold [&_th]:text-muted-foreground [&_td]:border-b [&_td]:px-4 [&_td]:py-3";
export function Notice({ children }: { children: ReactNode }) {
  return (
    <p className="rounded-lg border border-info/20 bg-info-soft p-3 text-xs leading-relaxed text-info">
      {children}
    </p>
  );
}
