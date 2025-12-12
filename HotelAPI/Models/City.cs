using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelAPI.Models
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}
