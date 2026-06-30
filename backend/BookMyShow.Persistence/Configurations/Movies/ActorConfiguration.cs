using BookMyShow.Domain.Entities.Movies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Movies;

public class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.ToTable("actors");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ProfileImageUrl)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.FullName);
    }
}
