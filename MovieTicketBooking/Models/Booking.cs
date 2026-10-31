namespace MovieTicketBooking.Models
{
    // Represents a ticket booking reservation made by a customer
    public class Booking
    {
        // Primary Key (Identity)
        public int Id { get; set; }

        // Foreign Key: Which show was booked
        public int ShowId { get; set; }

        // Customer contact information
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        // When the customer placed the booking
        public DateTime BookingTime { get; set; }

        // Total calculated price of all seats booked in this transaction
        public decimal TotalAmount { get; set; }

        // Status of the booking: "Confirmed" or "Cancelled"
        public string BookingStatus { get; set; } = "Confirmed";

        // Navigation Property: The show that was booked
        public Show Show { get; set; } = null!;

        // Navigation Property: The list of seats included in this booking
        public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}
