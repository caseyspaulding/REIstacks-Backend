
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Billing;

[Table("stripe_subscriptions")]
public class StripeSubscription
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // ✅ Make OrganizationId nullable
    public string? OrganizationId { get; set; }

    [Required]
    public string StripeSubscriptionId { get; set; }

    [Required]
    public string PlanId { get; set; }

    [Required]
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trialing;

    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; } = false;
    public DateTime? TrialStart { get; set; }
    public DateTime? TrialEnd { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // ✅ Update Foreign Key Relationship
    [ForeignKey("OrganizationId")]
    public virtual Organization? Organization { get; set; }
}
