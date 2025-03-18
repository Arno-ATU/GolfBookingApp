using Microsoft.EntityFrameworkCore;
using GolfBookingApp.Models;

namespace GolfBookingApp.Data
{
    public class GolfClubContext:DbContext
    {
        public GolfClubContext(DbContextOptions<GolfClubContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingPlayer> BookingPlayers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<BookingPlayer>()
                .HasOne(bp => bp.Member)
                .WithMany(m => m.BookingPlayers)
                .HasForeignKey(bp => bp.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingPlayer>()
                .HasOne(bp => bp.Booking)
                .WithMany(b => b.Players)
                .HasForeignKey(bp => bp.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
