using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace BlockchainAppAPI
{
    public static class IHttpContextAccessorExtension
    {
        public static string CurrentUser(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor?.HttpContext?.User?.FindFirst("UserId")?.Value;
        }
    }
}