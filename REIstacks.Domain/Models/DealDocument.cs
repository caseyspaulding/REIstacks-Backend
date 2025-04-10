using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Models;
[Table("deal_documents")]
public class DealDocument
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int DealId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; }

    [Required]
    public string BlobUrl { get; set; }

    [MaxLength(50)]
    public string? DocumentType { get; set; } // Contract, Assignment, HUD-1

    [MaxLength(255)]
    public string? Description { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("DealId")]
    public virtual Deal Deal { get; set; }
}