using CarRental.Models;
using CarRental.Services;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FinanceController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly INotificationService _notificationService;

        public FinanceController(DbRenalCarContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        private bool CheckAdmin()
        {
            return Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session);
        }

        private static decimal CalcOrderTotal(CarRentalOrder o)
        {
            decimal total = (o.Deposit ?? 0m) + (o.Payment ?? 0m);
            if (total <= 0 && o.OrderDetails != null && o.OrderDetails.Any())
            {
                foreach (var d in o.OrderDetails)
                {
                    var pDate = d.PickupDate ?? (o.OrderDate.HasValue ? o.OrderDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Today);
                    var rDate = d.ReturnDate ?? (o.ReturnDate.HasValue ? o.ReturnDate.Value.ToDateTime(TimeOnly.MinValue) : pDate.AddDays(1));
                    int days = Math.Max(1, (int)Math.Ceiling((rDate.Date - pDate.Date).TotalDays));
                    total += (d.Price ?? d.Car?.Price ?? 0m) * days;
                }
            }
            if (total <= 0 && (o.Deposit ?? 0m) > 0)
            {
                total = Math.Round((o.Deposit ?? 0m) / 0.3m, 0);
            }
            return total;
        }

        public IActionResult Index()
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var allCars = _context.Cars.ToList();
            var allOrders = _context.CarRentalOrders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .ToList();

            var validOrders = allOrders.Where(o => o.StatusId == 3 || o.StatusId == 4 || o.StatusId == 5).ToList();

            var sysSetting = _context.SystemSettings.FirstOrDefault();
            decimal defaultCommRate = (sysSetting?.CommissionPct ?? 10) / 100m;
            decimal defaultDepRate = (sysSetting?.DepositPct ?? 30) / 100m;

            decimal totalGmv = validOrders.Sum(o => CalcOrderTotal(o));
            decimal totalDeposit = validOrders.Sum(o => o.Deposit ?? Math.Round(CalcOrderTotal(o) * defaultDepRate, 0));
            decimal totalCommission = validOrders.Sum(o => o.PlatformFee ?? Math.Round(CalcOrderTotal(o) * defaultCommRate, 0));
            decimal totalOwnerPayout = totalGmv - totalCommission;

            ViewBag.TotalGmv = totalGmv;
            ViewBag.TotalDeposit = totalDeposit;
            ViewBag.TotalCommission = totalCommission;
            ViewBag.TotalOwnerPayout = totalOwnerPayout;
            ViewBag.CurrentCommissionPct = sysSetting?.CommissionPct ?? 10;
            ViewBag.CurrentDepositPct = sysSetting?.DepositPct ?? 30;

            // Kỳ đối soát hiện tại (nửa tháng đồng bộ với Ví Chủ xe)
            var nowVn = Function.NowVn;
            int half = nowVn.Day <= 15 ? 1 : 2;
            string currentCycleKey = $"{nowVn.Year}-{nowVn.Month:D2}-0{half}";
            string periodLabel = $"Tháng {nowVn.Month:D2}/{nowVn.Year} (Kỳ {half})";

            var paidPayouts = _context.PartnerPayouts
                .Where(p => p.Status == "paid")
                .ToList();

            var registrations = _context.CarOwnerRegistrations
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            // Bảng đối soát theo từng chủ xe (100% dữ liệu thật CSDL)
            var ownersList = _context.Customers.Where(a => a.RoleId == 3).ToList();
            var settlements = ownersList.Select(a =>
            {
                var ownerCarIds = allCars.Where(c => c.OwnerId == a.CustomerId).Select(c => c.CarId).ToHashSet();
                var ownerOrders = validOrders.Where(o => o.OrderDetails.Any(od => ownerCarIds.Contains(od.CarId))).ToList();

                int trips = ownerOrders.Count;
                decimal gmvForOwner = ownerOrders.Sum(o => CalcOrderTotal(o));
                decimal depositForOwner = ownerOrders.Sum(o => o.Deposit ?? Math.Round(CalcOrderTotal(o) * defaultDepRate, 0));
                decimal fee = ownerOrders.Sum(o => o.PlatformFee ?? Math.Round(CalcOrderTotal(o) * defaultCommRate, 0));
                decimal netPayout = Math.Max(0m, depositForOwner - fee);

                var reg = registrations.FirstOrDefault(r => r.CustomerId == a.CustomerId);
                string bankName = reg?.BankName ?? a.Bank ?? "";
                string bankAcc = reg?.AccountNumber ?? a.AccountNumber ?? "";
                string bankInfo = !string.IsNullOrWhiteSpace(bankName) && !string.IsNullOrWhiteSpace(bankAcc)
                    ? $"{bankName} · {bankAcc}"
                    : "Chưa liên kết ngân hàng";

                string settlementCode = $"PO-{nowVn.Year}{nowVn.Month:D2}-0{half}-{a.CustomerId}";
                bool isSettled = paidPayouts.Any(p => p.OwnerId == a.CustomerId && (p.SettlementCode == settlementCode || p.CycleKey == currentCycleKey));

                return new CarRental.ViewModels.SettlementVM
                {
                    SettlementCode = settlementCode,
                    OwnerId = a.CustomerId,
                    OwnerName = a.FullName ?? a.Name ?? a.Username ?? "Chủ xe",
                    OwnerPhone = a.PhoneNumber ?? "Chưa cập nhật",
                    Bank = bankInfo,
                    Period = periodLabel,
                    Trips = trips,
                    TotalGmv = gmvForOwner,
                    Fee = fee,
                    NetPayout = netPayout,
                    IsSettled = isSettled
                };
            }).OrderByDescending(s => s.TotalGmv).ToList();

            ViewBag.Settlements = settlements;

            ViewBag.WithdrawRequests = _context.WalletTransactions
                .Include(w => w.Customer)
                .Where(w => w.Type == "withdraw")
                .OrderBy(w => w.Status == "pending" ? 0 : 1)
                .ThenByDescending(w => w.CreatedAt)
                .Take(30)
                .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SettlePayout(int ownerId, string code, string? period, decimal amount = 0)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var nowVn = Function.NowVn;
            int half = nowVn.Day <= 15 ? 1 : 2;
            string currentCycleKey = $"{nowVn.Year}-{nowVn.Month:D2}-0{half}";

            bool exists = _context.PartnerPayouts.Any(p => p.SettlementCode == code);
            if (!exists)
            {
                var payout = new PartnerPayout
                {
                    SettlementCode = code,
                    OwnerId = ownerId,
                    CycleKey = currentCycleKey,
                    Period = period ?? $"Tháng {nowVn.Month:D2}/{nowVn.Year} (Kỳ {half})",
                    Amount = amount,
                    Status = "paid",
                    PaidAt = nowVn,
                    ConfirmedBy = string.IsNullOrEmpty(Function.GetAdminUserName(HttpContext.Session)) ? "Admin" : Function.GetAdminUserName(HttpContext.Session)
                };
                _context.PartnerPayouts.Add(payout);
                await _context.SaveChangesAsync();

                try
                {
                    await _notificationService.CreateNotificationAsync(
                        ownerId,
                        "CarOwner",
                        "Đã quyết toán đối soát",
                        $"Admin đã xác nhận chuyển khoản quyết toán mã {code} ({amount:N0} đ). Kiểm tra ngay trong Ví đối tác.",
                        "Payout",
                        "/CarOwner/Wallet?tab=payouts"
                    );
                }
                catch { }
            }

            return Json(new
            {
                success = true,
                message = $"Đã xác nhận lệnh chuyển tiền quyết toán {code} thành công. Trạng thái Ví đối tác của chủ xe đã được đồng bộ."
            });
        }

        [HttpPost]
        public async Task<IActionResult> ApproveWithdraw(int transactionId)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var tx = _context.WalletTransactions.FirstOrDefault(w => w.TransactionId == transactionId && w.Type == "withdraw");
            if (tx == null)
            {
                return Json(new { success = false, message = "Không tìm thấy yêu cầu rút tiền." });
            }
            if (tx.Status != "pending")
            {
                return Json(new { success = false, message = "Yêu cầu này đã được xử lý trước đó." });
            }

            tx.Status = "completed";
            tx.ProcessedAt = Function.NowVn;
            await _context.SaveChangesAsync();

            try
            {
                await _notificationService.CreateNotificationAsync(
                    tx.CustomerId,
                    "Customer",
                    "Rút tiền thành công",
                    $"Yêu cầu rút {tx.Amount:N0} đ về {tx.BankName} ({tx.BankAccount}) đã được Admin chuyển khoản thành công.",
                    "Wallet",
                    "/Accounts/Index?tab=wallet");
            }
            catch { }

            return Json(new { success = true, message = $"Đã xác nhận chuyển khoản {tx.Amount:N0} đ thành công." });
        }

        [HttpPost]
        public async Task<IActionResult> RejectWithdraw(int transactionId)
        {
            if (!CheckAdmin())
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var tx = _context.WalletTransactions.FirstOrDefault(w => w.TransactionId == transactionId && w.Type == "withdraw");
            if (tx == null)
            {
                return Json(new { success = false, message = "Không tìm thấy yêu cầu rút tiền." });
            }
            if (tx.Status != "pending")
            {
                return Json(new { success = false, message = "Yêu cầu này đã được xử lý trước đó." });
            }

            tx.Status = "rejected";
            tx.ProcessedAt = Function.NowVn;

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == tx.CustomerId);
            if (customer != null)
            {
                customer.Balance += tx.Amount;
                _context.WalletTransactions.Add(new WalletTransaction
                {
                    CustomerId = customer.CustomerId,
                    Type = "refund",
                    Amount = tx.Amount,
                    BalanceAfter = customer.Balance,
                    Description = $"Hoàn lại tiền do từ chối yêu cầu rút #{tx.TransactionId}",
                    Status = "completed",
                    CreatedAt = Function.NowVn,
                    ProcessedAt = Function.NowVn
                });
            }

            await _context.SaveChangesAsync();

            try
            {
                await _notificationService.CreateNotificationAsync(
                    tx.CustomerId,
                    "Customer",
                    "Yêu cầu rút tiền bị từ chối",
                    $"Yêu cầu rút {tx.Amount:N0} đ đã bị từ chối và số tiền đã được hoàn lại vào Số dư Ví ReCar của bạn.",
                    "Wallet",
                    "/Accounts/Index?tab=wallet");
            }
            catch { }

            return Json(new { success = true, message = $"Đã từ chối yêu cầu rút tiền và hoàn {tx.Amount:N0} đ vào ví khách hàng." });
        }
    }
}
