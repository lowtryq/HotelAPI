using HotelAPI.Dtos;
using HotelAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetCities()
        {
            var cities = await _cityService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> GetCity(int id)
        {
            var city = await _cityService.GetCityByIdAsync(id);
            if (city == null) return NotFound();
            return Ok(city);
        }

        [HttpPost]
        public async Task<ActionResult<CityDto>> PostCity(CityDto dto)
        {
            var city = await _cityService.CreateCityAsync(dto);
            return CreatedAtAction(nameof(GetCity), new { id = city.Id }, city);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, CityDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var result = await _cityService.UpdateCityAsync(id, dto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            try
            {
                var result = await _cityService.DeleteCityAsync(id);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}