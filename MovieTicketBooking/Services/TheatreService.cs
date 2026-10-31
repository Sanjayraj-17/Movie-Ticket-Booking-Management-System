using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    // Service implementing business rules and DTO mapping for Theatres
    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository _theatreRepository;

        public TheatreService(ITheatreRepository theatreRepository)
        {
            _theatreRepository = theatreRepository;
        }

        public async Task<List<TheatreResponseDto>> GetAllTheatresAsync()
        {
            var theatres = await _theatreRepository.GetAllTheatresAsync();
            var responseList = new List<TheatreResponseDto>();

            foreach (var theatre in theatres)
            {
                responseList.Add(new TheatreResponseDto
                {
                    Id = theatre.Id,
                    Name = theatre.Name,
                    City = theatre.City,
                    Address = theatre.Address
                });
            }

            return responseList;
        }

        public async Task<TheatreResponseDto?> GetTheatreByIdAsync(int id)
        {
            var theatre = await _theatreRepository.GetTheatreByIdAsync(id);
            if (theatre == null)
            {
                return null;
            }

            return new TheatreResponseDto
            {
                Id = theatre.Id,
                Name = theatre.Name,
                City = theatre.City,
                Address = theatre.Address
            };
        }

        public async Task<TheatreResponseDto> CreateTheatreAsync(CreateTheatreDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                throw new ArgumentException("Theatre name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(createDto.City))
            {
                throw new ArgumentException("City cannot be empty.");
            }

            var theatre = new Theatre
            {
                Name = createDto.Name,
                City = createDto.City,
                Address = createDto.Address
            };

            var createdTheatre = await _theatreRepository.AddTheatreAsync(theatre);

            return new TheatreResponseDto
            {
                Id = createdTheatre.Id,
                Name = createdTheatre.Name,
                City = createdTheatre.City,
                Address = createdTheatre.Address
            };
        }

        public async Task<bool> UpdateTheatreAsync(int id, UpdateTheatreDto updateDto)
        {
            var existingTheatre = await _theatreRepository.GetTheatreByIdAsync(id);
            if (existingTheatre == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(updateDto.Name))
            {
                throw new ArgumentException("Theatre name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(updateDto.City))
            {
                throw new ArgumentException("City cannot be empty.");
            }

            existingTheatre.Name = updateDto.Name;
            existingTheatre.City = updateDto.City;
            existingTheatre.Address = updateDto.Address;

            await _theatreRepository.UpdateTheatreAsync(existingTheatre);
            return true;
        }

        public async Task<bool> DeleteTheatreAsync(int id)
        {
            var existingTheatre = await _theatreRepository.GetTheatreByIdAsync(id);
            if (existingTheatre == null)
            {
                return false;
            }

            await _theatreRepository.DeleteTheatreAsync(existingTheatre);
            return true;
        }
    }
}
