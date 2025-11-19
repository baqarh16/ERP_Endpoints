using ERP_Models.Data;
using ERP_Models.Entities.Data.ERP_Organization;
using Microsoft.EntityFrameworkCore;

namespace ERP_OrganizationService.Data
{
    public class OrganizationDbContext : BaseDbContext
    {
        public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Applies soft-delete + audit

            // Unique Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasFilter("IsDeleted = 0");

            // Role ↔ Permission Many-to-Many (Correct way)
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermissions",
                    j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionsId"),
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RolesId"),
                    j =>
                    {
                        j.HasKey("RolesId", "PermissionsId"); // This is the fix!
                        j.HasData(new { RolesId = 1, PermissionsId = 1 }); // Super Admin has All
                    });

            // RefreshToken → User
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Roles & Permissions
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Super Admin" },
                new Role { Id = 2, Name = "Admin" },
                new Role { Id = 3, Name = "User" }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "All" }
            );
        }
    }
}