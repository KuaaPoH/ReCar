import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { Mail, MapPin, Phone } from "lucide-react";
import { PageTitle, SiteLayout } from "@/components/site/Layout";

export const Route = createFileRoute("/contact")({
  head: () => ({ meta: [
    { title: "Liên hệ | ReCar" },
    { name: "description", content: "Liên hệ đội ngũ hỗ trợ ReCar về thuê xe và hợp tác chủ xe." },
    { property: "og:title", content: "Liên hệ | ReCar" },
    { property: "og:description", content: "Gửi câu hỏi cho ReCar." },
  ] }),
  component: Contact,
});

const inp = "w-full rounded-lg border px-3 py-2.5 text-sm";

function Contact() {
  const [ok, setOk] = useState(false);
  return (
    <SiteLayout>
      <PageTitle title="Liên hệ" sub="Chúng tôi luôn sẵn sàng hỗ trợ bạn." />
      <div className="mx-auto grid max-w-6xl gap-10 px-4 py-10 md:grid-cols-[1fr_1.4fr]">
        <div className="space-y-4">
          {[{ i: Phone, t: "1900 0000" }, { i: Mail, t: "hotro@recar.example" }, { i: MapPin, t: "Địa chỉ văn phòng (cập nhật sau)" }].map(({ i: I, t }) => (
            <p key={t} className="flex items-center gap-3 rounded-xl bg-secondary p-4"><I className="h-5 w-5 text-primary" />{t}</p>
          ))}
          <p className="text-xs text-warning">Thông tin liên hệ là dữ liệu mẫu, cần thay bằng thông tin chính thức.</p>
        </div>
        {ok ? <p className="rounded-xl border bg-success/10 p-6 font-medium text-success">Cảm ơn bạn! Chúng tôi sẽ phản hồi sớm.</p> : (
          <form className="grid gap-4" onSubmit={(e) => { e.preventDefault(); setOk(true); }}>
            <input required className={inp} placeholder="Họ và tên" />
            <input required type="email" className={inp} placeholder="Email" />
            <textarea required rows={5} className={inp} placeholder="Nội dung" />
            <button className="w-fit rounded-lg bg-primary px-6 py-3 font-semibold text-primary-foreground">Gửi liên hệ</button>
          </form>
        )}
      </div>
    </SiteLayout>
  );
}
