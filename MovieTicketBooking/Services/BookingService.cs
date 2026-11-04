
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    // Service implementing booking and cancellation workflows
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IShowRepository _showRepository;
        private readonly ISeatRepository _seatRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IShowRepository showRepository,
            ISeatRepository seatRepository)
        {
            _bookingRepository = bookingRepository;
            _showRepository = showRepository;
            _seatRepository = seatRepository;
        }

        // GET ALL BOOKINGS
        public async Task<List<BookingResponseDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync();

            var responseList = new List<BookingResponseDto>();

            foreach (var booking in bookings)
            {
                responseList.Add(MapToDto(booking));
            }

            return responseList;
        }

        // GET BOOKING BY ID
        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return null;
            }

            return MapToDto(booking);
        }

        // CREATE BOOKING
        public async Task<BookingResponseDto> CreateBookingAsync(
            CreateBookingDto createDto, int userId)
        {
            // Step 1: Validate customer details
            if (string.IsNullOrWhiteSpace(createDto.CustomerName))
            {
                throw new ArgumentException("Customer name is required.");
            }

            if (string.IsNullOrWhiteSpace(createDto.CustomerEmail))
            {
                throw new ArgumentException("Customer email is required.");
            }

            if (string.IsNullOrWhiteSpace(createDto.CustomerPhone))
            {
                throw new ArgumentException("Customer phone number is required.");
            }

            if (createDto.SeatIds == null || createDto.SeatIds.Count == 0)
            {
                throw new ArgumentException(
                    "At least one seat must be selected.");
            }

            // Step 2: Check whether the show exists
            var show = await _showRepository.GetShowByIdAsync(createDto.ShowId);

            if (show == null)
            {
                throw new ArgumentException(
                    $"Show with ID {createDto.ShowId} does not exist.");
            }

            // Step 3: Check duplicate seat IDs
            var distinctSeats = new HashSet<int>(createDto.SeatIds);

            if (distinctSeats.Count != createDto.SeatIds.Count)
            {
                throw new ArgumentException(
                    "Duplicate seat IDs requested.");
            }

            // Get all seats configured for this show
            var allShowSeats =
                await _showRepository.GetShowSeatsAsync(createDto.ShowId);

            var selectedShowSeats = new List<ShowSeat>();
            var bookingSeatsToCreate = new List<BookingSeat>();

            decimal calculatedTotalAmount = 0m;

            foreach (var requestedSeatId in createDto.SeatIds)
            {
                // Step 4: Check whether seat belongs to this show's screen
                ShowSeat? matchedShowSeat = null;

                foreach (var ss in allShowSeats)
                {
                    if (ss.SeatId == requestedSeatId)
                    {
                        matchedShowSeat = ss;
                        break;
                    }
                }

                if (matchedShowSeat == null)
                {
                    throw new ArgumentException(
                        $"Seat with ID {requestedSeatId} does not belong to this show.");
                }

                // Step 5: Check seat availability
                if (!matchedShowSeat.Status.Equals(
                    "Available", StringComparison.OrdinalIgnoreCase))
                {
                    var seatLabel = matchedShowSeat.Seat != null
                        ? matchedShowSeat.Seat.SeatNumber
                        : requestedSeatId.ToString();

                    throw new InvalidOperationException(
                        $"Seat '{seatLabel}' is already booked.");
                }

                // Step 6: Calculate price on the backend
                calculatedTotalAmount += matchedShowSeat.Price;

                selectedShowSeats.Add(matchedShowSeat);

                // Step 7: Prepare BookingSeat record
                bookingSeatsToCreate.Add(new BookingSeat
                {
                    SeatId = requestedSeatId,
                    Price = matchedShowSeat.Price
                });
            }

            // Step 8: Create Booking record
            var booking = new Booking
            {
                ShowId = createDto.ShowId,

                // IMPORTANT: Assign authenticated customer's ID
                UserId = userId,

                CustomerName = createDto.CustomerName,
                CustomerEmail = createDto.CustomerEmail,
                CustomerPhone = createDto.CustomerPhone,

                BookingTime = DateTime.UtcNow,
                TotalAmount = calculatedTotalAmount,
                BookingStatus = "Confirmed"
            };

            // Step 9: Save booking and seats using transaction
            var savedBooking =
                await _bookingRepository.CreateBookingWithTransactionAsync(
                    booking,
                    bookingSeatsToCreate,
                    selectedShowSeats);

            // Step 10: Retrieve saved booking
            var confirmedBooking =
                await _bookingRepository.GetBookingByIdAsync(savedBooking.Id);

            // Step 11: Return booking response
            return MapToDto(confirmedBooking!);
        }

        // CANCEL BOOKING WITH OWNERSHIP AUTHORIZATION
        public async Task<bool> CancelBookingAsync(int id, string userId)
        {
            // Step 1: Find booking
            var booking =
                await _bookingRepository.GetBookingByIdAsync(id);

            // Step 2: Check whether booking exists
            if (booking == null)
            {
                return false;
            }

            // Step 3: Check booking ownership
            if (booking.UserId.ToString() != userId)
            {
                return false;
            }

            // Step 4: Check whether already cancelled
            if (booking.BookingStatus.Equals(
                "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "This booking is already cancelled.");
            }

            // Step 5: Change booking status
            booking.BookingStatus = "Cancelled";

            // Step 6: Retrieve seats for this show
            var allShowSeats =
                await _showRepository.GetShowSeatsAsync(booking.ShowId);

            var showSeatsToRelease = new List<ShowSeat>();

            // Step 7: Release only seats belonging to this booking
            foreach (var bs in booking.BookingSeats)
            {
                foreach (var ss in allShowSeats)
                {
                    if (ss.SeatId == bs.SeatId)
                    {
                        ss.Status = "Available";
                        showSeatsToRelease.Add(ss);
                        break;
                    }
                }
            }

            // Step 8: Save changes using database transaction
            await _bookingRepository.UpdateBookingWithTransactionAsync(
                booking,
                showSeatsToRelease);

            return true;
        }

        // MAP BOOKING MODEL TO RESPONSE DTO
        private static BookingResponseDto MapToDto(Booking booking)
        {
            var seatDtos = new List<BookingSeatResponseDto>();

            if (booking.BookingSeats != null)
            {
                foreach (var bs in booking.BookingSeats)
                {
                    seatDtos.Add(new BookingSeatResponseDto
                    {
                        SeatId = bs.SeatId,
                        Row = bs.Seat != null
                            ? bs.Seat.Row : string.Empty,

                        SeatNumber = bs.Seat != null
                            ? bs.Seat.SeatNumber : string.Empty,

                        SeatType = bs.Seat != null
                            ? bs.Seat.SeatType : string.Empty,

                        Price = bs.Price
                    });
                }
            }

            return new BookingResponseDto
            {
                Id = booking.Id,
                ShowId = booking.ShowId,

                MovieTitle = booking.Show != null &&
                             booking.Show.Movie != null
                    ? booking.Show.Movie.Title : string.Empty,

                ScreenName = booking.Show != null &&
                             booking.Show.Screen != null
                    ? booking.Show.Screen.ScreenName : string.Empty,

                TheatreName = booking.Show != null &&
                              booking.Show.Screen != null &&
                              booking.Show.Screen.Theatre != null
                    ? booking.Show.Screen.Theatre.Name : string.Empty,

                StartTime = booking.Show != null
                    ? booking.Show.StartTime : DateTime.MinValue,

                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,

                BookingTime = booking.BookingTime,
                TotalAmount = booking.TotalAmount,
                BookingStatus = booking.BookingStatus,

                Seats = seatDtos
            };
        }
    }
}

