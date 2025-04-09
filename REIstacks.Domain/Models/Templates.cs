using REIstacks.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstack.Domain.Models;

[Table("Templates")]
public class Template
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [MaxLength(2000)]
    public string Description { get; set; }

    [MaxLength(1000)]
    public string ThumbnailUrl { get; set; }

    public bool IsSystem { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<TemplateComponent> Components { get; set; }
    public virtual ICollection<LandingPages> LandingPages { get; set; }
}