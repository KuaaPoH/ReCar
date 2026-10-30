import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import { Area, AreaChart, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { ArrowDownRight, ArrowUpRight, Camera, Car, CheckCircle2, ClipboardList, Lightbulb, MessageCircle, Plus, Star, Tags, Truck, Wallet, FileCheck } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useHost } from "@/lib/host/store";
import { financeBreakdown } from "@/lib/host/rules";
import { formatDateTime, formatShortVND, formatVND } from "@/lib/host/format";
import { DEMO_TODAY } from "@/lib/host/mock-data";
import { OrderStatusBadge } from "@/components/host/StatusBadge";
import { EmptyState } from "@/components/host/PageHeader";

export const Route = createFileRoute("/CarOwner/Home/Index")({
  head: () => ({
    meta: [
      { title: "Tổng quan — ReCar Host Pro" },
      { name: "description", content: "Doanh thu, đội xe, lượt thuê và đánh giá của chủ xe ReCar." },
      { property: "og:title", content: "Tổng quan — ReCar Host Pro" },
      { property: "og:description", content: "Bảng điều khiển kinh doanh dành cho chủ xe ReCar." },
    ],
  }),
  component: Dashboard,
});

const monthKey = (iso: string) => iso.slice(0, 7);
function lastMonths(n: number) {
  const y = Number(DEMO_TODAY.slice(0, 4)), m = Number(DEMO_TODAY.slice(5, 7));
  return Array.from({ length: n }, (_, i) => {
    const d = new Date(Date.UTC(y, m - 1 - (n - 1 - i), 1));
    return `${d.getUTCFullYear()}-${String(d.getUTCMonth() + 1).padStart(2, "0")}`;
  });
}

function Dashboard() {
  const { cars, orders, reviews } = useHost();
  const [scope, setScope] = useState<"month" | "all">("month");
  const completed = orders.filter((o) => o.status === 5);
  const months = lastMonths(6);
  const cur = months[5], prev = months[4];
  const netIn = (k?: string) => completed.filter((o) => !k || monthKey(o.returnAt) === k).reduce((s, o) => s + financeBreakdown(o.total).ownerNet, 0);
  const grossIn = (k?: string) => completed.filter((o) => !k || monthKey(o.returnAt) === k).reduce((s, o) => s + o.total, 0);
  const curNet = netIn(cur), prevNet = netIn(prev);
  const delta = prevNet ? ((curNet - prevNet) / prevNet) * 100 : 0;
  const shownNet = scope === "month" ? curNet : netIn();
  const shownGross = scope === "month" ? grossIn(cur) : grossIn();
  const heldDeposits = orders.filter((o) => o.status === 3 || o.status === 4).reduce((s, o) => s + financeBreakdown(o.total).deposit, 0);

  const chart = months.map((k) => ({ month: `T${Number(k.slice(5))}`, net: netIn(k), gross: grossIn(k) }));
  const processing = orders.filter((o) => o.status === 3 || o.status === 4).length;
  const finished = orders.filter((o) => o.status === 5 || o.status === 2);
  const completionRate = finished.length ? Math.round((completed.length / finished.length) * 100) : 0;
  const avg = reviews.length ? reviews.reduce((s, r) => s + r.stars, 0) / reviews.length : 0;
  const satisfied = reviews.length ? Math.round((reviews.filter((r) => r.stars >= 4).length / reviews.length) * 100) : 0;
  const latest = [...orders].sort((a, b) => b.createdAt.localeCompare(a.createdAt)).slice(0, 5);
  const carOf = (id: number) => cars.find((c) => c.id === id);
  const count = (s: string) => cars.filter((c) => c.status === s).length;

  return (
    <div className="space-y-6">
      <div className="flex flex-col justify-between gap-3 sm:flex-row sm:items-end">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Chào mừng trở lại 👋</h1>
          <p className="text-sm text-muted-foreground">Tình hình kinh doanh đội xe của bạn tính đến {formatDateTime(DEMO_TODAY)}.</p>
        </div>
        <Button asChild><Link to="/CarOwner/Cars/Create"><Plus /> Thêm xe mới</Link></Button>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <div className="card-surface p-5">
          <div className="flex items-center justify-between">
            <span className="grid size-10 place-items-center rounded-xl bg-primary-soft text-primary"><Wallet className="size-5" /></span>
            <div className="flex rounded-lg bg-muted p-0.5 text-xs font-medium">
              {(["month", "all"] as const).map((s) => (
                <button key={s} onClick={() => setScope(s)} className={scope === s ? "rounded-md bg-card px-2 py-1 shadow-card" : "px-2 py-1 text-muted-foreground"}>{s === "month" ? "Tháng này" : "Toàn bộ"}</button>
              ))}
            </div>
          </div>
          <p className="mt-4 text-sm text-muted-foreground">Thu nhập thực nhận</p>
          <p className="text-2xl font-bold">{formatVND(shownNet)}</p>
          {scope === "month" && (
            <p className={`mt-1 flex items-center gap-1 text-xs font-semibold ${delta >= 0 ? "text-success" : "text-destructive"}`}>
              {delta >= 0 ? <ArrowUpRight className="size-3.5" /> : <ArrowDownRight className="size-3.5" />}{Math.abs(delta).toFixed(1)}% so với tháng trước
            </p>
          )}
          <div className="mt-3 space-y-1 border-t pt-3 text-xs text-muted-foreground">
            <p className="flex justify-between"><span>Doanh thu giao dịch</span><span className="font-medium text-foreground">{formatVND(shownGross)}</span></p>
            <p className="flex justify-between"><span>Cọc sàn đang giữ (không tính vào thu nhập)</span><span className="font-medium text-foreground">{formatVND(heldDeposits)}</span></p>
          </div>
        </div>

        <Kpi icon={<Car className="size-5" />} label="Xe đang quản lý" value={String(cars.length)}>
          <Row k="Sẵn sàng" v={count("available")} /><Row k="Đang được thuê" v={count("busy")} /><Row k="Chờ duyệt / Tạm ngừng" v={count("pending") + count("paused")} />
        </Kpi>
        <Kpi icon={<ClipboardList className="size-5" />} label="Chuyến hoàn thành" value={String(completed.length)}>
          <Row k="Đơn đang xử lý" v={processing} /><Row k="Tỷ lệ hoàn tất" v={`${completionRate}%`} />
        </Kpi>
        <Kpi icon={<Star className="size-5" />} label="Đánh giá trung bình" value={reviews.length ? `${avg.toFixed(1)} / 5` : "Chưa có"}>
          <Row k="Lượt đánh giá" v={reviews.length} /><Row k="Hài lòng (≥ 4 sao)" v={reviews.length ? `${satisfied}%` : "—"} />
        </Kpi>
      </div>

      <div className="grid gap-6 xl:grid-cols-3">
        <div className="card-surface p-5 xl:col-span-2">
          <div className="mb-4">
            <h2 className="font-bold">Thu nhập 6 tháng gần nhất</h2>
            <p className="text-xs text-muted-foreground">Thu nhập thực nhận = tổng tiền thuê của đơn hoàn tất − phí dịch vụ sàn 10%.</p>
          </div>
          {chart.every((c) => c.gross === 0) ? (
            <EmptyState icon={<Wallet />} title="Chưa có dữ liệu doanh thu" />
          ) : (
            <div className="h-72">
              <ResponsiveContainer>
                <AreaChart data={chart} margin={{ left: 0, right: 8 }}>
                  <defs>
                    <linearGradient id="net" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="0%" stopColor="var(--chart-1)" stopOpacity={0.3} />
                      <stop offset="100%" stopColor="var(--chart-1)" stopOpacity={0} />
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" vertical={false} />
                  <XAxis dataKey="month" tickLine={false} axisLine={false} fontSize={12} stroke="var(--muted-foreground)" />
                  <YAxis tickFormatter={formatShortVND} tickLine={false} axisLine={false} fontSize={12} width={56} stroke="var(--muted-foreground)" />
                  <Tooltip formatter={(v: number) => formatVND(v)} contentStyle={{ borderRadius: 12, border: "1px solid var(--border)" }} />
                  <Legend iconType="circle" wrapperStyle={{ fontSize: 12 }} />
                  <Area type="monotone" dataKey="gross" name="Doanh thu giao dịch" stroke="var(--chart-2)" fill="transparent" strokeDasharray="5 4" strokeWidth={2} />
                  <Area type="monotone" dataKey="net" name="Thu nhập thực nhận" stroke="var(--chart-1)" fill="url(#net)" strokeWidth={2.5} />
                </AreaChart>
              </ResponsiveContainer>
            </div>
          )}
        </div>

        <div className="card-surface flex flex-col p-5">
          <h2 className="flex items-center gap-2 font-bold"><Lightbulb className="size-4.5 text-warning" /> Host Insights</h2>
          <p className="mb-3 text-xs text-muted-foreground">Mẹo tối ưu doanh thu</p>
          <ul className="flex-1 space-y-2.5 text-sm">
            {[
              [Camera, "Chụp ảnh xe rõ nét ở nhiều góc"],
              [CheckCircle2, "Bổ sung đầy đủ tiện nghi"],
              [Truck, "Cân nhắc hỗ trợ giao xe tận nơi"],
              [Tags, "Cập nhật giá thuê phù hợp thị trường"],
              [FileCheck, "Hoàn thành hồ sơ pháp lý"],
              [MessageCircle, "Duy trì tỷ lệ phản hồi khách tốt"],
            ].map(([Icon, t]) => {
              const I = Icon as typeof Camera;
              return <li key={t as string} className="flex items-center gap-2.5"><I className="size-4 shrink-0 text-primary" />{t as string}</li>;
            })}
          </ul>
          <Button asChild variant="outline" className="mt-4"><Link to="/CarOwner/Cars/Create"><Plus /> Thêm xe mới vào đội xe</Link></Button>
        </div>
      </div>

      <div className="card-surface overflow-hidden">
        <div className="flex items-center justify-between p-5">
          <h2 className="font-bold">Đơn thuê mới nhất</h2>
          <Button asChild variant="ghost" size="sm"><Link to="/CarOwner/Orders/Index">Xem tất cả đơn thuê</Link></Button>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full min-w-[900px] text-sm">
            <thead className="bg-muted text-left text-xs uppercase tracking-wide text-muted-foreground">
              <tr>{["Mã đơn", "Xe", "Khách thuê", "Nhận — Trả", "Cọc 30%", "Trạng thái", ""].map((h) => <th key={h} className="px-5 py-3 font-semibold">{h}</th>)}</tr>
            </thead>
            <tbody className="divide-y">
              {latest.map((o) => {
                const c = carOf(o.carId);
                return (
                  <tr key={o.id} className="hover:bg-muted/50">
                    <td className="px-5 py-3 font-mono text-xs font-semibold">{o.id}</td>
                    <td className="px-5 py-3">
                      <div className="flex items-center gap-3">
                        <img src={c?.image} alt="" className="h-10 w-16 rounded-lg object-cover" loading="lazy" />
                        <div><p className="font-semibold">{c?.name}</p><span className="plate">{c?.plate}</span></div>
                      </div>
                    </td>
                    <td className="px-5 py-3"><p className="font-medium">{o.guestName}</p><a href={`tel:${o.guestPhone}`} className="text-xs text-primary">{o.guestPhone}</a></td>
                    <td className="px-5 py-3 text-xs">{formatDateTime(o.pickupAt)}<br /><span className="text-muted-foreground">{formatDateTime(o.returnAt)}</span></td>
                    <td className="px-5 py-3 font-semibold">{formatVND(financeBreakdown(o.total).deposit)}</td>
                    <td className="px-5 py-3"><OrderStatusBadge status={o.status} /></td>
                    <td className="px-5 py-3 text-right"><Button asChild variant="outline" size="sm"><Link to="/CarOwner/Orders/Details/$id" params={{ id: o.id }}>Chi tiết</Link></Button></td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

function Kpi({ icon, label, value, children }: { icon: React.ReactNode; label: string; value: string; children: React.ReactNode }) {
  return (
    <div className="card-surface p-5">
      <span className="grid size-10 place-items-center rounded-xl bg-primary-soft text-primary">{icon}</span>
      <p className="mt-4 text-sm text-muted-foreground">{label}</p>
      <p className="text-2xl font-bold">{value}</p>
      <div className="mt-3 space-y-1 border-t pt-3 text-xs text-muted-foreground">{children}</div>
    </div>
  );
}
const Row = ({ k, v }: { k: string; v: React.ReactNode }) => (
  <p className="flex justify-between"><span>{k}</span><span className="font-semibold text-foreground">{v}</span></p>
);
