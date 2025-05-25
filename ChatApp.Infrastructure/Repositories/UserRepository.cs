using ChatApp.Core.Domain;
using ChatApp.Core.Domain.Interfaces.Repositories;
using ChatApp.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly ChatDbContext _context;

        public UserRepository(ILogger<UserRepository> logger, ChatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<User?> Get(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                _logger.LogWarning($"User with id: {id} not found");

            return user;
        }

        public async Task<User?> GetByUsername(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Username == username);

            if (user == null)
                _logger.LogWarning($"User with username: {username} not found");

            return user;
        }

        public async Task Create(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
