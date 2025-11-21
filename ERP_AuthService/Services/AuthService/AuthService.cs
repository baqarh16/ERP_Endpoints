// Services/AuthService.cs
using ERP_AuthService.Services.JwtTokenService;
using ERP_Clients.Services.OrganizationClient;
using ERP_Models.DTOs.ERP_AuthService;

namespace ERP_AuthService.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IOrganizationClient _orgClient;
        private readonly IJwtTokenService _jwt;

        public AuthService(IOrganizationClient orgClient, IJwtTokenService jwt)
        {
            _orgClient = orgClient;
            _jwt = jwt;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                // 1. Call OrganizationService to get user (public endpoint)
                var validationResult = await _orgClient.ValidateCredentialsForLoginAsync(new LoginValidationRequest { Email = request.Email });

                if (!validationResult.Success || validationResult.Data == null)
                    return AuthResponse.Fail("Invalid email.");

                var user = validationResult.Data;

                // 2. Validate password in AuthService (secure — hash never leaves Organization, but we verify here)
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return AuthResponse.Fail("Invalid password.");

                // 3. Generate tokens
                var accessToken = _jwt.GenerateAccessToken(user);
                var refreshToken = _jwt.GenerateRefreshToken();

                // 4. Save refresh token (call OrganizationService)
                var saveResult = await _orgClient.SaveRefreshTokenAsync(new SaveRefreshTokenRequest
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    JwtId = _jwt.GetJti(accessToken),
                    ExpiryDate = DateTime.UtcNow.AddDays(7)
                });

                if (!saveResult.Success)
                    return AuthResponse.Fail("Failed to save refresh token.");

                return AuthResponse.Success(new AuthSuccessResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiresInMinutes),
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role?.Name ?? "User"
                });
            }
            catch (Exception ex)
            {
                return AuthResponse.Fail(ex.Message);
            }
        }
    }
}