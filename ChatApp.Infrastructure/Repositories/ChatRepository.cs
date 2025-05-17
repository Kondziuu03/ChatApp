using ChatApp.Core.Domain;
using ChatApp.Core.Domain.Exceptions;
using ChatApp.Core.Domain.Interfaces.Repositories;
using ChatApp.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatDbContext _context;

        public ChatRepository(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> GetChatWithMessages(string chatName, int pageNumber, int pageSize)
        {
            var chat = await GetChat(chatName, pageNumber, pageSize);

            if (chat == null)
                throw new NotFoundException($"Could not find chat with name: {chatName}");

            return chat;
        }

        private async Task<Chat?> GetChat(string chatName, int pageNumber, int pageSize) =>
            await _context.Chats
                .Where(x => x.Name == chatName)
                .Include(x => x.Messages
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize))
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync();
    }
}
