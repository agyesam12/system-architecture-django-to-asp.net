using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtisanMarketplace.Models
{

    [Table("ArtisanWorks")]
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(ArtisanId), nameof(CreatedAt))]
    public class ArtisanWork
    {
        public ArtisanWork()
        {
            Id = Guid.NewGuid();
            ProjectStatus = ProjectStatuses.Completed;
            ViewsCount = 0;
            LikesCount = 0;
            IsFeatured = false;
            IsPublic = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Foreign Key
        [Required]
        [Display(Name = "Artisan")]
        public Guid ArtisanId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Display(Name = "URL Slug")]
        public string Slug { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        // Project Details
        [Required]
        [StringLength(100)]
        [Display(Name = "Project Type")]
        public string ProjectType { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Project Status")]
        public string ProjectStatus { get; set; }

        [Required]
        [Range(1, 3650)]
        [Display(Name = "Duration (Days)")]
        public int DurationDays { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Project Cost")]
        public decimal? ProjectCost { get; set; }

        // Location
        [Required]
        [StringLength(255)]
        public string Location { get; set; } = string.Empty;

        // Media
        [Required]
        [StringLength(500)]
        [Display(Name = "Featured Image Path")]
        public string FeaturedImage { get; set; } = string.Empty;

        // Client Information (Optional)
        [StringLength(255)]
        [Display(Name = "Client Name")]
        public string? ClientName { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Client Testimonial")]
        public string? ClientTestimonial { get; set; }

        [Range(1, 5)]
        [Display(Name = "Client Rating")]
        public int? ClientRating { get; set; }

        // Engagement Metrics
        [Range(0, int.MaxValue)]
        [Display(Name = "Views Count")]
        public int ViewsCount { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Likes Count")]
        public int LikesCount { get; set; }

        // Timestamps
        [Display(Name = "Completion Date")]
        [DataType(DataType.Date)]
        public DateTime? CompletionDate { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }

        // Visibility
        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Is Public")]
        public bool IsPublic { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(ArtisanId))]
        public virtual ArtisanProfile Artisan { get; set; } = null!;

        public virtual ICollection<ArtisanWorkImage> Images { get; set; } = new List<ArtisanWorkImage>();

        // Helper Methods
        public string GetProjectStatusDisplay()
        {
            return ProjectStatus switch
            {
                ProjectStatuses.Completed => "Completed",
                ProjectStatuses.InProgress => "In Progress",
                ProjectStatuses.Planned => "Planned",
                _ => ProjectStatus
            };
        }

        public void IncrementViews()
        {
            ViewsCount++;
        }

        public void IncrementLikes()
        {
            LikesCount++;
        }

        public void DecrementLikes()
        {
            if (LikesCount > 0)
                LikesCount--;
        }

        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{Title} by {Artisan?.BusinessName}";
        }
    }


    [Table("ArtisanWorkImages")]
    [Index(nameof(WorkId), nameof(Order))]
    public class ArtisanWorkImage
    {
        public ArtisanWorkImage()
        {
            Id = Guid.NewGuid();
            Order = 0;
            UploadedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Foreign Key
        [Required]
        [Display(Name = "Artisan Work")]
        public Guid WorkId { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Image Path")]
        public string Image { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Caption { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Display Order")]
        public int Order { get; set; }

        [Display(Name = "Uploaded At")]
        public DateTime UploadedAt { get; set; }

        // Navigation Property
        [ForeignKey(nameof(WorkId))]
        public virtual ArtisanWork Work { get; set; } = null!;

        public override string ToString()
        {
            return $"Image for {Work?.Title}";
        }
    }

    
    public static class ProjectStatuses
    {
        public const string Completed = "COMPLETED";
        public const string InProgress = "IN_PROGRESS";
        public const string Planned = "PLANNED";

        public static readonly string[] AllStatuses = 
        {
            Completed, InProgress, Planned
        };

        public static readonly Dictionary<string, string> StatusDisplayNames = new()
        {
            { Completed, "Completed" },
            { InProgress, "In Progress" },
            { Planned, "Planned" }
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