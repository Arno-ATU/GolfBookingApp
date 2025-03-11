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
    public class DeleteModel : PageModel
    {
        private readonly GolfBookingApp.Data.GolfClubContext _context;

        public DeleteModel(GolfBookingApp.Data.GolfClubContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                Booking = booking;
                _context.Bookings.Remove(Booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
