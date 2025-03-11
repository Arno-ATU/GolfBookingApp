using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GolfBookingApp.Data;
using GolfBookingApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GolfBookingApp.Pages.Bookings
{
    public class CreateModel:PageModel
    {
        private readonly GolfClubContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(GolfClubContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Booking Booking { get; set; } = new Booking();

        [BindProperty]
        [Required(ErrorMessage = "At least one player is required.")]
        public int?[] SelectedPlayerIds { get; set; } = new int?[4]; // Up to 4 players

        [TempData]
        public string ErrorMessage { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public SelectList MemberOptions { get; set; }
        public SelectList TeeTimeOptions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Initialize empty booking
            Booking = new Booking
            {
                BookingDate = DateTime.Today.AddDays(1), // Default to tomorrow
                TeeTime = new TimeSpan(9, 0, 0) // Default to 9:00 AM
            };

            await LoadMembersAsync();
            CreateTeeTimeOptions();

            return Page();
        }

        public JsonResult OnGetGetMemberHandicap(int memberId)
        {
            var member = _context.Members.Find(memberId);
            return new JsonResult(member?.Handicap ?? 0);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync called for Create Booking");

            ModelState.Remove("Booking.Players"); // Remove validation for Players collection

            if (!ModelState.IsValid)
            {
                await LoadMembersAsync();
                CreateTeeTimeOptions();
                return Page();
            }

            // Validate at least one player is selected
            if (!SelectedPlayerIds[0].HasValue)
            {
                ModelState.AddModelError("SelectedPlayerIds[0]", "At least one player is required.");
                await LoadMembersAsync();
                CreateTeeTimeOptions();
                return Page();
            }

            // Check if the tee time is already booked
            var teeTimeAlreadyBooked = await _context.Bookings
                .AnyAsync(b => b.BookingDate.Date == Booking.BookingDate.Date &&
                               b.TeeTime == Booking.TeeTime);

            if (teeTimeAlreadyBooked)
            {
                ModelState.AddModelError(string.Empty, $"Tee time {Booking.TeeTime.ToString(@"hh\:mm")} on {Booking.BookingDate.ToShortDateString()} is already booked.");
                await LoadMembersAsync();
                CreateTeeTimeOptions();
                return Page();
            }

            // Check for existing booking on the same day for any selected member
            var bookingDate = Booking.BookingDate.Date;
            var existingBookings = new HashSet<int>();

            foreach (var playerId in SelectedPlayerIds.Where(id => id.HasValue))
            {
                var memberHasBooking = await _context.BookingPlayers
                    .Include(bp => bp.Booking)
                    .AnyAsync(bp => bp.MemberId == playerId.Value &&
                                    bp.Booking.BookingDate.Date == bookingDate);

                if (memberHasBooking)
                {
                    var member = await _context.Members.FindAsync(playerId.Value);
                    existingBookings.Add(playerId.Value);
                    ModelState.AddModelError(string.Empty, $"{member.Name} already has a booking on {bookingDate.ToShortDateString()}.");
                }
            }

            if (existingBookings.Any())
            {
                await LoadMembersAsync();
                CreateTeeTimeOptions();
                return Page();
            }

            try
            {
                // Create the booking
                _context.Bookings.Add(Booking);
                await _context.SaveChangesAsync();

                // Add the selected players to the booking
                var playerEntries = new List<BookingPlayer>();
                foreach (var playerId in SelectedPlayerIds.Where(id => id.HasValue))
                {
                    var member = await _context.Members.FindAsync(playerId.Value);

                    playerEntries.Add(new BookingPlayer
                    {
                        BookingId = Booking.Id,
                        MemberId = playerId.Value,
                        HandicapAtBooking = member.Handicap
                    });
                }

                _context.BookingPlayers.AddRange(playerEntries);
                await _context.SaveChangesAsync();

                SuccessMessage = "Tee time booking created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the booking. Please try again.");
                await LoadMembersAsync();
                CreateTeeTimeOptions();
                return Page();
            }
        }

        private async Task LoadMembersAsync()
        {
            var members = await _context.Members.OrderBy(m => m.Name).ToListAsync();
            MemberOptions = new SelectList(members, "Id", "Name");
        }

        private void CreateTeeTimeOptions()
        {
            // Generate tee times at 15-minute intervals from 7:00 AM to 7:00 PM
            var teeTimeOptions = new List<SelectListItem>();
            var startTime = new TimeSpan(7, 0, 0); // 7:00 AM
            var endTime = new TimeSpan(19, 0, 0);  // 7:00 PM
            var interval = new TimeSpan(0, 15, 0); // 15 minutes

            for (var time = startTime; time < endTime; time = time.Add(interval))
            {
                teeTimeOptions.Add(new SelectListItem
                {
                    Value = time.ToString(),
                    Text = time.ToString(@"hh\:mm")
                });
            }

            TeeTimeOptions = new SelectList(teeTimeOptions, "Value", "Text");
        }
    }
}
