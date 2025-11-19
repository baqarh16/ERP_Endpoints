using ERP_Models.DTOs.ERP_AuthService;

namespace ERP_AuthService.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}