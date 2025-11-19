namespace ERP_Models.DTOs.ERP_AuthService
{
    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public AuthSuccessResponse? Data { get; set; }

        // Factory methods
        public static AuthResponse Success(AuthSuccessResponse data, string message = "Success")
            => new() { IsSuccess = true, Message = message, Data = data };

        public static AuthResponse Fail(string message)
            => new() { IsSuccess = false, Message = message, Data = null };
    }

    public class AuthSuccessResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}