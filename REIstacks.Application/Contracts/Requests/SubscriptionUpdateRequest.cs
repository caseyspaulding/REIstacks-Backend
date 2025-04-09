namespace REIstacks.Application.Contracts.Requests;

public class SubscriptionUpdateRequest
{
    public string UserId { get; set; }
    public string CustomerId { get; set; }
    public string SubscriptionId { get; set; }
    public string PlanId { get; set; }
    public string SubscriptionStatus { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public DateTime? TrialStart { get; set; }
    public DateTime? TrialEnd { get; set; }
    public string OrganizationId { get; set; }
}

