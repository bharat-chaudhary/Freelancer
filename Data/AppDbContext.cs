using Freelancer.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelancer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Freelancers> Freelancers { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Audit> Audits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Project Configuration
            modelBuilder.Entity<Project>()
                .Property(p => p.Bid_Date)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Project>()
                .Property(p => p.Budget)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bid Configuration
            modelBuilder.Entity<Bid>()
                .Property(b => b.BidAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Project)
                .WithMany(p => p.Bids)
                .HasForeignKey(b => b.Project_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment Configuration
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Payer)
                .WithMany(u => u.PaymentsMade)
                .HasForeignKey(p => p.PayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Payee)
                .WithMany(u => u.PaymentsReceived)
                .HasForeignKey(p => p.PayeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Explicitly define precision and scale for HourlyRate in User model
            modelBuilder.Entity<User>()
                .Property(u => u.HourlyRate)
                .HasColumnType("decimal(18,2)"); // Precision 18, scale 2

            // Seed Admin User
            modelBuilder.Entity<User>().HasData(new User
            {
                UserId = 1,
                Username = "admin",
                Password = "Admin@123", // Make sure to hash passwords in a real application
                Email = "admin@example.com",
                Role = "Admin",
                FullName = "Admin User"
            });
        }
    }
}
