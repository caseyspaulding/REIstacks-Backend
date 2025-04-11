using REIstacks.Domain.Entities.CRM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Marketing;
// Many-to-many relationship between Campaigns and Contacts
[Table("campaign_contacts")]
public class CampaignContact
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CampaignId { get; set; }

    [Required]
    public int ContactId { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } // Pending, Sent, Responded, Converted

    public DateTime AddedDate { get; set; } = DateTime.UtcNow;

    public DateTime? LastContactedDate { get; set; }

    // Navigation properties
    [ForeignKey("CampaignId")]
    public virtual MarketingCampaign Campaign { get; set; }

    [ForeignKey("ContactId")]
    public virtual Contact Contact { get; set; }
}