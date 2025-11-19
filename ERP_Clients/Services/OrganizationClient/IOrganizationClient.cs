using ERP_Models.DTOs.ERP_AuthService;
using ERP_Models.Entities.Common.Responses;
using ERP_Models.Entities.Data.ERP_Organization;

namespace ERP_Clients.Services.OrganizationClient
{
    public interface IOrganizationClient
    {
        Task<ApiResponse<User>> ValidateCredentialsForLoginAsync(LoginValidationRequest request);
        Task<ApiResponse<User>> ValidateLoginAsync(LoginRequest request);
        Task<ApiResponse<bool>> SaveRefreshTokenAsync(SaveRefreshTokenRequest request);
        Task<ApiResponse<bool>> ValidateRefreshTokenAsync(ValidateRefreshTokenRequest request);
        Task<ApiResponse<User>> GetUserByIdAsync(int userId);
        Task<ApiResponse<List<User>>> GetAllUsersAsync();
    }
}
