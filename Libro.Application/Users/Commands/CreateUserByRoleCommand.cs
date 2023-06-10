using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Commands
{
    public sealed record CreateUserByRoleCommand(User User, string RoleName) : IRequest<User>;
}
