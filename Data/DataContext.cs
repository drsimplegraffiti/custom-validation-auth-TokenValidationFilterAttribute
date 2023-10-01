using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthFilterProj.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthFilterProj.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }

        public DbSet<Otp> Otps { get; set; }

        public DbSet<Apartment> Apartments { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public object? HttpContext { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apartment>()
                .Property(b => b.Amenities)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .IsRequired(false);

            modelBuilder.Entity<Apartment>()
                .Property(b => b.Rules)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .IsRequired(false);

            modelBuilder.Entity<Apartment>()
                .Property(b => b.ApartmentImages)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .IsRequired(false);

            modelBuilder.Entity<Apartment>()
                .HasOne(a => a.User)  // Fix the typo here
                .WithMany(u => u.Apartments) // Specify the navigation property on the User entity
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Specify ON DELETE NO ACTION

            modelBuilder.Entity<Booking>()
                   .HasOne(b => b.User)
                   .WithMany(u => u.Bookings)
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Restrict); // Specify ON DELETE NO ACTION






            base.OnModelCreating(modelBuilder);

        }
    }
}