namespace ERP_Models.DTOs.ERP_AuthService
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginValidationRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class SaveRefreshTokenRequest
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }

    public class ValidateRefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class TokenRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}