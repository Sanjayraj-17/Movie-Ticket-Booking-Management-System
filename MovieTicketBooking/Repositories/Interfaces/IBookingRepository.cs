
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    // Interface for Booking database operations
    public interface IBookingRepository
    {
        // Get all bookings (Admin)
        Task<List<Booking>> GetAllBookingsAsync();

        // Get bookings belonging to a particular customer
        Task<List<Booking>> GetBookingsByUserIdAsync(int userId);

        // Get booking by ID
        Task<Booking?> GetBookingByIdAsync(int id);

        // Create booking using database transaction
        Task<Booking> CreateBookingWithTransactionAsync(
            Booking booking,
            List<BookingSeat> bookingSeats,
            List<ShowSeat> showSeatsToUpdate);

        // Update booking using database transaction
        Task UpdateBookingWithTransactionAsync(
            Booking booking,
            List<ShowSeat> showSeatsToUpdate);
    }
}
