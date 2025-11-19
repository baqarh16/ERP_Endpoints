// ERP_AuthService/Services/JwtTokenService/IJwtTokenService.cs
using ERP_Models.Entities.Data.ERP_Organization;
using System.Security.Claims;

namespace ERP_AuthService.Services.JwtTokenService
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        string GetJti(string accessToken);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        // Add these two properties
        int ExpiresInMinutes { get; }
        string Issuer { get; }
        string Audience { get; }
    }
}