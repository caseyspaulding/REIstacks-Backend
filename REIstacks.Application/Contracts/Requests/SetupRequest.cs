namespace reistacks_api.Contracts.Requests;

public class SetupRequest
{
    public string Token { get; set; }
    public string Nonce { get; set; }
    public string OrganizationName { get; set; }
    public string Subdomain { get; set; }

}


