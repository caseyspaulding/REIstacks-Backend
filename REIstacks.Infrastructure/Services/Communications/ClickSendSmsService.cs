using Microsoft.Extensions.Configuration;
using REIstacks.Application.Interfaces.IServices;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace REIstacks.Infrastructure.Services
{
    public class ClickSendSmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://rest.clicksend.com/v3";
        private readonly string _username;
        private readonly string _apiKey;

        public ClickSendSmsService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ClickSend");
            _username = configuration["ClickSend:Username"];
            _apiKey = configuration["ClickSend:ApiKey"];

            // Setup authentication
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_apiKey}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                // Create the message payload
                var smsPayload = new
                {
                    messages = new[]
                    {
                        new
                        {
                            source = "REIstacks",
                            body = message,
                            to = phoneNumber
                        }
                    }
                };

                // Serialize to JSON
                var content = new StringContent(
                    JsonSerializer.Serialize(smsPayload),
                    Encoding.UTF8,
                    "application/json"
                );

                // Send the request
                var response = await _httpClient.PostAsync($"{_baseUrl}/sms/send", content);

                // Check if successful
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"SMS sent successfully: {responseContent}");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to send SMS: {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception when sending SMS: {ex.Message}");
                return false;
            }
        }
    }
}