using Microsoft.EntityFrameworkCore;
using Toyana.Identity.Models;

namespace Toyana.Identity.Data;

public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {
            }

        public DbSet<ClientUser>           ClientUsers           { get; set; }
        public DbSet<VendorUser>           VendorUsers           { get; set; }
        public DbSet<VendorRole>           VendorRoles           { get; set; }
        public DbSet<VendorUserRole>       VendorUserRoles       { get; set; }
        public DbSet<VendorUserPermission> VendorUserPermissions { get; set; }
        public DbSet<AdminUser>            AdminUsers            { get; set; }
        public DbSet<Permission>           Permissions           { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Client Users
                modelBuilder.Entity<ClientUser>(entity =>
                                                    {
                                                        entity.HasKey(e => e.Id);
                                                        entity.HasIndex(e => e.Username).IsUnique();
                                                        entity.HasIndex(e => e.PhoneNumber).IsUnique();
                                                    });

                // Vendor Users
                modelBuilder.Entity<VendorUser>(entity =>
                                                    {
                                                        entity.HasKey(e => e.Id);
                                                        entity.HasIndex(e => e.Username)
                                                              .IsUnique(); // Global uniqueness for login simplicity, or scoped to Vendor? Usually global username/email is best.
                                                    });

                // Vendor Roles
                modelBuilder.Entity<VendorRole>(entity =>
                                                    {
                                                        entity.HasKey(e => e.Id);
                                                        // Permissions stored as CSV or JSON if provider supports it.
                                                        // For simplicity in Postgres standard, let's just stick to default or primitive collection if EF8 supports it.
                                                    });

                // Join: VendorUserRole
                modelBuilder.Entity<VendorUserRole>(entity =>
                                                        {
                                                            entity.HasKey(e => new { e.VendorUserId, e.VendorRoleId });
                                                            entity.HasOne(d => d.VendorUser)
                                                                  .WithMany(p => p.Roles)
                                                                  .HasForeignKey(d => d.VendorUserId);
                                                        });

                // VendorUserPermission
                modelBuilder.Entity<VendorUserPermission>(entity =>
                                                              {
                                                                  entity.HasOne(d => d.VendorUser)
                                                                        .WithMany(p => p.Permissions)
                                                                        .HasForeignKey(d => d.VendorUserId);
                                                              });

                // Admin Users
                modelBuilder.Entity<AdminUser>(entity =>
                                                   {
                                                       entity.HasKey(e => e.Id);
                                                       entity.HasIndex(e => e.Username).IsUnique();
                                                   });
            }
    }