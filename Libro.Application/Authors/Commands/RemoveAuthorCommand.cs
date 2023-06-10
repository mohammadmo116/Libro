using MediatR;

namespace Libro.Application.Authors.Commands
{
    public sealed record class RemoveAuthorCommand(Guid AuthorId) : IRequest<bool>;

}
