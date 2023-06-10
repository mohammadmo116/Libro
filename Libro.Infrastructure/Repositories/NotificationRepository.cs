
using Libro.Domain.Entities;
using Libro.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationRepository(
            ApplicationDbContext context,
            IHubContext<NotificationHub> hubContext
            )
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task NotifyAll(string method, string message)
        {
            await _hubContext.Clients.All.SendAsync(method, message);

        }
        public async Task NotifyUser(string UserId, string method, string message)
        {
            await _hubContext.Clients.User(UserId).SendAsync(method, message);
        }
        public async Task NotifyUsers(List<string> UserIds, string method, string message)
        {
            await _hubContext.Clients.Users(UserIds).SendAsync(method, message);
        }

        public async Task DataBaseNotify(List<Notification> notification)
        {
            await _context.Notifications.AddRangeAsync(notification);
        }

        public async Task<(List<Notification>, int)> GetNotifications(Guid UserId, int PageNumber, int Count)
        {
            var notificationsCount = await _context.Notifications.Where(a => a.UserId == UserId).CountAsync();

            var notifications = await _context.Notifications.Where(a => a.UserId == UserId)
                .Skip(PageNumber * Count).Take(Count).ToListAsync();

            var NumberOfPages = 1;
            if (notificationsCount > 0)
                NumberOfPages = (int)Math.Ceiling((double)notificationsCount / Count);

            return (notifications, NumberOfPages);

        }
    }
}
