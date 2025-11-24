using Ticketing.Domain;
using Ticketing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ticketing.Application;

public class TimeslotService
{
    private readonly TicketingDbContext _db;

    public TimeslotService(TicketingDbContext db)
    {
        _db = db;
    }

    public async Task<List<Timeslot>> GetByActivityAsync(Guid activityId)
    {
        return await _db.Timeslots
            .Where(x => x.ActivityId == activityId)
            .Include(x => x.Seats)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Timeslot?> GetByIdAsync(Guid id)
    {
        return await _db.Timeslots
            .Include(x => x.Seats)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Timeslot> CreateAsync(Timeslot entity)
    {
        _db.Timeslots.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Timeslot entity)
    {
        _db.Timeslots.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task ChangeStatusAsync(Timeslot timeslot, TimeslotStatus status)
    {
        timeslot.ChangeStatus(status);
        await _db.SaveChangesAsync();
    }
}
