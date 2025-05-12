using ChatApp.Core.Domain.Consts;
using ChatApp.Core.Domain.Dtos;
using ChatApp.Core.Domain.Interfaces.Producer;
using ChatApp.Core.Domain.Interfaces.Repositories;
using ChatApp.Core.Domain.Interfaces.Services;
using ChatApp.Core.Domain.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatApp.Core.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;
        private readonly IChatRepository _chatRepository;
        private readonly IKafkaProducer _kafkaProducer;

        public ChatService(ILogger<ChatService> logger, IChatRepository chatRepository, IKafkaProducer kafkaProducer)
        {
            _logger = logger;
            _chatRepository = chatRepository;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<ChatDto> GetPaginatedChat(string chatName, int pageNumber, int pageSize)
        {
            var chat = await _chatRepository.GetChatWithMessages(chatName, pageNumber, pageSize);

            return ConvertToChatDto(chat);
        }

        public async Task SaveMessage(MessageDto messageDto)
        {
            await _kafkaProducer.Produce(TopicKafka.Message, new Message<string, string>
            {
                Key = messageDto.Id.ToString(),
                Value = JsonSerializer.Serialize(messageDto)
            });
        } 

        private ChatDto ConvertToChatDto(Chat chat) =>
            new ChatDto
            {
                Id = chat.Id,
                Name = chat.Name,
                Messages = chat.Messages?
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Text = m.Text,
                    CreatedAt = m.CreatedAt,
                    Username = m.User?.Username ?? "Unknown",
                    UserId = m.User?.Id ?? Guid.Empty
                }).ToHashSet() ?? new HashSet<MessageDto>()
            };
    }
}
