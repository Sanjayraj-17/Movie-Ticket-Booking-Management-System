namespace MovieTicketBooking.Models
{
    // Represents a scheduled movie screening at a specific screen on a specific date/time
    public class Show
    {
        // Primary Key (Identity)
        public int Id { get; set; }

        // Foreign Key: Which movie is being played
        public int MovieId { get; set; }

        // Foreign Key: In which screen auditorium is the movie being played
        public int ScreenId { get; set; }

        // The exact date and time when the show starts
        public DateTime StartTime { get; set; }

        // The date and time when the show ends
        public DateTime EndTime { get; set; }

        // Base price of a ticket for this show
        public decimal Price { get; set; }

        // Navigation Property: The movie details
        public Movie Movie { get; set; } = null!;

        // Navigation Property: The screen auditorium details
        public Screen Screen { get; set; } = null!;

        // Navigation Property: All seat inventory records generated for this show
        public ICollection<ShowSeat> ShowSeats { get; set; } = new List<ShowSeat>();

        // Navigation Property: All bookings made for this show
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
