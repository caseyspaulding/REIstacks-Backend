using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;
[Table("contact_tags")]
public class ContactTag
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ContactId { get; set; }

    [Required]
    public int TagId { get; set; }

    [ForeignKey("ContactId")]
    public virtual Contact Contact { get; set; }

    [ForeignKey("TagId")]
    public virtual Tag Tag { get; set; }
}
