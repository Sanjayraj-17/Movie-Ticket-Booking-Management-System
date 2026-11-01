namespace MovieTicketBooking.Models
{
    // Represents the availability status and price of a specific physical seat for a specific show
    public class ShowSeat
    {
        // Primary Key (Identity)
        public int Id { get; set; }

        // Foreign Key: Which show this seat availability belongs to
        public int ShowId { get; set; }

        // Foreign Key: Which physical seat inside the screen this represents
        public int SeatId { get; set; }

        // Status of the seat for this show: "Available" or "Booked"
        public string Status { get; set; } = "Available";

        // The price for this specific seat in this show (supports different seat types)
        public decimal Price { get; set; }

        // Navigation Property: The show details
        public Show Show { get; set; } = null!;

        // Navigation Property: The physical seat details
        public Seat Seat { get; set; } = null!;
    }
}
