using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // GET: api/movies
        // Retrieves all movies
        [HttpGet]
        public async Task<ActionResult<List<MovieResponseDto>>> GetAll()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }

        // GET: api/movies/{id}
        // Retrieves a single movie by its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieResponseDto>> GetById(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                return NotFound(new { message = $"Movie with ID {id} was not found." });
            }

            return Ok(movie);
        }

        // GET: api/movies/search?title=...&genre=...&language=...
        // Searches movies using optional query parameters
        [HttpGet("search")]
        public async Task<ActionResult<List<MovieResponseDto>>> Search(
            [FromQuery] string? title,
            [FromQuery] string? genre,
            [FromQuery] string? language)
        {
            var results = await _movieService.SearchMoviesAsync(title, genre, language);
            return Ok(results);
        }

        // POST: api/movies
        // Creates a new movie
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<MovieResponseDto>> Create([FromBody] CreateMovieDto createDto)
        {
            try
            {
                var createdMovie = await _movieService.CreateMovieAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = createdMovie.Id }, createdMovie);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/movies/{id}
        // Updates an existing movie
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMovieDto updateDto)
        {
            try
            {
                var isUpdated = await _movieService.UpdateMovieAsync(id, updateDto);
                if (!isUpdated)
                {
                    return NotFound(new { message = $"Movie with ID {id} was not found to update." });
                }

                return Ok(new { message = "Movie updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/movies/{id}
        // Deletes a movie by ID
        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _movieService.DeleteMovieAsync(id);
            if (!isDeleted)
            {
                return NotFound(new { message = $"Movie with ID {id} was not found to delete." });
            }

            return Ok(new { message = "Movie deleted successfully." });
        }
    }
}
