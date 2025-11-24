using Ticketing.Application;
using Ticketing.Domain;

namespace Ticketing.Api;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /orders/{orderId}/notifications
        app.MapGet("/orders/{orderId:guid}/notifications", async (
            Guid orderId,
            NotificationService service) =>
        {
            var notifications = await service.GetByOrderAsync(orderId);
            var dtos = notifications
                .Select(n => new NotificationDto(
                    n.Id,
                    n.OrderId,
                    n.ScheduledSendTime,
                    n.ActualSentTime,
                    n.Channel,
                    n.Status,
                    n.Payload))
                .ToList();

            return Results.Ok(dtos);
        });

        // POST /orders/{orderId}/notifications/resend
        app.MapPost("/orders/{orderId:guid}/notifications/resend", async (
            Guid orderId,
            ResendNotificationRequest request,
            NotificationService service) =>
        {
            var scheduled = request.ScheduledSendTime ?? DateTimeOffset.UtcNow.AddMinutes(5);
            var notification = await service.ScheduleAsync(orderId, scheduled, request.Channel, request.Payload);
            var dto = new NotificationDto(
                notification.Id,
                notification.OrderId,
                notification.ScheduledSendTime,
                notification.ActualSentTime,
                notification.Channel,
                notification.Status,
                notification.Payload);

            return Results.Created($"/notifications/{notification.Id}", dto);
        });

        return app;
    }
}

public sealed record NotificationDto(
    Guid Id,
    Guid OrderId,
    DateTimeOffset ScheduledSendTime,
    DateTimeOffset? ActualSentTime,
    NotificationChannel Channel,
    NotificationStatus Status,
    string Payload);

public sealed record ResendNotificationRequest(
    NotificationChannel Channel,
    string Payload,
    DateTimeOffset? ScheduledSendTime);
