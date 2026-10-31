namespace MovieTicketBooking.DTOs.Bookings
{
    // DTO representing an individual seat item inside a booking confirmation
    public class BookingSeatResponseDto
    {
        public int SeatId { get; set; }
        public string Row { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string SeatType { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
