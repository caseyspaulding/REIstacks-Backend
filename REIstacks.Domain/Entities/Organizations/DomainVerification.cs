using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Organizations;

[Table("domain_verifications")]
public class DomainVerification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Domain { get; set; }

    [Required]
    [MaxLength(64)]
    public string VerificationToken { get; set; }

    public bool IsVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? VerifiedAt { get; set; }

    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }
    public DateTime UpdatedAt { get; set; }
}