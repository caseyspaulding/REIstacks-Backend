using REIstacks.Domain.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Organizations;

[Table("invitations")]
public class Invitation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    public Role Role { get; set; }

    [Required]
    public Guid InvitedBy { get; set; }

    [Required]
    public DateTime InvitedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);

    [MaxLength(20)]
    public string Status { get; set; } = "pending";  // "pending", "accepted", "revoked"

    public string Token { get; set; }  // Unique invite token

    public DateTime? AcceptedAt { get; set; }  // Nullable to indicate acceptance

    public Guid? AcceptedByProfileId { get; set; }  // Nullable until accepted

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  // Tracks changes

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    [ForeignKey("InvitedBy")]
    public virtual UserProfile InvitedByProfile { get; set; }

    [ForeignKey("AcceptedByProfileId")]
    public virtual UserProfile AcceptedByProfile { get; set; }  // Accepted user profile
}
