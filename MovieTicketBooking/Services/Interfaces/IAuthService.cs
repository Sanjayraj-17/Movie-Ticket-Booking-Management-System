
using MovieTicketBooking.DTOs.Auth;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(
            RegisterRequestDto request);

        Task<AuthResponseDto?> LoginAsync(
            LoginRequestDto request);
    }
}
