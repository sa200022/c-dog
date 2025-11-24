using System;
using System.Collections.Generic;

namespace Ticketing.Domain;

public class Activity
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public string Location { get; private set; } = null!;
    public string? Description { get; private set; }

    public decimal MinPrice { get; private set; }
    public decimal MaxPrice { get; private set; }

    public ActivityStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public ICollection<Timeslot> Timeslots { get; private set; } = new List<Timeslot>();

    private Activity() { }

    public Activity(
        Guid id,
        string name,
        string category,
        string location,
        string? description,
        decimal minPrice,
        decimal maxPrice)
    {
        Id = id;
        Name = name;
        Category = category;
        Location = location;
        Description = description;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        Status = ActivityStatus.Draft;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Publish()
    {
        Status = ActivityStatus.Published;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Archive()
    {
        Status = ActivityStatus.Archived;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateBasicInfo(
        string name,
        string category,
        string location,
        string? description,
        decimal minPrice,
        decimal maxPrice)
    {
        Name = name;
        Category = category;
        Location = location;
        Description = description;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
