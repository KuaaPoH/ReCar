import { Link } from "@tanstack/react-router";
import { Fuel, MapPin, Settings2, Star, Users } from "lucide-react";
import { type Car, vnd } from "@/lib/data";

export function CarCard({ car }: { car: Car }) {
  const tagCls = car.tag === "Xe điện" ? "bg-success" : car.tag === "Ưu đãi" ? "bg-warning" : "bg-primary";
  return (
    <article className="group overflow-hidden rounded-xl border bg-card shadow-card transition hover:-translate-y-1 hover:shadow-float">
      <div className="relative aspect-[4/3] overflow-hidden bg-muted">
        <img src={car.image} alt={car.name} loading="lazy" width={1024} height={768} className="h-full w-full object-cover transition duration-500 group-hover:scale-105" />
        {car.tag && <span className={`absolute left-3 top-3 rounded-full px-3 py-1 text-xs font-semibold text-primary-foreground ${tagCls}`}>{car.tag}</span>}
      </div>
      <div className="space-y-3 p-4">
        <div>
          <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">{car.brand}</p>
          <h3 className="font-semibold text-navy">{car.name}</h3>
        </div>
        <div className="grid grid-cols-3 gap-2 text-xs text-muted-foreground">
          <span className="flex items-center gap-1"><Users className="h-3.5 w-3.5" />{car.seats} chỗ</span>
          <span className="flex items-center gap-1"><Settings2 className="h-3.5 w-3.5" />{car.gear.replace("Số ", "")}</span>
          <span className="flex items-center gap-1"><Fuel className="h-3.5 w-3.5" />{car.fuel}</span>
        </div>
        <div className="flex items-center justify-between text-xs">
          <span className="flex items-center gap-1 text-muted-foreground"><MapPin className="h-3.5 w-3.5" />{car.district}, {car.city}</span>
          <span className="flex items-center gap-1 font-medium"><Star className="h-3.5 w-3.5 fill-warning text-warning" />{car.rating} ({car.reviews})</span>
        </div>
        <div className="flex items-end justify-between border-t pt-3">
          <p><span className="text-lg font-bold text-primary">{vnd(car.price)}</span><span className="text-xs text-muted-foreground">/ngày</span></p>
        </div>
        <div className="grid grid-cols-2 gap-2">
          <Link to="/product/$id" params={{ id: String(car.id) }} className="rounded-lg border px-3 py-2 text-center text-sm font-medium hover:bg-secondary">Xem chi tiết</Link>
          <Link to="/checkout/$id" params={{ id: String(car.id) }} className="rounded-lg bg-primary px-3 py-2 text-center text-sm font-semibold text-primary-foreground hover:opacity-90">Thuê ngay</Link>
        </div>
      </div>
    </article>
  );
}

export function StatusBadge({ s }: { s: string }) {
  const map: Record<string, string> = {
    "Sẵn sàng": "bg-success/15 text-success", "Hoàn thành": "bg-success/15 text-success", "Đã duyệt": "bg-success/15 text-success",
    "Đã đặt cọc": "bg-primary/15 text-primary", "Đang thuê": "bg-primary/15 text-primary", "Đang cho thuê": "bg-primary/15 text-primary",
    "Chờ xác nhận": "bg-warning/20 text-warning", "Chờ duyệt": "bg-warning/20 text-warning",
    "Đã hủy": "bg-destructive/15 text-destructive", "Tạm ngưng": "bg-destructive/15 text-destructive", "Từ chối": "bg-destructive/15 text-destructive",
  };
  return <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-semibold ${map[s] ?? "bg-muted text-muted-foreground"}`}>{s}</span>;
}
