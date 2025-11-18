// Services/AuthService.cs
using ERP_AuthService.Data;
using ERP_AuthService.Services.JwtTokenService;
using ERP_Models.DTOs.ERP_AuthService;
using ERP_Models.Entities.Data.ERP_AuthService;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ERP_AuthService.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _db;
        private readonly IJwtTokenService _jwt;

        public AuthService(AuthDbContext db, IJwtTokenService jwt)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // 1. Email already exists?
                var exists = await _db.Users
                    .AnyAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower());

                if (exists)
                    return AuthResponse.Fail("Email already exists.");

                // 2. Create user
                var user = new User
                {
                    FullName = request.FullName.Trim(),
                    Email = request.Email.Trim().ToLower(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    IsActive = true
                    // CreatedBy, CreatedAt → auto set by BaseDbContext
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                // 3. Generate JWT
                var accessToken = _jwt.GenerateToken(user);
                var refreshToken = _jwt.GenerateRefreshToken();

                var tokenEntity = new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    JwtId = new JwtSecurityToken(accessToken).Id,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    IsUsed = false,
                    IsRevoked = false
                };

                _db.RefreshTokens.Add(tokenEntity);
                await _db.SaveChangesAsync();

                return AuthResponse.Success(new AuthSuccessResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiresInMinutes),
                    FullName = user.FullName,
                    Email = user.Email
                });
            }
            catch (Exception ex)
            {
                return AuthResponse.Fail(ex.Message);
            }
        }
        public async Task<AuthResponse> RefreshTokenAsync(TokenRequest request)
        {
            try
            {
                var principal = _jwt.GetPrincipalFromExpiredToken(request.AccessToken);
                var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.NameId)?.Value
                                 ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out int userId))
                    return AuthResponse.Fail("Invalid token subject.");

                var user = await _db.Users.FindAsync(userId);
                if (user == null || !user.IsActive || user.IsDeleted)
                    return AuthResponse.Fail("User not found or inactive.");

                var savedRefreshToken = await _db.RefreshTokens
                    .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && t.UserId == userId);

                if (savedRefreshToken == null || savedRefreshToken.IsRevoked || savedRefreshToken.IsUsed ||
                    savedRefreshToken.ExpiryDate <= DateTime.UtcNow)
                    return AuthResponse.Fail("Invalid or expired refresh token.");

                // Mark old refresh token as used
                savedRefreshToken.IsUsed = true;
                await _db.SaveChangesAsync();

                // Generate new tokens
                var newAccessToken = _jwt.GenerateToken(user);
                var newRefreshToken = _jwt.GenerateRefreshToken();

                var newRefreshEntity = new RefreshToken
                {
                    UserId = user.Id,
                    Token = newRefreshToken,
                    JwtId = new JwtSecurityToken(newAccessToken).Id,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    IsUsed = false,
                    IsRevoked = false
                };

                _db.RefreshTokens.Add(newRefreshEntity);
                await _db.SaveChangesAsync();

                return AuthResponse.Success(new AuthSuccessResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiresInMinutes),
                    FullName = user.FullName,
                    Email = user.Email
                });
            }
            catch (Exception)
            {
                return AuthResponse.Fail("Invalid token.");
            }
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                // 1. Find user (case-insensitive + not deleted)
                var user = await _db.Users
                    .FirstOrDefaultAsync(u =>
                        u.Email.ToLower() == request.Email.Trim().ToLower() &&
                        !u.IsDeleted);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return AuthResponse.Fail("Invalid email or password.");

                if (!user.IsActive)
                    return AuthResponse.Fail("Account is deactivated.");

                // 2. Generate token
                var accessToken = _jwt.GenerateToken(user);

                return AuthResponse.Success(new AuthSuccessResponse
                {
                    AccessToken = accessToken,
                    FullName = user.FullName,
                    Email = user.Email
                });
            }
            catch (Exception ex)
            {
                return AuthResponse.Fail(ex.Message);
            }
        }
    }
}