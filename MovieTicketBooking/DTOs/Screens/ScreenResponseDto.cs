namespace MovieTicketBooking.DTOs.Screens
{
    // DTO returned to client containing screen details
    public class ScreenResponseDto
    {
        public int Id { get; set; }
        public string ScreenName { get; set; } = string.Empty;
        public int TheatreId { get; set; }
        public string TheatreName { get; set; } = string.Empty;
    }
}
