using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Books.Queries
{
    public sealed record class GetSearchedBooksQuery(string? Title,
        string? AuthorName,
        string? Genre,
        int PageNumber,
        int Count) : IRequest<(List<Book>, int)>;


}
