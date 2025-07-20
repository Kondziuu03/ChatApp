using ChatApp.Core.Domain.Dtos;

namespace ChatApp.Core.Domain.Interfaces.Services
{
    public interface IChatService
    {
        Task<ChatDto> GetPaginatedChatAsync(string chatName, int pageNumber, int pageSize);
        Task<string> ParaphraseMessageAsync(string message, string style = "standard");
        Task<string> CheckGrammarAsync(string message);
        Task SaveMessageAsync(MessageDto messageDto);
    }
}
