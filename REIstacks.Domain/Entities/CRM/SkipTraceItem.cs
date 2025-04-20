using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM
{
    [Table("skip_trace_items")]
    public class SkipTraceItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SkipTraceActivityId { get; set; }

        public int? ContactId { get; set; }
        public int? PropertyId { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending";    // Pending, InProgress, Completed, Failed

        [Required, MaxLength(50)]
        public string Category { get; set; }               // ← NEW: the “bucket” for your breakdown

        public string? RawResponseJson { get; set; }       // store the JSON result for this item

        [ForeignKey(nameof(SkipTraceActivityId))]
        public virtual SkipTraceActivity Activity { get; set; }
    }
}
