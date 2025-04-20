// Domain/Entities/CRM/SkipTraceActivity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM
{
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

        [Required, MaxLength(50)]
        public string Status { get; set; }

        public virtual ICollection<SkipTraceBreakdown> Breakdown { get; set; }
            = new List<SkipTraceBreakdown>();
    }


}
