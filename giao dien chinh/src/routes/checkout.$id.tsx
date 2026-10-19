import { createFileRoute, Link, notFound } from "@tanstack/react-router";
import { useState } from "react";
import { CheckCircle2, CreditCard, XCircle } from "lucide-react";
import { SiteLayout } from "@/components/site/Layout";
import { cars, DEPOSIT_RATE, vnd } from "@/lib/data";

export const Route = createFileRoute("/checkout/$id")({
  loader: ({ params }) => {
    const car = cars.find((c) => String(c.id) === params.id);
    if (!car) throw notFound();
    return { car };
  },
  head: () => ({ meta: [
    { title: "Xác nhận đặt xe | ReCar" },
    { name: "description", content: "Điền thông tin và thanh toán tiền cọc 30% để giữ xe." },
    { property: "og:title", content: "Xác nhận đặt xe | ReCar" },
    { property: "og:description", content: "Thanh toán tiền cọc trực tuyến." },
  ] }),
  component: Checkout,
});

const inp = "w-full rounded-lg border px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-ring";

function Checkout() {
  const { car } = Route.useLoaderData();
  const [days, setDays] = useState(3);
  const [step, setStep] = useState<"form" | "ok" | "fail">("form");
  const [agree, setAgree] = useState(false);
  const total = car.price * days;
  const deposit = total * DEPOSIT_RATE;

  if (step !== "form") {
    const ok = step === "ok";
    return (
      <SiteLayout>
        <div className="mx-auto max-w-lg px-4 py-20 text-center">
          {ok ? <CheckCircle2 className="mx-auto h-16 w-16 text-success" /> : <XCircle className="mx-auto h-16 w-16 text-destructive" />}
          <h1 className="mt-4 text-2xl font-bold text-navy">{ok ? "Đặt cọc thành công" : "Thanh toán không thành công"}</h1>
          <p className="mt-2 text-muted-foreground">{ok ? `Mã đơn RC10250 · Đã cọc ${vnd(deposit)}. Chủ xe sẽ xác nhận trong thời gian sớm nhất.` : "Giao dịch đã bị hủy hoặc lỗi. Bạn có thể thử lại."}</p>
          <p className="mt-2 text-xs text-warning">Kết quả mô phỏng – chưa kết nối VNPay thực tế.</p>
          <div className="mt-6 flex justify-center gap-3">
            {ok ? <Link to="/account" className="rounded-lg bg-primary px-5 py-2.5 font-semibold text-primary-foreground">Xem đơn thuê</Link> : <button onClick={() => setStep("form")} className="rounded-lg bg-primary px-5 py-2.5 font-semibold text-primary-foreground">Thử lại</button>}
            <Link to="/" className="rounded-lg border px-5 py-2.5 font-medium">Trang chủ</Link>
          </div>
        </div>
      </SiteLayout>
    );
  }

  return (
    <SiteLayout>
      <div className="mx-auto grid max-w-6xl gap-8 px-4 py-10 lg:grid-cols-[1fr_380px]">
        <form className="space-y-6" onSubmit={(e) => { e.preventDefault(); setStep("ok"); }}>
          <h1 className="text-3xl font-bold text-navy">Xác nhận đặt xe</h1>
          <Box t="Thông tin người thuê">
            <div className="grid gap-4 md:grid-cols-2">
              <input required className={inp} placeholder="Họ và tên" defaultValue="Hoàng Lan" />
              <input required className={inp} placeholder="Số điện thoại" pattern="0[0-9]{9}" title="Số điện thoại 10 chữ số" />
              <input required type="email" className={inp} placeholder="Email" />
              <input required className={inp} placeholder="Số CCCD" pattern="[0-9]{12}" title="CCCD gồm 12 chữ số" />
            </div>
          </Box>
          <Box t="Thời gian thuê">
            <label className="text-sm">Số ngày thuê
              <input type="number" min={1} max={30} value={days} onChange={(e) => setDays(Math.max(1, +e.target.value))} className={`${inp} mt-1 max-w-32`} />
            </label>
          </Box>
          <Box t="Phương thức thanh toán">
            <label className="flex items-center gap-3 rounded-lg border border-primary bg-accent p-4"><input type="radio" defaultChecked /><CreditCard className="h-5 w-5 text-primary" /><span className="text-sm font-medium">VNPay (ATM / QR / Thẻ quốc tế)</span></label>
          </Box>
          <label className="flex items-start gap-2 text-sm"><input type="checkbox" checked={agree} onChange={(e) => setAgree(e.target.checked)} className="mt-1" />Tôi đồng ý với điều khoản thuê xe và chính sách đặt cọc của ReCar.</label>
          <div className="flex gap-3">
            <button disabled={!agree} className="rounded-lg bg-primary px-6 py-3 font-semibold text-primary-foreground disabled:opacity-40">Thanh toán cọc {vnd(deposit)}</button>
            <button type="button" onClick={() => setStep("fail")} className="text-sm text-muted-foreground underline">Mô phỏng lỗi thanh toán</button>
          </div>
        </form>
        <aside className="h-fit space-y-4 rounded-2xl border p-6 shadow-card">
          <img src={car.image} alt={car.name} className="aspect-video w-full rounded-lg object-cover" />
          <p className="font-semibold text-navy">{car.name}</p>
          <div className="space-y-2 border-t pt-4 text-sm">
            <div className="flex justify-between text-muted-foreground"><span>{vnd(car.price)} × {days} ngày</span><span>{vnd(total)}</span></div>
            <div className="flex justify-between font-semibold text-navy"><span>Tổng tiền thuê</span><span>{vnd(total)}</span></div>
            <div className="flex justify-between text-primary font-semibold"><span>Đặt cọc 30%</span><span>{vnd(deposit)}</span></div>
            <div className="flex justify-between text-muted-foreground"><span>Thanh toán khi nhận xe</span><span>{vnd(total - deposit)}</span></div>
          </div>
        </aside>
      </div>
    </SiteLayout>
  );
}

function Box({ t, children }: { t: string; children: React.ReactNode }) {
  return <section className="space-y-4 rounded-xl border p-5"><h2 className="font-semibold text-navy">{t}</h2>{children}</section>;
}
