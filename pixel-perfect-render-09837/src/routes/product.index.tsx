import { createFileRoute, Link } from "@tanstack/react-router";
import { useMemo, useState } from "react";
import { SlidersHorizontal, SearchX } from "lucide-react";
import { SiteLayout } from "@/components/site/Layout";
import { CarCard } from "@/components/site/CarCard";
import { brands, carTypes, cars, vnd } from "@/lib/data";

type S = { city?: string; from?: string; to?: string; mode?: string; type?: string };

export const Route = createFileRoute("/product/")({
  validateSearch: (s: Record<string, unknown>): S => ({
    city: s.city as string | undefined, from: s.from as string | undefined, to: s.to as string | undefined,
    mode: s.mode as string | undefined, type: s.type as string | undefined,
  }),
  head: () => ({ meta: [
    { title: "Danh sách xe cho thuê | ReCar" },
    { name: "description", content: "Lọc xe theo giá, hãng, số chỗ, nhiên liệu và khu vực trên ReCar." },
    { property: "og:title", content: "Danh sách xe cho thuê | ReCar" },
    { property: "og:description", content: "Tìm xe phù hợp với hành trình của bạn." },
  ] }),
  component: ProductList,
});

function ProductList() {
  const s = Route.useSearch();
  const [max, setMax] = useState(5000000);
  const [sel, setSel] = useState<string[]>([]);
  const [type, setType] = useState(s.type ?? "");
  const [seats, setSeats] = useState(0);
  const [fuel, setFuel] = useState("");
  const [sort, setSort] = useState("pop");
  const [show, setShow] = useState(false);

  const list = useMemo(() => {
    let r = cars.filter((c) => c.status !== "Chờ duyệt" && c.price <= max && (!sel.length || sel.includes(c.brand)) && (!type || c.type === type) && (!seats || c.seats === seats) && (!fuel || c.fuel === fuel) && (!s.city || c.city === s.city));
    if (sort === "asc") r = [...r].sort((a, b) => a.price - b.price);
    if (sort === "desc") r = [...r].sort((a, b) => b.price - a.price);
    if (sort === "rate") r = [...r].sort((a, b) => b.rating - a.rating);
    return r;
  }, [max, sel, type, seats, fuel, sort, s.city]);

  const reset = () => { setMax(5000000); setSel([]); setType(""); setSeats(0); setFuel(""); };
  const chip = (on: boolean) => `rounded-full border px-3 py-1 text-xs font-medium ${on ? "border-primary bg-primary text-primary-foreground" : "hover:bg-secondary"}`;

  return (
    <SiteLayout>
      <div className="bg-secondary">
        <div className="mx-auto flex max-w-7xl flex-wrap items-end justify-between gap-4 px-4 py-8">
          <div>
            <h1 className="text-3xl font-bold text-navy">Danh sách xe cho thuê</h1>
            <p className="mt-2 text-sm text-muted-foreground">
              {s.city ?? "Tất cả khu vực"}{s.from && ` · ${s.from.split("-").reverse().join("/")} → ${s.to?.split("-").reverse().join("/")}`}{s.mode && ` · ${s.mode}`} · <b className="text-navy">{list.length} xe phù hợp</b>
            </p>
          </div>
          <Link to="/" className="rounded-lg border bg-background px-4 py-2 text-sm font-medium">Sửa tiêu chí tìm kiếm</Link>
        </div>
      </div>
      <div className="mx-auto grid max-w-7xl gap-8 px-4 py-8 lg:grid-cols-[260px_1fr]">
        <button className="flex items-center gap-2 rounded-lg border px-4 py-2 text-sm lg:hidden" onClick={() => setShow(!show)}><SlidersHorizontal className="h-4 w-4" />Bộ lọc</button>
        <aside className={`${show ? "block" : "hidden"} space-y-6 lg:block`}>
          <F t={`Giá tối đa: ${vnd(max)}/ngày`}><input type="range" min={300000} max={5000000} step={50000} value={max} onChange={(e) => setMax(+e.target.value)} className="w-full accent-[var(--primary)]" /></F>
          <F t="Hãng xe"><div className="flex flex-wrap gap-2">{brands.map((b) => <button key={b} className={chip(sel.includes(b))} onClick={() => setSel(sel.includes(b) ? sel.filter((x) => x !== b) : [...sel, b])}>{b}</button>)}</div></F>
          <F t="Dòng xe"><div className="flex flex-wrap gap-2"><button className={chip(!type)} onClick={() => setType("")}>Tất cả</button>{carTypes.map((c) => <button key={c.key} className={chip(type === c.key)} onClick={() => setType(c.key)}>{c.label}</button>)}</div></F>
          <F t="Số chỗ"><div className="flex gap-2">{[0, 5, 7].map((n) => <button key={n} className={chip(seats === n)} onClick={() => setSeats(n)}>{n ? `${n} chỗ` : "Tất cả"}</button>)}</div></F>
          <F t="Nhiên liệu"><div className="flex gap-2">{["", "Xăng", "Dầu", "Điện"].map((n) => <button key={n} className={chip(fuel === n)} onClick={() => setFuel(n)}>{n || "Tất cả"}</button>)}</div></F>
          <button onClick={reset} className="text-sm font-medium text-primary">Xóa bộ lọc</button>
        </aside>
        <div>
          <div className="mb-4 flex justify-end">
            <select value={sort} onChange={(e) => setSort(e.target.value)} className="rounded-lg border px-3 py-2 text-sm">
              <option value="pop">Phổ biến</option><option value="asc">Giá thấp đến cao</option><option value="desc">Giá cao đến thấp</option><option value="rate">Đánh giá cao</option>
            </select>
          </div>
          {list.length ? (
            <div className="grid gap-6 sm:grid-cols-2 xl:grid-cols-3">{list.map((c) => <CarCard key={c.id} car={c} />)}</div>
          ) : (
            <div className="rounded-xl border border-dashed p-12 text-center">
              <SearchX className="mx-auto h-10 w-10 text-muted-foreground" />
              <p className="mt-3 font-semibold text-navy">Không tìm thấy xe phù hợp</p>
              <p className="text-sm text-muted-foreground">Hãy thử nới rộng khoảng giá hoặc bỏ bớt bộ lọc.</p>
              <button onClick={reset} className="mt-4 rounded-lg bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground">Xóa bộ lọc</button>
            </div>
          )}
        </div>
      </div>
    </SiteLayout>
  );
}

function F({ t, children }: { t: string; children: React.ReactNode }) {
  return <div className="space-y-3 border-b pb-5"><p className="text-sm font-semibold text-navy">{t}</p>{children}</div>;
}
