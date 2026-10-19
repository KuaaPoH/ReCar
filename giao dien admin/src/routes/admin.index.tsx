import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import { ArrowDownRight, ArrowRight, ArrowUpRight, Banknote, CarFront, Eye, Percent, ShieldCheck, Star } from "lucide-react";
import { Bar, BarChart, CartesianGrid, Cell, Legend, Pie, PieChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { Button } from "@/components/ui/button";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group";
import { ErrorState, PageHeader, Panel, StatusBadge, TableSkeleton, useDemoLoad, DemoNotice } from "@/components/admin/shared";
import { useStore } from "@/data/store";
import { revenue6m, customerById } from "@/data/demo";
import { dateVN, splitDeposit, vnd, vndShort } from "@/lib/format";
import { seo } from "@/lib/seo";
import { cn } from "@/lib/utils";

export const Route = createFileRoute("/admin/")({
  head: () => seo("Tổng quan", "Bảng điều khiển tổng quan hoạt động sàn ReCar."),
  component: Dashboard,
});

function Kpi({ icon: Icon, label, value, sub, delta, action }: { icon: typeof Banknote; label: string; value: string; sub: string; delta?: number; action?: React.ReactNode }) {
  return (
    <div className="rounded-xl border bg-card p-5 shadow-card">
      <div className="flex items-start justify-between">
        <p className="text-sm text-muted-foreground">{label}</p>
        <div className="grid size-9 place-items-center rounded-lg bg-accent text-accent-foreground"><Icon className="size-4" /></div>
      </div>
      <p className="mt-2 text-2xl font-bold tracking-tight">{value}</p>
      <div className="mt-2 flex items-center justify-between gap-2 text-xs">
        <span className="flex items-center gap-1 text-muted-foreground">
          {delta !== undefined && (
            <span className={cn("inline-flex items-center font-semibold", delta >= 0 ? "text-success" : "text-destructive")}>
              {delta >= 0 ? <ArrowUpRight className="size-3.5" /> : <ArrowDownRight className="size-3.5" />}{Math.abs(delta).toFixed(1)}%
            </span>
          )}
          {sub}
        </span>
        {action}
      </div>
    </div>
  );
}

function Dashboard() {
  const { state, retry } = useDemoLoad();
  const cars = useStore((s) => s.cars);
  const orders = useStore((s) => s.orders);
  const owners = useStore((s) => s.owners);
  const commissionPct = useStore((s) => s.settings.commissionPct);
  const [series, setSeries] = useState<"gmv" | "trips">("gmv");

  const cur = revenue6m[5], prev = revenue6m[4];
  const gmvDelta = ((cur.gmv - prev.gmv) / prev.gmv) * 100;
  const active = cars.filter((c) => c.status === "ready" || c.status === "rented");
  const occupancy = Math.round((cars.filter((c) => c.status === "rented").length / Math.max(1, active.length)) * 100);
  const pending = cars.filter((c) => c.status === "pending").length;

  const fleet = [
    { name: "Sẵn sàng", value: cars.filter((c) => c.status === "ready").length, color: "var(--chart-3)" },
    { name: "Đang có khách", value: cars.filter((c) => c.status === "rented").length, color: "var(--chart-1)" },
    { name: "Bảo dưỡng", value: cars.filter((c) => c.status === "maintenance").length, color: "var(--chart-4)" },
    { name: "Chờ duyệt / Tạm ngừng", value: cars.filter((c) => ["pending", "suspended"].includes(c.status)).length, color: "var(--chart-5)" },
  ];
  const latest = [...orders].sort((a, b) => b.history[0].at.localeCompare(a.history[0].at)).slice(0, 6);
  const topOwners = [...owners].sort((a, b) => b.revenue - a.revenue).slice(0, 5);

  return (
    <>
      <PageHeader title="Tổng quan" desc="Số liệu tháng 10/2026 so với tháng trước." />
      <div className="mb-4"><DemoNotice>Dữ liệu demo minh họa — chưa kết nối MySQL. Bản tích hợp lấy từ DashboardController.</DemoNotice></div>
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <Kpi icon={Banknote} label="Tổng giá trị giao dịch (GMV)" value={vndShort(cur.gmv)} delta={gmvDelta} sub="so với tháng trước" />
        <Kpi icon={Percent} label="Doanh thu hoa hồng sàn" value={vndShort(cur.gmv * commissionPct / 100)} sub={`Tỷ lệ áp dụng ${commissionPct}% GMV`} />
        <Kpi icon={CarFront} label="Xe đang hoạt động" value={String(active.length)} sub={`Tỷ lệ lấp đầy ${occupancy}%`} />
        <Kpi icon={ShieldCheck} label="Xe chờ phê duyệt" value={String(pending)} sub="hồ sơ cần kiểm tra"
          action={<Button asChild size="sm" variant="outline" className="h-7 text-xs"><Link to="/admin/cars/pending">Duyệt ngay <ArrowRight /></Link></Button>} />
      </div>

      <div className="mt-6 grid gap-4 xl:grid-cols-3">
        <Panel className="xl:col-span-2" title="Doanh thu & lượt thuê thành công — 6 tháng"
          action={
            <ToggleGroup type="single" size="sm" value={series} onValueChange={(v) => v && setSeries(v as never)} variant="outline">
              <ToggleGroupItem value="gmv" className="text-xs">Doanh thu</ToggleGroupItem>
              <ToggleGroupItem value="trips" className="text-xs">Lượt thuê</ToggleGroupItem>
            </ToggleGroup>
          }>
          <div className="h-72 p-4">
            {state === "loading" ? <TableSkeleton rows={5} /> : (
              <ResponsiveContainer>
                <BarChart data={revenue6m} margin={{ left: 8, right: 8 }}>
                  <CartesianGrid vertical={false} stroke="var(--border)" />
                  <XAxis dataKey="month" tickLine={false} axisLine={false} fontSize={12} stroke="var(--muted-foreground)" />
                  <YAxis tickLine={false} axisLine={false} fontSize={12} stroke="var(--muted-foreground)" tickFormatter={(v) => series === "gmv" ? `${v / 1e6}tr` : v} width={56} />
                  <Tooltip cursor={{ fill: "var(--muted)" }} contentStyle={{ borderRadius: 10, border: "1px solid var(--border)", fontSize: 12 }}
                    formatter={(v: number, n) => [series === "gmv" ? vnd(v) : `${v} chuyến`, n]} />
                  <Legend iconType="circle" wrapperStyle={{ fontSize: 12 }} />
                  {series === "gmv" ? (
                    <>
                      <Bar dataKey="gmv" name="GMV" fill="var(--chart-1)" radius={[6, 6, 0, 0]} />
                      <Bar dataKey="commission" name="Hoa hồng sàn" fill="var(--chart-2)" radius={[6, 6, 0, 0]} />
                    </>
                  ) : <Bar dataKey="trips" name="Lượt thuê thành công" fill="var(--chart-1)" radius={[6, 6, 0, 0]} />}
                </BarChart>
              </ResponsiveContainer>
            )}
          </div>
        </Panel>
        <Panel title="Trạng thái đội xe">
          <div className="h-72 p-4">
            <ResponsiveContainer>
              <PieChart>
                <Pie data={fleet} dataKey="value" nameKey="name" innerRadius={55} outerRadius={85} paddingAngle={3}>
                  {fleet.map((f) => <Cell key={f.name} fill={f.color} />)}
                </Pie>
                <Tooltip contentStyle={{ borderRadius: 10, border: "1px solid var(--border)", fontSize: 12 }} formatter={(v: number) => `${v} xe`} />
                <Legend iconType="circle" wrapperStyle={{ fontSize: 12 }} />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </Panel>
      </div>

      <div className="mt-6 grid gap-4 xl:grid-cols-5">
        <Panel className="xl:col-span-3" title="Đơn thuê mới nhất" action={<Link to="/admin/orders" className="text-xs font-medium text-primary hover:underline">Xem tất cả</Link>}>
          {state === "loading" ? <TableSkeleton /> : state === "error" ? <ErrorState onRetry={retry} /> : (
            <Table>
              <TableHeader><TableRow><TableHead className="pl-5">Mã đơn</TableHead><TableHead>Xe thuê</TableHead><TableHead>Khách hàng</TableHead><TableHead>Nhận xe</TableHead><TableHead className="text-right">Tiền cọc</TableHead><TableHead>Trạng thái</TableHead><TableHead /></TableRow></TableHeader>
              <TableBody>
                {latest.map((o) => {
                  const car = cars.find((c) => c.id === o.carId)!;
                  return (
                    <TableRow key={o.id}>
                      <TableCell className="pl-5 font-mono text-xs font-semibold">{o.id}</TableCell>
                      <TableCell className="whitespace-nowrap">{car.name}</TableCell>
                      <TableCell className="whitespace-nowrap">{customerById(o.customerId).name}</TableCell>
                      <TableCell>{dateVN(o.pickup)}</TableCell>
                      <TableCell className="text-right">{vnd(splitDeposit(o.total).deposit)}</TableCell>
                      <TableCell><StatusBadge status={o.status} /></TableCell>
                      <TableCell><Button asChild variant="ghost" size="icon" className="size-8" aria-label="Xem chi tiết"><Link to="/admin/orders/$id" params={{ id: o.id }}><Eye /></Link></Button></TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          )}
        </Panel>
        <Panel className="xl:col-span-2" title="Chủ xe doanh thu cao">
          {state === "loading" ? <TableSkeleton /> : (
            <Table>
              <TableHeader><TableRow><TableHead className="pl-5">Chủ xe</TableHead><TableHead className="text-center">Xe</TableHead><TableHead className="text-center">Chuyến</TableHead><TableHead className="text-right">Doanh thu</TableHead><TableHead className="pr-5 text-right">Đánh giá</TableHead></TableRow></TableHeader>
              <TableBody>
                {topOwners.map((o) => (
                  <TableRow key={o.id}>
                    <TableCell className="pl-5 font-medium">{o.name}</TableCell>
                    <TableCell className="text-center">{o.cars}</TableCell>
                    <TableCell className="text-center">{o.trips}</TableCell>
                    <TableCell className="whitespace-nowrap text-right">{vndShort(o.revenue)}</TableCell>
                    <TableCell className="pr-5 text-right"><span className="inline-flex items-center gap-1"><Star className="size-3.5 fill-warning text-warning" />{o.rating}</span></TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}
        </Panel>
      </div>
    </>
  );
}
