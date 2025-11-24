using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain;

namespace Ticketing.Infrastructure.Configurations;

public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TimeslotId)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.LineAmount)
            .HasColumnType("numeric(18,2)");
    }
}
