using Confluent.Kafka;

namespace ChatApp.Core.Domain.Interfaces.Producer
{
    public interface IKafkaProducer
    {
        Task Produce(string topic, Message<string, string> message);
    }
}
