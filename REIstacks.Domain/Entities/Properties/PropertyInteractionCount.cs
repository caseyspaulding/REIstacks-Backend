using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Properties;

[Table("property_interaction_counts")]
public class PropertyInteractionCount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PropertyId { get; set; }

    public int CallAttempts { get; set; } = 0;

    public int DirectMailAttempts { get; set; } = 0;

    public int SMSAttempts { get; set; } = 0;

    public int RVMAttempts { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("PropertyId")]
    public virtual Property Property { get; set; }
}