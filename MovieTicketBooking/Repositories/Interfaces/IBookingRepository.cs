using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    // Interface for Booking database operations
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<Booking> CreateBookingWithTransactionAsync(Booking booking, List<BookingSeat> bookingSeats, List<ShowSeat> showSeatsToUpdate);
        Task UpdateBookingWithTransactionAsync(Booking booking, List<ShowSeat> showSeatsToUpdate);
    }
}
