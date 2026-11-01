using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    // Repository implementation for Booking operations using EF Core database transactions
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Show)
                    .ThenInclude(sh => sh.Movie)
                .Include(b => b.Show)
                    .ThenInclude(sh => sh.Screen)
                        .ThenInclude(sc => sc.Theatre)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .OrderByDescending(b => b.BookingTime)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Show)
                    .ThenInclude(sh => sh.Movie)
                .Include(b => b.Show)
                    .ThenInclude(sh => sh.Screen)
                        .ThenInclude(sc => sc.Theatre)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        // Executes booking creation, seat items creation, and show-seat status update inside a database transaction
        public async Task<Booking> CreateBookingWithTransactionAsync(
            Booking booking,
            List<BookingSeat> bookingSeats,
            List<ShowSeat> showSeatsToUpdate)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Save main Booking record
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                // 2. Assign foreign key BookingId to each booking seat
                foreach (var bs in bookingSeats)
                {
                    bs.BookingId = booking.Id;
                }
                await _context.BookingSeats.AddRangeAsync(bookingSeats);

                // 3. Mark ShowSeat records as "Booked"
                foreach (var ss in showSeatsToUpdate)
                {
                    ss.Status = "Booked";
                    _context.ShowSeats.Update(ss);
                }

                // 4. Save all changes
                await _context.SaveChangesAsync();

                // 5. Commit database transaction
                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                // Roll back any partial database writes if an error occurs
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Updates booking and corresponding ShowSeat records inside a database transaction
        public async Task UpdateBookingWithTransactionAsync(Booking booking, List<ShowSeat> showSeatsToUpdate)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Bookings.Update(booking);

                foreach (var ss in showSeatsToUpdate)
                {
                    _context.ShowSeats.Update(ss);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
