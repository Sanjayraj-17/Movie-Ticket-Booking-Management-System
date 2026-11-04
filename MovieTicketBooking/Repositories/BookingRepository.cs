
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    // Repository implementation for Booking operations
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL BOOKINGS (ADMIN)
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

        // GET BOOKINGS BY CUSTOMER USER ID
        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
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

        // GET BOOKING BY ID
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

        // CREATE BOOKING USING DATABASE TRANSACTION
        public async Task<Booking> CreateBookingWithTransactionAsync(
            Booking booking,
            List<BookingSeat> bookingSeats,
            List<ShowSeat> showSeatsToUpdate)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Save main Booking record
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                // 2. Assign BookingId to each BookingSeat
                foreach (var bs in bookingSeats)
                {
                    bs.BookingId = booking.Id;
                }

                await _context.BookingSeats.AddRangeAsync(bookingSeats);

                // 3. Mark selected ShowSeats as Booked
                foreach (var ss in showSeatsToUpdate)
                {
                    ss.Status = "Booked";
                    _context.ShowSeats.Update(ss);
                }

                // 4. Save all changes
                await _context.SaveChangesAsync();

                // 5. Commit transaction
                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // UPDATE BOOKING USING DATABASE TRANSACTION
        public async Task UpdateBookingWithTransactionAsync(
            Booking booking,
            List<ShowSeat> showSeatsToUpdate)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

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
