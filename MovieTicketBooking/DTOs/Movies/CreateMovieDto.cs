namespace MovieTicketBooking.DTOs.Movies
{
    // DTO received from the client when creating a new movie
    public class CreateMovieDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
