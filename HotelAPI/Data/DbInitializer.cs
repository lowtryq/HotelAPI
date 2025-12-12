using HotelAPI.Models;

namespace HotelAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BookingContext context)
        {
            context.Database.EnsureCreated();

            if (context.Cities.Any())
                return;

            var city1 = new City { Name = "Moscow" };
            var city2 = new City { Name = "Kostroma" };

            context.Cities.AddRange(city1, city2);

            var hotel1 = new Hotel { Name = "Leon", City = city1, Rating = 4.5, HasAvailableRooms = true };
            var hotel2 = new Hotel { Name = "Groza", City = city2, Rating = 4.0, HasAvailableRooms = false };

            context.Hotels.AddRange(hotel1, hotel2);

            context.SaveChanges();
        }
    }
}
