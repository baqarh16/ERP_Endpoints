using ERP_Models.Entities;
using ERP_Models.Entities.Data.ERP_AuthService;     // User
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ERP_AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        // For testing/seeding (optional)
        private readonly int? _currentUserId;

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }

        public AuthDbContext(DbContextOptions<AuthDbContext> options, int? currentUserId)
            : base(options)
        {
            _currentUserId = currentUserId;
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===================== GLOBAL SOFT-DELETE FILTER =====================
            // Only apply to entities that inherit from AuditableEntity (not plain Entity)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var filter = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, nameof(AuditableEntity.IsDeleted)),
                            Expression.Constant(false)
                        ),
                        parameter
                    );
                    entityType.SetQueryFilter(filter);
                }
            }

            // ===================== USER CONFIGURATION =====================
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users"); // optional, explicit name

                entity.HasKey(u => u.Id);

                entity.HasIndex(u => u.Email)
                      .IsUnique()
                      .HasFilter("IsDeleted = 0");

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(u => u.FullName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.PhoneNumber)
                      .HasMaxLength(20);

                // These are inherited from AuditableEntity – just ensuring defaults
                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd();

                entity.Property(u => u.IsDeleted)
                      .HasDefaultValue(false);
            });
            // In AuthDbContext.cs → OnModelCreating()
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(t => t.Token).IsUnique();
                entity.HasIndex(t => t.UserId);
            });

            base.OnModelCreating(modelBuilder);
        }

        // ===================== AUTO AUDIT FIELDS =====================
        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var currentUserId = GetCurrentUserId();

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = currentUserId;
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = currentUserId;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; // Soft delete
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedBy = currentUserId;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        break;
                }
            }
        }

        private int GetCurrentUserId()
        {
            // For migrations/seeding
            if (_currentUserId.HasValue)
                return _currentUserId.Value;

            // TODO: Uncomment when you add IHttpContextAccessor to DI
            // var userIdClaim = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // return int.TryParse(userIdClaim, out var id) ? id : 1;

            return 1; // System user
        }
    }
}
