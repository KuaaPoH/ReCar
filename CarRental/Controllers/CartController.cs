using CarRental.Extensions;

using CarRental.Models;

using CarRental.Services;

using CarRental.Utilities;

using CarRental.ViewModels;

using CarRental.Hubs;

using Humanizer.Localisation;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.SignalR;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Microsoft.EntityFrameworkCore;



namespace CarRental.Controllers

{

 public class CartController : Controller

 {

        private readonly DbRenalCarContext _context;
        private readonly IConfiguration _config;
        private readonly IHubContext<ReCarHub> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly IZaloPayService _zaloPayService;

        public CartController(DbRenalCarContext context, IConfiguration config, IHubContext<ReCarHub> hubContext, INotificationService notificationService, IZaloPayService zaloPayService)
        {
            _context = context;
            _config = config;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _zaloPayService = zaloPayService;
        }

 string CART_KEY = "MYCART";

 public List<CartItemsVM> CART => HttpContext.Session.Get<List<CartItemsVM>>(CART_KEY) ?? new List<CartItemsVM> ();

 public IActionResult Index()

 {

 

 return View(CART);

 }

 [HttpPost]

 public IActionResult AddToCart(int id, DateTime pickupDate, DateTime returnDate, int quantity = 1, string type = "Normal", string driveMode = "Tự lái", string deliveryMode = "Nhận tại địa chỉ xe")

 {

 var cart = CART;

 // l?y thng tin s?n ph?m t? DB

 var product = _context.Cars.FirstOrDefault(p => p.CarId == id);



 // X? l? thm s?n ph?m vo gi? hng

 int rentalDays = (returnDate - pickupDate).Days;

 if (rentalDays <= 0) rentalDays = 1;



 if (product == null || !product.IsActive || product.CarStatus != "Sẵn sàng") {

 TempData["Error"] = "Xe này hiện chưa sẵn sàng đón khách hoặc đang cập nhật thông tin trên sàn ReCar, quý khách vui lòng chọn xe khác.";

 return RedirectToAction("Index", "Product");

 }

 if (false) {

 return NotFound();// n?u khng t?m th?y sp

 }

 decimal finalPrice;

 if (product.IsSale && product.SalePrice.HasValue && product.SalePrice > 0 && product.SalePrice < product.Price)

 {

 finalPrice = product.SalePrice.Value;

 }

 else 

 {

 finalPrice = product.Price ?? 0;

 }



 bool withDriver = (driveMode == "Có tài xế" || driveMode == "Kèm tài xế");

 var carPolicy = CarPolicy.Parse(product);
            decimal driverFeeTotal = withDriver ? (carPolicy.DriverFee * rentalDays) : 0m;
            bool withDelivery = carPolicy.DeliverySupported && (deliveryMode == "Giao xe tận nơi");
            decimal deliveryFeeTotal = withDelivery ? carPolicy.DeliveryFee : 0m;
            string normalizedDriveMode = withDriver ? "Có tài xế" : "Tự lái";
            string normalizedDeliveryMode = withDelivery ? "Giao xe tận nơi" : "Nhận tại địa chỉ xe";



 // ki?m tra xem c s?n ph?m t?n tai trong gi? hng hay cha

 var CarItem = cart.FirstOrDefault(p => p.CartId == id);

 if (CarItem == null)

 {

 // n?u cha c s?n ph?m th? kh?i t?o s?n ph?m m?i

 CarItem = new CartItemsVM

 {

 CartId = product.CarId,

 CarName = product.CarName,

 Image = product.Image,

 pickupDate = pickupDate,

 returnDate = returnDate,

 Price = finalPrice * rentalDays,

 Quantity = quantity,

 DriveMode = normalizedDriveMode,

 DriverFee = driverFeeTotal,

 DeliveryMode = normalizedDeliveryMode,

 DeliveryFee = deliveryFeeTotal

 };

 cart.Add(CarItem);

 } else

 {

 // nếu đã có thì cập nhật lại

 CarItem.Quantity = quantity;

 CarItem.pickupDate = pickupDate;

 CarItem.returnDate = returnDate;

 CarItem.DriveMode = normalizedDriveMode;

 CarItem.DriverFee = driverFeeTotal;

 CarItem.DeliveryMode = normalizedDeliveryMode;

 CarItem.DeliveryFee = deliveryFeeTotal;

 CarItem.Price = finalPrice * rentalDays * CarItem.Quantity;

 }

 HttpContext.Session.Set(CART_KEY, cart);



 if (type == "ajax") 

 {

 return Json( new {

 quantity = cart.Sum(p => p.Quantity)

 });

 }

 ViewBag.MiniCart = cart;

 Function._pickupDate = pickupDate.ToString("dd-MM-yyyy");

 Function._returnDate = returnDate.ToString("dd-MM-yyyy");

 return RedirectToAction("Index");

 }

 public IActionResult CheckoutSummary()

 {

 var cart = CART; // L?y gi? hng t? Session

 if (cart == null || !cart.Any())

 {

 return RedirectToAction("Index", "Cart"); // Chuy?n h?ng n?u gi? hng tr?ng

 }



 // Tnh t?ng ti?n thu xe

 decimal total = cart.Sum(item => item.PriceTotal);

 



 var sysSetting = _context.SystemSettings.FirstOrDefault();

 decimal depositRate = (sysSetting?.DepositPct ?? 30) / 100m;

 decimal deposit = Math.Round(total * depositRate, 0); // Ti?n ?t c?c theo c?u h?nh sn

 ViewBag.DepositPct = sysSetting?.DepositPct ?? 30;

            decimal payment = total - deposit; // Số tiền còn lại khi nhận xe

            var accId = Function.GetAccountId(HttpContext.Session);
            var currentCustomer = _context.Customers.FirstOrDefault(c => c.CustomerId == accId);
            ViewBag.CustomerBalance = currentCustomer?.Balance ?? 0m;



 // T?o ViewModel

 var model = new CartVM

 {

 Total = total,

 Deposit = deposit,

 Payment = payment

 };



 return View(model);

 }



 [HttpPost]

 public IActionResult UpdateCart(List<CartUpdateVM> updatedCart)

 {

 // Gi? s? b?n c gi? hng trong session

 var cart = CART; // L?y gi? hng t? session ho?c c s? d? li?u



 foreach (var updatedItem in updatedCart)

 {

 var cartItem = cart.FirstOrDefault(item => item.CartId == updatedItem.CartId);

 if (cartItem != null)

 {

 // C?p nh?t s? l?ng v tnh l?i gi

 cartItem.Quantity = updatedItem.Quantity;



 // Tnh l?i gi thu xe d?a trn s? ngy thu v s? l?ng

 int rentalDays = (cartItem.returnDate - cartItem.pickupDate).Days;

 cartItem.Price = cartItem.Price * rentalDays * cartItem.Quantity;

 }

 }



 // C?p nh?t t?ng gi tr? gi? hng

 /*cart.UpdateTotalPrice();*/



 // Lu gi? hng l?i vo session

 HttpContext.Session.Set(CART_KEY, cart);



 // Tr? v? t?ng gi tr? gi? hng cho client

 return Json(new { totalPrice = cart.Sum(p=>p.PriceTotal) });

 }



 public IActionResult removeCart(int id)

 {

 var cart = CART;

 var items = cart.FirstOrDefault(p => p.CartId == id);

 if(items != null)

 {

 cart.Remove(items);

 HttpContext.Session.Set(CART_KEY, cart);

 }

 return RedirectToAction("Index");

 }

 [HttpGet]

 [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
 public IActionResult Checkout()

 {

 //kiem tra trang thai dang nhap

 if (!Function.IsLogin(HttpContext.Session))

 {



 return RedirectToAction("Index", "Login", new { ReturnUrl = Url.Action("Checkout", "Cart") });

 }

 var cart = CART; // Lấy giỏ hàng từ Session

 if(cart == null || !cart.Any())

 {
     Function.CancelExpiredPendingOrders(_context);
     var pendingOrderId = HttpContext.Session.GetInt32("PendingZaloPayOrderId");

     HttpContext.Session.Remove("PendingZaloPayOrderId");
     HttpContext.Session.Remove("PendingZaloPayOrderUrl");
     HttpContext.Session.Remove("PendingZaloPayCart");

     if (pendingOrderId.HasValue && pendingOrderId.Value > 0)
     {
         return RedirectToAction("Index", "Accounts", new { tab = "orders" });
     }

     return RedirectToAction("Index", "Cart"); // Chuyển hướng nếu giỏ hàng trống
 }

 // Tnh t?ng ti?n thu xe

 decimal total = cart.Sum(item => item.PriceTotal);





 var sysSetting = _context.SystemSettings.FirstOrDefault();

 decimal depositRate = (sysSetting?.DepositPct ?? 30) / 100m;

 decimal deposit = Math.Round(total * depositRate, 0); // Ti?n ?t c?c theo c?u h?nh sn

 ViewBag.DepositPct = sysSetting?.DepositPct ?? 30;

            decimal payment = total - deposit; // Số tiền còn lại khi nhận xe

            var accId = Function.GetAccountId(HttpContext.Session);
            var currentCustomer = _context.Customers.FirstOrDefault(c => c.CustomerId == accId);
            ViewBag.CustomerBalance = currentCustomer?.Balance ?? 0m;



 // T?o ViewModel

 var model = new CartVM

 {

 Total = total,

 Deposit = deposit,

 Payment = payment

 };

 return View(model);

 }

 [HttpPost]

 public async Task<IActionResult> Checkout([FromBody] CheckoutVM model)

 {

 if (!Function.IsLogin(HttpContext.Session))

 {

 return Json(new { success = false, message = "Bạn cần đăng nhập để tiếp tục." });

 

 }



 if (ModelState.IsValid)

 {

 





 var cart = CART;

 if (cart == null || !cart.Any())
 {
     var pendingOrderId = HttpContext.Session.GetInt32("PendingZaloPayOrderId");
     var pendingUrl = HttpContext.Session.GetString("PendingZaloPayOrderUrl");
     if (pendingOrderId.HasValue && pendingOrderId.Value > 0 && !string.IsNullOrEmpty(pendingUrl))
     {
         var pendingOrder = _context.CarRentalOrders.FirstOrDefault(o => o.OrderId == pendingOrderId.Value && o.StatusId == 1);
         if (pendingOrder != null)
         {
             return Json(new
             {
                 success = false,
                 pendingZaloPay = true,
                 orderId = pendingOrder.OrderId,
                 paymentUrl = pendingUrl,
                 ordersUrl = Url.Action("Index", "Accounts") + "?tab=orders",
                 message = $"Đơn thuê xe #RC-{pendingOrder.OrderId} đã được tạo và đang chờ thanh toán ZaloPay."
             });
         }
     }

     return Json(new
     {
         success = false,
         emptyCart = true,
         message = "Giỏ xe hiện đang trống hoặc đơn hàng đã được tạo trước đó. Vui lòng kiểm tra trong mục Đơn thuê của tôi.",
         redirectUrl = Url.Action("Index", "Accounts") + "?tab=orders"
     });
 }

 var customerID = Function.GetAccountId(HttpContext.Session);

 var userEmail = Function.GetEmail(HttpContext.Session);

 var customer = _context.Customers.FirstOrDefault(c => 

 c.CustomerId == customerID || 

 (!string.IsNullOrEmpty(userEmail) && c.Email == userEmail));



 if (customer == null)

 {

 return Json(new { success = false, message = "Không tìm thấy khách hàng." });

 }



 // Lun ?ng b? customerID chnh xc t? b?ng Customers

 customerID = customer.CustomerId;

 Function.SetAccountId(HttpContext.Session, customer.CustomerId);

 ViewBag.Order = _context.CarRentalOrders.Include(i => i.Status).Where(p => p.CustomerId == customerID).ToList();



 // Ki?m tra ?a ch?

 if (string.IsNullOrWhiteSpace(customer.Address))

 {

 // Lu returnUrl vo session ? quay l?i sau khi c?p nh?t ?a ch?

 var returnUrl = Url.Action("Checkout", "Cart");

 HttpContext.Session.SetString("AddressUpdateReturnUrl", returnUrl);

 

 return Json(new { 

 success = false, 

 message = "Vui lòng cập nhật địa chỉ nhận xe.", 

 needAddress = true,

 redirectUrl = Url.Action("editAdress", "Accounts", new { returnUrl = returnUrl })

 });

 }

 

 // Ki?m tra GPLX (Gi?y php li xe)

 if (string.IsNullOrWhiteSpace(customer.LicenseNumber))

 {

 // Lu returnUrl vo session ? quay l?i sau khi c?p nh?t GPLX

 var returnUrl = Url.Action("Checkout", "Cart");

 HttpContext.Session.SetString("LicenseUpdateReturnUrl", returnUrl);

 

 // Redirect ?n My Account v?i tab GPLX v returnUrl trong query string

 var accountsUrl = Url.Action("Index", "Accounts", new { returnUrl = returnUrl }) + "#download";

 

 return Json(new { 

 success = false, 

 message = "Vui lòng cập nhật thông tin Giấy phép lái xe trước khi đặt xe.", 

 needLicense = true,

 redirectUrl = accountsUrl

 });

 }





 

 // Ki?m tra l?ch ch?n xe c?a Ch? xe (CarCalendarBlocks)

 try

 {

 foreach (var item in cart)

 {

 var blockedSlot = _context.CarCalendarBlocks

 .FirstOrDefault(b => b.CarId == item.CartId && item.pickupDate < b.EndTime && item.returnDate > b.StartTime);

 if (blockedSlot != null)

 {

 return Json(new

 {

 success = false,

 message = $"Xe {item.CarName} đã được chủ xe khóa lịch ({blockedSlot.StartTime:dd/MM/yyyy} – {blockedSlot.EndTime:dd/MM/yyyy}). Vui lòng chọn khung giờ khác."

 });

 }

 }

 }

 catch

 {

 // B? qua n?u b?ng l?ch ch?n cha kh?i t?o

 }



            decimal price = cart.Sum(p => p.PriceTotal);

            if (price <= 0)
            {
                return Json(new
                {
                    success = false,
                    emptyCart = true,
                    message = "Giá trị đơn thuê không hợp lệ hoặc giỏ xe trống.",
                    redirectUrl = Url.Action("Index", "Cart")
                });
            }

            bool isWallet = model.paymentmethod == "wallet_deposit" || model.paymentmethod == "wallet_full";
            bool isZaloPay = model.paymentmethod == "zalopay_deposit" || model.paymentmethod == "zalopay_full";
            bool isFullPayment = model.paymentmethod == "full_100" ||
                model.paymentmethod == "wallet_full" ||
                model.paymentmethod == "zalopay_full" ||
                model.paymentmethod == "Thanh toán 100%" ||
                model.paymentmethod == "Thanh toán toàn bộ 100%";

            var sysSetting = _context.SystemSettings.FirstOrDefault();
            int currentCommissionPct = sysSetting?.CommissionPct ?? 10;
            int currentDepositPct = sysSetting?.DepositPct ?? 30;

            decimal deposit = isFullPayment ? price : Math.Round((price * (decimal)currentDepositPct) / 100m, 0);
            decimal payments = isFullPayment ? 0 : (price - deposit);
            decimal platformFee = Math.Round((price * (decimal)currentCommissionPct) / 100m, 0);

            var payingCustomer = isWallet ? _context.Customers.FirstOrDefault(c => c.CustomerId == customerID) : null;
            if (isWallet)
            {
                if (payingCustomer == null || payingCustomer.Balance < deposit)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Số dư Ví ReCar không đủ ({(payingCustomer?.Balance ?? 0m):N0} đ) để thanh toán khoản {deposit:N0} đ. Vui lòng chọn thanh toán qua ZaloPay."
                    });
                }
            }

            var driveModeSummary = string.Join("; ", cart.Select(c =>
                $"Hình thức: {c.DriveMode}" +
                (c.DriverFee > 0 ? $" (Phụ phí tài xế: {c.DriverFee:N0}đ)" : "") +
                $" | Nhận xe: {c.DeliveryMode}" +
                (c.DeliveryMode == "Giao xe tận nơi" ? (c.DeliveryFee > 0 ? $" (Phí giao xe: {c.DeliveryFee:N0}đ)" : " (Miễn phí)") : "")));
            string paymentChannel = isWallet ? "Ví ReCar" : (isZaloPay ? "ZaloPay" : "COD");
            string paymentPlanText = isFullPayment ? $"Gói thanh toán: Trọn gói 100% qua {paymentChannel}" : $"Gói thanh toán: Đặt cọc {currentDepositPct}% qua {paymentChannel}";

 string nowVnTime = Function.NowVn.ToString("dd/MM/yyyy HH:mm");

 string combinedNotes = $"[Tạo đơn {nowVnTime}] | " + driveModeSummary + $" | [{paymentPlanText}]" + (!string.IsNullOrWhiteSpace(model.Notes) ? $" | Ghi chú khách: {model.Notes}" : "");



 var order = new CarRentalOrder

 {

 CustomerId = customerID,

 OrderDate = DateOnly.FromDateTime(Function.NowVn),

 Deposit = deposit,

 StatusId = 1,

 Payment = payments,

 PlatformFeeRate = currentCommissionPct,

 PlatformFee = platformFee,

 Notes = combinedNotes

 };



 







 using (var transaction = _context.Database.BeginTransaction())

 {

 try

 {

 _context.Add(order);

 _context.SaveChanges();



 var orderDetails = cart.Select(item => new OrderDetail

 {

 

 OrderId = order.OrderId,

 CarId = item.CartId,

 Price = item.PriceTotal,

 Quantity = item.Quantity,

 PickupDate = item.pickupDate,

 ReturnDate = item.returnDate,



 }).ToList();



 



 _context.AddRange(orderDetails);

 _context.SaveChanges();

 

 // C?p nh?t tr?ng thi IsActive c?a xe

 var carIds = cart.Select(item => item.CartId).ToList(); // L?y danh sch ID c?a xe trong gi? hng

 var carsToUpdate = _context.Cars.Where(c => carIds.Contains(c.CarId)).ToList(); // L?y danh sch xe t? DB

 foreach (var car in carsToUpdate)

 {

 car.IsActive = false; // ?t thu?c tnh IsActive thnh false

 }

                        if (isWallet && payingCustomer != null)
                        {
                            payingCustomer.Balance -= deposit;
                            order.StatusId = 3;
                            string walletLog = isFullPayment
                                ? $"[Thanh toán {Function.NowVn:dd/MM/yyyy HH:mm}] | Đã thanh toán trọn gói 100% ({deposit:N0} đ) bằng Số dư Ví ReCar"
                                : $"[Thanh toán {Function.NowVn:dd/MM/yyyy HH:mm}] | Đã thanh toán đặt cọc {currentDepositPct}% ({deposit:N0} đ) bằng Số dư Ví ReCar";
                            order.Notes = (order.Notes ?? "") + " | " + walletLog;

                            _context.WalletTransactions.Add(new WalletTransaction
                            {
                                CustomerId = payingCustomer.CustomerId,
                                OrderId = order.OrderId,
                                Type = "payment",
                                Amount = deposit,
                                BalanceAfter = payingCustomer.Balance,
                                Description = isFullPayment
                                    ? $"Thanh toán trọn gói 100% đơn #{order.OrderId} bằng Ví ReCar"
                                    : $"Thanh toán đặt cọc {currentDepositPct}% đơn #{order.OrderId} bằng Ví ReCar",
                                Status = "completed",
                                CreatedAt = Function.NowVn,
                                ProcessedAt = Function.NowVn
                            });
                        }

                        _context.SaveChanges(); // Lưu thay đổi

                        HttpContext.Session.Set<List<CartItemsVM>>(CART_KEY, new List<CartItemsVM>());

                        transaction.Commit();

                        if (isWallet)
                        {
                            await SendOrderNotificationAsync(order.OrderId);
                            var ordersUrlWallet = Url.Action("Index", "Accounts") + "?tab=orders";
                            return Json(new
                            {
                                success = true,
                                redirectUrl = ordersUrlWallet,
                                isVnpay = false,
                                title = isFullPayment ? "Thanh toán trọn gói 100% thành công!" : $"Đặt cọc {currentDepositPct}% bằng Ví ReCar thành công!",
                                message = isFullPayment
                                    ? $"Đã trừ {deposit:N0} đ từ Số dư Ví ReCar để thanh toán trọn gói 100%. Đơn hàng đã được xác nhận!"
                                    : $"Đã trừ {deposit:N0} đ tiền cọc từ Số dư Ví ReCar. Phần còn lại {payments:N0} đ thanh toán khi nhận xe."
                            });
                        }

                        bool isZaloPayOrder = model.paymentmethod == "zalopay_deposit" || model.paymentmethod == "zalopay_full";
                        if (isZaloPayOrder)
                        {
                            var zpUser = Function.GetUserName(HttpContext.Session);
                            var zpDesc = isFullPayment
                                ? $"ReCar - Thanh toan tron goi 100% don #{order.OrderId}"
                                : $"ReCar - Dat coc {currentDepositPct}% don #{order.OrderId}";
                            var zpResult = await _zaloPayService.CreateOrderAsync(HttpContext, order.OrderId, (long)deposit, zpUser, zpDesc);
                            if (zpResult.Success && !string.IsNullOrEmpty(zpResult.OrderUrl))
                            {
                                var expireAt = Function.NowVn.AddMinutes(1);
                                order.Notes = (order.Notes ?? "") + $" | [ZaloPayPending:{expireAt:yyyyMMddHHmmss}] | [ZaloPayUrl:{zpResult.OrderUrl}]";
                                _context.SaveChanges();

                                HttpContext.Session.SetInt32("PendingZaloPayOrderId", order.OrderId);
                                HttpContext.Session.SetString("PendingZaloPayOrderUrl", zpResult.OrderUrl);
                                HttpContext.Session.Set("PendingZaloPayCart", cart);
                                return Json(new { success = true, redirectUrl = zpResult.OrderUrl, isVnpay = true });
                            }

                            order.StatusId = 2;
                            order.Notes = (order.Notes ?? "") + $" | [ZaloPay Error] {zpResult.ErrorMessage}";
                            foreach (var car in carsToUpdate)
                            {
                                car.IsActive = true;
                            }
                            _context.SaveChanges();
                            return Json(new { success = false, message = $"Không thể tạo giao dịch ZaloPay: {zpResult.ErrorMessage}" });
                        }

                        // Bắn thông báo thời gian thực tới Admin và Chủ xe
                        await SendOrderNotificationAsync(order.OrderId);



 // Tr? v? URL trang n hng c?a khch

 var ordersUrl = Url.Action("Index", "Accounts") + "?tab=orders";

 return Json(new 

 { 

 success = true, 

 redirectUrl = ordersUrl, 

 isVnpay = false,

 title = "Đặt xe thành công!",

 message = "Đơn thuê xe của bạn đã được ghi nhận. Chủ xe sẽ liên hệ sớm nhất."

 });

 }

 catch (Exception ex)

 {

 transaction.Rollback();

 return Json(new { success = false, message = "Đã xảy ra lỗi. Vui lòng thử lại.", error = ex.Message });

 }

 }



 

 }



 return Json(new { success = false, message = "Dữ liệu không hợp lệ." });

 }

 #region test checkout

 /*[HttpPost]

 public IActionResult Checkout(CheckoutVM model)

 {

 //kiem tra trang thai dang nhap

 if (!Function.IsLogin())

 {



 return RedirectToAction("Index", "Login");

 }

 if (ModelState.IsValid)

 {

 var cart = CART;

 if (cart == null || !cart.Any())
 {
     var pendingOrderId = HttpContext.Session.GetInt32("PendingZaloPayOrderId");
     var pendingUrl = HttpContext.Session.GetString("PendingZaloPayOrderUrl");
     if (pendingOrderId.HasValue && pendingOrderId.Value > 0 && !string.IsNullOrEmpty(pendingUrl))
     {
         var pendingOrder = _context.CarRentalOrders.FirstOrDefault(o => o.OrderId == pendingOrderId.Value && o.StatusId == 1);
         if (pendingOrder != null)
         {
             return Json(new
             {
                 success = false,
                 pendingZaloPay = true,
                 orderId = pendingOrder.OrderId,
                 paymentUrl = pendingUrl,
                 ordersUrl = Url.Action("Index", "Accounts") + "?tab=orders",
                 message = $"Đơn thuê xe #RC-{pendingOrder.OrderId} đã được tạo và đang chờ thanh toán ZaloPay."
             });
         }
     }

     return Json(new
     {
         success = false,
         emptyCart = true,
         message = "Giỏ xe hiện đang trống hoặc đơn hàng đã được tạo trước đó. Vui lòng kiểm tra trong mục Đơn thuê của tôi.",
         redirectUrl = Url.Action("Index", "Accounts") + "?tab=orders"
     });
 }

 var customerID = Function.GetAccountId(HttpContext.Session);

 var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerID);

 //ki?m tra n?u khch hng cha c?p nh?t thn tin ?a ch? th? ph?i quay v? c?p nh?t

 if (customer.Address == null)

 {

 TempData["ErrorMessage"] = "vui l?ng c?p nh?t d?a ch? nh?n xe";

 return RedirectToAction("Index", "Accounts");

 }



 var order = new CarRentalOrder

 {

 CustomerId = customerID,

 OrderDate = DateOnly.FromDateTime(Function.NowVn),

 Deposit = model.Depostit,

 Payment = cart.Sum(p => p.Price),

 PlatformFeeRate = 10,

 PlatformFee = Math.Round(((model.Depostit ?? 0m) + cart.Sum(p => p.Price)) * 0.1m, 0),

 ReturnDate = DateOnly.FromDateTime((DateTime)model.OrderReturn),

 StatusId = 1



 };

 _context.Database.BeginTransaction();



 try

 {

 _context.Database.CommitTransaction();

 _context.Add(order);

 _context.SaveChanges();

 var oderDetails = new List<OrderDetail>();

 foreach (var item in cart)

 {

 oderDetails.Add(new OrderDetail

 {

 OrderId = order.OrderId,

 CarId = item.CartId,

 Price = item.Price,

 Quantity = item.Quantity,



 });

 }

 _context.Add(oderDetails);

 _context.SaveChanges();

 HttpContext.Session.Set<List<CartItemsVM>>(CART_KEY, new List<CartItemsVM>());

 return View("Success");

 } 

 catch

 {

 _context.Database.RollbackTransaction();

 }



 }

 return View(CART);

 }*/

 #endregion end test



 public IActionResult Success()

 {

 return View();

 }



        public IActionResult PaymentFail()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ZaloPayCallBack()
        {
            HttpContext.Session.Remove("PendingZaloPayOrderId");
            HttpContext.Session.Remove("PendingZaloPayOrderUrl");
            HttpContext.Session.Remove("PendingZaloPayCart");

            var appTransId = Request.Query["apptransid"].ToString();
            var statusParam = Request.Query["status"].ToString();

            int orderId = 0;
            if (!string.IsNullOrEmpty(appTransId))
            {
                var parts = appTransId.Split('_');
                if (parts.Length >= 2)
                {
                    int.TryParse(parts[1], out orderId);
                }
            }

            var queryRes = await _zaloPayService.QueryOrderStatusAsync(appTransId);
            bool isSuccess = statusParam == "1" || queryRes.IsPaid;

            if (!isSuccess)
            {
                if (orderId > 0)
                {
                    var failOrder = _context.CarRentalOrders
                        .Include(o => o.OrderDetails)
                        .FirstOrDefault(o => o.OrderId == orderId);
                    if (failOrder != null && failOrder.StatusId == 1)
                    {
                        failOrder.StatusId = 2;
                        failOrder.Notes = (failOrder.Notes ?? "") + $" | [ZaloPay {Function.NowVn:dd/MM/yyyy HH:mm}] Giao dịch chưa hoàn tất hoặc đã hủy (Mã: {appTransId})";
                        var failCarIds = failOrder.OrderDetails.Select(d => d.CarId).ToList();
                        var failCars = _context.Cars.Where(c => failCarIds.Contains(c.CarId)).ToList();
                        foreach (var c in failCars)
                        {
                            c.IsActive = true;
                        }
                        _context.SaveChanges();
                    }
                }
                TempData["Message"] = $"Giao dịch ZaloPay chưa hoàn tất hoặc bạn đã hủy giao dịch (Mã GD: {appTransId}).";
                return RedirectToAction("PaymentFail");
            }

            bool isFullPaid = false;
            if (orderId > 0)
            {
                var order = _context.CarRentalOrders.FirstOrDefault(o => o.OrderId == orderId);
                if (order != null)
                {
                    isFullPaid = (order.Payment ?? 0) == 0;
                    if (order.StatusId != 3)
                    {
                        order.StatusId = 3;
                        string nowPayStr = Function.NowVn.ToString("dd/MM/yyyy HH:mm");
                        string noteSuffix = isFullPaid
                            ? $" | [Thanh toán {nowPayStr}] | Đã thanh toán trọn gói 100% qua ZaloPay (Mã GD: {appTransId})"
                            : $" | [Thanh toán {nowPayStr}] | Đã thanh toán cọc qua ZaloPay (Mã GD: {appTransId})";
                        order.Notes = (order.Notes ?? "") + noteSuffix;
                        _context.SaveChanges();

                        await SendOrderNotificationAsync(orderId);
                    }
                }
            }

            TempData["Message"] = isFullPaid
                ? "Thanh toán trọn gói 100% qua ZaloPay thành công!"
                : "Thanh toán đặt cọc qua ZaloPay thành công!";
            return RedirectToAction("Success", "Cart");
        }

        [HttpPost]
        public IActionResult ExpirePendingOrder(int id, bool manual = false)
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập." });
            }

            var customerId = Function.GetAccountId(HttpContext.Session);
            var order = _context.CarRentalOrders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderId == id && o.CustomerId == customerId);

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            if (order.StatusId != 1)
            {
                return Json(new { success = true, alreadyProcessed = true, statusId = order.StatusId });
            }

            var info = Function.ParseZaloPayPending(order.Notes);
            if (!manual && info.IsPendingPayment && info.ExpireAt.HasValue && Function.NowVn < info.ExpireAt.Value)
            {
                return Json(new { success = false, notExpiredYet = true });
            }

            var nowVn = Function.NowVn;
            order.StatusId = 2;
            order.Notes = (order.Notes ?? "") + (manual
                ? $" | [Khách hủy {nowVn:dd/MM/yyyy HH:mm}] Đã hủy đơn chưa thanh toán"
                : $" | [Hệ thống {nowVn:dd/MM/yyyy HH:mm}] Tự động hủy do quá hạn thanh toán 1 phút");

            var carIds = order.OrderDetails.Select(d => d.CarId).ToList();
            var cars = _context.Cars.Where(c => carIds.Contains(c.CarId)).ToList();
            foreach (var c in cars)
            {
                c.IsActive = true;
            }
            _context.SaveChanges();

            return Json(new { success = true, statusId = 2 });
        }



 private async Task SendOrderNotificationAsync(int orderId)

 {

 try

 {

 var order = await _context.CarRentalOrders

 .Include(o => o.Customer)

 .Include(o => o.OrderDetails)

 .ThenInclude(od => od.Car)

 .FirstOrDefaultAsync(o => o.OrderId == orderId);



 if (order == null) return;



 var firstDetail = order.OrderDetails.FirstOrDefault();

 string carName = firstDetail?.Car?.CarName ?? "Xe  t";

 string customerName = order.Customer?.FullName ?? "Khch hng";

 decimal deposit = order.Deposit ?? 0;

 string depositFormatted = deposit.ToString("N0") + "";



 var notificationPayload = new

 {

 orderId = order.OrderId,

 customerName = customerName,

 carName = carName,

 depositFormatted = depositFormatted,

 totalAmount = (order.Deposit ?? 0) + (order.Payment ?? 0),

 detailUrl = $"/CarOwner/Orders/Details/{order.OrderId}"

 };



 // Lu thng bo vo CSDL cho Admin, Ch? xe v Khch hng

 await _notificationService.CreateNotificationAsync(

 null,

 "Admin",

 "n thu xe m?i ",

 $"Khch hng {customerName} v?a ?t xe {carName}. M? n: #{order.OrderId}, ti?n c?c {depositFormatted}.",

 "NewOrder",

 $"/Admin/Orders/Edit/{order.OrderId}"

 );



 var ownerIds = order.OrderDetails

 .Where(od => od.Car != null && od.Car.OwnerId.HasValue)

 .Select(od => od.Car.OwnerId.Value)

 .Distinct()

 .ToList();



 foreach (var ownerId in ownerIds)

 {

 await _notificationService.CreateNotificationAsync(

 ownerId,

 "CarOwner",

 "Bạn có đơn đặt xe mới!",

 $"Khách hàng {customerName} vừa đặt thuê xe {carName}. Mã đơn: #{order.OrderId}. Vui lòng kiểm tra lịch bàn giao xe.",

 "NewOrder",

 $"/CarOwner/Orders/Details/{order.OrderId}"

 );

 }



 if (order.CustomerId > 0)

 {

 await _notificationService.CreateNotificationAsync(

 order.CustomerId,

 "Customer",

 "?t c?c thu xe thnh cng ",

 $"B?n ? ?t c?c thnh cng xe {carName}. M? n #{order.OrderId}. Ch? xe s? lin h? bn giao xe.",

 "NewOrder",

 "/Accounts/Orders"

 );

 }



 // 1. G?i cho ton b? Admin qua SignalR

 await _hubContext.Clients.Group("Role_Admin").SendAsync("ReceiveNewOrderNotification", notificationPayload);

 await _hubContext.Clients.Group("Role_Qu?n tr? vin").SendAsync("ReceiveNewOrderNotification", notificationPayload);



 // 2. G?i cho cc Ch? xe qua SignalR

 foreach (var ownerId in ownerIds)

 {

 await _hubContext.Clients.Group($"User_{ownerId}").SendAsync("ReceiveNewOrderNotification", notificationPayload);

 }

 }

 catch (Exception ex)

 {

 Console.WriteLine($"SignalR Order Notification Error: {ex.Message}");

 }

 }

 }

 

}

