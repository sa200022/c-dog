using System;
using System.Collections.Generic;

namespace Ticketing.Domain;

public class Timeslot
{
    public Guid Id { get; private set; }

    public Guid ActivityId { get; private set; }
    public Activity Activity { get; private set; } = null!;

    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }

    public decimal BasePrice { get; private set; }

    /// <summary>
    /// 總名額；若為座位制，可為 null，實際容量由座位數決定。
    /// </summary>
    public int? Capacity { get; private set; }

    public int RemainingCapacity { get; private set; }

    public TimeslotStatus Status { get; private set; }

    public ICollection<Seat> Seats { get; private set; } = new List<Seat>();

    private Timeslot() { }

    public Timeslot(
        Guid id,
        Guid activityId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        decimal basePrice,
        int? capacity)
    {
        if (endTime <= startTime)
            throw new ArgumentException("EndTime must be greater than StartTime.", nameof(endTime));

        Id = id;
        ActivityId = activityId;
        StartTime = startTime;
        EndTime = endTime;
        BasePrice = basePrice;
        Capacity = capacity;
        RemainingCapacity = capacity ?? 0;
        Status = TimeslotStatus.OnSale;
    }

    public void Update(
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        decimal basePrice,
        int? capacity)
    {
        if (endTime <= startTime)
            throw new ArgumentException("EndTime must be greater than StartTime.", nameof(endTime));

        StartTime = startTime;
        EndTime = endTime;
        BasePrice = basePrice;
        AdjustCapacity(capacity);
    }

    public void AdjustCapacity(int? newCapacity)
    {
        if (newCapacity.HasValue && newCapacity.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(newCapacity));

        Capacity = newCapacity;

        if (newCapacity.HasValue && RemainingCapacity > newCapacity.Value)
        {
            RemainingCapacity = newCapacity.Value;
        }
    }

    public void DecreaseCapacity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (RemainingCapacity - quantity < 0)
            throw new InvalidOperationException("RemainingCapacity cannot be negative.");

        RemainingCapacity -= quantity;
    }

    public void IncreaseCapacity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        RemainingCapacity += quantity;
    }

    public void ChangeStatus(TimeslotStatus status)
    {
        Status = status;
    }
}
