import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { BadgeCheck, ClipboardList, FileText, Home, Users, Wallet, Car } from "lucide-react";
import { Bars, DashboardShell, Panel, Stat, td, th } from "@/components/site/Dashboard";
import { StatusBadge } from "@/components/site/CarCard";
import { cars, orders, posts, vnd } from "@/lib/data";

export const Route = createFileRoute("/admin")({
  head: () => ({ meta: [
    { title: "Quản trị | ReCar" },
    { name: "description", content: "Bảng quản trị ReCar: người dùng, phê duyệt xe, đơn thuê và nội dung." },
    { property: "og:title", content: "Quản trị | ReCar" },
    { property: "og:description", content: "Phân hệ quản trị ReCar." },
  ] }),
  component: Admin,
});

type Tab = "home" | "approve" | "orders" | "users" | "blogs";
const users = [
  { n: "Hoàng Lan", e: "lan@example.com", r: "Khách hàng", s: "Hoạt động" },
  { n: "Trần Quốc Bảo", e: "bao@example.com", r: "Chủ xe", s: "Hoạt động" },
  { n: "Võ Hoàng Nam", e: "nam@example.com", r: "Chủ xe", s: "Chờ duyệt" },
  { n: "Admin ReCar", e: "admin@example.com", r: "Quản trị viên", s: "Hoạt động" },
];

function Admin() {
  const [tab, setTab] = useState<Tab>("home");
  const [decided, setDecided] = useState<Record<number, string>>({});
  const nav = [
    { label: "Tổng quan", icon: <Home className="h-4 w-4" />, k: "home" },
    { label: "Phê duyệt xe", icon: <BadgeCheck className="h-4 w-4" />, k: "approve" },
    { label: "Đơn thuê", icon: <ClipboardList className="h-4 w-4" />, k: "orders" },
    { label: "Người dùng", icon: <Users className="h-4 w-4" />, k: "users" },
    { label: "Bài viết", icon: <FileText className="h-4 w-4" />, k: "blogs" },
  ].map((n) => ({ ...n, active: tab === n.k, onClick: () => setTab(n.k as Tab) }));
  const titles: Record<Tab, string> = { home: "Tổng quan hệ thống", approve: "Phê duyệt xe", orders: "Quản lý đơn thuê", users: "Quản lý người dùng", blogs: "Quản lý bài viết" };
  const pending = cars.filter((c) => c.status === "Chờ duyệt" || c.id === 6);

  return (
    <DashboardShell area="Quản trị viên" nav={nav} title={titles[tab]}>
      {tab === "home" && (
        <div className="space-y-6">
          <div className="grid gap-4 md:grid-cols-4">
            <Stat label="Doanh thu hệ thống" value={vnd(312500000)} hint="+8% tháng này" icon={<Wallet className="h-5 w-5" />} />
            <Stat label="Người dùng" value="2.418" icon={<Users className="h-5 w-5" />} />
            <Stat label="Xe đang hoạt động" value="186" icon={<Car className="h-5 w-5" />} />
            <Stat label="Chờ phê duyệt" value={String(pending.length)} icon={<BadgeCheck className="h-5 w-5" />} />
          </div>
          <Panel title="Đơn thuê theo tháng"><Bars data={[{ label: "T5", v: 210 }, { label: "T6", v: 260 }, { label: "T7", v: 340 }, { label: "T8", v: 320 }, { label: "T9", v: 290 }, { label: "T10", v: 365 }]} /></Panel>
        </div>
      )}
      {tab === "approve" && (
        <div className="grid gap-4 md:grid-cols-2">
          {pending.map((c) => (
            <div key={c.id} className="flex gap-4 rounded-xl border bg-card p-4 shadow-card">
              <img src={c.image} alt="" className="h-24 w-32 rounded-lg object-cover" />
              <div className="flex-1 space-y-1">
                <p className="font-semibold text-navy">{c.name}</p>
                <p className="text-sm text-muted-foreground">Chủ xe: {c.owner} · {c.city}</p>
                <p className="text-sm">{vnd(c.price)}/ngày</p>
                {decided[c.id] ? <StatusBadge s={decided[c.id]} /> : (
                  <div className="flex gap-2 pt-1">
                    <button onClick={() => setDecided({ ...decided, [c.id]: "Đã duyệt" })} className="rounded-lg bg-success px-3 py-1.5 text-xs font-semibold text-primary-foreground">Phê duyệt</button>
                    <button onClick={() => setDecided({ ...decided, [c.id]: "Từ chối" })} className="rounded-lg border border-destructive px-3 py-1.5 text-xs font-semibold text-destructive">Từ chối</button>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
      {tab === "orders" && (
        <Panel title="Tất cả đơn thuê">
          <table className="w-full"><thead className="bg-secondary"><tr><th className={th}>Mã</th><th className={th}>Xe</th><th className={th}>Khách</th><th className={th}>Tổng</th><th className={th}>Cọc</th><th className={th}>Trạng thái</th></tr></thead>
            <tbody>{orders.map((o) => <tr key={o.id} className="border-t"><td className={td}>#{o.id}</td><td className={td}>{o.car}</td><td className={td}>{o.customer}</td><td className={td}>{vnd(o.total)}</td><td className={td}>{vnd(o.deposit)}</td><td className={td}><StatusBadge s={o.status} /></td></tr>)}</tbody></table>
        </Panel>
      )}
      {tab === "users" && (
        <Panel title="Người dùng">
          <table className="w-full"><thead className="bg-secondary"><tr><th className={th}>Họ tên</th><th className={th}>Email</th><th className={th}>Vai trò</th><th className={th}>Trạng thái</th></tr></thead>
            <tbody>{users.map((u) => <tr key={u.e} className="border-t"><td className={td}>{u.n}</td><td className={td}>{u.e}</td><td className={td}>{u.r}</td><td className={td}><StatusBadge s={u.s === "Hoạt động" ? "Sẵn sàng" : u.s} /></td></tr>)}</tbody></table>
        </Panel>
      )}
      {tab === "blogs" && (
        <Panel title="Bài viết" action={<button className="rounded-lg bg-primary px-3 py-1.5 text-sm font-semibold text-primary-foreground">+ Viết bài</button>}>
          <table className="w-full"><thead className="bg-secondary"><tr><th className={th}>Tiêu đề</th><th className={th}>Chuyên mục</th><th className={th}>Ngày đăng</th></tr></thead>
            <tbody>{posts.map((p) => <tr key={p.id} className="border-t"><td className={td}>{p.title}</td><td className={td}>{p.cat}</td><td className={td}>{p.date}</td></tr>)}</tbody></table>
        </Panel>
      )}
    </DashboardShell>
  );
}
