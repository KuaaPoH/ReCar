import { createFileRoute } from "@tanstack/react-router";
import { useMemo, useState } from "react";
import { EyeOff, ExternalLink, Eye, MoreHorizontal, PauseCircle, PlayCircle, Search } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { ConfirmDialog, EmptyState, ErrorState, Field, PageHeader, Pager, Panel, StatusBadge, TableSkeleton, useDebounced, useDemoLoad, usePaged } from "@/components/admin/shared";
import { update, useStore } from "@/data/store";
import { ownerById, type Car } from "@/data/demo";
import { vnd } from "@/lib/format";
import { seo } from "@/lib/seo";

export const Route = createFileRoute("/admin/cars/")({
  validateSearch: (s: Record<string, unknown>) => ({ q: typeof s.q === "string" ? s.q : undefined }),
  head: () => seo("Quản lý đội xe", "Danh sách toàn bộ xe trên sàn ReCar."),
  component: Fleet,
});

const ALL = "__all";
type Action = { car: Car; kind: "suspend" | "restore" | "hide" };

function Fleet() {
  const search = Route.useSearch();
  const { state, retry } = useDemoLoad();
  const cars = useStore((s) => s.cars);
  const owners = useStore((s) => s.owners);
  const [q, setQ] = useState(search.q ?? "");
  const [f, setF] = useState({ brand: ALL, segment: ALL, status: ALL, city: ALL, owner: ALL });
  const [act, setAct] = useState<Action | null>(null);
  const [detail, setDetail] = useState<Car | null>(null);
  const dq = useDebounced(q);
  const set = (k: keyof typeof f) => (v: string) => setF((p) => ({ ...p, [k]: v }));

  const list = useMemo(() => cars.filter((c) => {
    const k = dq.toLowerCase();
    return c.status !== "hidden" as never
      && (!k || c.name.toLowerCase().includes(k) || c.plate.toLowerCase().includes(k))
      && (f.brand === ALL || c.brand === f.brand) && (f.segment === ALL || c.segment === f.segment)
      && (f.status === ALL || c.status === f.status) && (f.city === ALL || c.city === f.city) && (f.owner === ALL || c.ownerId === f.owner);
  }), [cars, dq, f]);
  const paged = usePaged(list, 8);
  const uniq = (k: keyof Car) => [...new Set(cars.map((c) => String(c[k])))];

  const run = (a: Action, reason: string) => {
    const status = a.kind === "restore" ? "ready" : a.kind === "suspend" ? "suspended" : ("hidden" as never);
    update((s) => ({ cars: s.cars.map((x) => x.id === a.car.id ? { ...x, status } : x) }), { action: { suspend: "Tạm ngừng xe", restore: "Khôi phục xe", hide: "Ẩn bài đăng xe" }[a.kind], target: a.car.plate, reason });
    toast.success({ suspend: "Đã tạm ngừng hoạt động xe.", restore: "Đã khôi phục hoạt động xe.", hide: "Đã ẩn bài đăng (xóa mềm, dữ liệu vẫn được lưu)." }[a.kind]);
  };
  const tryAct = (car: Car, kind: Action["kind"]) => {
    if (kind !== "restore" && car.hasActiveOrder) { toast.error(`Không thể thực hiện: ${car.plate} đang gắn với đơn thuê chưa hoàn tất.`); return; }
    if (kind === "restore" && !car.docsVerified) { toast.error("Xe chưa đủ điều kiện khôi phục: giấy tờ chưa xác minh."); return; }
    setAct({ car, kind });
  };

  const sel = (k: keyof typeof f, all: string, opts: [string, string][]) => (
    <Select value={f[k]} onValueChange={set(k)}><SelectTrigger><SelectValue /></SelectTrigger><SelectContent><SelectItem value={ALL}>{all}</SelectItem>{opts.map(([v, l]) => <SelectItem key={v} value={v}>{l}</SelectItem>)}</SelectContent></Select>
  );

  return (
    <>
      <PageHeader title="Quản lý đội xe" desc={`${list.length} xe phù hợp bộ lọc`} />
      <Panel className="mb-4">
        <div className="grid gap-3 p-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6">
          <div className="relative"><Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" /><Input className="pl-9" placeholder="Tên xe, biển số" value={q} onChange={(e) => setQ(e.target.value)} /></div>
          {sel("brand", "Tất cả hãng", uniq("brand").map((b) => [b, b]))}
          {sel("segment", "Tất cả phân khúc", ["Sedan", "SUV", "MPV", "Bán tải", "Hatchback"].map((b) => [b, b]))}
          {sel("status", "Tất cả trạng thái", [["ready", "Sẵn sàng"], ["rented", "Đang có khách"], ["maintenance", "Bảo trì"], ["pending", "Chờ duyệt"], ["suspended", "Tạm ngừng"]])}
          {sel("city", "Tất cả thành phố", uniq("city").map((b) => [b, b]))}
          {sel("owner", "Tất cả chủ xe", owners.map((o) => [o.id, o.name]))}
        </div>
      </Panel>
      <Panel>
        {state === "loading" ? <TableSkeleton /> : state === "error" ? <ErrorState onRetry={retry} /> : list.length === 0 ? <EmptyState /> : (
          <>
            <Table>
              <TableHeader><TableRow><TableHead className="pl-5">Xe</TableHead><TableHead>Biển số</TableHead><TableHead>Phân khúc</TableHead><TableHead>Chủ xe</TableHead><TableHead className="text-right">Giá/ngày</TableHead><TableHead className="text-center">Chuyến</TableHead><TableHead>Trạng thái</TableHead><TableHead className="pr-5" /></TableRow></TableHeader>
              <TableBody>
                {paged.slice.map((c) => (
                  <TableRow key={c.id}>
                    <TableCell className="pl-5"><div className="flex items-center gap-3"><img src={c.image} alt="" className="h-10 w-14 rounded-md object-cover" /><div><p className="whitespace-nowrap font-medium">{c.name}</p><p className="text-xs text-muted-foreground">{c.year}</p></div></div></TableCell>
                    <TableCell><span className="plate">{c.plate}</span></TableCell>
                    <TableCell>{c.segment}</TableCell>
                    <TableCell className="whitespace-nowrap">{ownerById(c.ownerId).name}</TableCell>
                    <TableCell className="whitespace-nowrap text-right">{vnd(c.pricePerDay)}</TableCell>
                    <TableCell className="text-center">{c.trips}</TableCell>
                    <TableCell><StatusBadge status={c.status} /></TableCell>
                    <TableCell className="pr-5 text-right">
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild><Button variant="ghost" size="icon" className="size-8" aria-label="Thao tác"><MoreHorizontal /></Button></DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          <DropdownMenuItem onClick={() => setDetail(c)}><Eye /> Xem chi tiết</DropdownMenuItem>
                          <DropdownMenuItem onClick={() => toast.info(c.status === "ready" || c.status === "rented" ? `Mở trang công khai /Cars/${c.id} (bản tích hợp).` : "Xe chưa hiển thị công khai do chưa đủ điều kiện.")}><ExternalLink /> Xem trang công khai</DropdownMenuItem>
                          <DropdownMenuSeparator />
                          {c.status === "suspended"
                            ? <DropdownMenuItem onClick={() => tryAct(c, "restore")}><PlayCircle /> Khôi phục hoạt động</DropdownMenuItem>
                            : <DropdownMenuItem disabled={c.status === "pending"} onClick={() => tryAct(c, "suspend")}><PauseCircle /> Tạm ngừng hoạt động</DropdownMenuItem>}
                          <DropdownMenuItem className="text-destructive" onClick={() => tryAct(c, "hide")}><EyeOff /> Ẩn bài đăng</DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
            <Pager {...paged} />
          </>
        )}
      </Panel>

      <Sheet open={!!detail} onOpenChange={(o) => !o && setDetail(null)}>
        <SheetContent className="w-full overflow-y-auto sm:max-w-lg">
          {detail && (
            <>
              <SheetHeader><SheetTitle>{detail.name}</SheetTitle><SheetDescription><span className="plate">{detail.plate}</span></SheetDescription></SheetHeader>
              <div className="space-y-4 px-4 pb-6">
                <img src={detail.image} alt={detail.name} className="w-full rounded-lg" />
                <dl className="grid grid-cols-2 gap-4 rounded-lg border p-4">
                  <Field label="Trạng thái"><StatusBadge status={detail.status} /></Field>
                  <Field label="Chủ xe">{ownerById(detail.ownerId).name}</Field>
                  <Field label="Phân khúc">{detail.segment} · {detail.seats} chỗ</Field>
                  <Field label="Hộp số / Nhiên liệu">{detail.gearbox} · {detail.fuel}</Field>
                  <Field label="Giá/ngày">{vnd(detail.pricePerDay)}</Field>
                  <Field label="Chuyến hoàn thành">{detail.trips}</Field>
                  <Field label="Bàn giao">{detail.handover}</Field>
                  <Field label="Đơn đang mở">{detail.hasActiveOrder ? "Có — khóa thao tác xóa" : "Không"}</Field>
                </dl>
              </div>
            </>
          )}
        </SheetContent>
      </Sheet>

      <ConfirmDialog open={!!act} onOpenChange={(o) => !o && setAct(null)} destructive={act?.kind !== "restore"} requireReason={act?.kind !== "restore"}
        title={act ? { suspend: "Tạm ngừng hoạt động xe?", restore: "Khôi phục hoạt động xe?", hide: "Ẩn bài đăng xe?" }[act.kind] : ""}
        desc={act && <>Xe <span className="font-mono">{act.car.plate}</span>. {act.kind === "hide" ? "Bài đăng sẽ bị ẩn khỏi sàn; dữ liệu và lịch sử đơn vẫn được giữ (không xóa vật lý)." : "Thao tác được ghi vào nhật ký kiểm toán."}</>}
        onConfirm={(r) => act && run(act, r)} />
    </>
  );
}
