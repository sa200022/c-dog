using Ticketing.Application;
using Ticketing.Domain;

namespace Ticketing.Api;

public static class TimeslotEndpoints
{
    public static IEndpointRouteBuilder MapTimeslotEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /activities/{activityId}/timeslots?onlyOnSale=true
        app.MapGet("/activities/{activityId:guid}/timeslots", async (
            Guid activityId,
            bool? onlyOnSale,
            TimeslotService service) =>
        {
            var timeslots = await service.GetByActivityAsync(activityId);

            if (onlyOnSale == true)
            {
                timeslots = timeslots
                    .Where(t => t.Status == TimeslotStatus.OnSale)
                    .ToList();
            }

            var result = timeslots
                .Select(t => new TimeslotDto(
                    t.Id,
                    t.ActivityId,
                    t.StartTime,
                    t.EndTime,
                    t.BasePrice,
                    t.Capacity,
                    t.RemainingCapacity,
                    t.Status))
                .ToList();

            return Results.Ok(result);
        });

        return app;
    }
}

// ---- DTO ----

public sealed record TimeslotDto(
    Guid Id,
    Guid ActivityId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    decimal BasePrice,
    int? Capacity,
    int RemainingCapacity,
    TimeslotStatus Status);

