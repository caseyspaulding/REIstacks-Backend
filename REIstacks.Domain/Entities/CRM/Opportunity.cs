using REIstacks.Domain.Entities.Deals;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.Properties;
using REIstacks.Domain.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;
[Table("opportunities")]
public class Opportunity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public int Score { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public int StageId { get; set; }

    public DateTime? CloseDate { get; set; }

    // replace your old OwnerId (int?) with this:
    public Guid? OwnerProfileId { get; set; }

    // Foreign keys to related entities
    public int? PropertyId { get; set; }
    public int? ContactId { get; set; }
    public int? DealId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }
    [ForeignKey(nameof(OwnerProfileId))]
    public virtual UserProfile? Owner { get; set; }

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    [ForeignKey("StageId")]
    public virtual OpportunityStage Stage { get; set; }


    public virtual Property Property { get; set; }

    [ForeignKey("ContactId")]
    public virtual Contact Contact { get; set; }

    [ForeignKey("DealId")]
    public virtual Deal Deal { get; set; }
}