using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Libro.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<string>> AssignRoleToUserAsync(UserRole userRole)
        {
         
            if (await RoleOrUserNotFoundAsync(userRole))
            { throw new UserOrRoleNotFoundException(); }
            if (await UserHasTheAssignedRoleAsync(userRole))
            { throw new UserHasTheAssignedRoleException(); }

            await _context.AddAsync(userRole);
            await _context.SaveChangesAsync();
            var roleIds = await _context.UserRoles.Where(e => e.UserId == userRole.UserId).Select(r => r.RoleId).ToListAsync();
            return  await _context.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToListAsync();
        
        }

        private async Task<bool> UserHasTheAssignedRoleAsync(UserRole userRole) 
        {

            return await _context.UserRoles.Where(e=>e.RoleId== userRole.RoleId).AnyAsync(e=>e.UserId==userRole.UserId);
        }

        private async Task<bool> RoleOrUserNotFoundAsync(UserRole userRole)
        {
           
            if (await _context.Users.FindAsync(userRole.UserId) is null)
                return true;
            if (await _context.Roles.FindAsync(userRole.RoleId) is null)
                return true;
            return false;
        }
    }
}

