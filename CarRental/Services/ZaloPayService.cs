using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CarRental.Utilities;

namespace CarRental.Services
{
    public class ZaloPayService : IZaloPayService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ZaloPayService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<(bool Success, string OrderUrl, string AppTransId, string ErrorMessage)> CreateOrderAsync(
            HttpContext context,
            int orderId,
            long amount,
            string appUser,
            string description)
        {
            var appId = _config["ZaloPay:AppId"] ?? "2554";
            var key1 = _config["ZaloPay:Key1"] ?? "sdngKKJmqEMzvh5QQcdD2A9XBSKUNaYn";
            var endpoint = _config["ZaloPay:CreateOrderUrl"] ?? "https://sb-openapi.zalopay.vn/v2/create";
            var redirectUrl = _config["ZaloPay:RedirectUrl"];
            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                redirectUrl = $"{context.Request.Scheme}://{context.Request.Host}/Cart/ZaloPayCallBack";
            }

            var appTransId = $"{Function.NowVn:yyMMdd}_{orderId}_{Random.Shared.Next(100, 999)}";
            var appTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var safeUser = string.IsNullOrWhiteSpace(appUser) ? "recar_guest" : appUser.Trim();
            var embedData = JsonSerializer.Serialize(new { redirecturl = redirectUrl });
            const string item = "[]";

            var rawMac = $"{appId}|{appTransId}|{safeUser}|{amount}|{appTime}|{embedData}|{item}";
            var mac = ComputeHmacSha256(key1, rawMac);

            var payload = new Dictionary<string, string>
            {
                ["app_id"] = appId,
                ["app_user"] = safeUser,
                ["app_trans_id"] = appTransId,
                ["app_time"] = appTime,
                ["amount"] = amount.ToString(),
                ["item"] = item,
                ["embed_data"] = embedData,
                ["description"] = description,
                ["bank_code"] = "",
                ["mac"] = mac
            };

            using var content = new FormUrlEncodedContent(payload);
            var response = await _httpClient.PostAsync(endpoint, content);
            var body = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            int returnCode = root.TryGetProperty("return_code", out var rc) ? rc.GetInt32() : 0;
            string returnMsg = root.TryGetProperty("return_message", out var rm) ? (rm.GetString() ?? "") : "";
            string subMsg = root.TryGetProperty("sub_return_message", out var srm) ? (srm.GetString() ?? "") : "";

            if (returnCode == 1 && root.TryGetProperty("order_url", out var urlProp))
            {
                return (true, urlProp.GetString() ?? "", appTransId, "");
            }

            var err = string.IsNullOrWhiteSpace(subMsg) ? returnMsg : $"{returnMsg} ({subMsg})";
            return (false, "", appTransId, err);
        }

        public async Task<(bool IsPaid, int ReturnCode, string Message)> QueryOrderStatusAsync(string appTransId)
        {
            if (string.IsNullOrWhiteSpace(appTransId))
                return (false, 0, "Thiếu mã giao dịch ZaloPay.");

            var appId = _config["ZaloPay:AppId"] ?? "2554";
            var key1 = _config["ZaloPay:Key1"] ?? "sdngKKJmqEMzvh5QQcdD2A9XBSKUNaYn";
            var endpoint = _config["ZaloPay:QueryOrderUrl"] ?? "https://sb-openapi.zalopay.vn/v2/query";

            var rawMac = $"{appId}|{appTransId}|{key1}";
            var mac = ComputeHmacSha256(key1, rawMac);

            var payload = new Dictionary<string, string>
            {
                ["app_id"] = appId,
                ["app_trans_id"] = appTransId,
                ["mac"] = mac
            };

            using var content = new FormUrlEncodedContent(payload);
            var response = await _httpClient.PostAsync(endpoint, content);
            var body = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            int returnCode = root.TryGetProperty("return_code", out var rc) ? rc.GetInt32() : 0;
            string returnMsg = root.TryGetProperty("return_message", out var rm) ? (rm.GetString() ?? "") : "";

            return (returnCode == 1, returnCode, returnMsg);
        }

        private static string ComputeHmacSha256(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
