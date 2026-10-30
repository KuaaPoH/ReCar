import { createFileRoute, Link } from "@tanstack/react-router";
import { PageHeader } from "@/components/host/PageHeader";
import { PartnerProfile } from "@/components/host/PartnerProfile";

export const Route = createFileRoute("/CarOwner/Register/Pending")({
  head: () => ({
    meta: [
      { title: "Hồ sơ pháp lý & Ngân hàng — ReCar Host Pro" },
      { name: "description", content: "Trạng thái xác minh đối tác và tài khoản nhận tiền của chủ xe." },
      { property: "og:title", content: "Hồ sơ pháp lý & Ngân hàng — ReCar Host Pro" },
      { property: "og:description", content: "Hồ sơ đối tác chủ xe ReCar." },
    ],
  }),
  component: () => (
    <>
      <PageHeader title="Hồ sơ pháp lý & Ngân hàng" description="Thông tin xác minh đối tác và tài khoản quyết toán." actions={<Link to="/CarOwner/Register/Rejected" className="text-xs text-muted-foreground underline">Xem mẫu trạng thái bị từ chối</Link>} />
      <PartnerProfile view="pending" />
    </>
  ),
});
