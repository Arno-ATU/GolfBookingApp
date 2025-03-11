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
    public class IndexModel : PageModel
    {
        private readonly GolfBookingApp.Data.GolfClubContext _context;

        public IndexModel(GolfBookingApp.Data.GolfClubContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Booking = await _context.Bookings.ToListAsync();
        }
    }
}
