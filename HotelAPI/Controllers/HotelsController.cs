using HotelAPI.Data;
using HotelAPI.Dtos;
using HotelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly BookingContext _context;

    public HotelsController(BookingContext context)
    {
        _context = context;
    }

    // GET: api/Hotels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels(
    string? name = null,
    string? city = null,
    double? minRating = null,
    bool? hasAvailableRooms = null)
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

        var hotelDtos = hotels.Select(h => new HotelDto
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

        return Ok(hotelDtos);
    }




    // GET: api/Hotels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> GetHotel(int id)
    {
        var hotel = await _context.Hotels.Include(h => h.City).FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
            return NotFound();

        var hotelDto = new HotelDto
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

        return Ok(hotelDto);
    }

    // POST: api/Hotels
    [HttpPost]
    public async Task<ActionResult<HotelDto>> PostHotel(HotelCreateUpdateDto dto)
    {
        var cityExists = await _context.Cities.AnyAsync(c => c.Id == dto.CityId);
        if (!cityExists)
            return BadRequest($"City with Id {dto.CityId} does not exist.");

        var hotel = new Hotel
        {
            Name = dto.Name,
            CityId = dto.CityId,
            Rating = dto.Rating,
            HasAvailableRooms = dto.HasAvailableRooms
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Загружаем город для формирования DTO
        await _context.Entry(hotel).Reference(h => h.City).LoadAsync();

        var hotelDto = new HotelDto
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

        return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, hotelDto);
    }

    // PUT: api/Hotels/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int id, HotelCreateUpdateDto dto)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null)
            return NotFound();

        var cityExists = await _context.Cities.AnyAsync(c => c.Id == dto.CityId);
        if (!cityExists)
            return BadRequest($"City with Id {dto.CityId} does not exist.");

        hotel.Name = dto.Name;
        hotel.CityId = dto.CityId;
        hotel.Rating = dto.Rating;
        hotel.HasAvailableRooms = dto.HasAvailableRooms;

        _context.Entry(hotel).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!HotelExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/Hotels/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null)
            return NotFound();

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool HotelExists(int id) =>
        _context.Hotels.Any(e => e.Id == id);

}

