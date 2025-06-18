using ChatApp.Core.Domain.Dtos;
using ChatApp.Core.Domain.Interfaces.Services;
using ChatApp.Core.Domain.Entities;
using ChatApp.Core.Domain.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Core.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettingsOption _jwtSettingsOption;

        public JwtService(IOptions<JwtSettingsOption> jwtSettingsOption)
        {
            _jwtSettingsOption = jwtSettingsOption.Value;
        }

        public AuthDto GenerateJwtToken(User user)
        {
            var expiredDate = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettingsOption.ExpiryInMinutes));

            var token = new JwtSecurityToken(
                claims: GetClaims(user),
                expires: expiredDate,
                signingCredentials: GetSigningCredentials()
                );

            return new AuthDto { 
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiredDate = expiredDate
            };
        }

        private SigningCredentials GetSigningCredentials()
        {
            var byteSecretKey = Encoding.ASCII.GetBytes(_jwtSettingsOption.SecretKey);

            var key = new SymmetricSecurityKey(byteSecretKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return creds;
        }

        private Claim[] GetClaims(User user) =>
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
            ];
    }
}
