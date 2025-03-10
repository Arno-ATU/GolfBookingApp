using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolfBookingApp.Models
{
    public class BookingPlayer
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        public int MemberId { get; set; }
        [ForeignKey("MemberId")]
        public Member Member { get; set; }

        [Display(Name = "Handicap at Booking")]
        public int HandicapAtBooking { get; set; }
    }
}
