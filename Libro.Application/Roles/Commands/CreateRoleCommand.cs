using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Roles.Commands
{
    public sealed record CreateRoleCommand(Role Role) : IRequest<Role>;

}
