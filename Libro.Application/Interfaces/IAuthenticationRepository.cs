using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IAuthenticationRepository
    {
        
        Task<User> RegisterUserAsync(User user);
        Task<string> Authenticate(User User);
        Task ExceptionIfUserExistsAsync(User User);
        Task<User?> ValidateUserCredentialsAsync(string email, string password);

    }
}