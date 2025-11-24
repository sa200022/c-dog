using Ticketing.Domain;
using Ticketing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ticketing.Application;

public class SeatService
{
    private readonly TicketingDbContext _db;

    public SeatService(TicketingDbContext db)
    {
        _db = db;
    }

    public async Task<List<Seat>> GetByTimeslotAsync(Guid timeslotId)
    {
        return await _db.Seats
            .Where(x => x.TimeslotId == timeslotId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task MarkSoldAsync(List<Seat> seats)
    {
        foreach (var s in seats)
        {
            s.MarkSold();
        }

        await _db.SaveChangesAsync();
    }

    public async Task ReleaseAsync(List<Seat> seats)
    {
        foreach (var s in seats)
            s.Release();

        await _db.SaveChangesAsync();
    }
}
