using BookMyShow.Domain.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Venues;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("seats");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RowName)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.SeatNumber)
            .IsRequired();

        builder.Property(x => x.ScreenId)
            .IsRequired();

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.HasIndex(x => new { x.ScreenId, x.RowName, x.SeatNumber })
            .IsUnique();

        builder.HasOne(x => x.Screen)
            .WithMany(x => x.Seats)
            .HasForeignKey(x => x.ScreenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Seats)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
