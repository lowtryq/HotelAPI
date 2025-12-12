using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int HotelId { get; set; }

        [ForeignKey(nameof(HotelId))]
        public Hotel Hotel { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string GuestName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string GuestEmail { get; set; } = null!;

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }
    }
}
