using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> AddRoleAsync(Role role);
   
    }
}