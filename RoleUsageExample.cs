using Microsoft.AspNetCore.Mvc;
using ArtisanMarketplace.Models.Roles;
using ArtisanMarketplace.Services;
using ArtisanMarketplace.Attributes;

namespace ArtisanMarketplace.Controllers
{
    /// <summary>
    /// Example controller demonstrating role and permission usage
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public BookingController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Create a booking - requires CREATE_BOOKING permission
        /// </summary>
        [HttpPost]
        [RequirePermission(Permissions.CreateBooking)]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            // This endpoint is accessible to users with CREATE_BOOKING permission
            return Ok(new { message = "Booking created successfully" });
        }

        /// <summary>
        /// View booking requests - only for artisans
        /// </summary>
        [HttpGet("requests")]
        [RequirePermission(Permissions.ViewBookingRequests)]
        public async Task<IActionResult> GetBookingRequests()
        {
            // Only artisans can access this
            return Ok(new { requests = new List<object>() });
        }

        /// <summary>
        /// Accept a booking - artisans only
        /// </summary>
        [HttpPost("{id}/accept")]
        [RequirePermission(Permissions.AcceptBooking)]
        public async Task<IActionResult> AcceptBooking(Guid id)
        {
            return Ok(new { message = "Booking accepted" });
        }
    }

    /// <summary>
    /// Admin controller - requires admin role
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [RequireRole(RoleTypes.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public AdminController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Assign a role to a user
        /// </summary>
        [HttpPost("users/{userId}/roles")]
        public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleRequest request)
        {
            try
            {
                var role = await _roleService.AssignRoleAsync(
                    userId, 
                    request.RoleType, 
                    request.IsPrimary
                );

                return Ok(new 
                { 
                    message = $"Role {role.GetRoleDisplayName()} assigned successfully",
                    role = new
                    {
                        roleType = role.RoleType,
                        displayName = role.GetRoleDisplayName(),
                        permissions = role.GetPermissions()
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get user's roles and permissions
        /// </summary>
        [HttpGet("users/{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(Guid userId)
        {
            var roles = await _roleService.GetUserRolesAsync(userId);
            var permissions = await _roleService.GetUserPermissionsAsync(userId);
            var primaryRole = await _roleService.GetPrimaryRoleAsync(userId);

            return Ok(new
            {
                userId,
                primaryRole = primaryRole?.GetRoleDisplayName(),
                roles = roles.Select(r => new
                {
                    roleType = r.RoleType,
                    displayName = r.GetRoleDisplayName(),
                    priority = r.GetPriorityLevel(),
                    isPrimary = r.IsPrimary
                }),
                permissions = permissions.Distinct().OrderBy(p => p).ToList()
            });
        }

        /// <summary>
        /// Remove a role from a user
        /// </summary>
        [HttpDelete("users/{userId}/roles/{roleType}")]
        public async Task<IActionResult> RemoveRole(Guid userId, string roleType)
        {
            var success = await _roleService.RemoveRoleAsync(userId, roleType);
            
            if (!success)
                return NotFound(new { error = "Role not found" });

            return Ok(new { message = "Role removed successfully" });
        }
    }

    /// <summary>
    /// Moderator controller - requires moderator or admin role
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [RequireRole(RoleTypes.Moderator, RoleTypes.Admin)]
    public class ModerationController : ControllerBase
    {
        /// <summary>
        /// View reported content
        /// </summary>
        [HttpGet("reports")]
        [RequirePermission(Permissions.ViewReports)]
        public async Task<IActionResult> GetReports()
        {
            return Ok(new { reports = new List<object>() });
        }

        /// <summary>
        /// Handle a report
        /// </summary>
        [HttpPost("reports/{id}/handle")]
        [RequirePermission(Permissions.HandleReports)]
        public async Task<IActionResult> HandleReport(Guid id)
        {
            return Ok(new { message = "Report handled" });
        }

        /// <summary>
        /// Suspend a user
        /// </summary>
        [HttpPost("users/{userId}/suspend")]
        [RequirePermission(Permissions.SuspendUser)]
        public async Task<IActionResult> SuspendUser(Guid userId)
        {
            return Ok(new { message = "User suspended" });
        }
    }

    // Request models
    public class CreateBookingRequest
    {
        public Guid ArtisanId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
    }

    public class AssignRoleRequest
    {
        public string RoleType { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}