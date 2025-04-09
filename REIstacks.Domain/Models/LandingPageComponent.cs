using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Models;

[Table("LandingPageComponents")]
public class LandingPageComponent
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid LandingPageId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ComponentType { get; set; }  // e.g. "hero", "form", "testimonials"

    public int OrderIndex { get; set; }

    // JSON with per-component settings (headingText, buttonUrl, backgroundImage, etc.)
    public string Settings { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("LandingPageId")]
    public virtual LandingPages LandingPage { get; set; }
}
