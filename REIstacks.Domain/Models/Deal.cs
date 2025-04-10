using REIstack.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Models;
[Table("deals")]
public class Deal
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    public int PropertyId { get; set; }

    public int? SellerContactId { get; set; }

    public int? BuyerContactId { get; set; }

    [Required]
    [MaxLength(100)]
    public string DealName { get; set; }

    [MaxLength(50)]
    public string DealType { get; set; } // Wholesale, Flip, Buy and Hold

    [MaxLength(50)]
    public string DealStatus { get; set; } // Lead, Negotiating, Under Contract, Closed, Dead

    [MaxLength(50)]
    public string? DealStage { get; set; } // More detailed status tracking

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PurchasePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AssignmentFee { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? RehabBudget { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ActualRehabCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? EstimatedProfit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ActualProfit { get; set; }

    public DateTime? ContractDate { get; set; }

    public DateTime? ClosingDate { get; set; }

    public DateTime? ActualClosingDate { get; set; }

    public string? Notes { get; set; }

    public string? ReasonForSelling { get; set; } // JSON serialized

    public string? Timeline { get; set; } // JSON serialized

    public string? AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    [ForeignKey("PropertyId")]
    public virtual Property Property { get; set; }

    [ForeignKey("SellerContactId")]
    public virtual Contact SellerContact { get; set; }

    [ForeignKey("BuyerContactId")]
    public virtual Contact BuyerContact { get; set; }

    // Collection properties
    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    public virtual ICollection<DealDocument> Documents { get; set; } = new List<DealDocument>();
}