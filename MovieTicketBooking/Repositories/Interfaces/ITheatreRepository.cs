using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    // Interface for Theatre database operations
    public interface ITheatreRepository
    {
        Task<List<Theatre>> GetAllTheatresAsync();
        Task<Theatre?> GetTheatreByIdAsync(int id);
        Task<Theatre> AddTheatreAsync(Theatre theatre);
        Task UpdateTheatreAsync(Theatre theatre);
        Task DeleteTheatreAsync(Theatre theatre);
    }
}
