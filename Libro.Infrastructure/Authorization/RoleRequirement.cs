using Microsoft.AspNetCore.Authorization;

namespace Libro.Infrastructure.Authorization
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public RoleRequirement(string role) {
            Role = role;
        }
        public string Role { get; set; }
    }
}
