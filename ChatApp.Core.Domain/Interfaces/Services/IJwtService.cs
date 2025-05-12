using ChatApp.Core.Domain.Dtos;
using ChatApp.Core.Domain.Models;

namespace ChatApp.Core.Domain.Interfaces.Services
{
    public interface IJwtService
    {
        AuthDto GenerateJwtToken(User user);
    }
}
