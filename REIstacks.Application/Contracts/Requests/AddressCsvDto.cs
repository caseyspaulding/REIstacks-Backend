namespace REIstacks.Application.Contracts.Requests;


public class AddressCsvDto
{
    // Property address
    public string StreetAddress { get; set; }
    public string Unit { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string County { get; set; }
    public string APN { get; set; }

    // Owner information
    public bool OwnerOccupied { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Owner2FirstName { get; set; }
    public string Owner2LastName { get; set; }

    // Mailing information
    public string MailingCareOfName { get; set; }
    public string RelativeName { get; set; }
    public string DeceasedRelativeName { get; set; }
    public string MailingAddress { get; set; }
    public string MailingUnit { get; set; }
    public string MailingCity { get; set; }
    public string MailingState { get; set; }
    public string MailingZip { get; set; }
    public string MailingCounty { get; set; }

    // Contact information - multiple phone numbers
    public string Phone1 { get; set; }
    public string Phone1Type { get; set; }
    public string Phone2 { get; set; }
    public string Phone2Type { get; set; }
    public string Phone3 { get; set; }
    public string Phone3Type { get; set; }
    public string Phone4 { get; set; }
    public string Phone4Type { get; set; }
    public string Phone5 { get; set; }
    public string Phone5Type { get; set; }
    public string Phone6 { get; set; }
    public string Phone6Type { get; set; }
    public string Phone7 { get; set; }
    public string Phone7Type { get; set; }
    public string Phone8 { get; set; }
    public string Phone8Type { get; set; }
    public string Phone9 { get; set; }
    public string Phone9Type { get; set; }
    public string Phone10 { get; set; }
    public string Phone10Type { get; set; }

    // Multiple email addresses
    public string Email1 { get; set; }
    public string Email2 { get; set; }
    public string Email3 { get; set; }
    public string Email4 { get; set; }
    public string Email5 { get; set; }

    // Property details
    public bool DoNotMail { get; set; }
    public string PropertyType { get; set; }
    public int? Bedrooms { get; set; }
    public decimal? TotalBathrooms { get; set; }
    public int? BuildingSqft { get; set; }
    public int? LotSizeSqft { get; set; }
    public int? EffectiveYearBuilt { get; set; }
    public decimal? TotalAssessedValue { get; set; }
    public DateTime? LastSaleRecordingDate { get; set; }
    public decimal? LastSaleAmount { get; set; }
}