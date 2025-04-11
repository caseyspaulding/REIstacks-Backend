using REIstacks.Domain.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Auth;

[Table("external_auth")]
public class ExternalAuth
{
    [Key]
    public Guid Id { get; set; }

    public Guid ProfileId { get; set; }

    [MaxLength(255)]
    public string Provider { get; set; } = "Google"; // For future expansion

    [MaxLength(255)]
    public string? ExternalId { get; set; } // Google's unique user ID

    public string? RefreshToken { get; set; }

    [ForeignKey("ProfileId")]
    public virtual UserProfile Profile { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string AccessToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
}