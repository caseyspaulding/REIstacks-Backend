using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;
[Table("phone_tags")]
public class PhoneTag
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PhoneId { get; set; }

    [Required]
    public int TagId { get; set; }

    [ForeignKey("PhoneId")]
    public virtual ContactPhone Phone { get; set; }

    [ForeignKey("TagId")]
    public virtual Tag Tag { get; set; }
}
