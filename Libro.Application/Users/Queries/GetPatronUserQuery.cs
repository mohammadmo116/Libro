using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Queries
{
    public sealed record class GetPatronUserQuery(Guid PatronId) : IRequest<User>;
}
