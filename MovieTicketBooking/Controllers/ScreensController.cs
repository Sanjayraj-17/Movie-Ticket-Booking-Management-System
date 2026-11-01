using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScreensController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreensController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        // GET: api/screens
        [HttpGet]
        public async Task<ActionResult<List<ScreenResponseDto>>> GetAll()
        {
            var screens = await _screenService.GetAllScreensAsync();
            return Ok(screens);
        }

        // GET: api/screens/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ScreenResponseDto>> GetById(int id)
        {
            var screen = await _screenService.GetScreenByIdAsync(id);
            if (screen == null)
            {
                return NotFound(new { message = $"Screen with ID {id} was not found." });
            }

            return Ok(screen);
        }

        // GET: api/screens/theatre/{theatreId}
        [HttpGet("theatre/{theatreId}")]
        public async Task<ActionResult<List<ScreenResponseDto>>> GetByTheatreId(int theatreId)
        {
            var screens = await _screenService.GetScreensByTheatreIdAsync(theatreId);
            return Ok(screens);
        }

        // POST: api/screens
        [HttpPost]
        public async Task<ActionResult<ScreenResponseDto>> Create([FromBody] CreateScreenDto createDto)
        {
            try
            {
                var createdScreen = await _screenService.CreateScreenAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = createdScreen.Id }, createdScreen);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/screens/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateScreenDto updateDto)
        {
            try
            {
                var isUpdated = await _screenService.UpdateScreenAsync(id, updateDto);
                if (!isUpdated)
                {
                    return NotFound(new { message = $"Screen with ID {id} was not found to update." });
                }

                return Ok(new { message = "Screen updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/screens/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _screenService.DeleteScreenAsync(id);
            if (!isDeleted)
            {
                return NotFound(new { message = $"Screen with ID {id} was not found to delete." });
            }

            return Ok(new { message = "Screen deleted successfully." });
        }
    }
}
