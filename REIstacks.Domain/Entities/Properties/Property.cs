
using REIstacks.Domain.Common;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Deals;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.Tasks;
using REIstacks.Domain.Events;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Properties;
[Table("properties")]
public class Property : Entity
{
    public void ChangeStatus(string newStatus)
    {
        var old = PropertyStatus;
        PropertyStatus = newStatus;
        RaiseDomainEvent(new PropertyStatusChangedEvent(Id, old, newStatus));
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    public int? OwnerContactId { get; set; }

    [Required]
    [MaxLength(500)]
    public string StreetAddress { get; set; }

    [MaxLength(100)]
    public string City { get; set; }

    [MaxLength(50)]
    public string State { get; set; }

    [MaxLength(20)]
    public string ZipCode { get; set; }

    [MaxLength(100)]
    public string? County { get; set; }

    [MaxLength(50)]
    public string? PropertyType { get; set; } // Single Family, Multi-Family, Commercial, Land

    [MaxLength(50)]
    public string? PropertyStatus { get; set; } // Active, Pending, Sold, Off-Market

    [MaxLength(50)]
    public string? PropertyCondition { get; set; }

    public int? Bedrooms { get; set; }

    public int? Bathrooms { get; set; }

    public int? SquareFootage { get; set; }

    public int? LotSize { get; set; }

    public int? YearBuilt { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? EstimatedARV { get; set; } // After Repair Value

    [Column(TypeName = "decimal(18,2)")]
    public decimal? EstimatedRepairCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PurchasePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ListPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SellerAskingPrice { get; set; }

    public string? PropertyIssues { get; set; } // JSON serialized

    public string? Notes { get; set; }

    [MaxLength(50)]
    public string? AcquisitionStrategy { get; set; } // Wholesale, Fix & Flip, Buy & Hold

    [MaxLength(2048)]
    public string? ImageUrl { get; set; }

    [Column(TypeName = "decimal(9,6)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(9,6)")]
    public decimal? Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    [ForeignKey("OwnerContactId")]
    public virtual Contact OwnerContact { get; set; }
    // Collection properties

    public virtual ICollection<PropertyFile> Files { get; set; } = new List<PropertyFile>();
    public virtual ICollection<PropertyActivity> Activities { get; set; } = new List<PropertyActivity>();
    public virtual ICollection<PropertyBoard> PropertyBoards { get; set; } = new List<PropertyBoard>();
    public virtual ICollection<Offer> Offers { get; set; }
    public virtual ICollection<PropertyImage> Images { get; set; }
    = new List<PropertyImage>();
    public virtual ICollection<PropertyList> PropertyLists { get; set; }
    public virtual ICollection<PropertyTag> PropertyTags { get; set; }
    public virtual PropertyInteractionCount InteractionCounts { get; set; }
    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    public virtual ICollection<PropertyDocument> Documents { get; set; } = new List<PropertyDocument>();
    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
    public virtual ICollection<DirectMailCampaign> DirectMailCampaigns { get; set; } = new List<DirectMailCampaign>();
}
