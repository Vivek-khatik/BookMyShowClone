using BookMyShow.Domain.Entities.Movies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Movies;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("movies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(x => x.DurationMinutes)
            .IsRequired();

        builder.Property(x => x.ReleaseDate)
            .IsRequired();

        builder.Property(x => x.PosterUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.BannerUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.TrailerUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.CensorRating)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.Title);
        builder.HasIndex(x => x.ReleaseDate);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.GenreId);
        builder.HasIndex(x => x.LanguageId);

        builder.HasOne(x => x.Genre)
            .WithMany(x => x.Movies)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Language)
            .WithMany(x => x.Movies)
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
