using Microsoft.AspNetCore.Authorization;

namespace Libro.Infrastructure.Authorization
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public RoleRequirement(string roles)
        {
            if (roles.StartsWith(nameof(HasRoleAttribute), StringComparison.OrdinalIgnoreCase))
            {
                Roles = roles[nameof(HasRoleAttribute).Length..]
                .Trim()
                .Replace(" ", "")
                .Split(',')
                .ToHashSet();

                AttrbuteName = nameof(HasRoleAttribute);
            }
            if (roles.StartsWith(nameof(ToRoleAttribute), StringComparison.OrdinalIgnoreCase))
            {

                Roles = roles[nameof(ToRoleAttribute).Length..].Trim()
                .Replace(" ", "")
                .Split(',')
                .ToHashSet();

                AttrbuteName = nameof(ToRoleAttribute);

            }

        }
        public HashSet<string> Roles { get; set; }
        public string AttrbuteName { get; set; }
    }
}
