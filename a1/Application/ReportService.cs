using Microsoft.EntityFrameworkCore;
using Ticketing.Domain;
using Ticketing.Infrastructure;

namespace Ticketing.Application;

public class ReportService
{
    private readonly TicketingDbContext _db;

    public ReportService(TicketingDbContext db)
    {
        _db = db;
    }

    public async Task<List<ActivitySalesItem>> GetActivitySalesAsync(Guid? activityId = null)
    {
        var query = _db.Orders
            .AsNoTracking()
            .Where(o =>
                o.Status == OrderStatus.Paid ||
                o.Status == OrderStatus.PartiallyRefunded ||
                o.Status == OrderStatus.Refunded);

        if (activityId.HasValue)
            query = query.Where(o => o.ActivityId == activityId.Value);

        var grouped = await query
            .GroupBy(o => new { o.ActivityId })
            .Select(g => new ActivitySalesItem(
                g.Key.ActivityId,
                string.Empty,
                0m,
                g.Count(),
                g.Sum(x => x.TotalAmount),
                g.Sum(x => x.RefundedAmount)))
            .ToListAsync();

        var activityNames = await _db.Activities
            .AsNoTracking()
            .Where(a => grouped.Select(g => g.ActivityId).Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => a.Name);

        return grouped
            .Select(g =>
            {
                var name = activityNames.GetValueOrDefault(g.ActivityId) ?? "Unknown Activity";
                var net = g.GrossAmount - g.RefundedAmount;
                return g with { ActivityName = name, NetAmount = net };
            })
            .ToList();
    }

    public async Task<List<TimeslotInventoryItem>> GetTimeslotInventoryAsync(Guid activityId)
    {
        var timeslots = await _db.Timeslots
            .AsNoTracking()
            .Where(t => t.ActivityId == activityId)
            .Select(t => new TimeslotInventoryItem(
                t.Id,
                t.ActivityId,
                t.StartTime,
                t.EndTime,
                t.Capacity,
                t.RemainingCapacity,
                null,
                null))
            .ToListAsync();

        var timeslotIds = timeslots.Select(t => t.TimeslotId).ToList();
        var seatStats = await _db.Seats
            .AsNoTracking()
            .Where(s => timeslotIds.Contains(s.TimeslotId))
            .GroupBy(s => s.TimeslotId)
            .Select(g => new
            {
                TimeslotId = g.Key,
                Available = g.Count(s => s.Status == SeatStatus.Available),
                Sold = g.Count(s => s.Status == SeatStatus.Sold)
            })
            .ToListAsync();

        var seatLookup = seatStats.ToDictionary(x => x.TimeslotId, x => x);

        return timeslots
            .Select(t =>
            {
                if (seatLookup.TryGetValue(t.TimeslotId, out var seat))
                {
                    return t with
                    {
                        SeatsAvailable = seat.Available,
                        SeatsSold = seat.Sold
                    };
                }

                return t;
            })
            .ToList();
    }
}

public sealed record ActivitySalesItem(
    Guid ActivityId,
    string ActivityName,
    decimal NetAmount,
    int OrderCount,
    decimal GrossAmount,
    decimal RefundedAmount);

public sealed record TimeslotInventoryItem(
    Guid TimeslotId,
    Guid ActivityId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int? Capacity,
    int RemainingCapacity,
    int? SeatsAvailable,
    int? SeatsSold);
