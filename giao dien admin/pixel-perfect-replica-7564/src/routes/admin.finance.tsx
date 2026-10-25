import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { Banknote, CheckCircle2, Clock, HandCoins, Landmark, Lock, Percent, Wallet, Eye } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { ConfirmDialog, DemoNotice, ErrorState, Field, PageHeader, Pager, Panel, StatusBadge, TableSkeleton, useDemoLoad, usePaged } from "@/components/admin/shared";
import { update, useStore } from "@/data/store";
import { CURRENT_ADMIN, ownerById, type Settlement } from "@/data/demo";
import { dateTimeVN, maskAccount, payout, splitDeposit, vnd, vndShort } from "@/lib/format";
import { seo } from "@/lib/seo";

export const Route = createFileRoute("/admin/finance")({
  head: () => seo("Tài chính & đối soát", "Đối soát và quyết toán doanh thu cho chủ xe ReCar."),
  component: Finance,
});

function Finance() {
  const { state, retry } = useDemoLoad();
  const settlements = useStore((s) => s.settlements);
  const orders = useStore((s) => s.orders);
  const { commissionPct, depositPct } = useStore((s) => s.settings);
  const [detail, setDetail] = useState<Settlement | null>(null);
  const [confirm, setConfirm] = useState<Settlement | null>(null);
  const [ref, setRef] = useState("");
  const [refErr, setRefErr] = useState("");
  const [reveal, setReveal] = useState(false);
  const paged = usePaged(settlements, 6);
  const canSeeBank = CURRENT_ADMIN.role === "Super Admin" || CURRENT_ADMIN.role === "Kế toán";

  const depositsHeld = orders.filter((o) => ["deposited", "awaiting_handover", "renting", "awaiting_completion"].includes(o.status)).reduce((a, o) => a + splitDeposit(o.total, depositPct).deposit, 0);
  const completedGmv = orders.filter((o) => o.status === "completed").reduce((a, o) => a + o.total, 0);
  const sum = (f: (s: Settlement) => boolean) => settlements.filter(f).reduce((a, s) => a + payout(s.gross, commissionPct, s.adjustment).net, 0);
  const refunds = orders.filter((o) => o.payment === "refunded" || o.payment === "refunding").reduce((a, o) => a + splitDeposit(o.total, depositPct).deposit, 0);

  const cards = [
    { icon: Wallet, l: "Tiền cọc đang giữ", v: depositsHeld, s: "Đơn đã cọc, chưa hoàn tất" },
    { icon: Banknote, l: "GMV đơn hoàn thành", v: completedGmv, s: "Doanh thu gộp" },
    { icon: Percent, l: "Hoa hồng sàn", v: settlements.reduce((a, s) => a + payout(s.gross, commissionPct).fee, 0), s: `${commissionPct}% trên giá trị thuê` },
    { icon: HandCoins, l: "Phải trả chủ xe", v: sum((s) => s.status !== "settled"), s: "Chưa quyết toán" },
    { icon: CheckCircle2, l: "Đã quyết toán", v: sum((s) => s.status === "settled"), s: "Đã chuyển khoản" },
    { icon: Clock, l: "Chờ đối soát", v: sum((s) => s.status === "pending" || s.status === "discrepancy"), s: `Hoàn khách: ${vndShort(refunds)}` },
  ];

  const doConfirm = () => {
    if (!/^[A-Z0-9-]{6,}$/i.test(ref.trim())) { setRefErr("Mã giao dịch tham chiếu không hợp lệ (tối thiểu 6 ký tự, chữ/số/-)."); return false; }
    return true;
  };

  return (
    <>
      <PageHeader title="Tài chính & đối soát" desc="Phân biệt rõ doanh thu gộp, tiền cọc đang giữ, hoa hồng và tiền phải trả chủ xe." />
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-6">
        {cards.map((c) => (
          <div key={c.l} className="rounded-xl border bg-card p-4 shadow-card">
            <div className="flex items-center gap-2 text-xs text-muted-foreground"><c.icon className="size-4 text-primary" />{c.l}</div>
            <p className="mt-2 text-xl font-bold">{vndShort(c.v)}</p>
            <p className="mt-1 text-xs text-muted-foreground">{c.s}</p>
          </div>
        ))}
      </div>
      <Panel className="mt-6" title="Bảng đối soát chủ xe">
        {state === "loading" ? <TableSkeleton /> : state === "error" ? <ErrorState onRetry={retry} /> : (
          <>
            <Table>
              <TableHeader><TableRow><TableHead className="pl-5">Mã</TableHead><TableHead>Chủ xe</TableHead><TableHead>Kỳ</TableHead><TableHead className="text-center">Chuyến</TableHead><TableHead className="text-right">Giá trị đủ ĐK</TableHead><TableHead className="text-right">Phí sàn</TableHead><TableHead className="text-right">Điều chỉnh</TableHead><TableHead className="text-right">Thực nhận</TableHead><TableHead>Trạng thái</TableHead><TableHead>Ngày QT</TableHead><TableHead className="pr-5" /></TableRow></TableHeader>
              <TableBody>
                {paged.slice.map((s) => {
                  const { fee, net } = payout(s.gross, commissionPct, s.adjustment);
                  return (
                    <TableRow key={s.id}>
                      <TableCell className="pl-5 font-mono text-xs font-semibold">{s.id}</TableCell>
                      <TableCell className="whitespace-nowrap">{ownerById(s.ownerId).name}</TableCell>
                      <TableCell className="whitespace-nowrap text-xs">{s.period}</TableCell>
                      <TableCell className="text-center">{s.trips}</TableCell>
                      <TableCell className="whitespace-nowrap text-right">{vnd(s.gross)}</TableCell>
                      <TableCell className="whitespace-nowrap text-right text-muted-foreground">−{vnd(fee)}</TableCell>
                      <TableCell className="whitespace-nowrap text-right">{s.adjustment ? (s.adjustment > 0 ? "+" : "") + vnd(s.adjustment) : "—"}</TableCell>
                      <TableCell className="whitespace-nowrap text-right font-semibold">{vnd(net)}</TableCell>
                      <TableCell><StatusBadge status={s.status} /></TableCell>
                      <TableCell className="whitespace-nowrap text-xs">{s.settledAt ? dateTimeVN(s.settledAt) : "—"}</TableCell>
                      <TableCell className="pr-5"><Button variant="ghost" size="icon" className="size-8" aria-label="Chi tiết" onClick={() => { setDetail(s); setReveal(false); }}><Eye /></Button></TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
            <Pager {...paged} />
          </>
        )}
      </Panel>

      <Sheet open={!!detail} onOpenChange={(o) => !o && setDetail(null)}>
        <SheetContent className="w-full overflow-y-auto sm:max-w-lg">
          {detail && (() => {
            const o = ownerById(detail.ownerId);
            const { fee, net } = payout(detail.gross, commissionPct, detail.adjustment);
            return (
              <>
                <SheetHeader><SheetTitle>Đối soát {detail.id}</SheetTitle><SheetDescription>{o.name} · Kỳ {detail.period}</SheetDescription></SheetHeader>
                <div className="space-y-5 px-4 pb-6">
                  <div className="rounded-lg border p-4">
                    <p className="mb-3 flex items-center gap-2 text-sm font-semibold"><Landmark className="size-4" /> Tài khoản nhận tiền</p>
                    <dl className="grid grid-cols-2 gap-3">
                      <Field label="Ngân hàng">{o.bank.name}</Field>
                      <Field label="Chủ tài khoản">{o.bank.holder}</Field>
                      <Field label="Số tài khoản"><span className="font-mono">{reveal && canSeeBank ? o.bank.account : maskAccount(o.bank.account)}</span></Field>
                    </dl>
                    {canSeeBank ? <Button variant="link" size="sm" className="mt-1 h-auto px-0" onClick={() => setReveal(!reveal)}>{reveal ? "Ẩn" : "Hiện"} số tài khoản</Button>
                      : <p className="mt-2 flex items-center gap-1 text-xs text-muted-foreground"><Lock className="size-3" /> Bạn không có quyền xem đầy đủ.</p>}
                  </div>
                  <dl className="grid grid-cols-2 gap-3 rounded-lg border p-4">
                    <Field label="Giá trị đủ điều kiện">{vnd(detail.gross)}</Field>
                    <Field label={`Phí sàn ${commissionPct}%`}>−{vnd(fee)}</Field>
                    <Field label="Điều chỉnh">{vnd(detail.adjustment)}</Field>
                    <Field label="Thực nhận"><span className="text-primary">{vnd(net)}</span></Field>
                    <Field label="Trạng thái"><StatusBadge status={detail.status} /></Field>
                    <Field label="Mã tham chiếu">{detail.ref ?? "—"}</Field>
                  </dl>
                  <div>
                    <p className="mb-2 text-sm font-semibold">Đơn liên quan</p>
                    {detail.orderIds.length ? <div className="flex flex-wrap gap-2">{detail.orderIds.map((id) => <span key={id} className="plate">{id}</span>)}</div> : <p className="text-sm text-muted-foreground">{detail.trips} chuyến (danh sách lấy từ backend).</p>}
                  </div>
                  {detail.status !== "settled" && (
                    <Button className="w-full" disabled={detail.status === "discrepancy"} onClick={() => { setRef(""); setRefErr(""); setConfirm(detail); }}>Xác nhận đã chuyển khoản quyết toán</Button>
                  )}
                  {detail.status === "discrepancy" && <p className="text-xs text-destructive">Kỳ này có sai lệch, cần xử lý trước khi quyết toán.</p>}
                  <DemoNotice>Bản demo chỉ mô phỏng. Bản thật cần backend xác nhận giao dịch ngân hàng và lưu dấu vết kiểm toán.</DemoNotice>
                </div>
              </>
            );
          })()}
        </SheetContent>
      </Sheet>

      <ConfirmDialog open={!!confirm} onOpenChange={(o) => !o && setConfirm(null)} title="Xác nhận đã chuyển khoản quyết toán"
        desc={confirm && <>Chủ xe <b>{ownerById(confirm.ownerId).name}</b> · {vnd(payout(confirm.gross, commissionPct, confirm.adjustment).net)}</>}
        onConfirm={() => {
          if (!confirm || !doConfirm()) throw new Error("invalid");
          update((s) => ({ settlements: s.settlements.map((x) => x.id === confirm.id ? { ...x, status: "settled", settledAt: new Date().toISOString(), ref: ref.trim() } : x) }), { action: "Xác nhận quyết toán", target: confirm.id, reason: ref.trim() });
          toast.success("Đã ghi nhận quyết toán (mô phỏng). Chờ backend xác minh giao dịch.");
          setDetail(null);
        }}>
        <div className="space-y-1.5">
          <Label htmlFor="ref">Mã giao dịch tham chiếu <span className="text-destructive">*</span></Label>
          <Input id="ref" value={ref} onChange={(e) => { setRef(e.target.value); setRefErr(""); }} placeholder="VD: VCB-FT26292-10045" />
          {refErr && <p className="text-xs text-destructive">{refErr}</p>}
        </div>
      </ConfirmDialog>
    </>
  );
}
