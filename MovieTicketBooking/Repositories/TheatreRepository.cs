using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    // Repository implementation for Theatre database operations
    public class TheatreRepository : ITheatreRepository
    {
        private readonly ApplicationDbContext _context;

        public TheatreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Theatre>> GetAllTheatresAsync()
        {
            return await _context.Theatres.ToListAsync();
        }

        public async Task<Theatre?> GetTheatreByIdAsync(int id)
        {
            return await _context.Theatres.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Theatre> AddTheatreAsync(Theatre theatre)
        {
            await _context.Theatres.AddAsync(theatre);
            await _context.SaveChangesAsync();
            return theatre;
        }

        public async Task UpdateTheatreAsync(Theatre theatre)
        {
            _context.Theatres.Update(theatre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTheatreAsync(Theatre theatre)
        {
            _context.Theatres.Remove(theatre);
            await _context.SaveChangesAsync();
        }
    }
}
