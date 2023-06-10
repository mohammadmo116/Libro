using Libro.Domain.Entities;
using MediatR;


namespace Libro.Application.Books.Queries
{
    public sealed record GetBookByIdQuery(Guid BookId) : IRequest<Book>;
}
