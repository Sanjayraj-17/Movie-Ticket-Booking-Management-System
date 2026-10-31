using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    // Interface for Screen database operations
    public interface IScreenRepository
    {
        Task<List<Screen>> GetAllScreensAsync();
        Task<List<Screen>> GetScreensByTheatreIdAsync(int theatreId);
        Task<Screen?> GetScreenByIdAsync(int id);
        Task<Screen> AddScreenAsync(Screen screen);
        Task UpdateScreenAsync(Screen screen);
        Task DeleteScreenAsync(Screen screen);
    }
}
