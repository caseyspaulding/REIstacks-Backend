using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.Properties;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;
[Table("tags")]
public class Tag
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string OrganizationId { get; set; }


    [MaxLength(20)]
    public string Color { get; set; }

    [Required]
    [MaxLength(50)]
    public string TagType { get; set; } // "Property", "Phone", "Contact"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }
    public virtual ICollection<PhoneTag> PhoneTags { get; set; } = new List<PhoneTag>();
    public virtual ICollection<PropertyTag> PropertyTags { get; set; }
    public virtual ICollection<ContactTag> ContactTags { get; set; } = new List<ContactTag>();
}
