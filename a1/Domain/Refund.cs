using System;

namespace Ticketing.Domain;

public class Refund
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public decimal Amount { get; private set; }
    public string Reason { get; private set; } = null!;

    public RefundStatus Status { get; private set; }

    public DateTimeOffset RequestedAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }

    private Refund() { }

    public Refund(Guid id, Guid orderId, decimal amount, string reason)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Id = id;
        OrderId = orderId;
        Amount = amount;
        Reason = reason;
        Status = RefundStatus.Requested;
        RequestedAt = DateTimeOffset.UtcNow;
    }

    public void Approve()
    {
        Status = RefundStatus.Approved;
        ProcessedAt = DateTimeOffset.UtcNow;
    }

    public void Reject()
    {
        Status = RefundStatus.Rejected;
        ProcessedAt = DateTimeOffset.UtcNow;
    }

    public void Complete()
    {
        Status = RefundStatus.Completed;
        ProcessedAt = DateTimeOffset.UtcNow;
    }
}
