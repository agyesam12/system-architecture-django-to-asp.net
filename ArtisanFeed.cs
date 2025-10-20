using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtisanMarketplace.Models
{

    [Table("ArtisanFeeds")]
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(PostType), nameof(CreatedAt))]
    [Index(nameof(ServiceCategory), nameof(IsActive))]
    public class ArtisanFeed
    {
        public ArtisanFeed()
        {
            Id = Guid.NewGuid();
            PostType = PostTypes.Service;
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

        // Post Details
        [Required]
        [StringLength(20)]
        [Display(Name = "Post Type")]
        public string PostType { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Service Category")]
        public string ServiceCategory { get; set; } = string.Empty;

        // Media
        [Required]
        [StringLength(500)]
        [Display(Name = "Featured Image Path")]
        public string FeaturedImage { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Video URL")]
        [Url]
        public string? VideoUrl { get; set; }

        // Pricing (if applicable)
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, 100)]
        [Display(Name = "Discount Percentage")]
        public int? DiscountPercentage { get; set; }

        // Validity (for promotions)
        [Display(Name = "Valid From")]
        public DateTime? ValidFrom { get; set; }

        [Display(Name = "Valid Until")]
        public DateTime? ValidUntil { get; set; }

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
        [ForeignKey(nameof(ArtisanId))]
        public virtual ArtisanProfile Artisan { get; set; } = null!;

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

        // Helper Methods
        public string GetPostTypeDisplay()
        {
            return PostType switch
            {
                PostTypes.Service => "Service Offering",
                PostTypes.Promotion => "Promotion/Discount",
                PostTypes.Showcase => "Work Showcase",
                PostTypes.Tip => "Professional Tip",
                PostTypes.Announcement => "Announcement",
                _ => PostType
            };
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
            if (PostType != PostTypes.Promotion)
                return false;

            var now = DateTime.UtcNow;

            if (ValidFrom.HasValue && ValidFrom.Value > now)
                return false;

            if (ValidUntil.HasValue && ValidUntil.Value < now)
                return false;

            return true;
        }

        public bool IsPromotionExpired()
        {
            return ValidUntil.HasValue && ValidUntil.Value < DateTime.UtcNow;
        }

        public int GetDaysUntilExpiry()
        {
            if (!ValidUntil.HasValue)
                return int.MaxValue;

            var diff = ValidUntil.Value - DateTime.UtcNow;
            return diff.Days > 0 ? diff.Days : 0;
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
            return $"{Title} by {Artisan?.BusinessName}";
        }
    }

    
    public static class PostTypes
    {
        public const string Service = "SERVICE";
        public const string Promotion = "PROMOTION";
        public const string Showcase = "SHOWCASE";
        public const string Tip = "TIP";
        public const string Announcement = "ANNOUNCEMENT";

        public static readonly string[] AllTypes = 
        {
            Service, Promotion, Showcase, Tip, Announcement
        };

        public static readonly Dictionary<string, string> TypeDisplayNames = new()
        {
            { Service, "Service Offering" },
            { Promotion, "Promotion/Discount" },
            { Showcase, "Work Showcase" },
            { Tip, "Professional Tip" },
            { Announcement, "Announcement" }
        };

        public static bool IsValidType(string type)
        {
            return AllTypes.Contains(type.ToUpper());
        }

        public static string GetDisplayName(string type)
        {
            return TypeDisplayNames.TryGetValue(type.ToUpper(), out var displayName) 
                ? displayName 
                : type;
        }
    }
}