using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Authors.Commands
{
    public sealed record CreateAuthorCommand(Author author) : IRequest<Author>;

}
