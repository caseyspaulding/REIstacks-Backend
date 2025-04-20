using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Properties
{
    [Table("property_documents")]
    public class PropertyDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required, MaxLength(450)]
        public string OrganizationId { get; set; }    // ← tenant key

        [Required, MaxLength(255)]
        public string FileName { get; set; }

        [MaxLength(50)]
        public string FileType { get; set; }

        [MaxLength(2048)]
        public string FileUrl { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(PropertyId))]
        public virtual Property Property { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization Organization { get; set; }
    }
}
