namespace MovieTicketBooking.Models
{
    // Represents a permanent physical seat located inside a screen hall
    public class Seat
    {
        // Primary Key (Identity)
        public int Id { get; set; }

        // Foreign Key pointing to the Screen where this seat is physically installed
        public int ScreenId { get; set; }

        // Row indicator (e.g., "A", "B", "C")
        public string Row { get; set; } = string.Empty;

        // Seat label (e.g., "A1", "A2", "B10")
        public string SeatNumber { get; set; } = string.Empty;

        // Category of the seat (e.g., "Standard", "Premium", "VIP")
        public string SeatType { get; set; } = string.Empty;

        // Navigation Property: The screen where this seat belongs
        public Screen Screen { get; set; } = null!;

        // Navigation Property: The show-seat instances generated for various shows
        public ICollection<ShowSeat> ShowSeats { get; set; } = new List<ShowSeat>();

        // Navigation Property: The booking records associated with this seat
        public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}
