namespace MovieTicketBooking.DTOs.Shows
{
    // DTO returned to the client containing complete show information
    public class ShowResponseDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public int ScreenId { get; set; }
        public string ScreenName { get; set; } = string.Empty;
        public int TheatreId { get; set; }
        public string TheatreName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public int TotalSeats { get; set; }
    }
}
