using ChatApp.Core.Application.SKPlugins;
using ChatApp.Core.Domain.Consts;
using ChatApp.Core.Domain.Dtos;
using ChatApp.Core.Domain.Interfaces.Producer;
using ChatApp.Core.Domain.Interfaces.Repositories;
using ChatApp.Core.Domain.Interfaces.Services;
using ChatApp.Core.Domain.Models;
using Confluent.Kafka;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace ChatApp.Core.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly Kernel _kernel;

        public ChatService(IChatRepository chatRepository, IKafkaProducer kafkaProducer, Kernel kernel)
        {
            _chatRepository = chatRepository;
            _kafkaProducer = kafkaProducer;
            _kernel = kernel;
        }

        public async Task<ChatDto> GetPaginatedChat(string chatName, int pageNumber, int pageSize)
        {
            var chat = await _chatRepository.GetChatWithMessages(chatName, pageNumber, pageSize);

            return ConvertToChatDto(chat);
        }

        public async Task<string> ParaphraseMessage(string message, string style = "standard")
        {
            var arguments = new KernelArguments
            {
                ["input"] = message,
                ["style"] = style,

            };

            arguments.ExecutionSettings = new Dictionary<string, PromptExecutionSettings>
            {
                [$"{nameof(MessagePlugin)}.{nameof(MessagePlugin.ParaphraseMessage)}"] = new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }
            };

            var result = await _kernel.InvokeAsync(nameof(MessagePlugin), nameof(MessagePlugin.ParaphraseMessage), arguments);

            return result.GetValue<string>() ?? string.Empty;
        }

        public async Task<string> CheckGrammar(string message)
        {
            var arguments = new KernelArguments
            {
                ["input"] = message,
            };

            arguments.ExecutionSettings = new Dictionary<string, PromptExecutionSettings>
            {
                [$"{nameof(MessagePlugin)}.{nameof(MessagePlugin.CheckGrammar)}"] = new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }
            };

            var result = await _kernel.InvokeAsync(nameof(MessagePlugin), nameof(MessagePlugin.CheckGrammar), arguments);

            return result.GetValue<string>() ?? string.Empty;
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
            new()
            {
                Id = chat.Id,
                Name = chat.Name,
                Messages = chat.Messages?
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
