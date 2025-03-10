using System.ComponentModel.DataAnnotations;

namespace GolfBookingApp.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Tee Time")]
        public TimeSpan TeeTime { get; set; }

        // Navigation property for players in this booking
        public ICollection<BookingPlayer> Players { get; set; }
    }
}
