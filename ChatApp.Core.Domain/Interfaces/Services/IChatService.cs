using ChatApp.Core.Domain.Dtos;

namespace ChatApp.Core.Domain.Interfaces.Services
{
    public interface IChatService
    {
        Task<ChatDto> GetPaginatedChat(string chatName, int pageNumber, int pageSize);
        Task<string> ParaphraseMessage(string message, string style = "standard");
        Task<string> CheckGrammar(string message);
        Task SaveMessage(MessageDto messageDto);
    }
}
