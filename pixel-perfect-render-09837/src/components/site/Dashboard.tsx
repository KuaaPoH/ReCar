import { Link } from "@tanstack/react-router";
import type { ReactNode } from "react";
import { Logo } from "./Layout";

export type DashNav = { label: string; icon: ReactNode; active?: boolean; onClick?: () => void };

export function DashboardShell({ area, nav, children, title }: { area: string; nav: DashNav[]; children: ReactNode; title: string }) {
  return (
    <div className="min-h-screen bg-secondary md:flex">
      <aside className="bg-navy text-navy-foreground md:w-64 md:min-h-screen md:sticky md:top-0 p-4 space-y-6">
        <div className="flex items-center justify-between md:block">
          <Logo light />
          <p className="mt-1 text-xs uppercase tracking-widest opacity-60">{area}</p>
        </div>
        <nav className="flex md:flex-col gap-1 overflow-x-auto">
          {nav.map((n) => (
            <button key={n.label} onClick={n.onClick} className={`flex shrink-0 items-center gap-3 rounded-lg px-3 py-2 text-sm text-left ${n.active ? "bg-primary text-primary-foreground" : "opacity-80 hover:bg-navy-foreground/10"}`}>{n.icon}{n.label}</button>
          ))}
        </nav>
        <Link to="/" className="hidden md:block text-sm opacity-70 hover:opacity-100">← Về trang ReCar</Link>
      </aside>
      <div className="flex-1 p-4 md:p-8">
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-2xl font-bold text-navy">{title}</h1>
          <span className="rounded-full bg-warning/20 px-3 py-1 text-xs font-medium text-warning">Dữ liệu mẫu</span>
        </div>
        {children}
      </div>
    </div>
  );
}

export function Stat({ label, value, hint, icon }: { label: string; value: string; hint?: string; icon: ReactNode }) {
  return (
    <div className="rounded-xl border bg-card p-5 shadow-card">
      <div className="flex items-center justify-between text-muted-foreground text-sm">{label}<span className="text-primary">{icon}</span></div>
      <p className="mt-2 text-2xl font-bold text-navy">{value}</p>
      {hint && <p className="mt-1 text-xs text-success">{hint}</p>}
    </div>
  );
}

export function Panel({ title, children, action }: { title: string; children: ReactNode; action?: ReactNode }) {
  return (
    <section className="rounded-xl border bg-card shadow-card">
      <div className="flex items-center justify-between border-b px-5 py-4"><h2 className="font-semibold text-navy">{title}</h2>{action}</div>
      <div className="overflow-x-auto">{children}</div>
    </section>
  );
}

export function Bars({ data }: { data: { label: string; v: number }[] }) {
  const max = Math.max(...data.map((d) => d.v));
  return (
    <div className="flex h-48 items-end gap-3 p-5">
      {data.map((d) => (
        <div key={d.label} className="flex flex-1 flex-col items-center gap-2">
          <div className="w-full rounded-t-md bg-primary/80" style={{ height: `${(d.v / max) * 100}%` }} />
          <span className="text-xs text-muted-foreground">{d.label}</span>
        </div>
      ))}
    </div>
  );
}

export const th = "px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-muted-foreground";
export const td = "px-5 py-3 text-sm";
