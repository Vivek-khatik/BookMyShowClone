using BookMyShow.Domain.Entities.Movies;
using BookMyShow.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Movies;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("genres");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasData(
            new Genre { Id = SeedDataIds.Genres.Action, Name = "Action" },
            new Genre { Id = SeedDataIds.Genres.Comedy, Name = "Comedy" },
            new Genre { Id = SeedDataIds.Genres.Drama, Name = "Drama" },
            new Genre { Id = SeedDataIds.Genres.Horror, Name = "Horror" },
            new Genre { Id = SeedDataIds.Genres.Thriller, Name = "Thriller" },
            new Genre { Id = SeedDataIds.Genres.SciFi, Name = "Sci-Fi" },
            new Genre { Id = SeedDataIds.Genres.Romance, Name = "Romance" });
    }
}

