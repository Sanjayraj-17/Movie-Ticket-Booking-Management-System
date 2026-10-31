namespace MovieTicketBooking.DTOs.Shows
{
    // DTO received from the client when updating show timings or price
    public class UpdateShowDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
    }
}
