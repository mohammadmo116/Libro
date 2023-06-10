using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Books.Commands
{
    public sealed record UpdateBookCommand(Book Book) : IRequest<bool>;

}
