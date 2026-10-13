import { Link } from "@tanstack/react-router";
import { useState, type ReactNode } from "react";
import { Bell, Car, Menu, X, ChevronDown, LayoutDashboard, Shield, LogOut, User, FileText, KeyRound } from "lucide-react";

export function Logo({ light = false }: { light?: boolean }) {
  return (
    <Link to="/" className="flex items-center gap-2 font-bold text-xl tracking-tight">
      <span className="grid h-9 w-9 place-items-center rounded-lg bg-primary text-primary-foreground"><Car className="h-5 w-5" /></span>
      <span className={light ? "text-navy-foreground" : "text-navy"}>Re<span className="text-primary">Car</span></span>
    </Link>
  );
}

const nav = [
  { to: "/", label: "Trang chủ" },
  { to: "/product", label: "Thuê xe" },
  { to: "/blog", label: "Cẩm nang" },
  { to: "/news", label: "Tin tức" },
  { to: "/contact", label: "Liên hệ" },
] as const;

export function Header() {
  const [open, setOpen] = useState(false);
  const [menu, setMenu] = useState(false);
  return (
    <header className="sticky top-0 z-40 border-b bg-background/90 backdrop-blur">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4">
        <Logo />
        <nav className="hidden md:flex items-center gap-1">
          {nav.map((n) => (
            <Link key={n.to} to={n.to} activeOptions={{ exact: n.to === "/" }} className="rounded-md px-3 py-2 text-sm font-medium text-muted-foreground hover:text-navy" activeProps={{ className: "text-navy bg-secondary" }}>{n.label}</Link>
          ))}
        </nav>
        <div className="hidden md:flex items-center gap-3">
          <button aria-label="Thông báo" className="relative rounded-full p-2 hover:bg-secondary"><Bell className="h-5 w-5 text-navy" /><span className="absolute right-1.5 top-1.5 h-2 w-2 rounded-full bg-destructive" /></button>
          <div className="relative">
            <button onClick={() => setMenu(!menu)} className="flex items-center gap-2 rounded-full border py-1 pl-1 pr-3 hover:bg-secondary">
              <span className="grid h-8 w-8 place-items-center rounded-full bg-navy text-xs font-semibold text-navy-foreground">HL</span>
              <span className="text-sm font-medium">Hoàng Lan</span><ChevronDown className="h-4 w-4" />
            </button>
            {menu && (
              <div className="absolute right-0 mt-2 w-60 rounded-xl border bg-popover p-2 shadow-float" onClick={() => setMenu(false)}>
                <MenuItem to="/account" icon={<User className="h-4 w-4" />}>Tài khoản của tôi</MenuItem>
                <MenuItem to="/account" icon={<FileText className="h-4 w-4" />}>Đơn thuê xe</MenuItem>
                <MenuItem to="/car-owner" icon={<LayoutDashboard className="h-4 w-4" />}>Trang chủ xe</MenuItem>
                <MenuItem to="/admin" icon={<Shield className="h-4 w-4" />}>Quản trị (demo)</MenuItem>
                <div className="my-1 border-t" />
                <MenuItem to="/login" icon={<KeyRound className="h-4 w-4" />}>Đăng nhập / Đăng ký</MenuItem>
                <MenuItem to="/" icon={<LogOut className="h-4 w-4" />}>Đăng xuất</MenuItem>
              </div>
            )}
          </div>
        </div>
        <button className="md:hidden p-2" onClick={() => setOpen(!open)} aria-label="Menu">{open ? <X /> : <Menu />}</button>
      </div>
      {open && (
        <div className="md:hidden border-t px-4 py-3 space-y-1" onClick={() => setOpen(false)}>
          {nav.map((n) => <Link key={n.to} to={n.to} className="block rounded-md px-3 py-2 font-medium hover:bg-secondary">{n.label}</Link>)}
          <Link to="/account" className="block rounded-md px-3 py-2 font-medium hover:bg-secondary">Tài khoản của tôi</Link>
          <Link to="/car-owner" className="block rounded-md px-3 py-2 font-medium hover:bg-secondary">Trang chủ xe</Link>
          <Link to="/admin" className="block rounded-md px-3 py-2 font-medium hover:bg-secondary">Quản trị (demo)</Link>
          <Link to="/login" className="block rounded-md px-3 py-2 font-medium text-primary">Đăng nhập</Link>
        </div>
      )}
    </header>
  );
}

function MenuItem({ to, icon, children }: { to: "/account" | "/car-owner" | "/admin" | "/login" | "/"; icon: ReactNode; children: ReactNode }) {
  return <Link to={to} className="flex items-center gap-3 rounded-md px-3 py-2 text-sm hover:bg-secondary">{icon}{children}</Link>;
}

export function Footer() {
  return (
    <footer className="mt-24 bg-navy text-navy-foreground">
      <div className="mx-auto grid max-w-7xl gap-10 px-4 py-14 md:grid-cols-4">
        <div className="space-y-3">
          <Logo light />
          <p className="text-sm opacity-70">Chuyến đi của bạn, lựa chọn của bạn. Nền tảng kết nối khách thuê và chủ xe.</p>
        </div>
        <FooterCol title="Khám phá" items={["Thuê xe", "Cẩm nang", "Tin tức", "Trở thành chủ xe"]} />
        <FooterCol title="Chính sách" items={["Chính sách thuê xe", "Điều khoản sử dụng", "Chính sách quyền riêng tư", "Hỗ trợ khách hàng"]} />
        <div className="space-y-2 text-sm">
          <h4 className="font-semibold">Liên hệ <span className="ml-1 rounded bg-warning/20 px-1.5 py-0.5 text-[10px] text-warning">Dữ liệu mẫu</span></h4>
          <p className="opacity-70">Hotline: 1900 0000</p>
          <p className="opacity-70">Email: hotro@recar.example</p>
        </div>
      </div>
      <div className="border-t border-navy-foreground/10 py-5 text-center text-xs opacity-60">© 2026 ReCar. Bản mẫu giao diện.</div>
    </footer>
  );
}

function FooterCol({ title, items }: { title: string; items: string[] }) {
  return (
    <div className="space-y-2 text-sm">
      <h4 className="font-semibold">{title}</h4>
      {items.map((i) => <p key={i} className="opacity-70 hover:opacity-100 cursor-pointer">{i}</p>)}
    </div>
  );
}

export function SiteLayout({ children }: { children: ReactNode }) {
  return <div className="min-h-screen flex flex-col"><Header /><main className="flex-1">{children}</main><Footer /></div>;
}

export function PageTitle({ title, sub }: { title: string; sub?: string }) {
  return (
    <div className="bg-secondary">
      <div className="mx-auto max-w-7xl px-4 py-10">
        <h1 className="text-3xl font-bold text-navy">{title}</h1>
        {sub && <p className="mt-2 text-muted-foreground">{sub}</p>}
      </div>
    </div>
  );
}
