using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Authors.Queries
{
    public sealed record GetAuthorQuery(Guid AuthorId) : IRequest<Author>;

}
