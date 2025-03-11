using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GolfBookingApp.Data;
using GolfBookingApp.Models;

namespace GolfBookingApp.Pages.Bookings
{
    public class DetailsModel : PageModel
    {
        private readonly GolfBookingApp.Data.GolfClubContext _context;

        public DetailsModel(GolfBookingApp.Data.GolfClubContext context)
        {
            _context = context;
        }

        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FirstOrDefaultAsync(m => m.Id == id);

            if (booking is not null)
            {
                Booking = booking;

                return Page();
            }

            return NotFound();
        }
    }
}
