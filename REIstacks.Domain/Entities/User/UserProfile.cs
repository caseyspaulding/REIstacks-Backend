using REIstacks.Domain.Entities.Auth;
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.User;

[Table("profiles")]
public class UserProfile
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Email { get; set; }

    public string Name { get; set; }

    public string? AvatarUrl { get; set; }
    public bool SmsConsent { get; set; } = false;
    public DateTime? SmsConsentDate { get; set; }
    public string? ExternalId { get; set; }  // Google's unique user ID
    public string? ExternalProvider { get; set; } = "Google";  // In case you add more providers later
    public DateTime? LastLogin { get; set; }
    public bool IsSetupComplete { get; set; } = false;
    public string? RefreshToken { get; set; }  // Optional - if you want to refresh Google tokens
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<ExternalAuth> ExternalAuths { get; set; }

    [Required]
    public Role Role { get; set; } = Role.Member;


    public string? OrganizationId { get; set; } // Nullable for users who haven't joined an organization yet


    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;


    public DateTime? EmailVerifiedAt { get; set; }
    public bool IsOnboarded { get; set; } = false;

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }
    public int? OrganizationRoleId { get; set; }
    public virtual ICollection<Organization> OwnedOrganizations { get; set; }

    [ForeignKey("OrganizationRoleId")]
    public virtual OrganizationRole OrganizationRole { get; set; }
    public virtual ICollection<Invitation> InvitationsSent { get; set; }
    public virtual ICollection<ActivityLog> ActivityLogs { get; set; }
}
