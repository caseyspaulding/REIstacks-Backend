using System.Text.Json.Serialization;

namespace REIstacks.Application.Contracts.Requests;

public class RefreshTokenRequest
{
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}
