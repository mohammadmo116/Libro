using MediatR;

namespace Libro.Application.Notifications.Commands
{
    public sealed record NotifyPatronsForReservedBooksCommand() : IRequest<bool>;

}
