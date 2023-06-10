using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Books.Commands
{
    public sealed record CreateBookCommand(Book book) : IRequest<Book>;

}
