using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.ReadingLists.Commands
{
    public sealed record AddBookToUserReadingListCommand(Guid UserId, BookReadingList BookReadingList)
        : IRequest<bool>;
}
