
namespace MovieTicketBooking.DTOs.Movies
{
    // DTO returned to the client containing movie details
    public class MovieResponseDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public int DurationInMinutes { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string PosterUrl { get; set; } = string.Empty;
    }
}
