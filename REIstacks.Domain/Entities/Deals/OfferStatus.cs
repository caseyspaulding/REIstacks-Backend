using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Deals;
[Table("offer_statuses")]
public class OfferStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string Description { get; set; }

    // Navigation property
    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}