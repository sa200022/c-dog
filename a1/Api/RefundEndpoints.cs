using Ticketing.Application;
using Ticketing.Domain;

namespace Ticketing.Api;

public static class RefundEndpoints
{
    public static IEndpointRouteBuilder MapRefundEndpoints(this IEndpointRouteBuilder app)
    {
        // POST /orders/{orderId}/refunds
        app.MapPost("/orders/{orderId:guid}/refunds", async (
            Guid orderId,
            CreateRefundRequest request,
            OrderService orderService,
            RefundService refundService) =>
        {
            var order = await orderService.GetDetailAsync(orderId);
            if (order is null)
                return Results.NotFound(new ApiError("Order.NotFound", "Order not found."));

            try
            {
                var refund = await refundService.RequestAsync(order, request.Amount, request.Reason);
                return Results.Created($"/refunds/{refund.Id}", ToDto(refund));
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError("Refund.Invalid", ex.Message));
            }
        });

        // GET /refunds/{refundId}
        app.MapGet("/refunds/{refundId:guid}", async (Guid refundId, RefundService service) =>
        {
            var refund = await service.GetByIdAsync(refundId);
            if (refund is null)
                return Results.NotFound(new ApiError("Refund.NotFound", "Refund not found."));

            return Results.Ok(ToDto(refund));
        });

        // GET /refunds
        app.MapGet("/refunds", async (RefundService service) =>
        {
            var refunds = await service.GetAllAsync();
            return Results.Ok(refunds.Select(ToDto).ToList());
        });

        // POST /refunds/{refundId}/approve
        app.MapPost("/refunds/{refundId:guid}/approve", async (Guid refundId, RefundService service) =>
        {
            var refund = await service.GetByIdAsync(refundId);
            if (refund is null)
                return Results.NotFound(new ApiError("Refund.NotFound", "Refund not found."));

            try
            {
                await service.ApproveAsync(refund);
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError("Refund.Invalid", ex.Message));
            }
        });

        // POST /refunds/{refundId}/reject
        app.MapPost("/refunds/{refundId:guid}/reject", async (Guid refundId, RefundService service) =>
        {
            var refund = await service.GetByIdAsync(refundId);
            if (refund is null)
                return Results.NotFound(new ApiError("Refund.NotFound", "Refund not found."));

            try
            {
                await service.RejectAsync(refund);
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError("Refund.Invalid", ex.Message));
            }
        });

        // POST /refunds/{refundId}/complete
        app.MapPost("/refunds/{refundId:guid}/complete", async (Guid refundId, RefundService service) =>
        {
            var refund = await service.GetByIdAsync(refundId);
            if (refund is null)
                return Results.NotFound(new ApiError("Refund.NotFound", "Refund not found."));

            try
            {
                await service.CompleteAsync(refund);
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError("Refund.Invalid", ex.Message));
            }
        });

        return app;
    }

    private static RefundDto ToDto(Refund refund)
    {
        return new RefundDto(
            refund.Id,
            refund.OrderId,
            refund.Amount,
            refund.Reason,
            refund.Status,
            refund.RequestedAt,
            refund.ProcessedAt);
    }
}

public sealed record CreateRefundRequest(decimal Amount, string Reason);

public sealed record RefundDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    string Reason,
    RefundStatus Status,
    DateTimeOffset RequestedAt,
    DateTimeOffset? ProcessedAt);
