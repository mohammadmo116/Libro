using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IAuthenticationRepository
    {

        Task<string> Authenticate(User User);
        Task<User?> ValidateUserCredentialsAsync(string email, string password);

    }
}