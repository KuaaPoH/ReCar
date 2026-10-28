import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import { CarFront, MapPin, Pencil, Star, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/host/PageHeader";
import { CarStatusBadge, OrderStatusBadge } from "@/components/host/StatusBadge";
import { RemoveCarDialog } from "@/components/host/RemoveCarDialog";
import { useHost } from "@/lib/host/store";
import { formatDate, formatDateTime, formatVND } from "@/lib/host/format";

export const Route = createFileRoute("/CarOwner/Cars/Details/$id")({
  head: () => ({
    meta: [
      { title: "Chi tiết xe — ReCar Host Pro" },
      { name: "description", content: "Thông số, giá thuê, tiện nghi, đánh giá và lịch sử của xe." },
      { property: "og:title", content: "Chi tiết xe — ReCar Host Pro" },
      { property: "og:description", content: "Hồ sơ chi tiết xe cho thuê trên ReCar." },
    ],
  }),
  component: CarDetails,
});

function CarDetails() {
  const { id } = Route.useParams();
  const { cars, orders, reviews } = useHost();
  const [removing, setRemoving] = useState(false);
  const car = cars.find((c) => String(c.id) === id);
  if (!car)
    return <div className="card-surface"><EmptyState icon={<CarFront />} title="Không tìm thấy xe" body="Xe không tồn tại hoặc không thuộc quyền quản lý của bạn." action={<Button asChild><Link to="/CarOwner/Cars/Index">Về danh sách xe</Link></Button>} /></div>;
  const rv = reviews.filter((r) => r.carId === car.id);
  const avg = rv.length ? (rv.reduce((s, r) => s + r.stars, 0) / rv.length).toFixed(1) : null;
  const carOrders = orders.filter((o) => o.carId === car.id).sort((a, b) => b.createdAt.localeCompare(a.createdAt));
  const specs = [["Hãng", car.brand], ["Phân khúc", car.segment], ["Năm SX", car.year], ["Màu", car.color], ["Số chỗ", car.seats], ["Hộp số", car.transmission], ["Nhiên liệu", car.fuel], ["Tiêu thụ", car.consumption], ["Giới hạn", `${car.kmLimitPerDay} km/ngày`], ["Vượt km", `${formatVND(car.overKmFee)}/km`], ["Giao xe", car.delivery ? formatVND(car.deliveryFee) : "Không hỗ trợ"]];

  return (
    <div className="space-y-6">
      {car.status === "rejected" && car.rejectReason && (
        <div className="rounded-xl bg-destructive-soft p-4 text-sm text-destructive"><b>Lý do từ chối:</b> {car.rejectReason}</div>
      )}
      <div className="grid gap-6 xl:grid-cols-[1.4fr_1fr]">
        <div className="card-surface overflow-hidden">
          <img src={car.image} alt={car.name} className="aspect-[16/9] w-full object-cover" />
          <div className="grid grid-cols-4 gap-2 p-3">
            {[0, 1, 2, 3].map((i) => <img key={i} src={car.image} alt="" className="aspect-[4/3] rounded-lg object-cover opacity-80" style={{ objectPosition: `${i * 30}% 50%` }} loading="lazy" />)}
          </div>
        </div>
        <div className="card-surface space-y-4 p-6">
          <div className="flex items-start justify-between gap-3">
            <div>
              <h1 className="text-2xl font-bold">{car.name}</h1>
              <span className="plate mt-1 inline-block">{car.plate}</span>
            </div>
            <CarStatusBadge status={car.status} />
          </div>
          <p className="flex items-center gap-1 text-sm text-muted-foreground"><MapPin className="size-4" />{car.location}</p>
          <div className="flex items-baseline gap-2">
            <span className="text-3xl font-bold text-primary">{formatVND(car.saleEnabled && car.salePrice ? car.salePrice : car.pricePerDay)}</span>
            {car.saleEnabled && car.salePrice && <span className="text-muted-foreground line-through">{formatVND(car.pricePerDay)}</span>}
            <span className="text-sm text-muted-foreground">/ngày</span>
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div className="rounded-xl bg-muted p-3"><p className="text-xs text-muted-foreground">Chuyến hoàn thành</p><p className="text-xl font-bold">{car.completedTrips}</p></div>
            <div className="rounded-xl bg-muted p-3"><p className="text-xs text-muted-foreground">Đánh giá</p><p className="flex items-center gap-1 text-xl font-bold">{avg ? <><Star className="size-4 fill-warning text-warning" />{avg}</> : "—"}</p></div>
          </div>
          <div className="flex flex-wrap gap-2">
            <Button asChild><Link to="/CarOwner/Cars/Edit/$id" params={{ id }}><Pencil /> Chỉnh sửa</Link></Button>
            <Button variant="outline" className="text-destructive" onClick={() => setRemoving(true)}><Trash2 /> Gỡ xe</Button>
          </div>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="card-surface p-6">
          <h2 className="mb-4 font-bold">Thông số kỹ thuật</h2>
          <dl className="grid grid-cols-2 gap-x-6 gap-y-3 text-sm">
            {specs.map(([k, v]) => <div key={k as string}><dt className="text-xs text-muted-foreground">{k}</dt><dd className="font-semibold">{v}</dd></div>)}
          </dl>
          <h3 className="mb-2 mt-6 font-bold">Tiện nghi</h3>
          <div className="flex flex-wrap gap-2">{car.amenities.map((a) => <span key={a} className="rounded-full bg-primary-soft px-3 py-1 text-xs font-medium text-accent-foreground">{a}</span>)}</div>
        </div>
        <div className="card-surface p-6">
          <h2 className="mb-4 font-bold">Đánh giá của khách ({rv.length})</h2>
          {rv.length === 0 ? <p className="text-sm text-muted-foreground">Chưa có đánh giá nào.</p> : (
            <ul className="space-y-4">
              {rv.map((r) => (
                <li key={r.id} className="border-b pb-3 last:border-0">
                  <div className="flex items-center justify-between"><p className="font-semibold">{r.guest}</p><span className="text-xs text-muted-foreground">{formatDate(r.date)}</span></div>
                  <p className="flex gap-0.5">{Array.from({ length: 5 }, (_, i) => <Star key={i} className={i < r.stars ? "size-3.5 fill-warning text-warning" : "size-3.5 text-border"} />)}</p>
                  <p className="mt-1 text-sm">{r.comment}</p>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>

      <div className="card-surface p-6">
        <h2 className="mb-4 font-bold">Lịch sử đơn thuê của xe</h2>
        {carOrders.length === 0 ? <p className="text-sm text-muted-foreground">Xe chưa có đơn thuê.</p> : (
          <ul className="divide-y text-sm">
            {carOrders.map((o) => (
              <li key={o.id} className="flex flex-wrap items-center justify-between gap-2 py-3">
                <Link to="/CarOwner/Orders/Details/$id" params={{ id: o.id }} className="font-mono font-semibold text-primary">{o.id}</Link>
                <span>{o.guestName}</span>
                <span className="text-muted-foreground">{formatDateTime(o.pickupAt)} → {formatDateTime(o.returnAt)}</span>
                <OrderStatusBadge status={o.status} />
              </li>
            ))}
          </ul>
        )}
      </div>
      <RemoveCarDialog car={car} open={removing} onOpenChange={setRemoving} />
    </div>
  );
}
