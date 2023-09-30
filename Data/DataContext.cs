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
        public object? HttpContext { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apartment>()
                .Property(b => b.Amenities)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            modelBuilder.Entity<Apartment>()
                .HasOne(a => a.User)  // Fix the typo here
                .WithMany(u => u.Apartments) // Specify the navigation property on the User entity
                .HasForeignKey(a => a.UserId);

        base.OnModelCreating(modelBuilder);

        }
    }
}