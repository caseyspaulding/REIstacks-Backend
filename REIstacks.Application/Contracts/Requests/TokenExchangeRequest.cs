using System.Text.Json.Serialization;

namespace REIstacks.Application.Contracts.Requests;

public class TokenExchangeRequest
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("codeVerifier")]
    public string CodeVerifier { get; set; }

    [JsonPropertyName("redirectUri")]
    public string RedirectUri { get; set; }
}
