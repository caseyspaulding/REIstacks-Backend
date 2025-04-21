using REIstacks.Domain.Entities.Deals;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.Properties;
using REIstacks.Domain.Entities.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;

// Core person information
[Table("contacts")]
public class Contact
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(20)]
    public string? AlternatePhone { get; set; }

    [MaxLength(100)]
    public string? Company { get; set; }

    [MaxLength(50)]
    public string? Title { get; set; }

    [MaxLength(50)]
    public string? PreferredContactMethod { get; set; }

    [MaxLength(500)]
    public string? StreetAddress { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(50)]
    public string? ContactType { get; set; } // Seller, Buyer, Vendor, Agent, etc.

    [MaxLength(50)]
    public string? LeadSource { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? Tags { get; set; }

    public string? Notes { get; set; }

    // Marketing and communication tracking
    public int? Clicks { get; set; }

    public int? Opens { get; set; }

    public int? SMSResponses { get; set; }

    public int? CallsMade { get; set; }

    public int? MessagesLeft { get; set; }

    public DateTime? LastContacted { get; set; }

    public bool ConsentTextMessages { get; set; }

    public bool ConsentEmailMarketing { get; set; }

    public int? LeadOwnerId { get; set; }

    [ForeignKey("StatusId")]
    public virtual ContactStatus? Status { get; set; }

    [MaxLength(50)]
    public string? StatusId { get; set; }
    public int? LeadStageId { get; set; }

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }
    [ForeignKey("LeadStageId")]
    public virtual LeadStage LeadStage { get; set; }

    [MaxLength(50)]
    public string LeadStatus { get; set; } // "Open", "Contacted", "Qualified", "Converted", etc.

    public DateTime? LeadConvertedDate { get; set; }

    // Collection properties
    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
    public virtual ICollection<ContactTag> ContactTags { get; set; } = new List<ContactTag>();
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
    public virtual ICollection<Communication> Communications { get; set; } = new List<Communication>();
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public virtual ICollection<ContactPhone> PhoneNumbers { get; set; } = new List<ContactPhone>();
    public virtual ICollection<ContactEmail> EmailAddresses { get; set; } = new List<ContactEmail>();
    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
    public virtual ICollection<ContactActivity> ContactActivities { get; set; }
   = new List<ContactActivity>();
}