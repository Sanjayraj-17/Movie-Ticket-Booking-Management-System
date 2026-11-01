using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    // Service implementing business rules, overlapping validation, and seat availability
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IScreenRepository _screenRepository;
        private readonly ISeatRepository _seatRepository;

        public ShowService(
            IShowRepository showRepository,
            IMovieRepository movieRepository,
            IScreenRepository screenRepository,
            ISeatRepository seatRepository)
        {
            _showRepository = showRepository;
            _movieRepository = movieRepository;
            _screenRepository = screenRepository;
            _seatRepository = seatRepository;
        }

        public async Task<List<ShowResponseDto>> GetAllShowsAsync()
        {
            var shows = await _showRepository.GetAllShowsAsync();
            var responseList = new List<ShowResponseDto>();

            foreach (var show in shows)
            {
                responseList.Add(MapToDto(show));
            }

            return responseList;
        }

        public async Task<ShowResponseDto?> GetShowByIdAsync(int id)
        {
            var show = await _showRepository.GetShowByIdAsync(id);
            if (show == null)
            {
                return null;
            }

            return MapToDto(show);
        }

        public async Task<List<ShowResponseDto>> GetShowsFilteredAsync(int? movieId, int? theatreId, DateTime? date)
        {
            var shows = await _showRepository.GetShowsFilteredAsync(movieId, theatreId, date);
            var responseList = new List<ShowResponseDto>();

            foreach (var show in shows)
            {
                responseList.Add(MapToDto(show));
            }

            return responseList;
        }

        public async Task<ShowResponseDto> CreateShowAsync(CreateShowDto createDto)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(createDto.MovieId);
            if (movie == null)
            {
                throw new ArgumentException($"Movie with ID {createDto.MovieId} does not exist.");
            }

            var screen = await _screenRepository.GetScreenByIdAsync(createDto.ScreenId);
            if (screen == null)
            {
                throw new ArgumentException($"Screen with ID {createDto.ScreenId} does not exist.");
            }

            if (createDto.EndTime <= createDto.StartTime)
            {
                throw new ArgumentException("Show EndTime must be after StartTime.");
            }

            if (createDto.Price <= 0)
            {
                throw new ArgumentException("Ticket price must be greater than zero.");
            }

            bool overlaps = await _showRepository.HasOverlappingShowAsync(
                createDto.ScreenId,
                createDto.StartTime,
                createDto.EndTime);

            if (overlaps)
            {
                throw new ArgumentException($"An overlapping show is already scheduled in Screen '{screen.ScreenName}' during this time window.");
            }

            var show = new Show
            {
                MovieId = createDto.MovieId,
                ScreenId = createDto.ScreenId,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
                Price = createDto.Price
            };

            var createdShow = await _showRepository.AddShowAsync(show);

            var physicalSeats = await _seatRepository.GetSeatsByScreenIdAsync(createDto.ScreenId);
            if (physicalSeats.Count == 0)
            {
                throw new InvalidOperationException($"Screen '{screen.ScreenName}' has no configured seats. Please configure seats first.");
            }

            var showSeats = new List<ShowSeat>();
            foreach (var seat in physicalSeats)
            {
                decimal seatPrice = createdShow.Price;
                if (seat.SeatType.Equals("VIP", StringComparison.OrdinalIgnoreCase))
                {
                    seatPrice = createdShow.Price * 1.50m;
                }
                else if (seat.SeatType.Equals("Premium", StringComparison.OrdinalIgnoreCase))
                {
                    seatPrice = createdShow.Price * 1.20m;
                }

                showSeats.Add(new ShowSeat
                {
                    ShowId = createdShow.Id,
                    SeatId = seat.Id,
                    Status = "Available",
                    Price = seatPrice
                });
            }

            await _showRepository.AddShowSeatsBulkAsync(showSeats);

            return new ShowResponseDto
            {
                Id = createdShow.Id,
                MovieId = movie.Id,
                MovieTitle = movie.Title,
                ScreenId = screen.Id,
                ScreenName = screen.ScreenName,
                TheatreId = screen.TheatreId,
                TheatreName = screen.Theatre != null ? screen.Theatre.Name : string.Empty,
                StartTime = createdShow.StartTime,
                EndTime = createdShow.EndTime,
                Price = createdShow.Price,
                TotalSeats = showSeats.Count
            };
        }

        public async Task<bool> UpdateShowAsync(int id, UpdateShowDto updateDto)
        {
            var existingShow = await _showRepository.GetShowByIdAsync(id);
            if (existingShow == null)
            {
                return false;
            }

            if (updateDto.EndTime <= updateDto.StartTime)
            {
                throw new ArgumentException("Show EndTime must be after StartTime.");
            }

            if (updateDto.Price <= 0)
            {
                throw new ArgumentException("Ticket price must be greater than zero.");
            }

            bool overlaps = await _showRepository.HasOverlappingShowAsync(
                existingShow.ScreenId,
                updateDto.StartTime,
                updateDto.EndTime,
                excludeShowId: id);

            if (overlaps)
            {
                throw new ArgumentException("Cannot update show: Another show is scheduled during this time window.");
            }

            existingShow.StartTime = updateDto.StartTime;
            existingShow.EndTime = updateDto.EndTime;
            existingShow.Price = updateDto.Price;

            await _showRepository.UpdateShowAsync(existingShow);
            return true;
        }

        public async Task<bool> DeleteShowAsync(int id)
        {
            var existingShow = await _showRepository.GetShowByIdAsync(id);
            if (existingShow == null)
            {
                return false;
            }

            await _showRepository.DeleteShowAsync(existingShow);
            return true;
        }

        // Retrieves complete seat map with summary availability counts for a show
        public async Task<ShowSeatAvailabilitySummaryDto?> GetShowSeatAvailabilitySummaryAsync(int showId)
        {
            var show = await _showRepository.GetShowByIdAsync(showId);
            if (show == null)
            {
                return null;
            }

            var showSeats = await _showRepository.GetShowSeatsAsync(showId);
            var seatDtoList = new List<ShowSeatResponseDto>();
            int availableCount = 0;
            int bookedCount = 0;

            foreach (var ss in showSeats)
            {
                if (ss.Status.Equals("Available", StringComparison.OrdinalIgnoreCase))
                {
                    availableCount++;
                }
                else
                {
                    bookedCount++;
                }

                seatDtoList.Add(new ShowSeatResponseDto
                {
                    ShowSeatId = ss.Id,
                    ShowId = ss.ShowId,
                    SeatId = ss.SeatId,
                    Row = ss.Seat != null ? ss.Seat.Row : string.Empty,
                    SeatNumber = ss.Seat != null ? ss.Seat.SeatNumber : string.Empty,
                    SeatType = ss.Seat != null ? ss.Seat.SeatType : string.Empty,
                    Price = ss.Price,
                    Status = ss.Status
                });
            }

            return new ShowSeatAvailabilitySummaryDto
            {
                ShowId = show.Id,
                MovieTitle = show.Movie != null ? show.Movie.Title : string.Empty,
                ScreenName = show.Screen != null ? show.Screen.ScreenName : string.Empty,
                StartTime = show.StartTime,
                TotalSeats = showSeats.Count,
                AvailableSeatsCount = availableCount,
                BookedSeatsCount = bookedCount,
                Seats = seatDtoList
            };
        }

        // Retrieves seats for a show, optionally filtered by status ("Available" or "Booked")
        public async Task<List<ShowSeatResponseDto>> GetShowSeatsAsync(int showId, string? status = null)
        {
            List<ShowSeat> showSeats;

            if (string.Equals(status, "Available", StringComparison.OrdinalIgnoreCase))
            {
                showSeats = await _showRepository.GetAvailableShowSeatsAsync(showId);
            }
            else if (string.Equals(status, "Booked", StringComparison.OrdinalIgnoreCase))
            {
                showSeats = await _showRepository.GetBookedShowSeatsAsync(showId);
            }
            else
            {
                showSeats = await _showRepository.GetShowSeatsAsync(showId);
            }

            var responseList = new List<ShowSeatResponseDto>();
            foreach (var ss in showSeats)
            {
                responseList.Add(new ShowSeatResponseDto
                {
                    ShowSeatId = ss.Id,
                    ShowId = ss.ShowId,
                    SeatId = ss.SeatId,
                    Row = ss.Seat != null ? ss.Seat.Row : string.Empty,
                    SeatNumber = ss.Seat != null ? ss.Seat.SeatNumber : string.Empty,
                    SeatType = ss.Seat != null ? ss.Seat.SeatType : string.Empty,
                    Price = ss.Price,
                    Status = ss.Status
                });
            }

            return responseList;
        }

        private static ShowResponseDto MapToDto(Show show)
        {
            return new ShowResponseDto
            {
                Id = show.Id,
                MovieId = show.MovieId,
                MovieTitle = show.Movie != null ? show.Movie.Title : string.Empty,
                ScreenId = show.ScreenId,
                ScreenName = show.Screen != null ? show.Screen.ScreenName : string.Empty,
                TheatreId = show.Screen != null ? show.Screen.TheatreId : 0,
                TheatreName = show.Screen != null && show.Screen.Theatre != null ? show.Screen.Theatre.Name : string.Empty,
                StartTime = show.StartTime,
                EndTime = show.EndTime,
                Price = show.Price,
                TotalSeats = show.ShowSeats != null ? show.ShowSeats.Count : 0
            };
        }
    }
}
