using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Queries
{
    public sealed record class GetPatronRecommendedBooksQuery(Guid PatronId, int PageNumber, int Count) : IRequest<(List<Book>, int)>;

}
