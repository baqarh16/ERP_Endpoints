using ERP_Models.Common.Interfaces;
using ERP_Models.DTOs.ERP_Organization;
using ERP_Models.Entities.Common.Responses;
using ERP_Models.Entities.Data.ERP_Organization;

namespace ERP_Organization.Services.UserService
{
    public interface IUserService : IBaseService<User>
    {
        Task<ApiResponse<User>> RegisterAsync(RegisterUserDto dto);
        Task<ApiResponse<User>> GetUserByIdAsync(int id);
        Task<ApiResponse<List<User>>> GetAllUsersAsync();
        Task<ApiResponse<User>> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<ApiResponse<bool>> SoftDeleteUserAsync(int id);

        // Auth-related (used by AuthService)
        Task<ApiResponse<User>> GetUserWithHashByEmailAsync(string email);
        Task<ApiResponse<bool>> SaveRefreshTokenAsync(int userId, string token, string jwtId, DateTime expiry);
        Task<ApiResponse<bool>> ValidateAndUseRefreshTokenAsync(string token);
    }
}
