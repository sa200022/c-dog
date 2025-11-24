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

    public async Task<Refund> RequestAsync(Refund refund)
    {
        _db.Refunds.Add(refund);
        await _db.SaveChangesAsync();
        return refund;
    }

    public async Task ApproveAsync(Refund refund)
    {
        refund.Approve();
        await _db.SaveChangesAsync();
    }

    public async Task RejectAsync(Refund refund)
    {
        refund.Reject();
        await _db.SaveChangesAsync();
    }

    public async Task CompleteAsync(Refund refund)
    {
        refund.Complete();
        await _db.SaveChangesAsync();
    }
}
