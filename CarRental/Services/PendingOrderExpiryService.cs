using CarRental.Models;
using CarRental.Utilities;

namespace CarRental.Services
{
    /// <summary>
    /// Dịch vụ nền tự động kiểm tra và hủy các đơn chờ thanh toán ZaloPay quá hạn (1 phút),
    /// đồng thời mở lại trạng thái sẵn sàng cho xe trên sàn.
    /// </summary>
    public class PendingOrderExpiryService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PendingOrderExpiryService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<DbRenalCarContext>();
                    Function.CancelExpiredPendingOrders(db);
                }
                catch
                {
                    // Bỏ qua lỗi tạm thời trong background loop
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
