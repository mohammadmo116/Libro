using MediatR;

namespace Libro.Application.ReadingLists.Commands
{
    public sealed record RemoveReadingListCommand(Guid UserId, Guid ReadingListId) : IRequest<bool>;

}
