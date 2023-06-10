using MediatR;

namespace Libro.Application.Notifications.Commands
{
    public class NotifyPatronsForDueDatesCommand : IRequest<bool>
    {
    }
}
