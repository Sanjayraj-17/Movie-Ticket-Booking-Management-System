using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    // Interface defining database operations for the Movie entity
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAllMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(int id);
        Task<Movie> AddMovieAsync(Movie movie);
        Task UpdateMovieAsync(Movie movie);
        Task DeleteMovieAsync(Movie movie);
        Task<List<Movie>> SearchMoviesAsync(string? title, string? genre, string? language);
    }
}
