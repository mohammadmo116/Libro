using MediatR;

namespace Libro.Application.Users.Commands
{
    public sealed record RemoveUserCommand(Guid UserId) : IRequest<bool>;

}
