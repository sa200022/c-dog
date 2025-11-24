using Ticketing.Application;
using Ticketing.Domain;

namespace Ticketing.Api;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders");

        // 建立訂單（MVP = 直接 Paid）
        group.MapPost("", async (CreateOrderRequest request, OrderService service) =>
        {
            if (request.Items.Count == 0)
                return Results.BadRequest(new ApiError("Order.EmptyItems", "Order must contain at least 1 item."));

            var order = new Order(
                Guid.NewGuid(),
                request.CustomerEmail,
                request.CustomerName,
                request.Currency,
                request.Items.Select(i => new OrderItem(
                    Guid.NewGuid(),
                    i.TimeslotId,
                    i.SeatId,
                    i.UnitPrice,
                    i.Quantity
                )).ToList()
            );

            // MVP: 建立 = 直接支付成功
            order.MarkPaid();

            await service.CreateOrderAsync(order);

            var dto = new OrderDetailDto(
                order.Id,
                order.OrderNumber,
                order.CustomerEmail,
                order.CustomerName,
                order.Currency,
                order.TotalAmount,
                order.Status,
                order.CreatedAt,
                order.PaidAt,
                order.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.TimeslotId,
                    i.SeatId,
                    i.UnitPrice,
                    i.Quantity,
                    i.LineAmount
                )).ToList()
            );

            return Results.Created($"/orders/{order.Id}", dto);
        });

        // 查訂單詳細
        group.MapGet("/{orderId:guid}", async (Guid orderId, OrderService service) =>
        {
            var order = await service.GetDetailAsync(orderId);
            if (order is null)
                return Results.NotFound(new ApiError("Order.NotFound", "Order not found."));

            var dto = new OrderDetailDto(
                order.Id,
                order.OrderNumber,
                order.CustomerEmail,
                order.CustomerName,
                order.Currency,
                order.TotalAmount,
                order.Status,
                order.CreatedAt,
                order.PaidAt,
                order.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.TimeslotId,
                    i.SeatId,
                    i.UnitPrice,
                    i.Quantity,
                    i.LineAmount
                )).ToList()
            );

            return Results.Ok(dto);
        });

        // 取消訂單
        group.MapPatch("/{orderId:guid}/cancel", async (Guid orderId, OrderService service) =>
        {
            var order = await service.GetDetailAsync(orderId);
            if (order is null)
                return Results.NotFound(new ApiError("Order.NotFound", "Order not found."));

            order.Cancel();
            await service.CancelAsync(order);

            return Results.NoContent();
        });

        return app;
    }
}


// ---- DTOs ----

public sealed record CreateOrderRequest(
    string CustomerEmail,
    string CustomerName,
    string Currency,
    List<CreateOrderItemRequest> Items
);

public sealed record CreateOrderItemRequest(
    Guid TimeslotId,
    Guid? SeatId,
    decimal UnitPrice,
    int Quantity
);

public sealed record OrderDetailDto(
    Guid Id,
    string OrderNumber,
    string CustomerEmail,
    string CustomerName,
    string Currency,
    decimal TotalAmount,
    OrderStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt,
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
