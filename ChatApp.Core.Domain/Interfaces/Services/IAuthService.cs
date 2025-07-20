using ChatApp.Core.Domain.Dtos;

namespace ChatApp.Core.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthDto> GetTokenAsync(LoginDto loginDto);
        Task RegisterUserAsync(RegisterUserDto registerUserDto);
    }
}
