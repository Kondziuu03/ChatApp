using ChatApp.Core.Domain;
using ChatApp.Core.Domain.Exceptions;
using ChatApp.Core.Domain.Interfaces.Repositories;
using ChatApp.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ILogger<ChatRepository> _logger;
        private readonly ChatDbContext _context;

        public ChatRepository(ILogger<ChatRepository> logger, ChatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Chat> GetChatWithMessages(string chatName, int pageNumber, int pageSize)
        {
            try
            {
                var chat = await GetChat(chatName, pageNumber, pageSize);

                if (chat == null)
                    throw new ChatNotFoundException(chatName);

                return chat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting chat with name: {chatName}");
                throw;
            }
        }

        private async Task<Chat?> GetChat(string chatName, int pageNumber, int pageSize) => 
            await _context.Chats
                .Where(x => x.Name == chatName)
                .Include(x => x.Messages
                    .OrderBy(x => x.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize))
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync();
    }
}
