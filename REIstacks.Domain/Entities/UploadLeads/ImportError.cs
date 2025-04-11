// In REIstack.Domain.Models
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.UploadLeads;

[Table("import_errors")]
public class ImportError
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    [Required]
    public int JobId { get; set; }

    [Required]
    public int RowNumber { get; set; }

    [Required]
    [MaxLength(100)]
    public string Field { get; set; }

    // Add this missing property
    public DateTime OccurredAt { get; set; }
    [Required]
    public string ErrorMessage { get; set; }

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    // Navigation property
    [ForeignKey("JobId")]
    public virtual ImportJob Job { get; set; }
}