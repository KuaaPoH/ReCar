import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { SiteLayout } from "@/components/site/Layout";
import { StatusBadge } from "@/components/site/CarCard";
import { orders, vnd } from "@/lib/data";

export const Route = createFileRoute("/account")({
  head: () => ({ meta: [
    { title: "Tài khoản của tôi | ReCar" },
    { name: "description", content: "Quản lý đơn thuê xe, hợp đồng và hồ sơ cá nhân trên ReCar." },
    { property: "og:title", content: "Tài khoản của tôi | ReCar" },
    { property: "og:description", content: "Đơn thuê, hợp đồng và hồ sơ cá nhân." },
  ] }),
  component: Account,
});

const tabs = ["Đơn thuê xe", "Hồ sơ cá nhân", "Trở thành chủ xe"] as const;
const inp = "w-full rounded-lg border px-3 py-2.5 text-sm";

function Account() {
  const [t, setT] = useState<(typeof tabs)[number]>("Đơn thuê xe");
  const [sent, setSent] = useState(false);
  return (
    <SiteLayout>
      <div className="mx-auto max-w-6xl px-4 py-10">
        <div className="flex items-center gap-4">
          <span className="grid h-16 w-16 place-items-center rounded-full bg-navy text-xl font-bold text-navy-foreground">HL</span>
          <div><h1 className="text-2xl font-bold text-navy">Hoàng Lan</h1><p className="text-sm text-muted-foreground">Thành viên từ 03/2025 · Tài khoản mẫu</p></div>
        </div>
        <div className="mt-8 flex gap-2 overflow-x-auto border-b">
          {tabs.map((x) => <button key={x} onClick={() => setT(x)} className={`shrink-0 border-b-2 px-4 py-3 text-sm font-medium ${t === x ? "border-primary text-primary" : "border-transparent text-muted-foreground"}`}>{x}</button>)}
        </div>
        <div className="mt-6">
          {t === "Đơn thuê xe" && (
            <div className="space-y-3">
              {orders.map((o) => (
                <div key={o.id} className="flex flex-wrap items-center justify-between gap-4 rounded-xl border p-5 shadow-card">
                  <div><p className="text-xs text-muted-foreground">#{o.id}</p><p className="font-semibold text-navy">{o.car}</p><p className="text-sm text-muted-foreground">{o.from} → {o.to}</p></div>
                  <div className="text-right"><p className="font-semibold">{vnd(o.total)}</p><p className="text-xs text-muted-foreground">Đã cọc {vnd(o.deposit)}</p></div>
                  <StatusBadge s={o.status} />
                  <button className="rounded-lg border px-3 py-2 text-sm">Xem hợp đồng</button>
                </div>
              ))}
            </div>
          )}
          {t === "Hồ sơ cá nhân" && (
            <form className="grid max-w-2xl gap-4 md:grid-cols-2" onSubmit={(e) => e.preventDefault()}>
              <input className={inp} defaultValue="Hoàng Lan" /><input className={inp} defaultValue="0901 234 567" />
              <input className={inp} defaultValue="lan@example.com" /><input className={inp} placeholder="Số GPLX" />
              <button className="w-fit rounded-lg bg-primary px-5 py-2.5 font-semibold text-primary-foreground">Lưu thay đổi</button>
            </form>
          )}
          {t === "Trở thành chủ xe" && (sent ? (
            <div className="rounded-xl border bg-warning/10 p-6"><StatusBadge s="Chờ duyệt" /><p className="mt-3 font-semibold text-navy">Hồ sơ đã được gửi</p><p className="text-sm text-muted-foreground">Quản trị viên sẽ xem xét và phản hồi qua email.</p></div>
          ) : (
            <form className="grid max-w-2xl gap-4" onSubmit={(e) => { e.preventDefault(); setSent(true); }}>
              <p className="text-muted-foreground">Đăng ký để đưa xe của bạn lên ReCar. Hồ sơ cần được quản trị viên phê duyệt.</p>
              <input required className={inp} placeholder="Họ tên chủ xe" /><input required className={inp} placeholder="Số CCCD" />
              <input required className={inp} placeholder="Địa chỉ giao nhận xe" />
              <label className="text-sm">Ảnh CCCD / Giấy tờ xe<input type="file" className="mt-1 block text-sm" /></label>
              <button className="w-fit rounded-lg bg-primary px-5 py-2.5 font-semibold text-primary-foreground">Gửi hồ sơ đăng ký</button>
            </form>
          ))}
        </div>
      </div>
    </SiteLayout>
  );
}
