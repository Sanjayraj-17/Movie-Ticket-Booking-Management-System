namespace MovieTicketBooking.DTOs.Bookings
{
    // DTO received from the client when placing a ticket booking reservation
    public class CreateBookingDto
    {
        public int ShowId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        // List of physical Seat IDs requested by the customer
        public List<int> SeatIds { get; set; } = new List<int>();
    }
}
