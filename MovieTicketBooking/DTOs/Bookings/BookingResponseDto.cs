namespace MovieTicketBooking.DTOs.Bookings
{
    // DTO returned upon successful booking confirmation
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public int ShowId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public string TheatreName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime BookingTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string BookingStatus { get; set; } = string.Empty; // "Confirmed" or "Cancelled"
        public List<BookingSeatResponseDto> Seats { get; set; } = new List<BookingSeatResponseDto>();
    }
}
