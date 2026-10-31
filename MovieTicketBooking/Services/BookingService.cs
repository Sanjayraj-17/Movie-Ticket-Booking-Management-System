using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    // Service implementing the complete booking and cancellation workflows with transactions
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

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return null;
            }

            return MapToDto(booking);
        }

        // Complete 11-step Booking Business Workflow
        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createDto)
        {
            // Step 1: Validate customer contact details and seats list
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
                throw new ArgumentException("At least one seat must be selected to book.");
            }

            // Step 2: Check whether the show exists
            var show = await _showRepository.GetShowByIdAsync(createDto.ShowId);
            if (show == null)
            {
                throw new ArgumentException($"Show with ID {createDto.ShowId} does not exist.");
            }

            // Step 3: Check whether the requested seats are distinct
            var distinctSeats = new HashSet<int>(createDto.SeatIds);
            if (distinctSeats.Count != createDto.SeatIds.Count)
            {
                throw new ArgumentException("Duplicate seat IDs requested in the same booking.");
            }

            // Fetch all ShowSeats configured for this show
            var allShowSeats = await _showRepository.GetShowSeatsAsync(createDto.ShowId);

            var selectedShowSeats = new List<ShowSeat>();
            var bookingSeatsToCreate = new List<BookingSeat>();
            decimal calculatedTotalAmount = 0m;

            foreach (var requestedSeatId in createDto.SeatIds)
            {
                // Step 4: Check whether requested seat belongs to the selected show's screen
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
                    throw new ArgumentException($"Seat with ID {requestedSeatId} does not belong to the screen hall for this show.");
                }

                // Step 5: Check whether the requested seat is Available
                if (!matchedShowSeat.Status.Equals("Available", StringComparison.OrdinalIgnoreCase))
                {
                    var seatLabel = matchedShowSeat.Seat != null ? matchedShowSeat.Seat.SeatNumber : requestedSeatId.ToString();
                    throw new InvalidOperationException($"Seat '{seatLabel}' is already booked. Please select available seats.");
                }

                // Step 6: Calculate ticket amount on the backend
                calculatedTotalAmount += matchedShowSeat.Price;
                selectedShowSeats.Add(matchedShowSeat);

                // Step 8: Prepare BookingSeat record
                bookingSeatsToCreate.Add(new BookingSeat
                {
                    SeatId = requestedSeatId,
                    Price = matchedShowSeat.Price
                });
            }

            // Step 7: Create the Booking record
            var booking = new Booking
            {
                ShowId = createDto.ShowId,
                CustomerName = createDto.CustomerName,
                CustomerEmail = createDto.CustomerEmail,
                CustomerPhone = createDto.CustomerPhone,
                BookingTime = DateTime.UtcNow,
                TotalAmount = calculatedTotalAmount,
                BookingStatus = "Confirmed"
            };

            // Steps 9 & 10: Change ShowSeat statuses to "Booked" and save using a database transaction
            var savedBooking = await _bookingRepository.CreateBookingWithTransactionAsync(
                booking,
                bookingSeatsToCreate,
                selectedShowSeats);

            // Step 11: Return booking confirmation DTO
            var confirmedBooking = await _bookingRepository.GetBookingByIdAsync(savedBooking.Id);
            return MapToDto(confirmedBooking!);
        }

        // Complete Booking Cancellation Workflow:
        // 1. Find the booking.
        // 2. Check whether it exists.
        // 3. Check whether its status is Confirmed.
        // 4. Change the booking status to Cancelled.
        // 5. Change its corresponding ShowSeat statuses to Available.
        // 6. Save all changes using a database transaction.
        // 7. Preserve booking history. Never delete the booking records.
        public async Task<bool> CancelBookingAsync(int id)
        {
            // 1. Find the booking
            var booking = await _bookingRepository.GetBookingByIdAsync(id);

            // 2. Check whether it exists
            if (booking == null)
            {
                return false;
            }

            // 3. Check whether its status is Confirmed
            if (booking.BookingStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This booking is already cancelled.");
            }

            // 4. Change booking status to Cancelled (Never delete record to preserve history!)
            booking.BookingStatus = "Cancelled";

            // 5. Retrieve all ShowSeats for this show and release the seats back to "Available"
            var allShowSeats = await _showRepository.GetShowSeatsAsync(booking.ShowId);
            var showSeatsToRelease = new List<ShowSeat>();

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

            // 6. Save changes using a database transaction
            await _bookingRepository.UpdateBookingWithTransactionAsync(booking, showSeatsToRelease);
            return true;
        }

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
                        Row = bs.Seat != null ? bs.Seat.Row : string.Empty,
                        SeatNumber = bs.Seat != null ? bs.Seat.SeatNumber : string.Empty,
                        SeatType = bs.Seat != null ? bs.Seat.SeatType : string.Empty,
                        Price = bs.Price
                    });
                }
            }

            return new BookingResponseDto
            {
                Id = booking.Id,
                ShowId = booking.ShowId,
                MovieTitle = booking.Show != null && booking.Show.Movie != null ? booking.Show.Movie.Title : string.Empty,
                ScreenName = booking.Show != null && booking.Show.Screen != null ? booking.Show.Screen.ScreenName : string.Empty,
                TheatreName = booking.Show != null && booking.Show.Screen != null && booking.Show.Screen.Theatre != null ? booking.Show.Screen.Theatre.Name : string.Empty,
                StartTime = booking.Show != null ? booking.Show.StartTime : DateTime.MinValue,
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
