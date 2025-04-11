using Azure.Communication.Sms;
using Microsoft.Extensions.Configuration;
using REIstacks.Application.Interfaces.IServices;

namespace REIstacks.Infrastructure.Services.Communications;
public class AzureSmsService : ISmsService
{
    private readonly SmsClient _smsClient;
    private readonly string _fromNumber; // The number you've configured in ACS

    public AzureSmsService(IConfiguration configuration)
    {
        // 1) Retrieve connection string & “From” number from configuration
        var connectionString = configuration["AzureCommunicationServices:ConnectionString"];
        _fromNumber = configuration["AzureCommunicationServices:FromPhoneNumber"];

        // 2) Initialize the ACS SmsClient
        _smsClient = new SmsClient(connectionString);
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            // 3) Send via ACS
            var response = await _smsClient.SendAsync(
                from: _fromNumber,
                to: phoneNumber,
                message: message
            );

            // 4) You can check the status or handle exceptions
            if (response.Value?.MessageId != null)
            {
                // Succeeded
                Console.WriteLine($"SMS sent successfully. MessageId: {response.Value.MessageId}");
                return true;
            }
            else
            {
                // Something unexpected happened
                Console.WriteLine("SMS sending returned a null MessageId. Possibly failed.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception sending SMS: {ex.Message}");
            return false;
        }
    }
}