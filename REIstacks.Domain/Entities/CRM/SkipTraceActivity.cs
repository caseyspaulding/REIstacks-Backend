using REIstacks.Domain.Entities.CRM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("skip_trace_activities")]
public class SkipTraceActivity
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(450)]
    public string OrganizationId { get; set; }

    public int ProcessedCount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Saved { get; set; }

    public DateTime CompletedAt { get; set; }

    [Required]
    public SkipTraceStatus Status { get; set; } = SkipTraceStatus.Pending;

    public string? RawResponseJson { get; set; }

    public virtual ICollection<SkipTraceBreakdown> Breakdown { get; set; }
        = new List<SkipTraceBreakdown>();

    public virtual ICollection<SkipTraceItem> Items { get; set; }
        = new List<SkipTraceItem>();
}
