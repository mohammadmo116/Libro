using Libro.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Libro.Infrastructure.Authorization
{
    public sealed class HasRoleAttribute : AuthorizeAttribute
    {
     
        public HasRoleAttribute(RoleType Role) : base(policy : Role.ToString()) { }
    }
}
