using BookMyShow.Domain.Entities.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Bookings;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BookingReference)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TotalAmount)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.BookingReference)
            .IsUnique();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ShowId);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Show)
            .WithMany()
            .HasForeignKey(x => x.ShowId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
