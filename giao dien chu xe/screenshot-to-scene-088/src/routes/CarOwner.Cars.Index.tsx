import { createFileRoute, Link } from "@tanstack/react-router";
import { useMemo, useState } from "react";
import { toast } from "sonner";
import { CarFront, Eye, MapPin, MoreHorizontal, Pencil, Plus, Search, Star, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { PageHeader, EmptyState, Pager } from "@/components/host/PageHeader";
import { CarStatusBadge } from "@/components/host/StatusBadge";
import { RemoveCarDialog } from "@/components/host/RemoveCarDialog";
import { useHost } from "@/lib/host/store";
import { canToggleActive, CAR_STATUS_LABEL } from "@/lib/host/rules";
import { formatVND } from "@/lib/host/format";
import { DEMO_NOTE } from "@/lib/host/api";
import type { Car, CarStatus } from "@/lib/host/types";

export const Route = createFileRoute("/CarOwner/Cars/Index")({
  head: () => ({
    meta: [
      { title: "Quản lý xe — ReCar Host Pro" },
      { name: "description", content: "Danh sách xe, trạng thái, giá thuê và thao tác nhanh cho chủ xe." },
      { property: "og:title", content: "Quản lý xe — ReCar Host Pro" },
      { property: "og:description", content: "Quản lý đội xe cho thuê trên ReCar." },
    ],
  }),
  component: CarsIndex,
});

const PAGE = 6;
const ALL = "all";

function CarsIndex() {
  const { cars, orders, reviews, updateCar } = useHost();
  const [q, setQ] = useState("");
  const [brand, setBrand] = useState(ALL);
  const [segment, setSegment] = useState(ALL);
  const [status, setStatus] = useState(ALL);
  const [area, setArea] = useState(ALL);
  const [sort, setSort] = useState("newest");
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState<string | null>(null);
  const [removing, setRemoving] = useState<Car | null>(null);

  const uniq = (k: keyof Car) => [...new Set(cars.map((c) => String(c[k])))];
  const areaOf = (c: Car) => c.location.split(",").slice(-1)[0].trim();
  const rating = (id: number) => {
    const r = reviews.filter((x) => x.carId === id);
    return r.length ? (r.reduce((s, x) => s + x.stars, 0) / r.length).toFixed(1) : null;
  };

  const list = useMemo(() => {
    const t = q.trim().toLowerCase();
    const l = cars.filter((c) =>
      (!t || c.name.toLowerCase().includes(t) || c.plate.toLowerCase().replace(/[-.]/g, "").includes(t.replace(/[-.]/g, ""))) &&
      (brand === ALL || c.brand === brand) && (segment === ALL || c.segment === segment) &&
      (status === ALL || c.status === status) && (area === ALL || areaOf(c) === area),
    );
    const price = (c: Car) => (c.saleEnabled && c.salePrice ? c.salePrice : c.pricePerDay);
    return l.sort((a, b) =>
      sort === "newest" ? b.createdAt.localeCompare(a.createdAt) : sort === "priceAsc" ? price(a) - price(b) : sort === "priceDesc" ? price(b) - price(a) : b.completedTrips - a.completedTrips,
    );
  }, [cars, q, brand, segment, status, area, sort]);
  const pages = Math.max(1, Math.ceil(list.length / PAGE));
  const shown = list.slice((page - 1) * PAGE, page * PAGE);

  const toggleActive = async (c: Car) => {
    const chk = canToggleActive(c, orders);
    if (!chk.ok) return toast.error("Không thể đổi trạng thái", { description: chk.reason });
    setLoading(`s${c.id}`);
    await new Promise((r) => setTimeout(r, 500)); // POST ToggleStatus
    const next: CarStatus = c.status === "available" ? "paused" : "available";
    updateCar(c.id, { status: next });
    setLoading(null);
    toast.success(`${c.name}: ${CAR_STATUS_LABEL[next]}`, { description: DEMO_NOTE });
  };
  const toggleSale = async (c: Car) => {
    if (!c.salePrice) return toast.error("Chưa thiết lập giá khuyến mãi", { description: "Vào Chỉnh sửa để nhập giá khuyến mãi/ngày." });
    setLoading(`p${c.id}`);
    await new Promise((r) => setTimeout(r, 500)); // POST ToggleSale
    updateCar(c.id, { saleEnabled: !c.saleEnabled });
    setLoading(null);
    toast.success(c.saleEnabled ? "Đã tắt giá khuyến mãi" : "Đã bật giá khuyến mãi", { description: DEMO_NOTE });
  };

  const sel = (v: string, set: (s: string) => void, ph: string, opts: [string, string][]) => (
    <Select value={v} onValueChange={(x) => { set(x); setPage(1); }}>
      <SelectTrigger className="w-full bg-card sm:w-40"><SelectValue placeholder={ph} /></SelectTrigger>
      <SelectContent>
        <SelectItem value={ALL}>{ph}</SelectItem>
        {opts.map(([k, l]) => <SelectItem key={k} value={k}>{l}</SelectItem>)}
      </SelectContent>
    </Select>
  );

  return (
    <TooltipProvider>
      <PageHeader title="Quản lý xe" description={`${cars.length} xe thuộc đội xe của bạn`} actions={<Button asChild><Link to="/CarOwner/Cars/Create"><Plus /> Thêm xe mới</Link></Button>} />

      <div className="card-surface mb-5 flex flex-col gap-3 p-4 lg:flex-row lg:flex-wrap lg:items-center">
        <div className="relative flex-1 lg:min-w-64">
          <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input value={q} onChange={(e) => { setQ(e.target.value); setPage(1); }} placeholder="Tìm tên xe hoặc biển số…" className="bg-card pl-9" />
        </div>
        {sel(brand, setBrand, "Tất cả hãng", uniq("brand").map((b) => [b, b]))}
        {sel(segment, setSegment, "Mọi phân khúc", uniq("segment").map((b) => [b, b]))}
        {sel(status, setStatus, "Mọi trạng thái", Object.entries(CAR_STATUS_LABEL))}
        {sel(area, setArea, "Mọi khu vực", [...new Set(cars.map(areaOf))].map((a) => [a, a]))}
        <Select value={sort} onValueChange={setSort}>
          <SelectTrigger className="w-full bg-card sm:w-44"><SelectValue /></SelectTrigger>
          <SelectContent>
            <SelectItem value="newest">Mới đăng nhất</SelectItem>
            <SelectItem value="priceAsc">Giá thấp → cao</SelectItem>
            <SelectItem value="priceDesc">Giá cao → thấp</SelectItem>
            <SelectItem value="trips">Nhiều chuyến nhất</SelectItem>
          </SelectContent>
        </Select>
      </div>

      <div className="card-surface overflow-hidden">
        {shown.length === 0 ? (
          <EmptyState icon={<CarFront />} title="Không tìm thấy xe phù hợp" body="Thử đổi từ khóa hoặc bộ lọc." />
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full min-w-[1080px] text-sm">
              <thead className="bg-muted text-left text-xs uppercase tracking-wide text-muted-foreground">
                <tr>{["Xe", "Giá/ngày", "Nhận xe", "Chuyến", "Đánh giá", "Trạng thái", "Hoạt động", "Khuyến mãi", ""].map((h) => <th key={h} className="px-4 py-3 font-semibold">{h}</th>)}</tr>
              </thead>
              <tbody className="divide-y">
                {shown.map((c) => {
                  const chk = canToggleActive(c, orders);
                  const r = rating(c.id);
                  return (
                    <tr key={c.id} className="hover:bg-muted/40">
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-3">
                          <img src={c.image} alt={c.name} className="h-14 w-22 rounded-lg object-cover" loading="lazy" />
                          <div>
                            <Link to="/CarOwner/Cars/Details/$id" params={{ id: String(c.id) }} className="font-semibold hover:text-primary">{c.name}</Link>
                            <p className="text-xs text-muted-foreground">{c.brand} · {c.year} · {c.segment}</p>
                            <span className="plate mt-1 inline-block">{c.plate}</span>
                          </div>
                        </div>
                      </td>
                      <td className="px-4 py-3">
                        {c.saleEnabled && c.salePrice ? (
                          <><p className="font-bold text-primary">{formatVND(c.salePrice)}</p><p className="text-xs text-muted-foreground line-through">{formatVND(c.pricePerDay)}</p></>
                        ) : <p className="font-bold">{formatVND(c.pricePerDay)}</p>}
                      </td>
                      <td className="px-4 py-3 text-xs"><span className="flex items-center gap-1"><MapPin className="size-3.5 text-muted-foreground" />{c.location}</span></td>
                      <td className="px-4 py-3 font-semibold">{c.completedTrips}</td>
                      <td className="px-4 py-3">{r ? <span className="flex items-center gap-1 font-semibold"><Star className="size-3.5 fill-warning text-warning" />{r}</span> : <span className="text-xs text-muted-foreground">Chưa có</span>}</td>
                      <td className="px-4 py-3"><CarStatusBadge status={c.status} /></td>
                      <td className="px-4 py-3">
                        <Tooltip>
                          <TooltipTrigger asChild>
                            <span><Switch checked={c.status === "available"} disabled={!chk.ok || loading === `s${c.id}`} onCheckedChange={() => toggleActive(c)} aria-label="Bật/tắt hoạt động" /></span>
                          </TooltipTrigger>
                          <TooltipContent>{chk.ok ? (c.status === "available" ? "Tạm ngừng cho thuê" : "Mở lại cho thuê") : chk.reason}</TooltipContent>
                        </Tooltip>
                      </td>
                      <td className="px-4 py-3"><Switch checked={c.saleEnabled} disabled={loading === `p${c.id}`} onCheckedChange={() => toggleSale(c)} aria-label="Bật/tắt khuyến mãi" /></td>
                      <td className="px-4 py-3 text-right">
                        <DropdownMenu>
                          <DropdownMenuTrigger asChild><Button variant="ghost" size="icon" aria-label="Thao tác"><MoreHorizontal /></Button></DropdownMenuTrigger>
                          <DropdownMenuContent align="end">
                            <DropdownMenuItem asChild><Link to="/CarOwner/Cars/Details/$id" params={{ id: String(c.id) }}><Eye /> Xem chi tiết</Link></DropdownMenuItem>
                            <DropdownMenuItem asChild><Link to="/CarOwner/Cars/Edit/$id" params={{ id: String(c.id) }}><Pencil /> Chỉnh sửa</Link></DropdownMenuItem>
                            <DropdownMenuItem className="text-destructive" onClick={() => setRemoving(c)}><Trash2 /> Gỡ xe</DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
        <Pager page={page} pages={pages} onChange={setPage} />
      </div>
      <RemoveCarDialog car={removing} open={!!removing} onOpenChange={(v) => !v && setRemoving(null)} />
    </TooltipProvider>
  );
}
