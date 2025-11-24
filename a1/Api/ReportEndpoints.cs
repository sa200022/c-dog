using Ticketing.Application;

namespace Ticketing.Api;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /reports/activity-sales?activityId=...
        app.MapGet("/reports/activity-sales", async (Guid? activityId, ReportService service) =>
        {
            var data = await service.GetActivitySalesAsync(activityId);
            return Results.Ok(data);
        });

        // GET /reports/timeslot-inventory?activityId=...
        app.MapGet("/reports/timeslot-inventory", async (Guid? activityId, ReportService service) =>
        {
            if (!activityId.HasValue)
                return Results.BadRequest(new ApiError("Report.MissingActivityId", "activityId is required."));

            var data = await service.GetTimeslotInventoryAsync(activityId.Value);
            return Results.Ok(data);
        });

        return app;
    }
}
