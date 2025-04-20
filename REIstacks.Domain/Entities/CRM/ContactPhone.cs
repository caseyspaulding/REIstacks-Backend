using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;

[Table("contact_phones")]
public class ContactPhone
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ContactId { get; set; }

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; }

    [MaxLength(20)]
    public string PhoneType { get; set; } // Mobile, Landline

    [MaxLength(50)]
    public string? StatusId { get; set; }

    public int? Rating { get; set; } // 1-5 rating

    public bool IsDoNotCall { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("ContactId")]
    public virtual Contact Contact { get; set; }

    [ForeignKey("StatusId")]
    public virtual PhoneStatus? Status { get; set; }

    public virtual ICollection<PhoneTag> PhoneTags { get; set; } = new List<PhoneTag>();
}

