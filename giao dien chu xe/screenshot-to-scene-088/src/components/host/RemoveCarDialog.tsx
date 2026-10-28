import { useState } from "react";
import { toast } from "sonner";
import { AlertTriangle, Loader2 } from "lucide-react";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { useHost } from "@/lib/host/store";
import { removeMode } from "@/lib/host/rules";
import { DEMO_NOTE } from "@/lib/host/api";
import type { Car } from "@/lib/host/types";

export function RemoveCarDialog({ car, open, onOpenChange, onDone }: { car: Car | null; open: boolean; onOpenChange: (v: boolean) => void; onDone?: () => void }) {
  const { orders, updateCar, removeCar } = useHost();
  const [busy, setBusy] = useState(false);
  if (!car) return null;
  const { mode, reason } = removeMode(car, orders);

  const run = async () => {
    setBusy(true);
    await new Promise((r) => setTimeout(r, 600));
    if (mode === "soft") {
      updateCar(car.id, { status: "paused" });
      toast.success(`Đã tạm ngừng kinh doanh ${car.name}`, { description: DEMO_NOTE });
    } else {
      removeCar(car.id);
      toast.success(`Đã gỡ ${car.name}`, { description: DEMO_NOTE });
    }
    setBusy(false);
    onOpenChange(false);
    onDone?.();
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2"><AlertTriangle className="size-5 text-destructive" /> Gỡ xe {car.name}</DialogTitle>
          <DialogDescription>Biển số <span className="plate">{car.plate}</span></DialogDescription>
        </DialogHeader>
        {mode === "blocked" && (
          <p className="rounded-xl bg-destructive-soft p-4 text-sm text-destructive">Không thể gỡ xe: {reason} Hãy hoàn tất hoặc xử lý các đơn liên quan trước.</p>
        )}
        {mode === "soft" && (
          <div className="space-y-2 rounded-xl bg-warning-soft p-4 text-sm text-warning">
            <p className="font-semibold">Xe đã phát sinh giao dịch nên sẽ được tạm ngừng kinh doanh (xóa mềm).</p>
            <p>Xe bị ẩn khỏi sàn và không nhận đơn mới. Lịch sử đơn, doanh thu và đánh giá vẫn được giữ lại để đối soát.</p>
          </div>
        )}
        {mode === "hard" && (
          <p className="rounded-xl bg-muted p-4 text-sm">Xe chưa phát sinh giao dịch. Hồ sơ xe và hình ảnh sẽ bị xóa vĩnh viễn và không thể khôi phục.</p>
        )}
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>Đóng</Button>
          {mode !== "blocked" && (
            <Button variant="destructive" onClick={run} disabled={busy}>
              {busy && <Loader2 className="animate-spin" />}{mode === "soft" ? "Tạm ngừng kinh doanh" : "Xóa vĩnh viễn"}
            </Button>
          )}
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
