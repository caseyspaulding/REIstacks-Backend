using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Properties;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Deals;

[Table("offers")]
public class Offer
{
    [Key]
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public decimal OfferAmount { get; set; }

    // Add OpportunityId
    public int? OpportunityId { get; set; }

    // Expand status to use a proper enum or reference table
    public int OfferStatusId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("PropertyId")]
    public virtual Property Property { get; set; }

    [ForeignKey("OpportunityId")]
    public virtual Opportunity Opportunity { get; set; }

    [ForeignKey("OfferStatusId")]
    public virtual OfferStatus Status { get; set; }

    public virtual ICollection<OfferDocument> Documents { get; set; }
}