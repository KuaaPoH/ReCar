using CarRental.Extensions;
using CarRental.Models;
using CarRental.Services;
using CarRental.Utilities;
using CarRental.ViewModels;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class CartController : Controller
    {
        private readonly DbRenalCarContext _context;
        private readonly IVnpayServices _vnpayServices;
        private readonly IConfiguration _config;

        public CartController(DbRenalCarContext context, IVnpayServices vnpayServices, IConfiguration config)
        {
            _context = context;
            _vnpayServices = vnpayServices;
            _config = config;
        }
        string CART_KEY = "MYCART";
        public List<CartItemsVM> CART => HttpContext.Session.Get<List<CartItemsVM>>(CART_KEY) ?? new List<CartItemsVM> ();
        public IActionResult Index()
        {
            
            return View(CART);
        }
        [HttpPost]
        public IActionResult AddToCart(int id, DateTime pickupDate, DateTime returnDate, int quantity = 1, string type = "Normal", string driveMode = "Tự lái")
        {
            var cart = CART;
            // lấy thông tin sản phảm từ DB
            var product = _context.Cars.FirstOrDefault(p => p.CarId == id);

            // Xử lý thêm sản phẩm vào giỏ hàng
            int rentalDays = (returnDate - pickupDate).Days;
            if (rentalDays <= 0) rentalDays = 1;

            if (product == null) {
                return NotFound();// nếu không tìm thấy sp
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
            decimal driverFeeTotal = withDriver ? (500000m * rentalDays) : 0m;

            // kiểm tra xem có sản phẩm tồn tai trong giỏ hàng hay chưa
            var CarItem = cart.FirstOrDefault(p => p.CartId == id);
            if (CarItem == null)
            {
                // nếu chưa có sản phẩm thì khởi tạo sản phẩm mới
                CarItem = new CartItemsVM
                {
                    CartId = product.CarId,
                    CarName = product.CarName,
                    Image = product.Image,
                    pickupDate = pickupDate,
                    returnDate = returnDate,
                    Price = finalPrice * rentalDays,
                    Quantity = quantity,
                    DriveMode = withDriver ? "Có tài xế" : "Tự lái",
                    DriverFee = driverFeeTotal
                };
                cart.Add(CarItem);
            } else
            {
                // nếu đã có thì cập nhật lại
                CarItem.Quantity = quantity;
                CarItem.pickupDate = pickupDate;
                CarItem.returnDate = returnDate;
                CarItem.DriveMode = withDriver ? "Có tài xế" : "Tự lái";
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
            var cart = CART; // Lấy giỏ hàng từ Session
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart"); // Chuyển hướng nếu giỏ hàng trống
            }

            // Tính tổng tiền thuê xe
            decimal total = cart.Sum(item => item.Price);
           

            decimal deposit = total * 0.3m; // Tiền đặt cọc 30%
            decimal payment = total - deposit; // Số tiền còn lại khi nhận xe

            // Tạo ViewModel
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
            // Giả sử bạn có giỏ hàng trong session
            var cart = CART; // Lấy giỏ hàng từ session hoặc cơ sở dữ liệu

            foreach (var updatedItem in updatedCart)
            {
                var cartItem = cart.FirstOrDefault(item => item.CartId == updatedItem.CartId);
                if (cartItem != null)
                {
                    // Cập nhật số lượng và tính lại giá
                    cartItem.Quantity = updatedItem.Quantity;

                    // Tính lại giá thuê xe dựa trên số ngày thuê và số lượng
                    int rentalDays = (cartItem.returnDate - cartItem.pickupDate).Days;
                    cartItem.Price = cartItem.Price * rentalDays * cartItem.Quantity;
                }
            }

            // Cập nhật tổng giá trị giỏ hàng
            /*cart.UpdateTotalPrice();*/

            // Lưu giỏ hàng lại vào session
            HttpContext.Session.Set(CART_KEY, cart);

            // Trả về tổng giá trị giỏ hàng cho client
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
            var cart = CART; // Lấy giỏ hàng từ Session
            if(cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart"); // Chuyển hướng nếu giỏ hàng trống
            }
            // Tính tổng tiền thuê xe
            decimal total = cart.Sum(item => item.Price);


            decimal deposit = total * 0.3m; // Tiền đặt cọc 30%
            decimal payment = total - deposit;  // Số tiền còn lại khi nhận xe

            // Tạo ViewModel
            var model = new CartVM
            {
                Total = total,
                Deposit = deposit,
                Payment = payment
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Checkout([FromBody] CheckoutVM model)
        {
            if (!Function.IsLogin(HttpContext.Session))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để tiếp tục." });
                
            }

            if (ModelState.IsValid)
            {
               


                var cart = CART;
                var customerID = Function.GetAccountId(HttpContext.Session);
                var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerID);
                ViewBag.Order =  _context.CarRentalOrders.Include(i => i.Status).Where(p => p.CustomerId == customerID).ToList();
                if (customer == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khách hàng." });
                }

                // Kiểm tra địa chỉ
                if (string.IsNullOrWhiteSpace(customer.Address))
                {
                    // Lưu returnUrl vào session để quay lại sau khi cập nhật địa chỉ
                    var returnUrl = Url.Action("Checkout", "Cart");
                    HttpContext.Session.SetString("AddressUpdateReturnUrl", returnUrl);
                    
                    return Json(new { 
                        success = false, 
                        message = "Vui lòng cập nhật địa chỉ nhận xe.", 
                        needAddress = true,
                        redirectUrl = Url.Action("editAdress", "Accounts", new { returnUrl = returnUrl })
                    });
                }
                
                // Kiểm tra GPLX (Giấy phép lái xe)
                if (string.IsNullOrWhiteSpace(customer.LicenseNumber))
                {
                    // Lưu returnUrl vào session để quay lại sau khi cập nhật GPLX
                    var returnUrl = Url.Action("Checkout", "Cart");
                    HttpContext.Session.SetString("LicenseUpdateReturnUrl", returnUrl);
                    
                    // Redirect đến My Account với tab GPLX và returnUrl trong query string
                    var accountsUrl = Url.Action("Index", "Accounts", new { returnUrl = returnUrl }) + "#download";
                    
                    return Json(new { 
                        success = false, 
                        message = "Vui lòng cập nhật thông tin Giấy phép lái xe trước khi đặt xe.", 
                        needLicense = true,
                        redirectUrl = accountsUrl
                    });
                }


                
                decimal price = cart.Sum(p => p.PriceTotal);
                decimal deposit = (30*price)/100;
                decimal payments = price - deposit;

                var driveModeSummary = string.Join("; ", cart.Select(c => $"Hình thức: {c.DriveMode}" + (c.DriverFee > 0 ? $" (Phụ phí tài xế: {c.DriverFee:N0}đ)" : "")));
                string combinedNotes = driveModeSummary + (!string.IsNullOrWhiteSpace(model.Notes) ? $" | Ghi chú khách: {model.Notes}" : "");

                var order = new CarRentalOrder
                {
                    CustomerId = customerID,
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    Deposit = deposit,
                    //ReturnDate = DateOnly.FromDateTime(cart.Max(p => p.returnDate)),
                    StatusId = 1,
                    Payment = payments,
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
                        
                        // Cập nhật trạng thái IsActive của xe
                        var carIds = cart.Select(item => item.CartId).ToList(); // Lấy danh sách ID của xe trong giỏ hàng
                        var carsToUpdate = _context.Cars.Where(c => carIds.Contains(c.CarId)).ToList(); // Lấy danh sách xe từ DB
                        foreach (var car in carsToUpdate)
                        {
                            car.IsActive = false; // Đặt thuộc tính IsActive thành false
                        }
                        _context.SaveChanges(); // Lưu thay đổi
                        HttpContext.Session.Set<List<CartItemsVM>>(CART_KEY, new List<CartItemsVM>());
                        transaction.Commit();
                        if (model.paymentmethod == "Thanh toán VNPay")
                        {
                            bool isBypass = _config.GetValue<bool>("VnPay:BypassPayment", false);
                            if (isBypass)
                            {
                                // Chế độ giả lập test: Xác nhận cọc 30% thành công ngay lập tức
                                order.StatusId = 3; // Đã thanh toán / cọc thành công
                                order.Notes = (order.Notes ?? "") + " | [VNPay Test] Đã thanh toán đặt cọc 30% thành công";
                                _context.SaveChanges();

                                var ordersUrlMock = Url.Action("Index", "Accounts") + "?tab=orders";
                                return Json(new 
                                { 
                                    success = true, 
                                    redirectUrl = ordersUrlMock, 
                                    isVnpay = false,
                                    title = "Thanh toán VNPay thành công!",
                                    message = "Bạn đã đặt cọc 30% thành công qua cổng VNPay (Chế độ giả lập để kiểm thử). Đơn hàng đã chuyển sang trạng thái Đã thanh toán!"
                                });
                            }

                            var vnPayModel = new VnPaymentRequestModel
                            {
                                Amount = (double)deposit,
                                CreatedDate = DateTime.Now,
                                Description = $"{Function.GetUserName(HttpContext.Session)} {Function.GetPhone(HttpContext.Session)}",
                                FullName = Function.GetUserName(HttpContext.Session),
                                OrderId = order.OrderId
                            };
                            var paymentUrl = _vnpayServices.CreatePaymentUrl(HttpContext, vnPayModel);

                            return Json(new { success = true, redirectUrl = paymentUrl, isVnpay = true });
                        }
                        // Trả về URL trang đơn hàng của khách
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
                var customerID = Function.GetAccountId(HttpContext.Session);
                var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerID);
                //kiểm tra nếu khách hàng chưa cập nhật thôn tin địa chỉ thì phải quay về cập nhật
                if (customer.Address == null)
                {
                    TempData["ErrorMessage"] = "vui lòng cập nhật dịa chỉ nhận xe";
                    return RedirectToAction("Index", "Accounts");
                }

                var order = new CarRentalOrder
                {
                    CustomerId = customerID,
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    Deposit = model.Depostit,
                    Payment = cart.Sum(p => p.Price),
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

        public IActionResult PaymentCallBack()
        {
            var response = _vnpayServices.PaymentExecute(Request.Query);

            if (response == null || !response.Success || response.VnPayResponseCode != "00")
            {
                var errCode = response?.VnPayResponseCode ?? "99";
                TempData["Message"] = $"Giao dịch VNPay không thành công hoặc bạn đã hủy giao dịch (Mã phản hồi: {errCode}).";
                return RedirectToAction("PaymentFail");
            }

            // Cập nhật trạng thái đơn hàng đã cọc qua VNPay
            if (!string.IsNullOrEmpty(response.OrderId) && int.TryParse(response.OrderId, out int orderId))
            {
                var order = _context.CarRentalOrders.FirstOrDefault(o => o.OrderId == orderId);
                if (order != null)
                {
                    order.StatusId = 3; // Trạng thái: Đã thanh toán / cọc thành công
                    order.Notes = (order.Notes ?? "") + $" | Đã thanh toán cọc qua VNPay (Mã GD: {response.TransactionId})";
                    _context.SaveChanges();
                }
            }

            TempData["Message"] = "Thanh toán đặt cọc qua VNPay thành công!";
            return RedirectToAction("Success", "Cart");
        }
    }
    
}
