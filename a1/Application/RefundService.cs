using Ticketing.Domain;
using Ticketing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ticketing.Application;

public class RefundService
{
    private readonly TicketingDbContext _db;

    public RefundService(TicketingDbContext db)
    {
        _db = db;
    }

    public async Task<Refund?> GetByIdAsync(Guid id)
    {
        return await _db.Refunds
            .Include(r => r.Order)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Refund>> GetAllAsync()
    {
        return await _db.Refunds
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Refund> RequestAsync(Order order, decimal amount, string reason)
    {
        if (order.Status is not (OrderStatus.Paid or OrderStatus.PartiallyRefunded))
            throw new InvalidOperationException("Only paid orders can request refunds.");

        var refundable = order.TotalAmount - order.RefundedAmount;
        if (amount <= 0 || amount > refundable)
            throw new InvalidOperationException("Invalid refund amount.");

        var refund = new Refund(Guid.NewGuid(), order.Id, amount, reason);

        _db.Refunds.Add(refund);
        await _db.SaveChangesAsync();
        return refund;
    }

    public async Task ApproveAsync(Refund refund)
    {
        if (refund.Status != RefundStatus.Requested)
            throw new InvalidOperationException("Refund is not in requested status.");

        refund.Approve();
        await _db.SaveChangesAsync();
    }

    public async Task RejectAsync(Refund refund)
    {
        if (refund.Status != RefundStatus.Requested)
            throw new InvalidOperationException("Refund is not in requested status.");

        refund.Reject();
        await _db.SaveChangesAsync();
    }

    public async Task CompleteAsync(Refund refund)
    {
        if (refund.Status != RefundStatus.Approved)
            throw new InvalidOperationException("Refund must be approved before completion.");

        var order = refund.Order ?? await _db.Orders.FirstOrDefaultAsync(o => o.Id == refund.OrderId);
        if (order is null)
            throw new InvalidOperationException("Order not found for refund.");

        order.ApplyRefund(refund.Amount);
        refund.Complete();
        await _db.SaveChangesAsync();
    }
}
