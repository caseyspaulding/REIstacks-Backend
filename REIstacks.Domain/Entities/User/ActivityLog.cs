using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.User;

[Table("activity_logs")]
public class ActivityLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }


    public string? OrganizationId { get; set; }

    public Guid? UserId { get; set; }


    public string? Action { get; set; }


    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    [ForeignKey("UserId")]
    public virtual UserProfile User { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ProfileId { get; set; } = Guid.Empty;
    public string? ActionType { get; set; } = "Unknown";
    public string? Details { get; set; } = string.Empty;
}
