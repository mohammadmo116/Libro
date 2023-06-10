using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Queries
{
    public sealed record GetUserQuery(Guid UserId) : IRequest<User>;

}
