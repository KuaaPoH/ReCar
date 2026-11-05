import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
      { title: "ReCar Host Pro — Quản lý kinh doanh cho chủ xe" },
      { name: "description", content: "Phân hệ quản lý đội xe, đơn thuê và doanh thu dành cho đối tác chủ xe ReCar." },
      { property: "og:title", content: "ReCar Host Pro" },
      { property: "og:description", content: "Quản lý đội xe, đơn thuê và doanh thu dành cho chủ xe ReCar." },
    ],
  }),
  beforeLoad: () => {
    throw redirect({ to: "/CarOwner/Home/Index" });
  },
});
