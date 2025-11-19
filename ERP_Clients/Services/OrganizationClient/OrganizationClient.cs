using ERP_Clients.Services.Base;
using ERP_Models.DTOs.ERP_AuthService;
using ERP_Models.Entities.Common.Responses;
using ERP_Models.Entities.Data.ERP_Organization;

namespace ERP_Clients.Services.OrganizationClient
{
    public class OrganizationClient : BaseHttpClient, IOrganizationClient
    {
        public OrganizationClient(HttpClient httpClient) : base(httpClient) { }

        public Task<ApiResponse<User>> ValidateCredentialsForLoginAsync(LoginValidationRequest request)
            => PostAsync<User>("api/organization/auth/validate-credentials", request);

        public Task<ApiResponse<User>> ValidateLoginAsync(LoginRequest request)
            => PostAsync<User>("api/organization/auth/login", request);

        public Task<ApiResponse<bool>> SaveRefreshTokenAsync(SaveRefreshTokenRequest request)
            => PostAsync<bool>("api/organization/auth/refresh-token", request);

        public Task<ApiResponse<bool>> ValidateRefreshTokenAsync(ValidateRefreshTokenRequest request)
            => PostAsync<bool>("api/organization/auth/refresh-token/validate", request);

        public Task<ApiResponse<User>> GetUserByIdAsync(int userId)
                => GetAsync<User>($"api/organization/users/{userId}");

        public Task<ApiResponse<List<User>>> GetAllUsersAsync()
            => GetAsync<List<User>>("api/organization/users");
    }
}
