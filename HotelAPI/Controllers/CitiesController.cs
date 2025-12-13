using HotelAPI.Data;
using HotelAPI.Dtos;
using HotelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace HotelBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly BookingContext _context;

        public CitiesController(BookingContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetCities()
        {
            var cities = await _context.Cities.ToListAsync();

            var cityDtos = cities.Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name
            });

            return Ok(cityDtos);
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> GetCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            var cityDto = new CityDto
            {
                Id = city.Id,
                Name = city.Name
            };

            return Ok(cityDto);
        }

        // POST: api/Cities
        [HttpPost]
        public async Task<ActionResult<CityDto>> PostCity(CityDto cityDto)
        {
            var city = new City
            {
                Name = cityDto.Name
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            cityDto.Id = city.Id;

            return CreatedAtAction(nameof(GetCity), new { id = city.Id }, cityDto);
        }

        // PUT: api/Cities/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, CityDto cityDto)
        {
            if (id != cityDto.Id)
            {
                return BadRequest();
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            city.Name = cityDto.Name;

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.Include(c => c.Hotels).FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            if (city.Hotels.Any())
            {
                return BadRequest("Нельзя удалить город, в котором есть отели.");
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}

