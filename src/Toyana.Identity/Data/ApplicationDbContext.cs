using Microsoft.EntityFrameworkCore;
using Toyana.Identity.Models;

namespace Toyana.Identity.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ClientUser>          ClientUsers          { get; set; }
    public DbSet<VendorUser>          VendorUsers          { get; set; }
    public DbSet<VendorOrganization>  VendorOrganizations  { get; set; }
    public DbSet<AdminUser>           AdminUsers           { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Client Users
        modelBuilder.Entity<ClientUser>(entity =>
                                        {
                                            entity.HasKey(e => e.Id);
                                            entity.HasIndex(e => e.Username).IsUnique();
                                            entity.HasIndex(e => e.PhoneNumber).IsUnique();
                                        });

        // VendorOrganization
        modelBuilder.Entity<VendorOrganization>(entity =>
                                                {
                                                    entity.HasKey(e => e.Id);
                                                    entity.HasIndex(e => e.TaxId).IsUnique();

                                                    // One-to-one with Owner
                                                    entity.HasOne(e => e.Owner)
                                                          .WithOne()
                                                          .HasForeignKey<VendorOrganization>(e => e.OwnerId)
                                                          .OnDelete(DeleteBehavior.Restrict);

                                                    // One-to-many with Users
                                                    entity.HasMany(e => e.Users)
                                                          .WithOne(u => u.VendorOrganization)
                                                          .HasForeignKey(u => u.VendorOrganizationId)
                                                          .OnDelete(DeleteBehavior.Restrict);
                                                });

        // Vendor Users
        modelBuilder.Entity<VendorUser>(entity =>
                                        {
                                            entity.HasKey(e => e.Id);
                                            entity.HasIndex(e => e.Username).IsUnique();

                                            // Role is stored as int
                                            entity.Property(e => e.Role)
                                                  .HasConversion<int>();
                                        });

        // Admin Users
        modelBuilder.Entity<AdminUser>(entity =>
                                       {
                                           entity.HasKey(e => e.Id);
                                           entity.HasIndex(e => e.Username).IsUnique();
                                       });
    }
}