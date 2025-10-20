using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtisanMarketplace.Models
{
    
    [Table("reactions")]
    public class Reaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        
        [Required]
        public Guid UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        
        [Required]
        [MaxLength(10)]
        public ReactionType ReactionType { get; set; }

        
        [Required]
        [MaxLength(20)]
        public ContentType ContentType { get; set; }

       
        public Guid? UserFeedId { get; set; }
        
        [ForeignKey("UserFeedId")]
        public virtual UserFeed UserFeed { get; set; }

        public Guid? ArtisanFeedId { get; set; }
        
        [ForeignKey("ArtisanFeedId")]
        public virtual ArtisanFeed ArtisanFeed { get; set; }

        public Guid? CommentId { get; set; }
        
        [ForeignKey("CommentId")]
        public virtual Comment Comment { get; set; }

        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public Reaction()
        {
            Id = Guid.NewGuid();
        }
    }

    
    public enum ReactionType
    {
        [Display(Name = "Like")]
        Like,
        
        [Display(Name = "Dislike")]
        Dislike
    }

    public enum ContentType
    {
        [Display(Name = "User Feed")]
        UserFeed,
        
        [Display(Name = "Artisan Feed")]
        ArtisanFeed,
        
        [Display(Name = "Comment")]
        Comment
    }

    
    [Table("user_feeds")]
    public partial class UserFeed
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        // Engagement metrics
        public int ViewsCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;
        public int LikesCount { get; set; } = 0;
        public int DislikesCount { get; set; } = 0;
        public int ReportsCount { get; set; } = 0;

        
        public virtual ICollection<Reaction> Reactions { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public UserFeed()
        {
            Reactions = new HashSet<Reaction>();
            Comments = new HashSet<Comment>();
        }
    }

    
    [Table("artisan_feeds")]
    public partial class ArtisanFeed
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ArtisanId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        
        public int ViewsCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;
        public int LikesCount { get; set; } = 0;
        public int DislikesCount { get; set; } = 0;
        public int ReportsCount { get; set; } = 0;
        public int SharesCount { get; set; } = 0;

        
        public virtual ICollection<Reaction> Reactions { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public ArtisanFeed()
        {
            Reactions = new HashSet<Reaction>();
            Comments = new HashSet<Comment>();
        }
    }

    /// <summary>
    /// Comment model - partial definition for reaction relationships
    /// </summary>
    [Table("comments")]
    public partial class Comment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }

        // Engagement
        public int LikesCount { get; set; } = 0;
        public int DislikesCount { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<Reaction> Reactions { get; set; }

        public Comment()
        {
            Reactions = new HashSet<Reaction>();
        }
    }

    /// <summary>
    /// User model - partial definition for reaction relationships
    /// </summary>
    [Table("users")]
    public partial class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Navigation properties
        public virtual ICollection<Reaction> Reactions { get; set; }

        public User()
        {
            Reactions = new HashSet<Reaction>();
        }
    }
}