using BookMyShow.Domain.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Venues;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("cities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.StateName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CountryName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.Name);
    }
}
