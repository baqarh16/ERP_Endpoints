namespace ERP_Models.Entities.Data.ERP_AuthService
{
    public class User : AuditableEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Optional: Add these for better UX
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ProfileImageUrl { get; set; }
    }
}
