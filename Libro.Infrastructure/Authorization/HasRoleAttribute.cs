using Microsoft.AspNetCore.Authorization;

namespace Libro.Infrastructure.Authorization
{
    public sealed class HasRoleAttribute : AuthorizeAttribute
    {

        public HasRoleAttribute(string roles) : base(policy: nameof(HasRoleAttribute) + roles.ToLower())
        {

        }
    }
}
