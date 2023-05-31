using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Commands
{
    public sealed record AddRoleToUserCommand(UserRole UserRole) : IRequest<bool>;
}
