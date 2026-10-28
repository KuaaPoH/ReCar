import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import { CalendarClock, CheckCircle2, ClipboardList, Eye, EyeOff, Mail, Phone } from "lucide-react";
import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/host/PageHeader";
import { OrderStatusBadge } from "@/components/host/StatusBadge";
import { HANDOVER_CHECKS, OrderActions } from "@/components/host/OrderActions";
import { useHost } from "@/lib/host/store";
import { DEPOSIT_RATE, financeBreakdown, PLATFORM_FEE_RATE } from "@/lib/host/rules";
import { formatDateTime, formatVND, initials, maskTail } from "@/lib/host/format";

export const Route = createFileRoute("/CarOwner/Orders/Details/$id")({
  head: () => ({
    meta: [
      { title: "Chi tiết đơn thuê — ReCar Host Pro" },
      { name: "description", content: "Thông tin xe, khách thuê, tài chính và thao tác của đơn thuê." },
      { property: "og:title", content: "Chi tiết đơn thuê — ReCar Host Pro" },
      { property: "og:description", content: "Chi tiết đơn thuê xe trên ReCar." },
    ],
  }),
  component: OrderDetails,
});

function OrderDetails() {
  const { id } = Route.useParams();
  const { orders, cars } = useHost();
  const [reveal, setReveal] = useState(false);
  const o = orders.find((x) => x.id === id);
  const car = o && cars.find((c) => c.id === o.carId);
  if (!o || !car)
    return <div className="card-surface"><EmptyState icon={<ClipboardList />} title="Không tìm thấy đơn thuê" body="Đơn không tồn tại hoặc không thuộc xe của bạn." action={<Button asChild><Link to="/CarOwner/Orders/Index">Về danh sách đơn</Link></Button>} /></div>;
  const f = financeBreakdown(o.total);
  const pct = (r: number) => `${Math.round(r * 100)}%`;

  return (
    <div className="space-y-6">
      <div className="card-surface overflow-hidden">
        <div className="grid md:grid-cols-[minmax(0,420px)_1fr]">
          <img src={car.image} alt={car.name} className="h-full max-h-72 w-full object-cover" />
          <div className="space-y-3 p-6">
            <div className="flex flex-wrap items-center gap-3">
              <span className="font-mono text-sm font-semibold text-muted-foreground">{o.id}</span>
              <OrderStatusBadge status={o.status} />
            </div>
            <h1 className="text-2xl font-bold">{car.name}</h1>
            <span className="plate">{car.plate}</span>
            <p className="text-sm text-muted-foreground">{car.seats} chỗ · {car.transmission} · {car.fuel} · {car.color}</p>
            <div className="grid max-w-md grid-cols-2 gap-3 pt-2">
              <div className="rounded-xl bg-muted p-3"><p className="text-xs text-muted-foreground">Nhận xe</p><p className="font-semibold">{formatDateTime(o.pickupAt)}</p></div>
              <div className="rounded-xl bg-muted p-3"><p className="text-xs text-muted-foreground">Trả xe</p><p className="font-semibold">{formatDateTime(o.returnAt)}</p></div>
            </div>
            <div className="pt-2"><OrderActions order={o} size="default" /></div>
          </div>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="card-surface space-y-4 p-6">
          <h2 className="font-bold">Khách thuê</h2>
          <div className="flex items-center gap-3">
            <span className="grid size-12 place-items-center rounded-full bg-primary-soft font-bold text-accent-foreground">{initials(o.guestName)}</span>
            <p className="text-lg font-semibold">{o.guestName}</p>
          </div>
          <a href={`tel:${o.guestPhone}`} className="flex items-center gap-2 text-sm text-primary"><Phone className="size-4" />{o.guestPhone}</a>
          <p className="flex items-center gap-2 text-sm"><Mail className="size-4 text-muted-foreground" />{o.guestEmail}</p>
          <div className="space-y-1 rounded-xl bg-muted p-3 text-sm">
            <p className="flex justify-between"><span className="text-muted-foreground">CCCD</span><span className="font-mono">{reveal ? o.guestIdNumber : maskTail(o.guestIdNumber)}</span></p>
            <p className="flex justify-between"><span className="text-muted-foreground">GPLX</span><span className="font-mono">{reveal ? o.guestLicense : maskTail(o.guestLicense)}</span></p>
            {o.status === 3 && (
              <button onClick={() => setReveal((r) => !r)} className="mt-1 flex items-center gap-1 text-xs font-medium text-primary">
                {reveal ? <EyeOff className="size-3.5" /> : <Eye className="size-3.5" />}{reveal ? "Ẩn" : "Xem đầy đủ để đối chiếu khi bàn giao"}
              </button>
            )}
          </div>
        </div>

        <div className="card-surface p-6">
          <h2 className="mb-4 font-bold">Checklist bàn giao an toàn</h2>
          <ul className="space-y-3 text-sm">
            {HANDOVER_CHECKS.map((c) => (
              <li key={c} className="flex items-center gap-2">
                <CheckCircle2 className={o.status >= 4 ? "size-4.5 text-success" : "size-4.5 text-border"} />{c}
              </li>
            ))}
          </ul>
          <p className="mt-4 text-xs text-muted-foreground">Checklist được xác nhận trong hộp thoại Bàn giao xe.</p>
        </div>

        <div className="card-surface p-6">
          <h2 className="mb-4 font-bold">Phân rã tài chính</h2>
          <dl className="space-y-2.5 text-sm">
            <div className="flex justify-between"><dt>Tổng tiền thuê ({o.days} ngày)</dt><dd className="font-bold">{formatVND(f.total)}</dd></div>
            <div className="flex justify-between"><dt className="text-muted-foreground">Cọc {pct(DEPOSIT_RATE)} — sàn thu</dt><dd>{formatVND(f.deposit)}</dd></div>
            <div className="flex justify-between"><dt className="text-muted-foreground">Còn lại {pct(1 - DEPOSIT_RATE)} — khách trả bạn khi nhận xe</dt><dd>{formatVND(f.remaining)}</dd></div>
            <div className="flex justify-between border-t pt-2.5"><dt className="text-muted-foreground">Phí dịch vụ sàn {pct(PLATFORM_FEE_RATE)} (trừ một lần từ tiền cọc)</dt><dd className="text-destructive">−{formatVND(f.platformFee)}</dd></div>
            <div className="flex justify-between"><dt className="text-muted-foreground">Sàn chuyển lại cho bạn</dt><dd>{formatVND(f.payoutFromPlatform)}</dd></div>
            <div className="flex justify-between rounded-xl bg-success-soft p-3 text-success"><dt className="font-semibold">Thu nhập thực nhận dự kiến</dt><dd className="font-bold">{formatVND(f.ownerNet)}</dd></div>
          </dl>
          <p className="mt-3 text-xs text-muted-foreground">Tỷ lệ theo cấu hình nghiệp vụ hiện tại; số liệu chính thức do hệ thống đối soát xác nhận.</p>
        </div>
      </div>

      <div className="card-surface p-6">
        <h2 className="mb-4 flex items-center gap-2 font-bold"><CalendarClock className="size-4.5" /> Lịch sử trạng thái</h2>
        <ol className="relative space-y-4 border-l pl-6">
          {o.history.map((h, i) => (
            <li key={i}>
              <span className="absolute -left-1.5 mt-1.5 size-3 rounded-full bg-primary" />
              <p className="text-sm font-semibold">{h.label}</p>
              <p className="text-xs text-muted-foreground">{formatDateTime(h.at)}{h.note && ` — ${h.note}`}</p>
            </li>
          ))}
        </ol>
      </div>
    </div>
  );
}
