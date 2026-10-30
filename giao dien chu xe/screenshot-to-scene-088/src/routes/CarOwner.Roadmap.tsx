import { createFileRoute } from "@tanstack/react-router";
import { BarChart3, CalendarDays, ClipboardCheck, MessagesSquare, Star, Tags, Wallet } from "lucide-react";
import { PageHeader } from "@/components/host/PageHeader";

export const Route = createFileRoute("/CarOwner/Roadmap")({
  head: () => ({
    meta: [
      { title: "Tính năng tương lai — ReCar Host Pro" },
      { name: "description", content: "Lộ trình các tính năng mở rộng dành cho chủ xe ReCar." },
      { property: "og:title", content: "Tính năng tương lai — ReCar Host Pro" },
      { property: "og:description", content: "Lộ trình phát triển ReCar Host Pro." },
    ],
  }),
  component: Roadmap,
});

const items = [
  [CalendarDays, "Lịch xe trực quan", "/CarOwner/Calendar", "Lịch tháng/tuần theo từng xe; chặn lịch thủ công khi backend kiểm tra xung đột."],
  [ClipboardCheck, "Biên bản bàn giao điện tử", "/CarOwner/Orders/Inspection/{id}", "Ảnh 4 góc, ODO, nhiên liệu/pin; đối chiếu km (VD: vượt 30 km × 3.000đ = 90.000đ)."],
  [Tags, "Giá thông minh", "/CarOwner/Pricing", "Giá cuối tuần, lễ Tết, theo mùa; giảm giá thuê dài ngày."],
  [MessagesSquare, "Trung tâm tin nhắn", "/CarOwner/Messages", "Trò chuyện với khách theo đơn thuê."],
  [Wallet, "Ví & đối soát", "/CarOwner/Wallet", "Số dư quyết toán, lịch sử giải ngân, phí dịch vụ."],
  [BarChart3, "Phân tích hiệu suất xe", "/CarOwner/Analytics", "Tỷ lệ lấp đầy, doanh thu và số chuyến theo xe."],
  [Star, "Đánh giá khách thuê", "/CarOwner/Orders/ReviewGuest/{id}", "Chấm sao đúng giờ, vệ sinh, thái độ cho đơn đã hoàn tất."],
] as const;

function Roadmap() {
  return (
    <>
      <PageHeader title="Tính năng tương lai" description="Các tính năng đang được thiết kế — chưa kết nối hệ thống và chưa hoạt động." />
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
        {items.map(([Icon, t, path, d]) => (
          <div key={t} className="card-surface p-5">
            <div className="flex items-center justify-between">
              <span className="grid size-10 place-items-center rounded-xl bg-primary-soft text-primary"><Icon className="size-5" /></span>
              <span className="rounded-full bg-muted px-2.5 py-1 text-xs font-semibold text-muted-foreground">Sắp ra mắt</span>
            </div>
            <h3 className="mt-4 font-bold">{t}</h3>
            <p className="mt-1 text-sm text-muted-foreground">{d}</p>
            <p className="mt-3 font-mono text-xs text-muted-foreground">{path}</p>
          </div>
        ))}
      </div>
    </>
  );
}
