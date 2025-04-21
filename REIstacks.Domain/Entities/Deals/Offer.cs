using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.Properties;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Deals
{
    [Table("offers")]
    public class Offer
    {
        [Key]
        public int Id { get; set; }

        // ← new: tie back to tenant
        [Required, MaxLength(450)]
        public string OrganizationId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OfferAmount { get; set; }

        // ← new: optional link into your opportunity pipeline
        public int? OpportunityId { get; set; }

        public int OfferStatusId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // navigation:
        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization Organization { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property Property { get; set; }

        [ForeignKey(nameof(OpportunityId))]
        public virtual Opportunity Opportunity { get; set; }

        [ForeignKey(nameof(OfferStatusId))]
        public virtual OfferStatus Status { get; set; }

        public virtual ICollection<OfferDocument> Documents { get; set; }
            = new List<OfferDocument>();
    }
}
