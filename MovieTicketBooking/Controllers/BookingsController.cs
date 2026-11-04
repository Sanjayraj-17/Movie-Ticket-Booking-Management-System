
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

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

        // Helper method: Get authenticated customer ID from JWT
        private bool TryGetUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userIdClaim, out userId);
        }

        // GET: api/bookings
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<BookingResponseDto>>> GetAll()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();

            return Ok(bookings);
        }

        // GET: api/bookings/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<BookingResponseDto>> GetById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new
                {
                    message = $"Booking with ID {id} was not found."
                });
            }

            return Ok(booking);
        }

        // POST: api/bookings
        // Creates a booking for the authenticated customer
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<BookingResponseDto>> Create(
            [FromBody] CreateBookingDto createDto)
        {
            try
            {
                // Step 1: Get authenticated UserId from JWT
                if (!TryGetUserId(out int userId))
                {
                    return Unauthorized(new
                    {
                        message = "Valid customer ID not found in JWT token."
                    });
                }

                // Step 2: Create booking with authenticated UserId
                var confirmedBooking =
                    await _bookingService.CreateBookingAsync(
                        createDto, userId);

                // Step 3: Return booking confirmation
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = confirmedBooking.Id },
                    confirmedBooking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/bookings/{id}/cancel
        // Cancels booking only if it belongs to the logged-in customer
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                // Step 1: Get authenticated UserId from JWT
                if (!TryGetUserId(out int userId))
                {
                    return Unauthorized(new
                    {
                        message = "Valid customer ID not found in JWT token."
                    });
                }

                // Step 2: Check ownership and cancel booking
                var isCancelled =
                    await _bookingService.CancelBookingAsync(
                        id, userId.ToString());

                // Step 3: Check result
                if (!isCancelled)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                // Step 4: Successful cancellation
                return Ok(new
                {
                    message = "Booking cancelled successfully and seats released."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
