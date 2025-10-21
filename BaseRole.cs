using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtisanMarketplace.Models.Roles
{
    /// <summary>
    /// Base role model for all user roles
    /// </summary>
    [Table("Roles")]
    [Index(nameof(UserId), nameof(RoleType), IsUnique = true)]
    public abstract class BaseRole
    {
        protected BaseRole()
        {
            Id = Guid.NewGuid();
            AssignedDate = DateTime.UtcNow;
            IsActive = true;
            IsPrimary = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "User")]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Role Type")]
        public string RoleType { get; set; } = string.Empty;

        [Display(Name = "Is Primary Role")]
        public bool IsPrimary { get; set; }

        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        // Abstract methods to be implemented by derived classes
        public abstract string GetRoleDisplayName();
        public abstract List<string> GetPermissions();
        public abstract int GetPriorityLevel();

        public virtual bool HasPermission(string permission)
        {
            return GetPermissions().Contains(permission);
        }

        public override string ToString()
        {
            return $"{User?.FullName} - {GetRoleDisplayName()}";
        }
    }
}