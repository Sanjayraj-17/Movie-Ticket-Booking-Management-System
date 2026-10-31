namespace MovieTicketBooking.DTOs.Seats
{
    // DTO received when creating a single seat
    public class CreateSeatDto
    {
        public int ScreenId { get; set; }
        public string Row { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string SeatType { get; set; } = "Standard";
    }
}
