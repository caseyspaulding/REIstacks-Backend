namespace REIstacks.Domain.Events;
public class PropertyStatusChangedEvent : IDomainEvent
{
    public int PropertyId { get; }
    public string From { get; }
    public string To { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PropertyStatusChangedEvent(
        int propertyId, string from, string to)
    {
        PropertyId = propertyId;
        From = from;
        To = to;
    }
}