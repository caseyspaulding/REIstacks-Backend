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
    public string Status { get; set; } // Active, Accepted, Rejected, etc.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("PropertyId")]
    public virtual Property Property { get; set; }
    public virtual ICollection<OfferDocument> Documents { get; set; }
}
