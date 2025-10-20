using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtisanMarketplace.Models
{
    /// <summary>
    /// Role model to manage user types and permissions
    /// Allows users to have multiple roles (e.g., both User and Artisan)
    /// </summary>
    [Table("Roles")]
    [Index(nameof(UserId), nameof(RoleType), IsUnique = true)]
    public class Role
    {
        public Role()
        {
            Id = Guid.NewGuid();
            AssignedDate = DateTime.UtcNow;
            IsActive = true;
            IsPrimary = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Foreign Key
        [Required]
        [Display(Name = "User")]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Role Type")]
        public string RoleType { get; set; } = RoleTypes.User;

        [Display(Name = "Is Primary Role")]
        public bool IsPrimary { get; set; }

        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        // Navigation Property
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        // Helper Methods
        public string GetRoleDisplayName()
        {
            return RoleType switch
            {
                RoleTypes.User => "Regular User",
                RoleTypes.Artisan => "Artisan/Contractor",
                RoleTypes.Mason => "Mason",
                RoleTypes.Plumber => "Plumber",
                RoleTypes.Electrician => "Electrician",
                RoleTypes.Carpenter => "Carpenter",
                RoleTypes.Painter => "Painter",
                RoleTypes.Tiler => "Tiler",
                RoleTypes.Roofer => "Roofer",
                RoleTypes.Admin => "Administrator",
                RoleTypes.Moderator => "Moderator",
                _ => RoleType
            };
        }

        public override string ToString()
        {
            return $"{User?.FullName} - {GetRoleDisplayName()}";
        }
    }

    /// <summary>
    /// Constants for role types
    /// </summary>
    public static class RoleTypes
    {
        public const string User = "USER";
        public const string Artisan = "ARTISAN";
        public const string Mason = "MASON";
        public const string Plumber = "PLUMBER";
        public const string Electrician = "ELECTRICIAN";
        public const string Carpenter = "CARPENTER";
        public const string Painter = "PAINTER";
        public const string Tiler = "TILER";
        public const string Roofer = "ROOFER";
        public const string Admin = "ADMIN";
        public const string Moderator = "MODERATOR";

        public static readonly string[] AllRoles = 
        {
            User, Artisan, Mason, Plumber, Electrician, 
            Carpenter, Painter, Tiler, Roofer, Admin, Moderator
        };

        public static readonly Dictionary<string, string> RoleDisplayNames = new()
        {
            { User, "Regular User" },
            { Artisan, "Artisan/Contractor" },
            { Mason, "Mason" },
            { Plumber, "Plumber" },
            { Electrician, "Electrician" },
            { Carpenter, "Carpenter" },
            { Painter, "Painter" },
            { Tiler, "Tiler" },
            { Roofer, "Roofer" },
            { Admin, "Administrator" },
            { Moderator, "Moderator" }
        };

        public static bool IsValidRole(string roleType)
        {
            return AllRoles.Contains(roleType.ToUpper());
        }

        public static string GetDisplayName(string roleType)
        {
            return RoleDisplayNames.TryGetValue(roleType.ToUpper(), out var displayName) 
                ? displayName 
                : roleType;
        }
    }
}