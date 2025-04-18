namespace REIstacks.Application.Contracts.Requests;
public class ContactDto
{
    public int Id { get; set; }
    public string OrganizationId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string AlternatePhone { get; set; }
    public string Company { get; set; }
    public string Title { get; set; }
    public string PreferredContactMethod { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public bool IsActive { get; set; }
    public string ContactType { get; set; }
    public string LeadSource { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Tags { get; set; }
    public string Notes { get; set; }
    public int? Clicks { get; set; }
    public int? Opens { get; set; }
    public int? SMSResponses { get; set; }
    public int? CallsMade { get; set; }
    public int? MessagesLeft { get; set; }
    public DateTime? LastContacted { get; set; }
    public bool ConsentTextMessages { get; set; }
    public bool ConsentEmailMarketing { get; set; }

    // Organization data (just what's needed)
    public string OrganizationName { get; set; }

    // Counts instead of collections
    public int PropertiesCount { get; set; }
    public int DealsCount { get; set; }
    public int CommunicationsCount { get; set; }
    public int TasksCount { get; set; }
}