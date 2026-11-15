namespace CarRental.Services
{
    public interface IZaloPayService
    {
        Task<(bool Success, string OrderUrl, string AppTransId, string ErrorMessage)> CreateOrderAsync(
            HttpContext context,
            int orderId,
            long amount,
            string appUser,
            string description);

        Task<(bool IsPaid, int ReturnCode, string Message)> QueryOrderStatusAsync(string appTransId);
    }
}
