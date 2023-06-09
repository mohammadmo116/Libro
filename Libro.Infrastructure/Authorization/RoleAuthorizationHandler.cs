
using Microsoft.AspNetCore.Authorization;
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
            string? userId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
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
