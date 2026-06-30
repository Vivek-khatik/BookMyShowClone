using BookMyShow.Domain.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Venues;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.AddressLine1)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.AddressLine2)
            .HasMaxLength(500);

        builder.Property(x => x.Latitude)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .IsRequired();

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.CityId);

        builder.HasOne(x => x.City)
            .WithMany(x => x.Venues)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
