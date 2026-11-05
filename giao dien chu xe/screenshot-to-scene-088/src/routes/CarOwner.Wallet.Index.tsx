import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import { Download, FileText, ShieldCheck, ArrowUpRight, Wallet } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { PageHeader, EmptyState } from "@/components/host/PageHeader";
import { Stats, Section, SelectField, tableClass, Notice } from "@/components/host/ExtensionUI";
import { useHost } from "@/lib/host/store";
import { financeBreakdown } from "@/lib/host/rules";
import {
  walletBalances,
  payoutAmount,
  payoutLabels,
  type Payout,
  type PayoutStatus,
} from "@/lib/host/extension";
import { payouts, DEMO_TODAY } from "@/lib/host/mock-data";
import { formatVND, formatDate, maskTail } from "@/lib/host/format";
import { API } from "@/lib/host/api";
export const Route = createFileRoute("/CarOwner/Wallet/Index")({
  head: () => ({
    meta: [
      { title: "Ví đối tác & Đối soát — ReCar Host Pro" },
      {
        name: "description",
        content: "Theo dõi dòng tiền, phí sàn, thu nhập ròng và lịch sử quyết toán của chủ xe.",
      },
      { property: "og:title", content: "Ví đối tác & Đối soát — ReCar Host Pro" },
      {
        property: "og:description",
        content: "Phân biệt tiền thu trực tiếp và khoản sàn còn phải chuyển cho chủ xe.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: WalletPage,
});
const payoutColor: Record<PayoutStatus, string> = {
  pending: "bg-warning-soft text-warning",
  processing: "bg-info-soft text-info",
  paid: "bg-success-soft text-success",
  failed: "bg-destructive-soft text-destructive",
  review: "bg-maintenance-soft text-maintenance",
};
function WalletPage() {
  const { owner, cars, orders } = useHost();
  const [start, setStart] = useState("2026-05-01");
  const [end, setEnd] = useState(DEMO_TODAY);
  const [status, setStatus] = useState("all");
  const [tab, setTab] = useState("transactions");
  const [selected, setSelected] = useState<Payout | null>(null);
  const [transaction, setTransaction] = useState("");
  const completed = orders.filter((o) => o.status === 5);
  const scoped = completed.filter(
    (o) => o.returnAt.slice(0, 10) >= start && o.returnAt.slice(0, 10) <= end,
  );
  const scopedPayouts = payouts
    .map((p) => ({ ...p, orderIds: p.orderIds.filter((id) => completed.some((o) => o.id === id)) }))
    .filter((p) => p.orderIds.length);
  const balance = walletBalances(completed, scopedPayouts);
  const periodGross = scoped.reduce((s, o) => s + o.total, 0);
  const finances = financeBreakdown(periodGross);
  const inPeriod = (date: string) => date >= start && date <= end;
  const payoutFor = (id: string) => scopedPayouts.find((p) => p.orderIds.includes(id));
  const periodPaid = scopedPayouts
    .filter((p) => p.status === "paid" && p.date && inPeriod(p.date))
    .reduce((s, p) => s + payoutAmount(p, completed), 0);
  const shownPayouts = scopedPayouts.filter((p) => {
    const anchor = p.date ?? p.period + "-28";
    return inPeriod(anchor) && (status === "all" || p.status === status);
  });
  const txs = scoped
    .flatMap((o) => {
      const f = financeBreakdown(o.total);
      const p = payoutFor(o.id);
      return [
        {
          id: `TN-${o.id}`,
          orderId: o.id,
          date: o.returnAt.slice(0, 10),
          type: "Thu nhập từ đơn thuê",
          amount: o.total,
          fee: f.platformFee,
          net: f.ownerNet,
          status: p?.status ?? ("pending" as PayoutStatus),
          note: "Ròng gồm 70% khách trả trực tiếp + 20% sàn chuyển; phí đã trừ một lần.",
        },
      ];
    })
    .concat(
      scopedPayouts
        .filter((p) => p.status === "paid" && p.date && inPeriod(p.date))
        .map((p) => ({
          id: `CK-${p.id}`,
          orderId: p.orderIds.join(", "),
          date: p.date ?? "",
          type: "Thanh toán cho chủ xe",
          amount: payoutAmount(p, completed),
          fee: 0,
          net: payoutAmount(p, completed),
          status: p.status,
          note: "Chuyển khoản mẫu; không cộng khoản này vào doanh thu lần nữa.",
        })),
    )
    .filter((t) => status === "all" || t.status === status)
    .sort((a, b) => b.date.localeCompare(a.date));
  const selectedTx = txs.find((t) => t.id === transaction);
  const exportReport = (type: "Excel" | "PDF") =>
    toast.info(`Chưa xuất ${type}`, {
      description: `Chưa kết nối xuất báo cáo kế toán. Điểm tích hợp: ${type === "Excel" ? API.exportExcel : API.exportPdf}`,
    });
  return (
    <div className="space-y-6">
      <PageHeader
        title="Ví đối tác & Đối soát"
        actions={
          <>
            <Button variant="outline" onClick={() => exportReport("Excel")}>
              <Download />
              Xuất Excel
            </Button>
            <Button variant="outline" onClick={() => exportReport("PDF")}>
              <FileText />
              Xuất PDF
            </Button>
          </>
        }
      />
      <div className="flex flex-wrap items-end gap-3">
        <label className="space-y-1 text-xs text-muted-foreground">
          Từ ngày
          <Input
            aria-label="Ví từ ngày"
            type="date"
            value={start}
            onChange={(e) => e.target.value && setStart(e.target.value)}
          />
        </label>
        <label className="space-y-1 text-xs text-muted-foreground">
          Đến ngày
          <Input
            aria-label="Ví đến ngày"
            type="date"
            value={end}
            onChange={(e) => e.target.value && setEnd(e.target.value)}
          />
        </label>
        <SelectField
          label="Trạng thái đối soát"
          value={status}
          onChange={setStatus}
          options={[
            { value: "all", label: "Tất cả trạng thái" },
            ...Object.entries(payoutLabels).map(([value, label]) => ({ value, label })),
          ]}
        />
      </div>
      {start > end ? (
        <p role="alert" className="text-destructive">
          Ngày kết thúc phải sau ngày bắt đầu.
        </p>
      ) : (
        <>
          <Stats
            items={[
              {
                label: "Chờ quyết toán · toàn bộ",
                value: formatVND(balance.pending),
                note: "Khoản 20% từ đơn hoàn tất chưa được chuyển và chưa thuộc đợt đang xử lý. Không gồm tiền khách trả trực tiếp.",
              },
              {
                label: "Doanh thu ròng · trong kỳ",
                value: formatVND(finances.ownerNet),
                note: "Giá trị đơn hoàn tất trong kỳ trừ 10% phí sàn đúng một lần.",
              },
              {
                label: "Đã nhận · trong kỳ (mẫu)",
                value: formatVND(periodPaid),
                note: "Chỉ tính đợt mẫu đã chuyển có ngày chuyển trong kỳ. Không cộng số dư chờ quyết toán.",
              },
              {
                label: "Đang xử lý · toàn bộ",
                value: formatVND(balance.processing),
                note: "Khoản sàn chuyển của các đơn thuộc đợt đang xử lý. Tách khỏi số dư chờ và đã nhận.",
              },
            ]}
          />
          <Notice>
            Số dư và đợt chuyển khoản là minh họa, không phải ví tiền thật. Kỳ báo cáo áp dụng cho
            doanh thu và lịch sử; số dư chờ/đang xử lý thể hiện toàn bộ khoản chưa kết thúc.
          </Notice>
          <Section title="Dòng tiền trong kỳ">
            <div className="grid gap-6 lg:grid-cols-[1fr_300px]">
              <div className="divide-y rounded-lg border bg-card">
                {[
                  { label: "Tổng giá trị thuê xe", amount: finances.total },
                  { label: "Cọc 30% khách thanh toán qua sàn", amount: finances.deposit },
                  {
                    label: "70% khách thanh toán trực tiếp cho chủ xe",
                    amount: finances.remaining,
                  },
                  { label: "Phí sàn 10% (khấu trừ từ cọc)", amount: -finances.platformFee },
                  { label: "Doanh thu ròng của chủ xe", amount: finances.ownerNet },
                  {
                    label: "Phần sàn phải chuyển (30% − 10%)",
                    amount: finances.payoutFromPlatform,
                  },
                ].map((r, i) => (
                  <div
                    key={r.label}
                    className={`flex flex-wrap justify-between gap-2 px-4 py-3 text-sm ${i >= 4 ? "font-semibold text-primary" : ""}`}
                  >
                    <span>{r.label}</span>
                    <span className="whitespace-nowrap">{formatVND(r.amount)}</span>
                  </div>
                ))}
                <p className="px-4 py-3 text-xs text-muted-foreground">
                  Phần sàn phải chuyển là tổng nghĩa vụ trong kỳ, gồm cả khoản đã chuyển; không phải
                  số dư chờ hiện tại.
                </p>
              </div>
              <div className="space-y-3 border-l pl-5">
                <div className="flex items-center gap-2 text-primary">
                  <ShieldCheck className="size-5" />
                  <h3 className="text-sm font-semibold">Tài khoản nhận tiền</h3>
                </div>
                <p className="text-lg font-bold">{owner.bankName || "Chưa cập nhật"}</p>
                <p className="break-all font-mono text-sm">{maskTail(owner.bankAccount)}</p>
                <p className="text-sm">{owner.bankHolder}</p>
                <p className="text-xs text-muted-foreground">
                  Ngân hàng: chưa xác minh riêng trong dữ liệu mẫu.
                </p>
                <p className="text-xs text-success">
                  Hồ sơ đối tác:{" "}
                  {owner.verified === "approved"
                    ? "đã xác minh"
                    : owner.verified === "pending"
                      ? "đang xét duyệt"
                      : "cần bổ sung"}
                </p>
                <Button variant="outline" asChild>
                  <Link to="/CarOwner/Register/Pending">
                    Cập nhật tài khoản
                    <ArrowUpRight />
                  </Link>
                </Button>
              </div>
            </div>
          </Section>
          <Section
            title="Lịch sử tài chính"
            action={
              <Tabs value={tab} onValueChange={setTab}>
                <TabsList>
                  <TabsTrigger value="transactions">Giao dịch</TabsTrigger>
                  <TabsTrigger value="payouts">Quyết toán</TabsTrigger>
                </TabsList>
              </Tabs>
            }
          >
            <div className="overflow-x-auto rounded-lg border">
              {tab === "transactions" ? (
                <>
                  <table className={tableClass}>
                    <thead>
                      <tr>
                        <th>Mã giao dịch / Đơn</th>
                        <th>Ngày</th>
                        <th>Loại</th>
                        <th>Số tiền</th>
                        <th>Phí</th>
                        <th>Ròng</th>
                        <th>Trạng thái</th>
                        <th />
                      </tr>
                    </thead>
                    <tbody>
                      {txs.map((t) => (
                        <tr key={t.id}>
                          <td>
                            <p className="font-semibold">{t.id}</p>
                            <p className="text-xs text-muted-foreground">{t.orderId}</p>
                          </td>
                          <td className="whitespace-nowrap">{formatDate(t.date)}</td>
                          <td>{t.type}</td>
                          <td className="whitespace-nowrap">{formatVND(t.amount)}</td>
                          <td className="whitespace-nowrap">{formatVND(t.fee)}</td>
                          <td className="whitespace-nowrap font-semibold">{formatVND(t.net)}</td>
                          <td>
                            <span
                              className={`whitespace-nowrap rounded px-2 py-1 text-xs ${payoutColor[t.status]}`}
                            >
                              {payoutLabels[t.status]}
                            </span>
                          </td>
                          <td>
                            <Button
                              variant="ghost"
                              size="icon"
                              aria-label={`Chi tiết giao dịch ${t.id}`}
                              onClick={() => setTransaction(t.id)}
                            >
                              <ArrowUpRight />
                            </Button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                  {!txs.length && (
                    <EmptyState icon={<Wallet />} title="Không có giao dịch phù hợp" />
                  )}
                </>
              ) : (
                <>
                  <table className={tableClass}>
                    <thead>
                      <tr>
                        <th>Mã / Kỳ đối soát</th>
                        <th>Ngày chuyển</th>
                        <th>Số tiền sàn chuyển</th>
                        <th>Ngân hàng</th>
                        <th>Tham chiếu</th>
                        <th>Trạng thái</th>
                        <th />
                      </tr>
                    </thead>
                    <tbody>
                      {shownPayouts.map((p) => (
                        <tr key={p.id}>
                          <td>
                            <p className="font-semibold">{p.id}</p>
                            <p className="text-xs text-muted-foreground">
                              {p.period.slice(5)}/{p.period.slice(0, 4)}
                            </p>
                          </td>
                          <td>{p.date ? formatDate(p.date) : "Chưa chuyển"}</td>
                          <td className="whitespace-nowrap font-semibold">
                            {formatVND(payoutAmount(p, completed))}
                          </td>
                          <td>
                            <p>{owner.bankName}</p>
                            <p className="text-xs">{maskTail(owner.bankAccount)}</p>
                          </td>
                          <td>{p.reference ?? "—"}</td>
                          <td>
                            <span
                              className={`whitespace-nowrap rounded px-2 py-1 text-xs ${payoutColor[p.status]}`}
                            >
                              {payoutLabels[p.status]}
                            </span>
                          </td>
                          <td>
                            <Button
                              variant="ghost"
                              size="icon"
                              aria-label={`Chi tiết quyết toán ${p.id}`}
                              onClick={() => setSelected(p)}
                            >
                              <ArrowUpRight />
                            </Button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                  {!shownPayouts.length && (
                    <EmptyState icon={<Wallet />} title="Không có đợt quyết toán phù hợp" />
                  )}
                </>
              )}
            </div>
            <Notice>
              Phí dịch vụ nằm trong từng dòng thu nhập để tránh trừ trùng. Chưa có dữ liệu điều
              chỉnh, hoàn tiền hoặc khấu trừ nên không tạo các giao dịch đó.
            </Notice>
          </Section>
        </>
      )}
      <Dialog open={!!selected} onOpenChange={(o) => !o && setSelected(null)}>
        <DialogContent className="max-h-[85vh] overflow-y-auto sm:max-w-xl">
          <DialogHeader>
            <DialogTitle>Quyết toán {selected?.id}</DialogTitle>
          </DialogHeader>
          {selected && (
            <div className="space-y-4">
              <p className="text-sm">
                {payoutLabels[selected.status]} · {owner.bankName} · {maskTail(owner.bankAccount)}
              </p>
              {completed
                .filter((o) => selected.orderIds.includes(o.id))
                .map((o) => {
                  const f = financeBreakdown(o.total);
                  return (
                    <div key={o.id} className="space-y-2 border-b pb-3 text-sm">
                      <div className="flex justify-between gap-2">
                        <Link
                          className="font-semibold text-primary"
                          to="/CarOwner/Orders/Details/$id"
                          params={{ id: o.id }}
                        >
                          {o.id}
                        </Link>
                        <span>{cars.find((c) => c.id === o.carId)?.name}</span>
                      </div>
                      <p className="flex justify-between">
                        <span>Cọc 30%</span>
                        <span>{formatVND(f.deposit)}</span>
                      </p>
                      <p className="flex justify-between">
                        <span>Phí 10%</span>
                        <span>−{formatVND(f.platformFee)}</span>
                      </p>
                      <p className="flex justify-between font-semibold">
                        <span>Sàn chuyển</span>
                        <span>{formatVND(f.payoutFromPlatform)}</span>
                      </p>
                    </div>
                  );
                })}
              <p className="flex justify-between font-bold">
                <span>Tổng sàn chuyển</span>
                <span className="text-primary">{formatVND(payoutAmount(selected, completed))}</span>
              </p>
              <Notice>Đợt và mã ngân hàng là dữ liệu mẫu; chưa có chuyển khoản thật.</Notice>
            </div>
          )}
        </DialogContent>
      </Dialog>
      <Dialog open={!!selectedTx} onOpenChange={(o) => !o && setTransaction("")}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{selectedTx?.id}</DialogTitle>
          </DialogHeader>
          {selectedTx && (
            <div className="space-y-3 text-sm">
              <p>
                {selectedTx.type} · {formatDate(selectedTx.date)}
              </p>
              <p>Số tiền: {formatVND(selectedTx.amount)}</p>
              <p>Phí: {formatVND(selectedTx.fee)}</p>
              <p className="font-semibold">Ròng: {formatVND(selectedTx.net)}</p>
              <p>{selectedTx.note}</p>
              <span className={`rounded px-2 py-1 text-xs ${payoutColor[selectedTx.status]}`}>
                {payoutLabels[selectedTx.status]}
              </span>
              {selectedTx.type === "Thu nhập từ đơn thuê" && (
                <Button className="mt-3" variant="outline" asChild>
                  <Link to="/CarOwner/Orders/Details/$id" params={{ id: selectedTx.orderId }}>
                    Xem đơn thuê
                    <ArrowUpRight />
                  </Link>
                </Button>
              )}
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}
