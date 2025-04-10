using REIstacks.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace REIstack.Domain.Models;

[Table("organizations")]
public class Organization
{
    [Key]
    public string Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    [MaxLength(255)]
    public string Subdomain { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? CustomDomain { get; set; } = string.Empty;

    public bool HasVerifiedDomain { get; set; } = false;

    [MaxLength(255)]
    public string? StripeCustomerId { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? StripeSubscriptionId { get; set; } = string.Empty;

    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trialing;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(255)]
    public string? PrimaryDomain { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? ThemeColor { get; set; } = "#4F46E5"; // Default indigo

    [MaxLength(2048)] // URLs can be long
    public string? LogoUrl { get; set; } = string.Empty;


    public Guid? OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    public virtual UserProfile Owner { get; set; } // Navigation Property

    // Initialize collection properties to prevent null reference exceptions
    public virtual ICollection<DomainVerification> DomainVerifications { get; set; } = new List<DomainVerification>();
    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public virtual ICollection<OrganizationRole> OrganizationRoles { get; set; } = new List<OrganizationRole>();
    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    public virtual ICollection<StripeSubscription> StripeSubscriptions { get; set; } = new List<StripeSubscription>();
    public virtual ICollection<LandingPages> LandingPages { get; set; } = new List<LandingPages>();
    public virtual ICollection<FieldMappingTemplate> FieldMappingTemplates { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    public virtual ICollection<Communication> Communications { get; set; } = new List<Communication>();
    public virtual ICollection<MarketingCampaign> MarketingCampaigns { get; set; } = new List<MarketingCampaign>();
}
