using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace REIstacks.Api.Controllers.Communications
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsTestController : ControllerBase
    {
        private readonly ISmsService _smsService;
        private readonly ILogger<SmsTestController> _logger;

        public SmsTestController(ISmsService smsService, ILogger<SmsTestController> logger)
        {
            _smsService = smsService;
            _logger = logger;
        }

        // Simple model for the request
        public class SmsRequest
        {
            public string PhoneNumber { get; set; }
            public string Message { get; set; }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendTestSms([FromBody] SmsRequest request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Phone number and message are required");
            }

            _logger.LogInformation($"Attempting to send SMS to {request.PhoneNumber}");

            var result = await _smsService.SendSmsAsync(request.PhoneNumber, request.Message);

            if (result)
            {
                _logger.LogInformation("SMS sent successfully");
                return Ok(new { success = true, message = "SMS sent successfully" });
            }
            else
            {
                _logger.LogError("Failed to send SMS");
                return BadRequest(new { success = false, message = "Failed to send SMS" });
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SmsEventsController : ControllerBase
    {
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var egEvents = JsonSerializer.Deserialize<List<EventGridEvent>>(requestBody, options);

            if (egEvents == null) return BadRequest();

            foreach (var egEvent in egEvents)
            {
                if (egEvent.EventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
                {
                    var data = JsonSerializer.Deserialize<SubscriptionValidationEventData>(egEvent.Data.ToString(), options);
                    var response = new { validationResponse = data.ValidationCode };
                    return new OkObjectResult(response);
                }
                else if (egEvent.EventType == "Microsoft.Communication.SMSReceived")
                {
                    // ...
                }
            }

            return Ok();
        }
    }

    public class EventGridEvent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("eventType")]
        public string EventType { get; set; }
        [JsonPropertyName("data")]
        public object Data { get; set; }
        // other fields omitted for brevity
    }

    public class SubscriptionValidationEventData
    {
        [JsonPropertyName("validationCode")]
        public string ValidationCode { get; set; }

        [JsonPropertyName("validationUrl")]
        public string ValidationUrl { get; set; }
    }
    public class SmsReceivedData
    {
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("to")]
        public string To { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("messageId")]
        public string MessageId { get; set; }
    }



}