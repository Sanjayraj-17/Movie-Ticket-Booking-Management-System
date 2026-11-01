namespace MovieTicketBooking.DTOs.Screens
{
    // DTO received when creating a screen inside a theatre
    public class CreateScreenDto
    {
        public string ScreenName { get; set; } = string.Empty;
        public int TheatreId { get; set; }
    }
}
