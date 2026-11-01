namespace MovieTicketBooking.Models
{
    // Represents an auditorium or screen hall inside a theatre
    public class Screen
    {
        // Primary Key (Identity)
        public int Id { get; set; }

        // Screen name or identifier (e.g., "Screen 1", "IMAX Hall")
        public string ScreenName { get; set; } = string.Empty;

        // Foreign Key pointing to the parent Theatre
        public int TheatreId { get; set; }

        // Navigation Property: The theatre that this screen belongs to
        public Theatre Theatre { get; set; } = null!;

        // Navigation Property: One screen has many physical seats
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

        // Navigation Property: One screen can host many scheduled shows
        public ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
