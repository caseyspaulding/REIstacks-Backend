

using Microsoft.Extensions.Configuration;
using REIstacks.Domain.Entities.Properties;
using System.Net.Http.Json;

namespace REIstacks.Infrastructure.Services.PropertyData;

public class RentCastService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public RentCastService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["RentCast:ApiKey"];
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
        _httpClient.BaseAddress = new Uri("https://api.rentcast.io/v1/");
    }

    // Get property by address
    public async Task<PropertyRecord> GetPropertyByAddressAsync(string address)
    {
        var response = await _httpClient.GetAsync($"properties?address={Uri.EscapeDataString(address)}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<List<PropertyRecord>>();
        return content?.FirstOrDefault();
    }

    // Get property value estimate
    public async Task<ValueEstimate> GetPropertyValueEstimateAsync(string address)
    {
        var response = await _httpClient.GetAsync($"avm/value?address={Uri.EscapeDataString(address)}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ValueEstimate>();
    }
}