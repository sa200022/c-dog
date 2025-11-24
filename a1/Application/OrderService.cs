using Ticketing.Domain;
using Ticketing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ticketing.Application;

public class OrderService
{
    private readonly TicketingDbContext _db;

    public OrderService(TicketingDbContext db)
    {
        _db = db;
    }

    public async Task<Order> CreateOrderAsync(
        Guid activityId,
        Guid timeslotId,
        string? userId,
        string customerEmail,
        string customerName,
        string currency,
        List<OrderItem> items,
        bool markAsPaid = true)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var activity = await _db.Activities.FirstOrDefaultAsync(a => a.Id == activityId);
        if (activity is null || activity.Status != ActivityStatus.Published)
            throw new InvalidOperationException("Activity not available.");

        var timeslot = await _db.Timeslots
            .Include(t => t.Seats)
            .FirstOrDefaultAsync(t => t.Id == timeslotId && t.ActivityId == activityId);
        if (timeslot is null || timeslot.Status != TimeslotStatus.OnSale)
            throw new InvalidOperationException("Timeslot not available.");

        if (items.Any(i => i.TimeslotId != timeslotId))
            throw new InvalidOperationException("Items must match the order timeslot.");

        var seatItems = items.Where(i => i.SeatId.HasValue).ToList();
        if (seatItems.Any(i => i.Quantity != 1))
            throw new InvalidOperationException("Seat-based items must have quantity = 1.");

        if (seatItems.Any())
        {
            var seatIds = seatItems.Select(i => i.SeatId!.Value).ToList();
            var seats = timeslot.Seats.Where(s => seatIds.Contains(s.Id)).ToList();

            if (seats.Count != seatIds.Count || seats.Any(s => s.Status != SeatStatus.Available))
                throw new InvalidOperationException("Some seats are not available.");

            foreach (var seat in seats)
                seat.MarkSold();
        }
        else
        {
            var totalQty = items.Sum(i => i.Quantity);
            if (!timeslot.Capacity.HasValue)
                throw new InvalidOperationException("Timeslot has no capacity for non-seat orders.");
            timeslot.DecreaseCapacity(totalQty);
        }

        var order = new Order(
            Guid.NewGuid(),
            activityId,
            timeslotId,
            userId,
            customerEmail,
            customerName,
            currency,
            items);

        if (markAsPaid)
            order.MarkPaid();

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return order;
    }

    public async Task<Order?> GetDetailAsync(Guid id)
    {
        return await _db.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task CancelAsync(Order order)
    {
        if (order.Status is not (OrderStatus.PendingPayment or OrderStatus.Paid))
            throw new InvalidOperationException("Only pending or paid orders can be cancelled.");

        var seatItemIds = order.Items.Where(i => i.SeatId.HasValue).Select(i => i.SeatId!.Value).ToList();
        await using var tx = await _db.Database.BeginTransactionAsync();

        if (seatItemIds.Any())
        {
            var seats = await _db.Seats
                .Where(s => seatItemIds.Contains(s.Id))
                .ToListAsync();

            if (seats.Count != seatItemIds.Count)
                throw new InvalidOperationException("Some seats were not found for cancellation.");

            foreach (var seat in seats)
                seat.Release();
        }
        else
        {
            var qty = order.Items.Sum(i => i.Quantity);
            var timeslot = await _db.Timeslots.FirstOrDefaultAsync(t => t.Id == order.TimeslotId);
            if (timeslot is null)
                throw new InvalidOperationException("Timeslot not found for order.");

            timeslot.IncreaseCapacity(qty);
        }

        order.Cancel();
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
