import { createFileRoute } from "@tanstack/react-router";
import { AuthForm } from "@/components/site/AuthForm";

export const Route = createFileRoute("/login")({
  head: () => ({ meta: [
    { title: "Đăng nhập | ReCar" },
    { name: "description", content: "Đăng nhập tài khoản ReCar để thuê xe và quản lý đơn thuê." },
    { property: "og:title", content: "Đăng nhập | ReCar" },
    { property: "og:description", content: "Đăng nhập tài khoản ReCar." },
  ] }),
  component: () => <AuthForm mode="login" />,
});
