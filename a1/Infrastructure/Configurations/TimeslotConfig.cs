using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain;

namespace Ticketing.Infrastructure.Configurations;

public class TimeslotConfig : IEntityTypeConfiguration<Timeslot>
{
    public void Configure(EntityTypeBuilder<Timeslot> builder)
    {
        builder.ToTable("Timeslots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BasePrice)
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.Capacity);

        builder.Property(x => x.RemainingCapacity);

        builder.Property(x => x.Status).IsRequired();

        builder.HasMany(x => x.Seats)
               .WithOne(x => x.Timeslot)
               .HasForeignKey(x => x.TimeslotId);
    }
}
