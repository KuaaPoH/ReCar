using CarRental.Models;
using CarRental.Utilities;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class AccountsController : Controller
    {
        private readonly DbRenalCarContext _context;
        public AccountsController(DbRenalCarContext context)
        {
            _context = context;
        }
        public IActionResult Index(string returnUrl = null)
        {
            //kiem tra trang thai dang nhap
            if (!Function.IsLogin(HttpContext.Session))
            {

                return RedirectToAction("Index", "Login", new { returnUrl = HttpContext.Request.Path });
            }
            // Lấy ID khách hàng từ session
            var customerId = Function.GetAccountId(HttpContext.Session);

            // Truy vấn thông tin khách hàng từ database
            var customer = _context.Customers
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home"); // Redirect nếu không tìm thấy khách hàng
            }
            // Lấy danh sách đơn hàng, đơn mới nhất hiển thị trước
            ViewBag.Order = _context.CarRentalOrders
                .Include(i => i.Status)
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.OrderDate)
                .ThenByDescending(p => p.OrderId)
                .ToList();
            
            // Kiểm tra xem customer đã có Account với RoleId = 3 (Chủ xe) chưa
            var existingCarOwnerAccount = _context.Accounts
                .FirstOrDefault(a => a.Email == customer.Email && a.RoleId == 3);
            ViewBag.ExistingCarOwnerAccount = existingCarOwnerAccount;
            
            // Lưu returnUrl vào ViewBag nếu có
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnUrl = returnUrl;
            }
            else
            {
                // Lấy từ session nếu không có trong query string
                ViewBag.ReturnUrl = HttpContext.Session.GetString("LicenseUpdateReturnUrl");
            }
            
            // Truyền dữ liệu khách hàng sang View
            return View(customer);

        }
        public IActionResult Logout()
        {
            //kiem tra trang thai dang nhap
            if (!Function.IsLogin(HttpContext.Session))
            {

                return RedirectToAction("Index", "Login");
            }
            Function.ClearSession(HttpContext.Session);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult AccountDetails()
        {
            if (!Function.IsLogin(HttpContext.Session))
            {

                return RedirectToAction("Index", "Login");
            }
            var customerId = Function.GetAccountId(HttpContext.Session);
            var customer = _context.Customers.FirstOrDefault(p => p.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        public IActionResult AccountDetails(Customer c)
        {
            if (!Function.IsLogin(HttpContext.Session))
            {

                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                var customerId = Function.GetAccountId(HttpContext.Session);
                var customer = _context.Customers.FirstOrDefault(p => p.CustomerId == customerId);

                if (customer == null)
                {
                    return NotFound();
                }

                customer.Name = c.Name;
                if (c.Email != customer.Email)
                {
                    // Kiểm tra xem email mới có tồn tại trong cơ sở dữ liệu không
                    var emailExists = _context.Customers.Any(p => p.Email == c.Email);
                    if (emailExists)
                    {
                        Function._MessageEmail = "Email đã tồn tại";
                        return View(customer); // Trả về View với dữ liệu để người dùng chỉnh sửa
                    }
                    else
                    {
                        customer.Email = c.Email;
                        Function._MessageEmail = string.Empty;
                    }
                }
                Function._MessageEmail = string.Empty;

                customer.Password = Function.MD5Password(c.Password);

                // Xử lý ảnh đại diện
                /*if (img != null && img.Length > 0)
                {
                    // Gọi hàm UploadImage để lưu ảnh và lấy tên file
                    var uploadedImagePath = Function.UploadImage(img, "Customer");
                    if (!string.IsNullOrEmpty(uploadedImagePath))
                    {
                        customer.Avartar = uploadedImagePath; // Lưu đường dẫn hoặc tên file vào thuộc tính Avartar
                    }
                }*/
                
                _context.SaveChanges();
                Function.SetUserName(HttpContext.Session, customer.Name);
                return RedirectToAction("Index", "Accounts");

            }


            return View(c);
        }
        [HttpGet]
        public IActionResult licenseDetails(string returnUrl = null)
        {
            if (!Function.IsLogin(HttpContext.Session))
            {

                return RedirectToAction("Index", "Login");
            }
            var customerId = Function.GetAccountId(HttpContext.Session);
            var customer = _context.Customers.FirstOrDefault(p => p.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound();
            }
            
            // Lưu returnUrl vào session nếu có
            if (!string.IsNullOrEmpty(returnUrl))
            {
                HttpContext.Session.SetString("LicenseUpdateReturnUrl", returnUrl);
            }
            else
            {
                // Lấy từ session nếu không có trong query string
                returnUrl = HttpContext.Session.GetString("LicenseUpdateReturnUrl");
            }
            
            ViewBag.ReturnUrl = returnUrl;

            return View(customer);
            
        }
        [HttpPost]
        [IgnoreAntiforgeryToken] // Bỏ qua anti-forgery token vì dùng AJAX
        public IActionResult licenseDetails(Customer c, IFormFile? img)
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để tiếp tục." });
            }
            
            // Kiểm tra dữ liệu đầu vào
            if (c == null)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }
            
            // Kiểm tra validation thủ công
            if (string.IsNullOrWhiteSpace(c.LicenseNumber))
            {
                return Json(new { success = false, message = "Vui lòng nhập số giấy phép lái xe." });
            }
            
            var customerId = Function.GetAccountId(HttpContext.Session);
            var customer = _context.Customers.FirstOrDefault(p => p.CustomerId == customerId);

            if (customer == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            customer.FullName = c.FullName;
            customer.DateofBirth = c.DateofBirth;
            customer.LicenseNumber = c.LicenseNumber;

            // Xử lý ảnh đại diện
            if (img != null && img.Length > 0)
            {
                // Gọi hàm UploadImage để lưu ảnh và lấy tên file
                var uploadedImagePath = Function.UploadImage(img, "license");
                if (!string.IsNullOrEmpty(uploadedImagePath))
                {
                    customer.LicenseImage = uploadedImagePath;
                }
            }
            else
            {
                customer.LicenseImage = customer.LicenseImage;
            }

            _context.SaveChanges();
            
            // Lấy returnUrl từ session
            var returnUrl = HttpContext.Session.GetString("LicenseUpdateReturnUrl");
            
            // Xóa returnUrl khỏi session sau khi sử dụng
            if (!string.IsNullOrEmpty(returnUrl))
            {
                HttpContext.Session.Remove("LicenseUpdateReturnUrl");
            }
            
            // Trả về JSON để hiển thị dialog
            return Json(new { 
                success = true, 
                message = "Đã cập nhật thông tin GPLX thành công!",
                returnUrl = returnUrl ?? Url.Action("Index", "Accounts")
            });
        }
        [HttpGet]
        public IActionResult editAdress(string returnUrl = null)
        {
            //kiem tra trang thai dang nhap
            if (!Function.IsLogin(HttpContext.Session))
            {

                return RedirectToAction("Index", "Login");
            }
            //lấy thông tin khách hàng từ DB
            var id = Function.GetAccountId(HttpContext.Session);
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return View();
            }
            
            // Lưu returnUrl vào session nếu có (từ query string hoặc session)
            if (!string.IsNullOrEmpty(returnUrl))
            {
                HttpContext.Session.SetString("AddressUpdateReturnUrl", returnUrl);
            }
            else
            {
                // Lấy từ session nếu không có trong query string
                returnUrl = HttpContext.Session.GetString("AddressUpdateReturnUrl");
            }
            
            //truyền thông  tin dếnd view
            var model = new EditAddressVM
            {
                CustomerID = customer.CustomerId,
                Address = customer.Address,
                Phone = customer.PhoneNumber,
            };
            
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] // Bỏ qua anti-forgery token vì dùng AJAX
        public IActionResult editAdress(EditAddressVM model)
        {
            //kiem tra trang thai dang nhap
            if (!Function.IsLogin(HttpContext.Session))
            {

                return Json(new { success = false, message = "Bạn cần đăng nhập để tiếp tục." });
            }
            // Kiểm tra dữ liệu đầu vào
            if (model == null)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }
            
            // Kiểm tra validation thủ công
            if (string.IsNullOrWhiteSpace(model.Address) || string.IsNullOrWhiteSpace(model.Phone))
            {
                return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin địa chỉ và số điện thoại." });
            }
            
            var id = Function.GetAccountId(HttpContext.Session);
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            //cập nhật địa chỉ và số điện thoại mới cho khách hàng
            customer.Address = model.Address;
            customer.PhoneNumber = model.Phone;

            _context.SaveChanges();
            Function.SetAddress(HttpContext.Session, customer.Address);
            Function.SetPhone(HttpContext.Session, customer.PhoneNumber);
            
            // Lấy returnUrl từ session
            var returnUrl = HttpContext.Session.GetString("AddressUpdateReturnUrl");
            
            // Xóa returnUrl khỏi session sau khi sử dụng
            if (!string.IsNullOrEmpty(returnUrl))
            {
                HttpContext.Session.Remove("AddressUpdateReturnUrl");
            }
            
            // Trả về JSON để hiển thị dialog
            return Json(new { 
                success = true, 
                message = "Đã cập nhật địa chỉ thành công!",
                returnUrl = returnUrl ?? Url.Action("Index", "Accounts")
            });
        }
        [HttpGet]
        public IActionResult OrderDetails(int id)
        {
            var order = _context.CarRentalOrders
                .Include(o => o.Status)
                .FirstOrDefault(p => p.OrderId == id);
            
            if (order == null)
            {
                return NotFound();
            }
            
            var orderItems = _context.OrderDetails
                .Where(od => od.OrderId == id)
                .Join(_context.Cars,
                        od => od.CarId,
                        c => c.CarId,
                        (od, c) => new OrderDetailVM
                        {
                            OrderDetailID = od.OrderId,
                            CarName = c.CarName,
                            Image = c.Image,
                            Quantity = (int)od.Quantity,
                            pickupDate = (DateTime)od.PickupDate,
                            returnDate = (DateTime)od.ReturnDate,

                        })
                .ToList();
            ViewBag.OrderId = id; // Lưu OrderId vào ViewBag
            ViewBag.StatusId = (int)(order.StatusId ?? 0); // Lưu StatusId vào ViewBag (StatusId = 2 là đã hủy)
            ViewBag.StatusDescription = order.Status?.StatusDescription ?? ""; // Lưu mô tả trạng thái
            ViewBag.Notes = order.Notes ?? "";
            ViewBag.Deposit = order.Deposit ?? 0;
            ViewBag.Payment = order.Payment ?? 0;
            return View(orderItems);
        }
        [HttpPost]
        public IActionResult CancelOrder(int orderId)
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            if (!Function.IsLogin(HttpContext.Session))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để tiếp tục." });
            }

            var customerId = Function.GetAccountId(HttpContext.Session);

            // Lấy thông tin đơn hàng từ cơ sở dữ liệu kèm OrderDetails và Status
            var order = _context.CarRentalOrders
                .Include(o => o.OrderDetails)
                .Include(o => o.Status)
                .FirstOrDefault(o => o.OrderId == orderId);

            // Kiểm tra nếu đơn hàng không tồn tại
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            // Kiểm tra xem đơn hàng có thuộc về khách hàng đang đăng nhập không
            if (order.CustomerId != customerId)
            {
                return Json(new { success = false, message = "Bạn không có quyền hủy đơn hàng này." });
            }

            // Kiểm tra nếu đơn hàng đã được hủy - kiểm tra cả StatusId và StatusDescription
            if (order.StatusId == 2 || 
                (order.Status != null && 
                 (order.Status.StatusDescription != null && 
                  (order.Status.StatusDescription.Contains("hủy", StringComparison.OrdinalIgnoreCase) || 
                   order.Status.StatusDescription.Contains("Huỷ") ||
                   order.Status.StatusDescription.Contains("huy", StringComparison.OrdinalIgnoreCase)))))
            {
                return Json(new { success = false, message = "Đơn hàng đã được hủy." });
            }

            // Cập nhật trạng thái đơn hàng thành "Đã hủy" (StatusId = 2)
            order.StatusId = 2;
            _context.CarRentalOrders.Update(order);

            // Cập nhật trạng thái IsActive = true cho tất cả các xe trong đơn hàng để có thể thuê tiếp
            foreach (var detail in order.OrderDetails)
            {
                var car = _context.Cars.FirstOrDefault(c => c.CarId == detail.CarId);
                if (car != null)
                {
                    car.IsActive = true;
                    _context.Cars.Update(car);
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Trả về phản hồi thành công
            return Json(new { success = true, message = "Đơn hàng đã được hủy. Các xe đã được cập nhật về trạng thái có thể thuê." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCarOwner()
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            var customerId = Function.GetAccountId(HttpContext.Session);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy thông tin khách hàng.");
                return RedirectToAction("Index");
            }

            // Kiểm tra giấy phép lái xe
            if (string.IsNullOrEmpty(customer.LicenseNumber))
            {
                Function.SetMessage(HttpContext.Session, "Bạn cần cập nhật Giấy phép lái xe trước khi đăng ký làm chủ xe. Vui lòng vào tab 'Giấy phép lái xe' để cập nhật.");
                return RedirectToAction("Index");
            }

            // Kiểm tra xem đã có tài khoản chủ xe chưa
            var existingCarOwnerAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == customer.Email && a.RoleId == 3);

            if (existingCarOwnerAccount != null)
            {
                Function.SetMessage(HttpContext.Session, "Bạn đã là chủ xe rồi!");
                return RedirectToAction("Index");
            }

            // Tạo username tự động từ email hoặc tên
            string username = !string.IsNullOrEmpty(customer.Email) 
                ? customer.Email.Split('@')[0] 
                : (!string.IsNullOrEmpty(customer.Name) ? customer.Name.Replace(" ", "").ToLower() : "carowner" + customerId);
            
            // Đảm bảo username là duy nhất
            int counter = 1;
            string originalUsername = username;
            while (await _context.Accounts.AnyAsync(a => a.Username == username))
            {
                username = originalUsername + counter.ToString();
                counter++;
            }

            // Tạo mật khẩu mặc định từ password của customer (hoặc tạo mới)
            string password = !string.IsNullOrEmpty(customer.Password) 
                ? customer.Password 
                : Function.MD5Password("123456"); // Mật khẩu mặc định

            // Tạo tài khoản chủ xe mới
            var newCarOwnerAccount = new Account
            {
                Username = username,
                Password = password, // Password đã được mã hóa từ Customer
                Email = customer.Email,
                FullName = !string.IsNullOrEmpty(customer.FullName) ? customer.FullName : customer.Name,
                Phone = customer.PhoneNumber,
                RoleId = 3, // Chủ xe
                IsActive = true,
                LastLogin = DateTime.Now.ToString("yyyy-MM-dd")
            };

            _context.Accounts.Add(newCarOwnerAccount);
            await _context.SaveChangesAsync();

            // Tự động đăng nhập vào tài khoản chủ xe - lưu vào Session
            Function.SetAccountId(HttpContext.Session, newCarOwnerAccount.AccountId);
            Function.SetCarOwnerAccountId(HttpContext.Session, newCarOwnerAccount.AccountId);
            Function.SetUserName(HttpContext.Session, string.IsNullOrEmpty(newCarOwnerAccount.Username) ? string.Empty : newCarOwnerAccount.Username);
            Function.SetEmail(HttpContext.Session, string.IsNullOrEmpty(newCarOwnerAccount.Email) ? string.Empty : newCarOwnerAccount.Email);
            Function.SetMessage(HttpContext.Session, "Đăng ký chủ xe thành công! Chào mừng bạn đến với trang quản lý xe.");

            // Redirect đến trang CarOwner
            return RedirectToAction("Index", "Home", new { area = "CarOwner" });
        }

        [HttpGet]
        public IActionResult GoToCarOwner()
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            // Kiểm tra xem Customer có Account với RoleId=3 không
            var customerId = Function.GetAccountId(HttpContext.Session);
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
            {
                Function.SetMessage(HttpContext.Session, "Không tìm thấy thông tin khách hàng.");
                return RedirectToAction("Index");
            }

            var carOwnerAccount = _context.Accounts
                .FirstOrDefault(a => a.Email == customer.Email && a.RoleId == 3);

            if (carOwnerAccount == null)
            {
                Function.SetMessage(HttpContext.Session, "Bạn chưa đăng ký làm chủ xe.");
                return RedirectToAction("Index");
            }

            // Chuyển sang đăng nhập với tài khoản chủ xe - lưu vào Session
            Function.SetAccountId(HttpContext.Session, carOwnerAccount.AccountId);
            Function.SetUserName(HttpContext.Session, string.IsNullOrEmpty(carOwnerAccount.Username) ? string.Empty : carOwnerAccount.Username);
            Function.SetEmail(HttpContext.Session, string.IsNullOrEmpty(carOwnerAccount.Email) ? string.Empty : carOwnerAccount.Email);
            Function.SetCarOwnerAccountId(HttpContext.Session, carOwnerAccount.AccountId);

            return RedirectToAction("Index", "Home", new { area = "CarOwner" });
        }

    }
}
