namespace MovieTicketBooking.DTOs.Seats
{
    // DTO returned to client containing seat details
    public class SeatResponseDto
    {
        public int Id { get; set; }
        public int ScreenId { get; set; }
        public string Row { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string SeatType { get; set; } = string.Empty;
    }
}
