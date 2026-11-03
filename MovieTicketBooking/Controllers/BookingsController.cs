
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/bookings
        [HttpGet]
        public async Task<ActionResult<List<BookingResponseDto>>> GetAll()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }



        // GET: api/bookings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = $"Booking with ID {id} was not found." });
            }

            return Ok(booking);
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> Create([FromBody] CreateBookingDto createDto)
        {
            try
            {
                var confirmedBooking = await _bookingService.CreateBookingAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = confirmedBooking.Id }, confirmedBooking);
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

        // PUT: api/bookings/{id}/cancel
        // Cancels an existing booking, releases booked seats, and preserves booking history
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var isCancelled = await _bookingService.CancelBookingAsync(id);
                if (!isCancelled)
                {
                    return NotFound(new { message = $"Booking with ID {id} was not found to cancel." });
                }

                return Ok(new { message = "Booking cancelled successfully and seats released." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
