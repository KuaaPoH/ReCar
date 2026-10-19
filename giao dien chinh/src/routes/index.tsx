import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { useState } from "react";
import { BadgeCheck, CalendarCheck, Car, FileCheck2, Headphones, KeyRound, Search, Truck, Wallet } from "lucide-react";
import { SiteLayout } from "@/components/site/Layout";
import { CarCard } from "@/components/site/CarCard";
import { carTypes, cars, cities, images, posts } from "@/lib/data";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "ReCar – Chủ động mọi hành trình | Thuê ô tô trực tuyến" },
      { name: "description", content: "Tìm và đặt thuê ô tô tự lái hoặc có tài xế, đặt cọc 30% trực tuyến cùng ReCar." },
      { property: "og:title", content: "ReCar – Chủ động mọi hành trình" },
      { property: "og:description", content: "Khám phá hàng trăm mẫu xe phù hợp với nhu cầu di chuyển của bạn." },
    ],
  }),
  component: Home,
});

const today = () => new Date().toISOString().slice(0, 10);
const plus = (d: number) => new Date(Date.now() + d * 864e5).toISOString().slice(0, 10);
const field = "w-full rounded-lg border bg-background px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-ring";

function SearchForm() {
  const nav = useNavigate();
  const [f, setF] = useState({ city: "Hà Nội", from: plus(1), fromT: "08:00", to: plus(3), toT: "08:00", mode: "Tự lái" });
  const [err, setErr] = useState("");
  const set = (k: keyof typeof f) => (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => setF({ ...f, [k]: e.target.value });
  const submit = (e: React.FormEvent) => {
    e.preventDefault();
    if (new Date(`${f.to}T${f.toT}`) <= new Date(`${f.from}T${f.fromT}`)) return setErr("Thời gian trả xe phải sau thời gian nhận xe.");
    setErr("");
    nav({ to: "/product", search: { city: f.city, from: f.from, to: f.to, mode: f.mode } });
  };
  return (
    <form onSubmit={submit} className="rounded-2xl border bg-card p-4 shadow-float md:p-6">
      <div className="mb-4 flex gap-2">
        {["Tự lái", "Có tài xế"].map((m) => (
          <button type="button" key={m} onClick={() => setF({ ...f, mode: m })} className={`rounded-full px-4 py-1.5 text-sm font-medium ${f.mode === m ? "bg-navy text-navy-foreground" : "bg-secondary"}`}>{m}</button>
        ))}
      </div>
      <div className="grid gap-3 md:grid-cols-[1.3fr_1fr_0.7fr_1fr_0.7fr_auto] md:items-end">
        <L label="Địa điểm nhận xe"><select className={field} value={f.city} onChange={set("city")}>{cities.map((c) => <option key={c}>{c}</option>)}</select></L>
        <L label="Ngày nhận xe"><input type="date" min={today()} className={field} value={f.from} onChange={set("from")} /></L>
        <L label="Giờ nhận"><input type="time" className={field} value={f.fromT} onChange={set("fromT")} /></L>
        <L label="Ngày trả xe"><input type="date" min={f.from} className={field} value={f.to} onChange={set("to")} /></L>
        <L label="Giờ trả"><input type="time" className={field} value={f.toT} onChange={set("toT")} /></L>
        <button className="flex items-center justify-center gap-2 rounded-lg bg-primary px-6 py-2.5 font-semibold text-primary-foreground hover:opacity-90"><Search className="h-4 w-4" />Tìm kiếm xe</button>
      </div>
      {err && <p className="mt-3 text-sm text-destructive">{err}</p>}
    </form>
  );
}

function L({ label, children }: { label: string; children: React.ReactNode }) {
  return <label className="block space-y-1"><span className="text-xs font-medium text-muted-foreground">{label}</span>{children}</label>;
}

const perks = [
  { i: Car, t: "Đa dạng lựa chọn xe", d: "Từ xe mini đến SUV 7 chỗ." },
  { i: CalendarCheck, t: "Đặt xe trực tuyến", d: "Vài bước, xác nhận nhanh." },
  { i: Headphones, t: "Hỗ trợ 24/7", d: "Luôn sẵn sàng khi bạn cần." },
  { i: BadgeCheck, t: "Minh bạch", d: "Thông tin xe và chủ xe rõ ràng." },
  { i: Truck, t: "Giao xe tận nơi", d: "Theo điều kiện từng chủ xe." },
  { i: FileCheck2, t: "Điều khoản rõ ràng", d: "Hợp đồng và chính sách cụ thể." },
];

function Home() {
  return (
    <SiteLayout>
      <section className="relative">
        <img src={images.hero} alt="SUV trên đại lộ thành phố" width={1920} height={1088} className="absolute inset-0 h-full w-full object-cover" />
        <div className="absolute inset-0 bg-gradient-to-r from-navy via-navy/70 to-transparent" />
        <div className="relative mx-auto max-w-7xl px-4 pb-40 pt-20 md:pt-28">
          <p className="mb-4 inline-block rounded-full bg-primary/20 px-4 py-1 text-sm font-medium text-navy-foreground">Chuyến đi của bạn, lựa chọn của bạn.</p>
          <h1 className="max-w-2xl text-4xl font-extrabold leading-tight text-navy-foreground md:text-6xl">Chủ động mọi hành trình cùng ReCar</h1>
          <p className="mt-5 max-w-lg text-lg text-navy-foreground/80">Khám phá hàng trăm mẫu xe phù hợp với nhu cầu di chuyển của bạn.</p>
        </div>
      </section>
      <div className="relative z-10 mx-auto -mt-28 max-w-7xl px-4"><SearchForm /></div>

      <section className="mx-auto mt-16 grid max-w-7xl grid-cols-2 gap-4 px-4 md:grid-cols-3 lg:grid-cols-6">
        {perks.map(({ i: I, t, d }) => (
          <div key={t} className="rounded-xl bg-secondary p-4">
            <I className="h-6 w-6 text-primary" />
            <p className="mt-3 text-sm font-semibold text-navy">{t}</p>
            <p className="mt-1 text-xs text-muted-foreground">{d}</p>
          </div>
        ))}
      </section>

      <Section title="Danh mục dòng xe">
        <div className="grid grid-cols-2 gap-4 md:grid-cols-5">
          {carTypes.map((c) => (
            <Link key={c.key} to="/product" search={{ type: c.key }} className="group overflow-hidden rounded-xl border bg-card shadow-card">
              <img src={c.image} alt={c.label} loading="lazy" width={1024} height={768} className="aspect-[4/3] w-full object-cover transition group-hover:scale-105" />
              <div className="flex items-center justify-between p-3"><span className="text-sm font-semibold text-navy">{c.label}</span><span className="text-xs text-primary">Xem xe →</span></div>
            </Link>
          ))}
        </div>
      </Section>

      <Section title="Xe nổi bật và ưu đãi" link>
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">{cars.filter((c) => c.status !== "Chờ duyệt").slice(0, 4).map((c) => <CarCard key={c.id} car={c} />)}</div>
      </Section>

      <Section title="Thuê xe chỉ với 3 bước">
        <div className="grid gap-6 md:grid-cols-3">
          {[
            { i: Search, t: "Chọn xe và thời gian thuê" },
            { i: Wallet, t: "Đặt cọc 30% trực tuyến" },
            { i: KeyRound, t: "Nhận xe, kiểm tra giấy tờ và ký hợp đồng" },
          ].map(({ i: I, t }, idx) => (
            <div key={t} className="relative rounded-xl border bg-card p-6 shadow-card">
              <span className="text-5xl font-extrabold text-primary/15">0{idx + 1}</span>
              <I className="mt-2 h-7 w-7 text-primary" />
              <p className="mt-3 font-semibold text-navy">{t}</p>
            </div>
          ))}
        </div>
      </Section>

      <Section title="Cẩm nang và tin tức">
        <div className="grid gap-6 md:grid-cols-3">
          {posts.map((p) => (
            <article key={p.id} className="overflow-hidden rounded-xl border bg-card shadow-card">
              <img src={p.image} alt={p.title} loading="lazy" className="aspect-video w-full object-cover" />
              <div className="p-5">
                <p className="text-xs text-muted-foreground">{p.cat} · {p.date}</p>
                <h3 className="mt-2 font-semibold text-navy">{p.title}</h3>
                <p className="mt-2 text-sm text-muted-foreground">{p.excerpt}</p>
                <Link to="/blog" className="mt-3 inline-block text-sm font-medium text-primary">Đọc thêm →</Link>
              </div>
            </article>
          ))}
        </div>
      </Section>

      <section className="mx-auto mt-20 max-w-7xl px-4">
        <div className="grid items-center gap-8 overflow-hidden rounded-2xl bg-navy p-8 md:grid-cols-2 md:p-12">
          <div>
            <h2 className="text-3xl font-bold text-navy-foreground">Bạn có ô tô? Hãy để ReCar giúp bạn khai thác giá trị chiếc xe.</h2>
            <p className="mt-4 text-navy-foreground/75">Đăng ký xe cho thuê, nhận đơn và theo dõi doanh thu ngay trên tài khoản ReCar của bạn.</p>
            <Link to="/account" className="mt-6 inline-block rounded-lg bg-primary px-6 py-3 font-semibold text-primary-foreground">Tìm hiểu cách trở thành chủ xe</Link>
          </div>
          <img src={images.suv} alt="SUV" loading="lazy" className="rounded-xl" />
        </div>
      </section>
    </SiteLayout>
  );
}

function Section({ title, children, link }: { title: string; children: React.ReactNode; link?: boolean }) {
  return (
    <section className="mx-auto mt-20 max-w-7xl px-4">
      <div className="mb-6 flex items-end justify-between">
        <h2 className="text-2xl font-bold text-navy md:text-3xl">{title}</h2>
        {link && <Link to="/product" className="text-sm font-medium text-primary">Xem tất cả →</Link>}
      </div>
      {children}
    </section>
  );
}
