using MediatR;

namespace Libro.Application.Books.Commands
{
    public sealed record RemoveBookCommand(Guid BookId) : IRequest<bool>;

}
