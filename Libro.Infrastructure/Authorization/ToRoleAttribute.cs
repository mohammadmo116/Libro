using Microsoft.AspNetCore.Authorization;

namespace Libro.Infrastructure.Authorization
{
    public sealed class ToRoleAttribute : AuthorizeAttribute
    {
        public ToRoleAttribute(string Roles) : base(policy: nameof(ToRoleAttribute) + Roles.ToLower()) { }
    }
}
