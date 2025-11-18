// ERP_Models/Entities/Data/ERP_AuthService/RefreshToken.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_Models.Entities.Data.ERP_AuthService
{
    public class RefreshToken : AuditableEntity
    {
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        public string JwtId { get; set; } = string.Empty; // Jti from access token

        public DateTime ExpiryDate { get; set; }

        public bool IsUsed { get; set; } = false;
        public bool IsRevoked { get; set; } = false;

        // Navigation
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}