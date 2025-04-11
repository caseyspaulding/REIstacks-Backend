using REIstacks.Domain.Entities.Deals;
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;

// For tracking communications with contacts
[Table("communications")]
public class Communication
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    [Required]
    public int ContactId { get; set; }

    public int? DealId { get; set; }

    [Required]
    [MaxLength(50)]
    public string CommunicationType { get; set; } // Email, Call, Text, Meeting

    [Required]
    public DateTime CommunicationDate { get; set; } = DateTime.UtcNow;

    [MaxLength(255)]
    public string? Subject { get; set; }

    public string? Content { get; set; }

    [MaxLength(50)]
    public string? Direction { get; set; } // Inbound, Outbound

    [MaxLength(50)]
    public string? Status { get; set; } // Sent, Received, Failed

    public string? Notes { get; set; }

    public string? CreatedBy { get; set; }

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }

    [ForeignKey("ContactId")]
    public virtual Contact Contact { get; set; }

    [ForeignKey("DealId")]
    public virtual Deal Deal { get; set; }
}