using MediatR;

namespace Libro.Application.Users.Commands
{
    public sealed record RemoveUserByRoleCommand(Guid UserId, string RoleName) : IRequest<bool>;

}
