namespace MovieTicketBooking.DTOs.Shows
{
    // DTO received from the client when creating a new show
    public class CreateShowDto
    {
        public int MovieId { get; set; }
        public int ScreenId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
    }
}
