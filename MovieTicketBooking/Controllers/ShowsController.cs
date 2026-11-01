using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        // GET: api/shows
        [HttpGet]
        public async Task<ActionResult<List<ShowResponseDto>>> GetAll()
        {
            var shows = await _showService.GetAllShowsAsync();
            return Ok(shows);
        }

        // GET: api/shows/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ShowResponseDto>> GetById(int id)
        {
            var show = await _showService.GetShowByIdAsync(id);
            if (show == null)
            {
                return NotFound(new { message = $"Show with ID {id} was not found." });
            }

            return Ok(show);
        }

        // GET: api/shows/filter?movieId=1&theatreId=1&date=2026-11-01
        [HttpGet("filter")]
        public async Task<ActionResult<List<ShowResponseDto>>> Filter(
            [FromQuery] int? movieId,
            [FromQuery] int? theatreId,
            [FromQuery] DateTime? date)
        {
            var shows = await _showService.GetShowsFilteredAsync(movieId, theatreId, date);
            return Ok(shows);
        }

        // GET: api/shows/{id}/seats?status=Available
        // Retrieves seats for a show, optionally filtered by status ("Available" or "Booked")
        [HttpGet("{id}/seats")]
        public async Task<ActionResult<List<ShowSeatResponseDto>>> GetSeats(int id, [FromQuery] string? status)
        {
            var show = await _showService.GetShowByIdAsync(id);
            if (show == null)
            {
                return NotFound(new { message = $"Show with ID {id} was not found." });
            }

            var seats = await _showService.GetShowSeatsAsync(id, status);
            return Ok(seats);
        }

        // GET: api/shows/{id}/availability
        // Retrieves the full interactive seat availability summary and grid
        [HttpGet("{id}/availability")]
        public async Task<ActionResult<ShowSeatAvailabilitySummaryDto>> GetAvailability(int id)
        {
            var summary = await _showService.GetShowSeatAvailabilitySummaryAsync(id);
            if (summary == null)
            {
                return NotFound(new { message = $"Show with ID {id} was not found." });
            }

            return Ok(summary);
        }

        // POST: api/shows
        [HttpPost]
        public async Task<ActionResult<ShowResponseDto>> Create([FromBody] CreateShowDto createDto)
        {
            try
            {
                var createdShow = await _showService.CreateShowAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = createdShow.Id }, createdShow);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/shows/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShowDto updateDto)
        {
            try
            {
                var isUpdated = await _showService.UpdateShowAsync(id, updateDto);
                if (!isUpdated)
                {
                    return NotFound(new { message = $"Show with ID {id} was not found to update." });
                }

                return Ok(new { message = "Show updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/shows/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _showService.DeleteShowAsync(id);
            if (!isDeleted)
            {
                return NotFound(new { message = $"Show with ID {id} was not found to delete." });
            }

            return Ok(new { message = "Show deleted successfully." });
        }
    }
}
