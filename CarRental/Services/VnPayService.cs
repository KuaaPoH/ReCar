using CarRental.Utilities;
using CarRental.ViewModels;

namespace CarRental.Services
{
    public class VnPayService : IVnpayServices
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"] ?? "2.1.0");
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"] ?? "pay");
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", ((long)(model.Amount * 100)).ToString());

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"] ?? "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"] ?? "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang " + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");

            var returnUrl = _config["VnPay:PaymentBackReturnUrl"];
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = $"{context.Request.Scheme}://{context.Request.Host}/Cart/PaymentCallBack";
            }
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);

            // Mã TxnRef gắn OrderId để callback dễ dàng map lại đơn hàng
            vnpay.AddRequestData("vnp_TxnRef", $"{model.OrderId}_{tick}");

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var txnRef = vnpay.GetResponseData("vnp_TxnRef");
            string parsedOrderId = txnRef;
            if (!string.IsNullOrEmpty(txnRef) && txnRef.Contains("_"))
            {
                parsedOrderId = txnRef.Split('_')[0];
            }

            var txnNo = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                    VnPayResponseCode = "97",
                    OrderId = parsedOrderId
                };
            }

            return new VnPaymentResponseModel
            {
                Success = (vnp_ResponseCode == "00"),
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = parsedOrderId,
                TransactionId = txnNo,
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }
    }
}
