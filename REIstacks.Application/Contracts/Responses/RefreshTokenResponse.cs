﻿using System.Text.Json.Serialization;

namespace REIstacks.Application.Contracts.Responses;

public class RefreshTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}
