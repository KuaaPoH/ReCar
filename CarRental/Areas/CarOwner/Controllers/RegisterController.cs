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

        // GET: /CarOwner/Register/Pending (Màn hình chờ xét duyệt)
        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var customerId = Function.GetAccountId(HttpContext.Session);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (customer == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            var registration = await _context.CarOwnerRegistrations
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => r.CustomerId == customerId || r.Email == customer.Email);

            // Nếu chưa có hồ sơ nào thì đẩy về form đăng ký
            if (registration == null)
            {
                return RedirectToAction("Index");
            }

            // Nếu đã được duyệt rồi thì dẫn thẳng vào trang chủ xe
            if (registration.Status == "Approved")
            {
                return RedirectToAction("Index", "Home");
            }

            // Nếu bị từ chối thì dẫn về trang thông báo từ chối
            if (registration.Status == "Rejected")
            {
                return RedirectToAction("Rejected");
            }

            return View(registration);
        }

        // GET: /CarOwner/Register/Rejected (Màn hình thông báo hồ sơ bị từ chối)
        [HttpGet]
        public async Task<IActionResult> Rejected()
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var customerId = Function.GetAccountId(HttpContext.Session);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (customer == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            var registration = await _context.CarOwnerRegistrations
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => r.CustomerId == customerId || r.Email == customer.Email);

            if (registration == null || registration.Status != "Rejected")
            {
                return RedirectToAction("Index");
            }

            return View(registration);
        }
    }
}
