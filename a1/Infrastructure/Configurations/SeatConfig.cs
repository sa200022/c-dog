using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain;

namespace Ticketing.Infrastructure.Configurations;

public class SeatConfig : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("Seats");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Area).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Row).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Number).HasMaxLength(50).IsRequired();

        builder.Property(x => x.PriceOverride)
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.Status).IsRequired();
    }
}
