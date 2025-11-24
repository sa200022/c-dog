using Ticketing.Application;
using Ticketing.Domain;

namespace Ticketing.Api;

public static class SeatEndpoints
{
    public static IEndpointRouteBuilder MapSeatEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/timeslots/{timeslotId:guid}/seats", async (
            Guid timeslotId,
            bool? onlyAvailable,
            SeatService service) =>
        {
            var seats = await service.GetByTimeslotAsync(timeslotId);

            if (onlyAvailable == true)
            {
                seats = seats
                    .Where(s => s.Status == SeatStatus.Available)
                    .ToList();
            }

            var result = seats
                .Select(s => new SeatDto(
                    s.Id,
                    s.TimeslotId,
                    s.Area,
                    s.Row,
                    s.Number,
                    s.SeatLabel,
                    s.PriceOverride,
                    s.Status))
                .ToList();

            return Results.Ok(result);
        });

        return app;
    }
}

public sealed record SeatDto(
    Guid Id,
    Guid TimeslotId,
    string Area,
    string Row,
    string Number,
    string SeatLabel,
    decimal? PriceOverride,
    SeatStatus Status);
