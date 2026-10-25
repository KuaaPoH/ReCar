export const vnd = (n: number) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND", maximumFractionDigits: 0 }).format(n);

export const vndShort = (n: number) => {
  if (n >= 1e9) return `${(n / 1e9).toFixed(2).replace(".", ",")} tỷ`;
  if (n >= 1e6) return `${(n / 1e6).toFixed(1).replace(".", ",")} tr`;
  return vnd(n);
};

export const dateVN = (iso: string) => new Date(iso).toLocaleDateString("vi-VN");
export const dateTimeVN = (iso: string) =>
  new Date(iso).toLocaleString("vi-VN", { hour: "2-digit", minute: "2-digit", day: "2-digit", month: "2-digit", year: "numeric" });

export const daysBetween = (a: string, b: string) =>
  Math.max(1, Math.round((new Date(b).getTime() - new Date(a).getTime()) / 86400000));

/** Deposit / remaining split. Rates come from settings (default 30%). */
export const splitDeposit = (total: number, depositPct = 30) => {
  const deposit = Math.round((total * depositPct) / 100);
  return { deposit, remaining: total - deposit };
};

/** Platform commission & owner payout (default 10%). */
export const payout = (gross: number, commissionPct = 10, adjustment = 0) => {
  const fee = Math.round((gross * commissionPct) / 100);
  return { fee, net: gross - fee + adjustment };
};

export const maskAccount = (acc: string) => acc.slice(0, 2) + "•".repeat(Math.max(0, acc.length - 6)) + acc.slice(-4);
