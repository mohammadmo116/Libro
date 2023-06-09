
namespace Libro.Infrastructure.Repositories
{
    public interface INotificationRepository
    {
        Task NotifyAll(string method, string message);
        Task NotifyUser(string UserId, string method, string message);
    }
}