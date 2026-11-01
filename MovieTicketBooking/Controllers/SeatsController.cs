using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        // GET: api/seats/screen/{screenId}
        // Retrieves all physical seats configured for a specific screen
        [HttpGet("screen/{screenId}")]
        public async Task<ActionResult<List<SeatResponseDto>>> GetByScreenId(int screenId)
        {
            var seats = await _seatService.GetSeatsByScreenIdAsync(screenId);
            return Ok(seats);
        }

        // GET: api/seats/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SeatResponseDto>> GetById(int id)
        {
            var seat = await _seatService.GetSeatByIdAsync(id);
            if (seat == null)
            {
                return NotFound(new { message = $"Seat with ID {id} was not found." });
            }

            return Ok(seat);
        }

        // POST: api/seats
        // Creates a single seat with duplicate validation
        [HttpPost]
        public async Task<ActionResult<SeatResponseDto>> Create([FromBody] CreateSeatDto createDto)
        {
            try
            {
                var createdSeat = await _seatService.CreateSeatAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = createdSeat.Id }, createdSeat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/seats/bulk
        // Bulk generates seats for a row (e.g. Row 'A', 10 seats -> A1 to A10)
        [HttpPost("bulk")]
        public async Task<ActionResult<List<SeatResponseDto>>> CreateBulk([FromBody] BulkCreateSeatsDto bulkDto)
        {
            try
            {
                var createdSeats = await _seatService.BulkCreateSeatsAsync(bulkDto);
                return Ok(createdSeats);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/seats/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _seatService.DeleteSeatAsync(id);
            if (!isDeleted)
            {
                return NotFound(new { message = $"Seat with ID {id} was not found to delete." });
            }

            return Ok(new { message = "Seat deleted successfully." });
        }
    }
}
