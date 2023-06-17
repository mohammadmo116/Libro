using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.ReadingLists.Queries
{
    public sealed record GetUserReadingListWithBooksQuery(Guid UserId, Guid ReadingListId, int PageNumber, int Count) : IRequest<(ReadingList, int)>;

}
