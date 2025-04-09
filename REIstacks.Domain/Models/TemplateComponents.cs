using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstack.Domain.Models;

[Table("TemplateComponents")]
public class TemplateComponent
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid TemplateId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ComponentType { get; set; }  // e.g. "hero", "form", "gallery"

    public int OrderIndex { get; set; }

    // Default settings for each component
    public string DefaultSettings { get; set; }

    [ForeignKey("TemplateId")]
    public virtual Template Template { get; set; }
}
