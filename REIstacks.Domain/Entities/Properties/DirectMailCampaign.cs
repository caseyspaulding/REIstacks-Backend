namespace REIstacks.Domain.Entities.Properties;
public class DirectMailCampaign
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public Property Property { get; set; }

    public DateTime SentDate { get; set; }
    public DateTime? CompletedAt { get; set; }

    public CampaignStatus Status { get; set; }
    // you can extend with fields like: TemplateUsed, ResponseRate, Cost, etc.
}