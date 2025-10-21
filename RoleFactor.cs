namespace ArtisanMarketplace.Models.Roles
{
    /// <summary>
    /// Factory for creating role instances based on role type
    /// </summary>
    public static class RoleFactory
    {
        public static BaseRole CreateRole(string roleType)
        {
            return roleType.ToUpper() switch
            {
                RoleTypes.User => new UserRole(),
                RoleTypes.Artisan => new ArtisanRole(),
                RoleTypes.Mason => new MasonRole(),
                RoleTypes.Plumber => new PlumberRole(),
                RoleTypes.Electrician => new ElectricianRole(),
                RoleTypes.Carpenter => new CarpenterRole(),
                RoleTypes.Painter => new PainterRole(),
                RoleTypes.Tiler => new TilerRole(),
                RoleTypes.Roofer => new RooferRole(),
                RoleTypes.Admin => new AdminRole(),
                RoleTypes.Moderator => new ModeratorRole(),
                _ => throw new ArgumentException($"Invalid role type: {roleType}")
            };
        }

        public static BaseRole CreateRoleForUser(Guid userId, string roleType)
        {
            var role = CreateRole(roleType);
            role.UserId = userId;
            return role;
        }

        public static Dictionary<string, string> GetAllRoleDisplayNames()
        {
            return new Dictionary<string, string>
            {
                { RoleTypes.User, "Regular User" },
                { RoleTypes.Artisan, "Artisan/Contractor" },
                { RoleTypes.Mason, "Mason" },
                { RoleTypes.Plumber, "Plumber" },
                { RoleTypes.Electrician, "Electrician" },
                { RoleTypes.Carpenter, "Carpenter" },
                { RoleTypes.Painter, "Painter" },
                { RoleTypes.Tiler, "Tiler" },
                { RoleTypes.Roofer, "Roofer" },
                { RoleTypes.Admin, "Administrator" },
                { RoleTypes.Moderator, "Moderator" }
            };
        }
    }
}