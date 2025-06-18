using ChatApp.Core.Domain.Dtos;
using ChatApp.Core.Domain.Interfaces.Repositories;
using ChatApp.Core.Domain.Interfaces.Services;
using ChatApp.Core.Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ChatApp.Core.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(ILogger<AuthService> logger, IUserRepository userRepository, IJwtService jwtService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<AuthDto> GetToken(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsername(loginDto.Username);

            if (user == null)
                throw new InvalidOperationException($"User with username: {loginDto.Username} does not exist");

            if (!VerifyPassword(loginDto.Password, user.Password))
                throw new UnauthorizedAccessException($"Invalid password for user: {loginDto.Username}");

            return _jwtService.GenerateJwtToken(user);
        }

        public async Task RegisterUser(RegisterUserDto registerUserDto)
        {
            var existingUser = await _userRepository.GetByUsername(registerUserDto.Username);

            if (existingUser != null)
            {
                _logger.LogWarning($"User with username: {registerUserDto.Username} already exists");
                throw new InvalidOperationException($"User with username: {registerUserDto.Username} already exists");
            }

            var user = new User(registerUserDto.Username, HashPassword(registerUserDto.Password));

            await _userRepository.Create(user);
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            var parts = storedPassword.Split(':');

            if (parts.Length != 2)
                throw new FormatException("Invalid password format");

            var salt = Convert.FromBase64String(parts[0]);
            var storedHashedPassword = parts[1];

            var enteredHashedPassword = Hash(enteredPassword, salt);

            return enteredHashedPassword == storedHashedPassword;
        }

        private string HashPassword(string password)
        {
            var salt = new byte[128 / 8];

            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            var hashed = Hash(password, salt);

            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        private string Hash(string password, byte[] salt) => 
            Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));
    }
}
