using System.Text.Json.Serialization;

namespace REIstacks.Application.Contracts.Responses;

public class ProfileResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("pictureUrl")]
    public string PictureUrl { get; set; }

    [JsonPropertyName("isSetupComplete")]
    public bool IsSetupComplete { get; set; }
}