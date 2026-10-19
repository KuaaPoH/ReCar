export const seo = (title: string, description: string) => ({
  meta: [
    { title: `${title} · ReCar Admin` },
    { name: "description", content: description },
    { property: "og:title", content: `${title} · ReCar Admin` },
    { property: "og:description", content: description },
    { property: "og:type", content: "website" },
    { name: "twitter:card", content: "summary" },
  ],
});
