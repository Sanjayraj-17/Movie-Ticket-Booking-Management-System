namespace MovieTicketBooking.DTOs.Shows
{
    // DTO providing high-level seat counts and the full seat grid for a show
    public class ShowSeatAvailabilitySummaryDto
    {
        public int ShowId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeatsCount { get; set; }
        public int BookedSeatsCount { get; set; }
        public List<ShowSeatResponseDto> Seats { get; set; } = new List<ShowSeatResponseDto>();
    }
}
