import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import { ArrowLeft, Ban, Check, FileText, RotateCcw, ShieldAlert } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { ConfirmDialog, DemoNotice, EmptyState, Field, PageHeader, Panel, StatusBadge, STATUS } from "@/components/admin/shared";
import { update, useStore } from "@/data/store";
import { customerById, ownerById, type OrderStatus } from "@/data/demo";
import { dateTimeVN, daysBetween, payout, splitDeposit, vnd } from "@/lib/format";
import { seo } from "@/lib/seo";
import { cn } from "@/lib/utils";

export const Route = createFileRoute("/admin/orders/$id")({
  head: ({ params }) => seo(`Đơn ${params.id}`, "Chi tiết đơn thuê xe ReCar."),
  component: OrderDetail,
});

const FLOW: OrderStatus[] = ["created", "awaiting_deposit", "deposited", "awaiting_handover", "renting", "awaiting_completion", "completed"];
const BRANCH: OrderStatus[] = ["cancelled", "rejected", "refunding", "refunded"];

function OrderDetail() {
  const { id } = Route.useParams();
  const order = useStore((s) => s.orders.find((o) => o.id === id));
  const cars = useStore((s) => s.cars);
  const { depositPct, commissionPct } = useStore((s) => s.settings);
  const [dlg, setDlg] = useState<"cancel" | "refund" | null>(null);

  if (!order) return <Panel><EmptyState title="Không tìm thấy đơn thuê" desc={`Mã ${id} không tồn tại.`} /></Panel>;
  const car = cars.find((c) => c.id === order.carId)!;
  const cu = customerById(order.customerId);
  const ow = ownerById(car.ownerId);
  const days = daysBetween(order.pickup, order.dropoff);
  const { deposit, remaining } = splitDeposit(order.total, depositPct);
  const { fee, net } = payout(order.total, commissionPct);
  const reached = new Set(order.history.map((h) => h.status));
  const branch = BRANCH.filter((b) => reached.has(b));
  const closed = ["completed", "cancelled", "rejected", "refunded"].includes(order.status);

  const act = (kind: "cancel" | "refund", reason: string) => {
    const status: OrderStatus = kind === "cancel" ? (order.payment === "unpaid" ? "cancelled" : "refunding") : "refunding";
    update((s) => ({
      orders: s.orders.map((o) => o.id !== id ? o : {
        ...o, status, payment: status === "refunding" ? "refunding" : o.payment, cancelReason: reason,
        history: [...o.history, ...(kind === "cancel" ? [{ status: "cancelled" as OrderStatus, at: new Date().toISOString(), by: "Trần Hải (Admin)", reason }] : []), ...(status === "refunding" ? [{ status: "refunding" as OrderStatus, at: new Date().toISOString(), by: "Trần Hải (Admin)", reason }] : [])],
      }),
    }), { action: kind === "cancel" ? "Hủy đơn" : "Tạo yêu cầu hoàn tiền", target: id, reason });
    toast.success(status === "refunding" ? "Đã tạo yêu cầu hoàn tiền. Trạng thái ‘Đã hoàn tiền’ chỉ cập nhật khi cổng thanh toán xác nhận." : "Đã hủy đơn.");
  };

  return (
    <>
      <Link to="/admin/orders" className="mb-3 inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"><ArrowLeft className="size-4" /> Danh sách đơn</Link>
      <PageHeader title={`Đơn ${order.id}`} desc={`Tạo lúc ${dateTimeVN(order.history[0].at)} · ${order.method}`}
        actions={<>
          <StatusBadge status={order.status} /><StatusBadge status={order.payment} />
          {!closed && <Button variant="outline" size="sm" className="text-destructive" onClick={() => setDlg("cancel")}><Ban /> Hủy đơn</Button>}
          {order.status === "cancelled" && order.payment !== "refunded" && order.payment !== "unpaid" && <Button variant="outline" size="sm" onClick={() => setDlg("refund")}><RotateCcw /> Hoàn tiền</Button>}
        </>} />

      <Panel title="Tiến trình đơn thuê" className="mb-6">
        <div className="overflow-x-auto p-5">
          <ol className="flex min-w-[760px] items-start">
            {FLOW.map((s, i) => {
              const done = reached.has(s);
              const current = order.status === s;
              return (
                <li key={s} className="flex flex-1 flex-col items-center text-center">
                  <div className="flex w-full items-center">
                    <span className={cn("h-0.5 flex-1", i === 0 ? "bg-transparent" : done ? "bg-primary" : "bg-border")} />
                    <span className={cn("grid size-8 shrink-0 place-items-center rounded-full border-2 text-xs font-bold", done ? "border-primary bg-primary text-primary-foreground" : "border-border bg-card text-muted-foreground", current && "ring-4 ring-accent")}>{done ? <Check className="size-4" /> : i + 1}</span>
                    <span className={cn("h-0.5 flex-1", i === FLOW.length - 1 ? "bg-transparent" : reached.has(FLOW[i + 1]) ? "bg-primary" : "bg-border")} />
                  </div>
                  <p className={cn("mt-2 px-1 text-xs", done ? "font-medium" : "text-muted-foreground")}>{STATUS[s][0]}</p>
                </li>
              );
            })}
          </ol>
          {branch.length > 0 && (
            <div className="mt-5 flex flex-wrap items-center gap-2 rounded-lg bg-danger-soft px-4 py-3 text-sm">
              <ShieldAlert className="size-4 text-destructive" /> Nhánh xử lý: {branch.map((b) => <StatusBadge key={b} status={b} />)}
              {order.cancelReason && <span className="text-muted-foreground">— {order.cancelReason}</span>}
            </div>
          )}
        </div>
      </Panel>

      <div className="grid gap-6 xl:grid-cols-3">
        <div className="space-y-6 xl:col-span-2">
          <div className="grid gap-6 md:grid-cols-2">
            <Panel title="Khách hàng"><dl className="grid gap-3 p-5"><Field label="Họ tên">{cu.name}</Field><Field label="Điện thoại">{cu.phone}</Field><Field label="Email">{cu.email}</Field></dl></Panel>
            <Panel title="Chủ xe"><dl className="grid gap-3 p-5"><Field label="Họ tên">{ow.name}</Field><Field label="Điện thoại">{ow.phone}</Field><Field label="Email">{ow.email}</Field></dl></Panel>
          </div>
          <Panel title="Xe & lịch thuê">
            <div className="flex flex-col gap-5 p-5 sm:flex-row">
              <img src={car.image} alt={car.name} className="w-full rounded-lg object-cover sm:w-56" />
              <dl className="grid flex-1 grid-cols-2 gap-4">
                <Field label="Xe">{car.name} ({car.year})</Field><Field label="Biển số"><span className="plate">{car.plate}</span></Field>
                <Field label="Nhận xe">{dateTimeVN(order.pickup)}</Field><Field label="Trả xe">{dateTimeVN(order.dropoff)}</Field>
                <Field label="Số ngày">{days} ngày</Field><Field label="Địa điểm bàn giao">{car.handover}</Field>
              </dl>
            </div>
          </Panel>
          <Panel title="Lịch sử thay đổi trạng thái">
            <ol className="space-y-4 p-5">
              {[...order.history].reverse().map((h, i) => (
                <li key={i} className="flex gap-3">
                  <span className={cn("mt-1.5 size-2.5 shrink-0 rounded-full", i === 0 ? "bg-primary" : "bg-border")} />
                  <div className="text-sm"><p className="font-medium">{STATUS[h.status][0]}</p><p className="text-xs text-muted-foreground">{dateTimeVN(h.at)} · {h.by}{h.reason && ` · Lý do: ${h.reason}`}</p></div>
                </li>
              ))}
            </ol>
          </Panel>
        </div>
        <div className="space-y-6">
          <Panel title="Chi tiết giá">
            <dl className="space-y-2.5 p-5 text-sm">
              <Row l={`Giá thuê ${vnd(car.pricePerDay)} × ${days} ngày`} v={vnd(order.total)} />
              <Row l={`Tiền cọc (${depositPct}%)`} v={vnd(deposit)} />
              <Row l={`Còn lại khi nhận xe (${100 - depositPct}%)`} v={vnd(remaining)} />
              <div className="border-t pt-2.5" />
              <Row l={`Phí sàn (${commissionPct}%)`} v={vnd(fee)} />
              <Row l="Chủ xe thực nhận" v={vnd(net)} bold />
            </dl>
          </Panel>
          <Panel title="Lịch sử thanh toán">
            <ul className="divide-y text-sm">
              {reached.has("deposited") && <li className="flex justify-between px-5 py-3"><span>Thanh toán cọc · {order.method}</span><span className="font-medium text-success">+{vnd(deposit)}</span></li>}
              {order.payment === "paid" && <li className="flex justify-between px-5 py-3"><span>Thanh toán phần còn lại</span><span className="font-medium text-success">+{vnd(remaining)}</span></li>}
              {reached.has("refunded") && <li className="flex justify-between px-5 py-3"><span>Hoàn tiền cho khách</span><span className="font-medium text-destructive">−{vnd(deposit)}</span></li>}
              {!reached.has("deposited") && <li className="px-5 py-3 text-muted-foreground">Chưa có giao dịch.</li>}
            </ul>
            <div className="p-4 pt-0"><DemoNotice>Trạng thái thanh toán chỉ được cập nhật khi backend xác minh callback từ cổng thanh toán.</DemoNotice></div>
          </Panel>
          <Panel title="Hợp đồng điện tử">
            <button onClick={() => toast.info(`Mở hợp đồng HD-${order.id}.pdf (bản tích hợp tải từ máy chủ).`)} className="flex w-full items-center gap-3 p-5 text-left text-sm hover:bg-muted">
              <FileText className="size-8 text-primary" /><span><span className="block font-medium">HD-{order.id}.pdf</span><span className="text-xs text-muted-foreground">{reached.has("deposited") ? "Đã ký bởi khách và chủ xe" : "Chưa phát hành"}</span></span>
            </button>
          </Panel>
          {order.note && <Panel title="Ghi chú"><p className="p-5 text-sm">{order.note}</p></Panel>}
        </div>
      </div>

      <ConfirmDialog open={dlg === "cancel"} onOpenChange={(o) => !o && setDlg(null)} title={`Hủy đơn ${order.id}?`} destructive requireReason confirmLabel="Hủy đơn"
        desc={order.payment === "unpaid" ? "Đơn chưa thanh toán sẽ chuyển sang Đã hủy." : "Khách đã thanh toán cọc — hệ thống sẽ tạo yêu cầu hoàn tiền theo chính sách."} onConfirm={(r) => act("cancel", r)} />
      <ConfirmDialog open={dlg === "refund"} onOpenChange={(o) => !o && setDlg(null)} title="Tạo yêu cầu hoàn tiền" requireReason confirmLabel="Gửi yêu cầu"
        desc={`Số tiền đề xuất hoàn: ${vnd(deposit)}.`} onConfirm={(r) => act("refund", r)} />
    </>
  );
}

function Row({ l, v, bold }: { l: string; v: string; bold?: boolean }) {
  return <div className="flex justify-between gap-3"><dt className="text-muted-foreground">{l}</dt><dd className={bold ? "font-bold" : "font-medium"}>{v}</dd></div>;
}
