import { createFileRoute, Link } from "@tanstack/react-router";
import { useMemo, useState } from "react";
import { Eye, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { EmptyState, ErrorState, PageHeader, Pager, Panel, StatusBadge, TableSkeleton, useDebounced, useDemoLoad, usePaged } from "@/components/admin/shared";
import { useStore } from "@/data/store";
import { customerById, ownerById, type OrderStatus } from "@/data/demo";
import { dateVN, daysBetween, splitDeposit, vnd } from "@/lib/format";
import { seo } from "@/lib/seo";

export const Route = createFileRoute("/admin/orders/")({
  head: () => seo("Đơn thuê xe", "Quản lý toàn bộ đơn thuê xe trên ReCar."),
  component: Orders,
});

const TABS: [string, string, OrderStatus[]][] = [
  ["all", "Tất cả", []],
  ["awaiting_deposit", "Chờ cọc", ["created", "awaiting_deposit"]],
  ["deposited", "Đã đặt cọc", ["deposited"]],
  ["renting", "Bàn giao / Đang thuê", ["awaiting_handover", "renting", "awaiting_completion"]],
  ["completed", "Hoàn thành", ["completed"]],
  ["cancelled", "Đã hủy", ["cancelled", "rejected", "refunding", "refunded"]],
];

function Orders() {
  const { state, retry } = useDemoLoad();
  const orders = useStore((s) => s.orders);
  const cars = useStore((s) => s.cars);
  const depositPct = useStore((s) => s.settings.depositPct);
  const [tab, setTab] = useState("all");
  const [q, setQ] = useState("");
  const [method, setMethod] = useState("all");
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");
  const dq = useDebounced(q);

  const list = useMemo(() => orders.filter((o) => {
    const t = TABS.find((x) => x[0] === tab)!;
    const car = cars.find((c) => c.id === o.carId)!;
    const k = dq.toLowerCase();
    const hay = [o.id, customerById(o.customerId).name, ownerById(car.ownerId).name, car.plate].join(" ").toLowerCase();
    return (t[2].length === 0 || t[2].includes(o.status)) && (!k || hay.includes(k))
      && (method === "all" || o.method === method)
      && (!from || o.pickup.slice(0, 10) >= from) && (!to || o.pickup.slice(0, 10) <= to);
  }), [orders, cars, tab, dq, method, from, to]);
  const paged = usePaged(list, 8);

  return (
    <>
      <PageHeader title="Đơn thuê xe" desc="Theo dõi tiến trình, thanh toán và xử lý sự cố đơn thuê." />
      <Tabs value={tab} onValueChange={setTab} className="mb-4">
        <TabsList className="h-auto flex-wrap">
          {TABS.map(([v, l, s]) => <TabsTrigger key={v} value={v}>{l} <span className="ml-1 text-xs text-muted-foreground">{s.length ? orders.filter((o) => s.includes(o.status)).length : orders.length}</span></TabsTrigger>)}
        </TabsList>
      </Tabs>
      <Panel className="mb-4">
        <div className="grid gap-3 p-4 sm:grid-cols-2 lg:grid-cols-4">
          <div className="relative"><Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" /><Input className="pl-9" placeholder="Mã đơn, khách hàng, chủ xe" value={q} onChange={(e) => setQ(e.target.value)} /></div>
          <Select value={method} onValueChange={setMethod}><SelectTrigger><SelectValue /></SelectTrigger><SelectContent><SelectItem value="all">Mọi phương thức</SelectItem>{["VNPay", "MoMo", "Chuyển khoản"].map((m) => <SelectItem key={m} value={m}>{m}</SelectItem>)}</SelectContent></Select>
          <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} aria-label="Từ ngày" />
          <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} aria-label="Đến ngày" />
        </div>
      </Panel>
      <Panel>
        {state === "loading" ? <TableSkeleton /> : state === "error" ? <ErrorState onRetry={retry} /> : list.length === 0 ? <EmptyState /> : (
          <>
            <Table>
              <TableHeader><TableRow><TableHead className="pl-5">Mã đơn</TableHead><TableHead>Xe</TableHead><TableHead>Khách thuê</TableHead><TableHead>Nhận / Trả</TableHead><TableHead className="text-center">Ngày</TableHead><TableHead className="text-right">Tổng tiền</TableHead><TableHead className="text-right">Cọc {depositPct}%</TableHead><TableHead className="text-right">Còn lại</TableHead><TableHead>Trạng thái</TableHead><TableHead>Thanh toán</TableHead><TableHead className="pr-5" /></TableRow></TableHeader>
              <TableBody>
                {paged.slice.map((o) => {
                  const car = cars.find((c) => c.id === o.carId)!;
                  const cu = customerById(o.customerId);
                  const { deposit, remaining } = splitDeposit(o.total, depositPct);
                  return (
                    <TableRow key={o.id}>
                      <TableCell className="pl-5 font-mono text-xs font-semibold">{o.id}</TableCell>
                      <TableCell><div className="flex items-center gap-2"><img src={car.image} alt="" className="h-9 w-12 rounded object-cover" /><div><p className="whitespace-nowrap text-sm font-medium">{car.name}</p><span className="plate">{car.plate}</span></div></div></TableCell>
                      <TableCell><p className="whitespace-nowrap">{cu.name}</p><p className="text-xs text-muted-foreground">{cu.phone}</p></TableCell>
                      <TableCell className="whitespace-nowrap text-xs">{dateVN(o.pickup)}<br />{dateVN(o.dropoff)}</TableCell>
                      <TableCell className="text-center">{daysBetween(o.pickup, o.dropoff)}</TableCell>
                      <TableCell className="whitespace-nowrap text-right font-medium">{vnd(o.total)}</TableCell>
                      <TableCell className="whitespace-nowrap text-right">{vnd(deposit)}</TableCell>
                      <TableCell className="whitespace-nowrap text-right">{vnd(remaining)}</TableCell>
                      <TableCell><StatusBadge status={o.status} /></TableCell>
                      <TableCell><StatusBadge status={o.payment} /></TableCell>
                      <TableCell className="pr-5"><Button asChild variant="ghost" size="icon" className="size-8" aria-label="Chi tiết"><Link to="/admin/orders/$id" params={{ id: o.id }}><Eye /></Link></Button></TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
            <Pager {...paged} />
          </>
        )}
      </Panel>
    </>
  );
}
