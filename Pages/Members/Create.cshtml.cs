using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GolfBookingApp.Models;
using GolfBookingApp.Data;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace GolfBookingApp.Pages.Members
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

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Member Member { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync method called");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");

                // Log validation errors
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            try
            {
                _logger.LogInformation($"Attempting to add member: {Member.Name}, Email: {Member.Email}");

                _context.Members.Add(Member);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully created member with ID: {Member.Id}");
                SuccessMessage = "Member created successfully!";

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating member");

                // Add detailed error messages
                ModelState.AddModelError(string.Empty, $"An error occurred while creating the member: {ex.Message}");

                // For deeper nested exceptions
                var innerException = ex.InnerException;
                if (innerException != null)
                {
                    _logger.LogError(innerException, "Inner exception details");
                    ModelState.AddModelError(string.Empty, $"Additional error details: {innerException.Message}");
                }

                // Check if database connection issues
                if (ex.Message.Contains("connection") || ex.Message.Contains("network") ||
                    ex.Message.Contains("database") || ex.Message.Contains("SQL"))
                {
                    ModelState.AddModelError(string.Empty, "There appears to be a database connection issue. Please contact support.");
                }

                return Page();
            }
        }
    }
}
