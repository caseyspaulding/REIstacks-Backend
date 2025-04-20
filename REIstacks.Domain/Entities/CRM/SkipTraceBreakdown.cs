using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;

[Table("skip_trace_breakdowns")]
public class SkipTraceBreakdown
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SkipTraceActivityId { get; set; }

    [Required, MaxLength(100)]
    public string Category { get; set; } // e.g. “Properties”, “Owners”, …

    public int Count { get; set; } // e.g. 1119, 765, …

    [ForeignKey(nameof(SkipTraceActivityId))]
    public SkipTraceActivity Activity { get; set; }
}