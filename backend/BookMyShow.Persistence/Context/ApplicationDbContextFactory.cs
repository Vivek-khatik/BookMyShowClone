using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookMyShow.Persistence.Context
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder =
            new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=bookmyshow_dev;Username=postgres;Password=abcd");

            return new ApplicationDbContext(
                optionsBuilder.Options);
        }
    }
}
