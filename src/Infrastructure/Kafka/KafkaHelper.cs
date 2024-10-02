using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Core.CommonModels.BaseModels;
using Core.CoreUtils;

namespace Infrastructure.Kafka
{
    public class KafkaHelper
    {
        private readonly string _bootstrapServers;
        private readonly AdminClientConfig _adminClientConfig;
        public KafkaHelper(string bootstrapServers)
        {
            _bootstrapServers = bootstrapServers;
            _adminClientConfig = new AdminClientConfig()
            {
                BootstrapServers = _bootstrapServers,
            };
        }
        public async Task<AcknowledgementInternal> CreateTopicAsync(string topicName, int numPartitions = 1, short replicationFactor = 1)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = _bootstrapServers,
            };
            using (var adminClient = new AdminClientBuilder(config).Build())
            {
                try
                {
                    var topicSpecifications = new TopicSpecification()
                    {
                        Name = topicName,
                        NumPartitions = numPartitions,
                        ReplicationFactor = replicationFactor
                    };
                    await adminClient.CreateTopicsAsync(new[] { topicSpecifications });
                    Console.WriteLine($"Topic '{topicName}' created successfully.");
                    return new AcknowledgementInternal(true);
                }
                catch (CreateTopicsException e)
                {
                    Console.WriteLine($"An error occurred creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                    return new AcknowledgementInternal(false, e.Results[0].Error.Reason.ToSingleList());
                }
            }
        }
        public async Task<bool> TopicExistAsync(string topicName)
        {
            using(var adminClient = new AdminClientBuilder(_adminClientConfig).Build())
            {
                try
                {
                    var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                    var topicExists = metadata.Topics.Any(t => t.Topic == topicName && !t.Error.IsError);
                    return topicExists;

                }
                catch (KafkaException e) {
                    Console.WriteLine($"Error checking if topic exists: {e.Message}");
                    return false;
                }
            }
        }
    }
}
