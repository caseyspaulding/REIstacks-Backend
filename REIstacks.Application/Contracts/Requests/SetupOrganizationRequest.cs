// Contracts/Requests/SetupOrganizationRequest.cs
namespace REIstacks.Application.Contracts.Requests
{
    public class SetupOrganizationRequest
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string OrganizationName { get; set; }
        public string Subdomain { get; set; }
        public List<string> Vas { get; set; } = new List<string>();
        public string? LeadsData { get; set; }
    }
}