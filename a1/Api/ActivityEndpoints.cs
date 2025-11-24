using Microsoft.EntityFrameworkCore;
using Ticketing.Application;
using Ticketing.Domain;

namespace Ticketing.Api;

public static class ActivityEndpoints
{
    public static IEndpointRouteBuilder MapActivityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/activities");

        // GET /activities?keyword=&category=&location=
        group.MapGet("", async (
            string? keyword,
            string? category,
            string? location,
            ActivityService service) =>
        {
            var activities = await service.GetListAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                activities = activities
                    .Where(a =>
                        a.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        (a.Description != null &&
                         a.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                activities = activities
                    .Where(a => string.Equals(a.Category, category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                activities = activities
                    .Where(a => a.Location.Contains(location, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var result = activities
                .Select(a => new ActivityListItemDto(
                    a.Id,
                    a.Name,
                    a.Category,
                    a.Location,
                    a.MinPrice,
                    a.MaxPrice,
                    a.Status))
                .ToList();

            return Results.Ok(result);
        });

        // GET /activities/{id}
        group.MapGet("/{activityId:guid}", async (Guid activityId, ActivityService service) =>
        {
            var activity = await service.GetDetailAsync(activityId);
            if (activity is null)
                return Results.NotFound(new ApiError("Activity.NotFound", "Activity not found."));

            var dto = new ActivityDetailDto(
                activity.Id,
                activity.Name,
                activity.Category,
                activity.Location,
                activity.Description,
                activity.MinPrice,
                activity.MaxPrice,
                activity.Status,
                activity.CreatedAt,
                activity.UpdatedAt);

            return Results.Ok(dto);
        });

        return app;
    }
}

// ---- DTOs ----

public sealed record ActivityListItemDto(
    Guid Id,
    string Name,
    string Category,
    string Location,
    decimal MinPrice,
    decimal MaxPrice,
    ActivityStatus Status);

public sealed record ActivityDetailDto(
    Guid Id,
    string Name,
    string Category,
    string Location,
    string? Description,
    decimal MinPrice,
    decimal MaxPrice,
    ActivityStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
