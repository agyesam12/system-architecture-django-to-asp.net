using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtisanMarketplace.Models
{

    [Table("ArtisanProfiles")]
    [Index(nameof(Slug), IsUnique = true)]
    public class ArtisanProfile
    {
        public ArtisanProfile()
        {
            Id = Guid.NewGuid();
            AverageRating = 0.0m;
            TotalReviews = 0;
            CompletedProjects = 0;
            AvailabilityStatus = AvailabilityStatuses.Available;
            IsVerified = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Foreign Key - One-to-One relationship with User
        [Required]
        [Display(Name = "User")]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Display(Name = "URL Slug")]
        public string Slug { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        // Professional Details
        [Required]
        [Range(0, 100)]
        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Experience Level")]
        public string ExperienceLevel { get; set; } = ExperienceLevels.Beginner;

        [StringLength(100)]
        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }

        [StringLength(500)]
        [Display(Name = "Certification Path")]
        public string? Certification { get; set; }

        // Business Information
        [StringLength(100)]
        [Display(Name = "Business Registration")]
        public string? BusinessRegistration { get; set; }

        [StringLength(50)]
        [Display(Name = "Tax ID")]
        public string? TaxId { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Insurance Details")]
        public string? InsuranceDetails { get; set; }

        // Ratings and Reputation
        [Range(0.0, 5.0)]
        [Column(TypeName = "decimal(3,2)")]
        [Display(Name = "Average Rating")]
        public decimal AverageRating { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Total Reviews")]
        public int TotalReviews { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Completed Projects")]
        public int CompletedProjects { get; set; }

        // Availability
        [Required]
        [StringLength(20)]
        [Display(Name = "Availability Status")]
        public string AvailabilityStatus { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Hourly Rate")]
        public decimal? HourlyRate { get; set; }

        [Range(0, 1000)]
        [Display(Name = "Service Radius (km)")]
        public int? ServiceRadius { get; set; }

        // Professional Description
        [StringLength(2000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "About")]
        public string? About { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Services Offered")]
        public string ServicesOffered { get; set; } = string.Empty;

        // Verification
        [Display(Name = "Is Verified")]
        public bool IsVerified { get; set; }

        [Display(Name = "Verified Date")]
        public DateTime? VerifiedDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Verification Documents Path")]
        public string? VerificationDocuments { get; set; }

        // Timestamps
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<ArtisanWork> PortfolioWorks { get; set; } = new List<ArtisanWork>();
        public virtual ICollection<ArtisanFeed> FeedPosts { get; set; } = new List<ArtisanFeed>();
        public virtual ICollection<ArtisanProposal> Proposals { get; set; } = new List<ArtisanProposal>();

        public List<string> GetServicesAsList()
        {
            return ServicesOffered
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();
        }

        public void SetServicesFromList(List<string> services)
        {
            ServicesOffered = string.Join(", ", services);
        }

        public string GetExperienceLevelDisplay()
        {
            return ExperienceLevel switch
            {
                ExperienceLevels.Beginner => "0-2 years",
                ExperienceLevels.Intermediate => "2-5 years",
                ExperienceLevels.Experienced => "5-10 years",
                ExperienceLevels.Expert => "10+ years",
                _ => ExperienceLevel
            };
        }

        public string GetAvailabilityStatusDisplay()
        {
            return AvailabilityStatus switch
            {
                AvailabilityStatuses.Available => "Available",
                AvailabilityStatuses.Busy => "Busy",
                AvailabilityStatuses.Unavailable => "Unavailable",
                _ => AvailabilityStatus
            };
        }

        public void UpdateRating(decimal newRating)
        {
            var totalRatingPoints = AverageRating * TotalReviews;
            TotalReviews++;
            AverageRating = (totalRatingPoints + newRating) / TotalReviews;
            AverageRating = Math.Round(AverageRating, 2);
        }

        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{BusinessName} - {User?.FullName}";
        }
    }

    
    public static class ExperienceLevels
    {
        public const string Beginner = "BEGINNER";
        public const string Intermediate = "INTERMEDIATE";
        public const string Experienced = "EXPERIENCED";
        public const string Expert = "EXPERT";

        public static readonly string[] AllLevels = 
        {
            Beginner, Intermediate, Experienced, Expert
        };

        public static readonly Dictionary<string, string> LevelDisplayNames = new()
        {
            { Beginner, "0-2 years" },
            { Intermediate, "2-5 years" },
            { Experienced, "5-10 years" },
            { Expert, "10+ years" }
        };

        public static bool IsValidLevel(string level)
        {
            return AllLevels.Contains(level.ToUpper());
        }

        public static string GetDisplayName(string level)
        {
            return LevelDisplayNames.TryGetValue(level.ToUpper(), out var displayName) 
                ? displayName 
                : level;
        }
    }

    
    public static class AvailabilityStatuses
    {
        public const string Available = "AVAILABLE";
        public const string Busy = "BUSY";
        public const string Unavailable = "UNAVAILABLE";

        public static readonly string[] AllStatuses = 
        {
            Available, Busy, Unavailable
        };

        public static readonly Dictionary<string, string> StatusDisplayNames = new()
        {
            { Available, "Available" },
            { Busy, "Busy" },
            { Unavailable, "Unavailable" }
        };

        public static bool IsValidStatus(string status)
        {
            return AllStatuses.Contains(status.ToUpper());
        }

        public static string GetDisplayName(string status)
        {
            return StatusDisplayNames.TryGetValue(status.ToUpper(), out var displayName) 
                ? displayName 
                : status;
        }
    }
}