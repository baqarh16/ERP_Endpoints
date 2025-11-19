namespace ERP_Models.Entities.Data.ERP_Organization
{
    public class Permission : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Role> Roles { get; set; } = new HashSet<Role>();
    }
}
