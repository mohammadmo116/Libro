using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Commands
{
    public sealed record CreateUserCommand(User User) : IRequest<User>;

}
