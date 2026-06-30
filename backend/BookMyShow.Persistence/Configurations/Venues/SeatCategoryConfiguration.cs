using BookMyShow.Domain.Entities.Venues;
using BookMyShow.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Venues;

public class SeatCategoryConfiguration : IEntityTypeConfiguration<SeatCategory>
{
    public void Configure(EntityTypeBuilder<SeatCategory> builder)
    {
        builder.ToTable("seat_categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasData(
            new SeatCategory { Id = SeedDataIds.SeatCategories.Vip, Name = "VIP" },
            new SeatCategory { Id = SeedDataIds.SeatCategories.Platinum, Name = "Platinum" },
            new SeatCategory { Id = SeedDataIds.SeatCategories.Gold, Name = "Gold" },
            new SeatCategory { Id = SeedDataIds.SeatCategories.Silver, Name = "Silver" });
    }
}

