import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/")({
  beforeLoad: () => {
    throw redirect({ to: "/admin" });
  },
  head: () => ({
    meta: [
      { title: "ReCar Admin Portal" },
      { name: "description", content: "Khu vực quản trị sàn cho thuê xe tự lái ReCar." },
      { property: "og:title", content: "ReCar Admin Portal" },
      { property: "og:description", content: "Khu vực quản trị sàn cho thuê xe tự lái ReCar." },
    ],
  }),
});
