using Application.Base;
using Application.Services.WebInterfaces;
using Core.CommonModels.BaseModels;
using Core.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class KafkaController : BaseController<KafkaController>
    {
        private readonly KafkaProducerService _producerService;
        private readonly IKafkaConsumerService _consumerService;

        public KafkaController(ILogger<KafkaController> logger, IUserService userService, KafkaProducerService producerService, IKafkaConsumerService consumerService) : base(logger, userService)
        {
            _producerService = producerService;
            _consumerService = consumerService;
        }

        [HttpPost("produce")]
        public async Task<Acknowledgement> ProduceMessage(string topicName, string message)
        {
            var ack = await _producerService.SendMessageAsync(topicName ?? "test-topic", message);
            return ack.AsAcknowledgement();
        }

        [HttpGet("consume")]
        public async Task<IActionResult> ConsumeMessages(string topicName)
        {
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    await _consumerService.ConsumeMessages(topicName, cts.Token, null);
                    return Ok("Messages consumed successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
    }
}
