namespace MovieTicketBooking.DTOs.Theatres
{
    // DTO received when updating a theatre
    public class UpdateTheatreDto
    {
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
