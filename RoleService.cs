namespace ArtisanMarketplace.Services
{
    using ArtisanMarketplace.Models.Roles;

    /// <summary>
    /// Service for managing user roles and permissions
    /// </summary>
    public interface IRoleService
    {
        Task<List<BaseRole>> GetUserRolesAsync(Guid userId);
        Task<bool> UserHasPermissionAsync(Guid userId, string permission);
        Task<bool> UserHasRoleAsync(Guid userId, string roleType);
        Task<BaseRole> AssignRoleAsync(Guid userId, string roleType, bool isPrimary = false);
        Task<bool> RemoveRoleAsync(Guid userId, string roleType);
        Task<BaseRole?> GetPrimaryRoleAsync(Guid userId);
        Task<List<string>> GetUserPermissionsAsync(Guid userId);
    }

    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BaseRole>> GetUserRolesAsync(Guid userId)
        {
            var roleEntities = await _context.Roles
                .Where(r => r.UserId == userId && r.IsActive)
                .ToListAsync();

            // Convert to specific role types
            return roleEntities.Select(r => RoleFactory.CreateRole(r.RoleType)).ToList();
        }

        public async Task<bool> UserHasPermissionAsync(Guid userId, string permission)
        {
            var roles = await GetUserRolesAsync(userId);
            return roles.Any(role => role.HasPermission(permission));
        }

        public async Task<bool> UserHasRoleAsync(Guid userId, string roleType)
        {
            return await _context.Roles
                .AnyAsync(r => r.UserId == userId 
                    && r.RoleType.ToUpper() == roleType.ToUpper() 
                    && r.IsActive);
        }

        public async Task<BaseRole> AssignRoleAsync(Guid userId, string roleType, bool isPrimary = false)
        {
            // Check if role already exists
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.UserId == userId && r.RoleType == roleType.ToUpper());

            if (existingRole != null)
            {
                existingRole.IsActive = true;
                existingRole.IsPrimary = isPrimary;
                await _context.SaveChangesAsync();
                return RoleFactory.CreateRole(roleType);
            }

            // Create new role
            var newRole = RoleFactory.CreateRoleForUser(userId, roleType);
            newRole.IsPrimary = isPrimary;

            // If setting as primary, unset other primary roles
            if (isPrimary)
            {
                var existingPrimaryRoles = await _context.Roles
                    .Where(r => r.UserId == userId && r.IsPrimary)
                    .ToListAsync();

                foreach (var role in existingPrimaryRoles)
                {
                    role.IsPrimary = false;
                }
            }

            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();

            return newRole;
        }

        public async Task<bool> RemoveRoleAsync(Guid userId, string roleType)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.UserId == userId && r.RoleType == roleType.ToUpper());

            if (role == null)
                return false;

            role.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BaseRole?> GetPrimaryRoleAsync(Guid userId)
        {
            var primaryRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.UserId == userId && r.IsPrimary && r.IsActive);

            if (primaryRole == null)
                return null;

            return RoleFactory.CreateRole(primaryRole.RoleType);
        }

        public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
        {
            var roles = await GetUserRolesAsync(userId);
            
            var permissions = new HashSet<string>();
            foreach (var role in roles)
            {
                foreach (var permission in role.GetPermissions())
                {
                    permissions.Add(permission);
                }
            }

            return permissions.ToList();
        }
    }
}