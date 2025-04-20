using REIstacks.Domain.Entities.Properties;
using REIstacks.Domain.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;

[Table("property_communications")]
public class PropertyCommunication
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PropertyId { get; set; }

    public int? ContactId { get; set; }

    [Required]
    [MaxLength(50)]
    public string CommunicationType { get; set; } // Call, SMS, Email, DirectMail, RVM

    [MaxLength(50)]
    public string Status { get; set; } // Attempted, Completed, Failed

    public string? Notes { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("PropertyId")]
    public virtual Property Property { get; set; }

    [ForeignKey("ContactId")]
    public virtual Contact Contact { get; set; }

    [ForeignKey("UserId")]
    public virtual UserProfile User { get; set; }
}