using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using REIstacks.Application.Interfaces.IServices; // for ISmsService

namespace AzureFunctions
{
    public class SmsEventGridFunction
    {
        private readonly ILogger<SmsEventGridFunction> _logger;
        private readonly ISmsService _smsService;

        public SmsEventGridFunction(
            ILogger<SmsEventGridFunction> logger,
            ISmsService smsService) // add your service here
        {
            _logger = logger;
            _smsService = smsService;
        }

        [Function("SmsEventGridFunction")]
        public void Run([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            if (eventGridEvent.EventType == "Microsoft.Communication.SMSReceived")
            {
                _logger.LogInformation("Received SMS event!");

                // Optionally parse the data from eventGridEvent.Data to see the 'from', 'to', 'message' etc.
                // var smsData = JsonSerializer.Deserialize<SmsReceivedData>(eventGridEvent.Data.ToString());

                // Then pass it to your application / domain logic, or even auto-reply:
                // await _smsService.SendSmsAsync(smsData.From, "Thanks for your message!");
            }
        }
    }
}
