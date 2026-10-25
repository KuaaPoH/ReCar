import { useEffect, useMemo, useState, type ReactNode } from "react";
import { AlertTriangle, ChevronLeft, ChevronRight, Inbox, Loader2, RotateCcw } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import {
  Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle,
} from "@/components/ui/dialog";
import { cn } from "@/lib/utils";
import { simulate } from "@/data/store";

/* ---------- Status badge ---------- */
type Tone = "success" | "warning" | "danger" | "info" | "neutral";
const toneCls: Record<Tone, string> = {
  success: "bg-success-soft text-success",
  warning: "bg-warning-soft text-warning",
  danger: "bg-danger-soft text-destructive",
  info: "bg-info-soft text-accent-foreground",
  neutral: "bg-muted text-muted-foreground",
};

export const STATUS: Record<string, [string, Tone]> = {
  ready: ["Sẵn sàng", "success"], rented: ["Đang có khách", "info"], maintenance: ["Bảo trì", "warning"],
  pending: ["Chờ duyệt", "warning"], suspended: ["Tạm ngừng", "neutral"], rejected: ["Từ chối", "danger"], hidden: ["Đã ẩn", "neutral"],
  created: ["Khởi tạo", "neutral"], awaiting_deposit: ["Chờ thanh toán cọc", "warning"], deposited: ["Đã đặt cọc", "info"],
  awaiting_handover: ["Chờ bàn giao", "info"], renting: ["Đang thuê", "info"], awaiting_completion: ["Chờ hoàn tất", "warning"],
  completed: ["Hoàn thành", "success"], cancelled: ["Đã hủy", "danger"], refunding: ["Đang hoàn tiền", "warning"], refunded: ["Đã hoàn tiền", "neutral"],
  unpaid: ["Chưa thanh toán", "neutral"], deposit_paid: ["Đã cọc 30%", "info"], paid: ["Đã thanh toán đủ", "success"],
  processing: ["Đang xử lý", "info"], settled: ["Đã quyết toán", "success"], discrepancy: ["Có sai lệch", "danger"],
  active: ["Hoạt động", "success"], locked: ["Đã khóa", "danger"], verified: ["Đã xác minh", "success"],
  draft: ["Bản nháp", "neutral"], published: ["Đã xuất bản", "success"],
};

export function StatusBadge({ status, label }: { status: string; label?: string }) {
  const [text, tone] = STATUS[status] ?? [status, "neutral" as Tone];
  return (
    <span className={cn("inline-flex items-center gap-1.5 whitespace-nowrap rounded-full px-2.5 py-0.5 text-xs font-medium", toneCls[tone])}>
      <span className="size-1.5 rounded-full bg-current" />
      {label ?? text}
    </span>
  );
}

/* ---------- Page header ---------- */
export function PageHeader({ title, desc, actions }: { title: string; desc?: string; actions?: ReactNode }) {
  return (
    <div className="mb-6 flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">{title}</h1>
        {desc && <p className="mt-1 text-sm text-muted-foreground">{desc}</p>}
      </div>
      {actions && <div className="flex flex-wrap gap-2">{actions}</div>}
    </div>
  );
}

export function Panel({ title, action, children, className }: { title?: ReactNode; action?: ReactNode; children: ReactNode; className?: string }) {
  return (
    <section className={cn("rounded-xl border bg-card shadow-card", className)}>
      {title && (
        <div className="flex items-center justify-between gap-2 border-b px-5 py-3.5">
          <h2 className="text-sm font-semibold">{title}</h2>
          {action}
        </div>
      )}
      {children}
    </section>
  );
}

/* ---------- States ---------- */
export function EmptyState({ title = "Không có dữ liệu", desc = "Thử thay đổi bộ lọc hoặc từ khóa tìm kiếm." }) {
  return (
    <div className="flex flex-col items-center justify-center px-6 py-14 text-center">
      <div className="mb-3 grid size-12 place-items-center rounded-full bg-muted"><Inbox className="size-5 text-muted-foreground" /></div>
      <p className="font-medium">{title}</p>
      <p className="mt-1 text-sm text-muted-foreground">{desc}</p>
    </div>
  );
}

export function ErrorState({ onRetry }: { onRetry: () => void }) {
  return (
    <div className="flex flex-col items-center justify-center px-6 py-14 text-center">
      <div className="mb-3 grid size-12 place-items-center rounded-full bg-danger-soft"><AlertTriangle className="size-5 text-destructive" /></div>
      <p className="font-medium">Không tải được dữ liệu</p>
      <p className="mt-1 text-sm text-muted-foreground">Kết nối tới máy chủ bị gián đoạn.</p>
      <Button variant="outline" size="sm" className="mt-4" onClick={onRetry}><RotateCcw /> Thử lại</Button>
    </div>
  );
}

export function TableSkeleton({ rows = 5 }: { rows?: number }) {
  return (
    <div className="space-y-3 p-5">
      {Array.from({ length: rows }).map((_, i) => <Skeleton key={i} className="h-10 w-full" />)}
    </div>
  );
}

/** Mô phỏng tải dữ liệu. Thêm ?demoError=1 vào URL để xem trạng thái lỗi. */
export function useDemoLoad() {
  const [state, setState] = useState<"loading" | "error" | "ready">("loading");
  const load = () => {
    setState("loading");
    simulate(500).then(() => {
      const fail = typeof window !== "undefined" && new URLSearchParams(window.location.search).has("demoError");
      setState(fail ? "error" : "ready");
    });
  };
  useEffect(load, []);
  return { state, retry: () => { const u = new URL(window.location.href); u.searchParams.delete("demoError"); window.history.replaceState(null, "", u); load(); } };
}

export function useDebounced<T>(v: T, ms = 250) {
  const [d, setD] = useState(v);
  useEffect(() => { const t = setTimeout(() => setD(v), ms); return () => clearTimeout(t); }, [v, ms]);
  return d;
}

/* ---------- Pagination ---------- */
export function usePaged<T>(items: T[], size = 6) {
  const [page, setPage] = useState(1);
  const pages = Math.max(1, Math.ceil(items.length / size));
  useEffect(() => { if (page > pages) setPage(1); }, [pages, page]);
  const slice = useMemo(() => items.slice((page - 1) * size, page * size), [items, page, size]);
  return { slice, page, pages, setPage, total: items.length, size };
}

export function Pager({ page, pages, setPage, total, size }: { page: number; pages: number; setPage: (n: number) => void; total: number; size: number }) {
  if (total === 0) return null;
  const from = (page - 1) * size + 1;
  const to = Math.min(total, page * size);
  return (
    <div className="flex items-center justify-between border-t px-5 py-3 text-sm text-muted-foreground">
      <span>Hiển thị {from}–{to} / {total}</span>
      <div className="flex items-center gap-1">
        <Button variant="outline" size="icon" className="size-8" disabled={page === 1} onClick={() => setPage(page - 1)} aria-label="Trang trước"><ChevronLeft /></Button>
        {Array.from({ length: pages }).map((_, i) => (
          <Button key={i} variant={page === i + 1 ? "default" : "ghost"} size="icon" className="size-8" onClick={() => setPage(i + 1)}>{i + 1}</Button>
        ))}
        <Button variant="outline" size="icon" className="size-8" disabled={page === pages} onClick={() => setPage(page + 1)} aria-label="Trang sau"><ChevronRight /></Button>
      </div>
    </div>
  );
}

/* ---------- Confirm / reason dialog ---------- */
export function ConfirmDialog({
  open, onOpenChange, title, desc, confirmLabel = "Xác nhận", destructive, requireReason, reasonLabel = "Lý do", children, onConfirm,
}: {
  open: boolean; onOpenChange: (o: boolean) => void; title: string; desc?: ReactNode; confirmLabel?: string;
  destructive?: boolean; requireReason?: boolean; reasonLabel?: string; children?: ReactNode;
  onConfirm: (reason: string) => Promise<void> | void;
}) {
  const [reason, setReason] = useState("");
  const [err, setErr] = useState("");
  const [busy, setBusy] = useState(false);
  useEffect(() => { if (open) { setReason(""); setErr(""); setBusy(false); } }, [open]);
  const submit = async () => {
    if (requireReason && reason.trim().length < 5) { setErr("Vui lòng nhập lý do (tối thiểu 5 ký tự)."); return; }
    setBusy(true);
    await simulate();
    await onConfirm(reason.trim());
    setBusy(false);
    onOpenChange(false);
  };
  return (
    <Dialog open={open} onOpenChange={(o) => !busy && onOpenChange(o)}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          {desc && <DialogDescription asChild><div>{desc}</div></DialogDescription>}
        </DialogHeader>
        {children}
        {requireReason && (
          <div className="space-y-1.5">
            <Label htmlFor="reason">{reasonLabel} <span className="text-destructive">*</span></Label>
            <Textarea id="reason" rows={3} value={reason} onChange={(e) => { setReason(e.target.value); setErr(""); }} placeholder="Nhập lý do cụ thể…" />
            {err && <p className="text-xs text-destructive">{err}</p>}
          </div>
        )}
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)} disabled={busy}>Hủy bỏ</Button>
          <Button variant={destructive ? "destructive" : "default"} onClick={submit} disabled={busy}>
            {busy && <Loader2 className="animate-spin" />} {confirmLabel}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

export function Field({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div>
      <dt className="text-xs text-muted-foreground">{label}</dt>
      <dd className="mt-0.5 text-sm font-medium">{children}</dd>
    </div>
  );
}

export function DemoNotice({ children }: { children: ReactNode }) {
  return (
    <div className="flex items-start gap-2 rounded-lg border border-dashed bg-warning-soft/60 px-3 py-2 text-xs text-warning">
      <AlertTriangle className="mt-0.5 size-3.5 shrink-0" />
      <span>{children}</span>
    </div>
  );
}
