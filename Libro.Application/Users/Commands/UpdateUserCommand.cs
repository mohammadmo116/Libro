using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.Users.Commands
{
    public sealed record UpdateUserCommand(User user) : IRequest<bool>;


}
