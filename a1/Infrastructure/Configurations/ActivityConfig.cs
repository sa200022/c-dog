using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain;

namespace Ticketing.Infrastructure.Configurations;

public class ActivityConfig : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("Activities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Location).HasMaxLength(200).IsRequired();

        builder.Property(x => x.MinPrice).HasColumnType("numeric(18,2)");
        builder.Property(x => x.MaxPrice).HasColumnType("numeric(18,2)");

        builder.Property(x => x.Status).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasMany(x => x.Timeslots)
               .WithOne(x => x.Activity)
               .HasForeignKey(x => x.ActivityId);
    }
}
