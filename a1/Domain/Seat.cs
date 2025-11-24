using System;

namespace Ticketing.Domain;

public class Seat
{
    public Guid Id { get; private set; }

    public Guid TimeslotId { get; private set; }
    public Timeslot Timeslot { get; private set; } = null!;

    public string Area { get; private set; } = null!;
    public string Row { get; private set; } = null!;
    public string Number { get; private set; } = null!;

    /// <summary>
    /// 若為 null 則使用 Timeslot.BasePrice。
    /// </summary>
    public decimal? PriceOverride { get; private set; }

    public SeatStatus Status { get; private set; }

    private Seat() { }

    public Seat(
        Guid id,
        Guid timeslotId,
        string area,
        string row,
        string number,
        decimal? priceOverride)
    {
        Id = id;
        TimeslotId = timeslotId;
        Area = area;
        Row = row;
        Number = number;
        PriceOverride = priceOverride;
        Status = SeatStatus.Available;
    }

    public string SeatLabel => $"{Area}-{Row}-{Number}";

    public void MarkSold()
    {
        if (Status == SeatStatus.Sold)
            return;

        if (Status is SeatStatus.Locked or SeatStatus.Reserved or SeatStatus.Available)
            Status = SeatStatus.Sold;
        else
            throw new InvalidOperationException($"Cannot mark seat as sold from status {Status}.");
    }

    public void Release()
    {
        Status = SeatStatus.Available;
    }
}
