using Microsoft.AspNetCore.Authorization;

namespace Libro.Infrastructure.Authorization
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public RoleRequirement(string roles) {
            Roles = roles;
        }
        public string Roles { get; set; }
    }
}
