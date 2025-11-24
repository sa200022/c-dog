using Ticketing.Domain;
using Ticketing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ticketing.Application;

public class ActivityService
{
    private readonly TicketingDbContext _db;

    public ActivityService(TicketingDbContext db)
    {
        _db = db;
    }

    public async Task<List<Activity>> GetListAsync()
    {
        return await _db.Activities
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Activity?> GetDetailAsync(Guid id)
    {
        return await _db.Activities
            .Include(x => x.Timeslots)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Activity> CreateAsync(Activity entity)
    {
        _db.Activities.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Activity entity)
    {
        _db.Activities.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task ChangeStatusAsync(Activity activity, ActivityStatus status)
    {
        if (status == ActivityStatus.Published)
            activity.Publish();
        else if (status == ActivityStatus.Archived)
            activity.Archive();

        await _db.SaveChangesAsync();
    }
}
