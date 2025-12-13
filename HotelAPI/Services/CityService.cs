using HotelAPI.Data;
using HotelAPI.Dtos;
using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Services
{
    public class CityService : ICityService
    {
        private readonly BookingContext _context;

        public CityService(BookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CityDto>> GetAllCitiesAsync()
        {
            var cities = await _context.Cities.ToListAsync();
            return cities.Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public async Task<CityDto?> GetCityByIdAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null) return null;

            return new CityDto
            {
                Id = city.Id,
                Name = city.Name
            };
        }

        public async Task<CityDto> CreateCityAsync(CityDto dto)
        {
            var city = new City
            {
                Name = dto.Name
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            dto.Id = city.Id;
            return dto;
        }

        public async Task<bool> UpdateCityAsync(int id, CityDto dto)
        {
            if (id != dto.Id) return false;

            var city = await _context.Cities.FindAsync(id);
            if (city == null) return false;

            city.Name = dto.Name;
            _context.Entry(city).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCityAsync(int id)
        {
            var city = await _context.Cities
                .Include(c => c.Hotels)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null) return false;

            if (city.Hotels.Any())
                throw new InvalidOperationException("Cannot delete city that has hotels.");

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}