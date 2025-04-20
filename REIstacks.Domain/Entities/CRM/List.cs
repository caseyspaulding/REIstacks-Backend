using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.Properties;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;
[Table("lists")]
public class List
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string OrganizationId { get; set; }

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }
    public virtual ICollection<PropertyList> PropertyLists { get; set; }
    public string? Description { get; set; }
}