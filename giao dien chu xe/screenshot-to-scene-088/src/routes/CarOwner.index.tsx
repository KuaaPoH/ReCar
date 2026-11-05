import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/CarOwner/")({
  head: () => ({ meta: [
    { title: "Đối tác chủ xe — ReCar Host Pro" },
    { name: "description", content: "Không gian đối tác chủ xe ReCar: đội xe, đơn thuê và thu nhập." },
    { property: "og:title", content: "Đối tác chủ xe — ReCar Host Pro" },
    { property: "og:description", content: "Quản lý hoạt động cho thuê xe của đối tác ReCar." },
    { property: "og:type", content: "website" },
    { name: "twitter:card", content: "summary_large_image" },
  ] }),
  beforeLoad: () => {
    throw redirect({ to: "/CarOwner/Home/Index" });
  },
});
