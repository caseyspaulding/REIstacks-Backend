using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;
[Table("contact_statuses")]
public class ContactStatus
{
    [Key]
    [MaxLength(50)]
    public string Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Label { get; set; }

    [MaxLength(20)]
    public string Color { get; set; }

    // Navigation properties
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
