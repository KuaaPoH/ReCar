const vnd = new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND", maximumFractionDigits: 0 });
export const formatVND = (n: number) => vnd.format(n);
export const formatShortVND = (n: number) =>
  n >= 1_000_000 ? `${(n / 1_000_000).toFixed(1).replace(".0", "")} tr` : `${Math.round(n / 1000)}k`;

const pad = (n: number) => String(n).padStart(2, "0");
/** Deterministic (no locale/timezone dependence) to avoid hydration mismatch. */
export function formatDateTime(iso: string) {
  const [d, t] = iso.split("T");
  const [y, m, day] = d.split("-");
  return `${day}/${m}/${y}${t ? " " + t.slice(0, 5) : ""}`;
}
export function formatDate(iso: string) {
  return formatDateTime(iso.split("T")[0]);
}
export const maskTail = (s: string, keep = 4) => "•".repeat(Math.max(0, s.length - keep)) + s.slice(-keep);
export const initials = (name: string) =>
  name.split(" ").filter(Boolean).slice(-2).map((p) => p[0]).join("").toUpperCase();
export { pad };
