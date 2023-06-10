using Microsoft.AspNetCore.Authorization;

namespace Libro.Infrastructure.Authorization
{
    public sealed class HasRoleAttribute : AuthorizeAttribute
    {

        public HasRoleAttribute(string Roles) : base(policy: Roles.ToLower()) { }
    }
}
