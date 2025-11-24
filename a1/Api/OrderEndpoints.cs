using Microsoft.EntityFrameworkCore;
using Ticketing.Application;
using Ticketing.Domain;
using Ticketing.Infrastructure;

namespace Ticketing.Api;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders");

        // POST /orders (MVP: mark as paid immediately)
        group.MapPost("", async (
            CreateOrderRequest request,
            OrderService orderService,
            NotificationService notificationService,
            TicketingDbContext db) =>
        {
            if (request.Items.Count == 0)
                return Results.BadRequest(new ApiError("Order.EmptyItems", "Order must contain at least 1 item."));

            var items = request.Items.Select(i => new OrderItem(
                Guid.NewGuid(),
                request.TimeslotId,
                i.SeatId,
                i.UnitPrice,
                i.Quantity)).ToList();

            try
            {
                var order = await orderService.CreateOrderAsync(
                    request.ActivityId,
                    request.TimeslotId,
                    request.UserId,
                    request.CustomerEmail,
                    request.CustomerName,
                    request.Currency,
                    items,
                    markAsPaid: true);

                // Schedule itinerary notification (24h before start)
                var timeslot = await db.Timeslots.Include(t => t.Activity).FirstOrDefaultAsync(t => t.Id == request.TimeslotId);
                if (timeslot is not null)
                {
                    var scheduled = timeslot.StartTime.AddHours(-24);
                    var payload = $"Itinerary for {timeslot.Activity.Name} at {timeslot.StartTime:u}";
                    await notificationService.ScheduleAsync(order.Id, scheduled, NotificationChannel.Email, payload);
                }

                var dto = ToDto(order);
                return Results.Created($"/orders/{order.Id}", dto);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError("Order.InvalidState", ex.Message));
            }
        });

        // GET /orders/{id}
        group.MapGet("/{orderId:guid}", async (Guid orderId, OrderService service) =>
        {
            var order = await service.GetDetailAsync(orderId);
            if (order is null)
                return Results.NotFound(new ApiError("Order.NotFound", "Order not found."));

            return Results.Ok(ToDto(order));
        });

        // PATCH /orders/{id}/cancel
        group.MapPatch("/{orderId:guid}/cancel", async (Guid orderId, OrderService service) =>
        {
            var order = await service.GetDetailAsync(orderId);
            if (order is null)
                return Results.NotFound(new ApiError("Order.NotFound", "Order not found."));

            try
            {
                await service.CancelAsync(order);
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError("Order.InvalidState", ex.Message));
            }
        });

        return app;
    }

    private static OrderDetailDto ToDto(Order order)
    {
        return new OrderDetailDto(
            order.Id,
            order.ActivityId,
            order.TimeslotId,
            order.OrderNumber,
            order.UserId,
            order.CustomerEmail,
            order.CustomerName,
            order.Currency,
            order.TotalAmount,
            order.RefundedAmount,
            order.Status,
            order.CreatedAt,
            order.PaidAt,
            order.CancelledAt,
            order.RefundedAt,
            order.Items.Select(i => new OrderItemDto(
                i.Id,
                i.TimeslotId,
                i.SeatId,
                i.UnitPrice,
                i.Quantity,
                i.LineAmount
            )).ToList()
        );
    }
}


// ---- DTOs ----

public sealed record CreateOrderRequest(
    Guid ActivityId,
    Guid TimeslotId,
    string? UserId,
    string CustomerEmail,
    string CustomerName,
    string Currency,
    List<CreateOrderItemRequest> Items
);

public sealed record CreateOrderItemRequest(
    Guid? SeatId,
    decimal UnitPrice,
    int Quantity
);

public sealed record OrderDetailDto(
    Guid Id,
    Guid ActivityId,
    Guid TimeslotId,
    string OrderNumber,
    string? UserId,
    string CustomerEmail,
    string CustomerName,
    string Currency,
    decimal TotalAmount,
    decimal RefundedAmount,
    OrderStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt,
    DateTimeOffset? CancelledAt,
    DateTimeOffset? RefundedAt,
    List<OrderItemDto> Items
);

public sealed record OrderItemDto(
    Guid Id,
    Guid TimeslotId,
    Guid? SeatId,
    decimal UnitPrice,
    int Quantity,
    decimal LineAmount
);
