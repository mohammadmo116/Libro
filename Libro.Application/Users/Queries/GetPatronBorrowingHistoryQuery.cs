using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Queries
{
    public sealed record GetPatronBorrowingHistoryQuery(Guid PatronId, int PageNumber, int Count)
        : IRequest<(List<BookTransaction>, int)>;

}
