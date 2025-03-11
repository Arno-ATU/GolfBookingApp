using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GolfBookingApp.Data;
using GolfBookingApp.Models;

namespace GolfBookingApp.Pages.Bookings
{
    public class CreateModel : PageModel
    {
        private readonly GolfBookingApp.Data.GolfClubContext _context;

        public CreateModel(GolfBookingApp.Data.GolfClubContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
