using BookMyShow.Domain.Entities.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Bookings;

public class SeatLockConfiguration : IEntityTypeConfiguration<SeatLock>
{
    public void Configure(EntityTypeBuilder<SeatLock> builder)
    {
        builder.ToTable("seat_locks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LockedUntil)
            .IsRequired();

        builder.HasIndex(x => x.ShowId);
        builder.HasIndex(x => x.SeatId);
        builder.HasIndex(x => x.LockedUntil);
        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.Show)
            .WithMany()
            .HasForeignKey(x => x.ShowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Seat)
            .WithMany()
            .HasForeignKey(x => x.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
