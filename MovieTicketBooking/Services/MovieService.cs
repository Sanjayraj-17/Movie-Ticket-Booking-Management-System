using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    // Service handling business logic, validation, and manual mapping for Movies
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        // Get all movies and map them to response DTOs
        public async Task<List<MovieResponseDto>> GetAllMoviesAsync()
        {
            var movies = await _movieRepository.GetAllMoviesAsync();
            var responseList = new List<MovieResponseDto>();

            foreach (var movie in movies)
            {
                responseList.Add(new MovieResponseDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    Language = movie.Language,
                    Genre = movie.Genre,
                    DurationInMinutes = movie.DurationInMinutes,
                    ReleaseDate = movie.ReleaseDate
                });
            }

            return responseList;
        }

        // Get single movie by ID
        public async Task<MovieResponseDto?> GetMovieByIdAsync(int id)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(id);
            if (movie == null)
            {
                return null;
            }

            return new MovieResponseDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Language = movie.Language,
                Genre = movie.Genre,
                DurationInMinutes = movie.DurationInMinutes,
                ReleaseDate = movie.ReleaseDate
            };
        }

        // Validate and create a new movie
        public async Task<MovieResponseDto> CreateMovieAsync(CreateMovieDto createDto)
        {
            // Simple validation
            if (string.IsNullOrWhiteSpace(createDto.Title))
            {
                throw new ArgumentException("Movie title cannot be empty.");
            }

            if (createDto.DurationInMinutes <= 0)
            {
                throw new ArgumentException("Duration must be greater than 0 minutes.");
            }

            // Manual mapping from CreateMovieDto to Movie entity model
            var movie = new Movie
            {
                Title = createDto.Title,
                Description = createDto.Description,
                Language = createDto.Language,
                Genre = createDto.Genre,
                DurationInMinutes = createDto.DurationInMinutes,
                ReleaseDate = createDto.ReleaseDate
            };

            var createdMovie = await _movieRepository.AddMovieAsync(movie);

            // Manual mapping from Movie entity to MovieResponseDto
            return new MovieResponseDto
            {
                Id = createdMovie.Id,
                Title = createdMovie.Title,
                Description = createdMovie.Description,
                Language = createdMovie.Language,
                Genre = createdMovie.Genre,
                DurationInMinutes = createdMovie.DurationInMinutes,
                ReleaseDate = createdMovie.ReleaseDate
            };
        }

        // Validate and update an existing movie
        public async Task<bool> UpdateMovieAsync(int id, UpdateMovieDto updateDto)
        {
            var existingMovie = await _movieRepository.GetMovieByIdAsync(id);
            if (existingMovie == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(updateDto.Title))
            {
                throw new ArgumentException("Movie title cannot be empty.");
            }

            if (updateDto.DurationInMinutes <= 0)
            {
                throw new ArgumentException("Duration must be greater than 0 minutes.");
            }

            // Update entity properties
            existingMovie.Title = updateDto.Title;
            existingMovie.Description = updateDto.Description;
            existingMovie.Language = updateDto.Language;
            existingMovie.Genre = updateDto.Genre;
            existingMovie.DurationInMinutes = updateDto.DurationInMinutes;
            existingMovie.ReleaseDate = updateDto.ReleaseDate;

            await _movieRepository.UpdateMovieAsync(existingMovie);
            return true;
        }

        // Delete movie by ID
        public async Task<bool> DeleteMovieAsync(int id)
        {
            var existingMovie = await _movieRepository.GetMovieByIdAsync(id);
            if (existingMovie == null)
            {
                return false;
            }

            await _movieRepository.DeleteMovieAsync(existingMovie);
            return true;
        }

        // Search movies by title, genre, and language
        public async Task<List<MovieResponseDto>> SearchMoviesAsync(string? title, string? genre, string? language)
        {
            var movies = await _movieRepository.SearchMoviesAsync(title, genre, language);
            var responseList = new List<MovieResponseDto>();

            foreach (var movie in movies)
            {
                responseList.Add(new MovieResponseDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    Language = movie.Language,
                    Genre = movie.Genre,
                    DurationInMinutes = movie.DurationInMinutes,
                    ReleaseDate = movie.ReleaseDate
                });
            }

            return responseList;
        }
    }
}
