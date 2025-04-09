using Azure;
using Azure.Messaging.EventGrid;
using REIstack.Domain.Models;


namespace REIstacks.Infrastructure.Services
{
    public class EventGridPublisher
    {
        private readonly EventGridPublisherClient _client;

        public EventGridPublisher(string topicEndpoint, string topicKey)
        {
            _client = new EventGridPublisherClient(
                new Uri(topicEndpoint),
                new AzureKeyCredential(topicKey)
            );
        }

        public async Task PublishNewLeadAsync(Lead lead)
        {
            // Construct the event body
            var newEvent = new Azure.Messaging.EventGrid.EventGridEvent(
                subject: $"NewLead/{lead.Id}",
                eventType: "com.reistacks.LeadCreated",
                dataVersion: "1.0",
                data: new
                {
                    LeadId = lead.Id,
                    Email = lead.Email,
                    CreatedAt = lead.CreatedAt
                }
            );

            await _client.SendEventAsync(newEvent);
        }

        // Add more specialized methods, e.g. PublishUserRegisteredAsync, etc.
    }
}
