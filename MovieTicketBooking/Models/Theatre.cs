namespace MovieTicketBooking.Models
{
    // Represents a physical cinema theatre building
    public class Theatre
    {
        // Primary Key (Identity)
        public int Id { get; set; }

        // Theatre brand/name (e.g., "PVR Cinemas", "AMC Theatre")
        public string Name { get; set; } = string.Empty;

        // City where the theatre is located (e.g., "New York", "Mumbai")
        public string City { get; set; } = string.Empty;

        // Full street address
        public string Address { get; set; } = string.Empty;

        // Navigation Property: One theatre can have multiple cinema screen halls
        public ICollection<Screen> Screens { get; set; } = new List<Screen>();
    }
}
