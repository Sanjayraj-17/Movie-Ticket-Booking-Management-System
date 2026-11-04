
namespace MovieTicketBooking.Models
{
    // Represents a ticket booking reservation made by a customer
    public class Booking
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key: Which show was booked
        public int ShowId { get; set; }

        // Foreign Key: Which registered user owns this booking
        public int UserId { get; set; }

        // Customer contact information
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        // Booking details
        public DateTime BookingTime { get; set; }
        public decimal TotalAmount { get; set; }

        // Booking status
        public string BookingStatus { get; set; } = "Confirmed";

        // Navigation Property: The show that was booked
        public Show Show { get; set; } = null!;

        // Navigation Property: The user who owns this booking
        public User User { get; set; }

        // Navigation Property: Seats included in this booking
        public ICollection<BookingSeat> BookingSeats { get; set; }
            = new List<BookingSeat>();
    }
}
