using BookMyShow.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasIndex(x => x.PhoneNumber)
            .IsUnique();

        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}