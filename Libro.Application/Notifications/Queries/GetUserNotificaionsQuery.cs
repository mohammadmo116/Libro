using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Notifications.Queries
{
    public sealed record class GetUserNotificaionsQuery(Guid UserId, int PageNumbr, int Count) : IRequest<(List<Notification>, int)>;

}
