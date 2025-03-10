using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GolfBookingApp.Data;
using GolfBookingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GolfBookingApp.Pages.Members
{
    public class IndexModel:PageModel
    {
        private readonly GolfClubContext _context;

        public IndexModel(GolfClubContext context)
        {
            _context = context;
        }

        public IList<Member> Members { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Members != null)
            {
                Members = await _context.Members.ToListAsync();
            }
        }
    }
}
