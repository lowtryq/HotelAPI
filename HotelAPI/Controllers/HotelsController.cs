using HotelAPI.Dtos;
using HotelAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels(
            [FromQuery] string? name = null,
            [FromQuery] string? city = null,
            [FromQuery] double? minRating = null,
            [FromQuery] bool? hasAvailableRooms = null)
        {
            var hotels = await _hotelService.GetHotelsAsync(name, city, minRating, hasAvailableRooms);
            return Ok(hotels);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }

        [HttpPost]
        public async Task<ActionResult<HotelDto>> PostHotel(HotelCreateUpdateDto dto)
        {
            try
            {
                var hotel = await _hotelService.CreateHotelAsync(dto);
                return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, hotel);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, HotelCreateUpdateDto dto)
        {
            try
            {
                var result = await _hotelService.UpdateHotelAsync(id, dto);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var result = await _hotelService.DeleteHotelAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}