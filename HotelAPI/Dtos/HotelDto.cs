namespace HotelAPI.Dtos
{
    public class HotelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CityDto City { get; set; }
        public double Rating { get; set; }
        public bool HasAvailableRooms { get; set; }
    }
}
