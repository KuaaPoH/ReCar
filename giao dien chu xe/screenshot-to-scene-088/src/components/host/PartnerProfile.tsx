import { useState } from "react";
import { toast } from "sonner";
import { Clock, IdCard, Landmark, Loader2, ShieldCheck, XCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useHost } from "@/lib/host/store";
import { formatDateTime, maskTail } from "@/lib/host/format";
import { DEMO_NOTE } from "@/lib/host/api";

export function PartnerProfile({ view }: { view: "pending" | "rejected" }) {
  const { owner, setOwner } = useHost();
  const [bank, setBank] = useState({ bankName: owner.bankName, bankAccount: owner.bankAccount, bankHolder: owner.bankHolder });
  const [err, setErr] = useState<Partial<Record<"bankName" | "bankAccount" | "bankHolder", string>>>({});
  const [busy, setBusy] = useState(false);
  const status = view === "rejected" ? "rejected" : owner.verified;

  const save = async () => {
    const e: Partial<Record<"bankName" | "bankAccount" | "bankHolder", string>> = {};
    if (bank.bankName.trim().length < 2) e.bankName = "Nhập tên ngân hàng.";
    if (!/^\d{6,19}$/.test(bank.bankAccount)) e.bankAccount = "Số tài khoản chỉ gồm 6–19 chữ số.";
    if (!/^[A-Z ]{3,}$/.test(bank.bankHolder)) e.bankHolder = "Viết IN HOA, không dấu, đúng tên chủ tài khoản.";
    setErr(e);
    if (Object.keys(e).length) return;
    setBusy(true);
    await new Promise((r) => setTimeout(r, 700));
    setOwner({ ...owner, ...bank });
    setBusy(false);
    toast.success(view === "rejected" ? "Đã gửi lại hồ sơ — chờ xét duyệt" : "Đã cập nhật thông tin ngân hàng", { description: DEMO_NOTE });
  };

  return (
    <div className="space-y-6">
      {status === "approved" && (
        <div className="flex items-center gap-3 rounded-xl bg-success-soft p-4 text-success"><ShieldCheck /> <p className="text-sm font-semibold">Hồ sơ đối tác đã được duyệt — bạn có thể sử dụng đầy đủ chức năng.</p></div>
      )}
      {status === "pending" && (
        <div className="flex items-start gap-3 rounded-xl bg-warning-soft p-4 text-warning"><Clock className="mt-0.5" /><div className="text-sm"><p className="font-semibold">Hồ sơ đang được xét duyệt</p><p>Gửi lúc {formatDateTime(owner.submittedAt)}. Thông thường mất 1–2 ngày làm việc; bạn sẽ nhận thông báo khi có kết quả.</p></div></div>
      )}
      {status === "rejected" && (
        <div className="flex items-start gap-3 rounded-xl bg-destructive-soft p-4 text-destructive"><XCircle className="mt-0.5" /><div className="text-sm">
          <p className="font-semibold">Hồ sơ bị từ chối</p>
          <p>{owner.rejectReason ?? "Ảnh CCCD mặt sau bị lóa, không đọc được ngày cấp."}</p>
          <ul className="mt-2 list-disc pl-5">{(owner.missingItems ?? ["Chụp lại ảnh CCCD mặt sau", "Bổ sung ảnh GPLX mặt sau"]).map((m) => <li key={m}>{m}</li>)}</ul>
        </div></div>
      )}

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="card-surface p-6">
          <h2 className="mb-4 flex items-center gap-2 font-bold"><IdCard className="size-5 text-primary" /> Hồ sơ định danh</h2>
          <dl className="grid grid-cols-2 gap-4 text-sm">
            <div><dt className="text-xs text-muted-foreground">Họ tên</dt><dd className="font-semibold">{owner.fullName}</dd></div>
            <div><dt className="text-xs text-muted-foreground">Số CCCD</dt><dd className="font-mono">{maskTail(owner.idNumber)}</dd></div>
            <div><dt className="text-xs text-muted-foreground">Số GPLX</dt><dd className="font-mono">{maskTail(owner.licenseNumber)}</dd></div>
            <div><dt className="text-xs text-muted-foreground">Hạng</dt><dd className="font-semibold">{owner.licenseClass}</dd></div>
          </dl>
          <div className="mt-5 grid grid-cols-2 gap-3">
            {["CCCD mặt trước", "CCCD mặt sau", "GPLX mặt trước", "GPLX mặt sau"].map((d) => (
              <div key={d} className="grid h-24 place-items-center rounded-xl border border-dashed bg-muted text-xs text-muted-foreground">{d} · đã tải lên</div>
            ))}
          </div>
          {status === "rejected" && <Button className="mt-4" variant="outline" onClick={() => toast.info("Tải lại giấy tờ", { description: DEMO_NOTE })}>Cập nhật hồ sơ</Button>}
          <p className="mt-3 text-xs text-muted-foreground">Ảnh giấy tờ được bảo vệ, chỉ bạn và Admin xét duyệt xem được.</p>
        </div>

        <div className="card-surface p-6">
          <h2 className="mb-1 flex items-center gap-2 font-bold"><Landmark className="size-5 text-primary" /> Tài khoản nhận tiền</h2>
          <p className="mb-4 text-xs text-muted-foreground">Dùng để quyết toán phần cọc sàn chuyển lại. Không hiển thị công khai.</p>
          <div className="space-y-4">
            {([["bankName", "Tên ngân hàng", "VD: Vietcombank"], ["bankAccount", "Số tài khoản", "Chỉ nhập chữ số"], ["bankHolder", "Chủ tài khoản", "IN HOA KHÔNG DẤU"]] as const).map(([k, l, ph]) => (
              <div key={k} className="space-y-1.5">
                <Label htmlFor={k}>{l}</Label>
                <Input id={k} value={bank[k]} placeholder={ph} className={k !== "bankName" ? "font-mono" : ""}
                  onChange={(e) => setBank({ ...bank, [k]: k === "bankHolder" ? e.target.value.toUpperCase() : k === "bankAccount" ? e.target.value.replace(/\D/g, "") : e.target.value })} />
                {err[k] && <p className="text-xs font-medium text-destructive">{err[k]}</p>}
              </div>
            ))}
            <Button onClick={save} disabled={busy}>{busy && <Loader2 className="animate-spin" />}{status === "rejected" ? "Gửi lại hồ sơ" : "Lưu thông tin"}</Button>
          </div>
        </div>
      </div>
    </div>
  );
}
