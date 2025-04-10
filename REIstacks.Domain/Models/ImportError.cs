// In REIstack.Domain.Models
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstack.Domain.Models;

[Table("import_errors")]
public class ImportError
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int JobId { get; set; }

    [Required]
    public int RowNumber { get; set; }

    [Required]
    [MaxLength(100)]
    public string Field { get; set; }

    [Required]
    public string ErrorMessage { get; set; }

    // Navigation property
    [ForeignKey("JobId")]
    public virtual ImportJob Job { get; set; }
}