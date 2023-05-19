using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IAuthenticationRepository
    {
        
        Task<User> RegisterUserAsync(User user);
        Task<string> Authenticate(string Email, string Password);
    }
}