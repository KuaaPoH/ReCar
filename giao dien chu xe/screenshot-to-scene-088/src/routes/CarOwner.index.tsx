import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/CarOwner/")({
  beforeLoad: () => {
    throw redirect({ to: "/CarOwner/Home/Index" });
  },
});
