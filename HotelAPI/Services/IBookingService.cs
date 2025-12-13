using HotelAPI.Dtos;

namespace HotelAPI.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto?> GetBookingByIdAsync(int id);
        Task<BookingDto> CreateBookingAsync(BookingCreateUpdateDto dto);
        Task<bool> UpdateBookingAsync(int id, BookingCreateUpdateDto dto);
        Task<bool> DeleteBookingAsync(int id);
    }
}