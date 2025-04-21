using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;
[Table("opportunity_stages")]
public class OpportunityStage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }



    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public int DisplayOrder { get; set; }

    // For the UI progress indicator
    public bool IsCompleted { get; set; }


    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}