using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Authors.Commands
{
    public sealed record UpdateAuthorCommand(Author author) : IRequest<bool>;

}
