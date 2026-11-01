using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces
{
    // Interface for Booking business operations
    public interface IBookingService
    {
        Task<List<BookingResponseDto>> GetAllBookingsAsync();
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createDto);
        Task<bool> CancelBookingAsync(int id);
    }
}
