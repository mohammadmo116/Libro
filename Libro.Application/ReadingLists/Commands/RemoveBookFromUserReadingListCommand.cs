using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.ReadingLists.Commands
{
    public sealed record RemoveBookFromUserReadingListCommand(Guid UserId, BookReadingList BookReadingList)
                : IRequest<bool>;
}
