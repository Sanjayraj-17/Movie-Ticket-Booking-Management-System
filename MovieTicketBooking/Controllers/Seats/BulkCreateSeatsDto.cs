namespace MovieTicketBooking.DTOs.Seats
{
    // DTO received when creating seats in bulk for a screen row
    public class BulkCreateSeatsDto
    {
        public int ScreenId { get; set; }
        // The row identifier, e.g. "A"
        public string Row { get; set; } = string.Empty;
        // Total number of seats in this row, e.g. 10 (will generate A1, A2... A10)
        public int TotalSeats { get; set; }
        // Seat category: "Standard", "Premium", or "VIP"
        public string SeatType { get; set; } = "Standard";
    }
}
