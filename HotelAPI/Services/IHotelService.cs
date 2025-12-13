using HotelAPI.Dtos;

namespace HotelAPI.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> GetHotelsAsync(string? name, string? city, double? minRating, bool? hasAvailableRooms);
        Task<HotelDto?> GetHotelByIdAsync(int id);
        Task<HotelDto> CreateHotelAsync(HotelCreateUpdateDto dto);
        Task<bool> UpdateHotelAsync(int id, HotelCreateUpdateDto dto);
        Task<bool> DeleteHotelAsync(int id);
    }
}