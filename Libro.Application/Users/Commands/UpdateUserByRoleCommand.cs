using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Commands
{
    public sealed record UpdateUserByRoleCommand(User user, string RoleName) : IRequest<bool>;

}
