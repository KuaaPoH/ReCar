import { createContext, useContext, useState } from "react";
import { toast } from "sonner";
import { Check, ImagePlus, Loader2, X, Map, Bluetooth, Camera, Gauge, SunMedium, Snowflake, Video, Usb, Baby, ShieldAlert } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { cn } from "@/lib/utils";
import { formatVND } from "@/lib/host/format";
import type { Car } from "@/lib/host/types";

// Amenity catalog — production: loaded from system catalog table.
export const AMENITIES = [
  ["Bản đồ", Map], ["Bluetooth", Bluetooth], ["Camera lùi", Camera], ["Cảm biến lốp", Gauge],
  ["Cửa sổ trời", SunMedium], ["Điều hòa", Snowflake], ["Camera hành trình", Video], ["Cổng sạc USB", Usb],
  ["Ghế trẻ em", Baby], ["Cảnh báo va chạm", ShieldAlert],
] as const;

export type CarFormValues = Omit<Car, "id" | "ownerId" | "completedTrips" | "status" | "createdAt" | "hasTransactions" | "rejectReason"> & {
  docs: Record<string, string | null>;
  gallery: string[];
};

const DOCS = [["registration", "Đăng ký xe / cà vẹt"], ["inspection", "Sổ đăng kiểm"], ["insurance", "Bảo hiểm xe"]] as const;
const STEPS = ["Thông tin cơ bản", "Giá thuê", "Quy định", "Tiện nghi", "Hình ảnh & giấy tờ", "Kiểm tra & gửi"];
const PLATE_RE = /^\d{2}[A-Z]{1,2}-\d{3}\.\d{2}$/;
const ACCEPT = ["image/jpeg", "image/png", "image/webp"];

const ErrCtx = createContext<Record<string, string | undefined>>({});
function F({ id, label, children }: { id: string; label: string; children: React.ReactNode }) {
  const errors = useContext(ErrCtx);
  return (
    <div className="space-y-1.5">
      <Label htmlFor={id}>{label}</Label>
      {children}
      {errors[id] && <p className="text-xs font-medium text-destructive">{errors[id]}</p>}
    </div>
  );
}

export function emptyForm(): CarFormValues {
  return {
    name: "", brand: "", segment: "", year: 2024, plate: "", color: "", seats: 5, transmission: "Tự động", fuel: "Xăng", consumption: "",
    pricePerDay: 0, salePrice: null, saleEnabled: false, location: "", amenities: [], kmLimitPerDay: 300, overKmFee: 3000,
    delivery: false, deliveryFee: 0, image: "", gallery: [], docs: { registration: null, inspection: null, insurance: null },
  };
}

type Errs = Partial<Record<keyof CarFormValues | "registration" | "inspection" | "insurance", string>>;
export function validateStep(step: number, v: CarFormValues): Errs {
  const e: Errs = {};
  if (step === 0) {
    if (v.name.trim().length < 3) e.name = "Nhập tên xe (tối thiểu 3 ký tự).";
    if (!v.brand) e.brand = "Chọn hãng xe.";
    if (!v.segment) e.segment = "Chọn phân khúc.";
    if (v.year < 2000 || v.year > 2027) e.year = "Năm sản xuất từ 2000 đến 2027.";
    if (!PLATE_RE.test(v.plate.trim().toUpperCase())) e.plate = "Biển số theo dạng 51K-238.45.";
    if (!v.color.trim()) e.color = "Nhập màu xe.";
    if (v.seats < 2 || v.seats > 16) e.seats = "Số chỗ từ 2 đến 16.";
    if (!v.location.trim()) e.location = "Nhập địa điểm nhận xe.";
  }
  if (step === 1) {
    if (!(v.pricePerDay > 0)) e.pricePerDay = "Giá thuê/ngày phải lớn hơn 0.";
    if (v.salePrice !== null && (v.salePrice < 0 || v.salePrice >= v.pricePerDay)) e.salePrice = "Giá khuyến mãi phải ≥ 0 và thấp hơn giá niêm yết.";
  }
  if (step === 2) {
    if (v.kmLimitPerDay < 0) e.kmLimitPerDay = "Không được âm.";
    if (v.overKmFee < 0) e.overKmFee = "Không được âm.";
    if (v.delivery && v.deliveryFee < 0) e.deliveryFee = "Không được âm.";
  }
  if (step === 4) {
    if (!v.image) e.image = "Cần ảnh đại diện.";
    DOCS.forEach(([k, l]) => { if (!v.docs[k]) e[k as keyof Errs] = `Thiếu ảnh ${l.toLowerCase()}.`; });
  }
  return e;
}

export function CarForm({ initial, submitLabel, onSubmit, rejectReason }: { initial: CarFormValues; submitLabel: string; onSubmit: (v: CarFormValues) => void; rejectReason?: string | undefined }) {
  const [v, setV] = useState(initial);
  const [step, setStep] = useState(0);
  const [errors, setErrors] = useState<Errs>({});
  const [busy, setBusy] = useState(false);
  const set = <K extends keyof CarFormValues>(k: K, val: CarFormValues[K]) => setV((s) => ({ ...s, [k]: val }));

  const next = () => {
    const e = validateStep(step, v);
    setErrors(e);
    if (Object.keys(e).length) return void toast.error("Vui lòng kiểm tra lại thông tin", { description: Object.values(e)[0] });
    setStep((s) => Math.min(s + 1, STEPS.length - 1));
  };
  const pick = (files: FileList | null, cb: (urls: string[]) => void) => {
    if (!files) return;
    const ok = [...files].filter((f) => ACCEPT.includes(f.type) && f.size <= 5 * 1024 * 1024);
    if (ok.length < files.length) toast.error("Một số tệp không hợp lệ", { description: "Chỉ chấp nhận JPG, PNG, WEBP tối đa 5MB." });
    cb(ok.map((f) => URL.createObjectURL(f)));
  };
  const submit = async () => {
    for (let s = 0; s < 5; s++) {
      const e = validateStep(s, v);
      if (Object.keys(e).length) { setErrors(e); setStep(s); return void toast.error("Hồ sơ chưa đầy đủ", { description: Object.values(e)[0] }); }
    }
    setBusy(true);
    await new Promise((r) => setTimeout(r, 900));
    setBusy(false);
    onSubmit({ ...v, plate: v.plate.toUpperCase() });
  };

  const num = (k: keyof CarFormValues) => ({
    id: k, type: "number", min: 0, value: (v[k] as number | null) ?? "",
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => set(k, (e.target.value === "" ? (k === "salePrice" ? null : 0) : Number(e.target.value)) as never),
    "aria-invalid": !!errors[k as keyof Errs],
  });

  return (
    <ErrCtx.Provider value={errors as Record<string, string | undefined>}>
    <div className="grid gap-6 lg:grid-cols-[240px_1fr]">
      <ol className="card-surface h-fit space-y-1 p-3">
        {STEPS.map((s, i) => (
          <li key={s}>
            <button onClick={() => i < step && setStep(i)} className={cn("flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-left text-sm font-medium", i === step ? "bg-primary-soft text-accent-foreground" : i < step ? "hover:bg-muted" : "text-muted-foreground")}>
              <span className={cn("grid size-6 shrink-0 place-items-center rounded-full text-xs font-bold", i < step ? "bg-success text-primary-foreground" : i === step ? "bg-primary text-primary-foreground" : "bg-muted")}>{i < step ? <Check className="size-3.5" /> : i + 1}</span>
              {s}
            </button>
          </li>
        ))}
      </ol>

      <div className="card-surface p-6">
        {rejectReason && (
          <div className="mb-5 rounded-xl bg-destructive-soft p-4 text-sm text-destructive">
            <p className="font-semibold">Hồ sơ bị từ chối</p><p>{rejectReason}</p>
            <p className="mt-1 text-xs">Sau khi gửi lại, xe chuyển về trạng thái Chờ duyệt.</p>
          </div>
        )}
        <h2 className="mb-5 text-lg font-bold">Bước {step + 1}: {STEPS[step]}</h2>

        {step === 0 && (
          <div className="grid gap-4 sm:grid-cols-2">
            <F id="name" label="Tên xe"><Input id="name" value={v.name} onChange={(e) => set("name", e.target.value)} placeholder="VD: Toyota Vios G 2023" /></F>
            <F id="brand" label="Hãng xe">
              <Select value={v.brand} onValueChange={(x) => set("brand", x)}>
                <SelectTrigger id="brand"><SelectValue placeholder="Chọn hãng" /></SelectTrigger>
                <SelectContent>{["Toyota", "VinFast", "Mitsubishi", "Mazda", "Honda", "Hyundai", "Kia", "Ford"].map((b) => <SelectItem key={b} value={b}>{b}</SelectItem>)}</SelectContent>
              </Select>
            </F>
            <F id="segment" label="Phân khúc">
              <Select value={v.segment} onValueChange={(x) => set("segment", x)}>
                <SelectTrigger id="segment"><SelectValue placeholder="Chọn phân khúc" /></SelectTrigger>
                <SelectContent>{["Sedan hạng A", "Sedan hạng B", "Sedan hạng C", "SUV hạng C", "SUV hạng D", "MPV 7 chỗ", "Bán tải"].map((b) => <SelectItem key={b} value={b}>{b}</SelectItem>)}</SelectContent>
              </Select>
            </F>
            <F id="year" label="Năm sản xuất"><Input {...num("year")} /></F>
            <F id="plate" label="Biển số"><Input id="plate" value={v.plate} onChange={(e) => set("plate", e.target.value.toUpperCase())} placeholder="51K-238.45" className="font-mono" /></F>
            <F id="color" label="Màu xe"><Input id="color" value={v.color} onChange={(e) => set("color", e.target.value)} /></F>
            <F id="seats" label="Số chỗ ngồi"><Input {...num("seats")} /></F>
            <F id="transmission" label="Hộp số">
              <Select value={v.transmission} onValueChange={(x) => set("transmission", x as Car["transmission"])}>
                <SelectTrigger id="transmission"><SelectValue /></SelectTrigger>
                <SelectContent><SelectItem value="Tự động">Tự động</SelectItem><SelectItem value="Số sàn">Số sàn</SelectItem></SelectContent>
              </Select>
            </F>
            <F id="fuel" label="Nhiên liệu">
              <Select value={v.fuel} onValueChange={(x) => set("fuel", x as Car["fuel"])}>
                <SelectTrigger id="fuel"><SelectValue /></SelectTrigger>
                <SelectContent>{["Xăng", "Dầu", "Điện", "Hybrid"].map((b) => <SelectItem key={b} value={b}>{b}</SelectItem>)}</SelectContent>
              </Select>
            </F>
            <F id="consumption" label={v.fuel === "Điện" ? "Điện năng tiêu thụ" : "Mức tiêu thụ nhiên liệu"}><Input id="consumption" value={v.consumption} onChange={(e) => set("consumption", e.target.value)} placeholder={v.fuel === "Điện" ? "18 kWh/100km" : "6L/100km"} /></F>
            <div className="sm:col-span-2"><F id="location" label="Địa điểm nhận xe"><Input id="location" value={v.location} onChange={(e) => set("location", e.target.value)} placeholder="Quận 7, TP.HCM" /></F></div>
          </div>
        )}

        {step === 1 && (
          <div className="grid gap-4 sm:grid-cols-2">
            <F id="pricePerDay" label="Giá thuê/ngày (VND)"><Input {...num("pricePerDay")} step={10000} /></F>
            <F id="salePrice" label="Giá khuyến mãi/ngày (không bắt buộc)"><Input {...num("salePrice")} step={10000} /></F>
            <div className="rounded-xl border bg-muted p-4 sm:col-span-2">
              <p className="text-xs text-muted-foreground">Khách sẽ thấy</p>
              {v.salePrice ? (
                <p className="text-xl font-bold text-primary">{formatVND(v.salePrice)} <span className="text-sm font-normal text-muted-foreground line-through">{formatVND(v.pricePerDay)}</span></p>
              ) : <p className="text-xl font-bold">{formatVND(v.pricePerDay)}</p>}
              <p className="text-xs text-muted-foreground">/ngày · giá khuyến mãi chỉ áp dụng khi bạn bật trong Quản lý xe</p>
            </div>
          </div>
        )}

        {step === 2 && (
          <div className="grid gap-4 sm:grid-cols-2">
            <F id="kmLimitPerDay" label="Giới hạn quãng đường (km/ngày)"><Input {...num("kmLimitPerDay")} /></F>
            <F id="overKmFee" label="Phụ phí vượt km (VND/km)"><Input {...num("overKmFee")} step={500} /></F>
            <label className="flex items-center justify-between rounded-xl border p-4 sm:col-span-2">
              <span><span className="block font-semibold">Giao xe tận nơi</span><span className="text-xs text-muted-foreground">Tăng khả năng được chọn của xe</span></span>
              <Switch checked={v.delivery} onCheckedChange={(x) => set("delivery", x)} />
            </label>
            {v.delivery && <F id="deliveryFee" label="Phí giao xe (VND)"><Input {...num("deliveryFee")} step={10000} /></F>}
          </div>
        )}

        {step === 3 && (
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 xl:grid-cols-5">
            {AMENITIES.map(([name, Icon]) => {
              const on = v.amenities.includes(name);
              return (
                <button key={name} type="button" onClick={() => set("amenities", on ? v.amenities.filter((a) => a !== name) : [...v.amenities, name])}
                  className={cn("flex flex-col items-center gap-2 rounded-xl border p-4 text-sm font-medium transition", on ? "border-primary bg-primary-soft text-accent-foreground" : "hover:bg-muted")} aria-pressed={on}>
                  <Icon className="size-5" />{name}
                </button>
              );
            })}
          </div>
        )}

        {step === 4 && (
          <div className="space-y-6">
            <div>
              <Label>Ảnh đại diện</Label>
              <div className="mt-2 flex items-center gap-4">
                {v.image ? <img src={v.image} alt="" className="h-28 w-44 rounded-xl object-cover" /> : <div className="grid h-28 w-44 place-items-center rounded-xl border border-dashed text-muted-foreground"><ImagePlus /></div>}
                <label className="cursor-pointer"><span className="inline-flex h-9 items-center rounded-md border px-3 text-sm font-medium hover:bg-muted">{v.image ? "Thay ảnh" : "Chọn ảnh"}</span>
                  <input type="file" accept={ACCEPT.join(",")} className="hidden" onChange={(e) => pick(e.target.files, (u) => u[0] && set("image", u[0]))} /></label>
              </div>
              {errors.image && <p className="mt-1 text-xs font-medium text-destructive">{errors.image}</p>}
            </div>
            <div>
              <Label>Bộ sưu tập (ngoại thất, nội thất)</Label>
              <div className="mt-2 flex flex-wrap gap-3">
                {v.gallery.map((g, i) => (
                  <div key={g} className="relative">
                    <img src={g} alt="" className="h-20 w-28 rounded-lg object-cover" />
                    <button onClick={() => set("gallery", v.gallery.filter((_, j) => j !== i))} className="absolute -right-2 -top-2 grid size-6 place-items-center rounded-full bg-destructive text-destructive-foreground" aria-label="Xóa ảnh"><X className="size-3.5" /></button>
                  </div>
                ))}
                <label className="grid h-20 w-28 cursor-pointer place-items-center rounded-lg border border-dashed text-muted-foreground hover:bg-muted"><ImagePlus />
                  <input type="file" multiple accept={ACCEPT.join(",")} className="hidden" onChange={(e) => pick(e.target.files, (u) => set("gallery", [...v.gallery, ...u]))} /></label>
              </div>
            </div>
            <div className="grid gap-3 sm:grid-cols-3">
              {DOCS.map(([k, l]) => (
                <div key={k}>
                  <label className={cn("flex h-32 cursor-pointer flex-col items-center justify-center gap-1 overflow-hidden rounded-xl border border-dashed text-sm text-muted-foreground hover:bg-muted", errors[k as keyof Errs] && "border-destructive")}>
                    {v.docs[k] ? <img src={v.docs[k]!} alt="" className="size-full object-cover" /> : <><ImagePlus />{l}</>}
                    <input type="file" accept={ACCEPT.join(",")} className="hidden" onChange={(e) => pick(e.target.files, (u) => u[0] && set("docs", { ...v.docs, [k]: u[0] }))} />
                  </label>
                  {errors[k as keyof Errs] && <p className="mt-1 text-xs font-medium text-destructive">{errors[k as keyof Errs]}</p>}
                </div>
              ))}
            </div>
            <p className="text-xs text-muted-foreground">Ảnh giấy tờ chỉ Admin xét duyệt mới xem được, không hiển thị công khai.</p>
          </div>
        )}

        {step === 5 && (
          <div className="space-y-4">
            <div className="flex flex-col gap-4 rounded-xl border p-4 sm:flex-row">
              {v.image && <img src={v.image} alt="" className="h-32 w-52 rounded-lg object-cover" />}
              <div className="space-y-1 text-sm">
                <p className="text-lg font-bold">{v.name}</p>
                <p className="text-muted-foreground">{v.brand} · {v.segment} · {v.year} · {v.color}</p>
                <span className="plate">{v.plate}</span>
                <p>{v.seats} chỗ · {v.transmission} · {v.fuel} {v.consumption && `· ${v.consumption}`}</p>
                <p className="font-semibold">{formatVND(v.pricePerDay)}/ngày {v.salePrice ? `(KM ${formatVND(v.salePrice)})` : ""}</p>
                <p>Giới hạn {v.kmLimitPerDay} km/ngày · vượt {formatVND(v.overKmFee)}/km · {v.delivery ? `Giao xe ${formatVND(v.deliveryFee)}` : "Không giao xe"}</p>
                <p className="text-muted-foreground">Tiện nghi: {v.amenities.join(", ") || "—"}</p>
              </div>
            </div>
            <p className="rounded-xl bg-warning-soft p-4 text-sm text-warning">Sau khi gửi, xe ở trạng thái <b>Chờ duyệt</b> và chỉ hiển thị trên sàn khi Admin phê duyệt.</p>
          </div>
        )}

        <div className="mt-8 flex flex-wrap justify-between gap-2 border-t pt-5">
          <Button variant="outline" onClick={() => setStep((s) => Math.max(0, s - 1))} disabled={step === 0}>Quay lại</Button>
          <div className="flex gap-2">
            <Button variant="ghost" onClick={() => toast.info("Lưu bản nháp", { description: "Chỉ khả dụng khi backend hỗ trợ bản nháp." })}>Lưu bản nháp</Button>
            {step < STEPS.length - 1 ? <Button onClick={next}>Tiếp tục</Button> : <Button onClick={submit} disabled={busy}>{busy && <Loader2 className="animate-spin" />}{submitLabel}</Button>}
          </div>
        </div>
      </div>
    </div>
    </ErrCtx.Provider>
  );
}
