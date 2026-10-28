import { useState } from "react";
import { toast } from "sonner";
import { KeyRound, Loader2, RotateCcw, XCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Textarea } from "@/components/ui/textarea";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { useHost } from "@/lib/host/store";
import { canCancel, canHandover, canReceiveBack } from "@/lib/host/rules";
import { formatDateTime } from "@/lib/host/format";
import { DEMO_NOTE } from "@/lib/host/api";
import type { Order } from "@/lib/host/types";

type Kind = "handover" | "return" | "cancel";

export const HANDOVER_CHECKS = ["Xác nhận đúng khách thuê (đối chiếu CCCD)", "Kiểm tra giấy phép lái xe còn hiệu lực", "Kiểm tra & chụp ảnh tình trạng xe", "Xác nhận thời gian bàn giao"];
const RETURN_CHECKS = ["Kiểm tra ngoại thất & nội thất", "Ghi nhận ODO và mức nhiên liệu/pin", "Đã thu đủ 70% còn lại theo quy trình", "Đã nhận lại chìa khóa & giấy tờ xe"];

export function OrderActions({ order, size = "sm" }: { order: Order; size?: "sm" | "default" }) {
  const [kind, setKind] = useState<Kind | null>(null);
  return (
    <>
      <div className="flex flex-wrap gap-2">
        {order.status === 3 && (
          <Button size={size} onClick={() => (canHandover(order) ? setKind("handover") : toast.error("Chưa thể bàn giao", { description: "Cọc 30% của đơn chưa được sàn xác nhận." }))} variant={canHandover(order) ? "default" : "outline"}>
            <KeyRound /> Bàn giao xe
          </Button>
        )}
        {canReceiveBack(order) && <Button size={size} onClick={() => setKind("return")}><RotateCcw /> Nhận lại xe</Button>}
        {canCancel(order) && <Button size={size} variant="outline" className="text-destructive" onClick={() => setKind("cancel")}><XCircle /> Hủy đơn</Button>}
      </div>
      {kind && <ActionDialog order={order} kind={kind} onClose={() => setKind(null)} />}
    </>
  );
}

function ActionDialog({ order, kind, onClose }: { order: Order; kind: Kind; onClose: () => void }) {
  const { cars, handover, receiveBack, cancelOrder } = useHost();
  const car = cars.find((c) => c.id === order.carId);
  const checks = kind === "handover" ? HANDOVER_CHECKS : kind === "return" ? RETURN_CHECKS : [];
  const [ticked, setTicked] = useState<boolean[]>(checks.map(() => false));
  const [reason, setReason] = useState("");
  const [busy, setBusy] = useState(false);
  const ready = kind === "cancel" ? reason.trim().length >= 10 : ticked.every(Boolean);

  const confirm = async () => {
    if (busy || !ready) return; // guard against double submit
    setBusy(true);
    await new Promise((r) => setTimeout(r, 700));
    if (kind === "handover") { handover(order.id); toast.success(`Đã bàn giao ${order.id} — Đang cho thuê`, { description: DEMO_NOTE }); }
    if (kind === "return") { receiveBack(order.id); toast.success(`Đơn ${order.id} hoàn tất`, { description: DEMO_NOTE }); }
    if (kind === "cancel") { cancelOrder(order.id, reason.trim()); toast.success(`Đã hủy đơn ${order.id}`, { description: DEMO_NOTE }); }
    setBusy(false);
    onClose();
  };

  const title = { handover: "Xác nhận bàn giao xe", return: "Xác nhận nhận lại xe", cancel: "Hủy đơn thuê" }[kind];
  return (
    <Dialog open onOpenChange={(v) => !v && onClose()}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          <DialogDescription>
            <span className="font-mono font-semibold">{order.id}</span> · {car?.name} <span className="plate">{car?.plate}</span>
          </DialogDescription>
        </DialogHeader>
        <div className="rounded-xl bg-muted p-3 text-sm">
          <p><b>{order.guestName}</b> · {order.guestPhone}</p>
          <p className="text-muted-foreground">{formatDateTime(order.pickupAt)} → {formatDateTime(order.returnAt)} ({order.days} ngày)</p>
        </div>
        {kind !== "cancel" ? (
          <ul className="space-y-2.5">
            {checks.map((c, i) => (
              <li key={c}>
                <label className="flex cursor-pointer items-center gap-3 text-sm">
                  <Checkbox checked={ticked[i]} onCheckedChange={(v) => setTicked((t) => t.map((x, j) => (j === i ? !!v : x)))} />{c}
                </label>
              </li>
            ))}
          </ul>
        ) : (
          <div className="space-y-2">
            <p className="rounded-xl bg-destructive-soft p-3 text-sm text-destructive">Khách sẽ nhận thông báo hủy. Việc hoàn cọc thực hiện theo chính sách của sàn; lịch xe sẽ được mở lại.</p>
            <Textarea value={reason} onChange={(e) => setReason(e.target.value)} placeholder="Lý do hủy (bắt buộc, tối thiểu 10 ký tự)" />
          </div>
        )}
        <DialogFooter>
          <Button variant="outline" onClick={onClose}>Đóng</Button>
          <Button variant={kind === "cancel" ? "destructive" : "default"} disabled={!ready || busy} onClick={confirm}>
            {busy && <Loader2 className="animate-spin" />}{kind === "cancel" ? "Xác nhận hủy" : "Xác nhận"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
