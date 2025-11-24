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

    public async Task<Order> CreateOrderAsync(Order order)
    {
        // MVP: CreateOrder = Already Paid
        order.MarkPaid();

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
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
        order.Cancel();
        await _db.SaveChangesAsync();
    }
}
