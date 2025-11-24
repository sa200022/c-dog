using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain;

namespace Ticketing.Infrastructure.Configurations;

public class ItineraryNotificationConfig : IEntityTypeConfiguration<ItineraryNotification>
{
    public void Configure(EntityTypeBuilder<ItineraryNotification> builder)
    {
        builder.ToTable("ItineraryNotifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ScheduledSendTime).IsRequired();
        builder.Property(x => x.ActualSentTime);

        builder.Property(x => x.Channel).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.Property(x => x.Payload).HasColumnType("text").IsRequired();
    }
}
