using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces
{
    // Interface for Seat business operations
    public interface ISeatService
    {
        Task<List<SeatResponseDto>> GetSeatsByScreenIdAsync(int screenId);
        Task<SeatResponseDto?> GetSeatByIdAsync(int id);
        Task<SeatResponseDto> CreateSeatAsync(CreateSeatDto createDto);
        Task<List<SeatResponseDto>> BulkCreateSeatsAsync(BulkCreateSeatsDto bulkDto);
        Task<bool> DeleteSeatAsync(int id);
    }
}
