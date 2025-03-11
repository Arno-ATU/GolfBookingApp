using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GolfBookingApp.Data;
using GolfBookingApp.Models;
using System.Threading.Tasks;

namespace GolfBookingApp.Pages.Bookings
{
    public class DeleteModel:PageModel
    {
        private readonly GolfClubContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(GolfClubContext context, ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Booking = await _context.Bookings.FindAsync(id);

            if (Booking != null)
            {
                _logger.LogInformation($"Deleting booking ID: {Booking.Id}");

                // All BookingPlayer entries will be automatically deleted due to 
                // the Cascade DeleteBehavior set in the OnModelCreating method

                _context.Bookings.Remove(Booking);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Booking ID {id} deleted successfully");
            }
            else
            {
                _logger.LogWarning($"Attempted to delete booking ID {id}, but it was not found");
            }

            return RedirectToPage("./Index");
        }
    }
}
