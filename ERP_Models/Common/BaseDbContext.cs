using ERP_Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ERP_Models.Common
{
    /// <summary>
    /// Shared base DbContext used across ALL microservices
    /// Features:
    /// - Auto applies all IEntityTypeConfiguration<> from the calling assembly
    /// - Global soft-delete query filter for all BaseEntity descendants
    /// - No need to repeat OnModelCreating in every service
    /// </summary>
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Auto apply all fluent configurations from the CALLING assembly
            //    (not this assembly — this is the key fix!)
            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly()  // This is the service assembly (AuthService, AttendanceService, etc.)
            );

            // 2. Global soft-delete filter for all entities inheriting from BaseEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Build lambda: e => !e.IsDeleted
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Not(
                        Expression.PropertyOrField(parameter, nameof(AuditableEntity.IsDeleted))
                    );
                    var lambda = Expression.Lambda(body, parameter);

                    entityType.SetQueryFilter(lambda);
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}