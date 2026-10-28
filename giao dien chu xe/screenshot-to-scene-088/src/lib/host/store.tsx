import { createContext, useContext, useMemo, useState, type ReactNode } from "react";
import * as seed from "./mock-data";
import { completeOrder } from "./rules";
import type { Car, HostNotification, Order, OwnerProfile } from "./types";

interface HostState {
  owner: OwnerProfile;
  cars: Car[];
  orders: Order[];
  reviews: typeof seed.reviews;
  notifications: HostNotification[];
  setOwner: (o: OwnerProfile) => void;
  updateCar: (id: number, patch: Partial<Car>) => void;
  addCar: (c: Omit<Car, "id" | "ownerId">) => number;
  removeCar: (id: number) => void;
  handover: (orderId: string) => void;
  receiveBack: (orderId: string) => void;
  cancelOrder: (orderId: string, reason: string) => void;
  markAllRead: () => void;
}

const Ctx = createContext<HostState | null>(null);
const nowStamp = () => `${seed.DEMO_TODAY}T${new Date().toTimeString().slice(0, 5)}`;

export function HostProvider({ children }: { children: ReactNode }) {
  const me = seed.CURRENT_OWNER_ID;
  // Scope to the signed-in owner (backend does this from the auth session).
  const [cars, setCars] = useState(() => seed.cars.filter((c) => c.ownerId === me));
  const myCarIds = new Set(cars.map((c) => c.id));
  const [orders, setOrders] = useState(() =>
    seed.orders.filter((o) => seed.cars.some((c) => c.id === o.carId && c.ownerId === me)),
  );
  const [owner, setOwner] = useState(seed.owner);
  const [notifications, setNotifications] = useState(seed.notifications);
  const reviews = seed.reviews.filter((r) => myCarIds.has(r.carId));

  const value = useMemo<HostState>(
    () => ({
      owner, cars, orders, reviews, notifications, setOwner,
      updateCar: (id, patch) => setCars((cs) => cs.map((c) => (c.id === id ? { ...c, ...patch } : c))),
      addCar: (c) => {
        const id = Math.max(...cars.map((x) => x.id), 100) + 1;
        setCars((cs) => [{ ...c, id, ownerId: me }, ...cs]);
        return id;
      },
      removeCar: (id) => setCars((cs) => cs.filter((c) => c.id !== id)),
      handover: (orderId) => {
        const o = orders.find((x) => x.id === orderId);
        if (!o || o.status !== 3 || !o.depositConfirmed) return;
        setOrders((os) => os.map((x) => (x.id === orderId ? { ...x, status: 4, history: [...x.history, { at: nowStamp(), label: "Chủ xe bàn giao xe" }] } : x)));
        setCars((cs) => cs.map((c) => (c.id === o.carId ? { ...c, status: "busy" } : c)));
      },
      receiveBack: (orderId) => {
        const o = orders.find((x) => x.id === orderId);
        const car = o && cars.find((c) => c.id === o.carId);
        if (!o || !car || o.status !== 4) return;
        const hasNext = orders.some((x) => x.carId === car.id && x.id !== o.id && x.status === 4);
        const res = completeOrder(o, car, hasNext);
        setOrders((os) => os.map((x) => (x.id === orderId ? { ...res.order, history: [...x.history, { at: nowStamp(), label: "Chủ xe nhận lại xe — Hoàn tất" }] } : x)));
        setCars((cs) => cs.map((c) => (c.id === car.id ? res.car : c)));
      },
      cancelOrder: (orderId, reason) =>
        setOrders((os) => os.map((x) => (x.id === orderId && x.status === 3 ? { ...x, status: 2, cancelReason: reason, history: [...x.history, { at: nowStamp(), label: "Chủ xe hủy đơn", note: reason }] } : x))),
      markAllRead: () => setNotifications((ns) => ns.map((n) => ({ ...n, read: true }))),
    }),
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [owner, cars, orders, notifications],
  );
  return <Ctx.Provider value={value}>{children}</Ctx.Provider>;
}

export function useHost() {
  const v = useContext(Ctx);
  if (!v) throw new Error("useHost must be used inside HostProvider");
  return v;
}
