import { Link, useLocation } from "@tanstack/react-router";
import { useState, type ReactNode } from "react";
import {
  Bell, Car, CarFront, ChevronRight, FileBadge, LayoutDashboard, LogOut, Menu, PlusCircle, ShieldCheck, Sparkles, Store, CalendarRange, BadgeCheck,
} from "lucide-react";
import { toast } from "sonner";
import { Sheet, SheetContent } from "@/components/ui/sheet";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { useHost } from "@/lib/host/store";
import { isTrustedHost } from "@/lib/host/rules";
import { formatDateTime } from "@/lib/host/format";
import { DEMO_NOTE } from "@/lib/host/api";
import { cn } from "@/lib/utils";

const groups = [
  {
    label: "Quản trị phương tiện",
    items: [
      { to: "/CarOwner/Home/Index", label: "Tổng quan", icon: LayoutDashboard },
      { to: "/CarOwner/Cars/Index", label: "Quản lý xe", icon: Car, match: "/CarOwner/Cars" },
      { to: "/CarOwner/Cars/Create", label: "Thêm xe mới", icon: PlusCircle, badge: "+ Mới" },
    ],
  },
  {
    label: "Kinh doanh & Thu nhập",
    items: [
      { to: "/CarOwner/Orders/Index", label: "Lịch trình & Đơn xe", icon: CalendarRange, match: "/CarOwner/Orders" },
      { to: "/CarOwner/Register/Pending", label: "Hồ sơ pháp lý & Ngân hàng", icon: FileBadge, match: "/CarOwner/Register" },
    ],
  },
  {
    label: "Sắp ra mắt",
    items: [{ to: "/CarOwner/Roadmap", label: "Tính năng tương lai", icon: Sparkles }],
  },
] as const;

const titles: [string, string, string][] = [
  ["/CarOwner/Home", "Tổng quan", "Tổng quan"],
  ["/CarOwner/Cars/Create", "Thêm xe mới", "Quản lý xe / Thêm xe mới"],
  ["/CarOwner/Cars/Edit", "Chỉnh sửa xe", "Quản lý xe / Chỉnh sửa"],
  ["/CarOwner/Cars/Details", "Chi tiết xe", "Quản lý xe / Chi tiết"],
  ["/CarOwner/Cars", "Quản lý xe", "Quản lý xe"],
  ["/CarOwner/Orders/Details", "Chi tiết đơn thuê", "Lịch trình & Đơn xe / Chi tiết"],
  ["/CarOwner/Orders", "Lịch trình & Đơn xe", "Lịch trình & Đơn xe"],
  ["/CarOwner/Register", "Hồ sơ pháp lý & Ngân hàng", "Hồ sơ đối tác"],
  ["/CarOwner/Roadmap", "Tính năng tương lai", "Roadmap"],
];

function SidebarBody({ onNav }: { onNav?: () => void }) {
  const { owner, cars, reviews } = useHost();
  const path = useLocation({ select: (l) => l.pathname });
  const trips = cars.reduce((s, c) => s + c.completedTrips, 0);
  const avg = reviews.length ? reviews.reduce((s, r) => s + r.stars, 0) / reviews.length : 0;
  const trusted = isTrustedHost(avg, reviews.length, trips, owner.verified === "approved");
  const verifyLabel = { approved: "Đã xác minh", pending: "Đang xét duyệt", rejected: "Cần bổ sung" }[owner.verified];

  return (
    <div className="flex h-full flex-col bg-sidebar text-sidebar-foreground">
      <div className="flex items-center gap-3 px-5 pb-5 pt-6">
        <div className="grid size-10 place-items-center rounded-xl bg-sidebar-primary text-sidebar-primary-foreground">
          <CarFront className="size-5" />
        </div>
        <div>
          <p className="text-[15px] font-bold leading-tight text-sidebar-accent-foreground">ReCar Host Pro</p>
          <p className="text-xs text-sidebar-muted">Đối tác Chủ xe</p>
        </div>
      </div>

      <div className="mx-4 mb-4 rounded-xl bg-sidebar-accent p-3">
        <div className="flex items-center gap-3">
          <div className="grid size-10 shrink-0 place-items-center rounded-full bg-primary text-sm font-bold text-primary-foreground">{owner.initials}</div>
          <div className="min-w-0">
            <p className="truncate text-sm font-semibold text-sidebar-accent-foreground">{owner.fullName}</p>
            <p className="flex items-center gap-1 text-xs text-sidebar-muted">
              <ShieldCheck className="size-3.5" /> {verifyLabel}
            </p>
          </div>
        </div>
        {trusted && (
          <p className="mt-2 inline-flex items-center gap-1 rounded-full bg-sidebar-primary/15 px-2 py-0.5 text-[11px] font-semibold text-sidebar-primary">
            <BadgeCheck className="size-3.5" /> Chủ xe uy tín
          </p>
        )}
      </div>

      <nav className="flex-1 space-y-5 overflow-y-auto px-3">
        {groups.map((g) => (
          <div key={g.label}>
            <p className="px-3 pb-2 text-[11px] font-semibold uppercase tracking-wider text-sidebar-muted">{g.label}</p>
            <ul className="space-y-1">
              {g.items.map((it) => {
                const m = "match" in it ? it.match : it.to;
                const active = it.to === "/CarOwner/Cars/Create" ? path === it.to : path.startsWith(m) && !(m === "/CarOwner/Cars" && path === "/CarOwner/Cars/Create");
                const Icon = it.icon;
                return (
                  <li key={it.to}>
                    <Link
                      to={it.to}
                      onClick={onNav}
                      className={cn(
                        "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors",
                        active ? "bg-sidebar-primary text-sidebar-primary-foreground" : "text-sidebar-foreground hover:bg-sidebar-accent",
                      )}
                    >
                      <Icon className="size-4.5 shrink-0" />
                      <span className="flex-1">{it.label}</span>
                      {"badge" in it && (
                        <span className={cn("rounded-md px-1.5 py-0.5 text-[10px] font-bold", active ? "bg-sidebar/15" : "bg-sidebar-primary/20 text-sidebar-primary")}>{it.badge}</span>
                      )}
                    </Link>
                  </li>
                );
              })}
            </ul>
          </div>
        ))}
      </nav>

      <div className="space-y-1 border-t border-sidebar-border p-3">
        <a href="/" onClick={(e) => { e.preventDefault(); toast.info("Chuyển về sàn khách thuê ReCar", { description: DEMO_NOTE }); }} className="flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium hover:bg-sidebar-accent">
          <Store className="size-4.5" /> Về sàn khách thuê
        </a>
        {/* Production: <form method="post" action="/Account/Logout"> with antiforgery token */}
        <button onClick={() => toast.info("Đăng xuất", { description: "Sẽ gửi POST /Account/Logout khi tích hợp ASP.NET Core." })} className="flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium text-sidebar-muted hover:bg-sidebar-accent hover:text-sidebar-accent-foreground">
          <LogOut className="size-4.5" /> Đăng xuất
        </button>
      </div>
    </div>
  );
}

export function HostShell({ children }: { children: ReactNode }) {
  const [open, setOpen] = useState(false);
  const { owner, notifications, markAllRead } = useHost();
  const path = useLocation({ select: (l) => l.pathname });
  const [, title, crumb] = titles.find(([p]) => path.startsWith(p)) ?? ["", "ReCar Host Pro", ""];
  const unread = notifications.filter((n) => !n.read).length;

  return (
    <div className="min-h-screen">
      <aside className="fixed inset-y-0 left-0 z-30 hidden w-[260px] lg:block">
        <SidebarBody />
      </aside>
      <Sheet open={open} onOpenChange={setOpen}>
        <SheetContent side="left" className="w-[270px] border-0 p-0">
          <SidebarBody onNav={() => setOpen(false)} />
        </SheetContent>
      </Sheet>

      <div className="lg:pl-[260px]">
        <header className="sticky top-0 z-20 flex h-16 items-center gap-3 border-b bg-card/90 px-4 backdrop-blur sm:px-8">
          <button className="rounded-lg p-2 hover:bg-muted lg:hidden" onClick={() => setOpen(true)} aria-label="Mở menu">
            <Menu className="size-5" />
          </button>
          <div className="min-w-0 flex-1">
            <p className="flex items-center gap-1 text-xs text-muted-foreground">
              Host Pro {crumb.split(" / ").map((c) => (<span key={c} className="flex items-center gap-1"><ChevronRight className="size-3" />{c}</span>))}
            </p>
            <p className="truncate text-base font-bold">{title}</p>
          </div>

          {/* SignalR hub (/hubs/carowner-notifications) sẽ đẩy thông báo vào đây khi tích hợp backend */}
          <DropdownMenu>
            <DropdownMenuTrigger className="relative rounded-full p-2.5 hover:bg-muted" aria-label="Thông báo">
              <Bell className="size-5" />
              {unread > 0 && <span className="absolute right-1 top-1 grid min-w-4.5 place-items-center rounded-full bg-destructive px-1 text-[10px] font-bold text-destructive-foreground">{unread}</span>}
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-80">
              <div className="flex items-center justify-between px-2 py-1.5">
                <DropdownMenuLabel className="p-0">Thông báo</DropdownMenuLabel>
                <button onClick={markAllRead} className="text-xs font-medium text-primary">Đánh dấu đã đọc</button>
              </div>
              <DropdownMenuSeparator />
              {notifications.map((n) => (
                <DropdownMenuItem key={n.id} asChild>
                  <Link to={n.orderId ? "/CarOwner/Orders/Details/$id" : "/CarOwner/Cars/Index"} params={{ id: n.orderId ?? "" }} className="flex items-start gap-2 py-2">
                    <span className={cn("mt-1.5 size-2 shrink-0 rounded-full", n.read ? "bg-border" : "bg-primary")} />
                    <span>
                      <span className="block text-sm font-semibold">{n.title}</span>
                      <span className="block text-xs text-muted-foreground">{n.body}</span>
                      <span className="block text-[11px] text-muted-foreground">{formatDateTime(n.at)}</span>
                    </span>
                  </Link>
                </DropdownMenuItem>
              ))}
              <DropdownMenuSeparator />
              <p className="px-2 py-1 text-[11px] text-muted-foreground">Thông báo mẫu — thời gian thực qua SignalR chưa được kết nối.</p>
            </DropdownMenuContent>
          </DropdownMenu>

          <DropdownMenu>
            <DropdownMenuTrigger className="flex items-center gap-2 rounded-full py-1 pl-1 pr-3 hover:bg-muted">
              <span className="grid size-8 place-items-center rounded-full bg-primary text-xs font-bold text-primary-foreground">{owner.initials}</span>
              <span className="hidden text-sm font-semibold sm:block">{owner.fullName}</span>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-52">
              <DropdownMenuItem asChild><Link to="/CarOwner/Register/Pending">Hồ sơ đối tác</Link></DropdownMenuItem>
              <DropdownMenuItem asChild><Link to="/CarOwner/Cars/Index">Đội xe của tôi</Link></DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => toast.info("Đăng xuất", { description: "Sẽ gửi POST /Account/Logout khi tích hợp." })}>Đăng xuất</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </header>

        <div className="border-b bg-warning-soft px-4 py-1.5 text-center text-xs font-medium text-warning sm:px-8">
          Bản prototype — dữ liệu mẫu, thao tác chỉ mô phỏng, chưa lưu vào MySQL.
        </div>
        <main className="mx-auto max-w-[1400px] px-4 py-6 sm:px-8 sm:py-8">{children}</main>
      </div>
    </div>
  );
}
