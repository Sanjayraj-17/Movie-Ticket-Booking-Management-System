
using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces
{
    // Interface for Booking business operations
    public interface IBookingService
    {
        // Get all bookings
        Task<List<BookingResponseDto>> GetAllBookingsAsync();

        // Get booking by ID
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);

        // Create booking with authenticated customer ID
        Task<BookingResponseDto> CreateBookingAsync(
            CreateBookingDto createDto,
            int userId);

        // Cancel booking only if it belongs to the logged-in customer
        Task<bool> CancelBookingAsync(
            int id,
            string userId);
    }
}
