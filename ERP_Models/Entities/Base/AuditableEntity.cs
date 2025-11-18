namespace ERP_Models.Entities
{
    public abstract class AuditableEntity : Entity
    {
        public int CreatedBy { get; set; } = 1;                    // 1 = System
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
