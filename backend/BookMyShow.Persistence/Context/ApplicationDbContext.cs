using BookMyShow.Domain.Entities.Bookings;
using BookMyShow.Domain.Entities.Identity;
using BookMyShow.Domain.Entities.Movies;
using BookMyShow.Domain.Entities.Shows;
using BookMyShow.Domain.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BookMyShow.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Genre> Genres => Set<Genre>();

        public DbSet<Language> Languages => Set<Language>();

        public DbSet<Actor> Actors => Set<Actor>();

        public DbSet<Movie> Movies => Set<Movie>();

        public DbSet<MovieActor> MovieActors => Set<MovieActor>();

        public DbSet<City> Cities => Set<City>();

        public DbSet<Venue> Venues => Set<Venue>();

        public DbSet<Screen> Screens => Set<Screen>();

        public DbSet<SeatCategory> SeatCategories => Set<SeatCategory>();

        public DbSet<Seat> Seats => Set<Seat>();

        public DbSet<Show> Shows => Set<Show>();

        public DbSet<ShowPrice> ShowPrices => Set<ShowPrice>();

        public DbSet<Booking> Bookings => Set<Booking>();

        public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();

        public DbSet<SeatLock> SeatLocks => Set<SeatLock>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
