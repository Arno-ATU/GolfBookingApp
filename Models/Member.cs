using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolfBookingApp.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Membership Number")]
        public string MembershipNumber { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Range(0, 54)]  // Standard golf handicap range
        public int Handicap { get; set; }

        // Navigation property for bookings
        public ICollection<BookingPlayer> BookingPlayers { get; set; } = new List<BookingPlayer>();
    }
}
