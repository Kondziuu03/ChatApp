using System.Security.Claims;

namespace ChatApp.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetClaimValue(this ClaimsPrincipal? claims, string claimType)
        {
            if (claims == null)
                throw new ArgumentNullException("Empty claims");

            var claimValue = claims.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

            if (string.IsNullOrEmpty(claimValue))
                throw new ArgumentException($"Empty claim {claimType}", claimValue);

            return claimValue;
        }
    }
}
