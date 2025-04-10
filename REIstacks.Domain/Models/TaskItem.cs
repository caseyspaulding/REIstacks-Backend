using REIstack.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Models;

[Table("task_items")]
public class TaskItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    public int? ContactId { get; set; }

    public int? PropertyId { get; set; }

    public int? DealId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string TaskType { get; set; } // Call, Email, Meeting, Site Visit

    [Required]
    [MaxLength(50)]
    public string TaskStatus { get; set; } // Pending, Completed, Cancelled

    [Required]
    public DateTime DueDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    [MaxLength(50)]
    public string? Priority { get; set; } // High, Medium, Low

    public string? AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    [ForeignKey("ContactId")]
    public virtual Contact Contact { get; set; }

    [ForeignKey("PropertyId")]
    public virtual Property Property { get; set; }

    [ForeignKey("DealId")]
    public virtual Deal Deal { get; set; }
}
