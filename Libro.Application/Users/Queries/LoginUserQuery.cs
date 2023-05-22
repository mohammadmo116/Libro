using MediatR;

namespace Libro.Application.Users.Queries
{
    public sealed record LoginUserQuery(string Email, string Password) : IRequest<string>;
    
}
