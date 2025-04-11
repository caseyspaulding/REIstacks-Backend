using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Properties;
// For property-related documents
[Table("property_documents")]
public class PropertyDocument
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PropertyId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; }

    [Required]
    public string BlobUrl { get; set; }

    [MaxLength(50)]
    public string? DocumentType { get; set; } // Photos, Inspection, Appraisal

    [MaxLength(255)]
    public string? Description { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("PropertyId")]
    public virtual Property Property { get; set; }
}
