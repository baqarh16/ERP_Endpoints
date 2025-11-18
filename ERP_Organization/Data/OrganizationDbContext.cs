// ERP_OrganizationService/Data/OrganizationDbContext.cs
using ERP_Models.Entities;
using ERP_Models.Entities.Data.ERP_Organization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ERP_OrganizationService.Data
{
    public class OrganizationDbContext : DbContext
    {
        public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Soft Delete Global Filter
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

            // Unique Email (only active users)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasFilter("IsDeleted = 0");

            // Configure many-to-many Role ↔ Permission
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity(j => j.ToTable("RolePermissions")); // creates junction table automatically

            // Optional: Seed default data
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Super Admin" },
                new Role { Id = 2, Name = "Admin" },
                new Role { Id = 3, Name = "User" }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "All" }
                // Add more permissions as needed
            );

            modelBuilder.Entity<Role>()
                .HasData(new { Id = 1, Permissions = new[] { new Permission { Id = 1 } } });
        }
    }
}