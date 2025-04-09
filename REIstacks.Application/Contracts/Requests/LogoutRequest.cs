using System.Text.Json.Serialization;

namespace REIstacks.Application.Contracts.Requests;

public class LogoutRequest
{
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}
