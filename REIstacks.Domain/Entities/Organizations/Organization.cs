using REIstacks.Domain.Entities.Billing;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Deals;
using REIstacks.Domain.Entities.Marketing;
using REIstacks.Domain.Entities.Properties;
using REIstacks.Domain.Entities.Tasks;
using REIstacks.Domain.Entities.UploadLeads;
using REIstacks.Domain.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Organizations
{
    [Table("organizations")]
    public class Organization
    {
        [Key]
        public string Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string Subdomain { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? CustomDomain { get; set; }

        public bool HasVerifiedDomain { get; set; }

        [MaxLength(255)]
        public string? StripeCustomerId { get; set; }

        [MaxLength(255)]
        public string? StripeSubscriptionId { get; set; }

        public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trialing;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string? PrimaryDomain { get; set; }

        [MaxLength(50)]
        public string? ThemeColor { get; set; } = "#4F46E5";

        [MaxLength(2048)]
        public string? LogoUrl { get; set; }

        public Guid? OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual UserProfile? Owner { get; set; }

        // ← Make sure *all* collections are initialized
        public virtual ICollection<PropertyDocument> PropertyDocuments { get; set; } = new List<PropertyDocument>();
        public virtual ICollection<List> Lists { get; set; } = new List<List>();
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public virtual ICollection<DomainVerification> DomainVerifications { get; set; } = new List<DomainVerification>();
        public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
        public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<OrganizationRole> OrganizationRoles { get; set; } = new List<OrganizationRole>();
        public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
        public virtual ICollection<StripeSubscription> StripeSubscriptions { get; set; } = new List<StripeSubscription>();
        public virtual ICollection<LandingPages> LandingPages { get; set; } = new List<LandingPages>();
        public virtual ICollection<FieldMappingTemplate> FieldMappingTemplates { get; set; } = new List<FieldMappingTemplate>();
        public virtual ICollection<PropertyTag> PropertyTags { get; set; } = new List<PropertyTag>();
        public virtual ICollection<ImportError> ImportErrors { get; set; } = new List<ImportError>();
        public virtual ICollection<PropertyList> PropertyLists { get; set; } = new List<PropertyList>();
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
        public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
        public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
        public virtual ICollection<Communication> Communications { get; set; } = new List<Communication>();
        public virtual ICollection<PropertyNote> PropertyNotes { get; set; } = new List<PropertyNote>();
        public virtual ICollection<ImportJob> ImportJobs { get; set; } = new List<ImportJob>();
        public virtual ICollection<Board> Boards { get; set; } = new List<Board>();
        public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
        public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
        public virtual ICollection<OfferDocument> OfferDocuments { get; set; } = new List<OfferDocument>();
        public virtual ICollection<LeadStage> LeadStages { get; set; } = new List<LeadStage>();
        public virtual ICollection<MarketingCampaign> MarketingCampaigns { get; set; } = new List<MarketingCampaign>();
    }

}
