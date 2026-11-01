using MovieTicketBooking.DTOs.Theatres;

namespace MovieTicketBooking.Services.Interfaces
{
    // Interface for Theatre business operations
    public interface ITheatreService
    {
        Task<List<TheatreResponseDto>> GetAllTheatresAsync();
        Task<TheatreResponseDto?> GetTheatreByIdAsync(int id);
        Task<TheatreResponseDto> CreateTheatreAsync(CreateTheatreDto createDto);
        Task<bool> UpdateTheatreAsync(int id, UpdateTheatreDto updateDto);
        Task<bool> DeleteTheatreAsync(int id);
    }
}
