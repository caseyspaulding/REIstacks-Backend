using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Marketing
{
    [Table("marketing_campaigns")]
    public class MarketingCampaign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; } // Campaign name (e.g., "Facebook Ad - August 2024")

        [MaxLength(255)]
        public string? Description { get; set; }


        [Required]
        [MaxLength(50)]
        public string CampaignType { get; set; } // Direct Mail, Email, SMS, PPC

        [Column(TypeName = "decimal(18,2)")]
        public decimal Budget { get; set; } // Budget allocated for the campaign

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ActualCost { get; set; }

        public int LeadsGenerated { get; set; } // Number of leads collected

        [MaxLength(50)]
        public string? Status { get; set; } // Active, Paused, Completed

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? TargetCriteria { get; set; } // JSON serialized

        public int? DealsGenerated { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPerLead { get; set; } // Budget / LeadsGenerated

        public int Conversions { get; set; } // Number of leads turned into deals

        [Column(TypeName = "decimal(18,2)")]
        public decimal ROI { get; set; } // Return on investment


        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public virtual ICollection<Lead> Leads { get; set; }
        public virtual ICollection<CampaignContact> CampaignContacts { get; set; } = new List<CampaignContact>();
    }
}
