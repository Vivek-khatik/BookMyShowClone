using BookMyShow.Domain.Entities.Shows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Shows;

public class ShowPriceConfiguration : IEntityTypeConfiguration<ShowPrice>
{
    public void Configure(EntityTypeBuilder<ShowPrice> builder)
    {
        builder.ToTable("show_prices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.HasIndex(x => x.ShowId);
        builder.HasIndex(x => x.SeatCategoryId);

        builder.HasIndex(x => new { x.ShowId, x.SeatCategoryId })
            .IsUnique();

        builder.HasOne(x => x.Show)
            .WithMany(x => x.ShowPrices)
            .HasForeignKey(x => x.ShowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SeatCategory)
            .WithMany()
            .HasForeignKey(x => x.SeatCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
