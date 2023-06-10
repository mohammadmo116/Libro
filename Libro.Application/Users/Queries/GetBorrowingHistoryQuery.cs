using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Queries
{
    public sealed record GetBorrowingHistoryQuery(Guid UserId, int PageNumber, int Count)
        : IRequest<(List<BookTransaction>, int)>;

}
