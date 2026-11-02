using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data
{
    // ApplicationDbContext acts as the bridge between our C# entity models and Microsoft SQL Server.
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // 1. DBSET PROPERTIES (TABLES IN DATABASE)
        // ==========================================
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowSeat> ShowSeats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<User> Users { get; set; }
        // ==========================================
        // 2. FLUENT API CONFIGURATION
        // ==========================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------------------------------
            // 1. MOVIE CONFIGURATION
            // ------------------------------------------
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Description).HasMaxLength(1000);
                entity.Property(m => m.Language).IsRequired().HasMaxLength(50);
                entity.Property(m => m.Genre).IsRequired().HasMaxLength(50);
            });

            // ------------------------------------------
            // 2. THEATRE CONFIGURATION
            // ------------------------------------------
            modelBuilder.Entity<Theatre>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(150);
                entity.Property(t => t.City).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Address).IsRequired().HasMaxLength(300);
            });

            // ------------------------------------------
            // 3. SCREEN CONFIGURATION
            // ------------------------------------------
            modelBuilder.Entity<Screen>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.ScreenName).IsRequired().HasMaxLength(50);

                // Relationship: Theatre (1) -> Screens (Many)
                // If a theatre is deleted, its screens are deleted (Cascade)
                entity.HasOne(s => s.Theatre)
                      .WithMany(t => t.Screens)
                      .HasForeignKey(s => s.TheatreId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------------------------------
            // 4. SEAT CONFIGURATION
            // ------------------------------------------
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Row).IsRequired().HasMaxLength(5);
                entity.Property(s => s.SeatNumber).IsRequired().HasMaxLength(10);
                entity.Property(s => s.SeatType).IsRequired().HasMaxLength(50);

                // Unique Constraint: In the same screen, no two seats can share the same SeatNumber (e.g. only one "A1" per screen)
                entity.HasIndex(s => new { s.ScreenId, s.SeatNumber }).IsUnique();

                // Relationship: Screen (1) -> Seats (Many)
                // If a screen is deleted, its seats are deleted (Cascade)
                entity.HasOne(s => s.Screen)
                      .WithMany(sc => sc.Seats)
                      .HasForeignKey(s => s.ScreenId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------------------------------
            // 5. SHOW CONFIGURATION
            // ------------------------------------------
            modelBuilder.Entity<Show>(entity =>
            {
                entity.HasKey(sh => sh.Id);

                // Decimal precision for base ticket price
                entity.Property(sh => sh.Price).HasPrecision(10, 2);

                // Relationship: Movie (1) -> Shows (Many)
                // Restrict: Prevent deleting a movie if it has scheduled shows
                entity.HasOne(sh => sh.Movie)
                      .WithMany(m => m.Shows)
                      .HasForeignKey(sh => sh.MovieId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship: Screen (1) -> Shows (Many)
                // Restrict: Prevent deleting a screen if it has scheduled shows
                entity.HasOne(sh => sh.Screen)
                      .WithMany(sc => sc.Shows)
                      .HasForeignKey(sh => sh.ScreenId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ------------------------------------------
            // 6. SHOWSEAT CONFIGURATION
            // ------------------------------------------
            modelBuilder.Entity<ShowSeat>(entity =>
            {
                entity.HasKey(ss => ss.Id);
                entity.Property(ss => ss.Status).IsRequired().HasMaxLength(20);
                entity.Property(ss => ss.Price).HasPrecision(10, 2);

                // Unique Constraint: A physical seat can appear only once in a given show's inventory
                entity.HasIndex(ss => new { ss.ShowId, ss.SeatId }).IsUnique();

                // Relationship: Show (1) -> ShowSeats (Many)
                // If a show is cancelled/deleted, its show seat inventory is deleted (Cascade)
                entity.HasOne(ss => ss.Show)
                      .WithMany(sh => sh.ShowSeats)
                      .HasForeignKey(ss => ss.ShowId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relationship: Seat (1) -> ShowSeats (Many)
                // Restrict: To avoid multiple cascade paths in SQL Server
                entity.HasOne(ss => ss.Seat)
                      .WithMany(s => s.ShowSeats)
                      .HasForeignKey(ss => ss.SeatId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ------------------------------------------
            // 7. BOOKING CONFIGURATION
            // ------------------------------------------
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(b => b.CustomerEmail).IsRequired().HasMaxLength(150);
                entity.Property(b => b.CustomerPhone).IsRequired().HasMaxLength(20);
                entity.Property(b => b.BookingStatus).IsRequired().HasMaxLength(20);
                entity.Property(b => b.TotalAmount).HasPrecision(10, 2);

                // Relationship: Show (1) -> Bookings (Many)
                // Restrict: Cannot delete a show that has active customer bookings
                entity.HasOne(b => b.Show)
                      .WithMany(sh => sh.Bookings)
                      .HasForeignKey(b => b.ShowId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ------------------------------------------
            // 8. BOOKINGSEAT CONFIGURATION
            // ------------------------------------------
            modelBuilder.Entity<BookingSeat>(entity =>
            {
                entity.HasKey(bs => bs.Id);
                entity.Property(bs => bs.Price).HasPrecision(10, 2);

                // Unique Constraint: A seat can only be booked once inside the same booking record
                entity.HasIndex(bs => new { bs.BookingId, bs.SeatId }).IsUnique();

                // Relationship: Booking (1) -> BookingSeats (Many)
                // If a booking record is removed, its itemized seat lines are deleted (Cascade)
                entity.HasOne(bs => bs.Booking)
                      .WithMany(b => b.BookingSeats)
                      .HasForeignKey(bs => bs.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relationship: Seat (1) -> BookingSeats (Many)
                // Restrict: To avoid multiple cascade paths in SQL Server
                entity.HasOne(bs => bs.Seat)
                      .WithMany(s => s.BookingSeats)
                      .HasForeignKey(bs => bs.SeatId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
