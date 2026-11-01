using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces
{
    // Interface defining business logic operations for Movies
    public interface IMovieService
    {
        Task<List<MovieResponseDto>> GetAllMoviesAsync();
        Task<MovieResponseDto?> GetMovieByIdAsync(int id);
        Task<MovieResponseDto> CreateMovieAsync(CreateMovieDto createDto);
        Task<bool> UpdateMovieAsync(int id, UpdateMovieDto updateDto);
        Task<bool> DeleteMovieAsync(int id);
        Task<List<MovieResponseDto>> SearchMoviesAsync(string? title, string? genre, string? language);
    }
}
