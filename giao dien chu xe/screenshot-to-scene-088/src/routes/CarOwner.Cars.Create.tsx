import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { toast } from "sonner";
import { CarForm, emptyForm } from "@/components/host/CarForm";
import { PageHeader } from "@/components/host/PageHeader";
import { useHost } from "@/lib/host/store";
import { DEMO_NOTE } from "@/lib/host/api";
import { DEMO_TODAY } from "@/lib/host/mock-data";

export const Route = createFileRoute("/CarOwner/Cars/Create")({
  head: () => ({
    meta: [
      { title: "Thêm xe mới — ReCar Host Pro" },
      { name: "description", content: "Đăng ký xe mới vào đội xe và gửi hồ sơ phê duyệt." },
      { property: "og:title", content: "Thêm xe mới — ReCar Host Pro" },
      { property: "og:description", content: "Đăng ký xe cho thuê trên ReCar." },
    ],
  }),
  component: CreateCar,
});

function CreateCar() {
  const { addCar } = useHost();
  const nav = useNavigate();
  return (
    <>
      <PageHeader title="Đăng ký xe mới" description="Hoàn thành 6 bước để gửi hồ sơ xe cho Admin phê duyệt." />
      <CarForm
        initial={emptyForm()}
        submitLabel="Gửi hồ sơ phê duyệt"
        onSubmit={({ docs: _d, gallery: _g, ...v }) => {
          const id = addCar({ ...v, completedTrips: 0, status: "pending", createdAt: DEMO_TODAY, hasTransactions: false });
          toast.success("Đã gửi hồ sơ xe — trạng thái Chờ duyệt", { description: DEMO_NOTE });
          nav({ to: "/CarOwner/Cars/Details/$id", params: { id: String(id) } });
        }}
      />
    </>
  );
}
