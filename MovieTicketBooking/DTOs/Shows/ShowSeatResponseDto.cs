namespace MovieTicketBooking.DTOs.Shows
{
    // DTO representing the status and price of an individual seat for a show
    public class ShowSeatResponseDto
    {
        public int ShowSeatId { get; set; }
        public int ShowId { get; set; }
        public int SeatId { get; set; }
        public string Row { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string SeatType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty; // "Available" or "Booked"
    }
}
