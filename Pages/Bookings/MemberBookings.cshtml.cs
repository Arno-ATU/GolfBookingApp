using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GolfBookingApp.Data;
using GolfBookingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GolfBookingApp.Pages.Bookings
{
    public class MemberBookingsModel:PageModel
    {
        private readonly GolfClubContext _context;
        private readonly ILogger<MemberBookingsModel> _logger;

        public MemberBookingsModel(GolfClubContext context, ILogger<MemberBookingsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Member Member { get; set; }
        public List<Booking> Bookings { get; set; }

        public async Task<IActionResult> OnGetAsync(int? memberId)
        {
            if (memberId == null)
            {
                return NotFound();
            }

            // Get the member details
            Member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId);

            if (Member == null)
            {
                return NotFound();
            }

            // Get all bookings for this member
            var bookingIds = await _context.BookingPlayers
                .Where(bp => bp.MemberId == memberId)
                .Select(bp => bp.BookingId)
                .ToListAsync();

            // Get the full booking details including other players
            Bookings = await _context.Bookings
                .Where(b => bookingIds.Contains(b.Id))
                .Include(b => b.Players)
                .ThenInclude(p => p.Member)
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.TeeTime)
                .ToListAsync();

            _logger.LogInformation($"Found {Bookings.Count} bookings for member {Member.Name}");

            return Page();
        }
    }
}
