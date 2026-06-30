using BookMyShow.Domain.Entities.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Bookings;

public class BookingSeatConfiguration : IEntityTypeConfiguration<BookingSeat>
{
    public void Configure(EntityTypeBuilder<BookingSeat> builder)
    {
        builder.ToTable("booking_seats");

        builder.HasKey(x => new { x.BookingId, x.SeatId });

        builder.HasIndex(x => x.SeatId);

        builder.HasOne(x => x.Booking)
            .WithMany(x => x.BookingSeats)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Seat)
            .WithMany()
            .HasForeignKey(x => x.SeatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
