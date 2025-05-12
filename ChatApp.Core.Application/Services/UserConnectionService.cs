using System.Collections.Concurrent;
using System.Security.Claims;

namespace ChatApp.Core.Application.Services
{
    public class UserConnectionService
    {
        private readonly ConcurrentDictionary<string, string> _userConnections = new();

        public void AddConnection(string username, string connectionId)
        {
            if (!string.IsNullOrEmpty(username))
                _userConnections[username] = connectionId;
        }

        public void RemoveConnection(string username)
        {
            if (!string.IsNullOrEmpty(username))
                _userConnections.TryRemove(username, out _);
        }
    }
}
