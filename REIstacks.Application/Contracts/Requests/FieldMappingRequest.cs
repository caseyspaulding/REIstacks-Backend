namespace REIstacks.Application.Contracts.Requests;
public class FieldMappingRequest
{
    public string FirstNameColumn { get; set; }
    public string LastNameColumn { get; set; }
    public string StreetColumn { get; set; }
    public string CityColumn { get; set; }
    public string StateColumn { get; set; }
    public string ZipColumn { get; set; }
}