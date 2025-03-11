using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GolfBookingApp.Data;
using GolfBookingApp.Models;
using System.Threading.Tasks;

namespace GolfBookingApp.Pages.Bookings
{
    public class DetailsModel:PageModel
    {
        private readonly GolfClubContext _context;

        public DetailsModel(GolfClubContext context)
        {
            _context = context;
        }

        public Booking Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get booking with players and their member details
            Booking = await _context.Bookings
                .Include(b => b.Players)
                .ThenInclude(p => p.Member)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Booking == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
