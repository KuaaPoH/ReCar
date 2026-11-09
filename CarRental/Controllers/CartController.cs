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

 private readonly IVnpayServices _vnpayServices;

 private readonly IConfiguration _config;

 private readonly IHubContext<ReCarHub> _hubContext;

 private readonly INotificationService _notificationService;



 public CartController(DbRenalCarContext context, IVnpayServices vnpayServices, IConfiguration config, IHubContext<ReCarHub> hubContext, INotificationService notificationService)

 {

 _context = context;

 _vnpayServices = vnpayServices;

 _config = config;

 _hubContext = hubContext;

 _notificationService = notificationService;

 }

 string CART_KEY = "MYCART";

 public List<CartItemsVM> CART => HttpContext.Session.Get<List<CartItemsVM>>(CART_KEY) ?? new List<CartItemsVM> ();

 public IActionResult Index()

 {

 

 return View(CART);

 }

 [HttpPost]

 public IActionResult AddToCart(int id, DateTime pickupDate, DateTime returnDate, int quantity = 1, string type = "Normal", string driveMode = "T? li")

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



 bool withDriver = (driveMode == "C ti x?" || driveMode == "Km ti x?");

 var carPolicy = CarPolicy.Parse(product);
            decimal driverFeeTotal = withDriver ? (carPolicy.DriverFee * rentalDays) : 0m;



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

 DriveMode = withDriver ? "C ti x?" : "T? li",

 DriverFee = driverFeeTotal

 };

 cart.Add(CarItem);

 } else

 {

 // n?u ? c th? c?p nh?t l?i

 CarItem.Quantity = quantity;

 CarItem.pickupDate = pickupDate;

 CarItem.returnDate = returnDate;

 CarItem.DriveMode = withDriver ? "C ti x?" : "T? li";

 CarItem.DriverFee = driverFeeTotal;

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

 decimal payment = total - deposit; // S? ti?n c?n l?i khi nh?n xe



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

 public IActionResult Checkout()

 {

 //kiem tra trang thai dang nhap

 if (!Function.IsLogin(HttpContext.Session))

 {



 return RedirectToAction("Index", "Login", new { ReturnUrl = Url.Action("Checkout", "Cart") });

 }

 var cart = CART; // L?y gi? hng t? Session

 if(cart == null || !cart.Any())

 {

 return RedirectToAction("Index", "Cart"); // Chuy?n h?ng n?u gi? hng tr?ng

 }

 // Tnh t?ng ti?n thu xe

 decimal total = cart.Sum(item => item.PriceTotal);





 var sysSetting = _context.SystemSettings.FirstOrDefault();

 decimal depositRate = (sysSetting?.DepositPct ?? 30) / 100m;

 decimal deposit = Math.Round(total * depositRate, 0); // Ti?n ?t c?c theo c?u h?nh sn

 ViewBag.DepositPct = sysSetting?.DepositPct ?? 30;

 decimal payment = total - deposit; // S? ti?n c?n l?i khi nh?n xe



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

 return Json(new { success = false, message = "B?n c?n ng nh?p ? ti?p t?c." });

 

 }



 if (ModelState.IsValid)

 {

 





 var cart = CART;

 var customerID = Function.GetAccountId(HttpContext.Session);

 var userEmail = Function.GetEmail(HttpContext.Session);

 var customer = _context.Customers.FirstOrDefault(c => 

 c.CustomerId == customerID || 

 (!string.IsNullOrEmpty(userEmail) && c.Email == userEmail));



 if (customer == null)

 {

 return Json(new { success = false, message = "Khng t?m th?y khch hng." });

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

 message = "Vui l?ng c?p nh?t ?a ch? nh?n xe.", 

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

 message = "Vui l?ng c?p nh?t thng tin Gi?y php li xe tr?c khi ?t xe.", 

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

 message = $"Xe {item.CarName} ? ?c ch? xe kha l?ch ({blockedSlot.StartTime:dd/MM/yyyy}  {blockedSlot.EndTime:dd/MM/yyyy}). Vui l?ng ch?n khung gi? khc."

 });

 }

 }

 }

 catch

 {

 // B? qua n?u b?ng l?ch ch?n cha kh?i t?o

 }



 decimal price = cart.Sum(p => p.PriceTotal);

 bool isFullPayment = model.paymentmethod == "full_100" || 

 model.paymentmethod == "Thanh ton 100%" ||

 model.paymentmethod == "Thanh ton ton b? 100%";



 var sysSetting = _context.SystemSettings.FirstOrDefault();

 int currentCommissionPct = sysSetting?.CommissionPct ?? 10;

 int currentDepositPct = sysSetting?.DepositPct ?? 30;



 decimal deposit = isFullPayment ? price : Math.Round((price * (decimal)currentDepositPct) / 100m, 0);

 decimal payments = isFullPayment ? 0 : (price - deposit);

 decimal platformFee = Math.Round((price * (decimal)currentCommissionPct) / 100m, 0);



 var driveModeSummary = string.Join("; ", cart.Select(c => $"H?nh th?c: {c.DriveMode}" + (c.DriverFee > 0 ? $" (Ph? ph ti x?: {c.DriverFee:N0})" : "")));

 string paymentPlanText = isFullPayment ? "Gi thanh ton: Tr?n gi 100% qua VNPay" : $"Gi thanh ton: ?t c?c {currentDepositPct}% qua VNPay";

 string nowVnTime = Function.NowVn.ToString("dd/MM/yyyy HH:mm");

 string combinedNotes = $"[T?o n {nowVnTime}] | " + driveModeSummary + $" | [{paymentPlanText}]" + (!string.IsNullOrWhiteSpace(model.Notes) ? $" | Ghi ch khch: {model.Notes}" : "");



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

 _context.SaveChanges(); // Lu thay ?i

 HttpContext.Session.Set<List<CartItemsVM>>(CART_KEY, new List<CartItemsVM>());

 transaction.Commit();

 bool isVnPay = model.paymentmethod != "cod" && model.paymentmethod != "Thanh ton COD";

 if (isVnPay)

 {

 bool isBypass = _config.GetValue<bool>("VnPay:BypassPayment", false);

 if (isBypass)

 {

 // Ch? ? gi? l?p test: Xc nh?n thanh ton thnh cng ngay l?p t?c

 order.StatusId = 3; // ? thanh ton / c?c thnh cng

 string testLog = isFullPayment

 ? $"[Thanh ton {Function.NowVn:dd/MM/yyyy HH:mm}] | [VNPay Test] ? thanh ton tr?n gi 100% qua VNPay thnh cng"

 : $"[Thanh ton {Function.NowVn:dd/MM/yyyy HH:mm}] | [VNPay Test] ? thanh ton ?t c?c 30% qua VNPay thnh cng";

 order.Notes = (order.Notes ?? "") + " | " + testLog;

 _context.SaveChanges();



 // B?n thng bo th?i gian th?c t?i Admin v Ch? xe

 await SendOrderNotificationAsync(order.OrderId);



 var ordersUrlMock = Url.Action("Index", "Accounts") + "?tab=orders";

 return Json(new 

 { 

 success = true, 

 redirectUrl = ordersUrlMock, 

 isVnpay = false,

 title = isFullPayment ? "Thanh ton tr?n gi 100% thnh cng!" : "?t c?c 30% qua VNPay thnh cng!",

 message = isFullPayment

 ? $"B?n ? thanh ton tr?n gi 100% ({price:N0}) thnh cng qua VNPay (Ch? ? gi? l?p). Khng c?n thanh ton thm ti?n thu khi nh?n xe!"

 : $"B?n ? ?t c?c 30% ({deposit:N0}) thnh cng qua VNPay (Ch? ? gi? l?p). n hng ? chuy?n sang tr?ng thi ? thanh ton!"

 });

 }



 var vnPayModel = new VnPaymentRequestModel

 {

 Amount = (double)deposit,

 CreatedDate = Function.NowVn,

 Description = $"{Function.GetUserName(HttpContext.Session)} {Function.GetPhone(HttpContext.Session)}",

 FullName = Function.GetUserName(HttpContext.Session),

 OrderId = order.OrderId

 };

 var paymentUrl = _vnpayServices.CreatePaymentUrl(HttpContext, vnPayModel);



 return Json(new { success = true, redirectUrl = paymentUrl, isVnpay = true });

 }



 // B?n thng bo th?i gian th?c t?i Admin v Ch? xe

 await SendOrderNotificationAsync(order.OrderId);



 // Tr? v? URL trang n hng c?a khch

 var ordersUrl = Url.Action("Index", "Accounts") + "?tab=orders";

 return Json(new 

 { 

 success = true, 

 redirectUrl = ordersUrl, 

 isVnpay = false,

 title = "?t xe thnh cng!",

 message = "n thu xe c?a b?n ? ?c ghi nh?n. Ch? xe s? lin h? s?m nh?t."

 });

 }

 catch (Exception ex)

 {

 transaction.Rollback();

 return Json(new { success = false, message = "? x?y ra l?i. Vui l?ng th? l?i.", error = ex.Message });

 }

 }



 

 }



 return Json(new { success = false, message = "D? li?u khng h?p l?." });

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



 public async Task<IActionResult> PaymentCallBack()

 {

 var response = _vnpayServices.PaymentExecute(Request.Query);



 if (response == null || !response.Success || response.VnPayResponseCode != "00")

 {

 var errCode = response?.VnPayResponseCode ?? "99";

 TempData["Message"] = $"Giao d?ch VNPay khng thnh cng ho?c b?n ? h?y giao d?ch (M? ph?n h?i: {errCode}).";

 return RedirectToAction("PaymentFail");

 }



 // C?p nh?t tr?ng thi n hng ? c?c qua VNPay

 if (!string.IsNullOrEmpty(response.OrderId) && int.TryParse(response.OrderId, out int orderId))

 {

 var order = _context.CarRentalOrders.FirstOrDefault(o => o.OrderId == orderId);

 if (order != null)

 {

 order.StatusId = 3; // Tr?ng thi: ? thanh ton / c?c thnh cng

 bool isFull = (order.Payment ?? 0) == 0;

 string nowPayStr = Function.NowVn.ToString("dd/MM/yyyy HH:mm");

 string noteSuffix = isFull

 ? $" | [Thanh ton {nowPayStr}] | ? thanh ton tr?n gi 100% qua VNPay (M? GD: {response.TransactionId})"

 : $" | [Thanh ton {nowPayStr}] | ? thanh ton c?c 30% qua VNPay (M? GD: {response.TransactionId})";

 order.Notes = (order.Notes ?? "") + noteSuffix;

 _context.SaveChanges();



 // B?n thng bo Realtime

 await SendOrderNotificationAsync(orderId);

 }

 }



 TempData["Message"] = "Thanh ton ?t c?c qua VNPay thnh cng!";

 return RedirectToAction("Success", "Cart");

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

 "B?n c n ?t xe m?i! ",

 $"Khch hng {customerName} v?a ?t thu xe {carName}. M? n: #{order.OrderId}. Vui l?ng ki?m tra l?ch bn giao xe.",

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

