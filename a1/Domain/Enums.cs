namespace Ticketing.Domain;

public enum ActivityStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2
}

public enum TimeslotStatus
{
    OnSale = 0,
    SoldOut = 1,
    Closed = 2
}

public enum SeatStatus
{
    Available = 0,
    Reserved = 1,
    Sold = 2,
    Locked = 3
}

public enum OrderStatus
{
    PendingPayment = 0,
    Paid = 1,
    Cancelled = 2,
    Refunded = 3,
    PartiallyRefunded = 4
}

public enum RefundStatus
{
    Requested = 0,
    Approved = 1,
    Rejected = 2,
    Completed = 3
}

public enum NotificationChannel
{
    Email = 0,
    Sms = 1,
    Other = 2
}

public enum NotificationStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2
}
