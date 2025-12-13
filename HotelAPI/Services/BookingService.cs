using HotelAPI.Data;
using HotelAPI.Dtos;
using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingContext _context;

        public BookingService(BookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Hotel)
                .ToListAsync();

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                HotelId = b.HotelId,
                HotelName = b.Hotel.Name,
                GuestName = b.GuestName,
                GuestEmail = b.GuestEmail,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate
            });
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return null;

            return new BookingDto
            {
                Id = booking.Id,
                HotelId = booking.HotelId,
                HotelName = booking.Hotel.Name,
                GuestName = booking.GuestName,
                GuestEmail = booking.GuestEmail,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate
            };
        }

        public async Task<BookingDto> CreateBookingAsync(BookingCreateUpdateDto dto)
        {
            var hotel = await _context.Hotels.FindAsync(dto.HotelId);
            if (hotel == null)
                throw new ArgumentException($"Hotel with Id {dto.HotelId} does not exist.");

            if (dto.CheckOutDate <= dto.CheckInDate)
                throw new ArgumentException("Check-out date must be later than check-in date.");

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

            return new BookingDto
            {
                Id = booking.Id,
                HotelId = booking.HotelId,
                HotelName = hotel.Name,
                GuestName = booking.GuestName,
                GuestEmail = booking.GuestEmail,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate
            };
        }

        public async Task<bool> UpdateBookingAsync(int id, BookingCreateUpdateDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            var hotel = await _context.Hotels.FindAsync(dto.HotelId);
            if (hotel == null)
                throw new ArgumentException($"Hotel with Id {dto.HotelId} does not exist.");

            if (dto.CheckOutDate <= dto.CheckInDate)
                throw new ArgumentException("Check-out date must be later than check-in date.");

            booking.HotelId = dto.HotelId;
            booking.GuestName = dto.GuestName;
            booking.GuestEmail = dto.GuestEmail;
            booking.CheckInDate = dto.CheckInDate;
            booking.CheckOutDate = dto.CheckOutDate;

            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}