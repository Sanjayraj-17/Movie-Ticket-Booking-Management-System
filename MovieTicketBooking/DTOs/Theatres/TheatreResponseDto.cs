namespace MovieTicketBooking.DTOs.Theatres
{
    // DTO returned to client containing theatre details
    public class TheatreResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
