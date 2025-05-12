using ChatApp.Core.Domain.Dtos;

namespace ChatApp.Core.Domain.Interfaces.Services
{
    public interface IChatService
    {
        Task<ChatDto> GetPaginatedChat(string chatName, int pageNumber, int pageSize);
        Task SaveMessage(MessageDto messageDto);
    }
}
