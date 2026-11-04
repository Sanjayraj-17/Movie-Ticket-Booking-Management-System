namespace MovieTicketBooking.Models
{
    // Represents a movie in the system
    public class Movie
    {
        // Primary Key (Identity)
        public int Id { get; set; }

        // Movie title (e.g., "Inception")
        public string Title { get; set; } = string.Empty;

        public string PosterUrl { get; set; } = string.Empty;

        // Brief synopsis or plot summary
        public string Description { get; set; } = string.Empty;

        // Language of the movie (e.g., "English", "Hindi", "Tamil")
        public string Language { get; set; } = string.Empty;

        // Genre category (e.g., "Action", "Sci-Fi", "Comedy")
        public string Genre { get; set; } = string.Empty;

        // Total running time in minutes (e.g., 148)
        public int DurationInMinutes { get; set; }

        // The date when the movie released
        public DateTime ReleaseDate { get; set; }

        // Navigation Property: One movie can have many scheduled shows
        public ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
