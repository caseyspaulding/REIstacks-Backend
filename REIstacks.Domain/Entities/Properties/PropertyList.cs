using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Properties
{
    [Table("property_lists")]
    public class PropertyList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int ListId { get; set; }

        [Required]
        [MaxLength(450)]
        public string OrganizationId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        [ForeignKey(nameof(ListId))]
        public virtual List? List { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization Organization { get; set; }
    }
}
