using Libro.Domain.Entities;
using Libro.Application.Interfaces;
using Libro.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Role> GetRoleByNameAsync(string RoleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == RoleName);
         
        }
        public async Task<Role> AddRoleAsync(Role role)
        {
            role.Name = role.Name.ToLower();
            if (await RoleNameExistsAsync(role.Name))
            { throw new RoleExistsException(role.Name); }
            role.Id = Guid.NewGuid();
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        private async Task<bool> RoleNameExistsAsync(string RoleName)
        {
            if (await _context.Roles.FirstOrDefaultAsync(r=>r.Name== RoleName) is null)
                return false;
            return true;
        }

       
    }
}
