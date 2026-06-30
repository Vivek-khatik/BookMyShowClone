using BookMyShow.Domain.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Venues;

public class ScreenConfiguration : IEntityTypeConfiguration<Screen>
{
    public void Configure(EntityTypeBuilder<Screen> builder)
    {
        builder.ToTable("screens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Capacity)
            .IsRequired();

        builder.HasIndex(x => x.VenueId);

        builder.HasOne(x => x.Venue)
            .WithMany(x => x.Screens)
            .HasForeignKey(x => x.VenueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
