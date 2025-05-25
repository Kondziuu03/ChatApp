
using ChatApp.Core.Domain;
using ChatApp.Core.Domain.Consts;
using ChatApp.Core.Domain.Dtos;
using ChatApp.Core.Domain.Models;
using ChatApp.Core.Domain.Options;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ChatApp.MessageBroker
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly KafkaOption _kafkaOption;
        private readonly IDbContextFactory<ChatDbContext> _dbContextFactory;

        public KafkaConsumer(ILogger<KafkaConsumer> logger, IOptions<KafkaOption> options, IDbContextFactory<ChatDbContext> dbContextFactory)
        {
            _logger = logger;
            _kafkaOption = options.Value;
            _dbContextFactory = dbContextFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Consume(TopicKafka.Message, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming messages");
                throw;
            }
        }

        private async Task Consume(string topic, CancellationToken stoppingToken)
        {
            var config = CrateConsumerConfig();

            using var consumer = new ConsumerBuilder<string, string>(config).Build();

            consumer.Subscribe(topic);
            _logger.LogInformation($"Subscribed to topic: {topic}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    await ProcessMessage(consumeResult.Message.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred while processing the message");
                    await Task.Delay(1000, stoppingToken);
                }
            }
            ;
        }

        private async Task ProcessMessage(string value)
        {
            var messageDto = JsonSerializer.Deserialize<MessageDto>(value);

            if (messageDto == null)
                throw new Exception("Could not deserialize message");

            var message = CreateMessage(messageDto);

            await SaveMessage(message);
        }

        private async Task SaveMessage(Message message)
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();

            await dbContext.Messages.AddAsync(message);
            await dbContext.SaveChangesAsync();

            _logger.LogInformation($"Message with id: {message.Id} saved successfully");
        }

        private Message CreateMessage(MessageDto messageDto) =>
            new()
            {
                Id = messageDto.Id,
                UserId = messageDto.UserId,
                CreatedAt = messageDto.CreatedAt.ToUniversalTime(),
                ChatId = messageDto.ChatId,
                Text = messageDto.Text
            };

        private ConsumerConfig CrateConsumerConfig() =>
            new()
            {
                GroupId = GroupKafka.Message,
                BootstrapServers = _kafkaOption.Url,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
    }
}
