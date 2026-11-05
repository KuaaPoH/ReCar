import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import { addDays, addMonths, startOfMonth } from "date-fns";
import {
  BarChart,
  Bar,
  LineChart,
  Line,
  CartesianGrid,
  XAxis,
  YAxis,
  ResponsiveContainer,
  Tooltip,
  Legend,
} from "recharts";
import { Star, RotateCcw, ArrowUpRight, Lightbulb, ChartNoAxesCombined } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { PageHeader, EmptyState } from "@/components/host/PageHeader";
import { Stats, Section, SelectField, tableClass, Notice } from "@/components/host/ExtensionUI";
import { useHost } from "@/lib/host/store";
import { useHostExtension } from "@/lib/host/extension-store";
import { vehiclePerformance, isoDate, dateRange } from "@/lib/host/extension";
import { financeBreakdown } from "@/lib/host/rules";
import { DEMO_TODAY } from "@/lib/host/mock-data";
import { formatVND, formatShortVND, formatDate } from "@/lib/host/format";
export const Route = createFileRoute("/CarOwner/Analytics/Index")({
  head: () => ({
    meta: [
      { title: "Phân tích hiệu suất — ReCar Host Pro" },
      {
        name: "description",
        content: "Phân tích doanh thu, tỷ lệ lấp đầy và đánh giá thực tế từ dữ liệu đội xe ReCar.",
      },
      { property: "og:title", content: "Phân tích hiệu suất — ReCar Host Pro" },
      {
        property: "og:description",
        content: "So sánh hiệu quả kinh doanh và đánh giá khách thuê theo từng xe.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: AnalyticsPage,
});
const chartColors = [
  "var(--chart-1)",
  "var(--chart-3)",
  "var(--chart-4)",
  "var(--chart-2)",
  "var(--chart-5)",
];
function AnalyticsPage() {
  const { cars, orders, reviews } = useHost();
  const { blocks } = useHostExtension();
  const [period, setPeriod] = useState("6m");
  const [carId, setCarId] = useState("all");
  const [brand, setBrand] = useState("all");
  const [segment, setSegment] = useState("all");
  const [customStart, setCustomStart] = useState("2026-05-01");
  const [customEnd, setCustomEnd] = useState(DEMO_TODAY);
  const [sort, setSort] = useState("gross");
  const [occupancySort, setOccupancySort] = useState("desc");
  const [chartMode, setChartMode] = useState("month");
  const today = new Date(DEMO_TODAY + "T12:00");
  const end =
    period === "prev"
      ? isoDate(startOfMonth(today))
      : period === "custom"
        ? isoDate(addDays(new Date(customEnd + "T12:00"), 1))
        : isoDate(addDays(today, 1));
  const start =
    period === "7"
      ? isoDate(addDays(today, -6))
      : period === "30"
        ? isoDate(addDays(today, -29))
        : period === "month"
          ? isoDate(startOfMonth(today))
          : period === "prev"
            ? isoDate(startOfMonth(addMonths(today, -1)))
            : period === "custom"
              ? customStart
              : isoDate(startOfMonth(addMonths(today, -5)));
  const shownCars = cars.filter(
    (c) =>
      (carId === "all" || String(c.id) === carId) &&
      (brand === "all" || c.brand === brand) &&
      (segment === "all" || c.segment === segment),
  );
  const rows = shownCars.map((c) => vehiclePerformance(c, orders, reviews, blocks, start, end));
  const sorted = [...rows].sort((a, b) =>
    sort === "low"
      ? a.occupancy - b.occupancy
      : sort === "occupancy"
        ? b.occupancy - a.occupancy
        : sort === "rating"
          ? b.avg - a.avg
          : b.gross - a.gross,
  );
  const rs = rows.flatMap((r) => r.reviews).sort((a, b) => b.date.localeCompare(a.date));
  const totals = rows.reduce(
    (t, r) => ({
      gross: t.gross + r.gross,
      net: t.net + r.net,
      trips: t.trips + r.trips,
      rented: t.rented + r.rentedDays,
      available: t.available + r.availableDays,
    }),
    { gross: 0, net: 0, trips: 0, rented: 0, available: 0 },
  );
  const avg = rs.length ? rs.reduce((s, r) => s + r.stars, 0) / rs.length : 0;
  const dates = dateRange(start, end);
  const key = (date: string) =>
    chartMode === "month"
      ? date.slice(0, 7)
      : chartMode === "week"
        ? isoDate(
            addDays(new Date(date + "T12:00"), -((new Date(date + "T12:00").getDay() + 6) % 7)),
          )
        : date;
  const keys = Array.from(new Set(dates.map(key)));
  const chart = keys.map((k) => ({
    period: k,
    ...Object.fromEntries(
      shownCars.map((c) => [
        String(c.id),
        orders
          .filter(
            (o) =>
              o.carId === c.id &&
              o.status === 5 &&
              o.returnAt.slice(0, 10) >= start &&
              o.returnAt.slice(0, 10) < end &&
              key(o.returnAt.slice(0, 10)) === k,
          )
          .reduce((s, o) => s + o.total, 0),
      ]),
    ),
  }));
  const occupancy = [...rows]
    .sort((a, b) =>
      occupancySort === "desc" ? b.occupancy - a.occupancy : a.occupancy - b.occupancy,
    )
    .map((r) => ({
      name: r.car.brand + " " + r.car.name.split(" ")[1],
      occupancy: Number(r.occupancy.toFixed(1)),
    }));
  const best = [...rows]
    .filter((r) => r.reviews.length)
    .sort(
      (a, b) =>
        b.reviews.filter((r) => r.stars >= 4).length - a.reviews.filter((r) => r.stars >= 4).length,
    )[0];
  const reset = () => {
    setPeriod("6m");
    setCarId("all");
    setBrand("all");
    setSegment("all");
  };
  return (
    <div className="space-y-6">
      <PageHeader
        title="Phân tích hiệu suất xe"
        description={`${formatDate(start)} – ${formatDate(isoDate(addDays(new Date(end + "T12:00"), -1)))}`}
        actions={
          <Button variant="outline" onClick={reset}>
            <RotateCcw />
            Đặt lại
          </Button>
        }
      />
      <div className="flex flex-wrap gap-3">
        <SelectField
          label="Kỳ báo cáo"
          value={period}
          onChange={setPeriod}
          options={[
            { value: "7", label: "7 ngày" },
            { value: "30", label: "30 ngày" },
            { value: "month", label: "Tháng này" },
            { value: "prev", label: "Tháng trước" },
            { value: "6m", label: "6 tháng" },
            { value: "custom", label: "Tùy chỉnh" },
          ]}
        />
        <SelectField
          label="Xe báo cáo"
          value={carId}
          onChange={setCarId}
          options={[
            { value: "all", label: "Tất cả xe" },
            ...cars.map((c) => ({ value: String(c.id), label: c.name })),
          ]}
        />
        <SelectField
          label="Hãng xe"
          value={brand}
          onChange={setBrand}
          options={[
            { value: "all", label: "Tất cả hãng" },
            ...Array.from(new Set(cars.map((c) => c.brand))).map((value) => ({
              value,
              label: value,
            })),
          ]}
        />
        <SelectField
          label="Phân khúc"
          value={segment}
          onChange={setSegment}
          options={[
            { value: "all", label: "Tất cả phân khúc" },
            ...Array.from(new Set(cars.map((c) => c.segment))).map((value) => ({
              value,
              label: value,
            })),
          ]}
        />
        {period === "custom" && (
          <>
            <Input
              aria-label="Báo cáo từ ngày"
              type="date"
              value={customStart}
              onChange={(e) => e.target.value && setCustomStart(e.target.value)}
              className="w-44"
            />
            <Input
              aria-label="Báo cáo đến ngày"
              type="date"
              value={customEnd}
              onChange={(e) => e.target.value && setCustomEnd(e.target.value)}
              className="w-44"
            />
          </>
        )}
      </div>
      {start >= end ? (
        <p role="alert" className="text-destructive">
          Khoảng ngày không hợp lệ.
        </p>
      ) : (
        <>
          <Stats
            items={[
              {
                label: "Tỷ lệ lấp đầy",
                value: `${totals.available ? ((totals.rented / totals.available) * 100).toFixed(1) : "0"}%`,
                note: "Số ngày lịch có đơn đang thuê hoặc hoàn tất / số ngày có thể kinh doanh. Loại ngày chặn, ngày trước khi tạo xe và xe không hoạt động; ngày trả có giờ thuê vẫn được tính.",
              },
              {
                label: "Tổng doanh thu",
                value: formatVND(totals.gross),
                note: "Tổng giá trị đơn hoàn tất có ngày trả nằm trong kỳ. Không tính đơn hủy hay chưa hoàn tất.",
              },
              {
                label: "Doanh thu ròng",
                value: formatVND(totals.net),
                note: "Tổng tiền thuê của đơn hoàn tất trừ phí sàn 10% đúng một lần.",
              },
              {
                label: "Chuyến hoàn thành",
                value: totals.trips,
                note: "Số đơn có trạng thái Hoàn tất và ngày trả trong kỳ; không dùng bộ đếm chuyến tích lũy.",
              },
              {
                label: `Đánh giá · ${rs.length} lượt`,
                value: rs.length ? `${avg.toFixed(1)} / 5` : "Chưa có",
                note: "Trung bình số sao của đánh giá thuộc xe và khoảng ngày được chọn.",
              },
            ]}
          />
          <Section
            title="Doanh thu theo thời gian"
            action={
              <SelectField
                label="Nhóm thời gian biểu đồ"
                value={chartMode}
                onChange={setChartMode}
                options={[
                  { value: "day", label: "Theo ngày" },
                  { value: "week", label: "Theo tuần" },
                  { value: "month", label: "Theo tháng" },
                ]}
              />
            }
          >
            <div className="h-80 min-w-0">
              {shownCars.length ? (
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart data={chart} margin={{ left: 12, right: 12, top: 10, bottom: 10 }}>
                    <CartesianGrid stroke="var(--border)" strokeDasharray="3 3" />
                    <XAxis
                      dataKey="period"
                      tick={{ fill: "var(--muted-foreground)", fontSize: 11 }}
                      tickFormatter={(v) =>
                        chartMode === "month"
                          ? `T${Number(String(v).slice(5, 7))}`
                          : String(v).slice(5).split("-").reverse().join("/")
                      }
                    />
                    <YAxis
                      tickFormatter={formatShortVND}
                      tick={{ fill: "var(--muted-foreground)", fontSize: 11 }}
                    />
                    <Tooltip
                      formatter={(v) => formatVND(Number(v))}
                      labelFormatter={(v) =>
                        chartMode === "month"
                          ? `Tháng ${String(v).slice(5)}/${String(v).slice(0, 4)}`
                          : formatDate(String(v))
                      }
                      contentStyle={{
                        background: "var(--card)",
                        borderColor: "var(--border)",
                        borderRadius: 8,
                      }}
                    />
                    <Legend wrapperStyle={{ fontSize: 11 }} />
                    {shownCars.map((c, i) => (
                      <Line
                        key={c.id}
                        dataKey={String(c.id)}
                        name={c.name}
                        stroke={chartColors[i % 5]}
                        strokeWidth={2}
                        dot={{ r: 3 }}
                        isAnimationActive={false}
                      />
                    ))}
                  </LineChart>
                </ResponsiveContainer>
              ) : (
                <EmptyState icon={<ChartNoAxesCombined />} title="Không có xe phù hợp" />
              )}
            </div>
          </Section>
          <Section
            title="Tỷ lệ lấp đầy từng xe"
            action={
              <SelectField
                label="Sắp xếp lấp đầy"
                value={occupancySort}
                onChange={setOccupancySort}
                options={[
                  { value: "desc", label: "Cao xuống thấp" },
                  { value: "asc", label: "Thấp lên cao" },
                ]}
              />
            }
          >
            <div className="h-64">
              {occupancy.length ? (
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={occupancy} layout="vertical" margin={{ left: 20, right: 24 }}>
                    <XAxis
                      type="number"
                      domain={[0, 100]}
                      tickFormatter={(v) => `${v}%`}
                      tick={{ fontSize: 11, fill: "var(--muted-foreground)" }}
                    />
                    <YAxis
                      dataKey="name"
                      type="category"
                      width={110}
                      tick={{ fontSize: 11, fill: "var(--muted-foreground)" }}
                    />
                    <Tooltip
                      formatter={(v) => `${v}%`}
                      contentStyle={{ background: "var(--card)", borderColor: "var(--border)" }}
                    />
                    <Bar
                      dataKey="occupancy"
                      name="Lấp đầy"
                      fill="var(--success)"
                      radius={[0, 4, 4, 0]}
                      isAnimationActive={false}
                    />
                  </BarChart>
                </ResponsiveContainer>
              ) : (
                <EmptyState icon={<ChartNoAxesCombined />} title="Chưa có dữ liệu" />
              )}
            </div>
          </Section>
          <Section
            title="Hiệu suất từng xe"
            action={
              <SelectField
                label="Sắp xếp hiệu suất"
                value={sort}
                onChange={setSort}
                options={[
                  { value: "gross", label: "Doanh thu cao nhất" },
                  { value: "occupancy", label: "Lấp đầy cao nhất" },
                  { value: "rating", label: "Đánh giá cao nhất" },
                  { value: "low", label: "Hiệu quả thấp nhất" },
                ]}
              />
            }
          >
            <div className="overflow-x-auto rounded-lg border">
              <table className={tableClass}>
                <thead>
                  <tr>
                    <th>Xe</th>
                    <th>Chuyến</th>
                    <th>Ngày thuê</th>
                    <th>Lấp đầy</th>
                    <th>Doanh thu</th>
                    <th>Ròng</th>
                    <th>Đánh giá</th>
                    <th />
                  </tr>
                </thead>
                <tbody>
                  {sorted.map((r) => (
                    <tr key={r.car.id}>
                      <td>
                        <div className="flex items-center gap-3">
                          <img
                            src={r.car.image}
                            alt={r.car.name}
                            className="h-12 w-20 rounded object-cover"
                          />
                          <div className="min-w-40">
                            <p className="font-semibold">{r.car.name}</p>
                            <p className="text-xs text-muted-foreground">{r.car.plate}</p>
                          </div>
                        </div>
                      </td>
                      <td>{r.trips}</td>
                      <td>{r.rentedDays}</td>
                      <td>{r.availableDays ? `${r.occupancy.toFixed(1)}%` : "Không hoạt động"}</td>
                      <td className="whitespace-nowrap">{formatVND(r.gross)}</td>
                      <td className="whitespace-nowrap font-semibold text-success">
                        {formatVND(r.net)}
                      </td>
                      <td>{r.reviews.length ? `${r.avg.toFixed(1)} ★` : "—"}</td>
                      <td>
                        <Button variant="ghost" size="icon" asChild>
                          <Link
                            aria-label={`Chi tiết ${r.car.name}`}
                            to="/CarOwner/Cars/Details/$id"
                            params={{ id: String(r.car.id) }}
                          >
                            <ArrowUpRight />
                          </Link>
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              {!rows.length && (
                <EmptyState icon={<ChartNoAxesCombined />} title="Không có xe phù hợp" />
              )}
            </div>
          </Section>
          <Section title="Đánh giá khách thuê">
            <div className="grid gap-6 lg:grid-cols-[280px_1fr]">
              <div>
                <p className="text-3xl font-bold">
                  {rs.length ? avg.toFixed(1) : "—"}{" "}
                  <Star className="inline size-5 fill-warning text-warning" />
                </p>
                <p className="mb-4 text-xs text-muted-foreground">{rs.length} đánh giá trong kỳ</p>
                {[5, 4, 3, 2, 1].map((n) => {
                  const count = rs.filter((r) => r.stars === n).length;
                  return (
                    <div
                      key={n}
                      className="mb-2 grid grid-cols-[40px_1fr_24px] items-center gap-2 text-xs"
                    >
                      <span>{n} sao</span>
                      <progress
                        className="h-2 w-full accent-primary"
                        max={Math.max(1, rs.length)}
                        value={count}
                      />
                      <span>{count}</span>
                    </div>
                  );
                })}
                {best && (
                  <p className="mt-4 text-xs text-success">
                    Nhiều phản hồi tích cực nhất: {best.car.name}
                  </p>
                )}
              </div>
              <div className="space-y-4">
                {rs.slice(0, 5).map((r) => (
                  <div key={r.id} className="border-b pb-4">
                    <div className="flex flex-wrap justify-between gap-2 text-sm">
                      <strong>{r.guest}</strong>
                      <span className="text-warning">
                        {"★".repeat(r.stars)}
                        {"☆".repeat(5 - r.stars)}
                      </span>
                    </div>
                    <p className="my-1 text-sm">{r.comment}</p>
                    <p className="text-xs text-muted-foreground">
                      {cars.find((c) => c.id === r.carId)?.name} · {formatDate(r.date)}
                    </p>
                  </div>
                ))}
                {!rs.length && <EmptyState icon={<Star />} title="Chưa có đánh giá trong kỳ" />}
              </div>
            </div>
          </Section>
          <Section title="Gợi ý tham khảo">
            <div className="grid gap-3 md:grid-cols-2">
              {rows
                .filter((r) => r.availableDays)
                .map((r) => (
                  <div
                    key={r.car.id}
                    className="flex gap-3 border-l-2 border-primary bg-primary-soft p-4"
                  >
                    <Lightbulb className="mt-0.5 size-5 shrink-0 text-primary" />
                    <div>
                      <p className="text-sm font-semibold">{r.car.name}</p>
                      <p className="mt-1 text-xs leading-relaxed text-muted-foreground">
                        {r.avg && r.avg < 4.5
                          ? "Xem lại phản hồi khách thuê để cải thiện trải nghiệm."
                          : r.occupancy < 40
                            ? "Tỷ lệ lấp đầy thấp trong kỳ; cân nhắc giá thuê và thời gian mở lịch."
                            : r.occupancy >= 70
                              ? "Tỷ lệ lấp đầy cao; cân nhắc điều chỉnh giá cuối tuần."
                              : "Theo dõi doanh thu và giữ lịch xe cập nhật."}
                      </p>
                    </div>
                  </div>
                ))}
            </div>
            <Notice>
              Gợi ý chỉ tham khảo, không tự thay đổi giá. Chưa có dữ liệu lượt xem hoặc chuyển đổi
              nên không đưa ra kết luận về những chỉ số này.
            </Notice>
          </Section>
        </>
      )}
    </div>
  );
}
