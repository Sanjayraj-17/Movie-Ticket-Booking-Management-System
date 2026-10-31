using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    // Service implementing business rules, bulk creation, and validation for Seats
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IScreenRepository _screenRepository;

        public SeatService(ISeatRepository seatRepository, IScreenRepository screenRepository)
        {
            _seatRepository = seatRepository;
            _screenRepository = screenRepository;
        }

        public async Task<List<SeatResponseDto>> GetSeatsByScreenIdAsync(int screenId)
        {
            var seats = await _seatRepository.GetSeatsByScreenIdAsync(screenId);
            var responseList = new List<SeatResponseDto>();

            foreach (var seat in seats)
            {
                responseList.Add(new SeatResponseDto
                {
                    Id = seat.Id,
                    ScreenId = seat.ScreenId,
                    Row = seat.Row,
                    SeatNumber = seat.SeatNumber,
                    SeatType = seat.SeatType
                });
            }

            return responseList;
        }

        public async Task<SeatResponseDto?> GetSeatByIdAsync(int id)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(id);
            if (seat == null)
            {
                return null;
            }

            return new SeatResponseDto
            {
                Id = seat.Id,
                ScreenId = seat.ScreenId,
                Row = seat.Row,
                SeatNumber = seat.SeatNumber,
                SeatType = seat.SeatType
            };
        }

        // Creates a single seat with duplicate validation
        public async Task<SeatResponseDto> CreateSeatAsync(CreateSeatDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Row))
            {
                throw new ArgumentException("Seat row cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(createDto.SeatNumber))
            {
                throw new ArgumentException("Seat number cannot be empty.");
            }

            // Check if Screen exists
            var screen = await _screenRepository.GetScreenByIdAsync(createDto.ScreenId);
            if (screen == null)
            {
                throw new ArgumentException($"Screen with ID {createDto.ScreenId} does not exist.");
            }

            // Validate duplicate seat number in the same screen
            bool exists = await _seatRepository.SeatExistsAsync(createDto.ScreenId, createDto.SeatNumber);
            if (exists)
            {
                throw new ArgumentException($"Seat '{createDto.SeatNumber}' already exists in this screen.");
            }

            var seat = new Seat
            {
                ScreenId = createDto.ScreenId,
                Row = createDto.Row.Trim().ToUpper(),
                SeatNumber = createDto.SeatNumber.Trim().ToUpper(),
                SeatType = string.IsNullOrWhiteSpace(createDto.SeatType) ? "Standard" : createDto.SeatType
            };

            var createdSeat = await _seatRepository.AddSeatAsync(seat);

            return new SeatResponseDto
            {
                Id = createdSeat.Id,
                ScreenId = createdSeat.ScreenId,
                Row = createdSeat.Row,
                SeatNumber = createdSeat.SeatNumber,
                SeatType = createdSeat.SeatType
            };
        }

        // Bulk creates seats for a row (e.g. Row 'A', 10 seats -> A1 to A10) with duplicate validation
        public async Task<List<SeatResponseDto>> BulkCreateSeatsAsync(BulkCreateSeatsDto bulkDto)
        {
            if (string.IsNullOrWhiteSpace(bulkDto.Row))
            {
                throw new ArgumentException("Seat row cannot be empty.");
            }

            if (bulkDto.TotalSeats <= 0 || bulkDto.TotalSeats > 100)
            {
                throw new ArgumentException("Total seats must be between 1 and 100.");
            }

            // Check if Screen exists
            var screen = await _screenRepository.GetScreenByIdAsync(bulkDto.ScreenId);
            if (screen == null)
            {
                throw new ArgumentException($"Screen with ID {bulkDto.ScreenId} does not exist.");
            }

            var cleanRow = bulkDto.Row.Trim().ToUpper();
            var seatCategory = string.IsNullOrWhiteSpace(bulkDto.SeatType) ? "Standard" : bulkDto.SeatType;
            var seatsToCreate = new List<Seat>();

            // Generate seats and check for duplicates
            for (int i = 1; i <= bulkDto.TotalSeats; i++)
            {
                var seatNumber = $"{cleanRow}{i}";

                bool exists = await _seatRepository.SeatExistsAsync(bulkDto.ScreenId, seatNumber);
                if (exists)
                {
                    throw new ArgumentException($"Seat '{seatNumber}' already exists in screen {bulkDto.ScreenId}. Bulk creation cancelled.");
                }

                seatsToCreate.Add(new Seat
                {
                    ScreenId = bulkDto.ScreenId,
                    Row = cleanRow,
                    SeatNumber = seatNumber,
                    SeatType = seatCategory
                });
            }

            await _seatRepository.AddSeatsBulkAsync(seatsToCreate);

            // Fetch and return the created seats
            var createdSeats = await _seatRepository.GetSeatsByScreenIdAsync(bulkDto.ScreenId);
            var responseList = new List<SeatResponseDto>();

            foreach (var s in createdSeats)
            {
                if (s.Row == cleanRow)
                {
                    responseList.Add(new SeatResponseDto
                    {
                        Id = s.Id,
                        ScreenId = s.ScreenId,
                        Row = s.Row,
                        SeatNumber = s.SeatNumber,
                        SeatType = s.SeatType
                    });
                }
            }

            return responseList;
        }

        public async Task<bool> DeleteSeatAsync(int id)
        {
            var existingSeat = await _seatRepository.GetSeatByIdAsync(id);
            if (existingSeat == null)
            {
                return false;
            }

            await _seatRepository.DeleteSeatAsync(existingSeat);
            return true;
        }
    }
}
