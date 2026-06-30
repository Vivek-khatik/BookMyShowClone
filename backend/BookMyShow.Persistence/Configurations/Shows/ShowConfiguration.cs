using BookMyShow.Domain.Entities.Shows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Shows;

public class ShowConfiguration : IEntityTypeConfiguration<Show>
{
    public void Configure(EntityTypeBuilder<Show> builder)
    {
        builder.ToTable("shows");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.MovieId);
        builder.HasIndex(x => x.ScreenId);
        builder.HasIndex(x => x.StartTime);

        builder.HasOne(x => x.Movie)
            .WithMany()
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Screen)
            .WithMany()
            .HasForeignKey(x => x.ScreenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
