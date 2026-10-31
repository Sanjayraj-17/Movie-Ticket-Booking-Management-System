using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    // Repository implementation for Show database operations
    public class ShowRepository : IShowRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Show>> GetAllShowsAsync()
        {
            return await _context.Shows
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                    .ThenInclude(sc => sc.Theatre)
                .Include(s => s.ShowSeats)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<Show?> GetShowByIdAsync(int id)
        {
            return await _context.Shows
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                    .ThenInclude(sc => sc.Theatre)
                .Include(s => s.ShowSeats)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Show>> GetShowsFilteredAsync(int? movieId, int? theatreId, DateTime? date)
        {
            var query = _context.Shows
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                    .ThenInclude(sc => sc.Theatre)
                .Include(s => s.ShowSeats)
                .AsQueryable();

            if (movieId.HasValue)
            {
                query = query.Where(s => s.MovieId == movieId.Value);
            }

            if (theatreId.HasValue)
            {
                query = query.Where(s => s.Screen.TheatreId == theatreId.Value);
            }

            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                query = query.Where(s => s.StartTime.Date == targetDate);
            }

            return await query.OrderBy(s => s.StartTime).ToListAsync();
        }

        public async Task<bool> HasOverlappingShowAsync(int screenId, DateTime startTime, DateTime endTime, int? excludeShowId = null)
        {
            var query = _context.Shows
                .Where(s => s.ScreenId == screenId);

            if (excludeShowId.HasValue)
            {
                query = query.Where(s => s.Id != excludeShowId.Value);
            }

            return await query.AnyAsync(s => s.StartTime < endTime && s.EndTime > startTime);
        }

        public async Task<Show> AddShowAsync(Show show)
        {
            await _context.Shows.AddAsync(show);
            await _context.SaveChangesAsync();
            return show;
        }

        public async Task UpdateShowAsync(Show show)
        {
            _context.Shows.Update(show);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteShowAsync(Show show)
        {
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();
        }

        public async Task AddShowSeatsBulkAsync(List<ShowSeat> showSeats)
        {
            await _context.ShowSeats.AddRangeAsync(showSeats);
            await _context.SaveChangesAsync();
        }

        // Retrieves all show seats for a given show along with physical seat details
        public async Task<List<ShowSeat>> GetShowSeatsAsync(int showId)
        {
            return await _context.ShowSeats
                .Include(ss => ss.Seat)
                .Where(ss => ss.ShowId == showId)
                .OrderBy(ss => ss.Seat.Row)
                .ThenBy(ss => ss.Seat.SeatNumber)
                .ToListAsync();
        }

        // Retrieves only available seats for a given show
        public async Task<List<ShowSeat>> GetAvailableShowSeatsAsync(int showId)
        {
            return await _context.ShowSeats
                .Include(ss => ss.Seat)
                .Where(ss => ss.ShowId == showId && ss.Status == "Available")
                .OrderBy(ss => ss.Seat.Row)
                .ThenBy(ss => ss.Seat.SeatNumber)
                .ToListAsync();
        }

        // Retrieves only booked seats for a given show
        public async Task<List<ShowSeat>> GetBookedShowSeatsAsync(int showId)
        {
            return await _context.ShowSeats
                .Include(ss => ss.Seat)
                .Where(ss => ss.ShowId == showId && ss.Status == "Booked")
                .OrderBy(ss => ss.Seat.Row)
                .ThenBy(ss => ss.Seat.SeatNumber)
                .ToListAsync();
        }
    }
}
