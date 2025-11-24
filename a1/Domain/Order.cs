using System;
using System.Collections.Generic;
using System.Linq;

namespace Ticketing.Domain;

public class Order
{
    public Guid Id { get; private set; }

    public string OrderNumber { get; private set; } = null!;

    public string? UserId { get; private set; }
    public string CustomerEmail { get; private set; } = null!;
    public string CustomerName { get; private set; } = null!;

    public Guid ActivityId { get; private set; }
    public Guid TimeslotId { get; private set; }

    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = "TWD";

    public OrderStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public DateTimeOffset? RefundedAt { get; private set; }

    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    private Order() { }

    public Order(
        Guid id,
        string orderNumber,
        string? userId,
        string customerEmail,
        string customerName,
        Guid activityId,
        Guid timeslotId,
        string currency)
    {
        Id = id;
        OrderNumber = orderNumber;
        UserId = userId;
        CustomerEmail = customerEmail;
        CustomerName = customerName;
        ActivityId = activityId;
        TimeslotId = timeslotId;
        Currency = currency;
        Status = OrderStatus.PendingPayment;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void AddItem(Guid? seatId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        var item = new OrderItem(
            Guid.NewGuid(),
            Id,
            seatId,
            quantity,
            unitPrice);

        Items.Add(item);
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.LineAmount);
    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.PendingPayment)
            throw new InvalidOperationException("Only pending orders can be marked as paid.");

        Status = OrderStatus.Paid;
        PaidAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            return;

        if (Status is OrderStatus.Paid or OrderStatus.PendingPayment)
        {
            Status = OrderStatus.Cancelled;
            CancelledAt = DateTimeOffset.UtcNow;
        }
        else
        {
            throw new InvalidOperationException($"Cannot cancel order in status {Status}.");
        }
    }

    public void MarkRefunded(bool partial)
    {
        if (partial)
            Status = OrderStatus.PartiallyRefunded;
        else
            Status = OrderStatus.Refunded;

        RefundedAt = DateTimeOffset.UtcNow;
    }
}

public class OrderItem
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public Guid? SeatId { get; private set; }

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineAmount { get; private set; }

    private OrderItem() { }

    public OrderItem(
        Guid id,
        Guid orderId,
        Guid? seatId,
        int quantity,
        decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        Id = id;
        OrderId = orderId;
        SeatId = seatId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        LineAmount = unitPrice * quantity;
    }
}
