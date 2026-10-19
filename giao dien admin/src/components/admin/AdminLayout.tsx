import { useMemo, useState, type ReactNode } from "react";
import { Link, useNavigate, useRouterState } from "@tanstack/react-router";
import {
  LayoutDashboard, ShieldCheck, CarFront, ClipboardList, Wallet, Users, Newspaper, Settings,
  ArrowLeftCircle, LogOut, Bell, Search, Menu, ChevronRight, UserRound, KeyRound,
} from "lucide-react";
import { toast } from "sonner";
import { Sheet, SheetContent, SheetTitle } from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { cn } from "@/lib/utils";
import { update, useStore } from "@/data/store";
import { CURRENT_ADMIN, customerById, ownerById } from "@/data/demo";
import { ConfirmDialog } from "./shared";

const NAV = [
  { to: "/admin", label: "Tổng quan", icon: LayoutDashboard, exact: true },
  { to: "/admin/cars/pending", label: "Phê duyệt xe", icon: ShieldCheck, badge: true },
  { to: "/admin/cars", label: "Quản lý đội xe", icon: CarFront },
  { to: "/admin/orders", label: "Đơn thuê xe", icon: ClipboardList },
  { to: "/admin/finance", label: "Tài chính & đối soát", icon: Wallet },
  { to: "/admin/users", label: "Người dùng & đối tác", icon: Users },
  { to: "/admin/blog", label: "Bài viết & tin tức", icon: Newspaper },
  { to: "/admin/settings", label: "Cấu hình hệ thống", icon: Settings },
] as const;

function useActive() {
  const path = useRouterState({ select: (s) => s.location.pathname });
  return (to: string, exact?: boolean) => {
    if (exact) return path === to || path === to + "/";
    if (to === "/admin/cars") return path === to || (path.startsWith("/admin/cars/") && !path.startsWith("/admin/cars/pending"));
    return path === to || path.startsWith(to + "/");
  };
}

function SidebarBody({ onNavigate, onLogout }: { onNavigate?: () => void; onLogout: () => void }) {
  const isActive = useActive();
  const pending = useStore((s) => s.cars.filter((c) => c.status === "pending").length);
  return (
    <div className="flex h-full flex-col bg-sidebar text-sidebar-foreground">
      <div className="px-6 pb-5 pt-6">
        <div className="flex items-center gap-2">
          <div className="grid size-9 place-items-center rounded-lg bg-primary text-primary-foreground"><CarFront className="size-5" /></div>
          <div>
            <p className="text-lg font-extrabold leading-none tracking-tight text-sidebar-accent-foreground">ReCar</p>
            <p className="mt-1 text-[10px] font-semibold tracking-[0.2em] text-sidebar-primary">ADMIN PORTAL</p>
          </div>
        </div>
        <p className="mt-4 inline-flex rounded-md bg-sidebar-accent px-2 py-1 text-xs text-sidebar-muted">Vai trò: Quản trị viên</p>
      </div>
      <nav className="flex-1 space-y-0.5 overflow-y-auto px-3">
        {NAV.map((n) => {
          const active = isActive(n.to, "exact" in n && n.exact);
          return (
            <Link
              key={n.to}
              to={n.to}
              onClick={onNavigate}
              className={cn(
                "relative flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors",
                active ? "bg-sidebar-accent text-sidebar-accent-foreground" : "text-sidebar-muted hover:bg-sidebar-accent/60 hover:text-sidebar-accent-foreground",
              )}
            >
              {active && <span className="absolute inset-y-2 left-0 w-1 rounded-r bg-sidebar-primary" />}
              <n.icon className={cn("size-4", active && "text-sidebar-primary")} />
              <span className="flex-1">{n.label}</span>
              {"badge" in n && pending > 0 && (
                <span className="rounded-full bg-warning px-2 py-0.5 text-[11px] font-bold text-sidebar">{pending}</span>
              )}
            </Link>
          );
        })}
      </nav>
      <div className="space-y-1 border-t border-sidebar-border p-3">
        <button onClick={() => toast.info("Chuyển về trang khách hàng ReCar (/) trong bản tích hợp.")} className="flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm text-sidebar-muted hover:bg-sidebar-accent hover:text-sidebar-accent-foreground">
          <ArrowLeftCircle className="size-4" /> Quay về sàn ReCar
        </button>
        <button onClick={onLogout} className="flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm text-sidebar-muted hover:bg-sidebar-accent hover:text-sidebar-accent-foreground">
          <LogOut className="size-4" /> Đăng xuất
        </button>
      </div>
    </div>
  );
}

const CRUMBS: Record<string, string> = {
  admin: "Tổng quan", cars: "Quản lý đội xe", pending: "Phê duyệt xe", orders: "Đơn thuê xe", finance: "Tài chính & đối soát",
  users: "Người dùng & đối tác", blog: "Bài viết & tin tức", settings: "Cấu hình hệ thống", new: "Tạo mới",
};

function Breadcrumbs() {
  const path = useRouterState({ select: (s) => s.location.pathname });
  const parts = path.split("/").filter(Boolean);
  return (
    <nav className="hidden items-center gap-1.5 text-sm md:flex">
      <span className="text-muted-foreground">Admin</span>
      {parts.slice(parts[1] ? 1 : 0).map((p, i, arr) => (
        <span key={i} className="flex items-center gap-1.5">
          <ChevronRight className="size-3.5 text-muted-foreground" />
          <span className={i === arr.length - 1 ? "font-medium" : "text-muted-foreground"}>{CRUMBS[p] ?? p}</span>
        </span>
      ))}
    </nav>
  );
}

function GlobalSearch() {
  const [q, setQ] = useState("");
  const [open, setOpen] = useState(false);
  const navigate = useNavigate();
  const { orders, cars, customers, owners } = useStore((s) => s);
  const results = useMemo(() => {
    const k = q.trim().toLowerCase();
    if (k.length < 2) return [];
    const r: { label: string; sub: string; go: () => void }[] = [];
    orders.filter((o) => o.id.toLowerCase().includes(k)).slice(0, 3).forEach((o) => r.push({ label: o.id, sub: "Đơn thuê", go: () => navigate({ to: "/admin/orders/$id", params: { id: o.id } }) }));
    cars.filter((c) => c.plate.toLowerCase().replace(/[-.]/g, "").includes(k.replace(/[-.]/g, "")) || c.name.toLowerCase().includes(k)).slice(0, 3).forEach((c) => r.push({ label: `${c.name} · ${c.plate}`, sub: "Xe", go: () => navigate({ to: c.status === "pending" ? "/admin/cars/pending" : "/admin/cars", search: { q: c.plate } as never }) }));
    customers.filter((c) => c.name.toLowerCase().includes(k)).slice(0, 3).forEach((c) => r.push({ label: c.name, sub: "Khách hàng", go: () => navigate({ to: "/admin/users", search: { q: c.name } as never }) }));
    owners.filter((c) => c.name.toLowerCase().includes(k)).slice(0, 3).forEach((c) => r.push({ label: c.name, sub: "Chủ xe", go: () => navigate({ to: "/admin/users", search: { tab: "owners", q: c.name } as never }) }));
    return r;
  }, [q, orders, cars, customers, owners, navigate]);
  void customerById; void ownerById;
  return (
    <Popover open={open && q.length >= 2} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <div className="relative w-full max-w-md">
          <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input value={q} onChange={(e) => { setQ(e.target.value); setOpen(true); }} onFocus={() => setOpen(true)} placeholder="Tìm mã đơn, biển số, khách hàng, chủ xe…" className="h-9 bg-background pl-9" />
        </div>
      </PopoverTrigger>
      <PopoverContent align="start" className="w-[var(--radix-popover-trigger-width)] p-1" onOpenAutoFocus={(e) => e.preventDefault()}>
        {results.length === 0 ? (
          <p className="px-3 py-4 text-center text-sm text-muted-foreground">Không tìm thấy kết quả cho “{q}”</p>
        ) : results.map((r, i) => (
          <button key={i} onClick={() => { r.go(); setOpen(false); setQ(""); }} className="flex w-full items-center justify-between rounded-md px-3 py-2 text-left text-sm hover:bg-muted">
            <span className="truncate font-medium">{r.label}</span>
            <span className="ml-2 shrink-0 text-xs text-muted-foreground">{r.sub}</span>
          </button>
        ))}
      </PopoverContent>
    </Popover>
  );
}

function Notifications() {
  const items = useStore((s) => s.notifications);
  const unread = items.filter((n) => !n.read).length;
  const navigate = useNavigate();
  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button variant="ghost" size="icon" className="relative" aria-label="Thông báo">
          <Bell />
          {unread > 0 && <span className="absolute right-1 top-1 grid size-4 place-items-center rounded-full bg-destructive text-[10px] font-bold text-destructive-foreground">{unread}</span>}
        </Button>
      </PopoverTrigger>
      <PopoverContent align="end" className="w-80 p-0">
        <div className="flex items-center justify-between border-b px-4 py-3">
          <p className="text-sm font-semibold">Thông báo</p>
          <button className="text-xs text-primary hover:underline" onClick={() => update((s) => ({ notifications: s.notifications.map((n) => ({ ...n, read: true })) }))}>Đánh dấu đã đọc</button>
        </div>
        {items.map((n) => (
          <button key={n.id} onClick={() => { update((s) => ({ notifications: s.notifications.map((x) => x.id === n.id ? { ...x, read: true } : x) })); navigate({ to: n.to }); }} className="flex w-full gap-3 border-b px-4 py-3 text-left last:border-0 hover:bg-muted">
            <span className={cn("mt-1.5 size-2 shrink-0 rounded-full", n.read ? "bg-transparent" : "bg-primary")} />
            <span className="min-w-0">
              <span className="block text-sm font-medium">{n.title}</span>
              <span className="block truncate text-xs text-muted-foreground">{n.body}</span>
              <span className="mt-0.5 block text-[11px] text-muted-foreground">{n.at}</span>
            </span>
          </button>
        ))}
      </PopoverContent>
    </Popover>
  );
}

export function AdminLayout({ children }: { children: ReactNode }) {
  const [mobile, setMobile] = useState(false);
  const [logout, setLogout] = useState(false);
  return (
    <div className="min-h-screen w-full">
      <aside className="fixed inset-y-0 left-0 z-30 hidden w-[260px] lg:block"><SidebarBody onLogout={() => setLogout(true)} /></aside>
      <Sheet open={mobile} onOpenChange={setMobile}>
        <SheetContent side="left" className="w-[260px] border-0 p-0">
          <SheetTitle className="sr-only">Điều hướng</SheetTitle>
          <SidebarBody onNavigate={() => setMobile(false)} onLogout={() => { setMobile(false); setLogout(true); }} />
        </SheetContent>
      </Sheet>
      <div className="lg:pl-[260px]">
        <header className="sticky top-0 z-20 flex h-16 items-center gap-3 border-b bg-card/95 px-4 backdrop-blur md:px-6">
          <Button variant="ghost" size="icon" className="lg:hidden" onClick={() => setMobile(true)} aria-label="Mở menu"><Menu /></Button>
          <Breadcrumbs />
          <div className="flex flex-1 justify-end md:justify-center"><GlobalSearch /></div>
          <Notifications />
          <DropdownMenu>
            <DropdownMenuTrigger className="flex items-center gap-2.5 rounded-lg p-1 pr-2 hover:bg-muted">
              <span className="grid size-8 place-items-center rounded-full bg-primary text-xs font-bold text-primary-foreground">TH</span>
              <span className="hidden text-left sm:block">
                <span className="block text-sm font-semibold leading-tight">{CURRENT_ADMIN.name}</span>
                <span className="block text-[11px] text-muted-foreground">Super Admin</span>
              </span>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-52">
              <DropdownMenuLabel className="font-normal"><p className="text-sm font-medium">{CURRENT_ADMIN.name}</p><p className="text-xs text-muted-foreground">{CURRENT_ADMIN.email}</p></DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => toast.info("Hồ sơ cá nhân: trang /Admin/Profile trong bản tích hợp.")}><UserRound /> Hồ sơ cá nhân</DropdownMenuItem>
              <DropdownMenuItem onClick={() => toast.success("Đã gửi liên kết đổi mật khẩu tới email của bạn (mô phỏng).")}><KeyRound /> Đổi mật khẩu</DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => setLogout(true)} className="text-destructive"><LogOut /> Đăng xuất</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </header>
        <main className="mx-auto max-w-[1440px] p-4 md:p-6 lg:p-8">{children}</main>
      </div>
      <ConfirmDialog open={logout} onOpenChange={setLogout} title="Đăng xuất khỏi Admin Portal?" desc="Phiên làm việc hiện tại sẽ kết thúc." confirmLabel="Đăng xuất" onConfirm={() => { toast.success("Đã đăng xuất (mô phỏng). Bản thật chuyển về /Account/Login."); }} />
    </div>
  );
}
