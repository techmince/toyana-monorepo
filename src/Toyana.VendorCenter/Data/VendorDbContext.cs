using Microsoft.EntityFrameworkCore;
using Toyana.VendorCenter.Models;

namespace Toyana.VendorCenter.Data;

public class VendorDbContext : DbContext
{
    public VendorDbContext(DbContextOptions<VendorDbContext> options) : base(options) { }

    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Availability> AvailabilitySlots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TaxId).IsUnique();
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(d => d.Vendor)
                .WithMany(p => p.Services)
                .HasForeignKey(d => d.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.VendorId, e.Date }).IsUnique(); // One status per date per vendor
            entity.HasOne(d => d.Vendor)
                .WithMany(p => p.AvailabilitySlots)
                .HasForeignKey(d => d.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
