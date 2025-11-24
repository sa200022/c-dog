using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain;

namespace Ticketing.Infrastructure.Configurations;

public class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.UserId);
        builder.Property(x => x.CustomerEmail).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();

        builder.Property(x => x.ActivityId).IsRequired();
        builder.Property(x => x.TimeslotId).IsRequired();

        builder.Property(x => x.TotalAmount)
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.RefundedAmount)
            .HasColumnType("numeric(18,2)")
            .HasDefaultValue(0m);

        builder.Property(x => x.Currency)
            .HasMaxLength(10)
            .HasDefaultValue("TWD");

        builder.Property(x => x.Status).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.PaidAt);
        builder.Property(x => x.CancelledAt);
        builder.Property(x => x.RefundedAt);

        builder.HasMany(x => x.Items)
               .WithOne(x => x.Order)
               .HasForeignKey(x => x.OrderId);

        builder.HasIndex(x => x.OrderNumber).IsUnique();
    }
}
