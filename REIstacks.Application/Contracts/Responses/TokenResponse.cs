using System.Text.Json.Serialization;

namespace REIstacks.Application.Contracts.Responses;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("profile")]
    public ProfileResponse Profile { get; set; }
}