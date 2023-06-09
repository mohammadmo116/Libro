using Libro.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Libro.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        public NotificationHub()
        {
        }

        public override Task OnConnectedAsync() {
            Console.WriteLine("--------------------------------------------------------------");
         
            Console.WriteLine(Context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value+  "/dslmnfdkl");
        return Task.CompletedTask;
        }

    }
}
