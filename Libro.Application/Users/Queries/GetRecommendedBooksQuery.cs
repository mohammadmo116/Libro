using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Queries
{
    public sealed record class GetRecommendedBooksQuery(Guid UserId, int PageNumber, int Count) : IRequest<(List<Book>, int)>;

}
