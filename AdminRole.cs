namespace ArtisanMarketplace.Models.Roles
{
    /// <summary>
    /// Administrator role with full system access
    /// </summary>
    public class AdminRole : BaseRole
    {
        public AdminRole()
        {
            RoleType = RoleTypes.Admin;
        }

        public override string GetRoleDisplayName()
        {
            return "Administrator";
        }

        public override List<string> GetPermissions()
        {
            // Admins have all permissions
            return new List<string>(Permissions.AdminPermissions)
                .Concat(Permissions.BasicUserPermissions)
                .Concat(Permissions.ArtisanPermissions)
                .Distinct()
                .ToList();
        }

        public override int GetPriorityLevel()
        {
            return 100; // Highest priority
        }
    }
}