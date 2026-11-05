import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { Plus, Save, Trash2, Pencil, Tags } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { PageHeader } from "@/components/host/PageHeader";
import {
  Stats,
  Section,
  SelectField,
  MonthNav,
  weekdayNames,
  tableClass,
  Notice,
} from "@/components/host/ExtensionUI";
import { useHost } from "@/lib/host/store";
import { useHostExtension, configFor } from "@/lib/host/extension-store";
import {
  calendarDays,
  dayPrice,
  quotePrice,
  validDiscount,
  type HolidayRule,
} from "@/lib/host/extension";
import { DEMO_TODAY } from "@/lib/host/mock-data";
import { formatVND, formatDate } from "@/lib/host/format";
import { DEMO_NOTE } from "@/lib/host/api";
export const Route = createFileRoute("/CarOwner/Pricing/Index")({
  head: () => ({
    meta: [
      { title: "Giá thông minh — ReCar Host Pro" },
      {
        name: "description",
        content: "Cấu hình giá theo thứ, ngày lễ và ưu đãi dài ngày cho đội xe ReCar.",
      },
      { property: "og:title", content: "Giá thông minh — ReCar Host Pro" },
      {
        property: "og:description",
        content: "Quản lý quy tắc giá và xem trước giá thuê theo lịch.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: PricingPage,
});
function PricingPage() {
  const { cars } = useHost();
  const { pricing, setPricing, holidays, setHolidays } = useHostExtension();
  const [carId, setCarId] = useState(String(cars[0]?.id ?? ""));
  const [month, setMonth] = useState(DEMO_TODAY.slice(0, 7));
  const [selectedDay, setSelectedDay] = useState("");
  const [start, setStart] = useState("2026-10-30");
  const [end, setEnd] = useState("2026-11-02");
  const [applyOpen, setApplyOpen] = useState(false);
  const [targets, setTargets] = useState<number[]>([]);
  const [holiday, setHoliday] = useState<HolidayRule | null>(null);
  const [deleteId, setDeleteId] = useState("");
  const car = cars.find((c) => String(c.id) === carId);
  if (!car) return <PageHeader title="Giá thông minh" description="Chưa có xe để cấu hình." />;
  const config = configFor(pricing, car.id);
  const quote = quotePrice(car, start, end, config, holidays);
  const update = (patch: Partial<typeof config>) => setPricing([car.id], { ...config, ...patch });
  const saveHoliday = () => {
    if (!holiday) return;
    if (
      !holiday.name.trim() ||
      !holiday.start ||
      holiday.end < holiday.start ||
      !Number.isFinite(holiday.percent) ||
      holiday.percent < 0 ||
      holiday.percent > 500 ||
      !holiday.carIds.length
    ) {
      toast.error("Kiểm tra tên dịp, ngày, xe và tỷ lệ tăng từ 0–500%.");
      return;
    }
    setHolidays([...holidays.filter((h) => h.id !== holiday.id), holiday]);
    setHoliday(null);
    toast.success("Đã cập nhật quy tắc mẫu", { description: DEMO_NOTE });
  };
  return (
    <div className="space-y-6">
      <PageHeader
        title="Giá thông minh"
        description={car.name}
        actions={
          <>
            <Button
              variant="outline"
              onClick={() => {
                setTargets([car.id]);
                setApplyOpen(true);
              }}
            >
              <Tags />
              Áp dụng nhiều xe
            </Button>
            <Button
              onClick={() => toast.success("Đã cập nhật cấu hình mẫu", { description: DEMO_NOTE })}
            >
              <Save />
              Lưu cấu hình mẫu
            </Button>
          </>
        }
      />
      <div className="flex flex-wrap gap-3">
        <SelectField
          label="Chọn xe cấu hình"
          value={carId}
          onChange={setCarId}
          options={cars.map((c) => ({ value: String(c.id), label: c.name }))}
        />
      </div>
      <Stats
        items={[
          { label: "Giá cơ bản / ngày", value: formatVND(car.pricePerDay) },
          {
            label: "Giá thứ 7",
            value: formatVND(dayPrice(car, "2026-10-31", config, holidays).price),
          },
          {
            label: "Giá ngày lễ cao nhất",
            value: formatVND(
              car.pricePerDay *
                (1 +
                  Math.max(
                    0,
                    ...holidays
                      .filter((h) => h.enabled && h.carIds.includes(car.id))
                      .map((h) => h.percent),
                  ) /
                    100),
            ),
          },
          {
            label: "Quy tắc đang bật",
            value:
              (config.enabled ? config.weekdays.filter((p) => p !== 0).length : 0) +
              config.discounts.filter((d) => d.enabled).length +
              holidays.filter((h) => h.enabled && h.carIds.includes(car.id)).length,
          },
          {
            label: "Xe áp dụng",
            value: cars.filter((c) => configFor(pricing, c.id).enabled).length,
          },
        ]}
      />
      <Section
        title="Giá theo thứ trong tuần"
        action={
          <label className="flex items-center gap-2 text-sm">
            <Switch
              aria-label="Bật giá theo thứ"
              checked={config.enabled}
              onCheckedChange={(enabled) => update({ enabled })}
            />
            Bật điều chỉnh theo thứ
          </label>
        }
      >
        <div className="overflow-x-auto rounded-lg border">
          <table className={tableClass}>
            <thead>
              <tr>
                <th>Ngày</th>
                <th>Giá cơ bản</th>
                <th>Điều chỉnh (%)</th>
                <th>Giá cuối cùng</th>
              </tr>
            </thead>
            <tbody>
              {weekdayNames.map((name, i) => (
                <tr key={name}>
                  <td>{name}</td>
                  <td>{formatVND(car.pricePerDay)}</td>
                  <td>
                    <Input
                      aria-label={`Điều chỉnh ${name}`}
                      type="number"
                      min="0"
                      max="500"
                      className="w-24"
                      disabled={!config.enabled}
                      value={config.weekdays[i]}
                      onChange={(e) => {
                        const n = Number(e.target.value);
                        if (n >= 0 && n <= 500)
                          update({ weekdays: config.weekdays.map((p, j) => (i === j ? n : p)) });
                      }}
                    />
                  </td>
                  <td className="font-semibold text-primary">
                    {formatVND(
                      car.pricePerDay *
                        (1 + (config.enabled ? (config.weekdays[i] ?? 0) : 0) / 100),
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Section>
      <Section
        title="Ngày lễ & dịp đặc biệt"
        action={
          <Button
            variant="outline"
            onClick={() =>
              setHoliday({
                id: crypto.randomUUID(),
                name: "",
                start: DEMO_TODAY,
                end: DEMO_TODAY,
                percent: 50,
                carIds: [car.id],
                enabled: true,
              })
            }
          >
            <Plus />
            Thêm dịp
          </Button>
        }
      >
        <div className="overflow-x-auto rounded-lg border">
          <table className={tableClass}>
            <thead>
              <tr>
                <th>Tên dịp</th>
                <th>Thời gian</th>
                <th>Tăng giá</th>
                <th>Xe áp dụng</th>
                <th>Kích hoạt</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {holidays.map((h) => (
                <tr key={h.id}>
                  <td className="font-medium">{h.name}</td>
                  <td>
                    {formatDate(h.start)} – {formatDate(h.end)}
                  </td>
                  <td>+{h.percent}%</td>
                  <td>{h.carIds.filter((id) => cars.some((c) => c.id === id)).length} xe</td>
                  <td>
                    <Switch
                      aria-label={`Kích hoạt ${h.name}`}
                      checked={h.enabled}
                      onCheckedChange={(enabled) =>
                        setHolidays(holidays.map((x) => (x.id === h.id ? { ...x, enabled } : x)))
                      }
                    />
                  </td>
                  <td>
                    <div className="flex gap-1">
                      <Button
                        size="icon"
                        variant="ghost"
                        aria-label={`Sửa ${h.name}`}
                        onClick={() => setHoliday({ ...h })}
                      >
                        <Pencil className="size-4" />
                      </Button>
                      <Button
                        size="icon"
                        variant="ghost"
                        aria-label={`Xóa ${h.name}`}
                        onClick={() => setDeleteId(h.id)}
                      >
                        <Trash2 className="size-4 text-destructive" />
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
              {!holidays.length && (
                <tr>
                  <td colSpan={6}>Chưa có quy tắc ngày lễ.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </Section>
      <Section
        title="Ưu đãi thuê dài ngày"
        action={
          <Button
            variant="outline"
            onClick={() =>
              update({
                discounts: [
                  ...config.discounts,
                  { id: crypto.randomUUID(), days: 21, percent: 0, enabled: false },
                ],
              })
            }
          >
            <Plus />
            Thêm mức thuê
          </Button>
        }
      >
        <div className="space-y-2">
          {config.discounts.map((d) => (
            <div key={d.id} className="flex flex-wrap items-center gap-3 border-b pb-3">
              <Switch
                aria-label={`Bật ưu đãi ${d.days} ngày`}
                checked={d.enabled}
                onCheckedChange={(enabled) =>
                  update({
                    discounts: config.discounts.map((x) => (x.id === d.id ? { ...x, enabled } : x)),
                  })
                }
              />
              <span className="text-sm">Từ</span>
              <Input
                aria-label={`Số ngày ${d.id}`}
                type="number"
                min="1"
                value={d.days}
                className="w-20"
                onChange={(e) => {
                  const days = Number(e.target.value);
                  if (validDiscount(days, d.percent))
                    update({
                      discounts: config.discounts.map((x) => (x.id === d.id ? { ...x, days } : x)),
                    });
                }}
              />
              <span className="text-sm">ngày · giảm</span>
              <Input
                aria-label={`Mức giảm ${d.days} ngày`}
                type="number"
                min="0"
                max="100"
                value={d.percent}
                className="w-20"
                onChange={(e) => {
                  const percent = Number(e.target.value);
                  if (validDiscount(d.days, percent))
                    update({
                      discounts: config.discounts.map((x) =>
                        x.id === d.id ? { ...x, percent } : x,
                      ),
                    });
                  else toast.error("Mức giảm phải từ 0 đến 100%.");
                }}
              />
              <span>%</span>
              <Button
                variant="ghost"
                size="icon"
                aria-label={`Xóa ưu đãi ${d.days} ngày`}
                onClick={() => update({ discounts: config.discounts.filter((x) => x.id !== d.id) })}
              >
                <Trash2 className="size-4" />
              </Button>
            </div>
          ))}
        </div>
      </Section>
      <Section title="Lịch giá dự kiến" action={<MonthNav month={month} onChange={setMonth} />}>
        <div className="overflow-x-auto">
          <div className="grid min-w-[560px] grid-cols-7 overflow-hidden rounded-lg border">
            {weekdayNames.map((n) => (
              <div key={n} className="bg-muted p-2 text-center text-xs font-semibold">
                {n}
              </div>
            ))}
            {calendarDays(month).map((date) => {
              const p = dayPrice(car, date, config, holidays);
              return (
                <Button
                  key={date}
                  variant="ghost"
                  onClick={() => setSelectedDay(date)}
                  className={`h-20 flex-col items-start justify-between rounded-none border-b border-r p-2 ${date.slice(0, 7) !== month ? "opacity-40" : ""} ${p.kind === "holiday" ? "bg-warning-soft text-warning" : p.kind === "weekend" ? "bg-primary-soft text-primary" : "bg-card"}`}
                >
                  <span className="text-xs">{Number(date.slice(-2))}</span>
                  <span className="text-xs font-bold">{formatVND(p.price)}</span>
                </Button>
              );
            })}
          </div>
        </div>
        <div className="flex flex-wrap gap-4 text-xs">
          <span>○ Ngày thường</span>
          <span className="text-primary">● Giá theo thứ</span>
          <span className="text-warning">● Ngày lễ</span>
        </div>
        {selectedDay && (
          <Notice>
            {formatDate(selectedDay)} · {dayPrice(car, selectedDay, config, holidays).label} · +
            {dayPrice(car, selectedDay, config, holidays).percent}% ·{" "}
            {formatVND(dayPrice(car, selectedDay, config, holidays).price)}
          </Notice>
        )}
      </Section>
      <Section title="Xem thử giá đơn mới">
        <div className="flex flex-wrap gap-4">
          <label className="space-y-1 text-sm">
            Ngày nhận
            <Input
              aria-label="Ngày nhận xem giá"
              type="date"
              value={start}
              onChange={(e) => setStart(e.target.value)}
            />
          </label>
          <label className="space-y-1 text-sm">
            Ngày trả
            <Input
              aria-label="Ngày trả xem giá"
              type="date"
              value={end}
              min={start}
              onChange={(e) => setEnd(e.target.value)}
            />
          </label>
        </div>
        {quote.days > 0 ? (
          <div className="grid gap-6 lg:grid-cols-2">
            <div className="max-h-64 overflow-auto divide-y">
              {quote.lines.map((l) => (
                <div key={l.date} className="flex justify-between gap-3 py-2 text-sm">
                  <span>
                    {formatDate(l.date)}{" "}
                    <span className="text-xs text-muted-foreground">{l.label}</span>
                  </span>
                  <span className="whitespace-nowrap font-medium">{formatVND(l.price)}</span>
                </div>
              ))}
            </div>
            <div className="space-y-3 bg-muted p-5 text-sm">
              <p className="flex justify-between">
                <span>{quote.days} ngày · trước ưu đãi</span>
                <strong>{formatVND(quote.subtotal)}</strong>
              </p>
              <p className="flex justify-between text-success">
                <span>Giảm {quote.discount}%</span>
                <strong>−{formatVND(quote.reduction)}</strong>
              </p>
              <p className="flex justify-between border-t pt-3">
                <span>Tổng dự kiến</span>
                <strong className="text-xl text-primary">{formatVND(quote.total)}</strong>
              </p>
            </div>
          </div>
        ) : (
          <p className="text-sm text-destructive">Ngày trả phải sau ngày nhận.</p>
        )}
        <Notice>
          Ưu tiên: ngày lễ (mức tăng cao nhất) → giá theo thứ → giá cơ bản. Chỉ áp dụng một ưu đãi
          dài ngày đủ điều kiện với ngưỡng ngày cao nhất. Đơn đã xác nhận giữ nguyên giá; ASP.NET
          Core phải tính và xác nhận giá cuối cùng.
        </Notice>
      </Section>
      <Dialog open={!!holiday} onOpenChange={(o) => !o && setHoliday(null)}>
        <DialogContent className="max-h-[85vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Quy tắc ngày lễ</DialogTitle>
          </DialogHeader>
          {holiday && (
            <div className="space-y-4">
              <label className="block space-y-1 text-sm">
                Tên dịp
                <Input
                  value={holiday.name}
                  onChange={(e) => setHoliday({ ...holiday, name: e.target.value })}
                />
              </label>
              <div className="grid grid-cols-2 gap-3">
                <label className="text-sm">
                  Bắt đầu
                  <Input
                    type="date"
                    value={holiday.start}
                    onChange={(e) => setHoliday({ ...holiday, start: e.target.value })}
                  />
                </label>
                <label className="text-sm">
                  Kết thúc
                  <Input
                    type="date"
                    value={holiday.end}
                    onChange={(e) => setHoliday({ ...holiday, end: e.target.value })}
                  />
                </label>
              </div>
              <label className="block text-sm">
                Tỷ lệ tăng (%)
                <Input
                  type="number"
                  min="0"
                  max="500"
                  value={holiday.percent}
                  onChange={(e) => setHoliday({ ...holiday, percent: Number(e.target.value) })}
                />
              </label>
              <p className="text-sm font-semibold">Xe áp dụng</p>
              {cars.map((c) => (
                <label key={c.id} className="flex items-center gap-2 text-sm">
                  <Checkbox
                    checked={holiday.carIds.includes(c.id)}
                    onCheckedChange={(v) =>
                      setHoliday({
                        ...holiday,
                        carIds: v
                          ? [...holiday.carIds, c.id]
                          : holiday.carIds.filter((id) => id !== c.id),
                      })
                    }
                  />
                  {c.name}
                </label>
              ))}
            </div>
          )}
          <DialogFooter>
            <Button variant="outline" onClick={() => setHoliday(null)}>
              Hủy
            </Button>
            <Button onClick={saveHoliday}>Lưu quy tắc mẫu</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
      <Dialog open={applyOpen} onOpenChange={setApplyOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Áp dụng cấu hình cho nhiều xe</DialogTitle>
          </DialogHeader>
          {cars.map((c) => (
            <label key={c.id} className="flex items-center gap-2 text-sm">
              <Checkbox
                checked={targets.includes(c.id)}
                onCheckedChange={(v) =>
                  setTargets(v ? [...targets, c.id] : targets.filter((id) => id !== c.id))
                }
              />
              {c.name}
            </label>
          ))}
          <DialogFooter>
            <Button variant="outline" onClick={() => setApplyOpen(false)}>
              Hủy
            </Button>
            <Button
              disabled={!targets.length}
              onClick={() => {
                setPricing(targets, config);
                setApplyOpen(false);
                toast.success("Đã áp dụng cấu hình mẫu", { description: DEMO_NOTE });
              }}
            >
              Xác nhận áp dụng
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
      <Dialog open={!!deleteId} onOpenChange={(o) => !o && setDeleteId("")}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Xóa quy tắc ngày lễ?</DialogTitle>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setDeleteId("")}>
              Hủy
            </Button>
            <Button
              variant="destructive"
              onClick={() => {
                setHolidays(holidays.filter((h) => h.id !== deleteId));
                setDeleteId("");
              }}
            >
              Xác nhận xóa
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
