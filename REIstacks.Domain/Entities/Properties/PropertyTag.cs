using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Properties;
[Table("property_tags")]
public class PropertyTag
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PropertyId { get; set; }

    [Required]
    public required string OrganizationId { get; set; }   // new
    [Required]
    public int TagId { get; set; }

    public DateTime TaggedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("PropertyId")]
    public virtual Property Property { get; set; }

    [ForeignKey("TagId")]
    public virtual Tag Tag { get; set; }
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }  // new navigation
}