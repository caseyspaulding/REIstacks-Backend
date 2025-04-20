using REIstacks.Domain.Entities.CRM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("contact_emails")]
public class ContactEmail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ContactId { get; set; }

    [Required]
    [MaxLength(255)]
    public string EmailAddress { get; set; }

    public bool IsDoNotEmail { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("ContactId")]
    public virtual Contact Contact { get; set; }
}