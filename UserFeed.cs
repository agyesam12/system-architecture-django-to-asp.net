using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtisanMarketplace.Models
{
    /// <summary>
    /// Feed model for users posting job requests with invoices
    /// Users upload invoices and descriptions to get better quotes from artisans
    /// </summary>
    [Table("UserFeeds")]
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(Status), nameof(CreatedAt))]
    [Index(nameof(JobCategory), nameof(Status))]
    public class UserFeed
    {
        public UserFeed()
        {
            Id = Guid.NewGuid();
            Status = FeedStatuses.Open;
            Priority = PriorityLevels.Medium;
            ViewsCount = 0;
            CommentsCount = 0;
            LikesCount = 0;
            DislikesCount = 0;
            ReportsCount = 0;
            IsActive = true;
            IsFeatured = false;
            IsFlagged = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Foreign Key
        [Required]
        [Display(Name = "User")]
        public Guid UserId { get; set; }

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

        // Job Details
        [Required]
        [StringLength(100)]
        [Display(Name = "Job Category")]
        public string JobCategory { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Budget Range Min")]
        public decimal? BudgetRangeMin { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Budget Range Max")]
        public decimal? BudgetRangeMax { get; set; }

        // Invoice and Documentation
        [Required]
        [StringLength(500)]
        [Display(Name = "Invoice Image Path")]
        public string InvoiceImage { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Invoice Amount")]
        public decimal InvoiceAmount { get; set; }

        [Display(Name = "Invoice Date")]
        [DataType(DataType.Date)]
        public DateTime? InvoiceDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Additional Documents Path")]
        public string? AdditionalDocuments { get; set; }

        // Location
        [Required]
        [StringLength(255)]
        public string Location { get; set; } = string.Empty;

        // Timeline
        [Display(Name = "Preferred Start Date")]
        [DataType(DataType.Date)]
        public DateTime? PreferredStartDate { get; set; }

        [Display(Name = "Deadline")]
        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }

        // Status and Priority
        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        [StringLength(10)]
        public string Priority { get; set; }

        // Engagement Metrics
        [Range(0, int.MaxValue)]
        [Display(Name = "Views Count")]
        public int ViewsCount { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Comments Count")]
        public int CommentsCount { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Likes Count")]
        public int LikesCount { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Dislikes Count")]
        public int DislikesCount { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Reports Count")]
        public int ReportsCount { get; set; }

        // Timestamps
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }

        // Visibility and Moderation
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Is Flagged")]
        public bool IsFlagged { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
        public virtual ICollection<ArtisanProposal> Proposals { get; set; } = new List<ArtisanProposal>();

        // Helper Methods
        public string GetStatusDisplay()
        {
            return Status switch
            {
                FeedStatuses.Open => "Open",
                FeedStatuses.InReview => "In Review",
                FeedStatuses.Negotiating => "Negotiating",
                FeedStatuses.Closed => "Closed",
                FeedStatuses.Completed => "Completed",
                FeedStatuses.Cancelled => "Cancelled",
                _ => Status
            };
        }

        public string GetPriorityDisplay()
        {
            return Priority switch
            {
                PriorityLevels.Low => "Low",
                PriorityLevels.Medium => "Medium",
                PriorityLevels.High => "High",
                PriorityLevels.Urgent => "Urgent",
                _ => Priority
            };
        }

        public string GetBudgetRangeDisplay()
        {
            if (BudgetRangeMin.HasValue && BudgetRangeMax.HasValue)
                return $"${BudgetRangeMin:N2} - ${BudgetRangeMax:N2}";
            if (BudgetRangeMin.HasValue)
                return $"From ${BudgetRangeMin:N2}";
            if (BudgetRangeMax.HasValue)
                return $"Up to ${BudgetRangeMax:N2}";
            return "Not specified";
        }

        public bool IsExpired()
        {
            return Deadline.HasValue && Deadline.Value < DateTime.UtcNow;
        }

        public void IncrementViews()
        {
            ViewsCount++;
        }

        public void IncrementComments()
        {
            CommentsCount++;
        }

        public void DecrementComments()
        {
            if (CommentsCount > 0)
                CommentsCount--;
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

        public void IncrementDislikes()
        {
            DislikesCount++;
        }

        public void DecrementDislikes()
        {
            if (DislikesCount > 0)
                DislikesCount--;
        }

        public void IncrementReports()
        {
            ReportsCount++;
        }

        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{Title} by {User?.FullName}";
        }
    }

    /// <summary>
    /// Constants for Feed Status
    /// </summary>
    public static class FeedStatuses
    {
        public const string Open = "OPEN";
        public const string InReview = "IN_REVIEW";
        public const string Negotiating = "NEGOTIATING";
        public const string Closed = "CLOSED";
        public const string Completed = "COMPLETED";
        public const string Cancelled = "CANCELLED";

        public static readonly string[] AllStatuses = 
        {
            Open, InReview, Negotiating, Closed, Completed, Cancelled
        };

        public static readonly Dictionary<string, string> StatusDisplayNames = new()
        {
            { Open, "Open" },
            { InReview, "In Review" },
            { Negotiating, "Negotiating" },
            { Closed, "Closed" },
            { Completed, "Completed" },
            { Cancelled, "Cancelled" }
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

    /// <summary>
    /// Constants for Priority Levels
    /// </summary>
    public static class PriorityLevels
    {
        public const string Low = "LOW";
        public const string Medium = "MEDIUM";
        public const string High = "HIGH";
        public const string Urgent = "URGENT";

        public static readonly string[] AllLevels = 
        {
            Low, Medium, High, Urgent
        };

        public static readonly Dictionary<string, string> LevelDisplayNames = new()
        {
            { Low, "Low" },
            { Medium, "Medium" },
            { High, "High" },
            { Urgent, "Urgent" }
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
}