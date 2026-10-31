using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheatresController : ControllerBase
    {
        private readonly ITheatreService _theatreService;

        public TheatresController(ITheatreService theatreService)
        {
            _theatreService = theatreService;
        }

        // GET: api/theatres
        [HttpGet]
        public async Task<ActionResult<List<TheatreResponseDto>>> GetAll()
        {
            var theatres = await _theatreService.GetAllTheatresAsync();
            return Ok(theatres);
        }

        // GET: api/theatres/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TheatreResponseDto>> GetById(int id)
        {
            var theatre = await _theatreService.GetTheatreByIdAsync(id);
            if (theatre == null)
            {
                return NotFound(new { message = $"Theatre with ID {id} was not found." });
            }

            return Ok(theatre);
        }

        // POST: api/theatres
        [HttpPost]
        public async Task<ActionResult<TheatreResponseDto>> Create([FromBody] CreateTheatreDto createDto)
        {
            try
            {
                var createdTheatre = await _theatreService.CreateTheatreAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = createdTheatre.Id }, createdTheatre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/theatres/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTheatreDto updateDto)
        {
            try
            {
                var isUpdated = await _theatreService.UpdateTheatreAsync(id, updateDto);
                if (!isUpdated)
                {
                    return NotFound(new { message = $"Theatre with ID {id} was not found to update." });
                }

                return Ok(new { message = "Theatre updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/theatres/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _theatreService.DeleteTheatreAsync(id);
            if (!isDeleted)
            {
                return NotFound(new { message = $"Theatre with ID {id} was not found to delete." });
            }

            return Ok(new { message = "Theatre deleted successfully." });
        }
    }
}
