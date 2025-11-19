namespace ERP_Models.Entities.Data.ERP_Organization
{
    public class RefreshToken : AuditableEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
    }
}
