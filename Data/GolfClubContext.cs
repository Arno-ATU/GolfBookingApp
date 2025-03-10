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

        // Add this method for Phase 1.3
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

            // Optional: Add seed data if desired
            // modelBuilder.Entity<Member>().HasData(
            //    new Member { Id = 1, MembershipNumber = "M001", Name = "John Smith", ... }
            // );
        }
    }
}
