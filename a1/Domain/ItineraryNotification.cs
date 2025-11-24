using System;

namespace Ticketing.Domain;

public class ItineraryNotification
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public DateTimeOffset ScheduledSendTime { get; private set; }
    public DateTimeOffset? ActualSentTime { get; private set; }

    public NotificationChannel Channel { get; private set; }
    public NotificationStatus Status { get; private set; }

    /// <summary>
    /// 要寄出的內容快照（JSON）
    /// </summary>
    public string Payload { get; private set; } = null!;

    private ItineraryNotification() { }

    public ItineraryNotification(
        Guid id,
        Guid orderId,
        DateTimeOffset scheduledSendTime,
        NotificationChannel channel,
        string payload)
    {
        Id = id;
        OrderId = orderId;
        ScheduledSendTime = scheduledSendTime;
        Channel = channel;
        Payload = payload;
        Status = NotificationStatus.Pending;
    }

    public void MarkSent()
    {
        Status = NotificationStatus.Sent;
        ActualSentTime = DateTimeOffset.UtcNow;
    }

    public void MarkFailed()
    {
        Status = NotificationStatus.Failed;
        ActualSentTime = DateTimeOffset.UtcNow;
    }
}
