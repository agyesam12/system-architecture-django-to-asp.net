using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ArtisanMarketplace.Models.Roles;

namespace ArtisanMarketplace.Models
{
    [Table("Users")]
    public class User : IdentityUser<Guid>
    {
        public User()
        {
            Id = Guid.NewGuid();
            DateJoined = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
            IsActive = true;
            IsVerified = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public override string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(17)]
        [RegularExpression(@"^\+?1?\d{9,15}$", 
            ErrorMessage = "Phone number must be entered in the format: '+999999999'. Up to 15 digits allowed.")]
        public override string? PhoneNumber { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Profile Picture Path")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Is Verified")]
        public bool IsVerified { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string? Bio { get; set; }

        // Address Fields
        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        // Navigation Properties
        public virtual ICollection<BaseRole> Roles { get; set; } = new List<BaseRole>();
        public virtual ArtisanProfile? ArtisanProfile { get; set; }
        public virtual ICollection<UserFeed> JobRequests { get; set; } = new List<UserFeed>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
        public virtual ICollection<Report> ReportsMade { get; set; } = new List<Report>();
        public virtual ICollection<Report> ReportsReceived { get; set; } = new List<Report>();
        public virtual ICollection<Report> ReportsReviewed { get; set; } = new List<Report>();

        // Helper Methods for Roles
        public BaseRole? GetPrimaryRole()
        {
            return Roles.FirstOrDefault(r => r.IsPrimary && r.IsActive);
        }

        public List<BaseRole> GetActiveRoles()
        {
            return Roles.Where(r => r.IsActive).ToList();
        }

        public bool HasRole(string roleType)
        {
            return Roles.Any(r => r.RoleType.Equals(roleType, StringComparison.OrdinalIgnoreCase) 
                && r.IsActive);
        }

        public bool HasPermission(string permission)
        {
            return Roles.Any(r => r.IsActive && r.HasPermission(permission));
        }

        public List<string> GetAllPermissions()
        {
            var permissions = new HashSet<string>();
            foreach (var role in Roles.Where(r => r.IsActive))
            {
                foreach (var permission in role.GetPermissions())
                {
                    permissions.Add(permission);
                }
            }
            return permissions.ToList();
        }

        public bool IsAdmin()
        {
            return HasRole(RoleTypes.Admin);
        }

        public bool IsModerator()
        {
            return HasRole(RoleTypes.Moderator);
        }

        public bool IsArtisan()
        {
            return Roles.Any(r => r.IsActive && 
                (r.RoleType == RoleTypes.Artisan ||
                 r.RoleType == RoleTypes.Mason ||
                 r.RoleType == RoleTypes.Plumber ||
                 r.RoleType == RoleTypes.Electrician ||
                 r.RoleType == RoleTypes.Carpenter ||
                 r.RoleType == RoleTypes.Painter ||
                 r.RoleType == RoleTypes.Tiler ||
                 r.RoleType == RoleTypes.Roofer));
        }

        public List<string> GetArtisanSpecializations()
        {
            var specializations = new List<string>();
            
            foreach (var role in Roles.Where(r => r.IsActive))
            {
                switch (role)
                {
                    case PlumberRole plumber:
                        specializations.AddRange(plumber.Specializations);
                        break;
                    case ElectricianRole electrician:
                        specializations.AddRange(electrician.Specializations);
                        break;
                    case MasonRole mason:
                        specializations.AddRange(mason.Specializations);
                        break;
                    case CarpenterRole carpenter:
                        specializations.AddRange(carpenter.Specializations);
                        break;
                    case PainterRole painter:
                        specializations.AddRange(painter.Specializations);
                        break;
                    case TilerRole tiler:
                        specializations.AddRange(tiler.Specializations);
                        break;
                    case RooferRole roofer:
                        specializations.AddRange(roofer.Specializations);
                        break;
                    case ArtisanRole artisan:
                        specializations.AddRange(artisan.Specializations);
                        break;
                }
            }

            return specializations.Distinct().ToList();
        }

        public string GetRolesSummary()
        {
            var activeRoles = GetActiveRoles();
            if (!activeRoles.Any())
                return "No Active Roles";

            var primaryRole = GetPrimaryRole();
            if (primaryRole != null)
                return $"{primaryRole.GetRoleDisplayName()} (Primary)" + 
                       (activeRoles.Count > 1 ? $" +{activeRoles.Count - 1} more" : "");

            return string.Join(", ", activeRoles.Select(r => r.GetRoleDisplayName()));
        }

        // Address Helper
        public string GetFullAddress()
        {
            var addressParts = new[] { Address, City, State, Country, PostalCode }
                .Where(s => !string.IsNullOrWhiteSpace(s));
            return string.Join(", ", addressParts);
        }

        public override string ToString()
        {
            return $"{FullName} ({Email}) - {GetRolesSummary()}";
        }

        // Update timestamp on save
        public void UpdateTimestamp()
        {
            LastUpdated = DateTime.UtcNow;
        }
    }
}