import { createFileRoute, Link } from "@tanstack/react-router";
import { useMemo, useState } from "react";
import { ClipboardList, Eye, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { EmptyState, PageHeader, Pager } from "@/components/host/PageHeader";
import { OrderStatusBadge } from "@/components/host/StatusBadge";
import { OrderActions } from "@/components/host/OrderActions";
import { useHost } from "@/lib/host/store";
import { financeBreakdown } from "@/lib/host/rules";
import { formatDate, formatDateTime, formatVND, initials } from "@/lib/host/format";
import { cn } from "@/lib/utils";
import type { OrderStatus } from "@/lib/host/types";

export const Route = createFileRoute("/CarOwner/Orders/Index")({
  head: () => ({
    meta: [
      { title: "Lịch trình & Đơn xe — ReCar Host Pro" },
      { name: "description", content: "Theo dõi, bàn giao, nhận lại và hủy đơn thuê xe." },
      { property: "og:title", content: "Lịch trình & Đơn xe — ReCar Host Pro" },
      { property: "og:description", content: "Quản lý đơn thuê xe dành cho chủ xe ReCar." },
    ],
  }),
  component: OrdersIndex,
});

const TABS: { key: "all" | OrderStatus; label: string }[] = [
  { key: "all", label: "Tất cả" }, { key: 3, label: "Chờ bàn giao" }, { key: 4, label: "Đang cho thuê" }, { key: 5, label: "Đã hoàn tất" }, { key: 2, label: "Đã hủy" },
];
const PAGE = 6;

function OrdersIndex() {
  const { orders, cars } = useHost();
  const [tab, setTab] = useState<"all" | OrderStatus>("all");
  const [q, setQ] = useState("");
  const [page, setPage] = useState(1);
  const carOf = (id: number) => cars.find((c) => c.id === id);

  const list = useMemo(() => {
    const t = q.trim().toLowerCase();
    return [...orders]
      .filter((o) => tab === "all" || o.status === tab)
      .filter((o) => {
        if (!t) return true;
        const c = carOf(o.carId);
        return [o.id, c?.name, c?.plate, o.guestName, o.guestPhone].some((x) => x?.toLowerCase().includes(t));
      })
      .sort((a, b) => b.createdAt.localeCompare(a.createdAt));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [orders, cars, tab, q]);
  const pages = Math.max(1, Math.ceil(list.length / PAGE));
  const shown = list.slice((page - 1) * PAGE, page * PAGE);

  return (
    <>
      <PageHeader title="Lịch trình & Đơn xe" description="Chỉ hiển thị đơn thuộc các xe của bạn." />
      <div className="mb-4 flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
        <div className="flex gap-1 overflow-x-auto rounded-xl bg-muted p-1">
          {TABS.map((t) => {
            const n = t.key === "all" ? orders.length : orders.filter((o) => o.status === t.key).length;
            return (
              <button key={t.label} onClick={() => { setTab(t.key); setPage(1); }} className={cn("flex items-center gap-2 whitespace-nowrap rounded-lg px-3 py-2 text-sm font-medium", tab === t.key ? "bg-card shadow-card" : "text-muted-foreground hover:text-foreground")}>
                {t.label}<span className={cn("rounded-full px-1.5 text-xs font-bold", tab === t.key ? "bg-primary text-primary-foreground" : "bg-border")}>{n}</span>
              </button>
            );
          })}
        </div>
        <div className="relative lg:w-80">
          <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input value={q} onChange={(e) => { setQ(e.target.value); setPage(1); }} placeholder="Mã đơn, xe, biển số, khách, SĐT…" className="bg-card pl-9" />
        </div>
      </div>

      <div className="card-surface overflow-hidden">
        {shown.length === 0 ? <EmptyState icon={<ClipboardList />} title="Không có đơn thuê" body="Chưa có đơn nào khớp với bộ lọc hiện tại." /> : (
          <div className="overflow-x-auto">
            <table className="w-full min-w-[1200px] text-sm">
              <thead className="bg-muted text-left text-xs uppercase tracking-wide text-muted-foreground">
                <tr>{["Đơn hàng", "Xe", "Khách thuê", "Lịch trình", "Tài chính", "Trạng thái & thao tác"].map((h) => <th key={h} className="px-4 py-3 font-semibold">{h}</th>)}</tr>
              </thead>
              <tbody className="divide-y">
                {shown.map((o) => {
                  const c = carOf(o.carId);
                  const f = financeBreakdown(o.total);
                  return (
                    <tr key={o.id} className="align-top hover:bg-muted/40">
                      <td className="px-4 py-4"><Link to="/CarOwner/Orders/Details/$id" params={{ id: o.id }} className="font-mono font-semibold text-primary">{o.id}</Link><p className="text-xs text-muted-foreground">Tạo {formatDate(o.createdAt)}</p></td>
                      <td className="px-4 py-4">
                        <div className="flex gap-3">
                          <img src={c?.image} alt="" className="h-12 w-18 rounded-lg object-cover" loading="lazy" />
                          <div><p className="font-semibold">{c?.name}</p><span className="plate">{c?.plate}</span><p className="mt-1 text-xs text-muted-foreground">{c?.transmission}</p></div>
                        </div>
                      </td>
                      <td className="px-4 py-4">
                        <div className="flex items-center gap-2">
                          <span className="grid size-8 place-items-center rounded-full bg-primary-soft text-xs font-bold text-accent-foreground">{initials(o.guestName)}</span>
                          <div><p className="font-medium">{o.guestName}</p><a href={`tel:${o.guestPhone}`} className="text-xs text-primary">{o.guestPhone}</a></div>
                        </div>
                      </td>
                      <td className="px-4 py-4 text-xs"><p>Nhận: <b>{formatDateTime(o.pickupAt)}</b></p><p>Trả: <b>{formatDateTime(o.returnAt)}</b></p><p className="text-muted-foreground">{o.days} ngày</p></td>
                      <td className="px-4 py-4 text-xs">
                        <p className="text-sm font-bold">{formatVND(f.total)}</p>
                        <p>Cọc 30% (sàn thu): {formatVND(f.deposit)}</p>
                        <p className="text-muted-foreground">Còn lại 70% (khách trả bạn): {formatVND(f.remaining)}</p>
                        {o.status === 3 && !o.depositConfirmed && <p className="mt-1 font-semibold text-warning">Chờ xác nhận cọc</p>}
                      </td>
                      <td className="space-y-2 px-4 py-4">
                        <OrderStatusBadge status={o.status} />
                        <div className="flex flex-wrap gap-2">
                          <Button asChild size="sm" variant="ghost"><Link to="/CarOwner/Orders/Details/$id" params={{ id: o.id }}><Eye /> Chi tiết</Link></Button>
                          <OrderActions order={o} />
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
        <Pager page={page} pages={pages} onChange={setPage} />
      </div>
    </>
  );
}
