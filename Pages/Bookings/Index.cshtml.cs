using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GolfBookingApp.Data;
using GolfBookingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GolfBookingApp.Pages.Bookings
{
    public class IndexModel:PageModel
    {
        private readonly GolfClubContext _context;

        public IndexModel(GolfClubContext context)
        {
            _context = context;
        }

        public IList<Booking> Bookings { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Bookings != null)
            {
                // Include the related Players for each booking
                Bookings = await _context.Bookings
                    .Include(b => b.Players)
                    .ToListAsync();
            }
        }
    }
}
