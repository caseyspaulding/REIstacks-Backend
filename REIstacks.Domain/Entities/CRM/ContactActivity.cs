using REIstacks.Domain.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM
{
    [Table("contact_activities")]
    public class ContactActivity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        [Required]
        public int ContactId { get; set; }

        [Required]
        public ActivityType Type { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public Guid CreatedByProfileId { get; set; }

        // only one navigation here, pointing at CreatedByProfileId
        [ForeignKey(nameof(CreatedByProfileId))]
        public virtual UserProfile CreatedByProfile { get; set; }

        // e.g. "Owner was called at (812)526‑5510"
        [Required, MaxLength(500)]
        public string Description { get; set; }

        // optional JSON payload for any extra bits (direction, recording URL…)
        public string? MetadataJson { get; set; }

        [ForeignKey(nameof(ContactId))]
        public virtual Contact Contact { get; set; }
    }
}
