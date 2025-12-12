using HotelAPI.Data;
using HotelAPI.Dtos;
using HotelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingContext _context;

    public BookingsController(BookingContext context)
    {
        _context = context;
    }

    // GET: api/Bookings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
    {
        var bookings = await _context.Bookings.Include(b => b.Hotel).ToListAsync();

        var bookingDtos = bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            HotelId = b.HotelId,
            HotelName = b.Hotel.Name,
            GuestName = b.GuestName,
            GuestEmail = b.GuestEmail,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate
        });

        return Ok(bookingDtos);
    }

    // GET: api/Bookings/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetBooking(int id)
    {
        var booking = await _context.Bookings.Include(b => b.Hotel).FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return NotFound();

        var bookingDto = new BookingDto
        {
            Id = booking.Id,
            HotelId = booking.HotelId,
            HotelName = booking.Hotel.Name,
            GuestName = booking.GuestName,
            GuestEmail = booking.GuestEmail,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate
        };

        return Ok(bookingDto);
    }

    // POST: api/Bookings
    [HttpPost]
    public async Task<ActionResult<BookingDto>> PostBooking(BookingCreateUpdateDto dto)
    {
        // Проверяем, что отель существует
        var hotel = await _context.Hotels.FindAsync(dto.HotelId);
        if (hotel == null)
            return BadRequest($"Hotel with Id {dto.HotelId} does not exist.");

        // Проверка дат (CheckOut > CheckIn)
        if (dto.CheckOutDate <= dto.CheckInDate)
            return BadRequest("Check-out date must be later than check-in date.");

        // TODO: здесь можно добавить проверку доступности комнат на даты

        var booking = new Booking
        {
            HotelId = dto.HotelId,
            GuestName = dto.GuestName,
            GuestEmail = dto.GuestEmail,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var bookingDto = new BookingDto
        {
            Id = booking.Id,
            HotelId = booking.HotelId,
            HotelName = hotel.Name,
            GuestName = booking.GuestName,
            GuestEmail = booking.GuestEmail,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate
        };

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, bookingDto);
    }

    // PUT: api/Bookings/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBooking(int id, BookingCreateUpdateDto dto)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return NotFound();

        var hotel = await _context.Hotels.FindAsync(dto.HotelId);
        if (hotel == null)
            return BadRequest($"Hotel with Id {dto.HotelId} does not exist.");

        if (dto.CheckOutDate <= dto.CheckInDate)
            return BadRequest("Check-out date must be later than check-in date.");

        booking.HotelId = dto.HotelId;
        booking.GuestName = dto.GuestName;
        booking.GuestEmail = dto.GuestEmail;
        booking.CheckInDate = dto.CheckInDate;
        booking.CheckOutDate = dto.CheckOutDate;

        _context.Entry(booking).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Bookings.Any(b => b.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/Bookings/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return NotFound();

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
