using HotelAPI.Data;
using HotelAPI.Dtos;
using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Services
{
    public class HotelService : IHotelService
    {
        private readonly BookingContext _context;

        public HotelService(BookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelDto>> GetHotelsAsync(string? name, string? city, double? minRating, bool? hasAvailableRooms)
        {
            var query = _context.Hotels.Include(h => h.City).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(h => h.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(h => h.City.Name.Contains(city));

            if (minRating.HasValue)
                query = query.Where(h => h.Rating >= minRating.Value);

            if (hasAvailableRooms.HasValue && hasAvailableRooms.Value)
                query = query.Where(h => h.HasAvailableRooms);

            var hotels = await query.ToListAsync();

            return hotels.Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                Rating = h.Rating,
                HasAvailableRooms = h.HasAvailableRooms,
                City = new CityDto
                {
                    Id = h.City.Id,
                    Name = h.City.Name
                }
            });
        }

        public async Task<HotelDto?> GetHotelByIdAsync(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.City)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null) return null;

            return new HotelDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Rating = hotel.Rating,
                HasAvailableRooms = hotel.HasAvailableRooms,
                City = new CityDto
                {
                    Id = hotel.City.Id,
                    Name = hotel.City.Name
                }
            };
        }

        public async Task<HotelDto> CreateHotelAsync(HotelCreateUpdateDto dto)
        {
            var cityExists = await _context.Cities.AnyAsync(c => c.Id == dto.CityId);
            if (!cityExists)
                throw new ArgumentException($"City with Id {dto.CityId} does not exist.");

            var hotel = new Hotel
            {
                Name = dto.Name,
                CityId = dto.CityId,
                Rating = dto.Rating,
                HasAvailableRooms = dto.HasAvailableRooms
            };

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            await _context.Entry(hotel).Reference(h => h.City).LoadAsync();

            return new HotelDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Rating = hotel.Rating,
                HasAvailableRooms = hotel.HasAvailableRooms,
                City = new CityDto
                {
                    Id = hotel.City.Id,
                    Name = hotel.City.Name
                }
            };
        }

        public async Task<bool> UpdateHotelAsync(int id, HotelCreateUpdateDto dto)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return false;

            var cityExists = await _context.Cities.AnyAsync(c => c.Id == dto.CityId);
            if (!cityExists)
                throw new ArgumentException($"City with Id {dto.CityId} does not exist.");

            hotel.Name = dto.Name;
            hotel.CityId = dto.CityId;
            hotel.Rating = dto.Rating;
            hotel.HasAvailableRooms = dto.HasAvailableRooms;

            _context.Entry(hotel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return false;

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}