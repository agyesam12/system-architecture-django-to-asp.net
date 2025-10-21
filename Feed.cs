using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtisanMarketplace.Models
{
    /// <summary>
    /// Unified feed model for both user job requests and artisan posts
    /// Supports multiple feed types including job requests, service offerings, promotions, etc.
    /// </summary>
    [Table("Feeds")]
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(FeedType), nameof(CreatedAt))]
    [Index(nameof(Category), nameof(IsActive))]
    [Index(nameof(Status), nameof(CreatedAt))]
    public class Feed
    {
        public Feed()
        {
            Id = Guid.NewGuid();
            FeedType = FeedTypes.UserJobRequest;
            Status = FeedStatuses.Open;
            Priority = PriorityLevels.Medium;
            ViewsCount = 0;
            CommentsCount = 0;
            LikesCount = 0;
            DislikesCount = 0;
            ReportsCount = 0;
            SharesCount = 0;
            IsActive = true;
            IsFeatured = false;
            IsPromoted = false;
            IsFlagged = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Owner Information - Either UserId or ArtisanId will be populated
        [Display(Name = "User")]
        public Guid? UserId { get; set; }

        [Display(Name = "Artisan")]
        public Guid? ArtisanId { get; set; }

        // Core Feed Information
        [Required]
        [StringLength(20)]
        [Display(Name = "Feed Type")]
        public string FeedType { get; set; }

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

        [Required]
        [StringLength(100)]
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        // Media
        [StringLength(500)]
        [Display(Name = "Featured Image Path")]
        public string? FeaturedImage { get; set; }

        [StringLength(500)]
        [Display(Name = "Video URL")]
        [Url]
        public string? VideoUrl { get; set; }

        // User Job Request Specific Fields
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Budget Range Min")]
        public decimal? BudgetRangeMin { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Budget Range Max")]
        public decimal? BudgetRangeMax { get; set; }

        [StringLength(500)]
        [Display(Name = "Invoice Image Path")]
        public string? InvoiceImage { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Invoice Amount")]
        public decimal? InvoiceAmount { get; set; }

        [Display(Name = "Invoice Date")]
        [DataType(DataType.Date)]
        public DateTime? InvoiceDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Additional Documents Path")]
        public string? AdditionalDocuments { get; set; }

        [StringLength(255)]
        public string? Location { get; set; }

        [Display(Name = "Preferred Start Date")]
        [DataType(DataType.Date)]
        public DateTime? PreferredStartDate { get; set; }

        [Display(Name = "Deadline")]
        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }

        // Artisan Post Specific Fields
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, 100)]
        [Display(Name = "Discount Percentage")]
        public int? DiscountPercentage { get; set; }

        [Display(Name = "Valid From")]
        public DateTime? ValidFrom { get; set; }

        [Display(Name = "Valid Until")]
        public DateTime? ValidUntil { get; set; }

        // Status and Priority (mainly for job requests)
        [StringLength(20)]
        public string? Status { get; set; }

        [StringLength(10)]
        public string? Priority { get; set; }

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

        [Range(0, int.MaxValue)]
        [Display(Name = "Shares Count")]
        public int SharesCount { get; set; }

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

        [Display(Name = "Is Promoted")]
        public bool IsPromoted { get; set; }

        [Display(Name = "Is Flagged")]
        public bool IsFlagged { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        [ForeignKey(nameof(ArtisanId))]
        public virtual ArtisanProfile? Artisan { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
        public virtual ICollection<ArtisanProposal> Proposals { get; set; } = new List<ArtisanProposal>();

        // Helper Methods
        public bool IsUserJobRequest()
        {
            return FeedType == FeedTypes.UserJobRequest;
        }

        public bool IsArtisanPost()
        {
            return FeedType == FeedTypes.ArtisanService || 
                   FeedType == FeedTypes.ArtisanPromotion || 
                   FeedType == FeedTypes.ArtisanShowcase || 
                   FeedType == FeedTypes.ArtisanTip || 
                   FeedType == FeedTypes.ArtisanAnnouncement;
        }

        public string GetFeedTypeDisplay()
        {
            return FeedTypes.GetDisplayName(FeedType);
        }

        public string GetStatusDisplay()
        {
            return string.IsNullOrEmpty(Status) ? "N/A" : FeedStatuses.GetDisplayName(Status);
        }

        public string GetPriorityDisplay()
        {
            return string.IsNullOrEmpty(Priority) ? "N/A" : PriorityLevels.GetDisplayName(Priority);
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

        public decimal? GetDiscountedPrice()
        {
            if (Price.HasValue && DiscountPercentage.HasValue && DiscountPercentage > 0)
            {
                var discount = Price.Value * (DiscountPercentage.Value / 100m);
                return Price.Value - discount;
            }
            return Price;
        }

        public bool IsPromotionValid()
        {
            if (FeedType != FeedTypes.ArtisanPromotion)
                return false;

            var now = DateTime.UtcNow;

            if (ValidFrom.HasValue && ValidFrom.Value > now)
                return false;

            if (ValidUntil.HasValue && ValidUntil.Value < now)
                return false;

            return true;
        }

        public bool IsExpired()
        {
            if (IsUserJobRequest())
                return Deadline.HasValue && Deadline.Value < DateTime.UtcNow;
            
            if (FeedType == FeedTypes.ArtisanPromotion)
                return ValidUntil.HasValue && ValidUntil.Value < DateTime.UtcNow;

            return false;
        }

        public int GetDaysUntilExpiry()
        {
            DateTime? expiryDate = null;

            if (IsUserJobRequest() && Deadline.HasValue)
                expiryDate = Deadline.Value;
            else if (FeedType == FeedTypes.ArtisanPromotion && ValidUntil.HasValue)
                expiryDate = ValidUntil.Value;

            if (!expiryDate.HasValue)
                return int.MaxValue;

            var diff = expiryDate.Value - DateTime.UtcNow;
            return diff.Days > 0 ? diff.Days : 0;
        }

        public string GetOwnerName()
        {
            if (User != null)
                return User.FullName;
            if (Artisan != null)
                return Artisan.BusinessName;
            return "Unknown";
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

        public void IncrementShares()
        {
            SharesCount++;
        }

        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{Title} by {GetOwnerName()}";
        }
    }

    /// <summary>
    /// Constants for Feed Types
    /// </summary>
    public static class FeedTypes
    {
        // User Feed Types
        public const string UserJobRequest = "USER_JOB_REQUEST";

        // Artisan Feed Types
        public const string ArtisanService = "ARTISAN_SERVICE";
        public const string ArtisanPromotion = "ARTISAN_PROMOTION";
        public const string ArtisanShowcase = "ARTISAN_SHOWCASE";
        public const string ArtisanTip = "ARTISAN_TIP";
        public const string ArtisanAnnouncement = "ARTISAN_ANNOUNCEMENT";

        public static readonly string[] AllTypes = 
        {
            UserJobRequest,
            ArtisanService,
            ArtisanPromotion,
            ArtisanShowcase,
            ArtisanTip,
            ArtisanAnnouncement
        };

        public static readonly string[] UserTypes = { UserJobRequest };
        
        public static readonly string[] ArtisanTypes = 
        { 
            ArtisanService, 
            ArtisanPromotion, 
            ArtisanShowcase, 
            ArtisanTip, 
            ArtisanAnnouncement 
        };

        public static readonly Dictionary<string, string> TypeDisplayNames = new()
        {
            { UserJobRequest, "Job Request" },
            { ArtisanService, "Service Offering" },
            { ArtisanPromotion, "Promotion/Discount" },
            { ArtisanShowcase, "Work Showcase" },
            { ArtisanTip, "Professional Tip" },
            { ArtisanAnnouncement, "Announcement" }
        };

        public static bool IsValidType(string type)
        {
            return AllTypes.Contains(type.ToUpper());
        }

        public static bool IsUserType(string type)
        {
            return UserTypes.Contains(type.ToUpper());
        }

        public static bool IsArtisanType(string type)
        {
            return ArtisanTypes.Contains(type.ToUpper());
        }

        public static string GetDisplayName(string type)
        {
            return TypeDisplayNames.TryGetValue(type.ToUpper(), out var displayName) 
                ? displayName 
                : type;
        }
    }

    /// <summary>
    /// Constants for Feed Status (mainly for user job requests)
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
    /// Constants for Priority Levels (mainly for user job requests)
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