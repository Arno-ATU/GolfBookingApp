using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GolfBookingApp.Data;
using GolfBookingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace GolfBookingApp.Pages.Members
{
    public class IndexModel:PageModel
    {
        private readonly GolfClubContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(GolfClubContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Member> Members { get; set; } = default!;
        public int TotalMembers { get; set; }

        [BindProperty(SupportsGet = true)]
        public string GenderFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string HandicapFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        public string NameSort { get; set; }
        public string HandicapSort { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync()
        {
            _logger.LogInformation("Loading members with filters: Gender={0}, Handicap={1}, Sort={2}",
                GenderFilter, HandicapFilter, SortOrder);

            // Set up sorting
            NameSort = string.IsNullOrEmpty(SortOrder) ? "name_desc" : "";
            HandicapSort = SortOrder == "handicap_asc" ? "handicap_desc" : "handicap_asc";

            // Track current sort for UI indicators
            CurrentSort = SortOrder;

            // Start with all members
            var query = _context.Members.AsQueryable();

            // Apply gender filter if selected
            if (!string.IsNullOrEmpty(GenderFilter))
            {
                query = query.Where(m => m.Gender == GenderFilter);
            }

            // Apply handicap filter if selected
            if (!string.IsNullOrEmpty(HandicapFilter))
            {
                switch (HandicapFilter)
                {
                    case "Below10":
                        query = query.Where(m => m.Handicap < 10);
                        break;
                    case "11to20":
                        query = query.Where(m => m.Handicap >= 11 && m.Handicap <= 20);
                        break;
                    case "Above20":
                        query = query.Where(m => m.Handicap > 20);
                        break;
                }
            }

            // Apply sorting
            switch (SortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(m => m.Name);
                    break;
                case "handicap_asc":
                    query = query.OrderBy(m => m.Handicap);
                    break;
                case "handicap_desc":
                    query = query.OrderByDescending(m => m.Handicap);
                    break;
                default:
                    query = query.OrderBy(m => m.Name); // Default to name ascending
                    break;
            }

            // Get the total count before pagination (if implemented later)
            TotalMembers = await _context.Members.CountAsync();

            // Execute the query with any applied filters and sorting
            Members = await query.ToListAsync();

            _logger.LogInformation("Loaded {0} members out of {1} total", Members.Count, TotalMembers);
        }
    }
}
