using System.Text;
using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.CarOwner.Controllers
{
    [Area("CarOwner")]
    public class WalletController : Controller
    {
        private readonly DbRenalCarContext _context;

        public WalletController(DbRenalCarContext context)
        {
            _context = context;
        }

        private Customer? GetCurrentOwner()
        {
            var accountId = Function.GetAccountId(HttpContext.Session);
            var carOwnerId = Function.GetCarOwnerAccountId(HttpContext.Session);
            var userEmail = Function.GetEmail(HttpContext.Session);

            var customer = _context.Customers
                .Include(a => a.Role)
                .FirstOrDefault(a => (carOwnerId > 0 && a.CustomerId == carOwnerId) ||
                                     (accountId > 0 && a.CustomerId == accountId) ||
                                     (!string.IsNullOrEmpty(userEmail) && a.Email == userEmail && a.RoleId == 3));

            if (customer == null)
            {
                customer = _context.Customers
                    .Include(a => a.Role)
                    .FirstOrDefault(a => a.CustomerId == 1022 && a.RoleId == 3)
                    ?? _context.Customers.Include(a => a.Role).FirstOrDefault(a => a.RoleId == 3);
            }

            return customer;
        }

        public static decimal CalculateOrderTotal(CarRentalOrder order)
        {
            decimal total = order.Payment ?? 0m;
            if (total <= 0 && order.OrderDetails != null && order.OrderDetails.Any())
            {
                foreach (var d in order.OrderDetails)
                {
                    var pDate = d.PickupDate ?? (order.OrderDate.HasValue ? order.OrderDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Today);
                    var rDate = d.ReturnDate ?? (order.ReturnDate.HasValue ? order.ReturnDate.Value.ToDateTime(TimeOnly.MinValue) : pDate.AddDays(1));
                    int days = Math.Max(1, (int)Math.Ceiling((rDate.Date - pDate.Date).TotalDays));
                    decimal unitPrice = d.Price ?? d.Car?.Price ?? 0m;
                    total += unitPrice * days;
                }
            }
            if (total <= 0 && (order.Deposit ?? 0m) > 0)
            {
                total = Math.Round((order.Deposit ?? 0m) / 0.3m, 0);
            }
            return total;
        }

        public IActionResult Index(string? start, string? end, string status = "all", string tab = "transactions")
        {
            if (!Function.IsCarOwnerLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            DateTime startDate = DateTime.TryParse(start, out var sDate) ? sDate.Date : new DateTime(DateTime.Today.Year, 1, 1);
            DateTime endDate = DateTime.TryParse(end, out var eDate) ? eDate.Date : DateTime.Today.AddMonths(1);

            var ownerCarIds = _context.Cars
                .Where(c => c.OwnerId == owner.CustomerId)
                .Select(c => c.CarId)
                .ToList();

            var orders = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .Where(o => o.OrderDetails.Any(od => ownerCarIds.Contains(od.CarId)))
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.OrderId)
                .ToList();

            // Lấy thông tin ngân hàng từ hồ sơ đăng ký chủ xe hoặc bảng Customer
            var registration = _context.CarOwnerRegistrations
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault(r => r.CustomerId == owner.CustomerId);

            string bankName = registration?.BankName ?? owner.Bank ?? "Vietcombank";
            string bankAccount = registration?.AccountNumber ?? owner.AccountNumber ?? "0071001234567";
            string bankHolder = registration?.AccountHolder ?? (owner.FullName ?? owner.Name ?? "TRAN QUOC BAO").ToUpperInvariant();

            // Xây dựng danh sách đơn đã hoàn thành (StatusId == 5) và đơn đang hoạt động (StatusId == 3, 4)
            var completedOrders = orders.Where(o => o.StatusId == 5).ToList();
            var activeOrders = orders.Where(o => o.StatusId == 3 || o.StatusId == 4).ToList();

            // Nhóm các đơn thành các kỳ quyết toán (Payout Batches) theo nửa tháng
            var payoutItems = new List<WalletPayoutVm>();
            var transactionItems = new List<WalletTransactionVm>();

            var allRelevantOrders = completedOrders.Concat(activeOrders)
                .OrderByDescending(o => o.ReturnDate ?? o.OrderDate)
                .ToList();

            var groupedByCycle = allRelevantOrders.GroupBy(o =>
            {
                var refDate = o.ReturnDate?.ToDateTime(TimeOnly.MinValue)
                              ?? o.OrderDetails.FirstOrDefault()?.ReturnDate
                              ?? o.OrderDate?.ToDateTime(TimeOnly.MinValue)
                              ?? DateTime.Today;
                int half = refDate.Day <= 15 ? 1 : 2;
                return new { refDate.Year, refDate.Month, Half = half };
            })
            .OrderByDescending(g => g.Key.Year)
            .ThenByDescending(g => g.Key.Month)
            .ThenByDescending(g => g.Key.Half)
            .ToList();

            foreach (var grp in groupedByCycle)
            {
                var cycleOrders = grp.ToList();
                int daysInMonth = DateTime.DaysInMonth(grp.Key.Year, grp.Key.Month);
                DateTime periodStart = new DateTime(grp.Key.Year, grp.Key.Month, grp.Key.Half == 1 ? 1 : 16);
                DateTime periodEnd = new DateTime(grp.Key.Year, grp.Key.Month, grp.Key.Half == 1 ? 15 : daysInMonth);

                string payoutId = $"PO-{grp.Key.Year}{grp.Key.Month:D2}-0{grp.Key.Half}";
                string periodLabel = $"{periodStart:dd/MM/yyyy} – {periodEnd:dd/MM/yyyy}";

                // Xác định trạng thái kỳ đối soát:
                // Nếu có đơn đang chạy (StatusId 3, 4) hoặc kỳ chưa kết thúc -> pending
                // Nếu kỳ kết thúc trong vòng 3 ngày -> processing
                // Nếu kỳ đã kết thúc hơn 3 ngày và tất cả đơn đã hoàn thành -> paid
                bool hasUnfinished = cycleOrders.Any(o => o.StatusId != 5);
                double daysSinceEnd = (DateTime.Today - periodEnd).TotalDays;

                string payoutStatus = "paid";
                DateTime? paidAt = periodEnd.AddDays(2);
                if (hasUnfinished || daysSinceEnd < 0)
                {
                    payoutStatus = "pending";
                    paidAt = null;
                }
                else if (daysSinceEnd <= 3)
                {
                    payoutStatus = "processing";
                    paidAt = null;
                }

                decimal depositTotal = 0m;
                decimal feeTotal = 0m;
                decimal payoutAmount = 0m;
                var orderCodes = new List<string>();

                foreach (var ord in cycleOrders)
                {
                    decimal total = CalculateOrderTotal(ord);
                    decimal dep = Math.Round(total * 0.30m, 0);
                    decimal rem = Math.Round(total * 0.70m, 0);
                    decimal fee = Math.Round(total * 0.10m, 0);
                    decimal ownerNet = total - fee; // 90%
                    decimal platformTransfer = dep - fee; // 20%

                    depositTotal += dep;
                    feeTotal += fee;
                    payoutAmount += platformTransfer;

                    string code = $"#RC-{ord.OrderId:D4}";
                    orderCodes.Add(code);

                    var txDate = ord.ReturnDate?.ToDateTime(new TimeOnly(18, 0))
                                 ?? ord.OrderDetails.FirstOrDefault()?.ReturnDate
                                 ?? ord.OrderDate?.ToDateTime(new TimeOnly(10, 0))
                                 ?? DateTime.Today;

                    var firstCar = ord.OrderDetails.FirstOrDefault()?.Car;
                    string carInfo = firstCar != null ? $"{firstCar.CarName} ({firstCar.LicensePlate})" : "Xe tự lái";

                    transactionItems.Add(new WalletTransactionVm
                    {
                        Id = $"TN-{code}",
                        CreatedAt = txDate,
                        Type = "earning",
                        OrderId = ord.OrderId,
                        OrderCode = code,
                        Description = $"Thu nhập đơn {code} · {carInfo}",
                        GrossAmount = total,
                        DepositAmount = dep,
                        DirectCollectAmount = rem,
                        PlatformFee = fee,
                        NetAmount = ownerNet,
                        PlatformPayoutAmount = platformTransfer,
                        Status = ord.StatusId == 5 ? (payoutStatus == "paid" ? "completed" : "pending") : "pending"
                    });
                }

                payoutItems.Add(new WalletPayoutVm
                {
                    Id = payoutId,
                    Period = periodLabel,
                    CreatedAt = periodEnd,
                    PaidAt = paidAt,
                    OrderCount = cycleOrders.Count,
                    Orders = orderCodes,
                    DepositTotal = depositTotal,
                    PlatformFeeTotal = feeTotal,
                    Amount = payoutAmount,
                    Status = payoutStatus,
                    BankName = bankName,
                    BankAccount = bankAccount,
                    BankHolder = bankHolder
                });

                if (payoutStatus == "paid" && paidAt.HasValue)
                {
                    transactionItems.Add(new WalletTransactionVm
                    {
                        Id = $"CK-{payoutId}",
                        CreatedAt = paidAt.Value.AddHours(9).AddMinutes(15),
                        Type = "payout",
                        OrderId = 0,
                        OrderCode = payoutId,
                        Description = $"Sàn chuyển khoản đối soát kỳ {periodLabel} ({cycleOrders.Count} đơn)",
                        GrossAmount = 0m,
                        DepositAmount = depositTotal,
                        DirectCollectAmount = 0m,
                        PlatformFee = 0m,
                        NetAmount = payoutAmount,
                        PlatformPayoutAmount = payoutAmount,
                        Status = "completed"
                    });
                }
            }

            // Lọc theo ngày và trạng thái
            var filteredTransactions = transactionItems
                .Where(t => t.CreatedAt.Date >= startDate && t.CreatedAt.Date <= endDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            var filteredPayouts = payoutItems
                .Where(p => p.CreatedAt.Date >= startDate && p.CreatedAt.Date <= endDate)
                .Where(p => status == "all" || p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            var earningTxsInPeriod = filteredTransactions.Where(t => t.Type == "earning").ToList();

            decimal grossInPeriod = earningTxsInPeriod.Sum(t => t.GrossAmount);
            decimal depositInPeriod = Math.Round(grossInPeriod * 0.30m, 0);
            decimal directInPeriod = Math.Round(grossInPeriod * 0.70m, 0);
            decimal feeInPeriod = Math.Round(grossInPeriod * 0.10m, 0);
            decimal netInPeriod = grossInPeriod - feeInPeriod;
            decimal payoutFromPlatformInPeriod = depositInPeriod - feeInPeriod;

            decimal pendingBalance = payoutItems.Where(p => p.Status == "pending").Sum(p => p.Amount);
            decimal processingBalance = payoutItems.Where(p => p.Status == "processing").Sum(p => p.Amount);
            decimal paidTotal = filteredPayouts.Where(p => p.Status == "paid").Sum(p => p.Amount);

            var vm = new WalletIndexVm
            {
                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd"),
                StatusFilter = status,
                ActiveTab = tab,
                BankName = bankName,
                BankAccount = bankAccount,
                BankHolder = bankHolder,
                PendingBalance = pendingBalance,
                ProcessingBalance = processingBalance,
                PaidTotal = paidTotal,
                GrossInPeriod = grossInPeriod,
                DepositInPeriod = depositInPeriod,
                DirectInPeriod = directInPeriod,
                FeeInPeriod = feeInPeriod,
                NetInPeriod = netInPeriod,
                PayoutFromPlatformInPeriod = payoutFromPlatformInPeriod,
                Transactions = filteredTransactions,
                Payouts = filteredPayouts
            };

            ViewData["Title"] = "Ví đối tác & Đối soát";
            return View(vm);
        }

        [HttpGet]
        public IActionResult ExportCsv(string? start, string? end)
        {
            if (!Function.IsCarOwnerLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var result = Index(start, end, "all", "transactions") as ViewResult;
            var vm = result?.Model as WalletIndexVm;
            if (vm == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var sb = new StringBuilder();
            sb.AppendLine("Mã GD,Thời gian,Loại giao dịch,Mã đơn,Mô tả,Tổng đơn (100%),Cọc sàn giữ (30%),Khách trả trực tiếp (70%),Phí sàn (10%),Chủ xe thực nhận (90%),Sàn đối soát (20%),Trạng thái");
            foreach (var t in vm.Transactions)
            {
                string typeLabel = t.Type == "earning" ? "Thu nhập chuyến" : "Sàn chuyển đối soát";
                string statusLabel = t.Status == "completed" ? "Hoàn tất" : "Chờ đối soát";
                string safeDesc = (t.Description ?? "").Replace("\"", "\"\"");
                sb.AppendLine($"\"{t.Id}\",\"{t.CreatedAt:dd/MM/yyyy HH:mm}\",\"{typeLabel}\",\"{t.OrderCode}\",\"{safeDesc}\",{t.GrossAmount},{t.DepositAmount},{t.DirectCollectAmount},{t.PlatformFee},{t.NetAmount},{t.PlatformPayoutAmount},\"{statusLabel}\"");
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return File(bytes, "text/csv; charset=utf-8", $"doi-soat-vi-doi-tac-{DateTime.Now:yyyyMMdd}.csv");
        }
    }

    public class WalletIndexVm
    {
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public string StatusFilter { get; set; } = "all";
        public string ActiveTab { get; set; } = "transactions";

        public string BankName { get; set; } = "";
        public string BankAccount { get; set; } = "";
        public string BankHolder { get; set; } = "";

        public decimal PendingBalance { get; set; }
        public decimal ProcessingBalance { get; set; }
        public decimal PaidTotal { get; set; }

        public decimal GrossInPeriod { get; set; }
        public decimal DepositInPeriod { get; set; }
        public decimal DirectInPeriod { get; set; }
        public decimal FeeInPeriod { get; set; }
        public decimal NetInPeriod { get; set; }
        public decimal PayoutFromPlatformInPeriod { get; set; }

        public List<WalletTransactionVm> Transactions { get; set; } = new();
        public List<WalletPayoutVm> Payouts { get; set; } = new();
    }

    public class WalletTransactionVm
    {
        public string Id { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; } = "earning"; // earning | payout
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal GrossAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal DirectCollectAmount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PlatformPayoutAmount { get; set; }
        public string Status { get; set; } = "completed";
    }

    public class WalletPayoutVm
    {
        public string Id { get; set; } = "";
        public string Period { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public int OrderCount { get; set; }
        public List<string> Orders { get; set; } = new();
        public decimal DepositTotal { get; set; }
        public decimal PlatformFeeTotal { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "paid"; // pending | processing | paid
        public string BankName { get; set; } = "";
        public string BankAccount { get; set; } = "";
        public string BankHolder { get; set; } = "";
    }
}

