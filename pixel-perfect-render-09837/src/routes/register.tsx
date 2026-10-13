import { createFileRoute } from "@tanstack/react-router";
import { AuthForm } from "@/components/site/AuthForm";

export const Route = createFileRoute("/register")({
  head: () => ({ meta: [
    { title: "Đăng ký | ReCar" },
    { name: "description", content: "Đăng ký tài khoản ReCar để thuê xe và quản lý đơn thuê." },
    { property: "og:title", content: "Đăng ký | ReCar" },
    { property: "og:description", content: "Đăng ký tài khoản ReCar." },
  ] }),
  component: () => <AuthForm mode="register" />,
});
