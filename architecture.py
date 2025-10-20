"""
Django Models for Artisan-User Marketplace System
A comprehensive system connecting users with artisans (masons, plumbers, contractors)
"""

from django.db import models
from django.contrib.auth.models import AbstractUser
from django.core.validators import MinValueValidator, MaxValueValidator, RegexValidator
from django.utils.text import slugify
from django.urls import reverse
import uuid


# ==================== Custom User Model ====================
class User(AbstractUser):
    """
    Extended User model inheriting from AbstractUser
    Serves as the base authentication model for all users
    """
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    email = models.EmailField(unique=True, db_index=True)
    phone_regex = RegexValidator(
        regex=r'^\+?1?\d{9,15}$',
        message="Phone number must be entered in the format: '+999999999'. Up to 15 digits allowed."
    )
    phone_number = models.CharField(validators=[phone_regex], max_length=17, blank=True)
    full_name = models.CharField(max_length=255)
    profile_picture = models.ImageField(upload_to='profiles/', blank=True, null=True)
    date_joined = models.DateTimeField(auto_now_add=True)
    last_updated = models.DateTimeField(auto_now=True)
    is_active = models.BooleanField(default=True)
    is_verified = models.BooleanField(default=False)
    bio = models.TextField(max_length=500, blank=True)
    
    # Address fields
    address = models.CharField(max_length=255, blank=True)
    city = models.CharField(max_length=100, blank=True)
    state = models.CharField(max_length=100, blank=True)
    country = models.CharField(max_length=100, blank=True)
    postal_code = models.CharField(max_length=20, blank=True)
    
    USERNAME_FIELD = 'email'
    REQUIRED_FIELDS = ['username', 'full_name']
    
    class Meta:
        db_table = 'users'
        verbose_name = 'User'
        verbose_name_plural = 'Users'
        ordering = ['-date_joined']
    
    def __str__(self):
        return f"{self.full_name} ({self.email})"
    
    def get_absolute_url(self):
        return reverse('user-detail', kwargs={'pk': self.pk})


# ==================== Role Management ====================
class Role(models.Model):
    """
    Role model to manage user types and permissions
    """
    ROLE_CHOICES = [
        ('USER', 'Regular User'),
        ('ARTISAN', 'Artisan/Contractor'),
        ('MASON', 'Mason'),
        ('PLUMBER', 'Plumber'),
        ('ELECTRICIAN', 'Electrician'),
        ('CARPENTER', 'Carpenter'),
        ('PAINTER', 'Painter'),
        ('TILER', 'Tiler'),
        ('ROOFER', 'Roofer'),
        ('ADMIN', 'Administrator'),
        ('MODERATOR', 'Moderator'),
    ]
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    user = models.ForeignKey(User, on_delete=models.CASCADE, related_name='roles')
    role_type = models.CharField(max_length=20, choices=ROLE_CHOICES)
    is_primary = models.BooleanField(default=False)
    assigned_date = models.DateTimeField(auto_now_add=True)
    is_active = models.BooleanField(default=True)
    
    class Meta:
        db_table = 'roles'
        verbose_name = 'Role'
        verbose_name_plural = 'Roles'
        unique_together = [['user', 'role_type']]
        ordering = ['-is_primary', 'role_type']
    
    def __str__(self):
        return f"{self.user.full_name} - {self.get_role_type_display()}"
    
    def save(self, *args, **kwargs):
        # Ensure only one primary role per user
        if self.is_primary:
            Role.objects.filter(user=self.user, is_primary=True).update(is_primary=False)
        super().save(*args, **kwargs)


# ==================== Artisan Profile ====================
class ArtisanProfile(models.Model):
    """
    Profile model specifically for Artisans/Contractors
    Contains business and professional information
    """
    EXPERIENCE_LEVELS = [
        ('BEGINNER', '0-2 years'),
        ('INTERMEDIATE', '2-5 years'),
        ('EXPERIENCED', '5-10 years'),
        ('EXPERT', '10+ years'),
    ]
    
    AVAILABILITY_STATUS = [
        ('AVAILABLE', 'Available'),
        ('BUSY', 'Busy'),
        ('UNAVAILABLE', 'Unavailable'),
    ]
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='artisan_profile')
    business_name = models.CharField(max_length=255)
    slug = models.SlugField(max_length=255, unique=True, blank=True)
    specialization = models.CharField(max_length=100)
    
    # Professional details
    years_of_experience = models.PositiveIntegerField(validators=[MinValueValidator(0)])
    experience_level = models.CharField(max_length=20, choices=EXPERIENCE_LEVELS)
    license_number = models.CharField(max_length=100, blank=True)
    certification = models.FileField(upload_to='certifications/', blank=True, null=True)
    
    # Business information
    business_registration = models.CharField(max_length=100, blank=True)
    tax_id = models.CharField(max_length=50, blank=True)
    insurance_details = models.TextField(blank=True)
    
    # Ratings and reputation
    average_rating = models.DecimalField(
        max_digits=3, decimal_places=2, default=0.0,
        validators=[MinValueValidator(0.0), MaxValueValidator(5.0)]
    )
    total_reviews = models.PositiveIntegerField(default=0)
    completed_projects = models.PositiveIntegerField(default=0)
    
    # Availability
    availability_status = models.CharField(
        max_length=20, choices=AVAILABILITY_STATUS, default='AVAILABLE'
    )
    hourly_rate = models.DecimalField(max_digits=10, decimal_places=2, null=True, blank=True)
    service_radius = models.PositiveIntegerField(
        help_text="Service radius in kilometers", null=True, blank=True
    )
    
    # Professional description
    about = models.TextField(max_length=2000, blank=True)
    services_offered = models.TextField(help_text="Comma-separated list of services")
    
    # Verification
    is_verified = models.BooleanField(default=False)
    verified_date = models.DateTimeField(null=True, blank=True)
    verification_documents = models.FileField(upload_to='verifications/', blank=True, null=True)
    
    # Timestamps
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    
    class Meta:
        db_table = 'artisan_profiles'
        verbose_name = 'Artisan Profile'
        verbose_name_plural = 'Artisan Profiles'
        ordering = ['-average_rating', '-completed_projects']
    
    def __str__(self):
        return f"{self.business_name} - {self.user.full_name}"
    
    def save(self, *args, **kwargs):
        if not self.slug:
            self.slug = slugify(f"{self.business_name}-{self.user.username}")
        super().save(*args, **kwargs)
    
    def get_absolute_url(self):
        return reverse('artisan-profile', kwargs={'slug': self.slug})


# ==================== Artisan Work Portfolio ====================
class ArtisanWork(models.Model):
    """
    Showcase of completed works by artisans
    Portfolio to demonstrate skills and experience
    """
    PROJECT_STATUS = [
        ('COMPLETED', 'Completed'),
        ('IN_PROGRESS', 'In Progress'),
        ('PLANNED', 'Planned'),
    ]
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    artisan = models.ForeignKey(
        ArtisanProfile, on_delete=models.CASCADE, related_name='portfolio_works'
    )
    title = models.CharField(max_length=255)
    slug = models.SlugField(max_length=255, blank=True)
    description = models.TextField()
    
    # Project details
    project_type = models.CharField(max_length=100)
    project_status = models.CharField(max_length=20, choices=PROJECT_STATUS, default='COMPLETED')
    duration_days = models.PositiveIntegerField(help_text="Project duration in days")
    project_cost = models.DecimalField(max_digits=10, decimal_places=2, null=True, blank=True)
    
    # Location
    location = models.CharField(max_length=255)
    
    # Media
    featured_image = models.ImageField(upload_to='works/featured/')
    
    # Client information (optional)
    client_name = models.CharField(max_length=255, blank=True)
    client_testimonial = models.TextField(blank=True)
    client_rating = models.PositiveIntegerField(
        validators=[MinValueValidator(1), MaxValueValidator(5)], null=True, blank=True
    )
    
    # Engagement metrics
    views_count = models.PositiveIntegerField(default=0)
    likes_count = models.PositiveIntegerField(default=0)
    
    # Timestamps
    completion_date = models.DateField(null=True, blank=True)
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    
    # Visibility
    is_featured = models.BooleanField(default=False)
    is_public = models.BooleanField(default=True)
    
    class Meta:
        db_table = 'artisan_works'
        verbose_name = 'Artisan Work'
        verbose_name_plural = 'Artisan Works'
        ordering = ['-is_featured', '-created_at']
    
    def __str__(self):
        return f"{self.title} by {self.artisan.business_name}"
    
    def save(self, *args, **kwargs):
        if not self.slug:
            self.slug = slugify(f"{self.title}-{uuid.uuid4().hex[:8]}")
        super().save(*args, **kwargs)
    
    def get_absolute_url(self):
        return reverse('artisan-work-detail', kwargs={'slug': self.slug})


class ArtisanWorkImage(models.Model):
    """
    Additional images for artisan work portfolio
    """
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    work = models.ForeignKey(ArtisanWork, on_delete=models.CASCADE, related_name='images')
    image = models.ImageField(upload_to='works/gallery/')
    caption = models.CharField(max_length=255, blank=True)
    order = models.PositiveIntegerField(default=0)
    uploaded_at = models.DateTimeField(auto_now_add=True)
    
    class Meta:
        db_table = 'artisan_work_images'
        ordering = ['order', 'uploaded_at']
    
    def __str__(self):
        return f"Image for {self.work.title}"


# ==================== User Feed (Job Requests) ====================
class UserFeed(models.Model):
    """
    Feed model for users posting job requests with invoices
    Users upload invoices and descriptions to get better quotes
    """
    STATUS_CHOICES = [
        ('OPEN', 'Open'),
        ('IN_REVIEW', 'In Review'),
        ('NEGOTIATING', 'Negotiating'),
        ('CLOSED', 'Closed'),
        ('COMPLETED', 'Completed'),
        ('CANCELLED', 'Cancelled'),
    ]
    
    PRIORITY_LEVELS = [
        ('LOW', 'Low'),
        ('MEDIUM', 'Medium'),
        ('HIGH', 'High'),
        ('URGENT', 'Urgent'),
    ]
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    user = models.ForeignKey(User, on_delete=models.CASCADE, related_name='job_requests')
    title = models.CharField(max_length=255)
    slug = models.SlugField(max_length=255, blank=True)
    description = models.TextField()
    
    # Job details
    job_category = models.CharField(max_length=100)
    budget_range_min = models.DecimalField(max_digits=10, decimal_places=2, null=True, blank=True)
    budget_range_max = models.DecimalField(max_digits=10, decimal_places=2, null=True, blank=True)
    
    # Invoice and documentation
    invoice_image = models.ImageField(upload_to='invoices/')
    invoice_amount = models.DecimalField(max_digits=10, decimal_places=2)
    invoice_date = models.DateField(null=True, blank=True)
    additional_documents = models.FileField(upload_to='documents/', blank=True, null=True)
    
    # Location
    location = models.CharField(max_length=255)
    
    # Timeline
    preferred_start_date = models.DateField(null=True, blank=True)
    deadline = models.DateField(null=True, blank=True)
    
    # Status and priority
    status = models.CharField(max_length=20, choices=STATUS_CHOICES, default='OPEN')
    priority = models.CharField(max_length=10, choices=PRIORITY_LEVELS, default='MEDIUM')
    
    # Engagement metrics
    views_count = models.PositiveIntegerField(default=0)
    comments_count = models.PositiveIntegerField(default=0)
    likes_count = models.PositiveIntegerField(default=0)
    dislikes_count = models.PositiveIntegerField(default=0)
    reports_count = models.PositiveIntegerField(default=0)
    
    # Timestamps
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    
    # Visibility and moderation
    is_active = models.BooleanField(default=True)
    is_featured = models.BooleanField(default=False)
    is_flagged = models.BooleanField(default=False)
    
    class Meta:
        db_table = 'user_feeds'
        verbose_name = 'User Feed'
        verbose_name_plural = 'User Feeds'
        ordering = ['-is_featured', '-created_at']
        indexes = [
            models.Index(fields=['status', 'created_at']),
            models.Index(fields=['job_category', 'status']),
        ]
    
    def __str__(self):
        return f"{self.title} by {self.user.full_name}"
    
    def save(self, *args, **kwargs):
        if not self.slug:
            self.slug = slugify(f"{self.title}-{uuid.uuid4().hex[:8]}")
        super().save(*args, **kwargs)
    
    def get_absolute_url(self):
        return reverse('user-feed-detail', kwargs={'slug': self.slug})


# ==================== Artisan Feed (Service Offerings) ====================
class ArtisanFeed(models.Model):
    """
    Feed model for artisans showcasing their services and promotions
    Artisans can post about their work, offers, and availability
    """
    POST_TYPE_CHOICES = [
        ('SERVICE', 'Service Offering'),
        ('PROMOTION', 'Promotion/Discount'),
        ('SHOWCASE', 'Work Showcase'),
        ('TIP', 'Professional Tip'),
        ('ANNOUNCEMENT', 'Announcement'),
    ]
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    artisan = models.ForeignKey(
        ArtisanProfile, on_delete=models.CASCADE, related_name='feed_posts'
    )
    title = models.CharField(max_length=255)
    slug = models.SlugField(max_length=255, blank=True)
    description = models.TextField()
    
    # Post details
    post_type = models.CharField(max_length=20, choices=POST_TYPE_CHOICES, default='SERVICE')
    service_category = models.CharField(max_length=100)
    
    # Media
    featured_image = models.ImageField(upload_to='artisan_feeds/images/')
    video_url = models.URLField(blank=True, null=True)
    
    # Pricing (if applicable)
    price = models.DecimalField(max_digits=10, decimal_places=2, null=True, blank=True)
    discount_percentage = models.PositiveIntegerField(
        validators=[MinValueValidator(0), MaxValueValidator(100)], 
        null=True, blank=True
    )
    
    # Validity (for promotions)
    valid_from = models.DateTimeField(null=True, blank=True)
    valid_until = models.DateTimeField(null=True, blank=True)
    
    # Engagement metrics
    views_count = models.PositiveIntegerField(default=0)
    comments_count = models.PositiveIntegerField(default=0)
    likes_count = models.PositiveIntegerField(default=0)
    dislikes_count = models.PositiveIntegerField(default=0)
    reports_count = models.PositiveIntegerField(default=0)
    shares_count = models.PositiveIntegerField(default=0)
    
    # Timestamps
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    
    # Visibility and moderation
    is_active = models.BooleanField(default=True)
    is_featured = models.BooleanField(default=False)
    is_promoted = models.BooleanField(default=False)
    is_flagged = models.BooleanField(default=False)
    
    class Meta:
        db_table = 'artisan_feeds'
        verbose_name = 'Artisan Feed'
        verbose_name_plural = 'Artisan Feeds'
        ordering = ['-is_promoted', '-is_featured', '-created_at']
        indexes = [
            models.Index(fields=['post_type', 'created_at']),
            models.Index(fields=['service_category', 'is_active']),
        ]
    
    def __str__(self):
        return f"{self.title} by {self.artisan.business_name}"
    
    def save(self, *args, **kwargs):
        if not self.slug:
            self.slug = slugify(f"{self.title}-{uuid.uuid4().hex[:8]}")
        super().save(*args, **kwargs)
    
    def get_absolute_url(self):
        return reverse('artisan-feed-detail', kwargs={'slug': self.slug})


# ==================== Comments System ====================
class Comment(models.Model):
    """
    Universal comment model for both UserFeed and ArtisanFeed
    """
    COMMENT_TYPE_CHOICES = [
        ('USER_FEED', 'User Feed Comment'),
        ('ARTISAN_FEED', 'Artisan Feed Comment'),
    ]
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    user = models.ForeignKey(User, on_delete=models.CASCADE, related_name='comments')
    
    # Polymorphic relationship
    comment_type = models.CharField(max_length=20, choices=COMMENT_TYPE_CHOICES)
    user_feed = models.ForeignKey(
        UserFeed, on_delete=models.CASCADE, related_name='comments', null=True, blank=True
    )
    artisan_feed = models.ForeignKey(
        ArtisanFeed, on_delete=models.CASCADE, related_name='comments', null=True, blank=True
    )
    
    # Comment content
    content = models.TextField(max_length=1000)
    parent_comment = models.ForeignKey(
        'self', on_delete=models.CASCADE, related_name='replies', null=True, blank=True
    )
    
    # Engagement
    likes_count = models.PositiveIntegerField(default=0)
    dislikes_count = models.PositiveIntegerField(default=0)
    
    # Timestamps
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    
    # Moderation
    is_edited = models.BooleanField(default=False)
    is_flagged = models.BooleanField(default=False)
    is_deleted = models.BooleanField(default=False)
    
    class Meta:
        db_table = 'comments'
        verbose_name = 'Comment'
        verbose_name_plural = 'Comments'
        ordering = ['-created_at']
        indexes = [
            models.Index(fields=['comment_type', 'created_at']),
        ]
    
    def __str__(self):
        return f"Comment by {self.user.full_name} on {self.get_comment_type_display()}"


# ==================== Reactions System ====================
class Reaction(models.Model):
    """
    Unified reaction system for likes/dislikes on feeds
    """
    REACTION_TYPES = [
        ('LIKE', 'Like'),
        ('DISLIKE', 'Dislike'),
    ]
    
    CONTENT_TYPE_CHOICES = [
        ('USER_FEED', 'User Feed'),
        ('ARTISAN_FEED', 'Artisan Feed'),
        ('COMMENT', 'Comment'),
    ]
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    user = models.ForeignKey(User, on_delete=models.CASCADE, related_name='reactions')
    reaction_type = models.CharField(max_length=10, choices=REACTION_TYPES)
    
    # Polymorphic relationship
    content_type = models.CharField(max_length=20, choices=CONTENT_TYPE_CHOICES)
    user_feed = models.ForeignKey(
        UserFeed, on_delete=models.CASCADE, related_name='reactions', null=True, blank=True
    )
    artisan_feed = models.ForeignKey(
        ArtisanFeed, on_delete=models.CASCADE, related_name='reactions', null=True, blank=True
    )
    comment = models.ForeignKey(
        Comment, on_delete=models.CASCADE, related_name='reactions', null=True, blank=True
    )
    
    created_at = models.DateTimeField(auto_now_add=True)
    
    class Meta:
        db_table = 'reactions'
        verbose_name = 'Reaction'
        verbose_name_plural = 'Reactions'
        unique_together = [['user', 'content_type', 'user_feed', 'artisan_feed', 'comment']]
    
    def __str__(self):
        return f"{self.user.full_name} - {self.reaction_type}"


# ==================== Reports/Flags System ====================
class Report(models.Model):
    """
    Reporting system for flagging inappropriate content
    """
    REPORT_REASONS = [
        ('SPAM', 'Spam'),
        ('INAPPROPRIATE', 'Inappropriate Content'),
        ('SCAM', 'Scam/Fraud'),
        ('MISLEADING', 'Misleading Information'),
        ('HARASSMENT', 'Harassment'),
        ('COPYRIGHT', 'Copyright Violation'),
        ('OTHER', 'Other'),
    ]
    
    CONTENT_TYPE_CHOICES = [
        ('USER_FEED', 'User Feed'),
        ('ARTISAN_FEED', 'Artisan Feed'),
        ('COMMENT', 'Comment'),
        ('USER', 'User Profile'),
    ]
    
    STATUS_CHOICES = [
        ('PENDING', 'Pending Review'),
        ('UNDER_REVIEW', 'Under Review'),
        ('RESOLVED', 'Resolved'),
        ('DISMISSED', 'Dismissed'),
    ]
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    reporter = models.ForeignKey(User, on_delete=models.CASCADE, related_name='reports_made')
    
    # Content being reported
    content_type = models.CharField(max_length=20, choices=CONTENT_TYPE_CHOICES)
    user_feed = models.ForeignKey(
        UserFeed, on_delete=models.CASCADE, related_name='reports', null=True, blank=True
    )
    artisan_feed = models.ForeignKey(
        ArtisanFeed, on_delete=models.CASCADE, related_name='reports', null=True, blank=True
    )
    comment = models.ForeignKey(
        Comment, on_delete=models.CASCADE, related_name='reports', null=True, blank=True
    )
    reported_user = models.ForeignKey(
        User, on_delete=models.CASCADE, related_name='reports_received', null=True, blank=True
    )
    
    # Report details
    reason = models.CharField(max_length=20, choices=REPORT_REASONS)
    description = models.TextField()
    status = models.CharField(max_length=20, choices=STATUS_CHOICES, default='PENDING')
    
    # Resolution
    reviewed_by = models.ForeignKey(
        User, on_delete=models.SET_NULL, related_name='reports_reviewed', 
        null=True, blank=True
    )
    resolution_notes = models.TextField(blank=True)
    
    # Timestamps
    created_at = models.DateTimeField(auto_now_add=True)
    reviewed_at = models.DateTimeField(null=True, blank=True)
    
    class Meta:
        db_table = 'reports'
        verbose_name = 'Report'
        verbose_name_plural = 'Reports'
        ordering = ['-created_at']
        indexes = [
            models.Index(fields=['status', 'created_at']),
        ]
    
    def __str__(self):
        return f"Report by {self.reporter.full_name} - {self.get_reason_display()}"


# ==================== Proposals/Quotes System ====================
class ArtisanProposal(models.Model):
    """
    Proposals from artisans responding to user job requests
    """
    STATUS_CHOICES = [
        ('PENDING', 'Pending'),
        ('ACCEPTED', 'Accepted'),
        ('REJECTED', 'Rejected'),
        ('WITHDRAWN', 'Withdrawn'),
    ]
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    user_feed = models.ForeignKey(
        UserFeed, on_delete=models.CASCADE, related_name='proposals'
    )
    artisan = models.ForeignKey(
        ArtisanProfile, on_delete=models.CASCADE, related_name='proposals'
    )
    
    # Proposal details
    proposed_price = models.DecimalField(max_digits=10, decimal_places=2)
    estimated_duration = models.PositiveIntegerField(help_text="Duration in days")
    message = models.TextField()
    
    # Terms and conditions
    terms_conditions = models.TextField(blank=True)
    payment_terms = models.TextField(blank=True)
    
    # Attachments
    quote_document = models.FileField(upload_to='proposals/', blank=True, null=True)
    
    # Status
    status = models.CharField(max_length=20, choices=STATUS_CHOICES, default='PENDING')
    
    # Timestamps
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    accepted_at = models.DateTimeField(null=True, blank=True)
    
    class Meta:
        db_table = 'artisan_proposals'
        verbose_name = 'Artisan Proposal'
        verbose_name_plural = 'Artisan Proposals'
        ordering = ['-created_at']
        unique_together = [['user_feed', 'artisan']]
    
    def __str__(self):
        return f"Proposal by {self.artisan.business_name} for {self.user_feed.title}"