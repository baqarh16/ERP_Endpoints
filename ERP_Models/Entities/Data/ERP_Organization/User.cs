using System.ComponentModel.DataAnnotations;

namespace ERP_Models.Entities.Data.ERP_Organization
{
    public class User : AuditableEntity
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ProfileImageUrl { get; set; }

        public int? RoleId { get; set; }
        public virtual Role? Role { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    }
}