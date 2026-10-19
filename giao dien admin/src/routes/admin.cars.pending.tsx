import { createFileRoute } from "@tanstack/react-router";
import { useMemo, useState } from "react";
import { CheckCircle2, Clock, FileCheck2, FileWarning, LayoutGrid, List, MapPin, Phone, Search, XCircle, ZoomIn } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetDescription } from "@/components/ui/sheet";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group";
import { ConfirmDialog, EmptyState, ErrorState, Field, PageHeader, Pager, Panel, StatusBadge, TableSkeleton, useDebounced, useDemoLoad, usePaged } from "@/components/admin/shared";
import { update, useStore } from "@/data/store";
import { ownerById, type Car } from "@/data/demo";
import { dateTimeVN, vnd } from "@/lib/format";
import { seo } from "@/lib/seo";

export const Route = createFileRoute("/admin/cars/pending")({
  validateSearch: (s: Record<string, unknown>) => ({ q: typeof s.q === "string" ? s.q : undefined }),
  head: () => seo("Phê duyệt xe", "Kiểm duyệt hồ sơ xe do chủ xe gửi lên sàn ReCar."),
  component: PendingCars,
});

const ALL = "__all";

function PendingCars() {
  const search = Route.useSearch();
  const { state, retry } = useDemoLoad();
  const cars = useStore((s) => s.cars);
  const [q, setQ] = useState(search.q ?? "");
  const [owner, setOwner] = useState("");
  const [brand, setBrand] = useState(ALL);
  const [city, setCity] = useState(ALL);
  const [date, setDate] = useState("");
  const [view, setView] = useState<"grid" | "table">("grid");
  const [selected, setSelected] = useState<Car | null>(null);
  const [approve, setApprove] = useState<Car | null>(null);
  const [reject, setReject] = useState<Car | null>(null);
  const [notify, setNotify] = useState(true);
  const [zoom, setZoom] = useState<string | null>(null);
  const dq = useDebounced(q), dOwner = useDebounced(owner);

  const pending = cars.filter((c) => c.status === "pending");
  const brands = [...new Set(pending.map((c) => c.brand))];
  const cities = [...new Set(pending.map((c) => c.city))];
  const list = useMemo(() => pending.filter((c) => {
    const o = ownerById(c.ownerId);
    const k = dq.toLowerCase();
    return (!k || c.name.toLowerCase().includes(k) || c.plate.toLowerCase().includes(k))
      && (!dOwner || o.name.toLowerCase().includes(dOwner.toLowerCase()) || o.phone.replace(/\s/g, "").includes(dOwner.replace(/\s/g, "")))
      && (brand === ALL || c.brand === brand) && (city === ALL || c.city === city)
      && (!date || c.submittedAt.slice(0, 10) === date);
  }), [pending, dq, dOwner, brand, city, date]);
  const paged = usePaged(list, 6);

  const doApprove = (c: Car) => {
    update((s) => ({ cars: s.cars.map((x) => x.id === c.id ? { ...x, status: "ready" } : x) }), { action: "Phê duyệt xe", target: c.plate });
    toast.success(`Đã phê duyệt ${c.name} (${c.plate}). Xe sẽ hiển thị công khai khi đủ điều kiện hoạt động.`);
    setSelected(null);
  };
  const doReject = (c: Car, reason: string) => {
    update((s) => ({ cars: s.cars.map((x) => x.id === c.id ? { ...x, status: "rejected", rejectReason: reason } : x) }), { action: "Từ chối duyệt xe", target: c.plate, reason });
    toast.success(`Đã từ chối hồ sơ ${c.plate}${notify ? " và gửi thông báo yêu cầu chỉnh sửa tới chủ xe" : ""}.`);
    setSelected(null);
  };
  const recentRejected = cars.filter((c) => c.status === "rejected");

  return (
    <>
      <PageHeader title="Phê duyệt xe" desc={`${pending.length} hồ sơ đang chờ kiểm tra. Xe chưa duyệt không xuất hiện công khai.`}
        actions={
          <ToggleGroup type="single" variant="outline" value={view} onValueChange={(v) => v && setView(v as never)}>
            <ToggleGroupItem value="grid" aria-label="Dạng thẻ"><LayoutGrid /></ToggleGroupItem>
            <ToggleGroupItem value="table" aria-label="Dạng bảng"><List /></ToggleGroupItem>
          </ToggleGroup>
        } />

      <Panel className="mb-4">
        <div className="grid gap-3 p-4 md:grid-cols-2 xl:grid-cols-5">
          <div className="relative"><Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" /><Input className="pl-9" placeholder="Tên xe hoặc biển số" value={q} onChange={(e) => setQ(e.target.value)} /></div>
          <Select value={brand} onValueChange={setBrand}><SelectTrigger><SelectValue /></SelectTrigger><SelectContent><SelectItem value={ALL}>Tất cả hãng</SelectItem>{brands.map((b) => <SelectItem key={b} value={b}>{b}</SelectItem>)}</SelectContent></Select>
          <Select value={city} onValueChange={setCity}><SelectTrigger><SelectValue /></SelectTrigger><SelectContent><SelectItem value={ALL}>Tất cả thành phố</SelectItem>{cities.map((b) => <SelectItem key={b} value={b}>{b}</SelectItem>)}</SelectContent></Select>
          <Input type="date" value={date} onChange={(e) => setDate(e.target.value)} aria-label="Ngày gửi duyệt" />
          <Input placeholder="Tên / SĐT chủ xe" value={owner} onChange={(e) => setOwner(e.target.value)} />
        </div>
      </Panel>

      {state === "loading" ? <Panel><TableSkeleton rows={4} /></Panel> : state === "error" ? <Panel><ErrorState onRetry={retry} /></Panel> : list.length === 0 ? (
        <Panel><EmptyState title={pending.length ? "Không có hồ sơ phù hợp" : "Đã xử lý hết hồ sơ"} desc={pending.length ? undefined : "Không còn xe nào chờ phê duyệt."} /></Panel>
      ) : view === "grid" ? (
        <>
          <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
            {paged.slice.map((c) => {
              const o = ownerById(c.ownerId);
              return (
                <article key={c.id} className="overflow-hidden rounded-xl border bg-card shadow-card">
                  <button className="block w-full" onClick={() => setSelected(c)}>
                    <img src={c.image} alt={c.name} loading="lazy" width={992} height={672} className="aspect-[16/10] w-full object-cover" />
                  </button>
                  <div className="space-y-3 p-4">
                    <div className="flex items-start justify-between gap-2">
                      <div><h3 className="font-semibold">{c.name}</h3><p className="text-xs text-muted-foreground">{c.brand} · {c.year} · {c.segment}</p></div>
                      <span className="plate">{c.plate}</span>
                    </div>
                    <p className="text-xs text-muted-foreground">{c.seats} chỗ · {c.gearbox} · {c.fuel}</p>
                    <div className="flex items-baseline justify-between"><span className="text-lg font-bold text-primary">{vnd(c.pricePerDay)}<span className="text-xs font-normal text-muted-foreground">/ngày</span></span>{c.driverFee && <span className="text-xs text-muted-foreground">Tài xế +{vnd(c.driverFee)}</span>}</div>
                    <div className="space-y-1 border-t pt-3 text-xs text-muted-foreground">
                      <p className="font-medium text-foreground">{o.name}</p>
                      <p className="flex items-center gap-1.5"><Phone className="size-3" />{o.phone}</p>
                      <p className="flex items-center gap-1.5"><MapPin className="size-3" />{c.handover}</p>
                      <p className="flex items-center gap-1.5"><Clock className="size-3" />Gửi lúc {dateTimeVN(c.submittedAt)}</p>
                    </div>
                    <DocsBadge ok={c.docsVerified} />
                    <div className="grid grid-cols-3 gap-2 pt-1">
                      <Button variant="outline" size="sm" onClick={() => setSelected(c)}>Chi tiết</Button>
                      <Button variant="outline" size="sm" className="text-destructive" onClick={() => setReject(c)}>Từ chối</Button>
                      <Button size="sm" onClick={() => setApprove(c)}>Duyệt</Button>
                    </div>
                  </div>
                </article>
              );
            })}
          </div>
          <div className="mt-4 rounded-xl border bg-card"><Pager {...paged} /></div>
        </>
      ) : (
        <Panel>
          <Table>
            <TableHeader><TableRow><TableHead className="pl-5">Xe</TableHead><TableHead>Biển số</TableHead><TableHead>Thông số</TableHead><TableHead>Giá/ngày</TableHead><TableHead>Chủ xe</TableHead><TableHead>Gửi lúc</TableHead><TableHead>Giấy tờ</TableHead><TableHead className="pr-5 text-right">Thao tác</TableHead></TableRow></TableHeader>
            <TableBody>
              {paged.slice.map((c) => {
                const o = ownerById(c.ownerId);
                return (
                  <TableRow key={c.id}>
                    <TableCell className="pl-5"><div className="flex items-center gap-3"><img src={c.image} alt="" className="h-10 w-14 rounded-md object-cover" /><div><p className="font-medium">{c.name}</p><p className="text-xs text-muted-foreground">{c.brand} · {c.year}</p></div></div></TableCell>
                    <TableCell><span className="plate">{c.plate}</span></TableCell>
                    <TableCell className="text-xs text-muted-foreground">{c.segment} · {c.seats} chỗ · {c.gearbox}</TableCell>
                    <TableCell className="whitespace-nowrap">{vnd(c.pricePerDay)}</TableCell>
                    <TableCell><p className="whitespace-nowrap">{o.name}</p><p className="text-xs text-muted-foreground">{o.phone}</p></TableCell>
                    <TableCell className="whitespace-nowrap text-xs">{dateTimeVN(c.submittedAt)}</TableCell>
                    <TableCell><DocsBadge ok={c.docsVerified} /></TableCell>
                    <TableCell className="pr-5 text-right"><div className="flex justify-end gap-1.5"><Button variant="ghost" size="sm" onClick={() => setSelected(c)}>Chi tiết</Button><Button variant="outline" size="sm" className="text-destructive" onClick={() => setReject(c)}>Từ chối</Button><Button size="sm" onClick={() => setApprove(c)}>Duyệt</Button></div></TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
          <Pager {...paged} />
        </Panel>
      )}

      {recentRejected.length > 0 && (
        <Panel className="mt-6" title="Hồ sơ đã từ chối / yêu cầu chỉnh sửa">
          <ul className="divide-y">
            {recentRejected.map((c) => (
              <li key={c.id} className="flex flex-wrap items-center justify-between gap-2 px-5 py-3 text-sm">
                <span className="flex items-center gap-2"><span className="plate">{c.plate}</span>{c.name}</span>
                <span className="text-xs text-muted-foreground">Lý do: {c.rejectReason}</span>
                <StatusBadge status="rejected" label="Yêu cầu chỉnh sửa" />
              </li>
            ))}
          </ul>
        </Panel>
      )}

      <Sheet open={!!selected} onOpenChange={(o) => !o && setSelected(null)}>
        <SheetContent className="w-full overflow-y-auto sm:max-w-xl">
          {selected && (() => {
            const o = ownerById(selected.ownerId);
            return (
              <>
                <SheetHeader><SheetTitle>{selected.name}</SheetTitle><SheetDescription>Hồ sơ gửi lúc {dateTimeVN(selected.submittedAt)}</SheetDescription></SheetHeader>
                <div className="space-y-5 px-4 pb-6">
                  <div className="grid grid-cols-2 gap-2">
                    {selected.gallery.map((img, i) => (
                      <button key={i} onClick={() => setZoom(img)} className="group relative overflow-hidden rounded-lg">
                        <img src={img} alt={`Ảnh ${i + 1}`} className="aspect-[4/3] w-full object-cover transition group-hover:scale-105" />
                        <span className="absolute bottom-1.5 left-1.5 rounded bg-card/90 px-1.5 py-0.5 text-[10px] font-medium">{["Ngoại thất trước", "Nội thất", "Góc bên", "Góc sau"][i]}</span>
                        <ZoomIn className="absolute right-2 top-2 size-4 text-card opacity-0 group-hover:opacity-100" />
                      </button>
                    ))}
                  </div>
                  <dl className="grid grid-cols-2 gap-4 rounded-lg border p-4">
                    <Field label="Hãng / Đời">{selected.brand} · {selected.year}</Field>
                    <Field label="Biển số"><span className="plate">{selected.plate}</span></Field>
                    <Field label="Phân khúc">{selected.segment}</Field>
                    <Field label="Số chỗ">{selected.seats}</Field>
                    <Field label="Hộp số">{selected.gearbox}</Field>
                    <Field label="Nhiên liệu">{selected.fuel}</Field>
                    <Field label="Giá thuê/ngày">{vnd(selected.pricePerDay)}</Field>
                    <Field label="Phụ phí tài xế">{selected.driverFee ? vnd(selected.driverFee) : "Không"}</Field>
                    <Field label="Chủ xe">{o.name}</Field>
                    <Field label="Điện thoại">{o.phone}</Field>
                    <Field label="Khu vực bàn giao">{selected.handover}</Field>
                    <Field label="Thành phố">{selected.city}</Field>
                  </dl>
                  <div>
                    <p className="mb-2 text-sm font-semibold">Giấy tờ xác minh</p>
                    <div className="space-y-2">
                      {["Giấy đăng ký xe (cà vẹt)", "Giấy chứng nhận đăng kiểm", "Bảo hiểm TNDS bắt buộc"].map((d, i) => {
                        const ok = selected.docsVerified || i === 0;
                        return (
                          <button key={d} onClick={() => setZoom(selected.gallery[0])} className="flex w-full items-center justify-between rounded-lg border px-3 py-2.5 text-sm hover:bg-muted">
                            <span className="flex items-center gap-2">{ok ? <FileCheck2 className="size-4 text-success" /> : <FileWarning className="size-4 text-warning" />}{d}</span>
                            <span className="text-xs text-muted-foreground">{ok ? "Đã tải lên · Xem" : "Thiếu / mờ"}</span>
                          </button>
                        );
                      })}
                    </div>
                  </div>
                  <div className="flex gap-2">
                    <Button variant="outline" className="flex-1 text-destructive" onClick={() => setReject(selected)}><XCircle /> Từ chối</Button>
                    <Button className="flex-1" onClick={() => setApprove(selected)}><CheckCircle2 /> Phê duyệt</Button>
                  </div>
                </div>
              </>
            );
          })()}
        </SheetContent>
      </Sheet>

      <Dialog open={!!zoom} onOpenChange={(o) => !o && setZoom(null)}>
        <DialogContent className="max-w-4xl p-2"><DialogTitle className="sr-only">Phóng to ảnh</DialogTitle>{zoom && <img src={zoom} alt="Phóng to" className="w-full rounded-lg" />}</DialogContent>
      </Dialog>

      <ConfirmDialog open={!!approve} onOpenChange={(o) => !o && setApprove(null)} title="Phê duyệt hồ sơ xe?"
        desc={approve && <>Xe <b>{approve.name}</b> (<span className="font-mono">{approve.plate}</span>) sẽ chuyển sang trạng thái <b>Sẵn sàng</b> và được hiển thị công khai.{!approve.docsVerified && <p className="mt-2 text-warning">Lưu ý: giấy tờ xác minh chưa đầy đủ.</p>}</>}
        confirmLabel="Phê duyệt" onConfirm={() => approve && doApprove(approve)} />
      <ConfirmDialog open={!!reject} onOpenChange={(o) => !o && setReject(null)} title="Từ chối duyệt xe" destructive requireReason reasonLabel="Lý do từ chối / nội dung cần chỉnh sửa"
        desc={reject && <>Hồ sơ <span className="font-mono">{reject.plate}</span> sẽ chuyển sang trạng thái <b>Yêu cầu chỉnh sửa</b>.</>}
        confirmLabel="Từ chối hồ sơ" onConfirm={(r) => reject && doReject(reject, r)}>
        <div className="flex items-center gap-2"><Checkbox id="notify" checked={notify} onCheckedChange={(v) => setNotify(!!v)} /><Label htmlFor="notify" className="text-sm font-normal">Gửi thông báo cho chủ xe</Label></div>
      </ConfirmDialog>
    </>
  );
}

function DocsBadge({ ok }: { ok: boolean }) {
  return ok ? <StatusBadge status="verified" label="Giấy tờ đầy đủ" /> : <StatusBadge status="pending" label="Giấy tờ cần bổ sung" />;
}
