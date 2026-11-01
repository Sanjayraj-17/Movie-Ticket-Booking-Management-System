using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    // Repository implementation for Seat database operations
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Seat>> GetSeatsByScreenIdAsync(int screenId)
        {
            return await _context.Seats
                .Where(s => s.ScreenId == screenId)
                .OrderBy(s => s.Row)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }

        public async Task<Seat?> GetSeatByIdAsync(int id)
        {
            return await _context.Seats.FirstOrDefaultAsync(s => s.Id == id);
        }

        // Checks if a seat number already exists in a given screen
        public async Task<bool> SeatExistsAsync(int screenId, string seatNumber)
        {
            return await _context.Seats.AnyAsync(s => s.ScreenId == screenId && s.SeatNumber == seatNumber);
        }

        public async Task<Seat> AddSeatAsync(Seat seat)
        {
            await _context.Seats.AddAsync(seat);
            await _context.SaveChangesAsync();
            return seat;
        }

        // Inserts a list of seats in a single batch
        public async Task AddSeatsBulkAsync(List<Seat> seats)
        {
            await _context.Seats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSeatAsync(Seat seat)
        {
            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
        }
    }
}
