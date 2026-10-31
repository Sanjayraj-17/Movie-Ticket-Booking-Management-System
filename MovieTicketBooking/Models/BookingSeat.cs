namespace MovieTicketBooking.Models
{
    // Represents an individual seat booked within a booking reservation
    public class BookingSeat
    {
        // Primary Key (Identity)
        public int Id { get; set; }

        // Foreign Key: Which booking this seat item belongs to
        public int BookingId { get; set; }

        // Foreign Key: Which physical seat was reserved
        public int SeatId { get; set; }

        // The price charged for this seat at the time of booking
        public decimal Price { get; set; }

        // Navigation Property: The parent booking
        public Booking Booking { get; set; } = null!;

        // Navigation Property: The physical seat details
        public Seat Seat { get; set; } = null!;
    }
}
