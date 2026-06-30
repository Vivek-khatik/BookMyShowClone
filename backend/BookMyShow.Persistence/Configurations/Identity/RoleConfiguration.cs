using BookMyShow.Domain.Entities.Identity;
using BookMyShow.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyShow.Persistence.Configurations.Identity;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId);

        builder.HasData(
            new Role
            {
                Id = SeedDataIds.Roles.SuperAdmin,
                Name = "SuperAdmin"
            },
            new Role
            {
                Id = SeedDataIds.Roles.VenueAdmin,
                Name = "VenueAdmin"
            },
            new Role
            {
                Id = SeedDataIds.Roles.Organizer,
                Name = "Organizer"
            },
            new Role
            {
                Id = SeedDataIds.Roles.Customer,
                Name = "Customer"
            });
    }
}
