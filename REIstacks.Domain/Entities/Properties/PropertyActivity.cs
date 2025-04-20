using REIstacks.Domain.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Properties;

[Table("property_activities")]
public class PropertyActivity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PropertyId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ActivityType { get; set; } // "AssigneeChanged", "CallMissed", "CallReceived", etc.

    public string Description { get; set; } // Formatted description of the activity

    public DateTime Timestamp { get; set; }

    public Guid? UserId { get; set; } // User who performed or logged the activity

    // Change from string to Guid to match UserProfile.Id
    public Guid? TargetUserId { get; set; } // For assignments, the user being assigned

    [MaxLength(50)]
    public string RelatedEntityType { get; set; } // "Contact", "Phone", etc.

    public string RelatedEntityId { get; set; } // ID of related entity if relevant

    [MaxLength(100)]
    public string PageSource { get; set; } // Where this activity was created from

    // Navigation properties
    [ForeignKey("PropertyId")]
    public virtual Property Property { get; set; }

    [ForeignKey("UserId")]
    public virtual UserProfile User { get; set; }

    [ForeignKey("TargetUserId")]
    public virtual UserProfile TargetUser { get; set; }
}