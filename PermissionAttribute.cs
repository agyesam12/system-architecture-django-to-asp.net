using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ArtisanMarketplace.Services;

namespace ArtisanMarketplace.Attributes
{
    /// <summary>
    /// Authorization attribute for permission-based access control
    /// Usage: [RequirePermission(Permissions.CreateBooking)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _permissions;

        public RequirePermissionAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Get the current user ID from claims
            var userIdClaim = context.HttpContext.User.FindFirst("UserId")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get role service from DI
            var roleService = context.HttpContext.RequestServices
                .GetService<IRoleService>();

            if (roleService == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // Check if user has any of the required permissions
            foreach (var permission in _permissions)
            {
                if (await roleService.UserHasPermissionAsync(userId, permission))
                {
                    return; // User has permission, allow access
                }
            }

            // User doesn't have any required permission
            context.Result = new ForbidResult();
        }
    }

    /// <summary>
    /// Authorization attribute for role-based access control
    /// Usage: [RequireRole(RoleTypes.Admin, RoleTypes.Moderator)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequireRoleAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;

        public RequireRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst("UserId")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleService = context.HttpContext.RequestServices
                .GetService<IRoleService>();

            if (roleService == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // Check if user has any of the required roles
            foreach (var role in _roles)
            {
                if (await roleService.UserHasRoleAsync(userId, role))
                {
                    return; // User has role, allow access
                }
            }

            context.Result = new ForbidResult();
        }
    }
}