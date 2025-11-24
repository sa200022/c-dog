using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain;

namespace Ticketing.Infrastructure.Configurations;

public class RefundConfig : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("Refunds");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.Reason).HasMaxLength(500).IsRequired();

        builder.Property(x => x.Status).IsRequired();

        builder.Property(x => x.RequestedAt).IsRequired();
        builder.Property(x => x.ProcessedAt);
    }
}
