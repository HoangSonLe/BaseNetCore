using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class TestKafkaConsumerService : BaseKafkaConsumerBackgroundService
{
    private readonly ILogger<TestKafkaConsumerService> _logger;
    public TestKafkaConsumerService(ILogger<TestKafkaConsumerService> logger, IKafkaConsumerService kafkaConsumerService, IConfiguration configuration) : base(logger, kafkaConsumerService, configuration)
    {
        _logger = logger;
    }
    public override async Task<bool> HandleMessage(string data)
    {
        _logger.LogInformation($"TestKafkaConsumerService received message: {data}");
        Console.WriteLine("TestKafkaConsumerService" + data);
        return true;
    }

}
