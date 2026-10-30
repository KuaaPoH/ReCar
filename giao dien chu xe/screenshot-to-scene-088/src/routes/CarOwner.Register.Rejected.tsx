import { createFileRoute } from "@tanstack/react-router";
import { PageHeader } from "@/components/host/PageHeader";
import { PartnerProfile } from "@/components/host/PartnerProfile";

export const Route = createFileRoute("/CarOwner/Register/Rejected")({
  head: () => ({
    meta: [
      { title: "Hồ sơ cần bổ sung — ReCar Host Pro" },
      { name: "description", content: "Lý do từ chối và thông tin cần bổ sung cho hồ sơ đối tác." },
      { property: "og:title", content: "Hồ sơ cần bổ sung — ReCar Host Pro" },
      { property: "og:description", content: "Cập nhật hồ sơ đối tác chủ xe ReCar." },
    ],
  }),
  component: () => (
    <>
      <PageHeader title="Hồ sơ cần bổ sung" description="Cập nhật thông tin theo yêu cầu và gửi lại để xét duyệt." />
      <PartnerProfile view="rejected" />
    </>
  ),
});
