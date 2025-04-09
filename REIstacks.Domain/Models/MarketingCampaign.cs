using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstack.Domain.Models
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

        public string? Type { get; set; } // Email, SMS, Direct Mail, Facebook Ads, etc.

        [Column(TypeName = "decimal(18,2)")]
        public decimal Budget { get; set; } // Budget allocated for the campaign

        public int LeadsGenerated { get; set; } // Number of leads collected

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPerLead { get; set; } // Budget / LeadsGenerated

        public int Conversions { get; set; } // Number of leads turned into deals

        [Column(TypeName = "decimal(18,2)")]
        public decimal ROI { get; set; } // Return on investment

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public virtual ICollection<Lead> Leads { get; set; }
    }
}
