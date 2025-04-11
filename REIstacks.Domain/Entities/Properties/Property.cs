
using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Deals;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Properties;
[Table("properties")]
public class Property
{
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

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    [ForeignKey("OwnerContactId")]
    public virtual Contact OwnerContact { get; set; }

    // Collection properties
    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    public virtual ICollection<PropertyDocument> Documents { get; set; } = new List<PropertyDocument>();
}
