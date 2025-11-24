using System;
using System.Collections.Generic;
using System.Linq;

namespace Ticketing.Domain;

public class Order
{
    public Guid Id { get; private set; }

    public Guid ActivityId { get; private set; }
    public Guid TimeslotId { get; private set; }

    public string OrderNumber { get; private set; } = null!;

    public string? UserId { get; private set; }
    public string CustomerEmail { get; private set; } = null!;
    public string CustomerName { get; private set; } = null!;

    public string Currency { get; private set; } = "TWD";

    public decimal TotalAmount { get; private set; }
    public decimal RefundedAmount { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public DateTimeOffset? RefundedAt { get; private set; }

    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    private Order() { }

    public Order(
        Guid id,
        Guid activityId,
        Guid timeslotId,
        string? userId,
        string customerEmail,
        string customerName,
        string currency,
        IEnumerable<OrderItem> items)
    {
        Id = id;
        ActivityId = activityId;
        TimeslotId = timeslotId;
        OrderNumber = GenerateOrderNumber(id);
        UserId = userId;
        CustomerEmail = customerEmail;
        CustomerName = customerName;
        Currency = string.IsNullOrWhiteSpace(currency) ? "TWD" : currency;

        Items = items.ToList();
        if (Items.Count == 0)
            throw new InvalidOperationException("Order must contain at least one item.");

        if (Items.Any(x => x.TimeslotId != TimeslotId))
            throw new InvalidOperationException("All order items must belong to the same timeslot as the order.");

        RecalculateTotal();

        RefundedAmount = 0;
        Status = OrderStatus.PendingPayment;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    private static string GenerateOrderNumber(Guid id)
    {
        // 簡單一點：年月日 + 前 8 碼
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{id.ToString("N")[..8].ToUpperInvariant()}";
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(x => x.LineAmount);
    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.PendingPayment)
            throw new InvalidOperationException("Only pending payment orders can be marked as paid.");

        Status = OrderStatus.Paid;
        PaidAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Cancelled or OrderStatus.Refunded or OrderStatus.PartiallyRefunded)
            throw new InvalidOperationException("Order already cancelled or refunded.");

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow;
    }

    public void ApplyRefund(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        if (Status is not (OrderStatus.Paid or OrderStatus.PartiallyRefunded))
            throw new InvalidOperationException("Only paid orders can be refunded.");
        if (RefundedAmount + amount > TotalAmount)
            throw new InvalidOperationException("Refund amount exceeds paid total.");

        RefundedAmount += amount;
        RefundedAt = DateTimeOffset.UtcNow;
        Status = RefundedAmount == TotalAmount
            ? OrderStatus.Refunded
            : OrderStatus.PartiallyRefunded;
    }
}

public class OrderItem
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public Guid TimeslotId { get; private set; }

    public Guid? SeatId { get; private set; }

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal LineAmount { get; private set; }

    private OrderItem() { }

    public OrderItem(
        Guid id,
        Guid timeslotId,
        Guid? seatId,
        decimal unitPrice,
        int quantity)
    {
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        Id = id;
        TimeslotId = timeslotId;
        SeatId = seatId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        LineAmount = unitPrice * quantity;
    }
}
