import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import { addDays, startOfWeek } from "date-fns";
import { CalendarDays, Plus, Trash2, ChevronLeft, ChevronRight, ArrowUpRight } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { PageHeader, EmptyState } from "@/components/host/PageHeader";
import {
  Section,
  SelectField,
  MonthNav,
  weekdayNames,
  tableClass,
  Notice,
} from "@/components/host/ExtensionUI";
import { OrderStatusBadge } from "@/components/host/StatusBadge";
import { useHost } from "@/lib/host/store";
import { useHostExtension, configFor } from "@/lib/host/extension-store";
import {
  calendarDays,
  isoDate,
  overlaps,
  dayPrice,
  blockLabels,
  type CalendarBlock,
} from "@/lib/host/extension";
import { DEMO_TODAY } from "@/lib/host/mock-data";
import { formatDate, formatDateTime, formatShortVND } from "@/lib/host/format";
import { DEMO_NOTE } from "@/lib/host/api";
export const Route = createFileRoute("/CarOwner/Calendar/Index")({
  head: () => ({
    meta: [
      { title: "Lịch xe — ReCar Host Pro" },
      {
        name: "description",
        content: "Lịch thuê xe theo tháng và tuần, quản lý lịch chặn và bảo dưỡng.",
      },
      { property: "og:title", content: "Lịch xe — ReCar Host Pro" },
      {
        property: "og:description",
        content: "Theo dõi đơn thuê và quản lý thời gian hoạt động của đội xe.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: CalendarPage,
});
const color = {
  available: "bg-success-soft text-success",
  booked: "bg-info-soft text-info",
  handover: "bg-warning-soft text-warning",
  blocked: "bg-destructive-soft text-destructive",
  maintenance: "bg-maintenance-soft text-maintenance",
};
function CalendarPage() {
  const { cars, orders } = useHost();
  const { pricing, holidays, blocks, addBlock, removeBlock } = useHostExtension();
  const [month, setMonth] = useState(DEMO_TODAY.slice(0, 7));
  const [week, setWeek] = useState(DEMO_TODAY);
  const [mode, setMode] = useState("month");
  const [carId, setCarId] = useState("all");
  const [status, setStatus] = useState("all");
  const [orderId, setOrderId] = useState("");
  const [block, setBlock] = useState<CalendarBlock | null>(null);
  const [error, setError] = useState("");
  const [removeId, setRemoveId] = useState("");
  const shownCars = cars.filter((c) => carId === "all" || String(c.id) === carId);
  const selectedOrder = orders.find((o) => o.id === orderId);
  const [detailBlock, setDetailBlock] = useState<CalendarBlock | null>(null);
  const dates =
    mode === "month"
      ? calendarDays(month)
      : Array.from({ length: 7 }, (_, i) =>
          isoDate(addDays(startOfWeek(new Date(week + "T12:00"), { weekStartsOn: 1 }), i)),
        );
  const hasOccupied = (date: string, id: number) => {
    const next = isoDate(addDays(new Date(date + "T12:00"), 1));
    return (
      orders.some(
        (o) =>
          o.carId === id &&
          o.status !== 2 &&
          overlaps(date + "T00:00", next + "T00:00", o.pickupAt, o.returnAt),
      ) ||
      blocks.some(
        (b) => b.carId === id && overlaps(date + "T00:00", next + "T00:00", b.start, b.end),
      )
    );
  };
  const events = (date: string, id: number) => {
    const next = isoDate(addDays(new Date(date + "T12:00"), 1));
    const rentals = orders
      .filter(
        (o) =>
          o.carId === id &&
          o.status !== 2 &&
          overlaps(date + "T00:00", next + "T00:00", o.pickupAt, o.returnAt),
      )
      .map((o) => ({
        id: o.id,
        kind: o.status === 3 ? "handover" : "booked",
        label: o.id,
        sub: o.guestName,
        order: o,
        block: undefined as CalendarBlock | undefined,
      }));
    const locked = blocks
      .filter((b) => b.carId === id && overlaps(date + "T00:00", next + "T00:00", b.start, b.end))
      .map((b) => ({
        id: b.id,
        kind: b.kind === "maintenance" ? "maintenance" : "blocked",
        label: blockLabels[b.kind],
        sub: b.note,
        order: undefined,
        block: b,
      }));
    return [...rentals, ...locked].filter((e) => status === "all" || e.kind === status);
  };
  const openBlock = (date = DEMO_TODAY) => {
    setError("");
    setBlock({
      id: crypto.randomUUID(),
      carId: shownCars[0]?.id ?? cars[0]?.id ?? 0,
      start: date + "T08:00",
      end: date + "T18:00",
      kind: "personal",
      note: "",
    });
  };
  const eventButton = (e: ReturnType<typeof events>[number], name?: string) => (
    <Button
      key={e.id}
      variant="ghost"
      title={`${name ?? ""} ${e.label} · ${e.sub}`}
      className={`h-auto w-full flex-col items-start gap-0 overflow-hidden rounded px-1.5 py-1 text-left ${color[e.kind as keyof typeof color]}`}
      onClick={() => (e.order ? setOrderId(e.id) : setDetailBlock(e.block ?? null))}
    >
      <span className="w-full truncate text-[10px] font-bold">
        {name ? `${name} · ` : ""}
        {e.label}
      </span>
      <span className="w-full truncate text-[10px] font-normal">{e.sub}</span>
    </Button>
  );
  return (
    <div className="space-y-6">
      <PageHeader
        title="Lịch xe của tôi"
        actions={
          <Button onClick={() => openBlock()}>
            <Plus />
            Chặn lịch
          </Button>
        }
      />
      <div className="flex flex-wrap items-center gap-3">
        <Button
          variant="outline"
          onClick={() => {
            setMonth(DEMO_TODAY.slice(0, 7));
            setWeek(DEMO_TODAY);
          }}
        >
          Hôm nay
        </Button>
        {mode === "month" ? (
          <MonthNav month={month} onChange={setMonth} />
        ) : (
          <div className="flex gap-1">
            <Button
              variant="outline"
              size="icon"
              aria-label="Tuần trước"
              onClick={() => setWeek(isoDate(addDays(new Date(week + "T12:00"), -7)))}
            >
              <ChevronLeft />
            </Button>
            <Input
              type="date"
              aria-label="Chọn tuần"
              value={week}
              onChange={(e) => e.target.value && setWeek(e.target.value)}
              className="w-40"
            />
            <Button
              variant="outline"
              size="icon"
              aria-label="Tuần sau"
              onClick={() => setWeek(isoDate(addDays(new Date(week + "T12:00"), 7)))}
            >
              <ChevronRight />
            </Button>
          </div>
        )}
        <Tabs value={mode} onValueChange={setMode}>
          <TabsList>
            <TabsTrigger value="month">Tháng</TabsTrigger>
            <TabsTrigger value="week">Tuần</TabsTrigger>
          </TabsList>
        </Tabs>
        <SelectField
          label="Lọc xe"
          value={carId}
          onChange={setCarId}
          options={[
            { value: "all", label: "Tất cả xe" },
            ...cars.map((c) => ({ value: String(c.id), label: c.name })),
          ]}
        />
        <SelectField
          label="Lọc trạng thái lịch"
          value={status}
          onChange={setStatus}
          options={[
            { value: "all", label: "Tất cả trạng thái" },
            { value: "available", label: "Sẵn sàng" },
            { value: "booked", label: "Có đơn / Đang thuê" },
            { value: "handover", label: "Chờ bàn giao" },
            { value: "blocked", label: "Đã chặn" },
            { value: "maintenance", label: "Bảo dưỡng" },
          ]}
        />
      </div>
      <div className="overflow-x-auto rounded-lg border bg-card">
        {mode === "month" ? (
          <div className="grid min-w-[770px] grid-cols-7">
            {weekdayNames.map((n) => (
              <div key={n} className="bg-muted p-3 text-center text-xs font-semibold">
                {n}
              </div>
            ))}
            {dates.map((date) => (
              <div
                key={date}
                className={`min-h-40 space-y-1 border-b border-r p-2 ${date.slice(0, 7) !== month ? "bg-muted/40 opacity-60" : ""}`}
              >
                <div className="flex items-center justify-between">
                  <span
                    className={`grid size-6 place-items-center rounded-full text-xs font-semibold ${date === DEMO_TODAY ? "bg-primary text-primary-foreground" : ""}`}
                  >
                    {Number(date.slice(-2))}
                  </span>
                  <Button
                    size="icon"
                    variant="ghost"
                    className="size-5"
                    aria-label={`Chặn lịch ${date}`}
                    onClick={() => openBlock(date)}
                  >
                    <Plus className="size-3" />
                  </Button>
                </div>
                {shownCars.map((c) => {
                  const es = events(date, c.id);
                  if (es.length)
                    return (
                      <div key={c.id} className="space-y-1">
                        {es.map((e) => eventButton(e, c.name.split(" ").slice(0, 2).join(" ")))}
                      </div>
                    );
                  const raw =
                    orders.some(
                      (o) =>
                        o.carId === c.id &&
                        o.status !== 2 &&
                        overlaps(
                          date + "T00:00",
                          isoDate(addDays(new Date(date + "T12:00"), 1)) + "T00:00",
                          o.pickupAt,
                          o.returnAt,
                        ),
                    ) ||
                    blocks.some(
                      (b) =>
                        b.carId === c.id &&
                        overlaps(
                          date + "T00:00",
                          isoDate(addDays(new Date(date + "T12:00"), 1)) + "T00:00",
                          b.start,
                          b.end,
                        ),
                    );
                  if (
                    raw ||
                    !["available", "busy"].includes(c.status) ||
                    (status !== "all" && status !== "available")
                  )
                    return null;
                  return (
                    <div
                      key={c.id}
                      className="flex justify-between gap-1 rounded bg-success-soft px-1.5 py-1 text-[10px] text-success"
                    >
                      <span className="truncate">{c.brand}</span>
                      <span>
                        {formatShortVND(
                          dayPrice(c, date, configFor(pricing, c.id), holidays).price,
                        )}
                      </span>
                    </div>
                  );
                })}
              </div>
            ))}
          </div>
        ) : (
          <table className="w-full min-w-[1050px] table-fixed text-sm">
            <thead>
              <tr>
                <th className="w-44 bg-muted p-3 text-left">Xe</th>
                {dates.map((d, i) => (
                  <th key={d} className="bg-muted p-3 text-xs">
                    {weekdayNames[i]}
                    <br />
                    {formatDate(d)}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {shownCars.map((c) => (
                <tr key={c.id}>
                  <td className="border-b p-3">
                    <img
                      src={c.image}
                      alt={c.name}
                      className="mb-2 h-16 w-28 rounded object-cover"
                    />
                    <p className="text-xs font-semibold">{c.name}</p>
                    <span className="text-[10px] text-muted-foreground">{c.plate}</span>
                  </td>
                  {dates.map((d) => (
                    <td key={d} className="border-b border-l p-2 align-top">
                      <div className="space-y-2">
                        {events(d, c.id).map((e) => eventButton(e))}
                        {!hasOccupied(d, c.id) &&
                          (status === "all" || status === "available") &&
                          ["available", "busy"].includes(c.status) && (
                            <span className="text-xs text-success">
                              {formatShortVND(
                                dayPrice(c, d, configFor(pricing, c.id), holidays).price,
                              )}
                            </span>
                          )}
                      </div>
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
      <div className="flex flex-wrap gap-4 text-xs">
        {Object.entries(color).map(([k, c]) => (
          <span key={k} className={`rounded px-2 py-1 ${c}`}>
            {
              {
                available: "Sẵn sàng",
                booked: "Có đơn / Đang thuê",
                handover: "Chờ bàn giao",
                blocked: "Đã chặn",
                maintenance: "Bảo dưỡng",
              }[k as keyof typeof color]
            }
          </span>
        ))}
      </div>
      <Section
        title="Lịch đã chặn"
        action={
          <span className="text-sm text-muted-foreground">
            {blocks.filter((b) => shownCars.some((c) => c.id === b.carId)).length} lịch
          </span>
        }
      >
        <div className="overflow-x-auto rounded-lg border">
          <table className={tableClass}>
            <thead>
              <tr>
                <th>Xe</th>
                <th>Thời gian</th>
                <th>Loại</th>
                <th>Ghi chú</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {blocks
                .filter((b) => shownCars.some((c) => c.id === b.carId))
                .map((b) => (
                  <tr key={b.id}>
                    <td>{cars.find((c) => c.id === b.carId)?.name}</td>
                    <td>
                      {formatDateTime(b.start)}
                      <br />
                      {formatDateTime(b.end)}
                    </td>
                    <td>{blockLabels[b.kind]}</td>
                    <td>{b.note || "—"}</td>
                    <td>
                      <Button
                        variant="ghost"
                        size="icon"
                        aria-label={`Gỡ lịch ${b.id}`}
                        onClick={() => setRemoveId(b.id)}
                      >
                        <Trash2 className="size-4 text-destructive" />
                      </Button>
                    </td>
                  </tr>
                ))}
            </tbody>
          </table>
          {!blocks.filter((b) => shownCars.some((c) => c.id === b.carId)).length && (
            <EmptyState icon={<CalendarDays />} title="Chưa có lịch chặn" />
          )}
        </div>
        <Notice>
          Lịch chặn và giá hiển thị là dữ liệu mẫu. Kiểm tra xung đột hiện chỉ diễn ra trong bản
          prototype; ASP.NET Core phải kiểm tra lại trước khi chặn lịch hoặc nhận đơn mới.
        </Notice>
      </Section>
      <Dialog open={!!block} onOpenChange={(o) => !o && setBlock(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Chặn lịch xe</DialogTitle>
          </DialogHeader>
          {block && (
            <div className="space-y-4">
              <SelectField
                label="Xe cần chặn"
                value={String(block.carId)}
                onChange={(v) => setBlock({ ...block, carId: Number(v) })}
                options={cars.map((c) => ({ value: String(c.id), label: c.name }))}
              />
              <label className="block text-sm">
                Bắt đầu
                <Input
                  aria-label="Bắt đầu chặn"
                  type="datetime-local"
                  value={block.start}
                  onChange={(e) => setBlock({ ...block, start: e.target.value })}
                />
              </label>
              <label className="block text-sm">
                Kết thúc
                <Input
                  aria-label="Kết thúc chặn"
                  type="datetime-local"
                  value={block.end}
                  onChange={(e) => setBlock({ ...block, end: e.target.value })}
                />
              </label>
              <SelectField
                label="Loại chặn"
                value={block.kind}
                onChange={(v) => setBlock({ ...block, kind: v as CalendarBlock["kind"] })}
                options={Object.entries(blockLabels).map(([value, label]) => ({ value, label }))}
              />
              <Textarea
                aria-label="Ghi chú lịch chặn"
                placeholder="Lý do / ghi chú"
                value={block.note}
                onChange={(e) => setBlock({ ...block, note: e.target.value })}
              />
              {error && (
                <p role="alert" className="text-sm text-destructive">
                  {error}
                </p>
              )}
            </div>
          )}
          <DialogFooter>
            <Button variant="outline" onClick={() => setBlock(null)}>
              Hủy
            </Button>
            <Button
              onClick={() => {
                if (!block) return;
                const error = addBlock(block);
                if (error) {
                  setError(error);
                  return;
                }
                setBlock(null);
                toast.success("Đã chặn lịch mẫu", { description: DEMO_NOTE });
              }}
            >
              Xác nhận chặn lịch
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
      <Dialog open={!!selectedOrder} onOpenChange={(o) => !o && setOrderId("")}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Đơn thuê {selectedOrder?.id}</DialogTitle>
          </DialogHeader>
          {selectedOrder && (
            <div className="space-y-4 text-sm">
              <p className="font-semibold">
                {cars.find((c) => c.id === selectedOrder.carId)?.name}
              </p>
              <p>{selectedOrder.guestName}</p>
              <p>Nhận: {formatDateTime(selectedOrder.pickupAt)}</p>
              <p>Trả: {formatDateTime(selectedOrder.returnAt)}</p>
              <OrderStatusBadge status={selectedOrder.status} />
              <p className={selectedOrder.depositConfirmed ? "text-success" : "text-warning"}>
                {selectedOrder.depositConfirmed ? "Đã xác nhận cọc 30%" : "Chưa xác nhận cọc"}
              </p>
              <Button asChild>
                <Link to="/CarOwner/Orders/Details/$id" params={{ id: selectedOrder.id }}>
                  Xem chi tiết đơn
                  <ArrowUpRight />
                </Link>
              </Button>
            </div>
          )}
        </DialogContent>
      </Dialog>
      <Dialog open={!!detailBlock} onOpenChange={(o) => !o && setDetailBlock(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{detailBlock ? blockLabels[detailBlock.kind] : ""}</DialogTitle>
          </DialogHeader>
          {detailBlock && (
            <div className="space-y-3 text-sm">
              <p>{cars.find((c) => c.id === detailBlock.carId)?.name}</p>
              <p>
                {formatDateTime(detailBlock.start)} – {formatDateTime(detailBlock.end)}
              </p>
              <p>{detailBlock.note}</p>
              <Button
                variant="destructive"
                onClick={() => {
                  setRemoveId(detailBlock.id);
                  setDetailBlock(null);
                }}
              >
                Gỡ lịch chặn
              </Button>
            </div>
          )}
        </DialogContent>
      </Dialog>
      <Dialog open={!!removeId} onOpenChange={(o) => !o && setRemoveId("")}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Gỡ lịch chặn?</DialogTitle>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setRemoveId("")}>
              Hủy
            </Button>
            <Button
              variant="destructive"
              onClick={() => {
                const error = removeBlock(removeId);
                if (error) {
                  toast.error(error);
                  return;
                }
                setRemoveId("");
                toast.success("Đã gỡ lịch mẫu", { description: DEMO_NOTE });
              }}
            >
              Xác nhận gỡ lịch
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
