using System.ComponentModel.DataAnnotations;

namespace HotelAPI.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

        public double Rating { get; set; }
        public bool HasAvailableRooms { get; set; }

    }
}
