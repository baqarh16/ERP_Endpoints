using System.Data;

namespace ERP_Models.Entities.Data.ERP_Organization
{
    public class User : AuditableEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Foreign keys
        public int? RoleId { get; set; }

        // Navigation
        public virtual Role? Role { get; set; }
    }
}
