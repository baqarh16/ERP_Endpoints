namespace ERP_Models.Entities.Data.ERP_Organization
{
    public class Role : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
        public virtual ICollection<Permission> Permissions { get; set; } = new HashSet<Permission>();
    }
}
