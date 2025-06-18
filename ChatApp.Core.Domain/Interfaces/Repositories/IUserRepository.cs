using ChatApp.Core.Domain.Entities;

namespace ChatApp.Core.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> Get(Guid id);
        Task<User?> GetByUsername(string username);
        Task Create(User user);
    }
}
