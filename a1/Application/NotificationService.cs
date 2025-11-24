using Ticketing.Domain;
using Ticketing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ticketing.Application;

public class NotificationService
{
    private readonly TicketingDbContext _db;

    public NotificationService(TicketingDbContext db)
    {
        _db = db;
    }

    public async Task<List<ItineraryNotification>> GetByOrderAsync(Guid orderId)
    {
        return await _db.ItineraryNotifications
            .Where(x => x.OrderId == orderId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ItineraryNotification> CreateAsync(ItineraryNotification n)
    {
        _db.ItineraryNotifications.Add(n);
        await _db.SaveChangesAsync();
        return n;
    }

    public async Task MarkSentAsync(ItineraryNotification n)
    {
        n.MarkSent();
        await _db.SaveChangesAsync();
    }

    public async Task MarkFailedAsync(ItineraryNotification n)
    {
        n.MarkFailed();
        await _db.SaveChangesAsync();
    }
}
