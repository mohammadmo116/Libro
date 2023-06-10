
using Libro.Domain.Entities;

namespace Libro.Infrastructure.Repositories
{
    public interface INotificationRepository
    {
        Task NotifyAll(string method, string message);
        Task NotifyUser(string UserId, string method, string message);
        Task NotifyUsers(List<string> UserIds, string method, string message);
        Task DataBaseNotify(List<Notification> notification);
        Task<(List<Notification>, int)> GetNotifications(Guid UserId, int PageNumber, int Count);
    }
}