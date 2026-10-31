using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces
{
    // Interface for Screen business operations
    public interface IScreenService
    {
        Task<List<ScreenResponseDto>> GetAllScreensAsync();
        Task<List<ScreenResponseDto>> GetScreensByTheatreIdAsync(int theatreId);
        Task<ScreenResponseDto?> GetScreenByIdAsync(int id);
        Task<ScreenResponseDto> CreateScreenAsync(CreateScreenDto createDto);
        Task<bool> UpdateScreenAsync(int id, UpdateScreenDto updateDto);
        Task<bool> DeleteScreenAsync(int id);
    }
}
