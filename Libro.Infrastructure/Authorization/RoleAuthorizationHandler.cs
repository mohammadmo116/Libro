using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;


namespace Libro.Infrastructure.Authorization
{
    public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public RoleAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleRequirement requirement)
        {
            string? userId = null;
            if (requirement.Roles.StartsWith(nameof(HasRoleAttribute), StringComparison.OrdinalIgnoreCase))
            {
                userId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                requirement.Roles = requirement.Roles[nameof(HasRoleAttribute).Length..];

            }
            if (requirement.Roles.StartsWith(nameof(ToRoleAttribute), StringComparison.OrdinalIgnoreCase))
            {

                if (context.Resource is HttpContext httpContext)
                {

                    userId = (string)httpContext.Request.RouteValues.FirstOrDefault(a => a.Key == "UserId").Value;
                    requirement.Roles = requirement.Roles[nameof(ToRoleAttribute).Length..];
                }
            }
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                return;
            }


            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IRoleService roleService = scope.ServiceProvider
            .GetRequiredService<IRoleService>();


            HashSet<string> roles = await roleService
                .GetRolesAsync(parsedUserId);

            var GivenRoles = requirement.Roles
                .Trim()
                .Replace(" ", "")
                .Split(',')
                .ToHashSet<string>();


            if (roles.Intersect(GivenRoles).Any())
            {
                context.Succeed(requirement);
            }

        }
    }
}
