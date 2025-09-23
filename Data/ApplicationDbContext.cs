using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShowtimeWebApplication.Models;

namespace ShowtimeWebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public IEnumerable<object> ApplicationUsers { get; internal set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure decimal precision for Price properties
            builder.Entity<Movie>()
                .Property(m => m.Price)
                .HasPrecision(18, 2); // 18 total digits, 2 decimal places

            builder.Entity<Booking>()
                .Property(b => b.Price)
                .HasPrecision(18, 2); // 18 total digits, 2 decimal places

            // Configure relationships
            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Booking>()
                .HasOne(b => b.Movie)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
