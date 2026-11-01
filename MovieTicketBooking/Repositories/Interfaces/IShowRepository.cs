using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    // Interface for Show database operations
    public interface IShowRepository
    {
        Task<List<Show>> GetAllShowsAsync();
        Task<Show?> GetShowByIdAsync(int id);
        Task<List<Show>> GetShowsFilteredAsync(int? movieId, int? theatreId, DateTime? date);
        Task<bool> HasOverlappingShowAsync(int screenId, DateTime startTime, DateTime endTime, int? excludeShowId = null);
        Task<Show> AddShowAsync(Show show);
        Task UpdateShowAsync(Show show);
        Task DeleteShowAsync(Show show);
        Task AddShowSeatsBulkAsync(List<ShowSeat> showSeats);

        // ShowSeat queries
        Task<List<ShowSeat>> GetShowSeatsAsync(int showId);
        Task<List<ShowSeat>> GetAvailableShowSeatsAsync(int showId);
        Task<List<ShowSeat>> GetBookedShowSeatsAsync(int showId);
    }
}
