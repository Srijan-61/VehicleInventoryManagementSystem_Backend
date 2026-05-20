using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Text;
using VehicleInventoryManagementSystem.Domain.Models;

namespace VehicleInventoryManagementSystem.Infrastructure.Presistance
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Profiles
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Staff> StaffProfiles { get; set; }
        public DbSet<Customer> Customers { get; set; }

        // Operational
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PartRequest> PartRequests { get; set; }

        // Inventory & Transactions
        public DbSet<VehiclePart> VehicleParts { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesItem> SalesItems { get; set; }

        public DbSet<Alert> Alerts { get; set; }
        public object AdminProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. Seed Roles
            builder.Entity<Role>().HasData(
                new Role { Id = "1", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "e3c4b406-ae78-4216-a12f-9c82e0449831", Description = "Full System Access" },
                new Role { Id = "2", Name = "Staff", NormalizedName = "STAFF", ConcurrencyStamp = "e3c4b406-ae78-4216-a12f-9c82e0449832", Description = "Inventory & Sales Management" },
                new Role { Id = "3", Name = "Customer", NormalizedName = "CUSTOMER", ConcurrencyStamp = "e3c4b406-ae78-4216-a12f-9c82e0449833", Description = "Vehicle Owner" }
            );

            

            // 2. Configure Composite Primary Keys (UserRole is handled natively now)
            builder.Entity<PurchaseItem>()
                .HasKey(pi => new { pi.Purchase_Invoice_No, pi.Part_ID });

            builder.Entity<SalesItem>()
                .HasKey(si => new { si.Sales_Invoice_No, si.Part_ID });

            // 3. Configure Unique Constraints
            builder.Entity<Vehicle>()
                .HasIndex(v => v.Reg_Number)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<Review>()
            .HasIndex(r => r.Appointment_ID)
            .IsUnique();
        }
    }
}
