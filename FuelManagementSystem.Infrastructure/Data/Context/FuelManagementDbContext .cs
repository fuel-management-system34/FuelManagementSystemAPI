using FuelManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Infrastructure.Data.Context
{
    public class FuelManagementDbContext : DbContext
    {
        public FuelManagementDbContext(DbContextOptions<FuelManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<FuelStation> FuelStations { get; set; }
        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<FuelTank> FuelTanks { get; set; }
        public DbSet<FuelPump> FuelPumps { get; set; }
        public DbSet<FuelPrice> FuelPrices { get; set; }
        public DbSet<FuelDelivery> FuelDeliveries { get; set; }
        public DbSet<FuelSale> FuelSales { get; set; }
        public DbSet<Customer> Customers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                //entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.ProfilePicture).HasMaxLength(255);
                entity.Property(e => e.PreferredLanguage).HasMaxLength(10).HasDefaultValue("en");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(255);
            });

            // UserRole configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");
                entity.HasKey(e => e.UserRoleId);
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            });

            // UserActivityLog configuration
            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.ToTable("UserActivityLog");
                entity.HasKey(e => e.LogId);
                entity.Property(e => e.ActivityType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.IPAddress).HasMaxLength(50);

                entity.HasOne(ual => ual.User)
                    .WithMany(u => u.ActivityLogs)
                    .HasForeignKey(ual => ual.UserId);
            });

            // Seed initial roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Admin", Description = "System Administrator with full access", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Role { RoleId = 2, Name = "StationManager", Description = "Manages station operations", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Role { RoleId = 3, Name = "Cashier", Description = "Handles payments and transactions", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Role { RoleId = 4, Name = "PumpAttendant", Description = "Operates fuel pumps", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Role { RoleId = 5, Name = "InventoryManager", Description = "Manages fuel inventory", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            );
        }
    }
}
