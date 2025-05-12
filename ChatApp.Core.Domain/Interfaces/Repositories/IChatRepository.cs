using ChatApp.Core.Domain.Models;

namespace ChatApp.Core.Domain.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task<Chat> GetChatWithMessages(string chatName, int pageNumber, int pageSize);
    }
}