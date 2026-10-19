import { createFileRoute, Link, notFound } from "@tanstack/react-router";
import { Check, Fuel, MapPin, Settings2, ShieldCheck, Star, Users } from "lucide-react";
import { SiteLayout } from "@/components/site/Layout";
import { CarCard } from "@/components/site/CarCard";
import { cars, DEPOSIT_RATE, vnd } from "@/lib/data";

export const Route = createFileRoute("/product/$id")({
  loader: ({ params }) => {
    const car = cars.find((c) => String(c.id) === params.id);
    if (!car) throw notFound();
    return { car };
  },
  head: ({ loaderData }) => ({ meta: [
    { title: loaderData ? `${loaderData.car.name} – Thuê xe | ReCar` : "Không tìm thấy xe | ReCar" },
    { name: "description", content: loaderData ? `Thuê ${loaderData.car.name} tại ${loaderData.car.city}, giá ${vnd(loaderData.car.price)}/ngày.` : "Xe không tồn tại." },
    { property: "og:title", content: loaderData?.car.name ?? "ReCar" },
    { property: "og:description", content: "Chi tiết xe cho thuê trên ReCar." },
  ] }),
  notFoundComponent: () => <SiteLayout><div className="p-20 text-center"><p className="text-xl font-semibold">Không tìm thấy xe</p><Link to="/product" className="text-primary">Quay lại danh sách</Link></div></SiteLayout>,
  component: Detail,
});

function Detail() {
  const { car } = Route.useLoaderData();
  const days = 3;
  const total = car.price * days;
  return (
    <SiteLayout>
      <div className="mx-auto max-w-7xl px-4 py-8">
        <p className="text-sm text-muted-foreground"><Link to="/">Trang chủ</Link> / <Link to="/product">Thuê xe</Link> / {car.name}</p>
        <div className="mt-6 grid gap-8 lg:grid-cols-[1fr_380px]">
          <div className="space-y-8">
            <div className="grid grid-cols-4 gap-3">
              <img src={car.image} alt={car.name} className="col-span-4 aspect-[16/10] w-full rounded-2xl object-cover" />
            </div>
            <div>
              <p className="text-sm uppercase tracking-wide text-muted-foreground">{car.brand}</p>
              <h1 className="text-3xl font-bold text-navy">{car.name}</h1>
              <p className="mt-2 flex items-center gap-4 text-sm text-muted-foreground">
                <span className="flex items-center gap-1"><Star className="h-4 w-4 fill-warning text-warning" />{car.rating} · {car.reviews} đánh giá</span>
                <span className="flex items-center gap-1"><MapPin className="h-4 w-4" />{car.district}, {car.city}</span>
              </p>
            </div>
            <div className="grid grid-cols-2 gap-4 md:grid-cols-4">
              {[{ i: Users, t: `${car.seats} chỗ` }, { i: Settings2, t: car.gear }, { i: Fuel, t: car.fuel }, { i: ShieldCheck, t: "Đã xác minh" }].map(({ i: I, t }) => (
                <div key={t} className="rounded-xl bg-secondary p-4"><I className="h-5 w-5 text-primary" /><p className="mt-2 text-sm font-medium">{t}</p></div>
              ))}
            </div>
            <div><h2 className="mb-3 text-lg font-semibold text-navy">Tiện nghi</h2>
              <div className="grid grid-cols-2 gap-2 md:grid-cols-3">{car.features.map((f) => <p key={f} className="flex items-center gap-2 text-sm"><Check className="h-4 w-4 text-success" />{f}</p>)}</div>
            </div>
            <div><h2 className="mb-3 text-lg font-semibold text-navy">Giấy tờ thuê xe</h2>
              <p className="text-sm text-muted-foreground">CCCD gắn chip và Giấy phép lái xe hạng B trở lên. Đối chiếu bản gốc khi nhận xe.</p>
            </div>
            <div className="flex items-center gap-4 rounded-xl border p-5">
              <span className="grid h-12 w-12 place-items-center rounded-full bg-navy font-semibold text-navy-foreground">{car.owner.split(" ").pop()?.[0]}</span>
              <div><p className="text-sm text-muted-foreground">Chủ xe</p><p className="font-semibold text-navy">{car.owner}</p></div>
            </div>
          </div>
          <aside className="h-fit space-y-4 rounded-2xl border bg-card p-6 shadow-float lg:sticky lg:top-24">
            <p><span className="text-3xl font-bold text-primary">{vnd(car.price)}</span><span className="text-muted-foreground">/ngày</span></p>
            <div className="space-y-2 border-y py-4 text-sm">
              <Row a={`${vnd(car.price)} × ${days} ngày`} b={vnd(total)} />
              <Row a="Tổng tiền thuê" b={vnd(total)} bold />
              <Row a="Tiền đặt cọc (30%)" b={vnd(total * DEPOSIT_RATE)} />
            </div>
            <Link to="/checkout/$id" params={{ id: String(car.id) }} className="block rounded-lg bg-primary py-3 text-center font-semibold text-primary-foreground">Thuê ngay</Link>
            <p className="text-center text-xs text-muted-foreground">Số tiền minh họa. Hệ thống sẽ tính lại khi xác nhận đơn.</p>
          </aside>
        </div>
        <h2 className="mb-6 mt-16 text-2xl font-bold text-navy">Xe tương tự</h2>
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">{cars.filter((c) => c.id !== car.id && c.status !== "Chờ duyệt").slice(0, 4).map((c) => <CarCard key={c.id} car={c} />)}</div>
      </div>
    </SiteLayout>
  );
}

export function Row({ a, b, bold }: { a: string; b: string; bold?: boolean }) {
  return <div className={`flex justify-between ${bold ? "font-semibold text-navy" : "text-muted-foreground"}`}><span>{a}</span><span>{b}</span></div>;
}
