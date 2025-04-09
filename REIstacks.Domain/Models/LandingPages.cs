
using REIstack.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Models;

[Table("LandingPages")]
public class LandingPages
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(450)]
    public string OrganizationId { get; set; }  // Ties to your Organization primary key

    public string Title { get; set; }
    public string Description { get; set; }


    [MaxLength(255)]
    public string Slug { get; set; }

    public Guid? TemplateId { get; set; }  // Optional reference to a Template

    public bool IsPublished { get; set; } = false;

    public string MetaTitle { get; set; }

    public string HeroImageUrl { get; set; }

    [MaxLength(2000)]
    public string MetaDescription { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    [ForeignKey("TemplateId")]
    public virtual Template Template { get; set; }

    public virtual ICollection<LandingPageComponent> Components { get; set; }
    public virtual ICollection<LandingPageLead> Leads { get; set; }
}