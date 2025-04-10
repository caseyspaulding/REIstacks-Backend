// In REIstack.Domain.Models
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstack.Domain.Models;

[Table("field_mapping_templates")]
public class FieldMappingTemplate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(450)]
    public string OrganizationId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(255)]
    public string Description { get; set; }

    [Required]
    public string MappingConfig { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }
}