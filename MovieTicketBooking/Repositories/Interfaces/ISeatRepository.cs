using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    // Interface for Seat database operations
    public interface ISeatRepository
    {
        Task<List<Seat>> GetSeatsByScreenIdAsync(int screenId);
        Task<Seat?> GetSeatByIdAsync(int id);
        Task<bool> SeatExistsAsync(int screenId, string seatNumber);
        Task<Seat> AddSeatAsync(Seat seat);
        Task AddSeatsBulkAsync(List<Seat> seats);
        Task DeleteSeatAsync(Seat seat);
    }
}
