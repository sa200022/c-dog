using Microsoft.EntityFrameworkCore;
using Ticketing.Application;
using Ticketing.Domain;

namespace Ticketing.Api;

public static class ActivityEndpoints
{
    public static IEndpointRouteBuilder MapActivityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/activities");

        // GET /activities
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

        // POST /activities
        group.MapPost("", async (CreateActivityRequest request, ActivityService service) =>
        {
            if (request.MinPrice < 0 || request.MaxPrice < 0 || request.MinPrice > request.MaxPrice)
                return Results.BadRequest(new ApiError("Activity.InvalidPrice", "MinPrice/MaxPrice is invalid."));

            var activity = new Activity(
                Guid.NewGuid(),
                request.Name,
                request.Category,
                request.Location,
                request.Description,
                request.MinPrice,
                request.MaxPrice);

            await service.CreateAsync(activity);

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

            return Results.Created($"/activities/{activity.Id}", dto);
        });

        // PUT /activities/{id}
        group.MapPut("/{activityId:guid}", async (
            Guid activityId,
            UpdateActivityRequest request,
            ActivityService service) =>
        {
            var activity = await service.GetDetailAsync(activityId);
            if (activity is null)
                return Results.NotFound(new ApiError("Activity.NotFound", "Activity not found."));

            if (request.MinPrice < 0 || request.MaxPrice < 0 || request.MinPrice > request.MaxPrice)
                return Results.BadRequest(new ApiError("Activity.InvalidPrice", "MinPrice/MaxPrice is invalid."));

            activity.UpdateBasicInfo(
                request.Name,
                request.Category,
                request.Location,
                request.Description,
                request.MinPrice,
                request.MaxPrice);

            await service.UpdateAsync(activity);

            return Results.NoContent();
        });

        // PATCH /activities/{id}/status
        group.MapPatch("/{activityId:guid}/status", async (
            Guid activityId,
            ChangeActivityStatusRequest request,
            ActivityService service) =>
        {
            var activity = await service.GetDetailAsync(activityId);
            if (activity is null)
                return Results.NotFound(new ApiError("Activity.NotFound", "Activity not found."));

            await service.ChangeStatusAsync(activity, request.Status);
            return Results.NoContent();
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

public sealed record CreateActivityRequest(
    string Name,
    string Category,
    string Location,
    string? Description,
    decimal MinPrice,
    decimal MaxPrice);

public sealed record UpdateActivityRequest(
    string Name,
    string Category,
    string Location,
    string? Description,
    decimal MinPrice,
    decimal MaxPrice);

public sealed record ChangeActivityStatusRequest(ActivityStatus Status);
