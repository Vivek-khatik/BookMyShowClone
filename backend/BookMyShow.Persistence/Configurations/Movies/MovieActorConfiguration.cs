using BookMyShow.Domain.Entities.Movies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Movies;

public class MovieActorConfiguration : IEntityTypeConfiguration<MovieActor>
{
    public void Configure(EntityTypeBuilder<MovieActor> builder)
    {
        builder.ToTable("movie_actors");

        builder.HasKey(x => new { x.MovieId, x.ActorId });

        builder.HasOne(x => x.Movie)
            .WithMany(x => x.MovieActors)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Actor)
            .WithMany(x => x.MovieActors)
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ActorId);
    }
}
