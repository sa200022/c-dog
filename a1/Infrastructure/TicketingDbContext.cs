using Microsoft.EntityFrameworkCore;
using Ticketing.Domain;
using Ticketing.Infrastructure.Configurations;

namespace Ticketing.Infrastructure;

public class TicketingDbContext : DbContext
{
    public TicketingDbContext(DbContextOptions<TicketingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Timeslot> Timeslots => Set<Timeslot>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Refund> Refunds => Set<Refund>();
    public DbSet<ItineraryNotification> ItineraryNotifications => Set<ItineraryNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ActivityConfig());
        modelBuilder.ApplyConfiguration(new TimeslotConfig());
        modelBuilder.ApplyConfiguration(new SeatConfig());
        modelBuilder.ApplyConfiguration(new OrderConfig());
        modelBuilder.ApplyConfiguration(new OrderItemConfig());
        modelBuilder.ApplyConfiguration(new RefundConfig());
        modelBuilder.ApplyConfiguration(new ItineraryNotificationConfig());
    }
}
