using Confluent.Kafka;
using Core.CommonModels.BaseModels;
using Core.CoreUtils;
using Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class KafkaProducerService
{
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly ProducerConfig _producerConfig;
    protected readonly IConfiguration _configuration;
    private readonly KafkaHelper _kafkaHelper;


    public KafkaProducerService(ILogger<KafkaProducerService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        _producerConfig = new ProducerConfig
        {
            BootstrapServers = _configuration.GetSection("KafkaConfig:BootstrapServers").Value//"localhost:9092"  // Kafka broker address
        };
        _kafkaHelper = new KafkaHelper(_producerConfig.BootstrapServers);
    }

    public async Task<AcknowledgementInternal> SendMessageAsync(string topic, string message)
    {
        using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
        {
            try
            {
                if (string.IsNullOrWhiteSpace(topic)) { 
                    topic = _configuration.GetSection("KafkaConfig:Topic").Value;
                }
                try
                {
                    var isExistTopic = await _kafkaHelper.TopicExistAsync(topic);
                    if (!isExistTopic)
                    {
                        var ackCreateTopic = await _kafkaHelper.CreateTopicAsync(topic);
                        if (!ackCreateTopic.IsSuccess)
                        {
                            _logger.LogError("Error CreateTopicAsync: " + ackCreateTopic.MessageList.ToStringList());
                            return ackCreateTopic;
                        }
                    }
                    var deliveryReport = await producer.ProduceAsync(topic, new Message<Null, string>
                    {
                        Value = message,
                    });

                    Console.WriteLine($"Delivered '{deliveryReport.Value}' to '{deliveryReport.TopicPartitionOffset}'");
                    return new AcknowledgementInternal(true);
                }
                catch (ProduceException<Null, string> e)
                {
                    _logger.LogError("Error ProduceAsync: " + e.Error.Reason);
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    return new AcknowledgementInternal(false,e.Error.Reason.ToSingleList());
                }
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError("Error ProduceAsync: " + ex.Error.Reason);
                _logger.LogError($"Error sending message: {ex.Error.Reason}");
                return new AcknowledgementInternal(false,ex.Error.Reason.ToSingleList());
            }
        }
    }
}
