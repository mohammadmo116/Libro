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
            if (requirement.AttrbuteName == nameof(HasRoleAttribute))
            {
                userId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            }
            if (requirement.AttrbuteName == nameof(ToRoleAttribute))
            {
                if (context.Resource is HttpContext httpContext)
                {

                    userId = (string)httpContext.Request.RouteValues.FirstOrDefault(a => a.Key == "UserId").Value;
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



            if (roles.Intersect(requirement.Roles).Any())
            {
                context.Succeed(requirement);
            }

        }
    }
}
