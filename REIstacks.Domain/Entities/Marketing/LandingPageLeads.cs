using REIstacks.Domain.Entities.CRM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Marketing;

[Table("LandingPageLeads")]
public class LandingPageLead
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid LandingPageId { get; set; }

    [Required]
    public int LeadId { get; set; }  // Ties back to your existing Lead entity

    [Required]
    public DateTime ConversionDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("LandingPageId")]
    public virtual LandingPages LandingPage { get; set; }

    [ForeignKey("LeadId")]
    public virtual Lead Lead { get; set; }
}
