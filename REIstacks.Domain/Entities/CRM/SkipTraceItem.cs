using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// add this:
using System.Text.Json.Serialization;

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

        // Basic input information
        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(255)]
        public string? StreetAddress { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? ZipCode { get; set; }

        // Skip trace result fields
        [MaxLength(50)]
        public string MatchStatus { get; set; } = "Unknown"; // Matched, PartialMatch, NoMatch

        [MaxLength(50)]
        public string? PhoneNumber { get; set; }

        [MaxLength(50)]
        public string? PhoneType { get; set; } // Landline, Mobile, etc.

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? DateOfBirth { get; set; }

        // Additional fields if available in API response
        [MaxLength(255)]
        public string? CurrentAddress { get; set; }

        [MaxLength(150)]
        public string? PersonLink { get; set; }

        [MaxLength(50)]
        public string? Age { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending";    // Pending, InProgress, Completed, Failed

        [Required, MaxLength(50)]
        public string Category { get; set; } = "Default";  // For breakdown grouping

        public string? RawResponseJson { get; set; }        // Store the full JSON result for this item

        [ForeignKey(nameof(SkipTraceActivityId))]
        [JsonIgnore]    // <- prevents the cycle by not serializing back to the parent
        public virtual SkipTraceActivity? Activity { get; set; }
    }
}
