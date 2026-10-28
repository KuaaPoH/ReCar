import { cn } from "@/lib/utils";
import { CAR_STATUS_LABEL, ORDER_STATUS_LABEL } from "@/lib/host/rules";
import type { CarStatus, OrderStatus } from "@/lib/host/types";

const carTone: Record<CarStatus, string> = {
  available: "bg-success-soft text-success",
  busy: "bg-info-soft text-info",
  pending: "bg-warning-soft text-warning",
  rejected: "bg-destructive-soft text-destructive",
  paused: "bg-muted text-muted-foreground",
  maintenance: "bg-maintenance-soft text-maintenance",
};
const orderTone: Record<OrderStatus, string> = {
  2: "bg-destructive-soft text-destructive",
  3: "bg-warning-soft text-warning",
  4: "bg-info-soft text-info",
  5: "bg-success-soft text-success",
};

function Pill({ className, children }: { className: string; children: React.ReactNode }) {
  return (
    <span className={cn("inline-flex items-center gap-1.5 whitespace-nowrap rounded-full px-2.5 py-1 text-xs font-semibold", className)}>
      <span className="size-1.5 rounded-full bg-current" />
      {children}
    </span>
  );
}

export const CarStatusBadge = ({ status }: { status: CarStatus }) => (
  <Pill className={carTone[status]}>{CAR_STATUS_LABEL[status]}</Pill>
);
export const OrderStatusBadge = ({ status }: { status: OrderStatus }) => (
  <Pill className={orderTone[status]}>{ORDER_STATUS_LABEL[status]}</Pill>
);
