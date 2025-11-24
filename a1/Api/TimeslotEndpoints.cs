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

        // POST /activities/{activityId}/timeslots
        app.MapPost("/activities/{activityId:guid}/timeslots", async (
            Guid activityId,
            CreateTimeslotRequest request,
            TimeslotService service) =>
        {
            if (request.BasePrice < 0)
                return Results.BadRequest(new ApiError("Timeslot.InvalidPrice", "BasePrice must be >= 0."));

            var timeslot = new Timeslot(
                Guid.NewGuid(),
                activityId,
                request.StartTime,
                request.EndTime,
                request.BasePrice,
                request.Capacity);

            await service.CreateAsync(timeslot);

            var dto = new TimeslotDto(
                timeslot.Id,
                timeslot.ActivityId,
                timeslot.StartTime,
                timeslot.EndTime,
                timeslot.BasePrice,
                timeslot.Capacity,
                timeslot.RemainingCapacity,
                timeslot.Status);

            return Results.Created($"/timeslots/{timeslot.Id}", dto);
        });

        // PUT /timeslots/{timeslotId}
        app.MapPut("/timeslots/{timeslotId:guid}", async (
            Guid timeslotId,
            UpdateTimeslotRequest request,
            TimeslotService service) =>
        {
            var timeslot = await service.GetByIdAsync(timeslotId);
            if (timeslot is null)
                return Results.NotFound(new ApiError("Timeslot.NotFound", "Timeslot not found."));

            if (request.BasePrice < 0)
                return Results.BadRequest(new ApiError("Timeslot.InvalidPrice", "BasePrice must be >= 0."));

            timeslot.Update(
                request.StartTime,
                request.EndTime,
                request.BasePrice,
                request.Capacity);

            await service.UpdateAsync(timeslot);

            return Results.NoContent();
        });

        // PATCH /timeslots/{timeslotId}/status
        app.MapPatch("/timeslots/{timeslotId:guid}/status", async (
            Guid timeslotId,
            ChangeTimeslotStatusRequest request,
            TimeslotService service) =>
        {
            var timeslot = await service.GetByIdAsync(timeslotId);
            if (timeslot is null)
                return Results.NotFound(new ApiError("Timeslot.NotFound", "Timeslot not found."));

            await service.ChangeStatusAsync(timeslot, request.Status);
            return Results.NoContent();
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

public sealed record CreateTimeslotRequest(
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    decimal BasePrice,
    int? Capacity);

public sealed record UpdateTimeslotRequest(
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    decimal BasePrice,
    int? Capacity);

public sealed record ChangeTimeslotStatusRequest(TimeslotStatus Status);
