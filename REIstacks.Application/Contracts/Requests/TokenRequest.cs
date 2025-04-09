namespace REIstacks.Application.Contracts.Requests;

public class TokenRequest
{
    public string Code { get; set; }
    public string CodeVerifier { get; set; }
    public string State { get; set; }
    public string RedirectUri { get; set; }
    public string? Nonce { get; set; }
}