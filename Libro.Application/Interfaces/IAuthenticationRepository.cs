using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<User> RegisterUser(User user);
    }
}