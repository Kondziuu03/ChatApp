using ChatApp.Core.Domain.Consts;
using ChatApp.Core.Domain.Interfaces.Producer;
using ChatApp.Core.Domain.Options;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatApp.Infrastructure.Producer
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(ILogger<KafkaProducer> logger, IOptions<KafkaOption> options)
        {
            var config = new ConsumerConfig
            {
                GroupId = GroupKafka.Message,
                BootstrapServers = options.Value.Url,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
            _logger = logger;
        }

        public async Task Produce(string topic, Message<string, string> message)
        {
            try
            {
                await _producer.ProduceAsync(topic, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while producing message on topic: {topic}");
                throw;
            }
        }
    }
}
