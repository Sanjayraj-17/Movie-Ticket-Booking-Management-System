namespace MovieTicketBooking.DTOs.Theatres
{
    // DTO received when creating a theatre
    public class CreateTheatreDto
    {
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
