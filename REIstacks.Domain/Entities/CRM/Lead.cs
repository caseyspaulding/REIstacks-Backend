using REIstacks.Domain.Entities.Marketing;
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace REIstacks.Domain.Entities.CRM
{
    [Table("leads")]
    public class Lead
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string OrganizationId { get; set; }

        // Updated to use first/last name instead of full name
        [MaxLength(255)]
        public string FirstName { get; set; }

        [MaxLength(255)]
        public string LastName { get; set; }

        // Original name field kept for backward compatibility
        [MaxLength(255)]
        public string Name { get; set; }

        public string? Company { get; set; }

        public string? Title { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        // New property for form data
        [MaxLength(50)]
        public string? PreferredContactMethod { get; set; }

        // Property address broken down
        [MaxLength(500)]
        public string? PropertyStreetAddress { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public string? City { get; set; }
        public string? PropertyCity { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }
        [MaxLength(50)]
        public string? PropertyState { get; set; }

        [MaxLength(20)]
        public string? ZipCode { get; set; }
        [MaxLength(20)]
        public string? PropertyZipCode { get; set; }

        public string? County { get; set; }

        public string? PropertyType { get; set; }

        // New fields from form data
        public string? PropertyCondition { get; set; }

        // Store as JSON strings
        public string? PropertyIssues { get; set; }
        public string? ReasonForSelling { get; set; }
        public string? Timeline { get; set; }

        public string? TargetPrice { get; set; }

        public string? AdditionalInfo { get; set; }

        public bool ConsentTextMessages { get; set; }
        public bool ConsentPrivacyPolicy { get; set; }

        public int Step { get; set; }

        public int? Bedrooms { get; set; }

        public int? Bathrooms { get; set; }

        public int? SquareFootage { get; set; }

        public int? LotSize { get; set; }

        public int? YearBuilt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AskingPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OfferPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RepairEstimate { get; set; }

        public int? CampaignId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ProfitPotential { get; set; }

        public string? LeadStatus { get; set; }

        public string? LeadSource { get; set; }

        public string? AssignedTo { get; set; }

        public DateTime? LastContacted { get; set; }

        public int? FollowUpDays { get; set; }

        public string? Notes { get; set; }

        public string? MarketingChannel { get; set; }

        public int? Clicks { get; set; }

        public int? Opens { get; set; }

        public int? SMSResponses { get; set; }

        public int? CallsMade { get; set; }

        public int? MessagesLeft { get; set; }

        public DateTime? LastMarketingTouch { get; set; }

        public bool IsRetargeted { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CostPerLead { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ROI { get; set; }

        public string? Message { get; set; }
        public string? Source { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        [ForeignKey("CampaignId")]
        public virtual MarketingCampaign Campaign { get; set; }

        /// <summary>
        /// This is the *one and only* FK to your LeadStage table.
        /// </summary>
        public int? LeadStageId { get; set; }

        /// <summary>
        /// Annotate the navigation so EF Core knows to use LeadStageId.
        /// </summary>
        [ForeignKey(nameof(LeadStageId))]
        public virtual LeadStage? LeadStage { get; set; }


        // Helper methods for JSON conversion
        public void SetPropertyCondition(string[] conditions)
        {
            PropertyCondition = JsonSerializer.Serialize(conditions);
        }

        public string[] GetPropertyCondition()
        {
            return string.IsNullOrEmpty(PropertyCondition)
                ? Array.Empty<string>()
                : JsonSerializer.Deserialize<string[]>(PropertyCondition);
        }

        public void SetPropertyIssues(Dictionary<string, string> issues)
        {
            PropertyIssues = JsonSerializer.Serialize(issues);
        }

        public Dictionary<string, string> GetPropertyIssues()
        {
            return string.IsNullOrEmpty(PropertyIssues)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(PropertyIssues);
        }

        public void SetReasonForSelling(Dictionary<string, string> reasons)
        {
            ReasonForSelling = JsonSerializer.Serialize(reasons);
        }

        public Dictionary<string, string> GetReasonForSelling()
        {
            return string.IsNullOrEmpty(ReasonForSelling)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(ReasonForSelling);
        }



        public void SetTimeline(string[] timeline)
        {
            Timeline = JsonSerializer.Serialize(timeline);
        }

        public string[] GetTimeline()
        {
            return string.IsNullOrEmpty(Timeline)
                ? Array.Empty<string>()
                : JsonSerializer.Deserialize<string[]>(Timeline);
        }
    }
}