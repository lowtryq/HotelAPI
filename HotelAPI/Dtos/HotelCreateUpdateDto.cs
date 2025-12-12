using System.ComponentModel.DataAnnotations;

public class HotelCreateUpdateDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public int CityId { get; set; }

    public double Rating { get; set; }
    public bool HasAvailableRooms { get; set; }
}