using IOTAssetTracking.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace IOTAssetTracking.Data
{
 
    // EF Core database context for firmware, devices, and groups.
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        // Tables EF will query/update
        public DbSet<Firmware> firmwares { get; set; }
        public DbSet<Device> devices { get; set; }
        public DbSet<Group> groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Match SQL script table names
            modelBuilder.Entity<Group>().ToTable("Group");
            modelBuilder.Entity<Firmware>().ToTable("Firmware");
            modelBuilder.Entity<Device>().ToTable("Device");

            // Group hierarchy: parent can have many children, block delete if children exist
            modelBuilder.Entity<Group>()
                .HasOne(g => g.ParentGroup)
                .WithMany(g => g.ChildGroups)
                .HasForeignKey(g => g.ParentGroupID)
                .OnDelete(DeleteBehavior.Restrict);

            // Device must keep its firmware, deleting firmware is blocked while devices use it
            modelBuilder.Entity<Device>()
                .HasOne(d => d.Firmware)
                .WithMany(f => f.Devices)
                .HasForeignKey(d => d.FirmwareID)
                .OnDelete(DeleteBehavior.Restrict);

            // Group is optional, removing a group clears Device.GroupID instead of deleting devices
            modelBuilder.Entity<Device>()
                .HasOne(d => d.Group)
                .WithMany(g => g.Devices)
                .HasForeignKey(d => d.GroupID)
                .OnDelete(DeleteBehavior.SetNull);

            // No two devices can share the same serial number
            modelBuilder.Entity<Device>()
                .HasIndex(d => d.SerialNumber)
                .IsUnique();

            // Same firmware name + version cannot be registered twice
            modelBuilder.Entity<Firmware>()
                .HasIndex(f => new { f.FirmwareName, f.Version })
                .IsUnique();
        }
    }
}