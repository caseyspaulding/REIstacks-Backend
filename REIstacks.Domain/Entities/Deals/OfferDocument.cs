using REIstacks.Domain.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Deals;

[Table("offer_documents")]
public class OfferDocument
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int OfferId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; }

    [Required]
    [MaxLength(2048)]
    public string FileUrl { get; set; }

    [MaxLength(50)]
    public string FileType { get; set; }

    public long FileSize { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("OfferId")]
    public virtual Offer Offer { get; set; }

    [ForeignKey("UserId")]
    public virtual UserProfile User { get; set; }
}