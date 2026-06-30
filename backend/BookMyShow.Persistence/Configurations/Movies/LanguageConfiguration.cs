using BookMyShow.Domain.Entities.Movies;
using BookMyShow.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Movies;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("languages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasData(
            new Language { Id = SeedDataIds.Languages.English, Name = "English" },
            new Language { Id = SeedDataIds.Languages.Hindi, Name = "Hindi" },
            new Language { Id = SeedDataIds.Languages.Tamil, Name = "Tamil" },
            new Language { Id = SeedDataIds.Languages.Telugu, Name = "Telugu" },
            new Language { Id = SeedDataIds.Languages.Malayalam, Name = "Malayalam" });
    }
}

