// In REIstack.Domain.Models
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstack.Domain.Models;

[Table("import_jobs")]
public class ImportJob
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int FileId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; }

    [Required]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public string ErrorMessage { get; set; }

    public int? RecordsProcessed { get; set; }

    public int? RecordsImported { get; set; }

    public int? RecordsRejected { get; set; }

    // Navigation property
    [ForeignKey("FileId")]
    public virtual LeadListFile File { get; set; }

    public virtual ICollection<ImportError> ImportErrors { get; set; }
}