using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    // Service implementing business rules and DTO mapping for Screens
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository _screenRepository;
        private readonly ITheatreRepository _theatreRepository;

        public ScreenService(IScreenRepository screenRepository, ITheatreRepository theatreRepository)
        {
            _screenRepository = screenRepository;
            _theatreRepository = theatreRepository;
        }

        public async Task<List<ScreenResponseDto>> GetAllScreensAsync()
        {
            var screens = await _screenRepository.GetAllScreensAsync();
            var responseList = new List<ScreenResponseDto>();

            foreach (var screen in screens)
            {
                responseList.Add(new ScreenResponseDto
                {
                    Id = screen.Id,
                    ScreenName = screen.ScreenName,
                    TheatreId = screen.TheatreId,
                    TheatreName = screen.Theatre != null ? screen.Theatre.Name : string.Empty
                });
            }

            return responseList;
        }

        public async Task<List<ScreenResponseDto>> GetScreensByTheatreIdAsync(int theatreId)
        {
            var screens = await _screenRepository.GetScreensByTheatreIdAsync(theatreId);
            var responseList = new List<ScreenResponseDto>();

            foreach (var screen in screens)
            {
                responseList.Add(new ScreenResponseDto
                {
                    Id = screen.Id,
                    ScreenName = screen.ScreenName,
                    TheatreId = screen.TheatreId,
                    TheatreName = screen.Theatre != null ? screen.Theatre.Name : string.Empty
                });
            }

            return responseList;
        }

        public async Task<ScreenResponseDto?> GetScreenByIdAsync(int id)
        {
            var screen = await _screenRepository.GetScreenByIdAsync(id);
            if (screen == null)
            {
                return null;
            }

            return new ScreenResponseDto
            {
                Id = screen.Id,
                ScreenName = screen.ScreenName,
                TheatreId = screen.TheatreId,
                TheatreName = screen.Theatre != null ? screen.Theatre.Name : string.Empty
            };
        }

        public async Task<ScreenResponseDto> CreateScreenAsync(CreateScreenDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.ScreenName))
            {
                throw new ArgumentException("Screen name cannot be empty.");
            }

            // Validate that the parent Theatre exists
            var theatre = await _theatreRepository.GetTheatreByIdAsync(createDto.TheatreId);
            if (theatre == null)
            {
                throw new ArgumentException($"Theatre with ID {createDto.TheatreId} does not exist.");
            }

            var screen = new Screen
            {
                ScreenName = createDto.ScreenName,
                TheatreId = createDto.TheatreId
            };

            var createdScreen = await _screenRepository.AddScreenAsync(screen);

            return new ScreenResponseDto
            {
                Id = createdScreen.Id,
                ScreenName = createdScreen.ScreenName,
                TheatreId = createdScreen.TheatreId,
                TheatreName = theatre.Name
            };
        }

        public async Task<bool> UpdateScreenAsync(int id, UpdateScreenDto updateDto)
        {
            var existingScreen = await _screenRepository.GetScreenByIdAsync(id);
            if (existingScreen == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(updateDto.ScreenName))
            {
                throw new ArgumentException("Screen name cannot be empty.");
            }

            existingScreen.ScreenName = updateDto.ScreenName;
            await _screenRepository.UpdateScreenAsync(existingScreen);
            return true;
        }

        public async Task<bool> DeleteScreenAsync(int id)
        {
            var existingScreen = await _screenRepository.GetScreenByIdAsync(id);
            if (existingScreen == null)
            {
                return false;
            }

            await _screenRepository.DeleteScreenAsync(existingScreen);
            return true;
        }
    }
}
