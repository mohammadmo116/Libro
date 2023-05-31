using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Authorization
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HashSet<string>> GetRolesAsync(Guid UserId)
        {
            var roleIds = await _context.UserRoles.Where(e => e.UserId == UserId).Select(r => r.RoleId).ToListAsync();
            var roles = _context.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToHashSet();
            return roles;




        }
    }
}
