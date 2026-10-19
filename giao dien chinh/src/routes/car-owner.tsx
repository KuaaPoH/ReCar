import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { Car, ClipboardList, Home, Plus, Wallet, TrendingUp } from "lucide-react";
import { Bars, DashboardShell, Panel, Stat, td, th } from "@/components/site/Dashboard";
import { StatusBadge } from "@/components/site/CarCard";
import { cars, orders, vnd } from "@/lib/data";

export const Route = createFileRoute("/car-owner")({
  head: () => ({ meta: [
    { title: "Trang chủ xe | ReCar" },
    { name: "description", content: "Quản lý xe, đơn thuê và doanh thu dành cho chủ xe ReCar." },
    { property: "og:title", content: "Trang chủ xe | ReCar" },
    { property: "og:description", content: "Phân hệ chủ xe ReCar." },
  ] }),
  component: Owner,
});

type Tab = "home" | "cars" | "create" | "orders";
const inp = "w-full rounded-lg border px-3 py-2.5 text-sm";

function Owner() {
  const [tab, setTab] = useState<Tab>("home");
  const mine = cars.filter((c) => ["Nguyễn Minh Anh", "Trần Quốc Bảo"].includes(c.owner));
  const nav = [
    { label: "Tổng quan", icon: <Home className="h-4 w-4" />, k: "home" },
    { label: "Quản lý xe", icon: <Car className="h-4 w-4" />, k: "cars" },
    { label: "Thêm xe mới", icon: <Plus className="h-4 w-4" />, k: "create" },
    { label: "Đơn thuê xe", icon: <ClipboardList className="h-4 w-4" />, k: "orders" },
  ].map((n) => ({ ...n, active: tab === n.k, onClick: () => setTab(n.k as Tab) }));
  const titles: Record<Tab, string> = { home: "Tổng quan", cars: "Quản lý xe", create: "Thêm xe mới", orders: "Đơn thuê xe" };

  return (
    <DashboardShell area="Chủ xe" nav={nav} title={titles[tab]}>
      {tab === "home" && (
        <div className="space-y-6">
          <div className="grid gap-4 md:grid-cols-4">
            <Stat label="Doanh thu tháng" value={vnd(18450000)} hint="+12% so với tháng trước" icon={<Wallet className="h-5 w-5" />} />
            <Stat label="Xe đang hoạt động" value={String(mine.length)} icon={<Car className="h-5 w-5" />} />
            <Stat label="Đơn chờ xác nhận" value="1" icon={<ClipboardList className="h-5 w-5" />} />
            <Stat label="Đánh giá trung bình" value="4.8" icon={<TrendingUp className="h-5 w-5" />} />
          </div>
          <Panel title="Doanh thu 6 tháng (triệu đồng)"><Bars data={[{ label: "T5", v: 9 }, { label: "T6", v: 12 }, { label: "T7", v: 15 }, { label: "T8", v: 14 }, { label: "T9", v: 16.5 }, { label: "T10", v: 18.4 }]} /></Panel>
          <OrdersTable />
        </div>
      )}
      {tab === "cars" && (
        <Panel title="Xe của tôi" action={<button onClick={() => setTab("create")} className="rounded-lg bg-primary px-3 py-1.5 text-sm font-semibold text-primary-foreground">+ Thêm xe</button>}>
          <table className="w-full"><thead className="bg-secondary"><tr><th className={th}>Xe</th><th className={th}>Khu vực</th><th className={th}>Giá/ngày</th><th className={th}>Trạng thái</th><th className={th}></th></tr></thead>
            <tbody>{mine.map((c) => (
              <tr key={c.id} className="border-t"><td className={td}><div className="flex items-center gap-3"><img src={c.image} alt="" className="h-10 w-14 rounded object-cover" />{c.name}</div></td><td className={td}>{c.city}</td><td className={td}>{vnd(c.price)}</td><td className={td}><StatusBadge s={c.status} /></td><td className={td}><button className="text-primary">Sửa</button></td></tr>
            ))}</tbody></table>
        </Panel>
      )}
      {tab === "create" && (
        <form className="grid max-w-3xl gap-4 rounded-xl border bg-card p-6 shadow-card md:grid-cols-2" onSubmit={(e) => { e.preventDefault(); setTab("cars"); }}>
          <input required className={inp} placeholder="Tên xe (VD: Toyota Vios G)" />
          <select className={inp}><option>Hãng xe</option>{["VinFast", "Toyota", "Mazda", "Hyundai", "Kia"].map((b) => <option key={b}>{b}</option>)}</select>
          <input required className={inp} placeholder="Biển số xe" />
          <input required type="number" min={300000} className={inp} placeholder="Giá thuê/ngày (đ)" />
          <select className={inp}><option>Số tự động</option><option>Số sàn</option></select>
          <select className={inp}><option>Xăng</option><option>Dầu</option><option>Điện</option></select>
          <textarea className={`${inp} md:col-span-2`} rows={3} placeholder="Mô tả xe" />
          <label className="text-sm md:col-span-2">Ảnh xe & giấy tờ<input type="file" multiple className="mt-1 block" /></label>
          <p className="text-xs text-muted-foreground md:col-span-2">Xe mới sẽ ở trạng thái “Chờ duyệt” cho đến khi quản trị viên phê duyệt.</p>
          <button className="w-fit rounded-lg bg-primary px-5 py-2.5 font-semibold text-primary-foreground">Gửi duyệt</button>
        </form>
      )}
      {tab === "orders" && <OrdersTable actions />}
    </DashboardShell>
  );
}

function OrdersTable({ actions }: { actions?: boolean }) {
  return (
    <Panel title="Đơn thuê gần đây">
      <table className="w-full"><thead className="bg-secondary"><tr><th className={th}>Mã</th><th className={th}>Xe</th><th className={th}>Khách</th><th className={th}>Thời gian</th><th className={th}>Tổng</th><th className={th}>Trạng thái</th>{actions && <th className={th}></th>}</tr></thead>
        <tbody>{orders.map((o) => (
          <tr key={o.id} className="border-t"><td className={td}>#{o.id}</td><td className={td}>{o.car}</td><td className={td}>{o.customer}</td><td className={`${td} whitespace-nowrap text-muted-foreground`}>{o.from}</td><td className={td}>{vnd(o.total)}</td><td className={td}><StatusBadge s={o.status} /></td>
            {actions && <td className={td}>{o.status === "Chờ xác nhận" && <div className="flex gap-2"><button className="rounded bg-success px-2 py-1 text-xs text-primary-foreground">Xác nhận</button><button className="rounded bg-destructive px-2 py-1 text-xs text-destructive-foreground">Từ chối</button></div>}</td>}</tr>
        ))}</tbody></table>
    </Panel>
  );
}
