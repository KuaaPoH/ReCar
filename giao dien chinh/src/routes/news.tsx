import { createFileRoute } from "@tanstack/react-router";
import { PageTitle, SiteLayout } from "@/components/site/Layout";
import { posts } from "@/lib/data";

export const Route = createFileRoute("/news")({
  head: () => ({ meta: [
    { title: "Tin tức | ReCar" },
    { name: "description", content: "Tin tức thuê xe, lái xe an toàn và điểm đến du lịch từ ReCar." },
    { property: "og:title", content: "Tin tức | ReCar" },
    { property: "og:description", content: "Tin tức từ ReCar." },
  ] }),
  component: Page,
});

function Page() {
  return (
    <SiteLayout>
      <PageTitle title="Tin tức" sub="Kinh nghiệm thuê xe, lái xe an toàn và gợi ý hành trình." />
      <div className="mx-auto grid max-w-7xl gap-6 px-4 py-10 md:grid-cols-3">
        {[...posts, ...posts].map((p, i) => (
          <article key={i} className="overflow-hidden rounded-xl border bg-card shadow-card">
            <img src={p.image} alt={p.title} loading="lazy" className="aspect-video w-full object-cover" />
            <div className="p-5"><p className="text-xs text-muted-foreground">{p.cat} · {p.date}</p><h2 className="mt-2 font-semibold text-navy">{p.title}</h2><p className="mt-2 text-sm text-muted-foreground">{p.excerpt}</p><p className="mt-3 text-sm font-medium text-primary">Đọc thêm →</p></div>
          </article>
        ))}
      </div>
    </SiteLayout>
  );
}
