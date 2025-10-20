using Microsoft.EntityFrameworkCore;

namespace ArtisanMarketplace.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Report> Reports { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasIndex(c => new { c.CommentType, c.CreatedAt })
                      .HasDatabaseName("IX_Comments_CommentType_CreatedAt");

                // User relationship
                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // UserFeed relationship (optional)
                entity.HasOne(c => c.UserFeed)
                      .WithMany(uf => uf.Comments)
                      .HasForeignKey(c => c.UserFeedId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.ArtisanFeed)
                      .WithMany(af => af.Comments)
                      .HasForeignKey(c => c.ArtisanFeedId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Self-referencing for replies
                entity.HasOne(c => c.ParentComment)
                      .WithMany(c => c.Replies)
                      .HasForeignKey(c => c.ParentCommentId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                // Convert enum to string in database
                entity.Property(c => c.CommentType)
                      .HasConversion<string>();
            });


            // ==================== Reaction Configuration ====================
            
            modelBuilder.Entity<Reaction>(entity =>
            {
                entity.HasKey(r => r.Id);

                // Composite unique constraint to prevent duplicate reactions
                entity.HasIndex(r => new { r.UserId, r.ContentType, r.UserFeedId, r.ArtisanFeedId, r.CommentId })
                      .IsUnique()
                      .HasDatabaseName("IX_Reactions_Unique");

                // User relationship
                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // UserFeed relationship (optional)
                entity.HasOne(r => r.UserFeed)
                      .WithMany(uf => uf.Reactions)
                      .HasForeignKey(r => r.UserFeedId)
                      .OnDelete(DeleteBehavior.Cascade);

                // ArtisanFeed relationship (optional)
                entity.HasOne(r => r.ArtisanFeed)
                      .WithMany(af => af.Reactions)
                      .HasForeignKey(r => r.ArtisanFeedId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Comment relationship (optional)
                entity.HasOne(r => r.Comment)
                      .WithMany(c => c.Reactions)
                      .HasForeignKey(r => r.CommentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Convert enums to strings in database
                entity.Property(r => r.ReactionType)
                      .HasConversion<string>();

                entity.Property(r => r.ContentType)
                      .HasConversion<string>();
            });

            
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasIndex(r => new { r.Status, r.CreatedAt })
                      .HasDatabaseName("IX_Reports_Status_CreatedAt");

                // Reporter relationship
                entity.HasOne(r => r.Reporter)
                      .WithMany()
                      .HasForeignKey(r => r.ReporterId)
                      .OnDelete(DeleteBehavior.Cascade);


                entity.HasOne(r => r.UserFeed)
                      .WithMany(uf => uf.Reports)
                      .HasForeignKey(r => r.UserFeedId)
                      .OnDelete(DeleteBehavior.Cascade);

                
                entity.HasOne(r => r.ArtisanFeed)
                      .WithMany(af => af.Reports)
                      .HasForeignKey(r => r.ArtisanFeedId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Comment)
                      .WithMany(c => c.Reports)
                      .HasForeignKey(r => r.CommentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.ReportedUser)
                      .WithMany()
                      .HasForeignKey(r => r.ReportedUserId)
                      .OnDelete(DeleteBehavior.Restrict); 

            
                entity.HasOne(r => r.ReviewedBy)
                      .WithMany()
                      .HasForeignKey(r => r.ReviewedById)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete


                entity.Property(r => r.Reason)
                      .HasConversion<string>();

                entity.Property(r => r.ContentType)
                      .HasConversion<string>();

                entity.Property(r => r.Status)
                      .HasConversion<string>();
            });
        }
    }
}