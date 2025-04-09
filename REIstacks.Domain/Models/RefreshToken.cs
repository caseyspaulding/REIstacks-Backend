using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace REIstack.Domain.Models;

[Table("refresh_tokens")]
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public Guid ProfileId { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string IpAddress { get; set; }

    public string UserAgent { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ProfileId")]
    public virtual UserProfile Profile { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; internal set; }

    // You can remove this and use RevokedAt != null instead
    // public bool IsRevoked { get; set; } = false;

    // This seems redundant with ExpiresAt
    // public DateTime ExpiryDate { get; set; }
}