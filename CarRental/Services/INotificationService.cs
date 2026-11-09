using System.Threading.Tasks;

namespace CarRental.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int? userId, string? role, string title, string content, string type = "System", string? linkUrl = null);
    }
}

