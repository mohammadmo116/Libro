using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<string>> AssignRoleToUserAsync(UserRole userRole);
    }
}