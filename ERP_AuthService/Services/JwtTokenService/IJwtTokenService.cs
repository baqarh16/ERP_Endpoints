// ERP_AuthService/Services/JwtTokenService/IJwtTokenService.cs
using ERP_Models.Entities.Data.ERP_AuthService;
using System.Security.Claims;

namespace ERP_AuthService.Services.JwtTokenService
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        // Add these two properties
        int ExpiresInMinutes { get; }
        string Issuer { get; }
        string Audience { get; }
    }
}