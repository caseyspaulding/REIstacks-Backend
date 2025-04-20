using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;
[Table("phone_statuses")]
public class PhoneStatus
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

    public virtual ICollection<ContactPhone> PhoneNumbers { get; set; } = new List<ContactPhone>();
}