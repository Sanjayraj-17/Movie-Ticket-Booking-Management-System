using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces
{
    // Interface for Show business operations
    public interface IShowService
    {
        Task<List<ShowResponseDto>> GetAllShowsAsync();
        Task<ShowResponseDto?> GetShowByIdAsync(int id);
        Task<List<ShowResponseDto>> GetShowsFilteredAsync(int? movieId, int? theatreId, DateTime? date);
        Task<ShowResponseDto> CreateShowAsync(CreateShowDto createDto);
        Task<bool> UpdateShowAsync(int id, UpdateShowDto updateDto);
        Task<bool> DeleteShowAsync(int id);

        // Show seat availability queries
        Task<ShowSeatAvailabilitySummaryDto?> GetShowSeatAvailabilitySummaryAsync(int showId);
        Task<List<ShowSeatResponseDto>> GetShowSeatsAsync(int showId, string? status = null);
    }
}
