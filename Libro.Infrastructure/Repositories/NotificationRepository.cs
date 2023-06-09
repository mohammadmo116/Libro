
using Libro.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task NotifyUser(string UserId,string method, string message)
        {
            await _hubContext.Clients.Users(UserId).SendAsync(method, message);
        }
    }
}
