using CarRental.Hubs;
using CarRental.Models;
using CarRental.Utilities;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Areas.CarOwner.Controllers
{
    [Area("CarOwner")]
    public class RegisterController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly IHubContext<ReCarHub> _hubContext;

        public RegisterController(DbRenalCarContext context, IHubContext<ReCarHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: /CarOwner/Register (Form đăng ký đối tác chủ xe với KYC & Ngân hàng)
        [HttpGet]
        public async Task<IActionResult> Index(bool edit = false)
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "", ReturnUrl = "/CarOwner/Register" });
            }

            var customerId = Function.GetAccountId(HttpContext.Session);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (customer == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            // Kiểm tra xem đã có hồ sơ đăng ký chưa
            var existingReg = await _context.CarOwnerRegistrations
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => r.CustomerId == customerId || r.Email == customer.Email);

            if (existingReg != null)
            {
                if (existingReg.Status == "Pending" && !edit)
                {
                    return RedirectToAction("Pending");
                }
                if (existingReg.Status == "Approved")
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // Kiểm tra xem đã có tài khoản Chủ xe chính thức trong bảng Accounts chưa
            var existingAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == customer.Email && a.RoleId == 3 && a.IsActive == true);
            if (existingAccount != null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Điền sẵn dữ liệu từ hồ sơ khách hàng hiện tại (nếu có)
            var model = new CarOwnerRegistrationVM
            {
                CustomerId = customerId,
                FullName = !string.IsNullOrEmpty(customer.FullName) ? customer.FullName : (customer.Name ?? string.Empty),
                Email = customer.Email ?? string.Empty,
                Phone = customer.PhoneNumber ?? string.Empty,
                Address = customer.Address ?? string.Empty,
                IdCard = customer.Idcard ?? string.Empty,
                LicenseNumber = customer.LicenseNumber ?? string.Empty,
                ExistingLicenseImage = customer.LicenseImage,
                BankName = customer.Bank ?? string.Empty,
                AccountNumber = customer.AccountNumber ?? string.Empty,
                AccountHolder = !string.IsNullOrEmpty(customer.CompanyName) ? customer.CompanyName : (!string.IsNullOrEmpty(customer.FullName) ? customer.FullName.ToUpper() : string.Empty)
            };

            // Nếu đã nộp hồ sơ (bị từ chối hoặc đang Pending muốn chỉnh sửa), tải lại toàn bộ thông tin cũ
            if (existingReg != null && (existingReg.Status == "Rejected" || (existingReg.Status == "Pending" && edit)))
            {
                model.Id = existingReg.Id;
                if (!string.IsNullOrEmpty(existingReg.FullName)) model.FullName = existingReg.FullName;
                if (!string.IsNullOrEmpty(existingReg.Email)) model.Email = existingReg.Email;
                if (!string.IsNullOrEmpty(existingReg.Phone)) model.Phone = existingReg.Phone;
                if (!string.IsNullOrEmpty(existingReg.Address)) model.Address = existingReg.Address;
                if (!string.IsNullOrEmpty(existingReg.IdCard)) model.IdCard = existingReg.IdCard;
                model.IdCardIssuePlace = existingReg.IdCardIssuePlace;
                model.IdCardIssueDate = existingReg.IdCardIssueDate;
                if (!string.IsNullOrEmpty(existingReg.LicenseNumber)) model.LicenseNumber = existingReg.LicenseNumber;
                model.LicenseClass = existingReg.LicenseClass ?? "B2";
                model.LicenseExpiryDate = existingReg.LicenseExpiryDate;
                if (!string.IsNullOrEmpty(existingReg.BankName)) model.BankName = existingReg.BankName;
                if (!string.IsNullOrEmpty(existingReg.AccountNumber)) model.AccountNumber = existingReg.AccountNumber;
                if (!string.IsNullOrEmpty(existingReg.AccountHolder)) model.AccountHolder = existingReg.AccountHolder;
                model.ExistingIdCardFrontImage = existingReg.IdCardFrontImage;
                model.ExistingIdCardBackImage = existingReg.IdCardBackImage;
                if (!string.IsNullOrEmpty(existingReg.LicenseImage)) model.ExistingLicenseImage = existingReg.LicenseImage;

                if (existingReg.Status == "Rejected")
                {
                    ViewBag.RejectReason = existingReg.RejectReason;
                }
                else if (existingReg.Status == "Pending")
                {
                    ViewBag.IsEditingPending = true;
                }
            }

            // Lấy danh sách ngân hàng thụ hưởng từ CSDL
            ViewBag.Banks = await _context.Banks
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ThenBy(b => b.ShortName)
                .ToListAsync();

            return View(model);
        }

        // POST: /CarOwner/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CarOwnerRegistrationVM model)
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "", ReturnUrl = "/CarOwner/Register" });
            }

            var customerId = Function.GetAccountId(HttpContext.Session);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (customer == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Banks = await _context.Banks
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.DisplayOrder)
                    .ThenBy(b => b.ShortName)
                    .ToListAsync();
                return View(model);
            }

            // Xử lý upload ảnh (nếu có)
            string idCardFrontPath = model.ExistingIdCardFrontImage ?? string.Empty;
            string idCardBackPath = model.ExistingIdCardBackImage ?? string.Empty;
            string licenseImagePath = model.ExistingLicenseImage ?? string.Empty;

            if (model.IdCardFrontFile != null && model.IdCardFrontFile.Length > 0)
            {
                idCardFrontPath = Function.UploadImage(model.IdCardFrontFile, "KYC");
            }

            if (model.IdCardBackFile != null && model.IdCardBackFile.Length > 0)
            {
                idCardBackPath = Function.UploadImage(model.IdCardBackFile, "KYC");
            }

            if (model.LicenseImageFile != null && model.LicenseImageFile.Length > 0)
            {
                licenseImagePath = Function.UploadImage(model.LicenseImageFile, "KYC");
            }

            // Kiểm tra xem đã có bản ghi cũ chưa để cập nhật hoặc tạo mới
            var registration = await _context.CarOwnerRegistrations
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => r.CustomerId == customerId || r.Email == customer.Email);

            if (registration == null)
            {
                registration = new CarOwnerRegistration
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.Now
                };
                _context.CarOwnerRegistrations.Add(registration);
            }

            // Cập nhật thông tin vào CarOwnerRegistration
            registration.FullName = model.FullName.Trim();
            registration.Phone = model.Phone.Trim();
            registration.Email = model.Email.Trim();
            registration.Address = model.Address?.Trim();
            registration.IdCard = model.IdCard.Trim();
            registration.IdCardIssueDate = model.IdCardIssueDate;
            registration.IdCardIssuePlace = model.IdCardIssuePlace?.Trim();
            registration.IdCardFrontImage = idCardFrontPath;
            registration.IdCardBackImage = idCardBackPath;
            registration.LicenseNumber = model.LicenseNumber.Trim();
            registration.LicenseClass = model.LicenseClass?.Trim() ?? "B2";
            registration.LicenseExpiryDate = model.LicenseExpiryDate;
            registration.LicenseImage = licenseImagePath;
            registration.BankName = model.BankName.Trim();
            registration.AccountNumber = model.AccountNumber.Trim();
            registration.AccountHolder = model.AccountHolder.Trim().ToUpper();
            registration.Status = "Pending"; // Đặt về trạng thái Chờ duyệt
            registration.RejectReason = null;
            registration.CreatedAt = DateTime.Now;

            // Đồng bộ cập nhật thông tin định danh vào bảng Customers để dữ liệu profile khách hàng luôn đầy đủ
            customer.FullName = model.FullName.Trim();
            customer.PhoneNumber = model.Phone.Trim();
            customer.Address = model.Address?.Trim();
            customer.Idcard = model.IdCard.Trim();
            customer.LicenseNumber = model.LicenseNumber.Trim();
            if (!string.IsNullOrEmpty(licenseImagePath))
            {
                customer.LicenseImage = licenseImagePath;
            }
            customer.Bank = model.BankName.Trim();
            customer.AccountNumber = model.AccountNumber.Trim();
            customer.CompanyName = model.AccountHolder.Trim().ToUpper(); // Tên chủ tài khoản

            await _context.SaveChangesAsync();

            // Gửi thông báo Real-time cho Admin Portal
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNewOwnerRegistrationNotification", new
                {
                    registrationId = registration.Id,
                    fullName = registration.FullName,
                    phone = registration.Phone,
                    email = registration.Email,
                    bankName = registration.BankName,
                    time = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
                });
            }
            catch { }

            return RedirectToAction("Pending");
        }

        // GET: /CarOwner/Register/Pending (Hồ sơ pháp lý & Ngân hàng)
        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var accountId = Function.GetAccountId(HttpContext.Session);
            var carOwnerId = Function.GetCarOwnerAccountId(HttpContext.Session);
            var userEmail = Function.GetEmail(HttpContext.Session);

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => (carOwnerId > 0 && a.AccountId == carOwnerId) ||
                                          a.AccountId == accountId ||
                                          (!string.IsNullOrEmpty(userEmail) && a.Email == userEmail && a.RoleId == 3));

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == accountId || (!string.IsNullOrEmpty(userEmail) && c.Email == userEmail));

            // Tìm hồ sơ đăng ký KYC gần nhất
            var registration = await _context.CarOwnerRegistrations
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => (customer != null && r.CustomerId == customer.CustomerId) ||
                                          (!string.IsNullOrEmpty(userEmail) && r.Email == userEmail));

            // Nếu là chủ xe đã có tài khoản RoleId 3 nhưng chưa có bản ghi đăng ký, tự tạo một bản ghi Approved mô phỏng
            if (registration == null && account != null && account.RoleId == 3)
            {
                registration = new CarOwnerRegistration
                {
                    CustomerId = customer?.CustomerId ?? account.AccountId,
                    FullName = account.FullName ?? "Chủ xe đối tác",
                    Phone = account.Phone ?? customer?.PhoneNumber ?? "0900000000",
                    Email = account.Email ?? userEmail ?? "partner@recar.vn",
                    IdCard = customer?.Idcard ?? "079204008921",
                    LicenseNumber = customer?.LicenseNumber ?? "790158291042",
                    LicenseClass = "B2",
                    BankName = customer?.Bank ?? "Vietcombank",
                    AccountNumber = customer?.AccountNumber ?? "9876543210",
                    AccountHolder = customer?.CompanyName ?? account.FullName?.ToUpper() ?? "NGUYEN VAN A",
                    Status = "Approved",
                    CreatedAt = DateTime.Now.AddDays(-10),
                    ApprovedAt = DateTime.Now.AddDays(-8),
                    ApprovedBy = "Admin"
                };
            }
            else if (registration == null)
            {
                return RedirectToAction("Index");
            }

            // Lấy danh sách ngân hàng gợi ý
            ViewBag.Banks = await _context.Banks.Where(b => b.IsActive).OrderBy(b => b.DisplayOrder).ToListAsync();
            ViewBag.IsRejectedView = false;

            return View(registration);
        }

        // GET: /CarOwner/Register/Rejected (Xem lý do từ chối & cập nhật lại hồ sơ)
        [HttpGet]
        public async Task<IActionResult> Rejected()
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var accountId = Function.GetAccountId(HttpContext.Session);
            var userEmail = Function.GetEmail(HttpContext.Session);

            var registration = await _context.CarOwnerRegistrations
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => r.CustomerId == accountId || (!string.IsNullOrEmpty(userEmail) && r.Email == userEmail));

            if (registration == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Banks = await _context.Banks.Where(b => b.IsActive).OrderBy(b => b.DisplayOrder).ToListAsync();
            ViewBag.IsRejectedView = true;

            return View("Pending", registration);
        }

        // POST: /CarOwner/Register/UpdateBank (Cập nhật thông tin tài khoản nhận tiền)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBank(string bankName, string accountNumber, string accountHolder, string? mode)
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện thao tác" });
            }

            if (string.IsNullOrWhiteSpace(bankName) || string.IsNullOrWhiteSpace(accountNumber) || string.IsNullOrWhiteSpace(accountHolder))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ Tên ngân hàng, Số tài khoản và Tên chủ tài khoản." });
            }

            var accountId = Function.GetAccountId(HttpContext.Session);
            var userEmail = Function.GetEmail(HttpContext.Session);

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == accountId || (!string.IsNullOrEmpty(userEmail) && c.Email == userEmail));

            var registration = await _context.CarOwnerRegistrations
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => (customer != null && r.CustomerId == customer.CustomerId) || (!string.IsNullOrEmpty(userEmail) && r.Email == userEmail));

            if (registration == null)
            {
                registration = new CarOwnerRegistration
                {
                    CustomerId = customer?.CustomerId ?? accountId,
                    FullName = customer?.FullName ?? "Chủ xe",
                    Phone = customer?.PhoneNumber ?? "0900000000",
                    Email = userEmail ?? "partner@recar.vn",
                    IdCard = customer?.Idcard ?? "079204008921",
                    LicenseNumber = customer?.LicenseNumber ?? "790158291042",
                    Status = "Approved",
                    CreatedAt = DateTime.Now
                };
                _context.CarOwnerRegistrations.Add(registration);
            }

            registration.BankName = bankName.Trim();
            registration.AccountNumber = accountNumber.Trim();
            registration.AccountHolder = accountHolder.Trim().ToUpper();

            if (mode == "resubmit" || registration.Status == "Rejected")
            {
                registration.Status = "Pending";
                registration.RejectReason = null;
                registration.CreatedAt = DateTime.Now;
            }

            if (customer != null)
            {
                customer.Bank = bankName.Trim();
                customer.AccountNumber = accountNumber.Trim();
                customer.CompanyName = accountHolder.Trim().ToUpper();
            }

            await _context.SaveChangesAsync();

            string msg = (mode == "resubmit" || registration.Status == "Pending")
                ? "Đã gửi lại hồ sơ đối tác thành công — vui lòng chờ xét duyệt!"
                : "Đã cập nhật thông tin tài khoản ngân hàng thành công!";

            return Json(new { success = true, message = msg });
        }
    }
}
