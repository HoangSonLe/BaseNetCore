using Confluent.Kafka;
using Core.Enums;
using Infrastructure.DBContexts;
using Infrastructure.Entitites;
using Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


public interface IKafkaConsumerService
{
    public Task ConsumeMessages(string topic, CancellationToken cancellationToken, Func<string, Task<bool>> handleMessage);
}
public abstract class BaseKafkaConsumerBackgroundService : BackgroundService
{
    private readonly IKafkaConsumerService _kafkaConsumerService;
    protected readonly IConfiguration _configuration;
    private readonly ILogger<BaseKafkaConsumerBackgroundService> _logger;
    public BaseKafkaConsumerBackgroundService(ILogger<BaseKafkaConsumerBackgroundService> logger, IKafkaConsumerService kafkaConsumerService, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _kafkaConsumerService = kafkaConsumerService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BaseKafkaConsumerBackgroundService is starting.");

        stoppingToken.Register(() => _logger.LogInformation("BaseKafkaConsumerBackgroundService is stopping."));

        var topic = _configuration.GetSection("KafkaConfig:Topic").Value;
        if (!string.IsNullOrWhiteSpace(topic))
        {
            _logger.LogInformation($"Consuming messages from topic: {topic}");
            await _kafkaConsumerService.ConsumeMessages(topic, stoppingToken, HandleMessage);
        }

        _logger.LogInformation("BaseKafkaConsumerBackgroundService has stopped.");
    }
    public abstract Task<bool> HandleMessage(string data);
}

public class BaseKafkaConsumerService : IKafkaConsumerService
{
    private readonly ILogger<BaseKafkaConsumerService> _logger;
    private readonly ConsumerConfig _consumerConfig;
    protected readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private SampleDBContext _myDatabaseContext;
    private readonly KafkaHelper _kafkaHelper;
    public BaseKafkaConsumerService(ILogger<BaseKafkaConsumerService> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _configuration.GetSection("KafkaConfig:BootstrapServers").Value,// "localhost:9092",  // Kafka broker address
            GroupId = _configuration.GetSection("KafkaConfig:GroupId").Value,// "test-consumer-group",  // Consumer group ID
            AutoOffsetReset = AutoOffsetReset.Earliest,  // Start reading from the earliest message 
            EnableAutoCommit = false,
            MaxPollIntervalMs = 30000, // 5 minutes
            SessionTimeoutMs = 10000,   // 10 seconds
        };
        _kafkaHelper = new KafkaHelper(_consumerConfig.BootstrapServers);
    }

    public async Task ConsumeMessages(string topic, CancellationToken cancellationToken, Func<string, Task<bool>>? handleMessage = null)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var consumer = new ConsumerBuilder<Null, string>(_consumerConfig).Build())
                {
                    //Đăng ký consumer xử lý topic cụ thể này
                    //Subscribe chỉ giữ lại lần đăng ký cuối cùng: Mỗi lần bạn gọi Subscribe, Kafka sẽ bỏ qua các topic đã đăng ký trước đó và chỉ giữ lại các topic từ lần gọi Subscribe cuối cùng
                    consumer.Subscribe(topic);

                    var kafkaHelper = new KafkaHelper(_consumerConfig.BootstrapServers);
                    var isExistTopic = await kafkaHelper.TopicExistAsync(topic);
                    if (!isExistTopic)
                    {
                        try
                        {
                            await kafkaHelper.CreateTopicAsync(topic);
                        }
                        catch (Confluent.Kafka.KafkaException ex)
                        {
                            _logger.LogError($"KafkaException while creating topic: {ex.Message}");
                            continue; // Skip to the next iteration
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Unexpected error while creating topic: {ex.Message}");
                            continue; // Skip to the next iteration
                        }
                    }
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(10)); // Set a 10-second timeout
                    if (consumeResult != null)
                    {
                        _logger.LogInformation($"Message '{consumeResult.Message.Value}' received from topic '{topic}'");

                        // Process message and save status to database
                        bool isProcessed = await ProcessMessage(consumeResult, handleMessage);
                        // Commit offset after processing message
                        if (isProcessed)
                        {
                            try
                            {
                                consumer.Commit(consumeResult);
                            }
                            catch (Confluent.Kafka.KafkaException ex)
                            {
                                _logger.LogError($"KafkaException while committing offset: {ex.Message}");
                                // Handle specific KafkaException if needed
                            }
                        }
                    }
                    await Task.Delay(100, cancellationToken);  // Delay nhỏ để không gây overload
                }
            }
        }
        catch (ConsumeException ex)
        {
            _logger.LogError($"Error consuming message: {ex.Error.Reason}");
        }
    }
    private async Task<bool> ProcessMessage(ConsumeResult<Null, string> consumeResult, Func<string, Task<bool>>? handleMessage)
    {
        var data = consumeResult.Message.Value;
        // Logic xử lý message (ví dụ lưu vào database)
        var kafkaMessage = new KafkaMessage()
        {
            MessageData = data,
            MessageId = Guid.NewGuid().ToString(),
            Offset = consumeResult.Offset,
            Partition = consumeResult.Partition,
            Status = EKafkaMessageStatus.Pending,
            Timestamp = DateTime.UtcNow
        };
        await SaveChangeMessage(kafkaMessage);
        Console.WriteLine($"Processing message: {data}");
        if (handleMessage != null)
        {
            var ack = await handleMessage(data);
            if (ack)
            {
                kafkaMessage.Status = EKafkaMessageStatus.Processed;
                await SaveChangeMessage(kafkaMessage);
            }
            return ack;  // Trả về true nếu xử lý thành công
        }
        return true;

    }
    protected async Task<int> SaveChangeMessage(KafkaMessage message)
    {

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SampleDBContext>();
            var kafkaExist = await dbContext.KafkaMessages.FirstOrDefaultAsync(i => i.MessageId == message.MessageId);
            if (kafkaExist != null)
            {
                kafkaExist.CopyData(message);
            }
            else
            {
                await dbContext.AddAsync(message);
            }
            return await dbContext.SaveChangesAsync();
        }
    }
}
