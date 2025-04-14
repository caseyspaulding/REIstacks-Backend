// In REIstack.Domain.Models
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.UploadLeads;

[Table("lead_list_files")]
public class LeadListFile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; }

    [Required]
    public string BlobUrl { get; set; }

    public string MappingConfig { get; set; }

    public string Tags { get; set; } = string.Empty;

    public int? RecordsCount { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public bool IsProcessed { get; set; } = false;

    public DateTime? ProcessedAt { get; set; }

    // Navigation property
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    public virtual ICollection<ImportJob> ImportJobs { get; set; }
}