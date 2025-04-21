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

    // Add CreatedAt field for proper timestamp tracking
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    [Required]
    public SkipTraceStatus Status { get; set; } = SkipTraceStatus.Pending;

    // Add count tracking fields
    public int Total { get; set; } // Total records to process
    public int Matched { get; set; } // Successfully matched
    public int Failed { get; set; } // Failed to match
    public int Pending { get; set; } // Still waiting to be processed

    // Existing fields
    public int ProcessedCount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Saved { get; set; }

    public int TotalRowCount { get; set; }

    public string? RawResponseJson { get; set; }

    public virtual ICollection<SkipTraceBreakdown> Breakdown { get; set; }
        = new List<SkipTraceBreakdown>();

    public virtual ICollection<SkipTraceItem> Items { get; set; }
        = new List<SkipTraceItem>();
    public string? ErrorMessage { get; set; } = string.Empty;
}