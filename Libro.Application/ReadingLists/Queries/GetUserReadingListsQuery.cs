using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.ReadingLists.Queries
{
    public sealed record class GetUserReadingListsQuery(Guid UserId, int PageNumber, int Count) : IRequest<(List<ReadingList>, int)>;
}
