import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { toast } from "sonner";
import { CarFront } from "lucide-react";
import { Button } from "@/components/ui/button";
import { CarForm } from "@/components/host/CarForm";
import { EmptyState, PageHeader } from "@/components/host/PageHeader";
import { useHost } from "@/lib/host/store";
import { DEMO_NOTE } from "@/lib/host/api";

export const Route = createFileRoute("/CarOwner/Cars/Edit/$id")({
  head: () => ({
    meta: [
      { title: "Chỉnh sửa xe — ReCar Host Pro" },
      { name: "description", content: "Cập nhật thông tin, giá, quy định và hình ảnh của xe." },
      { property: "og:title", content: "Chỉnh sửa xe — ReCar Host Pro" },
      { property: "og:description", content: "Cập nhật hồ sơ xe trên ReCar." },
    ],
  }),
  component: EditCar,
});

// Fields that require Admin re-approval when changed
const SENSITIVE = ["name", "brand", "year", "plate", "seats", "image"] as const;

function EditCar() {
  const { id } = Route.useParams();
  const { cars, updateCar } = useHost();
  const nav = useNavigate();
  const car = cars.find((c) => String(c.id) === id);
  if (!car)
    return <div className="card-surface"><EmptyState icon={<CarFront />} title="Không tìm thấy xe" body="Xe không tồn tại hoặc không thuộc quyền quản lý của bạn." action={<Button asChild><Link to="/CarOwner/Cars/Index">Về danh sách xe</Link></Button>} /></div>;

  return (
    <>
      <PageHeader title={`Chỉnh sửa: ${car.name}`} description="Thay đổi thông tin định danh hoặc ảnh sẽ cần Admin duyệt lại." />
      <CarForm
        initial={{ ...car, gallery: [], docs: { registration: car.image, inspection: car.image, insurance: car.image } }}
        rejectReason={car.status === "rejected" ? car.rejectReason : undefined}
        submitLabel={car.status === "rejected" ? "Gửi lại hồ sơ" : "Lưu thay đổi"}
        onSubmit={({ docs: _d, gallery: _g, ...v }) => {
          const needsReview = car.status === "rejected" || SENSITIVE.some((k) => v[k] !== car[k]);
          updateCar(car.id, { ...v, ...(needsReview ? { status: "pending" as const, rejectReason: undefined } : {}) });
          toast.success(needsReview ? "Đã gửi lại — xe chuyển sang Chờ duyệt" : "Đã lưu thay đổi", { description: DEMO_NOTE });
          nav({ to: "/CarOwner/Cars/Details/$id", params: { id } });
        }}
      />
    </>
  );
}
